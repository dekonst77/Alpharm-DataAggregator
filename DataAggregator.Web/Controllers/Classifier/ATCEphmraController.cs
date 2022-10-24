using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Web.Models.Classifier;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{

    [Authorize(Roles = "SBoss")]
    public class ATCEphmraController : BaseController
    {
        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        [HttpPost]
        public ActionResult Load()
        {
            var atc = _context.Database.SqlQuery<AtcEphmraLine>(@"SELECT   * FROM Classifier.ATCEphmraLine").ToList();


            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = atc
            };
        }

        [HttpPost]
        public ActionResult Add(AtcGroupModel value)
        {

            dynamic result = new ExpandoObject();
            result.Success = true;

            try
            {
                //Создаем ATCEphrma
                CreateAtcEphrma(value);

                _context.SaveChanges();

                result.Success = true;
            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = e.Message;
                result.Success = false;
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }

        private void CreateAtcEphrma(AtcGroupModel value)
        {
            if (string.IsNullOrEmpty(value.Atc1.Value) || string.IsNullOrEmpty(value.Atc1.Description))
                throw new ApplicationException("Верхний уровень должен быть заполнен");

            var atc1 = AddAtc(null, 1, value.Atc1.Value, value.Atc1.Description);

            if (string.IsNullOrEmpty(value.Atc2.Value) || string.IsNullOrEmpty(value.Atc2.Description))
                return;

            var atc2 = AddAtc(atc1, 2, value.Atc2.Value, value.Atc2.Description);

            if (string.IsNullOrEmpty(value.Atc3.Value) || string.IsNullOrEmpty(value.Atc3.Description))
                return;

            var atc3 = AddAtc(atc2, 3, value.Atc3.Value, value.Atc3.Description);

            if (string.IsNullOrEmpty(value.Atc4.Value) || string.IsNullOrEmpty(value.Atc4.Description))
                return;

            var atc4 = AddAtc(atc3, 4, value.Atc4.Value, value.Atc4.Description);
        }

        /// <summary>
        /// Создаем новую запись если её еще не было или возвращаем существующую
        /// </summary>
        /// <param name="parent">Элемент уровня выше</param>
        /// <param name="level">Уровень добавляемого элемента</param>
        /// <param name="value">Код</param>
        /// <param name="description">Описание</param>
        /// <returns>Элемент</returns>
        private ATCEphmra AddAtc(ATCEphmra parent, int level, string value, string description)
        {
            //Сначал найдем существует ли такая запись?

            ATCEphmra atcExist = null;

            atcExist = SearchAtc(parent, level, value, description);

            //Если такая запись уже существует то возвращаем её
            if (atcExist != null)
                return atcExist;



            //Проверим уникальность Value

            if (_context.ATCEphmra.Any(a => string.Equals(a.Value, value)))
            {
                throw new ApplicationException("Код для ATCEphmra уже существует у другой записи");
            }

            //Проверим уникальность Value и Description
            if (_context.ATCEphmra.Any(a => string.Equals(a.Value, value) && string.Equals(a.Description, description)))
            {
                throw new ApplicationException("ATCEphmra уже существует");
            }

            //Если записи не существует добавляем

            var atc = new ATCEphmra
            {
                Parent = parent,
                ValueLevel = level,
                Value = value,
                Description = description,
                IsUse = true
            };


            _context.ATCEphmra.Add(atc);

            return atc;



        }

        private ATCEphmra SearchAtc(ATCEphmra parent, int level, string value, string description)
        {
            //Если родитель новый
            if (parent != null && parent.Id == 0)
                return null;


            //Есть родитель
            if (parent != null && parent.Id > 0)
            {
                return _context.ATCEphmra.SingleOrDefault(a => a.ParentId == parent.Id &&
                                                                a.ValueLevel == level &&
                                                                string.Equals(a.Value, value) &&
                                                                string.Equals(a.Description, description));
            }

            //Родитель не задан
            if (parent == null)
            {
                return _context.ATCEphmra.SingleOrDefault(a => a.ParentId == null &&
                                                                a.ValueLevel == level &&
                                                                string.Equals(a.Value, value) &&
                                                                string.Equals(a.Description, description));
            }


            return null;

        }

        [HttpPost]
        public ActionResult Change(AtcGroupModel value)
        {
            dynamic result = new ExpandoObject();

            try
            {
                ChangeAtc(value.Atc1);
                ChangeAtc(value.Atc2);
                ChangeAtc(value.Atc3);
                ChangeAtc(value.Atc4);

                _context.SaveChanges();

                result.Success = true;

            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = e.Message;
                result.Success = false;
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }

        //Изменить описание ATC
        private void ChangeAtc(AtcModel atc)
        {
            if(atc.Id == null)
                return;

            var atcEntity = _context.ATCEphmra.Single(a => a.Id == atc.Id);

            //Проверим уникальность Value

            if (_context.ATCEphmra.Any(a => string.Equals(a.Value, atc.Value) && a.Id != atc.Id))
            {
                throw new ApplicationException("Код для ATC уже существует у другой записи");
            }

            atcEntity.Value = atc.Value;

            //Проверим уникальность Value и Description
            if (_context.ATCEphmra.Any(a => string.Equals(a.Value, atc.Value) && string.Equals(a.Description, atc.Description) && a.Id != atc.Id))
            {
                throw new ApplicationException("ATC1 уже существует");
            }

            atcEntity.Description = atc.Description;
        }

        //Удаляем 
        [HttpPost]
        public ActionResult Delete(AtcGroupModel value)
        {
            dynamic result = new ExpandoObject();

            try
            {
                //Удаляем последний выбранный
                if (value.Atc4.Id != null)
                {
                    DeleteAtc(value.Atc4);
                }
                else if (value.Atc3.Id != null)
                {
                    DeleteAtc(value.Atc3);
                }
                else if (value.Atc2.Id != null)
                {
                    DeleteAtc(value.Atc2);
                }
                else if (value.Atc1.Id != null)
                {
                    DeleteAtc(value.Atc1);
                }

                _context.SaveChanges();
                result.Success = true;
            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = "Нельзя удалить выбранный элемент";
                result.Success = false;
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }

        //Удаляем выбранную АТС
        private void DeleteAtc(AtcModel atc)
        {
            var atcEntity = _context.ATCEphmra.Single(a => a.Id == atc.Id);
            _context.ATCEphmra.Remove(atcEntity);

        }
    }
}