using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Distr;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.IO.Compression;
using Kendo.Mvc.Extensions;

namespace DataAggregator.Web.Controllers.DistrRep
{
    [Authorize(Roles = "DistrRep_Main")]
    public class DistrRepController : BaseController
    {
        private readonly DistrContext _context;
        public DistrRepController()
        {
            _context = new DistrContext(APP);
        }


        ~DistrRepController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetAllDataSource()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.DataSource.OrderBy(c => c.Name).ToList()
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
        public ActionResult GetAllProject()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.Project.OrderBy(c => c.Name).ToList()
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
        public ActionResult GetAllDataType()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.DataType.OrderBy(c => c.Id).ToList()
                };

                return jsonNetResult;
                //		Message	"Invalid column name 'DataSourceType_Id'."	string

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
        public ActionResult GetAllDataSourceType()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.DataSourceType.OrderBy(c => c.Name).ToList()
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
        public ActionResult GetRelation()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.Relation.OrderBy(c => c.NameRus).ToList()
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

        public ActionResult GetTemplatesMethod()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.TemplatesMethod.OrderBy(c => c.NameRus).ToList()
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
        public ActionResult GetTemplatesFieldName()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.TemplatesFieldName.OrderBy(c => c.Id).ToList()
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
        public ActionResult EditDataSource(DataAggregator.Domain.Model.Distr.DataSource array)
        {
            try
            {
                var upd = _context.DataSource.Where(w => w.Id == array.Id).FirstOrDefault();                
                    if (upd.ProjectId != array.ProjectId)
                {
                    upd.ProjectId = array.ProjectId;
                }
                if (upd.Name != array.Name)
                {
                    upd.Name = array.Name;
                }
                if (upd.NameFull != array.NameFull)
                {
                    upd.NameFull = array.NameFull;
                }
                if (upd.DataSourceTypeId != array.DataSourceTypeId)
                {
                    upd.DataSourceTypeId = array.DataSourceTypeId;
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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
        public ActionResult AddDataSource()
        {
            var source = new DataSource()
            {
                Name = "_Новый источник",
                NameFull = "_Новый источник"
            };

            _context.DataSource.Add(source);

            _context.SaveChanges();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = source
            };

            return jsonNetResult;
        }


        [HttpPost]
        public ActionResult RemoveDataSource(long id)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_Delete_DataSource] @id=@DataSourceId", new SqlParameter("@DataSourceId", id));


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
            };


            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetTemplates(long id)
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.Templates.Where(s => s.DataSourceId == id).OrderBy(c => c.Name).ToList()
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
        public ActionResult AddTemplate(long id)
        {
            int? pr = _context.DataSource.Where(s => s.Id == id).FirstOrDefault().ProjectId;
            var source = new Templates()
            {
                Name = "_Новый шаблон",
                IsActual = true,
                TemplatesMethodId = 1,
                DataSourceId = id,
                ProjectId= pr,
                DataTypeId=0

            };

            _context.Templates.Add(source);

            _context.SaveChanges();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = source
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult EditTemplates(DataAggregator.Domain.Model.Distr.Templates array)
        {
            try
            {
                var upd = _context.Templates.Where(w => w.Id == array.Id).FirstOrDefault();
                if (upd.Name != array.Name)
                {
                    upd.Name = array.Name;
                }
                if (upd.SheetRelationId != array.SheetRelationId)
                {
                    upd.SheetRelationId = array.SheetRelationId;
                }
                if (upd.TemplatesMethodId != array.TemplatesMethodId)
                {
                    upd.TemplatesMethodId = array.TemplatesMethodId;
                }
                if (upd.DataTypeId != array.DataTypeId)
                {
                    upd.DataTypeId = array.DataTypeId;
                }
                if (upd.Sheet != array.Sheet)
                {
                    upd.Sheet = array.Sheet;
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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
        public ActionResult GetTemplate(long id)
        {
            try
            {
                var filesLoadedUsingThisTemplate = _context.Database.SqlQuery<TemplatesField>(
               " select tfn.Id, tfn.Id as [TemplatesFieldNameId],t.Id as TemplateId,t.ProjectId as ProjectId,  tf.[RelationId],tf.[Value],tf.[ColOffSet],tf.[RowOffSet] ,t.DataTypeId ,tf.[DicMappping]  from[Loader].[Templates] t  cross apply(select * from[Distr].[Loader].[TemplatesFieldName] tfn1 where t.ProjectId = tfn1.ProjectId and tfn1.datatypeid = t.datatypeid) tfn  left join[Loader].[TemplatesField] tf on tf.[TemplatesFieldNameId] = tfn.id and tf.TemplateId = t.Id where t.Id = @templateId", new SqlParameter("@templateId", id)).ToList();
                // " select tfn.Id, tfn.Id as [TemplatesFieldNameId],t.Id as TemplateId,t.ProjectId as ProjectId,  tf.[RelationId],tf.[Value],tf.[ColOffSet],tf.[RowOffSet]  from[Loader].[Templates] t cross apply (select * from [Distr].[Loader].[TemplatesFieldName] tfn1 where t.ProjectId=tfn1.ProjectId ) tfn left join[Loader].[TemplatesField] tf on tf.[TemplatesFieldNameId] = tfn.id and tf.TemplateId = t.Id where t.Id = @templateId", new SqlParameter("@templateId", id)).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = filesLoadedUsingThisTemplate
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
        public ActionResult EditTemplate(DataAggregator.Domain.Model.Distr.TemplatesField array)
        {
            try
            {
                var upd1 = _context.TemplatesField.Where(w => w.TemplateId == array.TemplateId);
                var upd = upd1.Where(s => s.TemplatesFieldNameId == array.TemplatesFieldNameId).FirstOrDefault();

                if (upd != null)
                {
                    if (upd.RelationId != array.RelationId)
                    {
                        upd.RelationId = array.RelationId;
                    }
                    if (upd.Value != array.Value)
                    {
                        upd.Value = array.Value;
                    }
                    if (upd.ColOffSet != array.ColOffSet)
                    {
                        upd.ColOffSet = array.ColOffSet;
                    }
                    if (upd.RowOffSet != array.RowOffSet)
                    {
                        upd.RowOffSet = array.RowOffSet;
                    }
                    if (upd.DicMappping != array.DicMappping)
                    {
                        upd.DicMappping = array.DicMappping;
                    }

                }
                else {

                    var source = new TemplatesField()
                    {
                        TemplatesFieldNameId = array.TemplatesFieldNameId,
                        TemplateId = array.TemplateId,
                        RelationId = array.RelationId,
                        Value = array.Value,
                        ColOffSet = array.ColOffSet,
                        RowOffSet = array.RowOffSet, 
                        DicMappping = array.DicMappping

                    };

                    _context.TemplatesField.Add(source);
                }


                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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
        public ActionResult RemoveTemplate(long id)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_Delete_Templates] @id=@TemplateId", new SqlParameter("@TemplateId", id));



            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult DistrRep_Main(string param)
        {
            try
            {
                var _context = new DistrContext(APP);
                var rep = _context.Supplier.Take(10);
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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
        public ActionResult GetCompany(int? ProjectId, int? Id)
        {
            try
            {
                var _context = new DistrContext(APP);
                var rep = _context.Comp.Where(c=> (c.ProjectId==ProjectId || ProjectId ==null) && (c.Id==Id || Id==null));
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = rep.ToList(), count = 0, status = "ок", Success = true }
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




        public ActionResult DataFiles()
        {
            //var _context = new DistrContext(APP);
            var SourceList = _context.DataSource.ToList();
            SourceList.Add(new DataSource { Id = 0, Name = "_Все", NameFull = "_Все" });
            var comp = _context.Comp.ToList();
            var proj = _context.Project.ToList();
            //  comp.Add(new Comp { Id = 0, Company = "_Все" });
            var dictionaryList = new FileInfoJson { DataSourceList = SourceList.OrderBy(t=>t.Name).ToList(), CompanyList =  comp.OrderBy(t => t.Company).ToList(), ProjectList = proj.OrderBy(t => t.ProjectName).ToList() };

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = dictionaryList
            };
        }

        [HttpPost]
        public ActionResult GetFileInfo(SelectFilter filter)
        {
            var ret = _context.Database.SqlQuery<FileInfo>("exec [Loader].[W_GetFileInfo] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                ,new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }


        [HttpPost]
        public ActionResult ReloadFile(long id)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_Reload_File] @id=@FileInfoId", new SqlParameter("@FileInfoId", id));



            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult DeleteFile(long id)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_Delete_File] @id", new SqlParameter("@id", id));



            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetFileInfoDetail(long id)
        {
            var ret = _context.Database.SqlQuery<FileInfo_Detail>("exec [Loader].[W_GetFileInfo_Detail] @FileInfoId=@FileInfoId1"
                , new SqlParameter("@FileInfoId1", id)
              
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }


        [HttpPost]
        public ActionResult CheckNewFileInfo(SelectFilter filter)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_CheckNewFileInfo] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }

        
                    [HttpPost]
        public ActionResult UploadFiles(SelectFilter filter)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_UploadFiles] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }
        [HttpPost]
        public ActionResult SendToClassification(SelectFilter filter)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_SendToClassification] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }

        [HttpPost]
        public ActionResult ToQlik(SelectFilter filter)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_Export_To_Qlick_Test] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }

        [HttpPost]
        public ActionResult fromAPI(SelectFilter filter)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_fromAPI] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }


        
[HttpPost]
        public ActionResult toFtp(SelectFilter filter)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_toFtp] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }


        [HttpPost]
        public ActionResult ToQlikProd(SelectFilter filter)
        {


            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_Export_To_Qlick_Prod] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }

        [HttpPost]
        public ActionResult GetErrorInfo(SelectFilter filter)
        {


            var ret = _context.Database.SqlQuery<FileInfo>("exec [Loader].[W_GetErrorInfo] @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }

        [HttpPost]
        public ActionResult GetTaskList(SelectFilter filter)
        {
            var Companyid = 0;
            if (filter.Company != null) Companyid = filter.Company.Id;

             var ret = _context.Database.SqlQuery<TaskList>("Select * from dbo.GetTaskList(@Company1)"
                , new SqlParameter("@Company1", Companyid)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }


        [HttpPost]
        public FileResult GetReportSheet_ToExcel(SelectFilter filter)
        {
           var _context = new DistrContext(APP);
            _context.Database.CommandTimeout = 0;
            var ret = _context.Database.SqlQuery<ReportSheet>("exec [Loader].[W_GetReportSheet] @DataSourceId=@DataSource1,@CompanyId=@Company1"
                 , new SqlParameter("@DataSource1", filter.DataSource.Id)
                , new SqlParameter("@Company1", filter.Company.Id)
                 ).ToList();

            
            
           


            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("Отчет", 1, 1, ret, true, true, null);

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "ReportSheet" + DateTime.Today.ToShortDateString()+".xlsx");
        }

 
     [HttpPost]
        public FileResult GetReportSheet_top15_ToExcel(SelectFilter filter)
        {
            var _context = new DistrContext(APP);
            _context.Database.CommandTimeout = 0;
            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            if (filter.Project.Id == 2)
            {
                var ret = _context.Database.SqlQuery<ReportSheet_top15_6FP>("exec [Loader].[W_GetReportSheet_top15_6fp]  @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                   , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                    , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                    , new SqlParameter("@DataSource1", filter.DataSource.Id)
                    , new SqlParameter("@Company1", filter.Company.Id)
                    ).ToList();
                excel.InsertDataTable("Отчет", 1, 1, ret, true, true, null);
            }
            else
            {
                var ret = _context.Database.SqlQuery<ReportSheet_top15>("exec [Loader].[W_GetReportSheet_top15]  @Year=@Year1,@Month=@Month1,@DataSourceId=@DataSource1,@CompanyId=@Company1"
                      , new SqlParameter("@Year1", Convert.ToInt32(filter.Year))
                       , new SqlParameter("@Month1", Convert.ToInt32(filter.Month))
                       , new SqlParameter("@DataSource1", filter.DataSource.Id)
                       , new SqlParameter("@Company1", filter.Company.Id)
                       ).ToList();
                excel.InsertDataTable("Отчет", 1, 1, ret, true, true, null);
            }
    
    
    
    
    
            
    
        
    
            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "ReportSheet_top15" + DateTime.Today.ToShortDateString() + ".xlsx");
        }

        public ActionResult Rules()
        {
            //var _context = new DistrContext(APP);
            var SourceList = _context.DataSource.ToList();
            SourceList.Add(new DataSource { Id = 0, Name = "_Все", NameFull = "_Все" });
            var comp = _context.Comp.ToList();
            //  comp.Add(new Comp { Id = 0, Company = "_Все" });
            var dictionaryList = new FileInfoJson { DataSourceList = SourceList.OrderBy(t => t.Name).ToList(), CompanyList = comp.OrderBy(t => t.Company).ToList() };

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = dictionaryList
            };
        }

        [HttpPost]
        public ActionResult GetRules_Clients(SelectFilter filter)
        {
            var ret = _context.Database.SqlQuery<Rules_Clients>("exec [Loader].[W_GetRules_Clients] @CompanyId=@Company1"                
                , new SqlParameter("@Company1", filter.Company.Id)
                ).ToList();



            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
                //_context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.DataSource.Id,filter.Company.Id)
            };
        }

        [HttpPost]
        public ActionResult EditRules_Clients(DataAggregator.Domain.Model.Distr.Rules_Clients array)
        {
            try
            {
                var upd = _context.Rules_Clients.Where(w => w.Id == array.Id).FirstOrDefault();
                if (upd.Name != array.Name)
                {
                    upd.Name = array.Name;
                }
                if (upd.CompanyId != array.CompanyId)
                {
                    upd.CompanyId = array.CompanyId;
                }
                if (upd.INN != array.INN)
                {
                    upd.INN = array.INN;
                }
                if (upd.Region_Before != array.Region_Before)
                {
                    upd.Region_Before = array.Region_Before;
                }
                if (upd.Region_After != array.Region_After)
                {
                    upd.Region_After = array.Region_After;
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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
        public ActionResult AddRules_Clients(SelectFilter filter)
        {
            var source = new Rules_Clients()
            {
                Name = "_Новое исключение",
                CompanyId= filter.Company.Id
            };

            _context.Rules_Clients.Add(source);

            _context.SaveChanges();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = source
            };

            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult RemoveRules_Clients(long id)
        {

           
            var ret = _context.Database.SqlQuery<Val>("exec [Loader].[W_Delete_Rules_Clients] @id", new SqlParameter("@id", id));



            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret.FirstOrDefault().Value
            };

            return jsonNetResult;
           
        }
        
        [Authorize(Roles = "DistrRep_Main")]
        public ActionResult DistrRep_RawData_Init()
        {
            try
            {
                _context.Database.CommandTimeout = 0;
                ViewBag.spr_Comp = _context.Comp.OrderBy(o => o.Company).Select(s=> new GS.sprItem() { code=s.Id,Status=s.Company}).ToList();                    
                ViewBag.spr_Comp.Insert(0, new GS.sprItem() { Status = "все", code = 0 });
                ViewBag.spr_Tops = new List<GS.sprItem>
                {
                    new GS.sprItem() { code = 300, Status = "300" },
                    new GS.sprItem() { code = 1000, Status = "1000" },
                    new GS.sprItem() { code = 5000, Status = "5000" },
                    new GS.sprItem() { code = 50000, Status = "50000" },
                    new GS.sprItem() { code = 50000000, Status = "все строки" }
                };

                ViewBag.spr_DataSource = _context.DataSource.OrderBy(o => o.Name).Select(s => new GS.sprItemLg() { code = s.Id, Status = s.Name }).ToList();
                ViewBag.spr_DataSource.Insert(0, new GS.sprItemLg() { Status = "все", code = 0 });

                ViewBag.spr_DataSourceType = _context.DataSourceType.OrderBy(o => o.Name).Select(s => new GS.sprItemLg() { code = s.Id, Status = s.Name }).ToList();
                ViewBag.spr_DataSourceType.Insert(0, new GS.sprItemLg() { Status = "все", code = 0 });

                ViewBag.spr_DistributionType = _context.DistributionType.OrderBy(o => o.Name).Select(s => new GS.sprItemLg() { code = s.Id, Status = s.Name }).ToList();

                ViewBag.spr_Spec = new List<GS.sprItemst>
                {
                    new GS.sprItemst() { code = "", Status = "без спец Фильтров" },
                    new GS.sprItemst() { code = "noTypeClients", Status = "Нет Тип Получателя" },
                };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = ViewBag
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
        [Authorize(Roles = "DistrRep_Main")]
        public ActionResult DistrRep_RawData_GetData(DistrRep_RawData_Filter filter)
        {
            try
            {
                filter.isnull();
                _context.Database.CommandTimeout = 20 * 60;
               var where = _context.RawData_Out_View.Where(w=>1==1);
                if (filter.DataSourceId > 0)
                    where = where.Where(w => w.DataSourceId == filter.DataSourceId);
                if (filter.DataSourceTypeId > 0)
                    where = where.Where(w => w.DataSourceTypeId == filter.DataSourceTypeId);
                if (filter.CompanyId > 0)
                    where = where.Where(w => w.CompanyId == filter.CompanyId);
                if (!string.IsNullOrEmpty(filter.Search_text))
                {
                    where = where.Where(w => w.Address.Contains(filter.Search_text));
                }
                switch(filter.Spec)
                {
                    case "noTypeClients":
                        where = where.Where(w => w.DistributionTypeId==null);
                        break;
                }
                var result = where.Take(filter.top).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result, count = result.Count(), status = "ок", Success = true }
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
        [Authorize(Roles = "DistrRep_Main")]
        [HttpPost]
        public ActionResult DistrRep_RawData_SetData(ICollection<DataAggregator.Domain.Model.Distr.RawData_Out_View> array)
        {
            try
            {
                if (array != null)
                {
                    foreach (var item in array)
                    {
                        var upd = _context.RawData_Out.Where(w => w.Id == item.Id).Single();
                        upd.DistributionTypeId = item.DistributionTypeId;
                        upd.DistributionTypeId_UserId = aspUser.UserId;
                        upd.DistributionTypeId_Date = DateTime.Now;
                    }
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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


        [Authorize(Roles = "DistrRep_Main")]
        public ActionResult CompanyPeriod_Init()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = ViewBag
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
        [Authorize(Roles = "DistrRep_Main")]
        public ActionResult CompanyPeriod_Search()
        {
            try
            {
                ViewData["CompanyPeriod"] = _context.CompanyPeriod.OrderByDescending(o => o.period).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewData, count = 0, status = "ок", Success = true }
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
        [Authorize(Roles = "DistrRep_Main")]
        [HttpPost]
        public ActionResult CompanyPeriod_Save(ICollection<DataAggregator.Domain.Model.Distr.CompanyPeriod> array)
        {
            try
            {
                if (array != null)
                {
                    foreach (var item in array)
                    {
                        var upd = _context.CompanyPeriod.Where(w => w.Id == item.Id).Single();
                        upd.toProd = item.toProd;
                    }
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Блок Управления Загрузки чеков
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

        public ActionResult CheckReloadInit()
        {
            //var _context = new DistrContext(APP);
            var SourceList = _context.DataSource.Where(a=> a.ProjectId==2).ToList();
            SourceList.Add(new DataSource { Id = 0, Name = "_Все", NameFull = "_Все" });
            var comp = _context.Comp.Where(a => a.ProjectId == 2).ToList();         
            comp.Add(new Comp { Id = 0, Company = "_Все" });
            var dictionaryList = new FileInfoJson { DataSourceList = SourceList.OrderBy(t => t.Name).ToList(), CompanyList = comp.OrderBy(t => t.Company).ToList() };

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = dictionaryList
            };
        }








    }







    public class DistrRep_RawData_Filter
    {
        public int top { get; set; }
        public long DataSourceTypeId { get; set; }
        public int ProjectId { get; set; }
        public int CompanyId { get; set; }
        public long DataSourceId { get; set; }
        public string Spec { get; set; }
        public string Search_text { get; set; }       
        public void isnull()
        {
            if (Search_text == null)
                Search_text = "";
            if (Spec == null)
                Spec = "";
        }
    }



    public class JsonResult
    {
        public object Data { get; set; }
        public object Data2 { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public bool Success { get; set; }
    }
    public class Val
    {
       
        public string Value { get; set; }
     
    }
}