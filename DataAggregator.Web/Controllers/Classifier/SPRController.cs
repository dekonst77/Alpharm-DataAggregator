using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.MassFixesData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;

namespace DataAggregator.Web.Controllers.Classifier
{
    public class SPRController : BaseController
    {
        class AllowC
        {
            public string TableName { get; set; }
            public string Role { get; set; }
            public string Cmd { get; set; }
        }
        public bool AllowTbl(string db, string shema, string name)
        {
            System.Collections.Generic.List<AllowC> Arr = new List<AllowC>
            {
                //не забывать размножать тригерры
                new AllowC() { TableName = "[DrugClassifier].Classifier.[Manufacturer]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[Corporation]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[Dosage]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[TradeName]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[ManufacturerClear]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[Brand]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[INN]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[Dosage]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[Equipment]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[FormProduct]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[FTG]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[Packing]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[ATCWho]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[ATCEphmra]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[ATCBaa]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].Classifier.[NFC]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].grls.[ChemicalSPR]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[GovernmentPurchases].dbo.[spr_SiteName]", Role = "GManager", Cmd = @"
                    insert into [GovernmentPurchases].[dbo].[spr_SiteName](Value,Description)
                    select SiteName,SiteName from 
                    (select SiteName from [GovernmentPurchases].[dbo].[Purchase] group by SiteName) GG                    
                    where SiteName not in (select Value from [GovernmentPurchases].[dbo].[spr_SiteName])" },
                new AllowC() { TableName = "[DrugClassifier].GoodsClassifier.[GoodsTradeName]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].GoodsClassifier.[Goods]", Role = "SBoss", Cmd = "" },
                new AllowC() { TableName = "[DrugClassifier].GoodsSystematization.[GoodsCategory]", Role = "SBoss", Cmd = "" }
            };

            var res = Arr.Where(w => w.TableName == "[" + db + "]." + shema + ".[" + name + "]").FirstOrDefault();
            if (res != null)
            {
                if (User.IsInRole(res.Role))
                {
                    if (!string.IsNullOrEmpty(res.Cmd))
                    {
                        var _context = new DrugClassifierContext(APP);
                        _context.Database.ExecuteSqlCommand(res.Cmd);
                    }
                    return true;
                }
            }
            return false;
        }
        public ActionResult SPR_Init(string db, string shema, string name, string type)
        {
            if (!AllowTbl(db, shema, name))
                return Forbidden();
            try
            {
                var _context = new DrugClassifierContext(APP);

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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
        public ActionResult SPR_search(string db, string shema, string name, string type)
        {
            if (!AllowTbl(db, shema, name))
                return Forbidden();
            try
            {
                var _context = new DrugClassifierContext(APP);
                string query = "select 1/0";
                switch (type)
                {
                    case "t1":
                        query = "select Id,Value,Value_Eng from [" + db + "].[" + shema + "].[" + name + "]";
                        var SPR_t1 = _context.Database.SqlQuery<DataAggregator.Domain.Model.Common.DictionaryItem_t1>(query);
                        ViewData["SPR"] = SPR_t1.ToList();
                        break;
                    case "t2":
                        query = "select Id,Value,Description,Description_Eng,IsUse from [" + db + "].[" + shema + "].[" + name + "]";
                        var SPR_t2 = _context.Database.SqlQuery<DataAggregator.Domain.Model.Common.SPRItem_t2>(query);
                        ViewData["SPR"] = SPR_t2.ToList();
                        break;
                    case "t3":
                        query = "select Id,Value,Description from [" + db + "].[" + shema + "].[" + name + "]";
                        var SPR_t3 = _context.Database.SqlQuery<DataAggregator.Domain.Model.Common.SPRItem_t3>(query);
                        ViewData["SPR"] = SPR_t3.ToList();
                        break;
                    case "t4":
                        query = "select Id,Value,Value_Eng,UseClassifier,UseGoodsClassifier from  [" + db + "].[" + shema + "].[" + name + "]";
                        var SPR_t4 = _context.Database.SqlQuery<DataAggregator.Domain.Model.Common.DictionaryItem_t4>(query);
                        ViewData["SPR"] = SPR_t4.ToList();
                        break;
                    case "Goods":
                        query = "exec[GoodsClassifier].[GetGoodsSPR]";
                        var SPR_t5 = _context.Database.SqlQuery<DataAggregator.Domain.Model.Common.DictionaryItem_t5>(query);
                        ViewData["SPR"] = SPR_t5.ToList();
                        break;
                    case "GoodsCategory":
                        query = "exec[GoodsClassifier].[GetGoodsCategorySPR]";
                        var SPR_GoodsCategory = _context.Database.SqlQuery<DataAggregator.Domain.Model.Common.DictionaryItem_GoodsCategory>(query);
                        ViewData["SPR"] = SPR_GoodsCategory.ToList();
                        break;
                }

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SPR_save(string db, string shema, string name, string type, ICollection<DataAggregator.Domain.Model.Common.SPRItem_t_return> array_SPR
            )
        {
            if (!AllowTbl(db, shema, name))
                return Forbidden();
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_SPR != null)
                    foreach (var item in array_SPR)
                    {
                        item.Value = item.Value ?? "";
                        item.Value_Eng = item.Value_Eng ?? "";
                        item.MiniName = item.MiniName ?? "";

                        if (item.Description == null) item.Description = "";
                        if (item.Description_Eng == null) item.Description_Eng = "";

                        if (item.GoodsDescription == null) item.GoodsDescription = "";
                        if (item.GoodsDescription_Eng == null) item.GoodsDescription_Eng = "";

                        string query = "";
                        switch (type)
                        {
                            case "t1":
                            case "t4":
                                if (item.Id > 0)
                                {
                                    query = " update [" + db + "].[" + shema + "].[" + name + "] set [Value] = @Value, [Value_Eng] = @Value_Eng where id = @id ";
                                }
                                if (item.Id < 0)
                                {
                                    query = " delete from [" + db + "].[" + shema + "].[" + name + "] where id = @id ";
                                    item.Id = -1 * item.Id;
                                    if (name == "INN")
                                    {
                                        query = @"--declare @id bigint=30662

                                        if (not exists(select * from [Classifier].[Drug] where INNGroupId in (select INNGroupId from[Classifier].[INNGroup_INN] where INNId = @id)))
                                        begin
                                          delete from [Classifier].[INNGroup_INN] where INNGroupId in (select INNGroupId from[Classifier].[INNGroup_INN] where INNId = @id)
                                          delete from [Classifier].[INNGroup] where id in (select INNGroupId from[Classifier].[INNGroup_INN] where INNId = @id)
                                        end
                                        " + query;
                                    }
                                }
                                if (name == "INN")
                                {
                                    query += @" exec [Classifier].[INNGroup_Restore] ";
                                }
                                _context.Database.ExecuteSqlCommand(query, new SqlParameter("@Value", item.Value), new SqlParameter("@Value_Eng", item.Value_Eng), new SqlParameter("@id", item.Id));
                                break;
                            case "t2":
                                if (item.Id > 0)
                                {
                                    query = "update [" + db + "].[" + shema + "].[" + name + "] set [Description]=@Description,[Description_Eng]=@Description_Eng,[IsUse]=@IsUse where id=@id ";
                                }
                                _context.Database.ExecuteSqlCommand(query, new SqlParameter("@Value", item.Value), new SqlParameter("@Value_Eng", item.Value_Eng), new SqlParameter("@id", item.Id), new SqlParameter("@Description", item.Description), new SqlParameter("@Description_Eng", item.Description_Eng), new SqlParameter("@IsUse", item.IsUse));
                                break;
                            case "t3":
                                if (item.Id > 0)//Обновление
                                {
                                    query = "update [" + db + "].[" + shema + "].[" + name + "] set [Value]=@Value,[Description]=@Description where id=@id ";
                                }
                                if (item.Id < 0)//Удаление
                                {
                                    query = "delete from [" + db + "].[" + shema + "].[" + name + "] where id=@id ";
                                    item.Id = -1 * item.Id;
                                }
                                _context.Database.ExecuteSqlCommand(query, new SqlParameter("@Value", item.Value), new SqlParameter("@id", item.Id), new SqlParameter("@Description", item.Description), new SqlParameter("@IsUse", item.IsUse));
                                break;

                            case "t5":
                                if (item.Id > 0)
                                {
                                    query = " update [" + db + "].[" + shema + "].[" + name + "] set [GoodsDescription] = @Value, [GoodsDescription_Eng] = @Value_Eng where id = @id ";
                                }
                                if (item.Id < 0)
                                {
                                    query = " delete from [" + db + "].[" + shema + "].[" + name + "] where id = @id ";
                                    item.Id = -1 * item.Id;
                                }
                                _context.Database.ExecuteSqlCommand(query, new SqlParameter("@Value", item.GoodsDescription), new SqlParameter("@Value_Eng", item.GoodsDescription_Eng), new SqlParameter("@id", item.Id));
                                break;

                            // справочник ДОП <Форма выпуска>
                            case "Goods":
                                if (item.Id > 0)
                                {
                                    query = " update [" + db + "].[" + shema + "].[" + name + "] set [GoodsDescription] = @Value, [GoodsDescription_Eng] = @Value_Eng where id = @id ";
                                }
                                if (item.Id < 0)
                                {
                                    query = " delete from [" + db + "].[" + shema + "].[" + name + "] where id = @id ";
                                    item.Id = -1 * item.Id;
                                }
                                _context.Database.ExecuteSqlCommand(query, new SqlParameter("@Value", item.GoodsDescription), new SqlParameter("@Value_Eng", item.GoodsDescription_Eng), new SqlParameter("@id", item.Id));
                                break;

                            // справочник ДОП -> Категории
                            case "GoodsCategory":
                                if (item.Id > 0)
                                {
                                    query = " update [" + db + "].[" + shema + "].[" + name + "] set [Name] = @Value, [Name_Eng] = @Value_Eng, [MiniName] = @MiniValue where id = @id ";
                                }
                                if (item.Id < 0)
                                {
                                    query = " delete from [" + db + "].[" + shema + "].[" + name + "] where id = @id ";
                                    item.Id = -1 * item.Id;
                                }
                                _context.Database.ExecuteSqlCommand(query, new SqlParameter("@Value", item.Value), new SqlParameter("@Value_Eng", item.Value_Eng), new SqlParameter("@MiniValue", item.MiniName), new SqlParameter("@id", item.Id));
                                break;
                        }

                    }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SPR_FromExcel(string db, string shema, string name, string type, IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();

#if DEBUG
            string filename = @"\\alph-r01-s-db02\Upload\SPR_" + User.Identity.GetUserId() + ".xlsx";
#else
            string filename = @"\\s-sql1\Upload\SPR_" + User.Identity.GetUserId() + ".xlsx";
#endif

            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);

            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            string command = "";

            if (type == "t1" || type == "t4")
                command = string.Format(@"use [{1}];
SET XACT_ABORT ON;
begin transaction
declare @filename nvarchar(255)='S:\Upload\SPR_{0}.xlsx';
drop table if exists [tempdb].dbo.[SPR_{0}];

	exec [ControlALG].dbo.Excel_ToDB @filename,'{3}','[tempdb].dbo.[SPR_{0}]','withnull',1,2;

	delete from [tempdb].dbo.[SPR_{0}] where [Код] is null;

	update CL set CL.Value=NN.[Значение], CL.Value_Eng = isnull(NN.[Value], '')
	--select * 
	from
	[{2}].[{3}] CL
	inner join  tempdb.dbo.[SPR_{0}] NN
	on NN.[Код]=CL.Id --and NN.[Значение]=CL.Value
where (CL.Value Collate Cyrillic_General_CS_AS <> NN.[Значение]  Collate Cyrillic_General_CS_AS
OR isnull(CL.Value_Eng, '') Collate Cyrillic_General_CS_AS <> isnull(NN.[Value], '')  Collate Cyrillic_General_CS_AS);

drop table if exists [tempdb].dbo.[SPR_{0}];
commit transaction;
", User.Identity.GetUserId(), db, shema, name);

            if (type == "t2")
                command = string.Format(@"use [{1}];
SET XACT_ABORT ON;
begin transaction
declare @filename nvarchar(255)='S:\Upload\SPR_{0}.xlsx';
drop table if exists [tempdb].dbo.[SPR_{0}];

	exec [ControlALG].dbo.Excel_ToDB @filename,'{3}','[tempdb].dbo.[SPR_{0}]','withnull',1,2;

	delete from [tempdb].dbo.[SPR_{0}] where [Код] is null;

	update CL set CL.Description=NN.[Описание], CL.Description_Eng=NN.[Description]
	--select * 
	from
	[{2}].[{3}] CL
	inner join  tempdb.dbo.[SPR_{0}] NN
	on NN.[Код]=CL.Id --and NN.[Описание]=CL.Description
where (CL.Description Collate Cyrillic_General_CS_AS <>NN.[Описание]  Collate Cyrillic_General_CS_AS
OR CL.Description_Eng Collate Cyrillic_General_CS_AS <>NN.[Description]  Collate Cyrillic_General_CS_AS);


drop table if exists [tempdb].dbo.[SPR_{0}];
commit transaction;
", User.Identity.GetUserId(), db, shema, name);

            if ((type == "GoodsCategory") || (type == "Goods"))
            {
                command = $"use [{db}]; exec GoodsClassifier.ExportXLSToSPR @Sourcefilename = '{filename}', @UserId = '{User.Identity.GetUserId()}', @sheetName = '{name}'";
            }

            try
            {
                _context.Database.ExecuteSqlCommand(command);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
    }
}