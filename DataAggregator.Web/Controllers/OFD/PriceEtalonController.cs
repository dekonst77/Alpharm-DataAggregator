using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.OFD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAggregator.Domain.Model.OFD;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DataAggregator.Web.Controllers.OFD
{
    public class PriceEtalonController : BaseController
    {
        private readonly OFDContext _context;

        public PriceEtalonController()
        {
            _context = new OFDContext(APP);
        }

        ~PriceEtalonController()
        {
            _context.Dispose();
        }


        public ActionResult SearchDrug(string Description, int? ClassifierId)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());


                List<Classifier_ExternalView> classifier = new List<Classifier_ExternalView>();

                if (ClassifierId.HasValue)
                    classifier =
                        _context.Classifier_ExternalView.Where(e => e.ClassifierId == ClassifierId.Value).ToList();
                else
                    classifier = _context.Classifier_ExternalView.Where(e => e.DrugDescription.Contains(Description) || e.TradeName.Contains(Description)).ToList();


                var result = classifier.Select(c => new
                {
                    ClassifierId = c.ClassifierId,
                    DrugDescription = c.TradeName + c.DrugDescription,
                    OwnerTradeMark = c.OwnerTradeMark,
                    Packer = c.Packer
                }).ToList();


                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };

                return jsonNetResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [HttpPost]
        public ActionResult Add(PriceEtalonModel model)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());

                PriceEtalon priceEtalon = new PriceEtalon();
                priceEtalon.Period = new DateTime(model.Year, model.Month, 15);
                priceEtalon.ClassifierId = model.ClassifierId;
                priceEtalon.Price = model.Price;
                priceEtalon.DateUpdate = DateTime.Now;
                priceEtalon.UserIdUpdate = userGuid;
                _context.PriceEtalon.Add(priceEtalon);
                _context.SaveChanges();
               
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                Data = new
                {
                    Data = true
                }
            };

            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult Delete(PriceEtalonModel model)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());

                var priceEtalon = _context.PriceEtalon.FirstOrDefault(p => p.Id == model.Id);

                if (priceEtalon != null)
                {
                    _context.PriceEtalon.Remove(priceEtalon);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                Data = new
                {
                    Data = true
                }
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult Edit(PriceEtalonModel model)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());

                var priceEtalon = _context.PriceEtalon.FirstOrDefault(p => p.Id == model.Id);

                if (priceEtalon != null)
                {
                    priceEtalon.ClassifierId = model.ClassifierId;
                    priceEtalon.Price = model.Price;
                    priceEtalon.DateUpdate = DateTime.Now;
                    priceEtalon.UserIdUpdate = userGuid;
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                Data = new
                {
                    Data = true
                }
            };


            return jsonNetResult;
        }


        public ActionResult Load(PriceEtalonFilter filter)
        {
            try
            {
                var date = filter.date.Year * 100 + filter.date.Month;

                var priceetalonView = _context.PriceEtalonView.Where(p => 1 == 1);


                priceetalonView = priceetalonView.Where(p => p.Year * 100 + p.Month == date);


                var priceetalon = priceetalonView.ToList();

                var priceetalonmodel = priceetalon.Select(PriceEtalonModel.Create).ToList();

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




    }
}