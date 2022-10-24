using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;


namespace DataAggregator.Core.GoodsClassifier
{
    public class GoodsClassifierEditor
    {
        private readonly DrugClassifierContext _context;

        private readonly Guid _user;

        private readonly GoodsClassifierDictionary _dictionary;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <param name="user">Пользователь делающий изменения</param>
        public GoodsClassifierEditor(DrugClassifierContext context, Guid user)
        {
            _context = context;
            _context.Database.CommandTimeout = 6000;
            _user = user;
            _dictionary = new GoodsClassifierDictionary(context);
        }

        public class GoodsChangeInfo
        {
            public List<GoodsChangeDescription> Items { get; set; }
        }

        //Информация о том какие действия 
        public struct GoodsChangeStatus
        {
            public bool CanRecreate { get; set; }

            public bool NeedMerge { get; set; }

            public string DrugDescription { get; set; }

            public bool OnlyOneProductionInfo { get; set; }
        }

        public class GoodsChangeDescription
        {
            public string Title { get; set; }

            public string OldId { get; set; }

            public string NewId { get; set; }

            public string OldValue { get; set; }

            public string NewValue { get; set; }

            public GoodsChangeDescription(string title, string oldValue, string newValue)
            {
                this.OldValue = oldValue;
                this.NewValue = newValue;
                this.Title = title;
            }

            public GoodsChangeDescription(string title, long oldId, string oldValue, long newId, string newValue)
            {
                this.NewId = newId == 0 ? "Новый" : newId.ToString();
                this.OldId = oldId == 0 ? "Новый" : oldId.ToString();
                this.OldValue = oldValue;
                this.NewValue = newValue;
                this.Title = title;
            }

            public string NewText
            {
                get { return String.Format("{0} {1}", NewId, NewValue).Trim(); }
            }

            public string OldText
            {
                get { return String.Format("{0} {1}", OldId, OldValue).Trim(); }
            }
        }

        /// <summary>
        /// Проверяем что новое а что старое
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public GoodsClassifierInfoModel CheckClassifier(GoodsClassifierEditorModelJson model)
        {
            GoodsClassifierInfoModel infoModel = new GoodsClassifierInfoModel();

            var newWord = "Новый";

            var property = _dictionary.GetGoodProperty(model);
            var good = _dictionary.FindGood(property);

            var ownerTradeMark = _dictionary.FindManufacturer(model.OwnerTradeMark);
            var packer = _dictionary.FindManufacturer(model.Packer);

            infoModel.GoodKey = good != null ? good.Id.ToString() : newWord;
            infoModel.OwnerTradeMarkId = ownerTradeMark != null ? ownerTradeMark.Id : 0;
            infoModel.PackerId = packer != null ? packer.Id : 0;

            return infoModel;
        }

        /// <summary>
        /// Проверка перед изменением с новым LKCU, на допустимость операции и нужно ли проводить объединение с другим LKCU.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public GoodsChangeStatus CheckRecreate(GoodsClassifierEditorModelJson model)
        {
            GoodsChangeStatus changeStatus = new GoodsChangeStatus()
            {
                CanRecreate = true,
                NeedMerge = false,
                DrugDescription = string.Empty,
                OnlyOneProductionInfo = false
            };

            var goodProperty = _dictionary.GetGoodProperty(model);

            var good = _dictionary.FindGood(goodProperty);

            if (good == null)
            {
                return changeStatus;
            }

            if (good != null && good.Id == model.GoodsId)
            {
                changeStatus.CanRecreate = false;
            }

            if (good != null)
            {
                changeStatus.NeedMerge = true;

                StringBuilder builder = new StringBuilder();
                builder.Append("Id : ");
                builder.Append(good.Id + " ");
                builder.Append(good.GoodsTradeName.Value + " ");
                builder.Append(good.GoodsDescription);

                changeStatus.DrugDescription = builder.ToString();
            }

            return changeStatus;
        }

