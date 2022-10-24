using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.LPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace DataAggregator.Web.Controllers.LPU
{
    public class LPUPointController : BaseController
    {
        private GSContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GSContext(APP);
        }

        ~LPUPointController()
        {
            _context.Dispose();
        }

        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult LoadLPUPoint(LPUFilter filter)
        {
            try
            {
                var lpuenum = _context.LPUPointView.Where(l => 1 == 1);

                if (!string.IsNullOrEmpty(filter.Address))
                {
                    if (filter.Address == "-")
                    {
                        lpuenum = lpuenum.Where(l => l.Address==null || l.Address =="" );
                    }
                    else lpuenum = lpuenum.Where(l => l.Address.Contains(filter.Address));
                }

                
                 if (!string.IsNullOrEmpty(filter.City))
                {
                    if (filter.City == "-")
                    {
                        lpuenum = lpuenum.Where(l => l.Address_city == null || l.Address_city == "");
                    }
                    else lpuenum = lpuenum.Where(l => l.Address_city.Contains(filter.City));

                }
                if (filter.IsGPS!=null)
                {
                    if (filter.IsGPS == true) {
                        lpuenum = lpuenum.Where(l => l.Address_koor =="" || l.Address_koor == null);
                    }                
                }
                if (filter.IsLPU != null)
                {
                    if (filter.IsLPU == true)
                    {
                        lpuenum = lpuenum.Where(l => l.LPUcnt <1 || l.LPUcnt == null);
                    }
                }
                if (!string.IsNullOrEmpty(filter.SF))
                {
                    if (filter.SF == "-")
                    {
                        lpuenum = lpuenum.Where(l => l.Address_region == null || l.Address_region == "");
                    }
                    else lpuenum = lpuenum.Where(l => l.Address_region.Contains(filter.SF));


                }
                if (filter.Double != null)
                {
                    if (filter.Double == true)
                    {
                        lpuenum = lpuenum.Where(l => l.DoubleMinPoint > 1 && l.DoubleMinPoint != null);
                    }
                }





                if (!string.IsNullOrEmpty(filter.BrickId))
                {
                    if (filter.BrickId == "-")
                    {
                        lpuenum = lpuenum.Where(l => l.BricksId == null || l.BricksId == "");
                    }
                    else lpuenum = lpuenum.Where(l => l.BricksId == filter.BrickId);


                }

                if (filter.PointId.HasValue)
                {
                    lpuenum = lpuenum.Where(l => l.PointId == filter.PointId.Value);
                }

                var lpu = lpuenum.Select(LPUPointModel.Create).ToList();
                return ReturnData(lpu);
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

            var ret = _context.LPUPointView.Where(l => ids.Contains(l.PointId)).ToList();

            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("LPUPoint_Grid", 1, 1, ret, true, true, null);

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "Шаблон.xlsx");
        }


        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {

            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            var guid = Guid.NewGuid();
            var userName = User.Identity.GetUserName();//.GetUserId();
            var userId = User.Identity.GetUserId();


            string path = @"\\s-sql2\Upload\";

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string filepath = $"{path}\\LPUPoint{guid}{userName}.xlsx";

            try
            {     
                file.SaveAs(filepath);

                var _context = new GSContext(APP);
                _context.LpuPoint_FromExcel(filepath, userId);

                return ReturnData(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
            finally
            {
                if (System.IO.File.Exists(filepath))
                    System.IO.File.Delete(filepath);
            }
        }


        [HttpPost]
        [Authorize(Roles = "LPU_view")]
        public ActionResult Save(ICollection<LPUPointModel> lpupointmodels)
        {
            try
            {
                foreach (var lpupointmodel in lpupointmodels)
                {
                    var point = _context.LPUPoint.SingleOrDefault(l => l.Id == lpupointmodel.PointId);

                    if (point == null)
                        throw new Exception($"Не найден LPU_Point с Id {lpupointmodel.PointId}");

                    //Обновление point
                    point.BricksId = lpupointmodel.BricksId;
                    point.Address_street = lpupointmodel.Address_street;
                    point.Address_comment = lpupointmodel.Address_comment;
                    point.Address_float = lpupointmodel.Address_float;
                    point.Address_room = lpupointmodel.Address_room;
                    point.Address_index = lpupointmodel.Address_index;
                    point.Address_region = lpupointmodel.Address_region;
                    point.Address_city = lpupointmodel.Address_city;
                    point.Address_room_area = lpupointmodel.Address_room_area;
                    point.Address_koor = lpupointmodel.Address_koor;
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
        public ActionResult Merge(ICollection<int> LPUPointIds)
        {
            try
            {
                var user = User.Identity.GetUserId();
                int LPUpoint_min = LPUPointIds.Min();
                LPUPointIds.Remove(LPUpoint_min);
                _context.LPUPoint_Merge(LPUpoint_min, LPUPointIds.ToList(), user);
                return ReturnData(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }

        }
    }
}