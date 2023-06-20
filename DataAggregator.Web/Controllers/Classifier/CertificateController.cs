using DataAggregator.Core.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Certificate;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using DataAggregator.Domain.Model.GRLS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SPharmacist")]
    public class CertificateController : BaseController
    {
        public ActionResult Certificates_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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

        /// <summary>
        /// Скачивание реестра ГРЛС
        /// </summary>
        /// <param name="searchText">фильтр</param>
        /// <returns></returns>
        [HttpPost]        
        public ActionResult Certificates_search(string searchText)
        {
            try
            {
                using (var _context = new DrugClassifierContext(APP))
                {
                    IEnumerable<GetCertificates_SP_Result> result = _context.GetCertificates_SP(searchText).ToList();
                    ViewData["Certificates"] = result;
                }

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        //[HttpPost]
        //public ActionResult Certificate_save(string name, string type,
        //    ICollection<DataAggregator.Domain.Model.DrugClassifier.Certificate.Certificate> array_Certificate
        //    )
        //{
        //    try
        //    {
        //        var _context = new DrugClassifierContext(APP);
        //        if (array_Certificate != null)
        //            foreach (var item in array_Certificate)
        //            {
        //            }

        //        JsonNetResult jsonNetResult = new JsonNetResult
        //        {
        //            Formatting = Formatting.Indented,
        //            Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
        //        };
        //        return jsonNetResult;
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e);
        //    }
        //}



        public ActionResult Certificate_Init(string Id)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);

                var Cert_Certificate = _context.Cert_Certificate.Where(w => w.Number_ID == Id).ToList();
                var Cert_Certificate1 = Cert_Certificate.Single();
                ViewData["FV"] = _context.Cert_FV.Where(w => w.CertificateId == Cert_Certificate1.Id).ToList();
                ViewData["ManufactureWay"] = _context.Cert_ManufactureWay.Where(w => w.CertificateId == Cert_Certificate1.Id).ToList();
                ViewData["SubstRaw"] = _context.Cert_SubstRaw.Where(w => w.CertificateId == Cert_Certificate1.Id).ToList();

                ViewData["Certificate"] = Cert_Certificate;

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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



        public ActionResult Substance_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);

                ViewData["Substance"] = _context.SubstanceView.ToList();
                ViewData["Chemicals"] = _context.ChemicalsView.ToList();
                ViewData["ChemicalSPR"] = _context.ChemicalSPR.ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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

        [HttpPost]
        public ActionResult Substance_save(ICollection<DataAggregator.Domain.Model.DrugClassifier.Certificate.SubstanceView> array_Substance,
            ICollection<DataAggregator.Domain.Model.DrugClassifier.Certificate.ChemicalsView> array_Chemicals)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_Substance != null)
                {
                    foreach (var item in array_Substance)
                    {
                        if (item.Id > 0)
                        {
                            var UPD = _context.NumberINN.Where(w => w.Id == item.Id).Single();
                            UPD.Id_new = item.Id_new;
                        }
                    }
                }
                if (array_Chemicals != null)
                {
                    foreach (var item in array_Chemicals)
                    {
                        if (item.Value == null) item.Value = "";
                        if (item.Description == null) item.Description = "";
                        if (item.Id >= 0)//проверка по справочнику хим формул
                        {
                            var ChemicalSPR = _context.ChemicalSPR.Where(w => w.Value == item.Value).FirstOrDefault();
                            if (ChemicalSPR == null)
                            {//нет такого в справочнике нужно завести
                                ChemicalSPR = _context.ChemicalSPR.Add(new Domain.Model.DrugClassifier.Certificate.ChemicalSPR() { Value = item.Value, Description = item.Description });
                            }
                            if (item.ChemicalSPRId == ChemicalSPR.Id)
                            {
                                ChemicalSPR.Description = item.Description;
                            }
                            else
                            {
                                item.ChemicalSPRId = ChemicalSPR.Id;
                            }

                        }
                        if (item.Id > 0)//обновление
                        {
                            var UPD = _context.Chemicals.Where(w => w.Id == item.Id).Single();
                            UPD.ChemicalSPRId = item.ChemicalSPRId;
                        }
                        if (item.Id == 0)//новые
                        {
                            var ADD = new DataAggregator.Domain.Model.DrugClassifier.Certificate.Chemicals() { NumberINN_NewId = item.NumberINN_NewId, ChemicalSPRId = item.ChemicalSPRId };
                            _context.Chemicals.Add(ADD);
                        }
                        if (item.Id < 0)//удаление
                        {
                            var del = _context.Chemicals.Where(w => w.Id == -1 * item.Id).Single();
                            _context.Chemicals.Remove(del);
                        }
                    }
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }




        public ActionResult MnfWay_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                ViewData["ManufacturerClear"] = _context.ManufacturerClear.ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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
        [HttpPost]
        public ActionResult MnfWay_search(bool IsEmpty, bool IsOnlyEX)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                var ManufactureWayView = _context.Cert_ManufactureWayView.Where(w => w.CertType == "grls Лекарственные препараты");
                if (IsEmpty)
                    ManufactureWayView = ManufactureWayView.Where(w => w.ManufacturerClearId == null || (w.PackerId == null && w.Status == 0));
                if (IsOnlyEX)
                    ManufactureWayView = ManufactureWayView.Where(w => w.CertStatus == "Д" || w.CertStatus == "ЕЭК");
                ViewData["MnfWay"] = ManufactureWayView.ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult MnfWay_save(ICollection<DataAggregator.Domain.Model.DrugClassifier.Certificate.ManufactureWayView> array_MnfWay
            )
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_MnfWay != null)
                    foreach (var item in array_MnfWay)
                    {
                        var UPD = _context.Cert_ManufactureWay.Where(w => w.Id == item.Id).Single();

                        if (item.ManufacturerClearValue != null)
                        {
                            var ManufacturerClear = _context.ManufacturerClear.Where(w => w.Value == item.ManufacturerClearValue).FirstOrDefault();
                            if (ManufacturerClear == null)
                            {//нет такого в справочнике нужно завести
                                ManufacturerClear = _context.ManufacturerClear.Add(new Domain.Model.DrugClassifier.Classifier.ManufacturerClear() { Value = item.ManufacturerClearValue });
                                _context.SaveChanges();
                            }
                            if (item.ManufacturerClearId == ManufacturerClear.Id)
                            {
                                //ChemicalSPR.Description = item.Description;
                            }
                            else
                            {
                                UPD.ManufacturerClearId = ManufacturerClear.Id;
                            }
                        }
                        else
                        {
                            item.ManufacturerClearId = null;
                        }

                        UPD.PackerId = item.PackerId;
                        UPD.Status = item.Status;
                    }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult Classifier_search(string TypeSet, string RuNumber)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                switch (TypeSet)
                {
                    case "MnfWay":
                        var classifier = _context.ClassifierEditorFilterView.Where(w => w.RegistrationCertificateNumber == RuNumber);
                        ViewData["classifier"] = classifier.ToList();
                        break;
                    case "ESKLP":
                        var classifierdp = _context.ClassifierEditorFilterClassifierPackingView.Where(w => w.RegistrationCertificateNumber == RuNumber);
                        ViewData["classifier"] = classifierdp.ToList();
                        break;
                }
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        public ActionResult ESKLP_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                ViewData["Equipment"] = _context.Equipment.OrderBy(o => o.Value).ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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
        [HttpPost]
        public ActionResult ESKLP_search(bool IsEmpty, bool IsOnlyEX, string Text)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                var ESKLPView = _context.ESKLPView.Where(w => 1 == 1);
                if (IsEmpty)
                    ESKLPView = ESKLPView.Where(w => w.ClassifierPackingId == null || w.ClassifierId == null);
                if (IsOnlyEX)
                    ESKLPView = ESKLPView.Where(w => w.IsActual == true);
                if (!string.IsNullOrEmpty(Text))
                {
                    ESKLPView = ESKLPView.Where(w => w.RuNumber.Contains(Text) || w.TradeName.Contains(Text) || w.NormINN.Contains(Text) || w.Manufacturer.Contains(Text));
                }
                ViewData["ESKLP"] = ESKLPView.ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult ESKLP_save(ICollection<DataAggregator.Domain.Model.DrugClassifier.Certificate.ESKLP> array_ESKLP,
            ICollection<ClassifierEditorFilterClassifierPackingView> array_ClassifierPacking
            )
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_ESKLP != null)
                    foreach (var item in array_ESKLP)
                    {
                        var UPD = _context.ESKLP.Where(w => w.Id == item.Id).Single();

                        UPD.ClassifierPackingId = item.ClassifierPackingId;
                        UPD.ClassifierId = item.ClassifierId;
                        UPD.IsActual = item.IsActual;
                        UPD.Flag = item.Flag;
                    }
                if (array_ClassifierPacking != null)
                    foreach (var item in array_ClassifierPacking)
                    {
                        var UPD = _context.ClassifierPacking.Where(w => w.Id == item.ClassifierPackingId).Single();

                        UPD.CountInPrimaryPacking = (int)item.CountInPrimaryPacking;
                        UPD.CountPrimaryPacking = (int)item.CountPrimaryPacking;
                    }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult ESKLP_ClassifierPacking_save(string ESKLPId, long ClassifierId, int ClassifierPackingId)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);

                var CI = _context.ClassifierInfo.Where(w => w.Id == ClassifierId).Single();
                var ESKLP = _context.ESKLP.Where(w => w.Id == ESKLPId).Single();
                ClassifierPacking CP = new ClassifierPacking();
                if (ClassifierPackingId > 0)
                {
                    //обновление, но не обновлять
                    var ClassifierPacking_now = _context.ClassifierPacking.Where(w => w.Id == ClassifierPackingId).Single();
                    CP.CountInPrimaryPacking = ClassifierPacking_now.CountInPrimaryPacking;
                    CP.CountPrimaryPacking = ClassifierPacking_now.CountPrimaryPacking;
                }
                else
                {
                    //добавление
                    CP.CountInPrimaryPacking = Convert.ToInt32(ESKLP.FirstAmount);
                    CP.CountPrimaryPacking = Convert.ToInt32(ESKLP.SecondAmount);
                }
                CP.Id = ClassifierPackingId;
                CP.ClassifierId = ClassifierId;
                CP.PrimaryPackingId = null;
                CP.PrimaryPacking = new Packing() { Id = null, Value = ESKLP.FirstFV };

                CP.ConsumerPackingId = null;
                CP.ConsumerPacking = new Packing() { Id = null, Value = ESKLP.SecondFV };

                CP.PackingDescription = ESKLP.SecondFVAdd;

                ClassifierDictionary CD = new ClassifierDictionary(_context);

                var ClassifierPacking = CD.CreateClassifierPacking(CI, CP);
                _context.SaveChanges();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult ESKLP_ClassifierPacking_delete(long ClassifierId, int ClassifierPackingId)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                var CPs = _context.ClassifierPacking.Where(w => w.ClassifierId == ClassifierId);
                if (CPs.Count() <= 1)
                    throw new ApplicationException("Ошибка: нельзя удалять все упаковочки");
                var CP = _context.ClassifierPacking.Where(w => w.Id == ClassifierPackingId).Single();
                _context.ClassifierPacking.Remove(CP);
                _context.SaveChanges();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
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