        /// <summary>
        /// Получаем спиоск изменений
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public GoodsChangeInfo GetChanges(GoodsClassifierEditorModelJson model)
        {
            List<GoodsChangeDescription> changes = new List<GoodsChangeDescription>();
            var good = _context.Goods.First(d => d.Id == model.GoodsId);

            //Изменяемый good не найден
            if (good == null)
                throw new ApplicationException(string.Format("not found Good Id = {0}", model.GoodsId));

            var goodProperty = _dictionary.GetGoodProperty(model);

            if (good.GoodsTradeName.Value != goodProperty.GoodsTradeName.Value)
            {
                changes.Add(new GoodsChangeDescription("Торговое наименование", good.GoodsTradeName.Value, goodProperty.GoodsTradeName.Value));
            }

            if (good.GoodsDescription != goodProperty.GoodsDescription)
            {
                changes.Add(new GoodsChangeDescription("Форма выпуска", good.GoodsDescription, goodProperty.GoodsDescription));
            }

            var ownerTradeMark = _dictionary.FindManufacturer(model.OwnerTradeMark) ?? new Manufacturer() { Value = model.OwnerTradeMark.Value, Id = 0 };
            var packer = _dictionary.FindManufacturer(model.Packer) ?? new Manufacturer() { Value = model.Packer.Value, Id = 0 };
            var oldOwnerTradeMark = _context.Manufacturer.First(m => m.Id == model.OwnerTradeMarkId);
            var oldPacker = _context.Manufacturer.First(m => m.Id == model.PackerId);

            if (oldOwnerTradeMark.Value != ownerTradeMark.Value)
            {
                changes.Add(new GoodsChangeDescription("Правообладатель", oldOwnerTradeMark.Value, ownerTradeMark.Value));
            }

            if (oldPacker.Value != packer.Value)
            {
                changes.Add(new GoodsChangeDescription("Упаковщик", oldPacker.Value, packer.Value));
            }

            var goodsProductionInfo = _context.GoodsProductionInfo.First(p => p.Id == model.ProductionInfoId);

            if (model.Used != goodsProductionInfo.Used)
            {
                changes.Add(new GoodsChangeDescription("Used", goodsProductionInfo.Used.ToString(), model.Used.ToString()));
            }

            var classifier = _context.ClassifierInfo.FirstOrDefault(t => t.GoodsProductionInfoId == goodsProductionInfo.Id);
            if (classifier == null)
                throw new ApplicationException("Классификатор для доп. материала с id " + goodsProductionInfo.Id + " не обнаружен!");

            if (model.ToRetail != classifier.ToRetail)
            {
                changes.Add(new GoodsChangeDescription("ToRetail", classifier.ToRetail.ToString(), model.ToRetail.ToString()));
            }

            if ((goodsProductionInfo.GoodsCategory != null && model.GoodsCategory.Id == 0) ||
            (goodsProductionInfo.GoodsCategory == null && model.GoodsCategory.Id != 0) ||
            (goodsProductionInfo.GoodsCategory != null && model.GoodsCategory.Id != 0 && goodsProductionInfo.GoodsCategory.Name != model.GoodsCategory.Name))
            {

                var oldValue = String.Empty;
                if (goodsProductionInfo.GoodsCategory != null)
                    oldValue = goodsProductionInfo.GoodsCategory.Name;

                var newValue = String.Empty;
                if (model.GoodsCategory != null)
                    newValue = model.GoodsCategory.Name;

                changes.Add(new GoodsChangeDescription("Категория", oldValue, newValue));
            }

            var oldGoodsBrandClassification =
                _context.GoodsBrandClassification.FirstOrDefault(
                    gbc =>
                        gbc.GoodsTradeNameId == good.GoodsTradeName.Id && gbc.OwnerTradeMarkId == oldOwnerTradeMark.Id);
            if ((oldGoodsBrandClassification == null && !String.IsNullOrEmpty(model.GoodsBrand.Value)) ||
                (oldGoodsBrandClassification != null && String.IsNullOrEmpty(model.GoodsBrand.Value)) ||
                !oldGoodsBrandClassification.GoodsBrand.Value.Equals(model.GoodsBrand.Value)
                )
            {
                var oldValue = String.Empty;
                if (oldGoodsBrandClassification != null)
                {
                    oldValue = oldGoodsBrandClassification.GoodsBrand.Value;
                }

                var newValue = String.Empty;
                if (!String.IsNullOrEmpty(model.GoodsBrand.Value))
                {
                    newValue = model.GoodsBrand.Value;
                }

                changes.Add(new GoodsChangeDescription("Бренд", oldValue, newValue));
            }

            var gpipList =
                _context.GoodsProductionInfoParameter.Where(g => g.GoodsProductionInfoId == model.ProductionInfoId)
                    .ToList();

            var tempParameterIds = model.ParameterIds != null ? new List<long>(model.ParameterIds) : new List<long>();

            foreach (var gpip in gpipList)
            {
                if (!tempParameterIds.Contains(gpip.ParameterId))
                {
                    var oldValue = gpip.Parameter.Value;

                    var newParameter =
                        _context.Parameter.FirstOrDefault(
                            p =>
                                tempParameterIds.Contains(p.Id) &&
                                p.ParameterGroupId == gpip.Parameter.ParameterGroupId);

                    var newValue = String.Empty;
                    if (newParameter != null)
                    {
                        newValue = newParameter.Value;
                    }

                    changes.Add(new GoodsChangeDescription("Доп. свойство \"" + gpip.Parameter.ParameterGroup.Name + "\"",
                        oldValue, newValue));

                    var sameGroupParameterIds =
                        _context.Parameter.Where(p => p.ParameterGroupId == gpip.Parameter.ParameterGroupId)
                            .Select(p => p.Id)
                            .ToList();

                    tempParameterIds.RemoveAll(id => sameGroupParameterIds.Contains(id));
                }
                else
                {
                    tempParameterIds.Remove(gpip.ParameterId);
                }
            }

            foreach (var parId in tempParameterIds)
            {
                var parameter = _context.Parameter.First(p => p.Id == parId);

                changes.Add(new GoodsChangeDescription("Доп. свойство \"" + parameter.ParameterGroup.Name + "\"",
                        String.Empty, parameter.Value));
            }

            if (goodsProductionInfo.Comment != model.Comment)
            {
                changes.Add(new GoodsChangeDescription("Comment", goodsProductionInfo.Comment, model.Comment));
            }

            return new GoodsChangeInfo() { Items = changes };
        }

