using System;
using System.Collections.Generic;
using System.Linq;
using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using DataAggregator.Web.Models.Classifier;

namespace DataAggregator.Core.GoodsClassifier
{
    public class GoodsClassifierDictionary
    {
        private readonly DrugClassifierContext _context;

        public class GoodsProperty
        {
            public GoodsTradeName GoodsTradeName { get; set; }
            public Brand GoodsBrand { get; set; }
            public string GoodsDescription { get; set; }
        }

        public GoodsClassifierDictionary(DrugClassifierContext context)
        {
            _context = context;
        }

        public Goods FindGood(GoodsProperty property)
        {

            var good = _context.Goods.Where(d => d.GoodsTradeNameId == property.GoodsTradeName.Id &&
                                                 d.GoodsDescription == property.GoodsDescription);

            var foundGoods = good.ToList();

            if (foundGoods.Count > 1)
            {
                throw new ApplicationException("Найдено несколько Goods");
            }

            if (foundGoods.Count == 1)
                return foundGoods.First();

            return null;
        }

        public GoodsProperty GetGoodProperty(GoodsClassifierEditorModelJson model)
        {
            //Ищем tradeName
            var goodTradeName = FindOrCreateTradeName(model);

            if (goodTradeName == null)
                throw new ApplicationException("ТН не может быть пустым");

            //Ищем brand
            var goodsBrand = FindOrCreateGoodsBrand(model);

            if (goodsBrand == null)
                throw new ApplicationException("GoodsBrand не может быть пустым");
            
            if (model.GoodsDescription == null)
                throw new ApplicationException("GoodsDescription не может быть пустым");

            var drugProperty = new GoodsProperty
            {
                GoodsTradeName = goodTradeName,
                GoodsBrand = goodsBrand,
                GoodsDescription = model.GoodsDescription
            };

            return drugProperty;
        }

        private GoodsTradeName FindTradeName(GoodsClassifierEditorModelJson model)
        {
            GoodsTradeName goodTradeName = null;

            if (model.GoodsTradeName.Id > 0)
            {
                goodTradeName = _context.GoodsTradeName.First(tn => tn.Id == model.GoodsTradeName.Id);

            }
            else if (!string.IsNullOrEmpty(model.GoodsTradeName.Value))
            {
                var goodTradeNames = _context.GoodsTradeName.Where(tn => tn.Value == model.GoodsTradeName.Value).ToList();

                if (!goodTradeNames.Any())
                    goodTradeNames = _context.GoodsTradeName.Local.Where(tn => tn.Value == model.GoodsTradeName.Value).ToList();

                if (goodTradeNames.Count > 1)
                    throw new ApplicationException("нарушение целостности торгового наименования");

                if (goodTradeNames.Count == 1)
                    return goodTradeNames.First();
            }

            return goodTradeName;
        }

        private GoodsTradeName FindOrCreateTradeName(GoodsClassifierEditorModelJson model)
        {
            GoodsTradeName goodTradeName = FindTradeName(model);


            if (goodTradeName == null && !string.IsNullOrEmpty(model.GoodsTradeName.Value))
            {
                goodTradeName = new GoodsTradeName() { Value = model.GoodsTradeName.Value };
                _context.GoodsTradeName.Add(goodTradeName);
            }

            return goodTradeName;

        }

