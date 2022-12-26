using DataAggregator.Core.Filter;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Stat;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;
using DataAggregator.Domain.Model.DrugClassifier.Systematization.View;
using DataAggregator.Web.Models.Systematization;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace DataAggregator.Web.Controllers.Systematization
{
    [Authorize(Roles = "SBoss, SOperator, SPharmacist")]
    public class SystematizationController : BaseController
    {
        //private static readonly object LockDrugsData = new Object();
        /// <summary>
        /// Получить новые DrugClear для обработки
        /// </summary>
        [HttpPost]
        public JsonResult GetDrugs(DrugFilterJson drugFilterParameters, int rettype)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    context.Database.CommandTimeout = 0;

                    int userDrugsCount = context.DrugClassifierInWork.Count(dcw => dcw.UserId == userGuid);
                    if (userDrugsCount > 0)
                        throw new ApplicationException("У пользователя не должно быть данных в работе");

                    var us = context.UserSource.Where(w => w.UserId == userGuid).FirstOrDefault();
                    if (us == null)
                        throw new ArgumentNullException("Нет данных о блоке обработки");
                
                    var drugFilter = new DrugFilter(APP, us.SourceId, us.PeriodId)
                    {
                        Count = drugFilterParameters.Count,
                        RobotStat = drugFilterParameters.RobotStat,
                        DateStat = drugFilterParameters.DateStat,
                        DrugClearWorkStat = drugFilterParameters.DrugClearWorkStat,
                        DataTypeStat = drugFilterParameters.DataTypeStat,
                        UserWorkStat = drugFilterParameters.UserWorkStat,
                        CategoryStat = drugFilterParameters.CategoryStat,
                        PrioritetStat = drugFilterParameters.PrioritetStat,
                        Additional = drugFilterParameters.Additional
                    };

                    drugFilter.Count = GetLimitedCount(drugFilter.Count, userGuid);

                    //новый забор без сервиса
                    string drugFilter_string = "";
                    //if (rettype == 0)
                    //    drugFilter_string = drugFilter.GetFilter(userGuid);

                    if (rettype == 1)
                        drugFilter_string = drugFilter.GetFilter_v2(userGuid);

                    if (string.IsNullOrEmpty(drugFilter_string))
                    {
                        throw new ArgumentNullException("Задайте корректные значения для фильтров (drugFilter)");
                    }
               
                    context.GetDrugs(drugFilter_string, userGuid, drugFilterParameters.Count);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                
            }
            return LoadDrugs();
        }

        // Для SOperator в части GZ только 50 записей (задача 4674)
        // Для остальных не более 100000
        private int GetLimitedCount(int count, Guid userGuid)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var userSource = context.UserSource.FirstOrDefault(us => us.UserId == userGuid);
                if (userSource == null)
                    throw new ApplicationException("Текущего пользователя нет в системе обработки информации");

                var userIdentity = (ClaimsIdentity)User.Identity;
                var claims = userIdentity.Claims;
                var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();

                if (userSource.PeriodId == 2 && //GZ
                    roles.Count == 1 && roles[0].Value.Equals("SOperator")
                    && count > 50)
                {
                    return 50;
                }

                if (count > 100000) 
                    return 100000;

                return count;
            }
        }

        /// <summary>
        /// Вернуть данные - применить изменения
        /// </summary>
        [HttpPost]
        public JsonResult SetDrugs()
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            // TODO: зачем?
            using (var context = new DrugClassifierContext(APP))
            {
                context.DrugClassifierInWork.Where(dcw => dcw.UserId == userGuid).ForEach(dcw => dcw.HasChanges = null);

                //lock (LockDrugsData)
                //{
                context.SetDrugs(userGuid);
                //}
            }
            return Json(true);
        }

        /// <summary>
        /// Загрузить обрабатываемые DrugClear (например после забора данных пользователем или при открытии приложения, если в работе уже есть данные)
        /// </summary>
        [HttpPost]
        public JsonResult LoadDrugs()
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 3600;

                List<DrugInWorkView> drugs = context.DrugInWorkView
                    .Where(d => d.UserId == userGuid)
                    .OrderBy(d => d.DrugClearText)
                    .ToList();

                //выбираем все DrugId из списка
                var drugIds = drugs.Where(x => x.DrugId != null).Select(x => x.DrugId.Value).ToList();
                //выбираем все DrugClassifier по не пустым DrugId и пустым ClassifierId
                List<long> dcs = context.DrugClassifier
                    .Where(x => x.DrugId != null && x.ClassifierId == null && drugIds.Select(y => y)
                    .Contains(x.DrugId.Value))
                    .Select(x => x.DrugId.Value)
                    .ToList();

                //если есть такие записи
                if (dcs.Count > 0)
                {
                    foreach (var dcw in drugs)
                    {
                        foreach (var dc in dcs)
                        {
                            if (dcw.DrugId.HasValue && dcw.DrugId == dc)
                                dcw.HasEmptyClassfierId = true;
                        }
                    }
                }

                JsonResult jsonResult = Json(drugs, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
        }
        [HttpPost]
        public JsonResult DrugGoodInit()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var res = context.GoodsCategory
                    .OrderBy(o => o.GoodsSection.Name)
                    .Select(s => new GoodsSection_view() { 
                        Id = (byte)s.Id, 
                        Name = s.Name, 
                        MiniName = s.MiniName, 
                        Section = s.GoodsSection.Name 
                    })
                    .ToList();

                JsonResult jsonResult = Json(res, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
        }
        public class GoodsSection_view
        {
            public byte Id { get; set; }
            public string Name { get; set; }
            public string MiniName { get; set; }
            public string Section { get; set; }
        }
        /// <summary>
        /// Получить наименование обрабатываемого периода
        /// </summary>
        [HttpPost]
        public JsonResult GetPeriodName()
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                UserSource userSource = context.UserSource.SingleOrDefault(us => us.UserId == userGuid);

                return Json(userSource != null ? userSource.Period.Name : "Нет");
            }
        }

        [HttpPost]
        public JsonResult ForIsError(List<long> drugClassifierInWork, bool value)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                List<DrugClassifierInWork> drugsInWork = context.DrugClassifierInWork
                                                                .Where(dcw => dcw.UserId == userGuid)
                                                                .ToList();

                foreach (long id in drugClassifierInWork)
                {
                    DrugClassifierInWork currentDrugInWork = drugsInWork.FirstOrDefault(dcw => dcw.Id == id);

                    if (currentDrugInWork == null)
                        throw new ApplicationException("drug not found");

                    currentDrugInWork.IsError = value;
                    currentDrugInWork.HasChanges = true;
                }

                context.SaveChanges();
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult SetSuperCheck(List<long> drugClassifierInWork, bool value)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                List<DrugClassifierInWork> drugsInWork = context.DrugClassifierInWork.Where(dcw => dcw.UserId == userGuid).ToList();

                foreach (long id in drugClassifierInWork)
                {
                    DrugClassifierInWork currentDrugInWork = drugsInWork.FirstOrDefault(dcw => dcw.Id == id);

                    if (currentDrugInWork == null)
                        throw new ApplicationException("drug not found");

                    currentDrugInWork.SuperCheck = value;
                    currentDrugInWork.HasChanges = true;
                }

                context.SaveChanges();
            }

            return Json(true);
        }




        [HttpPost]
        public JsonResult ForChecking(List<long> drugClassifierInWork, bool value)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                List<DrugClassifierInWork> drugsInWork = context.DrugClassifierInWork.Where(dcw => dcw.UserId == userGuid).ToList();

                foreach (long id in drugClassifierInWork)
                {
                    DrugClassifierInWork currentDrugInWork = drugsInWork.FirstOrDefault(dcw => dcw.Id == id);

                    if (currentDrugInWork == null)
                        throw new ApplicationException("drug not found");

                    currentDrugInWork.ForChecking = value;

                    if (value)
                    {
                        currentDrugInWork.ForAdding = false;
                        currentDrugInWork.ClearDrugId();
                    }
                    currentDrugInWork.HasChanges = true;
                }

                context.SaveChanges();
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult ForAdding(List<long> drugClassifierInWork, bool value)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                List<DrugClassifierInWork> drugsInWork = context.DrugClassifierInWork.Where(dcw => dcw.UserId == userGuid).ToList();

                foreach (long id in drugClassifierInWork)
                {
                    DrugClassifierInWork currentDrugInWork = drugsInWork.FirstOrDefault(dcw => dcw.Id == id);

                    if (currentDrugInWork == null)
                        throw new ApplicationException("drug not found");

                    currentDrugInWork.ForAdding = value;

                    if (value)
                    {
                        currentDrugInWork.ForChecking = false;
                        currentDrugInWork.ClearDrugId();
                    }

                    currentDrugInWork.HasChanges = true;
                }

                context.SaveChanges();
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult ForIsOther(List<long> drugClassifierInWork, bool value, byte? GoodsCategoryId)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                List<DrugClassifierInWork> drugsInWork = context.DrugClassifierInWork.Where(dcw => dcw.UserId == userGuid).ToList();

                foreach (long id in drugClassifierInWork)
                {
                    DrugClassifierInWork currentDrugInWork = drugsInWork.FirstOrDefault(dcw => dcw.Id == id);

                    if (currentDrugInWork == null)
                        throw new ApplicationException("drug not found");

                    currentDrugInWork.IsOther = value;

                    if (value)
                    {
                        currentDrugInWork.GoodsCategoryId = GoodsCategoryId;
                        currentDrugInWork.ForAdding = false;
                        currentDrugInWork.ForChecking = false;
                    }
                    else
                    {
                        currentDrugInWork.GoodsCategoryId = null;
                    }
                    //4.	При обработке ЛС есть возможность проставить строчки из ДОП. При этом проставляется значок ДОП («прочее»), проставляется goodsid и категория. 
                    //Затем мы решили, что ошиблись, нажимаем кнопку «прочее» снять – он прочее снимает, а привязку к goodsid оставляет и категорию. 
                    //Было бы хорошо, чтобы сразу и привязка снималась.
                    currentDrugInWork.ClearDrugId();
                    currentDrugInWork.HasChanges = true;
                }
                context.SaveChanges();
            }

            return Json(true);
        }


        [HttpPost]
        public JsonResult PrioritetWords_isControl(List<long> drugClassifierInWork, int value)
        {
            if (value == 10)
            {
                var userGuid = new Guid(User.Identity.GetUserId());

                using (var context = new DrugClassifierContext(APP))
                {
                    var upd = context.PrioritetDrugClassifier.Where(w => drugClassifierInWork.Contains(w.DrugClassifierId));

                    foreach (var item in upd)
                    {
                        item.isControl = true;
                    }
                    context.SaveChanges();
                }
            }
            return Json(true);
        }


        [HttpPost]
        public JsonResult SetPromo(long drugClassifierInWorkId, string promoValue)
        {
            try
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    var drugClassInWork = context.DrugClassifierInWork.SingleOrDefault(dcw => dcw.UserId == userGuid && dcw.Id == drugClassifierInWorkId);

                    if (drugClassInWork == null)
                    {
                        return BadRequest("В таблице DrugClassifierInWork не найдена запись с Id=" + drugClassifierInWorkId);
                    }

                    drugClassInWork.Promo = String.IsNullOrEmpty(promoValue) ? null : promoValue;
                    context.SaveChanges();
                }

                return Json(true);
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public JsonResult SetFlags(long drugClassifierInWorkId, string FlagsValue)
        {
            try
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    var drugClassInWork = context.DrugClassifierInWork.SingleOrDefault(dcw => dcw.UserId == userGuid && dcw.Id == drugClassifierInWorkId);

                    if (drugClassInWork == null)
                    {
                        return BadRequest("В таблице DrugClassifierInWork не найдена запись с Id=" + drugClassifierInWorkId);
                    }

                    drugClassInWork.Flags = String.IsNullOrEmpty(FlagsValue) ? null : FlagsValue;
                    context.SaveChanges();
                }

                return Json(true);
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "SBoss")]
        public JsonResult UpdateStatistic()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 3600;
                context.Database.ExecuteSqlCommand("exec Stat.UpdateStat @PeriodId=" + Convert.ToString(aspUser.PeriodId));

                return Json(GetDrugFilterStatistic(context));
            }
        }

        [HttpPost]
        public JsonResult GetDrugFilterStatistic()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                return Json(GetDrugFilterStatistic(context));
            }
        }

        public DrugFilterJson GetDrugFilterStatistic(DrugClassifierContext context)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            UserSource currentUser = context.UserSource.Single(s => s.UserId == userGuid);

            ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 3600;

            List<UserWorkStat> userWorkStat = context.UserWorkStats.Where(s => s.PeriodId == currentUser.PeriodId).OrderBy(o => o.FullName).ToList();
            List<DataTypeStat> dataTypeStat = context.DataTypeStats.Where(s => s.PeriodId == currentUser.PeriodId).OrderBy(d => d.OrderId).ToList();
            DrugClearWorkStat drugClearWorkStat = context.DrugClearWorkStats.FirstOrDefault(s => s.PeriodId == currentUser.PeriodId);
            List<RobotStat> robotStat = context.RobotStat.Where(s => s.PeriodId == currentUser.PeriodId).ToList();
            List<DateStat> dateStat = context.DateStat.Where(s => s.PeriodId == currentUser.PeriodId).OrderBy(o => o.date).ToList();
            List<CategoryStatDrugView> CategoryStat = context.CategoryStatDrugView.Where(s => s.PeriodId == currentUser.PeriodId).OrderBy(o => o.SectionName).ThenBy(o2 => o2.CategoryName).ToList();
            var PrioritetStat = context.PrioritetStat.Where(s => s.PeriodId == currentUser.PeriodId).OrderBy(o => o.RuleName).ThenBy(o1 => o1.isReady).ThenBy(o2 => o2.IsOther).ToList();
            return new DrugFilterJson
            {
                UserWorkStat = userWorkStat,
                DataTypeStat = dataTypeStat,
                DrugClearWorkStat = drugClearWorkStat,
                DateStat = dateStat,
                RobotStat = robotStat,
                CategoryStat = CategoryStat,
                PrioritetStat = PrioritetStat,
                Count = 300
            };
        }

        [HttpPost]
        public ActionResult GetUpdateStatLockCount()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                context.Database.CommandTimeout = 3600 * 3;

                var locks = new List<string>();

                try
                {
                    //Получаем список всех локов
                    locks = context.Database.SqlQuery<string>(@"SELECT resource_description
                                               FROM sys.dm_tran_locks with(nolock)
                                               WHERE resource_type = 'APPLICATION' and resource_description like '%DC%'").ToList();
                }
                catch
                {
                    locks.Add("DC_UpdateStat_Lock");
                }

                int updateStatLockCount = locks.Count(l => l.Contains("DC_UpdateStat_Lock"));

                return new JsonNetResult(updateStatLockCount);
            }
        }

        [HttpPost]
        public ActionResult EditComment(Domain.Model.DrugClassifier.Systematization.View.DrugInWorkView record)
        {
            try
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    var upd = context.DrugInWorkView.Where(w => w.Id == record.Id).FirstOrDefault();

                    if (upd.OperatorComment != record.OperatorComment)
                    {
                        upd.OperatorComment = record.OperatorComment;
                    }

                    context.SaveChanges();
                }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    msg += e.Message;
                }
                return BadRequest(msg);
            }
        }

    }
}