        /// <summary>
        /// Объединение двух позиций классфикатора
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tryMode"></param>
        /// <returns></returns>
        public GoodsClassifierInfoModel MergeClassifier(GoodsClassifierEditorModelJson model, bool tryMode)
        {
            //Открываем транзакцию
            using (var transaction = _context.Database.BeginTransaction())
            {
                var goodProperty = _dictionary.GetGoodProperty(model);

                var fromGood = _context.Goods.First(g => g.Id == model.GoodsId);
                var toGood = _dictionary.FindGood(goodProperty);

                //Если по модели не удалось найти Good к которому будем объединять
                if (toGood == null)
                    throw new ApplicationException("toGood is null need ReCreate");

                //По модели находим Производителя и Упаковщика так как мы объединяем только по Drug

                var ownerTradeMark = _dictionary.FindOrCreateManufacturer(model.OwnerTradeMark);
                var packer = _dictionary.FindOrCreateManufacturer(model.Packer);

                var fromOwnerTradeMark = _context.Manufacturer.FirstOrDefault(m => m.Id == model.OwnerTradeMarkId);
                var fromPacker = _context.Manufacturer.FirstOrDefault(m => m.Id == model.PackerId);

                //Теперь пытаемся найти PIс которая будет удаляться
                var fromGoodsProductionInfo = _dictionary.FindGoodsProductionInfo(fromOwnerTradeMark, fromPacker, fromGood);

                //Пытаемся найти PIо к которой будем объединять, если такой нет - то создаем её.
                var toGoodsProductionInfo = _dictionary.FindGoodsProductionInfo(ownerTradeMark, packer, toGood) ??
                                       _dictionary.CreateGoodsProductionInfo(ownerTradeMark, packer, toGood, model.GoodsCategory, model.Used);


                _context.SaveChanges();

                GoodsLogAction.Log(_context, toGoodsProductionInfo.Id, GoodsLogAction.GoodsActionType.Merge, _user);
                if (fromGoodsProductionInfo != null)
                {
                    GoodsLogAction.Log(_context, fromGoodsProductionInfo.Id, GoodsLogAction.GoodsActionType.Merge, _user);
                }

                //Изменение ProductionInfo
                var change = GoodsProductionInfoController.ChangeProductionInfo(fromGoodsProductionInfo, toGoodsProductionInfo, _user, _context);

                if (fromGoodsProductionInfo != null)
                {
                    RemoveGoodsProductionInfo(fromOwnerTradeMark, fromPacker, fromGood);
                }

                _dictionary.UpdateGoodsProductionInfoParameter(toGoodsProductionInfo.Id, model.ParameterIds);

                _context.SaveChanges();

                transaction.Commit();

                GoodsProductionInfoController.PublishFull(change.ClassifierInfoFromId, _context);

                GoodsClassifierInfoModel info = new GoodsClassifierInfoModel();
                info.GoodsId = toGood.Id;
                info.GoodKey = toGood.Id.ToString();
                info.PackerId = packer.Id;
                info.OwnerTradeMarkId = ownerTradeMark.Id;

                return info;
            }
        }


