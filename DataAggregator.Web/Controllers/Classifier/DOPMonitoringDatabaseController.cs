using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace DataAggregator.Web.Controllers.Classifier
{
    /// <summary>
    /// Модуль для добавления ДОП ассортимента в БД мониторинг
    /// Разработка #11668
    /// </summary>
    [Authorize(Roles = "SPharmacist")]
    public class DOPMonitoringDatabaseController : BaseController
    {
        private readonly DrugClassifierContext _context;

        public DOPMonitoringDatabaseController()
        {
            _context = new DrugClassifierContext(APP);
        }

        ~DOPMonitoringDatabaseController()
        {
            _context.Dispose();
        }
        
        /// <summary>
        /// получить СКЮ доп. ассортимента + блокировки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Init()
        {
            try
            {
                var result = _context.GetDOPBlockingForMonitoringDatabase_Result().ToList();
                ViewData["DOPBlocking"] = result;

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// получить все блокировки (по категории, по доп. свойству, по СКЮ)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InitBlocking()
        {
            try
            {
                var result = _context.GetBlocking_Result().ToList();
                ViewData["Blocking"] = result;

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Загрузить список категорий
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetGoodsCategoryList()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    return ReturnData(context.GoodsCategory.Include(t => t.GoodsSection)
                        .OrderBy(c => c.GoodsSection.Name).ThenBy(c => c.Name).ToList());
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        #region Установка заглушек

        /// <summary>
        /// Заглушка на категорию
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        /// <returns></returns>
        public ActionResult SetPlugOnByCategory(long GoodsCategoryId)
        {
            try
            {
                _context.SetPlugOnByCategory_SP(GoodsCategoryId);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Поставить заглушку целой категории + доп. свойство
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        /// <param name="ParameterID"></param>
        /// <returns></returns>
        public ActionResult SetPlugOnByCategoryAndProperty(long GoodsCategoryId, long ParameterID)
        {
            try
            {
                _context.SetPlugOnByCategoryAndProperty_SP(GoodsCategoryId, ParameterID);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Поставить заглушку на список СКЮ
        /// </summary>
        /// <param name="ClassifierIdList"></param>
        /// <returns></returns>
        public ActionResult SetPlugOnByClassifierList(long[] ClassifierIdList)
        {
            try
            {
                _context.SetPlugOnByClassifierList_SP(ClassifierIdList);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        #endregion

        /// <summary>
        /// Снять заглушку c категории
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        /// <param name="PouringStartDate"></param>
        /// <returns></returns>
        public ActionResult SetPlugOffByCategory(long GoodsCategoryId, DateTime PouringStartDate)
        {
            try
            {
                _context.SetPlugOffByCategory_SP(GoodsCategoryId, PouringStartDate);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// снять заглушку с категории и доп. свойства
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        /// <param name="ParameterID"></param>
        /// <param name="PouringStartDate">День, за который будут отдаваться отклассифицированные данные ОФД, получать от них агрегаты, выливаться в клик БД [Мониторинг розничных продаж]</param>
        /// <returns></returns>
        public ActionResult SetPlugOffByCategoryAndProperty(long GoodsCategoryId, long ParameterID, DateTime PouringStartDate)
        {
            try
            {
                _context.SetPlugOffByCategoryAndProperty_SP(GoodsCategoryId, ParameterID, PouringStartDate);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        public ActionResult SetPlugOffByClassifierList(long[] ClassifierIdList, DateTime PouringStartDate)
        {
            try
            {
                _context.SetPlugOffByClassifierList_SP(ClassifierIdList, PouringStartDate);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

    }
}