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
    public class LPUDictionariesController : BaseController
    {
        private GSContext _context;

        private static readonly object LockObject = new object();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GSContext(APP);
        }

        ~LPUDictionariesController()
        {
            _context.Dispose();
        }


       
     
        public ActionResult GetLPUType()
        {
            try
            {
                var LPUType = _context.LPUType.ToList();
                return ReturnData(LPUType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
     
        public ActionResult GetLPUKind()
        {
            try
            {
                var LPUKind = _context.LPUKind.ToList();
                return ReturnData(LPUKind);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}