        /// <summary>
        /// Сохранить с новым LKCU путем удаления старой и создания новой позиции с перепривязкой
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <param name="tryMode">Сохранять изменения или нет</param>
        /// <returns>Список изменений</returns>
        public GoodsClassifierInfoModel ReCreateClassifier(GoodsClassifierEditorModelJson model, bool tryMode)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                var existGood = _context.Goods.FirstOrDefault(d => d.Id == model.GoodsId);

                if (existGood == null)
                    throw new ApplicationException("Good not found");


                var goodProperty = _dictionary.GetGoodProperty(model);

                //Ищем Good
                Goods good = _dictionary.FindGood(goodProperty);

                if (good != null)
                    throw new ApplicationException("Данный Good уже существует нужно воспользоваться объединением");

                GoodsProductionInfo goodsProductionInfo = null;

                //Нужно добавить, чтобы перенеслись все регисрационные сертификаты, а не только выбранный.
                GoodsClassifierInfoModel info = Add(model, false, out goodsProductionInfo);

                var fromGoodsProductionInfo = _context.GoodsProductionInfo.Single(p => p.Id == model.ProductionInfoId);
                fromGoodsProductionInfo.Comment = model.Comment;

                var ownerTradeMark = _dictionary.FindOrCreateManufacturer(model.OwnerTradeMark);
                var packer = _dictionary.FindOrCreateManufacturer(model.Packer);

                //Удаляем из существующего классификатора
                _context.SaveChanges();
                GoodsLogAction.Log(_context, model.ProductionInfoId, GoodsLogAction.GoodsActionType.Merge, _user);
                GoodsLogAction.Log(_context, goodsProductionInfo.Id, GoodsLogAction.GoodsActionType.Add, _user);

                //Изменение в привязке и классификации
                var change = GoodsProductionInfoController.ChangeProductionInfo(fromGoodsProductionInfo, goodsProductionInfo, _user, _context, true);

                RemoveGoodsProductionInfo(ownerTradeMark, packer, existGood);

                _dictionary.UpdateGoodsProductionInfoParameter(goodsProductionInfo.Id, model.ParameterIds);

                _context.SaveChanges();

                transaction.Commit();

                GoodsProductionInfoController.PublishFull(change.ClassifierInfoFromId, _context);