        private Brand FindGoodsBrand(GoodsClassifierEditorModelJson model)
        {
            Brand dbGoodsBrand = null;

            if (model.GoodsBrand.Id > 0)
            {
                dbGoodsBrand = _context.Brand.First(gb => gb.Id == model.GoodsBrand.Id );

            }
            else if (!string.IsNullOrEmpty(model.GoodsBrand.Value))
            {
                var dbGoodsBrands = _context.Brand.Where(gb => gb.Value == model.GoodsBrand.Value && gb.UseGoodsClassifier).ToList();

                if (!dbGoodsBrands.Any())
                    dbGoodsBrands = _context.Brand.Local.Where(gb => gb.Value == model.GoodsBrand.Value && gb.UseGoodsClassifier).ToList();

                if (dbGoodsBrands.Count > 1)
                    throw new ApplicationException("нарушение целостности брендов");

                if (dbGoodsBrands.Count == 1)
                    return dbGoodsBrands.First();
            }

            return dbGoodsBrand;
        }
        //Поиск GoodsBran среди ЛС
        private Brand FindBrand(GoodsClassifierEditorModelJson model)
        {
            Brand dbBrand = null;

            if (model.GoodsBrand.Id > 0)
            {
                dbBrand = _context.Brand.First(gb => gb.Id == model.GoodsBrand.Id && gb.UseClassifier);
            } 
            {
                var dbBrands = _context.Brand.Where(gb => gb.Value == model.GoodsBrand.Value && gb.UseClassifier).ToList();

                if (!dbBrands.Any())
                    dbBrands = _context.Brand.Local.Where(gb => gb.Value == model.GoodsBrand.Value && gb.UseClassifier).ToList();

                if (dbBrands.Count > 1)
                    throw new ApplicationException("нарушение целостности брендов");

                if (dbBrands.Count == 1)
                    return dbBrands.First();
            }

            return dbBrand;
        }

        private Brand FindOrCreateGoodsBrand(GoodsClassifierEditorModelJson model)
        {
            Brand dbGoodsBrand = FindGoodsBrand(model);

            if (dbGoodsBrand == null && !string.IsNullOrEmpty(model.GoodsBrand.Value))
            {
                //Найдем в лс
                dbGoodsBrand = FindBrand(model);

                if(dbGoodsBrand != null)
                {
                    //Присвоим что это еще и goods
                    dbGoodsBrand.UseGoodsClassifier = true;
                }
                else
                {
                    //Если нету, то создадим
                    dbGoodsBrand = new Brand() { Value = model.GoodsBrand.Value, UseGoodsClassifier = true };
                    _context.Brand.Add(dbGoodsBrand);
                }              
            }

            return dbGoodsBrand;
        }

        public Manufacturer FindManufacturer(DictionaryJson manufacturerJson)
        {
            if (manufacturerJson.Id > 0)
                return _context.Manufacturer.First(p => p.Id == manufacturerJson.Id);

            if (!string.IsNullOrEmpty(manufacturerJson.Value))
            {
                var foundManufacturers = _context.Manufacturer.Where(p => p.Value == manufacturerJson.Value).ToList();

                if (foundManufacturers.Count == 0)
                    foundManufacturers = _context.Manufacturer.Local.Where(p => p.Value == manufacturerJson.Value).ToList();

                if (foundManufacturers.Count > 1)
                    throw new ApplicationException("Нарушение целостности справочника производителей");

                if (foundManufacturers.Count == 1)
                    return foundManufacturers.First();

            }

            return null;
        }

        public Manufacturer FindOrCreateManufacturer(DictionaryJson manufacturerJson)
        {
            var manufacturer = FindManufacturer(manufacturerJson);

            if (manufacturer == null && !string.IsNullOrEmpty(manufacturerJson.Value))
            {

                ///var keyOrder = _context.Manufacturer.Max(k => k.KeyOrder);
                ///var localKeyOrder = _context.Manufacturer.Local.ToList().Max(k => k.KeyOrder);

                ///long maxOrder = 1;

                ///if (keyOrder.HasValue)
                ///    maxOrder = keyOrder.Value;

                ///if (localKeyOrder.HasValue)
                ///    maxOrder = localKeyOrder.Value;

                ///if (keyOrder.HasValue && localKeyOrder.HasValue)
                ///    maxOrder = keyOrder.Value > localKeyOrder.Value ? keyOrder.Value : localKeyOrder.Value;

                manufacturer = new Manufacturer()
                {
                    Value = manufacturerJson.Value,
                    ///Key = (maxOrder + 1).ToString(),
                    ///KeyOrder = maxOrder + 1
                };

                _context.Manufacturer.Add(manufacturer);
            }

            return manufacturer;
        }

