using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.OFD;
using System;
using System.Linq;
using System.Web.Mvc;
using DataAggregator.Domain.Model.EtalonPrice;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;

namespace DataAggregator.Web.Controllers.OFD
{
    public class PriceCurrentController : BaseController
    {

        private readonly OFDContext _context;


        public PriceCurrentController()
        {
            _context = new OFDContext(APP);
        }

        ~PriceCurrentController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetCalcLock()
        {
            _context.Database.CommandTimeout = 3600 * 3;

            List<string> locks = new List<string>();

            var calcStatus = false;

            //Получаем список всех локов
            locks = _context.Database.SqlQuery<string>(@"   SELECT resource_description
                                                            FROM sys.dm_tran_locks with(nolock)
                                                            WHERE resource_type = 'APPLICATION' and resource_description like '%EtalonPrice_Calculated_Lock%' ").ToList();

            if (locks.Any(l => l.Contains("EtalonPrice_Calculated_Lock")))
            {
                calcStatus = true;
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = calcStatus
            };
        }


        [HttpPost]
        public ActionResult GetStatuses()
        {
            string statusText = _context.StatusCalcCurrentPrice();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = statusText,
            };
        }


        //public ActionResult Add(PriceCurrentModel price)
        //{
        //    try
        //    {


        //        var jsonNetResult = new JsonNetResult
        //        {
        //            Formatting = Formatting.Indented,
        //            Data = priceEtalonModel
        //        };

        //        return jsonNetResult;
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        public ActionResult Edit(PriceCurrentModel price)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());

                var priceEtalon = _context.PriceCurrent.First(p => p.ClassifierId == price.ClassifierId);

                if (priceEtalon.Comment != price.Comment ||
                    priceEtalon.PriceNew != price.PriceNew ||
                    priceEtalon.WithoutPrice != price.WithoutPrice ||
                    priceEtalon.IsFractionalPackaging != price.IsFractionalPackaging ||
                    priceEtalon.ForChecking != price.ForChecking)
                {
                    priceEtalon.ForChecking = price.ForChecking;
                    priceEtalon.Comment = price.Comment;
                    priceEtalon.PriceNew = price.PriceNew;
                    priceEtalon.DateUpdate = DateTime.Now;
                    priceEtalon.WithoutPrice = price.WithoutPrice;
                    priceEtalon.IsFractionalPackaging = price.IsFractionalPackaging;
                    priceEtalon.UserIdUpdate = userGuid;
                    _context.SaveChanges();
                }

                var priceEtalonView = _context.PriceCurrentView.First(p => p.ClassifierId == price.ClassifierId);
                var priceEtalonModel = PriceCurrentModel.Create(priceEtalonView);

                var jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = priceEtalonModel
                };

                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //Запустить Job расчет параметров
        public ActionResult RunCalcCurrentPrice()
        {
            try
            {
                _context.RunCalcCurrentPrice();
                return null;
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }


        }

        //Сохранить текущие цены в эталонные цены
        public ActionResult CurrentPriceCopyToEtalonPrice(DateTime date)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());
                _context.CurrentPriceCopyToEtalonPrice(date.Year, date.Month, userGuid);
                return null;
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Загрузить список
        /// </summary>
        /// <returns></returns>
        public ActionResult Load()
        {
            try
            {

                var priceView = _context.PriceCurrentView.ToList();

                var priceetalonmodel = priceView.Select(PriceCurrentModel.Create).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Data = new
                    {
                        Data = priceetalonmodel
                    }
                };


                return jsonNetResult;
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: PriceEtalon
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult UploadFromExcel(int month, int year, HttpPostedFileBase file)
        {
            try
            {
                using (_context)
                {
                    var userId = new Guid(User.Identity.GetUserId());

                    string filename = $@"\\s-sql2\Upload\PriceCurrent_{userId}.xlsx";

                    if (System.IO.File.Exists(filename))
                        System.IO.File.Delete(filename);

                    file.SaveAs(filename);

                    _context.ImportPriceCurrent_from_Excel(month, year, userId, filename);
                }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonNetResult() { Data = null }
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}