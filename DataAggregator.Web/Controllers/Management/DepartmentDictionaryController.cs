using DataAggregator.Web.Models.Memberships;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;

namespace DataAggregator.Web.Controllers
{
    [Authorize(Roles = "Admin, UserManager")]
    public sealed class DepartmentDictionaryController : BaseController
    {
        [HttpGet]
        public ViewResult Index()
        {
            return View("../Management/DepartmentDictionary");
        }

        /// <summary>
        /// Получить подразделения
        /// </summary>
        [HttpGet]
        public async Task<JsonNetResult> GetDepartments()
        {
            return new JsonNetResult(await GetDepartmentModels());
        }

        /// <summary>
        /// Загрузка подразделений
        /// </summary>
        /// <returns></returns>
        private static async Task<List<DepartmentModel>> GetDepartmentModels()
        {
            using (var context = new ApplicationDbContext())
            {
                List<Department> items = await context.Departments.Include(s => s.Manager).ToListAsync();

                return ModelMapper.Mapper.Map<List<Department>, List<DepartmentModel>>(items);
            }
        }

        [HttpPost]
        public ActionResult DeleteRow(int id)
        {
            try
            {
                DeleteRowInDb(id);

                //Возвращаем успешный результат
                return new JsonNetResult(true);
            }
            catch (InvalidOperationException ex)
            {
                return ErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Сохраняем изменения
        /// </summary>
        [HttpPost]
        public ActionResult SaveRow(DepartmentModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            try
            {
                ChangeDepartmentModel(model);

                //Возвращаем успешный результат
                return new JsonNetResult(model);
            }
            catch (InvalidOperationException ex)
            {
                return ErrorMessage(ex.Message);
            }
        }

        private static void DeleteRowInDb(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                CheckIfAnyUserPresent(id, context);
                Department item = context.Departments.Single(cr => cr.Id == id);

                context.Departments.Remove(item);

                context.SaveChanges();
            }
        }

        private static void ChangeDepartmentModel(DepartmentModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                // ищем идентичные подразделения
                CheckSimilarItem(model, context);

                Department item;

                if (model.Id.HasValue)
                    item = context.Departments.First(cr => cr.Id == model.Id);
                else
                {
                    item = new Department();
                    context.Departments.Add(item);
                }
                throw  new Exception("обновили и теперь надо найти проблему");
                //AutoMapper.Mapper.Map(model, item, opt => opt.ConfigureMap( MemberList.None));

              /*context.SaveChanges();

                model.Id = item.Id;*/
            }
        }

        private static void CheckIfAnyUserPresent(int id, ApplicationDbContext context)
        {
            List<string> users = context.Users
                .Where(s => s.DepartmentId == id)
                .AsEnumerable()
                .Select(s => s.FullName)
                .ToList();

            if (users.Any())
                throw new InvalidOperationException(string.Format("Пользователи существуют в данном подразделении: {0}",
                    string.Join(", ", users)));
        }

        private static void CheckSimilarItem(DepartmentModel model, ApplicationDbContext context)
        {
            List<string> identicalNames = context.Departments
                .Where(s =>
                    s.Id != model.Id &&
                    (
                        s.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase) ||
                        s.ShortName.Equals(model.ShortName, StringComparison.OrdinalIgnoreCase)
                    )
                )
                .AsEnumerable()
                .Select(s => string.Format("{0} ({1})", s.Name, s.ShortName))
                .ToList();

            if (identicalNames.Any())
                throw new InvalidOperationException(string.Format("Идентичные подразделения существуют: {0}",
                    string.Join(", ", identicalNames)));
        }

        private static ActionResult ErrorMessage(string errorMessage)
        {
            return new JsonNetResult(new { isError = true, errorMessage });
        }
    }
}