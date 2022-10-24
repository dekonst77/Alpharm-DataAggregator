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
    public class ATCWhoController : BaseController
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
            var atc = _context.Database.SqlQuery<AtcWhoLine>(@"  SELECT   * FROM Classifier.ATCWhoLine").ToList();


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
                var atc1 = AddAtc(null, 1, value.Atc1.Value, value.Atc1.Description);
                var atc2 = AddAtc(atc1, 2, value.Atc2.Value, value.Atc2.Description);
                var atc3 = AddAtc(atc2, 3, value.Atc3.Value, value.Atc3.Description);
                var atc4 = AddAtc(atc3, 4, value.Atc4.Value, value.Atc4.Description);
                var atc5 = AddAtc(atc4, 5, value.Atc5.Value, value.Atc5.Description);

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
        /// <summary>
        /// Создаем новую запись если её еще не было или возвращаем существующую
        /// </summary>
        /// <param name="parent">Элемент уровня выше</param>
        /// <param name="level">Уровень добавляемого элемента</param>
        /// <param name="value">Код</param>
        /// <param name="description">Описание</param>
        /// <returns>Элемент</returns>
        private ATCWho AddAtc(ATCWho parent, int level, string value, string description)
        {
            //Сначал найдем существует ли такая запись?

            ATCWho atcExist = null;

            atcExist = SearchAtc(parent, level, value, description);

            //Если такая запись уже существует то возвращаем её
            if (atcExist != null)
                return atcExist;

            

            //Проверим уникальность Value

            if (_context.ATCWho.Any(a => string.Equals(a.Value, value)))
            {
                throw new ApplicationException("Код для ATC уже существует у другой записи");
            }

            //Проверим уникальность Value и Description
            if (_context.ATCWho.Any(a => string.Equals(a.Value, value) && string.Equals(a.Description, description)))
            {
                throw new ApplicationException("ATC1 уже существует");
            }

            //Если записи не существует добавляем

            var atc = new ATCWho
            {
                Parent = parent,
                ValueLevel = level,
                Value = value,
                Description = description
            };


            _context.ATCWho.Add(atc);

            return atc;



        }

        private ATCWho SearchAtc(ATCWho parent, int level, string value, string description)
        {
            //Если родитель новый
            if (parent != null && parent.Id == 0)
                return null;


            //Есть родитель
            if (parent != null && parent.Id > 0)
            {
                return _context.ATCWho.SingleOrDefault(a => a.ParentId == parent.Id &&
                                                                a.ValueLevel == level &&
                                                                string.Equals(a.Value, value) &&
                                                                string.Equals(a.Description, description));
            }

            //Родитель не задан
            if (parent == null)
            {
                return _context.ATCWho.SingleOrDefault(a => a.ParentId == null &&
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
                ChangeAtc(value.Atc5);

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

            var atcEntity = _context.ATCWho.Single(a => a.Id == atc.Id);

            //Проверим уникальность Value

            if (_context.ATCWho.Any(a => string.Equals(a.Value, atc.Value) && a.Id != atc.Id))
            {
                throw new ApplicationException("Код для ATC уже существует у другой записи");
            }

            atcEntity.Value = atc.Value;
            
            //Проверим уникальность Value и Description
            if (_context.ATCWho.Any(a => string.Equals(a.Value, atc.Value) && string.Equals(a.Description, atc.Description) && a.Id != atc.Id))
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
                if (value.Atc5.Id != null)
                {
                    DeleteAtc(value.Atc5);
                }
                else if (value.Atc4.Id != null)
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
            var atcEntity = _context.ATCWho.Single(a => a.Id == atc.Id);
            _context.ATCWho.Remove(atcEntity);

        }
    }
}