        public GoodsProductionInfo FindGoodsProductionInfo(Manufacturer ownerTradeMark, Manufacturer packer, Goods good)
        {
            GoodsProductionInfo goodsProductionInfo = null;

            if (good.Id > 0 && ownerTradeMark.Id > 0 && packer.Id > 0)
            {
                var goodsProductionInfoFound = _context.GoodsProductionInfo.Where(p => p.OwnerTradeMarkId == ownerTradeMark.Id && p.PackerId == packer.Id && p.GoodsId == good.Id).ToList();

                if (goodsProductionInfoFound.Count > 1)
                    throw new ApplicationException("Нарушение целостности ProductionInfo");

                if (goodsProductionInfoFound.Count == 1)
                    goodsProductionInfo = goodsProductionInfoFound.First();
            }

            return goodsProductionInfo;
        }

        public void UpdateOrCreateGoodsBrandClassification(GoodsTradeName goodsTradeName, Manufacturer ownerTradeMark, Brand goodsBrand)
        {
            GoodsBrandClassification dbGoodsBrandClassification = null;

            if (goodsTradeName.Id > 0 && ownerTradeMark.Id > 0)
            {
                var dbgoodsBrandClassificationList =
                    _context.GoodsBrandClassification.Where(
                        b =>
                            b.GoodsTradeNameId == goodsTradeName.Id && b.OwnerTradeMarkId == ownerTradeMark.Id).ToList();

                if (dbgoodsBrandClassificationList.Count > 1)
                {
                    throw new ApplicationException("Нарушение целостности GoodsBrandClassification");
                }

                if (dbgoodsBrandClassificationList.Count == 1)
                {
                    dbGoodsBrandClassification = dbgoodsBrandClassificationList.First();
                    dbGoodsBrandClassification.GoodsBrand = goodsBrand;
                }
                else
                {
                    dbGoodsBrandClassification = new GoodsBrandClassification()
                    {
                        GoodsTradeName = goodsTradeName,
                        OwnerTradeMark = ownerTradeMark,
                        GoodsBrand = goodsBrand
                    };
                    _context.GoodsBrandClassification.Add(dbGoodsBrandClassification);
                }
            }
        }

        public GoodsProductionInfo CreateGoodsProductionInfo(Manufacturer ownerTradeMark, Manufacturer packer, Goods good, GoodsCategoryJson goodsCategory, bool Used, string Comment = "")
        {
            var goodProductionInfo = new GoodsProductionInfo()
            {
                Goods = good,
                OwnerTradeMark = ownerTradeMark,
                Packer = packer,
                Used = Used,
                GoodsCategoryId = goodsCategory.Id > 0 ? goodsCategory.Id : null,
                Comment = Comment
            };
            _context.GoodsProductionInfo.Add(goodProductionInfo);

            return goodProductionInfo;
        }

        public Goods CreateGood(GoodsProperty goodProperty)
        {
            Goods good = new Goods();
            good.GoodsTradeName = goodProperty.GoodsTradeName;
            good.GoodsDescription = goodProperty.GoodsDescription;
            _context.Goods.Add(good);

            return good;
        }

        public void UpdateGoodsProductionInfoParameter(long goodsProductionInfoId, List<long> parameterIds)
        {
            var gpipList =
                _context.GoodsProductionInfoParameter.Where(
                    g => g.GoodsProductionInfoId == goodsProductionInfoId)
                    .ToList();

            if (parameterIds == null)
            {
                parameterIds = new List<long>();
            }

            foreach (var gpip in gpipList)
            {
                // если запись уже есть в БД
                if (parameterIds.Contains(gpip.ParameterId))
                {
                    parameterIds.Remove(gpip.ParameterId);
                }
                // если запись в БД уже неактуальна
                else
                {
                    _context.GoodsProductionInfoParameter.Remove(gpip);
                }
            }

            // в списке остались только те записи, которые надо добавить в БД
            foreach (var parameterId in parameterIds)
            {
                _context.GoodsProductionInfoParameter.Add(new GoodsProductionInfoParameter
                {
                    GoodsProductionInfoId = goodsProductionInfoId,
                    ParameterId = parameterId
                });
            }
        }
    }
}
