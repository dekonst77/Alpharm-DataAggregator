using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail.SendToClassification;
using DataAggregator.Web.App_Start;
using DataAggregator.Web.Models;
using DataAggregator.Web.Models.Retail.FilterInfo;
using DataAggregator.Web.RetailFileInfoService;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;


namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class FileInfoController : BaseController
    {
        private readonly RetailContext _context;

       
        public FileInfoController()
        {
            _context = new RetailContext();
        }

        ~FileInfoController()
        {
            _context.Dispose();
        }

        public ActionResult Initialize()
        {
            var dictionaryList = new FileInfoJson { SourceList = _context.Source.ToList() };
            
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = dictionaryList
            };
        }

        [HttpPost]
        public ActionResult SendFileInfoReload(List<long> ids)
        {

            FileInfoServiceClient client = new FileInfoServiceClient();
            foreach (var id in ids)
            {
                client.ReloadFileInfo(id);
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = null
            };
        }

        [HttpPost]
        public ActionResult SendFileInfoDelete(List<long> ids)
        {
            FileInfoServiceClient client = new FileInfoServiceClient();
            foreach (var id in ids)
            {
                client.DeleteFileInfo(id);
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = null
            };
        }

        [HttpPost]
        public ActionResult GetErrorInfo(SelectFilter filter)
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.GetErrorInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year))
            };
        }

        [HttpPost]
        public ActionResult CheckFiles(SelectFilter filter)
        {

            FileInfoServiceClient client = new FileInfoServiceClient();
            client.InnerChannel.OperationTimeout = new TimeSpan(1, 00, 0);
            client.UpdateFileInfo(filter.Source.Id, Convert.ToInt32(filter.Year), Convert.ToInt32(filter.Month));
       
            return GetInfo(filter);
        }
     

        [HttpPost]
        public ActionResult GetInfo(SelectFilter filter)
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.GetFileInfo(Convert.ToInt32(filter.Month), Convert.ToInt32(filter.Year), filter.Source.Id)
            };
        }       

      
    }



}