                return info;

            }
        }

        private void RemoveGoodsProductionInfo(Manufacturer ownerTradeMark, Manufacturer packer, Goods good)
        {
            //Удаляем измененную связку GoodsProductionInfo
            var existsGoodsProductionInfo = _dictionary.FindGoodsProductionInfo(ownerTradeMark, packer, good);

            if (existsGoodsProductionInfo != null)
            {
                existsGoodsProductionInfo.Goods = null;
                _context.GoodsProductionInfo.Remove(existsGoodsProductionInfo);
            }

            //Если у старого Goods больше не осталось других ProductionInfo, то удаляем его
            if (good.GoodsProductionInfo.Count == 0)
            {
                _context.Goods.Remove(good);
            }
        }

        /// <summary>
        /// Метод реализующий добавление новой записи к классификатор
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <param name="tryMode">Сохранять изменения или нет</param>
        /// <returns>Список изменений</returns>
        private GoodsClassifierInfoModel Add(GoodsClassifierEditorModelJson model, bool tryMode, out GoodsProductionInfo goodsProductionInfo)
        {
            var goodProperty = _dictionary.GetGoodProperty(model);

            Goods good = _dictionary.FindGood(goodProperty) ?? _dictionary.CreateGood(goodProperty);

            var ownerTradeMark = _dictionary.FindOrCreateManufacturer(model.OwnerTradeMark);
            var packer = _dictionary.FindOrCreateManufacturer(model.Packer);

            var goodProductionInfo = _dictionary.FindGoodsProductionInfo(ownerTradeMark, packer, good);
            if (goodProductionInfo != null)
            {
                throw new ApplicationException("Такой GoodsProductionInfo уже существует");
            }

            goodProductionInfo = _dictionary.CreateGoodsProductionInfo(ownerTradeMark, packer, good, model.GoodsCategory, model.Used, model.Comment);

            GoodsClassifierInfoModel info = new GoodsClassifierInfoModel
            {
                GoodKey = good.Id.ToString(),
                GoodsId = good.Id,
                GoodsDescription = good.GoodsDescription,
                GoodsTradeName = good.GoodsTradeName,
                OwnerTradeMarkId = ownerTradeMark.Id,
                PackerId = packer.Id,
                GoodsBrand = goodProperty.GoodsBrand

            };

            if (!tryMode)
            {
                _context.SaveChanges();

                _dictionary.UpdateOrCreateGoodsBrandClassification(good.GoodsTradeName, ownerTradeMark, goodProperty.GoodsBrand);

                _dictionary.UpdateGoodsProductionInfoParameter(goodProductionInfo.Id, model.ParameterIds);

                var change = GoodsProductionInfoController.ChangeProductionInfo(null, goodProductionInfo, _user, _context);

                ClassifierInfo classifierInfo = _context.ClassifierInfo.Find(change.ClassifierInfoToId);
                if (classifierInfo != null)
                    classifierInfo.ToRetail = model.ToRetail;

                GoodsLogAction.Log(_context, goodProductionInfo.Id, GoodsLogAction.GoodsActionType.Add, _user);
                _context.SaveChanges();

                info.GoodsId = good.Id;
                info.GoodKey = good.Id.ToString();
                info.OwnerTradeMarkId = ownerTradeMark.Id;
                info.PackerId = packer.Id;
            }

            goodsProductionInfo = goodProductionInfo;
            //info.Good = good;

            return info;
        }

        /// <summary>
        /// Изменить позицию классификатора
        /// </summary>
        /// <param name="model">Входная модель</param>
        /// <param name="tryMode">Сохранять изменения или нет</param>
        /// <returns>Список изменений</returns>
        public GoodsClassifierInfoModel ChangeClassifier(GoodsClassifierEditorModelJson model, bool tryMode)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                GoodsClassifierInfoModel info = new GoodsClassifierInfoModel();

                var good = _context.Goods.FirstOrDefault(d => d.Id == model.GoodsId);

                //Изменяемый good не найден
                if (good == null)
                    throw new ApplicationException(string.Format("Не найден изменяемый GoodsId = {0}", model.GoodsId));

                //Получаем описание всех параметров Good
                var goodProperty = _dictionary.GetGoodProperty(model);

                //Ищем Good по новым параметрам
                var goodAlreadyExists = _dictionary.FindGood(goodProperty);

                //С тамким характеристиками уже есть другой Drug
                if (goodAlreadyExists != null && goodAlreadyExists.Id != model.GoodsId)
                    throw new ApplicationException("Найден другой Good с такими же характеристиками");

                good.GoodsTradeName = goodProperty.GoodsTradeName;
                good.GoodsDescription = goodProperty.GoodsDescription;

                var ownerTradeMark = _dictionary.FindOrCreateManufacturer(model.OwnerTradeMark);

                var packer = _dictionary.FindOrCreateManufacturer(model.Packer);

                //Исходное ProductionInfo
                var goodsProductionInfo = _context.GoodsProductionInfo.First(p => p.Id == model.ProductionInfoId);

                //Новое ProductionInfo
                var existProductionInfo = _dictionary.FindGoodsProductionInfo(ownerTradeMark, packer, good);

                //Проверка нового ProductionInfo
                if (existProductionInfo != null && goodsProductionInfo.Id != existProductionInfo.Id)
                    throw new ApplicationException("У выбранного Good уже есть такие производитель и упаковщик");

                //Делаем первоночальную копию перед изменениями
                var productionInfoFrom = goodsProductionInfo.Copy();

                goodsProductionInfo.OwnerTradeMark = ownerTradeMark;
                goodsProductionInfo.Packer = packer;
                goodsProductionInfo.Used = model.Used;
                goodsProductionInfo.Comment = model.Comment;

                if (model.GoodsCategory.Id > 0)
                {
                    //Изменяем категорию
                    goodsProductionInfo.GoodsCategoryId = model.GoodsCategory.Id;
                }
                else if (model.GoodsCategory.Id == 0)
                {
                    //Изменяем категорию
                    goodsProductionInfo.GoodsCategoryId = null;
                }

                var classifier = _context.ClassifierInfo.FirstOrDefault(t => t.GoodsProductionInfoId == goodsProductionInfo.Id);
                if (classifier == null)
                    throw new ApplicationException("Классификатор для доп. материала с id " + goodsProductionInfo.Id + " не обнаружен!");

                classifier.ToRetail = model.ToRetail;

                //Сохраняем изменения в том числе, чтобы для нового OwnerTradeMarkId или PackerId повяились реальные Id
                GoodsLogAction.Log(_context, goodsProductionInfo.Id, GoodsLogAction.GoodsActionType.Change, _user);

                _context.SaveChanges();

                _dictionary.UpdateOrCreateGoodsBrandClassification(good.GoodsTradeName, ownerTradeMark,
                    goodProperty.GoodsBrand);

                var change = GoodsProductionInfoController.ChangeProductionInfo(productionInfoFrom, goodsProductionInfo, _user,
                     _context);

                _dictionary.UpdateGoodsProductionInfoParameter(goodsProductionInfo.Id, model.ParameterIds);

                _context.SaveChanges();

                transaction.Commit();

                GoodsProductionInfoController.PublishFull(change.ClassifierInfoFromId, _context);

                info.GoodsId = good.Id;
                info.GoodKey = good.Id.ToString();
                info.PackerId = packer.Id;
                info.OwnerTradeMarkId = ownerTradeMark.Id;

                return info;
            }
        }

        /// <summary>
        /// Добавить в классификатор
        /// </summary>
        /// <param name="model">Входная модель</param>
        /// <param name="tryMode">Сохранять изменения или нет</param>
        /// <returns>Список изменений</returns>
        public GoodsClassifierInfoModel AddClassifier(GoodsClassifierEditorModelJson model, bool tryMode)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                GoodsProductionInfo goodsProductionInfo = null;

                var result = Add(model, tryMode, out goodsProductionInfo);

                transaction.Commit();

                return result;
            }
        }
    }
}
