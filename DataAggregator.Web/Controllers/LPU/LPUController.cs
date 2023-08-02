using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.LPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace DataAggregator.Web.Controllers.LPU
{

    public class LPUController : BaseController
    {
        private GSContext _context;

        private static readonly object LockObject = new object();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GSContext(APP);
        }

        ~LPUController()
        {
            _context.Dispose();
        }

        [Authorize(Roles = "LPU_view")]
        public ActionResult Get_LPU_Status()
        {
            try
            {
                var Status = _context.OrganizationStatusView.OrderBy(t=> t.Name).ToList();
                //   var Dep = _context.LPU_Departments.Where(d => d.LPUId_Id == Id).ToList();

                return ReturnData(Status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }


        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult LoadLPUByPointId(int PointId)
        {
            try
            {  
                var lpu = _context.LPUView.Where(l => l.PointId == PointId && (l.ActualId<1 || l.ActualId == null)&& (l.IsDepartment < 1 || l.IsDepartment == null)).Select(LPUModel.Create).ToList();
                return ReturnData(lpu);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }

        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult GetDepartmentSource()
        {
            try
            {
                var department = _context.Department.ToList();
                return ReturnData(department);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }


        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult LoadLPU(LPUFilter filter)
        {
            try
            {
                var lpuenum = _context.LPUView.Where(l => 1 == 1);

                if (!string.IsNullOrEmpty(filter.Address))
                {
                    lpuenum = lpuenum.Where(l => l.Address.Contains(filter.Address));
                }
                if (filter.IsDepartment != null)
                {
                    if (filter.IsDepartment == true)
                    {
                        lpuenum = lpuenum.Where(l => l.IsDepartment < 1 || l.IsDepartment == null);
                    }
                }
                if (filter.IsNullType != null)
                {
                    if (filter.IsNullType == true)
                    {
                        lpuenum = lpuenum.Where(l => l.TypeId < 1 || l.TypeId == null);
                    }
                }
                if (filter.TypeId.HasValue)
                {
                    lpuenum = lpuenum.Where(l => l.TypeId == filter.TypeId.Value);
                }
                if (filter.KindId.HasValue)
                {
                    lpuenum = lpuenum.Where(l => l.KindId == filter.KindId.Value);
                }

                if (filter.IsNullKind != null)
                {
                    if (filter.IsNullKind == true)
                    {
                        lpuenum = lpuenum.Where(l => l.KindId < 1 || l.KindId == null);
                    }
                }

                if (filter.IsActual != null)
                {
                    if (filter.IsActual == true)
                    {
                        lpuenum = lpuenum.Where(l => l.ActualId < 1 || l.ActualId == null);
                    }
                }


                if (!string.IsNullOrEmpty(filter.BrickId))
                {
                    lpuenum = lpuenum.Where(l => l.BricksId == filter.BrickId);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    lpuenum = lpuenum.Where(l => l.EntityINN.Contains(filter.Name) || l.EntityName.Contains(filter.Name));
                }

                if (!string.IsNullOrEmpty(filter.Status))
                {
                    lpuenum = lpuenum.Where(l => l.Status.Contains(filter.Status));
                }
                if (!string.IsNullOrEmpty(filter.Address_region))
                {
                    lpuenum = lpuenum.Where(l => l.Address_region.Contains(filter.Address_region));
                }
                if (!string.IsNullOrEmpty(filter.Address_city))
                {
                    lpuenum = lpuenum.Where(l => l.Address_city.Contains(filter.Address_city));
                }


                if (filter.PointId.HasValue)
                {
                    lpuenum = lpuenum.Where(l => l.PointId == filter.PointId.Value);
                }

                if (filter.LPUId.HasValue)
                {
                    lpuenum = lpuenum.Where(l => l.LPUId == filter.LPUId.Value);
                }

                var lpu = lpuenum.Select(LPUModel.Create).ToList();
               
                return ReturnData(lpu);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }


        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult Save(ICollection<LPUModel> lpumodels)
        {
            try
            {
                foreach (var lpumodel in lpumodels)
                {
                    var lpu = _context.LPU.SingleOrDefault(l => l.Id == lpumodel.LPUId);

                    if (lpu == null)
                        throw new Exception($"Не найден LPU с Id {lpumodel.LPUId}");
                    //Обноавление LPU

                    lpu.Comment = lpumodel.Comment;

                    //Обновление point
                    lpu.LPUPoint.BricksId = lpumodel.BricksId;
                    lpu.ActualId = lpumodel.ActualId;
                    lpu.LPUPoint.Address_street = lpumodel.Address_street;
                    lpu.LPUPoint.Address_comment = lpumodel.Address_comment;
                    lpu.LPUPoint.Address_float = lpumodel.Address_float;
                    lpu.LPUPoint.Address_room = lpumodel.Address_room;
                    lpu.LPUPoint.Address_index = lpumodel.Address_index;
                    lpu.LPUPoint.Address_region = lpumodel.Address_region;
                    lpu.LPUPoint.Address_city = lpumodel.Address_city;
                    lpu.LPUPoint.Address_room_area = lpumodel.Address_room_area;
                    lpu.LPUPoint.Address_koor = lpumodel.Address_koor;
                    lpu.DepartmentId = lpumodel.DepartmentId;
                    lpu.Department = lpumodel.Department;
                    lpu.TypeId = lpumodel.TypeId;
                    lpu.KindId = lpumodel.KindId;
                }

                //Сохранение изменений
                _context.SaveChanges();
                return ReturnData(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }

        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult AddLPU(DataAggregator.Domain.Model.LPU.LPUView lpumodels)
        {
            try
            {
                _context.LPU_Add(lpumodels);
                return ReturnData(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }


        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult Get_LPU_Department_by_LPUId(long Id)
        {
            try
            {
                var Dep = _context.LPUView.Where(l => l.ParentId==Id ).ToList();
             //   var Dep = _context.LPU_Departments.Where(d => d.LPUId_Id == Id).ToList();
              
                return ReturnData(Dep);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }

        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult Add_LPU_Department_by_LPUId(int id, string value)
        {
            try
            {
                var lp = _context.LPU.Where(d => d.Id == id).ToList().FirstOrDefault();
                var source = new Domain.Model.LPU.LPU()
                    {
                     ParentId=id ,
                     Department= value,
                     OrganizationId= lp.OrganizationId,
                     LPUPointId=lp.LPUPointId    

                };

                _context.LPU.Add(source);    
                _context.SaveChanges();

                var Dep = _context.LPUView.Where(l => l.ParentId == id).ToList();
            //    var Dep = _context.LPU_Departments.Where(d => d.LPUId_Id == id).ToList();

                return ReturnData(Dep);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }


        
          [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult Edit_LPU_Department_by_LPUId(LPUModel  array)
        {
            try
            {
                var upd = _context.LPU.Where(w => w.Id == array.LPUId && w.ParentId>0).FirstOrDefault();
                if (upd == null)
                    throw new Exception($"Не найден LPU с Id {array.LPUId}");
                //Обноавление LPU
                upd.Department = array.Department;
                _context.SaveChanges();
              
                return null;
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
        [Authorize(Roles = "LPU_view")]
        public ActionResult Remove_LPU_Department_by_LPUId(long id)
        {
            var Dep = _context.LPU.Where(d => d.Id == id && d.ParentId>0).FirstOrDefault();
          
            if (Dep == null)
                throw new ApplicationException("Source not found");
            var Dep2 = _context.LPUView.Where(l => l.ParentId == Dep.ParentId).ToList();
            try
            {
              
                 _context.LPU.Remove(Dep);
                _context.SaveChanges();
              
               
                return ReturnData(Dep2);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }

        }

        [HttpPost]
        public ActionResult Merge(ICollection<int> LPUIds, int Actual_Id)
        {
            try
            {
                var user = User.Identity.GetUserId();
              
              
                _context.LPU_Merge(Actual_Id, LPUIds.ToList(), user);
                return ReturnData(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }

        }

























        [HttpPost]
        public FileResult ToExcel(ICollection<int> ids)
        {
            _context.Database.CommandTimeout = 0;

            var ret = _context.LPUView.Where(l => ids.Contains(l.LPUId)).ToList();

            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("ЛПУ", 1, 1, ret, true, true, null);

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "Шаблон.xlsx");
        }

        [HttpPost]
        public ActionResult FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            try
            {
                if (uploads == null || !uploads.Any())
                    throw new ApplicationException("uploads not set");

                var file = uploads.First();
                string filename = @"\\s-sql2\Upload\LPU_up.xlsx";
                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                file.SaveAs(filename);

                _context.GS_from_Excel(@"S:\Upload\ГС_up.xlsx");
                return ReturnData(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }






    }
}