using DataAggregator.Core.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.GTIN;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNet.Identity;

namespace DataAggregator.Web.Controllers.Classifier
{
    public class GTINController : BaseController
    {
        // GET: GTIN

        public ActionResult GTIN_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                ViewData["Source"] = _context.Database.SqlQuery<DataAggregator.Domain.Model.DrugClassifier.Systematization.Source>("[gtin].[GetGTINsSource_SP]").ToList();
                ViewData["DrugType"] = _context.Database.SqlQuery<string>("SELECT distinct DrugType FROM[DrugClassifier].[gtin].[GTINs] g with(nolock) inner join[Classifier].[ExternalView_FULL] c with(nolock) on c.ClassifierId = g.ClassifierId").ToList();


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
        public ActionResult GTINs_search(GTINs_Filter filter)
        {
            try
            {
                using (var _context = new DrugClassifierContext(APP))
                {
                    IEnumerable<GTINs_View> result = _context.Database.SqlQuery<GTINs_View>("[gtin].[GetGTINs_SP] @Id=@Id,@SourceId=@SourceId,@GTIN=@GTIN,@ClassifierId=@ClassifierId,@DrugId=@DrugId,@GoodsId=@GoodsId,@TradeName=@TradeName,@DrugDescription=@DrugDescription," +
                        "@OwnerTradeMarkId=@OwnerTradeMarkId,@OwnerTradeMark=@OwnerTradeMark,@PackerId=@PackerId,@Packer=@Packer,@DrugType=@DrugType,@Used=@Used,@GTIN2ClassifierId=@GTIN2ClassifierId,@ClassifierId2GTIN=@ClassifierId2GTIN,@AllValid=@AllValid"
                , new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.VarChar, Value = (object)filter.Id ?? DBNull.Value }
                , new SqlParameter { ParameterName = "@SourceId", SqlDbType = SqlDbType.BigInt, Value = (object)filter.SourceId ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@GTIN", SqlDbType = SqlDbType.VarChar, Value = (object)filter.GTIN ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@ClassifierId", SqlDbType = SqlDbType.VarChar, Value = (object)filter.ClassifierId ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@DrugId", SqlDbType = SqlDbType.VarChar, Value = (object)filter.DrugId ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@GoodsId", SqlDbType = SqlDbType.VarChar, Value = (object)filter.GoodsId ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@TradeName", SqlDbType = SqlDbType.VarChar, Value = (object)filter.TradeName ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@DrugDescription", SqlDbType = SqlDbType.VarChar, Value = (object)filter.DrugDescription ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@OwnerTradeMarkId", SqlDbType = SqlDbType.VarChar, Value = (object)filter.OwnerTradeMarkId ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@OwnerTradeMark", SqlDbType = SqlDbType.VarChar, Value = (object)filter.OwnerTradeMark ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@PackerId", SqlDbType = SqlDbType.VarChar, Value = (object)filter.PackerId ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@Packer", SqlDbType = SqlDbType.VarChar, Value = (object)filter.Packer ?? DBNull.Value }
                 , new SqlParameter { ParameterName = "@DrugType", SqlDbType = SqlDbType.VarChar, Value = (object)filter.DrugType ?? DBNull.Value }
                  , new SqlParameter { ParameterName = "@Used", SqlDbType = SqlDbType.Bit, Value = (object)filter.Used ?? DBNull.Value }
                      , new SqlParameter { ParameterName = "@GTIN2ClassifierId", SqlDbType = SqlDbType.Bit, Value = (object)filter.GTIN2ClassifierId ?? DBNull.Value }
                          , new SqlParameter { ParameterName = "@ClassifierId2GTIN", SqlDbType = SqlDbType.Bit, Value = (object)filter.ClassifierId2GTIN ?? DBNull.Value }
                              , new SqlParameter { ParameterName = "@AllValid", SqlDbType = SqlDbType.Bit, Value = (object)filter.AllValid ?? DBNull.Value }
               );


                   
                    ViewData["GTINs_View"] = result.ToList();
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
        public ActionResult Import_GTIN_NEW_from_Excel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            try
            {
                if (uploads == null || !uploads.Any())
                    return null;

                using (var _context = new DrugClassifierContext(APP))
                {
                    var guidd = Guid.NewGuid();
                    var file = uploads.First();
                    string filename = @"\\s-sql1\Data\GTIN_NEW\AddToClassif_" + User.Identity.GetUserId() +"_"+ guidd + ".xlsx";

                    if (System.IO.File.Exists(filename))
                        System.IO.File.Delete(filename);

                    file.SaveAs(filename);


                    var result = _context.Database.ExecuteSqlCommand("[gtin].[UploadGTINNewFromExcel] @filename ,@UserId"
                            , new SqlParameter { ParameterName = "@filename", SqlDbType = SqlDbType.NVarChar, Value = (object)filename ?? DBNull.Value }
                            , new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.NVarChar, Value = (object)User.Identity.GetUserId() ?? DBNull.Value }
                        );

                 
                }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost]
        public ActionResult GTINs_save(ICollection<GTINs_View> array_GTINs)
        {
            try
            {
                using (var _context = new DrugClassifierContext(APP))
                {
                    foreach (var item in array_GTINs)
                    {



                        var result = _context.Database.ExecuteSqlCommand("[gtin].[SetGTINs] @Id ,@IsActive ,@OperatorComment,@ClassifierId,@UserId"
                           , new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.BigInt, Value = (object)item.Id ?? DBNull.Value }
                           , new SqlParameter { ParameterName = "@IsActive", SqlDbType = SqlDbType.Bit, Value = (object)item.IsActive ?? DBNull.Value }
                           , new SqlParameter { ParameterName = "@OperatorComment", SqlDbType = SqlDbType.NVarChar, Value = (object)item.OperatorComment ?? DBNull.Value }
                           , new SqlParameter { ParameterName = "@ClassifierId", SqlDbType = SqlDbType.BigInt, Value = (object)item.ClassifierId ?? DBNull.Value }
                           , new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.NVarChar, Value = (object)User.Identity.GetUserId() ?? DBNull.Value }
                       );
                                             

                    }
            

                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                    };
                    return jsonNetResult;
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

    }
}