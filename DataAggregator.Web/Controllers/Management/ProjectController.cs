using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web.Mvc;

using DataAggregator.Domain.Model.Project;



namespace DataAggregator.Web.Controllers.Management
{
    [Authorize(Roles = "Project")]
    public class ProjectController : BaseController
    {
        [HttpPost]
        public ActionResult ProjectList()
        {
            try
            {
                var _context = new DataAggregatorContext(APP);

                ViewBag.Project = _context.Project.OrderByDescending(o => o.Id).ToList();
                ViewBag.ProjectType = _context.ProjectType.OrderBy(o => o.Id).ToList();
                ViewBag.Users = _context.UserViewAll.OrderBy(o => o.FullName).ToList();
                ViewBag.StepStatus = _context.StepStatus.OrderBy(o => o.Id).ToList();
                ViewBag.StepTemplate = _context.StepTemplate.OrderBy(o => o.Id).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult Project(int ProjectId)
        {
            try
            {
                var _context = new DataAggregatorContext(APP);

                ViewBag.Project = _context.Project.Where(w=>w.Id== ProjectId).Single();
                 var Steps= _context.Steps.Where(w => w.ProjectId == ProjectId).OrderBy(o => o.orderby).ToList();/*.Select(S=>new StepsEx() {
                     Id = S.Id,
                orderby = S.orderby,
                ProjectId = S.ProjectId,
                StepTemplateId = S.StepTemplateId,
                DateBeginPlan = S.DateBeginPlan,
                DateEndPlan = S.DateEndPlan,
                DateBeginReal = S.DateBeginReal,
                DateEndReal = S.DateEndReal,
                DateDay = S.DateDay,
                StepManagerId = S.StepManagerId,
                StepStatusId = S.StepStatusId
            }).ToList();*/
                foreach (var s_item in Steps)
                {
                    s_item.HHGet();
                }
                ViewBag.Step = Steps;
                ViewBag.History = _context.History.Where(w => w.ProjectId == ProjectId).OrderByDescending(o => o.Id).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult ProjectNew(int ProjectIdParent)
        {
            try
            {
                var _context = new DataAggregatorContext(APP);

                if (ProjectIdParent == 0)
                {
                    int TypeId = _context.ProjectType.First().Id;
                    var newProject = _context.Project.Add(new Domain.Model.Project.Project()
                    {
                        Name = "новый",
                        PeriodYM = DateTime.Now.Year * 100 + DateTime.Now.Month,
                        ProjectManagerId = User.Identity.GetUserId(),
                        TypeId = TypeId
                    });
                    _context.SaveChanges();
                    ViewBag.NewRow = newProject;
                    ViewBag.Success = true;
                }

                if (ProjectIdParent > 0)
                {
                    //тут копирование уже имеющегося проекта + 1 период к нему.
                }
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult ProjectSave(List<Project> array)
        {
            try
            {
                var _context = new DataAggregatorContext(APP);

                foreach(var prj in array)
                {
                    var upd = _context.Project.Where(w => w.Id == prj.Id).Single();
                    upd.Name = prj.Name;
                    upd.PeriodYM = prj.PeriodYM;
                    upd.ProjectManagerId = prj.ProjectManagerId;
                    upd.TypeId = prj.TypeId;
                }
                _context.SaveChanges();
                ViewBag.Success = true;
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [HttpPost]
        public ActionResult StepNew(int ProjectId)
        {
            try
            {
                var _context = new DataAggregatorContext(APP);

                byte orderby = (byte)(_context.Steps.Where(w => w.ProjectId == ProjectId).Count() + 1);
                int StepTemplateId = _context.StepTemplate.First().Id;
                byte StepStatusId = _context.StepStatus.Min(g => g.Id);
                var newStep = _context.Steps.Add(new Domain.Model.Project.Steps()
                {
                    DateBeginPlan = DateTime.Now,
                    DateBeginReal = null,
                    DateDay = 0,
                    DateEndPlan = DateTime.Now,
                    DateEndReal = null,
                    orderby = orderby,
                    ProjectId = ProjectId,
                    StepManagerId = User.Identity.GetUserId(),
                    StepStatusId = StepStatusId,
                    StepTemplateId = StepTemplateId
                });
                _context.SaveChanges();
                ViewBag.NewRow = newStep;
                ViewBag.Success = true;



                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult StepSave(List<Steps> array)
        {
            try
            {
                var _context = new DataAggregatorContext(APP);

                foreach (var stp in array)
                {
                    stp.HHSet();
                    var upd = _context.Steps.Where(w => w.Id == stp.Id).Single();
                    upd.DateBeginPlan = stp.DateBeginPlan;
                    upd.DateBeginReal = stp.DateBeginReal;
                    upd.DateDay = stp.DateDay;
                    upd.DateEndPlan = stp.DateEndPlan;
                    upd.DateEndReal = stp.DateEndReal;
                    upd.orderby = stp.orderby;
                    upd.StepManagerId = stp.StepManagerId;
                    upd.StepStatusId = stp.StepStatusId;
                    upd.StepTemplateId = stp.StepTemplateId;
                }
                _context.SaveChanges();
                ViewBag.Success = true;
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult StepDelete(List<Steps> array)
        {
            try
            {
                var _context = new DataAggregatorContext(APP);

                foreach (var stp in array)
                {
                    var del = _context.Steps.Where(w => w.Id == stp.Id).Single();
                    _context.Steps.Remove(del);
                }
                _context.SaveChanges();
                ViewBag.Success = true;
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
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