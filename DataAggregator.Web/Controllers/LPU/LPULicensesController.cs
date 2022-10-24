using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.LPU;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.LPU
{
    public class LPULicensesController : BaseController
    {
        private GSContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GSContext(APP);
        }

        ~LPULicensesController()
        {
            _context.Dispose();
        }


        [HttpPost]
        public ActionResult LoadLPULicenses(LPUFilter filter)
        {
            try
            {

                var lpuenum = _context.LPULicensesView.Where(l => 1 == 1);

                if (!string.IsNullOrEmpty(filter.Address))
                {
                    lpuenum = lpuenum.Where(l => l.Address.Contains(filter.Address));
                }

                if (!string.IsNullOrEmpty(filter.BrickId))
                {
                    lpuenum = lpuenum.Where(l => l.BricksId == filter.BrickId);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    lpuenum = lpuenum.Where(l => l.EntityINN.Contains(filter.Name) || l.EntityName.Contains(filter.Name));
                }

                if (filter.PointId.HasValue)
                {
                    lpuenum = lpuenum.Where(l => l.LPUPointId == filter.PointId.Value);
                }

                if (filter.LPUId.HasValue)
                {
                    lpuenum = lpuenum.Where(l => l.LPUPointId == filter.LPUId.Value);
                }

                var lpu = lpuenum.Take(3000).Select(LPULicensesModel.Create).ToList(); 
                return ReturnData(lpu);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteLPULicenses(int id)
        {
            try
            {


                var view = _context.LPULicensesView.FirstOrDefault(l => l.Id == id && l.manualAdd);

                if (view != null)
                {
                    _context.LPULicensesView.Remove(view);
                    await _context.SaveChangesAsync();

                    return ReturnData(null);
                }


                return ReturnData(LPULicensesModel.Create(view));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateLPULicenses(LPULicensesModel model)
        {
            try
            {
                var view = model.ConvertToView();
                view.manualAdd = true;
                _context.LPULicensesView.Add(view);
                await _context.SaveChangesAsync();

                return ReturnData(LPULicensesModel.Create(view));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}