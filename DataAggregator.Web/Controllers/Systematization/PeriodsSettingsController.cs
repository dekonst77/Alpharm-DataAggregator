using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;
using DataAggregator.Web.Models.Systematization.UserSourceSettings;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Systematization
{
    [Authorize(Roles = "SBoss,SPharmacist")]
    public class PeriodsSettingsController : BaseController
    {
        [HttpPost]
        public ActionResult GetUserSourceSettings()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var sources = context.Source.ToList();
                var users = context.User.ToList();
                var userSources = context.UserSource.ToList();
                var sourceStats = context.SourceStat.ToList();
                var json = new UserSourceSettingsJson(sources, users, userSources, sourceStats);

                return new JsonNetResult(json);
            }
        }

        [HttpPost]
        public ActionResult SaveUserSourceSettings(string changedSettingsString)
        {
            var changedSettings = JsonConvert.DeserializeObject<List<ChangedSettingsJson>>(changedSettingsString);
            using (var context = new DrugClassifierContext(APP))
            {
                foreach (ChangedSettingsJson cs in changedSettings)
                {
                    var dbUserSource = context.UserSource.SingleOrDefault(us => us.UserId.ToString().Equals(cs.UserId));

                    if (dbUserSource == null)
                    {
                        var newDbUserSource = new UserSource();
                        newDbUserSource.UserId = new Guid(cs.UserId);
                        newDbUserSource.PeriodId = cs.NewPeriodId;

                        var period = context.Period.SingleOrDefault(p => p.Id == cs.NewPeriodId);
                        if (period != null)
                            newDbUserSource.SourceId = period.SourceId;
                        else
                            return SendResult("Error", "Ошибка в структуре базы данных! В таблице Period не найдена запись Id=" + cs.NewPeriodId);

                        var userGuid = new Guid(User.Identity.GetUserId());
                        newDbUserSource.LastEditorId = userGuid;

                        context.UserSource.Add(newDbUserSource);
                    }
                    else
                    {
                        //за время работы пользователя данные в БД не изменились
                        if (dbUserSource.PeriodId == cs.OldDbPeriodId)
                        {
                            if (dbUserSource.PeriodId != cs.NewPeriodId)
                            {
                                dbUserSource.PeriodId = cs.NewPeriodId;

                                var period = context.Period.SingleOrDefault(p => p.Id == cs.NewPeriodId);
                                if (period != null)
                                    dbUserSource.SourceId = period.SourceId;
                                else
                                    return SendResult("Error", "Ошибка в структуре базы данных! В таблице Period не найдена запись Id=" + cs.NewPeriodId);

                                var userGuid = new Guid(User.Identity.GetUserId());
                                dbUserSource.LastEditorId = userGuid;

                                cs.NewDbPeriodId = 0;
                            }
                            else
                            {
                                cs.NewDbPeriodId = 0;
                            }
                        }
                        else
                        {
                            //отфильтровываем быстрое двойное нажатие на кнопку на клиенте
                            if (dbUserSource.PeriodId == cs.NewPeriodId)
                            {
                                cs.NewDbPeriodId = 0;
                            }
                            //за время работы пользователя кто-то уже поменял данные в БД
                            else
                            {
                                cs.NewDbPeriodId = dbUserSource.PeriodId;

                                if (dbUserSource.LastEditorId != null)
                                {
                                    var user = context.User.SingleOrDefault(u => u.Id.Equals(dbUserSource.LastEditorId.ToString()));
                                    cs.LastEditorName = user != null ? user.FullName : "";
                                }
                                else
                                {
                                    cs.LastEditorName = "";
                                }
                            }
                        }
                    }
                }

                context.SaveChanges();
            }

            if (changedSettings.Count(c => c.NewDbPeriodId > 0) == 0)
                return SendResult("OK", null);

            return SendResult("Conflict", changedSettings.Where(c => c.NewDbPeriodId > 0));
        }

        private static ActionResult SendResult(string message, object data)
        {
            var result = new Dictionary<string, object> { { "serverMessage", message }, { "serverData", data } };

            return new JsonNetResult(result);
        }
                

      

    }
}