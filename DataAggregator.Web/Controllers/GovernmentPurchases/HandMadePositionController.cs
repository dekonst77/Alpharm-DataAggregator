using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.MassFixesData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Serialization;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    public class HandMadePositionController : BaseController
    {
        private readonly GovernmentPurchasesContext _context;
        public HandMadePositionController()
        {
            _context = new GovernmentPurchasesContext(APP);
        }


        ~HandMadePositionController()
        {
            _context.Dispose();
        }

        public ActionResult HandMadePosition_Init()
        {
            try
            {
                var contextAgg = new DataAggregator.Domain.DAL.DataAggregatorContext(APP);
               // var _context = new GovernmentPurchasesContext(APP);
                ViewData["UsersAll"] = contextAgg.UserViewAll.ToList();
                ViewData["RegionList"] = _context.Covid19_Region.ToList();
                ViewData["ProductList"] = _context.Covid19_Product.ToList();
                ViewData["PriceList"] = _context.Covid19_Product_Price.ToList();
                // ViewData["ProductList"] = _context.Covid19_Product.ToList();
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
        public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        [HttpPost]
        public ActionResult HandMadePosition_search(long? Region_Id)
        {
            try
            {
                //    var _context = new GovernmentPurchasesContext(APP);

                //  var Raw = _context.Covid19_Vaccinate_View.Where(w => 1 == 1);
                // if (Region_Id.GetValueOrDefault(0) > 0)
                //  {
                //      Raw = Raw.Where(w => w.Region_Id == Region_Id);
                //   }
                DataTable Raw = new DataTable();


                /*
                 var cmd = _context;
                 SqlDataAdapter da = new SqlDataAdapter();
                 da.SelectCommand=new SqlCommand("exec dbo.Covid19_GetVaccinate @Region_Id = " + Region_Id.ToString(), cmd);
                 da.SelectCommand.CommandType= CommandType.StoredProcedure;
                 DataSet ds = new DataSet();
                 da.Fill(ds);
                 DataTable dt = ds.Tables["result_name"];
                */
          
                var conn = _context.Database.Connection;
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "[GovernmentPurchases].dbo.Covid19_GetVaccinate";
                        cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("Region_Id", Region_Id));
                        using (var reader = cmd.ExecuteReader())
                        {
                            Raw.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    throw ex;
                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }





                // var Raw1= _context.Database.SqlQuery<DataTable>("exec dbo.Covid19_GetVaccinate @Region_Id=@Region", new SqlParameter("@Region", Region_Id));

                Raw.TableName = "Raw";

                // DataTable qq = Raw;
                //   var Data = new JsonResultData() { Data = ret2, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Raw
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }



        
         [HttpPost]
        public ActionResult HandMadePosition_delete(List<long?> ids)
        {
            var conn = _context.Database.Connection;
            var connectionState = conn.State;
            try
            {
                if (connectionState != ConnectionState.Open) conn.Open();
                foreach (var Id in ids)
                {
                    using (var cmd = conn.CreateCommand())
                {

                   
                        DataTable Raw = new DataTable();
                        cmd.CommandText = "[GovernmentPurchases].[dbo].[Covid19_Vaccinate_Remove]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("Id", Id));
                        using (var reader = cmd.ExecuteReader())
                        {
                            Raw.Load(reader);
                        }                        
                    }


                  
                }
            }
            catch (Exception ex)
            {
                // error handling
                throw ex;
            }
            finally
            {
                if (connectionState != ConnectionState.Closed) conn.Close();
            }




                //  var userGuid = new Guid(User.Identity.GetUserId());
                // var product = _context.Covid19_Product.Where(c => c.Classifer_Id == id).Single();
                //_context.Covid19_Product_History.Add(new Domain.Model.GovernmentPurchases.Covid19_Product_History { Classifer_Id = product.Classifer_Id, Drug = product.Drug, DrugShort = product.DrugShort, Package = product.Package, UserLastUpdate = userGuid, UserLastUpdateDate = product.UserLastUpdateDate });
                //   var ret = _context.Database.SqlQuery<string>("delete from [GovernmentPurchases].[dbo].[Covid19_Product] where [Classifer_Id]=@prId", new SqlParameter("@prId", id));

                //            _context.Covid19_Product.Remove(product);
                //          _context.SaveChanges();



                JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };


            return jsonNetResult;
        }




        [HttpPost]
        public ActionResult getProductGrid()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,                    
                    Data = _context.Covid19_Product.ToList()
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
        public ActionResult getColumnGrid()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.Covid19_Column_View.ToList()
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
        public ActionResult getPriceGrid()
        {
            try
            {
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = _context.Covid19_Product_Price.ToList()
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
        public class Covid19_Vaccinate_Vi
        {
           
            public long Id { get; set; }
            public long Region_Id { get; set; }
        }


        [HttpPost]
        public ActionResult addPeriod(DateTime date) {
            var Date = date;
            var ret = _context.Database.SqlQuery<string>("exec [GovernmentPurchases].[dbo].[Covid19_Vaccinate_addPeriod] @Period=@Date", new SqlParameter("@Date", Date));

            string aa = ret.FirstOrDefault().ToString();
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }


          [HttpPost]
        public ActionResult HandMadePosition_save(
        ICollection<Dictionary<string, object>> array_Raw
           )
        {
       
          
            // var t = array_Raw[0];
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());
               
               
               
                foreach (Dictionary<string, object> row in array_Raw) {


                   // String qqqqq = (string)row["IputSecond"];

                    long Ids = Convert.ToInt64(row["Id"].ToString());
                    long Region_id = Convert.ToInt64(row["Region_id"].ToString());
                    long? InputFirst = 0;
                    long? InputSecond = 0;
                    long? InputRevaccinated = 0;
                    long? InputСhildren = 0;
                    long? First = 0;
                    long? Second = 0;
                    long? Revaccinated = 0;
                    long? Сhildren = 0;

                    if (row["InputFirst"] != null || row["InputFirst"] == System.DBNull.Value) {
                        string InputFirstStr = row["InputFirst"].ToString();
                        if(InputFirstStr.Length>0) InputFirst = Convert.ToInt64(InputFirstStr);
                    }
                    if (row["InputSecond"] != null || row["InputSecond"] == System.DBNull.Value)
                    {
                        string InputSecondStr = row["InputSecond"].ToString();
                        if (InputSecondStr.Length > 0) InputSecond = Convert.ToInt64(InputSecondStr);
                    }
                    if (row["InputRevaccinated"] != null || row["InputRevaccinated"] == System.DBNull.Value)
                    {
                        string InputRevaccinatedStr = row["InputRevaccinated"].ToString();
                        if (InputRevaccinatedStr.Length > 0) InputRevaccinated = Convert.ToInt64(InputRevaccinatedStr);
                    }
                    if (row["InputСhildren"] != null || row["InputСhildren"] == System.DBNull.Value)
                    {
                        string InputСhildrenStr = row["InputСhildren"].ToString();
                        if (InputСhildrenStr.Length > 0) InputСhildren = Convert.ToInt64(InputСhildrenStr);
                    }
                    if (row["First"] != null || row["First"] == System.DBNull.Value)
                    {
                        string FirstStr = row["First"].ToString();
                        if (FirstStr.Length > 0) First = Convert.ToInt64(FirstStr);
                    }
                    if (row["Second"] != null || row["Second"] == System.DBNull.Value)
                    {
                        string SecondStr = row["Second"].ToString();
                        if (SecondStr.Length > 0) Second = Convert.ToInt64(SecondStr);
                    }
                    if (row["Revaccinated"] != null || row["Revaccinated"] == System.DBNull.Value)
                    {
                        string RevaccinatedStr = row["Revaccinated"].ToString();
                        if (RevaccinatedStr.Length > 0) Revaccinated = Convert.ToInt64(RevaccinatedStr);
                    }
                    if (row["Сhildren"] != null || row["Сhildren"] == System.DBNull.Value)
                    {
                        string СhildrenStr = row["Сhildren"].ToString();
                        if (СhildrenStr.Length > 0) Сhildren = Convert.ToInt64(СhildrenStr);
                    }

                    var VV=_context.Covid19_Vaccinate.Where(w => w.Id == Ids).Single();
                    if (VV.InputFirst != InputFirst) VV.InputFirst = InputFirst == 0 ? null : InputFirst;
                    if (VV.InputSecond != InputSecond) VV.InputSecond = InputSecond == 0 ? null : InputSecond;
                    if (VV.InputRevaccinated != InputRevaccinated) VV.InputRevaccinated = InputRevaccinated == 0 ? null : InputRevaccinated;
                    if (VV.InputСhildren != InputСhildren) VV.InputСhildren = InputСhildren == 0 ? null : InputСhildren;
                    if ((VV.First == 0 || VV.First==null) && (First == 0 || First==null) && InputFirst>0) {
                        VV.First = InputFirst;
                    }
                    else  if (VV.First != First) VV.First = First==0?null:First;
                    if ((VV.Second == 0 || VV.Second ==null) && (Second == 0 || Second==null) && InputSecond > 0)
                    {
                        VV.Second = InputSecond;
                    }
                    else if(VV.Second != Second) VV.Second = Second == 0 ? null : Second;
                    if ((VV.Revaccinated == 0|| VV.Revaccinated==null) && (Revaccinated == 0|| Revaccinated==null) && InputRevaccinated > 0)
                    {
                        VV.Revaccinated = InputRevaccinated;
                    }
                    else if (VV.Revaccinated != Revaccinated) VV.Revaccinated = Revaccinated == 0 ? null : Revaccinated;
                    if ((VV.Сhildren == 0 || VV.Сhildren == null) && (Сhildren == 0 || Сhildren == null) && InputСhildren > 0)
                    {
                        VV.Сhildren = InputСhildren;
                    }
                    else if (VV.Сhildren != Сhildren) VV.Сhildren = Сhildren == 0 ? null : Сhildren;

                    VV.UserLastUpdate = userGuid;
                    VV.UserLastUpdateDate = DateTime.Now;

                    var prod = _context.Covid19_Product.ToList();
                    foreach (var pr in prod) {
                        decimal? PR_Dosage =  row[pr.Classifer_Id.ToString()] != null || row[pr.Classifer_Id.ToString()] == System.DBNull.Value ? Convert.ToDecimal(row[pr.Classifer_Id.ToString()].ToString()) : 0; 
                        string PR_Link = row[pr.Classifer_Id.ToString() + "_Link"] != null ? row[pr.Classifer_Id.ToString() + "_Link"].ToString() : null; 
                        var VPR = _context.Covid19_Vaccinate_Product.Where(w => w.Vaccinate_Id == Ids && w.Classifer_Id == pr.Classifer_Id);

                        if (VPR.Count() >= 1) {
                            var VPR1=VPR.Single();
                            if (PR_Dosage.GetValueOrDefault(0) == 0) {
                                VPR1.Dosage = null;
                            }
                            else if (VPR1.Dosage != PR_Dosage) VPR1.Dosage = PR_Dosage;
                            if (VPR1.Link != PR_Link) VPR1.Link = PR_Link;
                            VPR1.UserLastUpdate = userGuid;
                            VPR1.UserLastUpdateDate = DateTime.Now;
                                }
                        if (VPR.Count() == 0 && (PR_Dosage>0 || PR_Link!= null && PR_Link.Length>0)&& pr.Classifer_Id!= 346573)
                        {
                            _context.Covid19_Vaccinate_Product.Add(new Domain.Model.GovernmentPurchases.Covid19_Vaccinate_Product { Vaccinate_Id = Ids, Classifer_Id = pr.Classifer_Id, Dosage = PR_Dosage, Link = PR_Link, UserLastUpdate= userGuid, UserLastUpdateDate=DateTime.Now });
                        }
                        
                    }

                 
                }   
                _context.SaveChanges();
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
        public ActionResult addRawProduct()
        {
            
          //  var Product = new Domain.Model.GovernmentPurchases.Covid19_Product() { };
            var userGuid = new Guid(User.Identity.GetUserId());
            //     Product.UserLastUpdate = userGuid;
            //   Product.UserLastUpdateDate = DateTime.Now;
            var Product = _context.Covid19_Product;
            Product.Add(new Domain.Model.GovernmentPurchases.Covid19_Product { Classifer_Id = Int64.Parse("0"), Drug = "", DrugShort = "", Package = 0, UserLastUpdate= userGuid, UserLastUpdateDate= DateTime.Now  });


            _context.SaveChanges();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult RemoveRawProduct(long id)
        {
            var userGuid = new Guid(User.Identity.GetUserId());
            var product = _context.Covid19_Product.Where(c => c.Classifer_Id == id).Single();
            _context.Covid19_Product_History.Add(new Domain.Model.GovernmentPurchases.Covid19_Product_History { Classifer_Id=product.Classifer_Id, Drug=product.Drug, DrugShort=product.DrugShort, Package=product.Package, UserLastUpdate= userGuid, UserLastUpdateDate=product.UserLastUpdateDate });
         //   var ret = _context.Database.SqlQuery<string>("delete from [GovernmentPurchases].[dbo].[Covid19_Product] where [Classifer_Id]=@prId", new SqlParameter("@prId", id));
                    
            _context.Covid19_Product.Remove(product);
            _context.SaveChanges();



            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };


            return jsonNetResult;
        }


        
        [HttpPost]
        public ActionResult RawProduct_save(ICollection<Domain.Model.GovernmentPurchases.Covid19_Product> array_Raw )
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());
            if (array_Raw != null)
                    foreach (var item in array_Raw)
                    {
                        var upd = _context.Covid19_Product.Where(w => w.Classifer_Id == item.Classifer_Id).Single();

                        if (upd.Classifer_Id!=item.Classifer_Id) upd.Classifer_Id = item.Classifer_Id;
                        if (upd.Drug != item.Drug) upd.Drug = item.Drug;
                        if (upd.DrugShort != item.DrugShort) upd.DrugShort = item.DrugShort;
                        if (upd.Package != item.Package) upd.Package = item.Package;
                        upd.UserLastUpdate = userGuid;
                        upd.UserLastUpdateDate = DateTime.Now;                   
                    }
                _context.SaveChanges();
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
        public ActionResult addRawPrice()
        {
            var userGuid = new Guid(User.Identity.GetUserId());
            var Product = new Domain.Model.GovernmentPurchases.Covid19_Product_Price() { Classifer_Id = 0, DateBegin= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) , DateEnd= new DateTime(2100, 1, 1), Price=0, UserLastUpdate= userGuid, UserLastUpdateDate=DateTime.Now };
    
            _context.Covid19_Product_Price.Add(Product);

            _context.SaveChanges();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = Product
            };

            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult RemoveRawPrice(long id)
        {
            var userGuid = new Guid(User.Identity.GetUserId());
            var product = _context.Covid19_Product_Price.Where(c => c.Id == id).Single();
            _context.Covid19_Product_Price_History.Add(new Domain.Model.GovernmentPurchases.Covid19_Product_Price_History { Id=product.Id, Price=product.Price, DateBegin= product.DateBegin, DateEnd=product.DateEnd, Classifer_Id= product.Classifer_Id, UserLastUpdate = userGuid, UserLastUpdateDate = product.UserLastUpdateDate });
          //  var ret = _context.Database.SqlQuery<string>("delete from [GovernmentPurchases].[dbo].[Covid19_Product_Price]   where id=@prId", new SqlParameter("@prId", id));
         
            _context.Covid19_Product_Price.Remove(product);
            _context.SaveChanges();



            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };


            return jsonNetResult;
        }


        [HttpPost]
        public ActionResult RawPrice_save(ICollection<Domain.Model.GovernmentPurchases.Covid19_Product_Price> array_Raw)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());
                if (array_Raw != null)
                    foreach (var item in array_Raw)
                    {
                        var upd = _context.Covid19_Product_Price.Where(w => w.Id == item.Id).Single();

                        if (upd.Price != item.Price) upd.Price = item.Price;
                        if (upd.Classifer_Id != item.Classifer_Id) upd.Classifer_Id = item.Classifer_Id;
                        if (upd.DateBegin != item.DateBegin) upd.DateBegin = item.DateBegin;
                        if (upd.DateEnd != item.DateEnd) upd.DateEnd = item.DateEnd;                       
                        upd.UserLastUpdate = userGuid;
                        upd.UserLastUpdateDate = DateTime.Now;
                    }
                _context.SaveChanges();
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


        /*
        [HttpPost]
        public ActionResult HandMadePosition_save(
            ICollection<DataAggregator.Domain.Model.GovernmentPurchases.OrganizationRaw> array_Raw
            )
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);
                if (array_Raw != null)
                    foreach (var item in array_Raw)
                    {
                        var upd = _context.OrganizationRaw.Where(w => w.Id == item.Id).Single();

                        if (item.OrganizationId == 0) item.OrganizationId = null;
                        if (item.UserId == 0) item.UserId = null;

                        upd.OrganizationId = item.OrganizationId;
                        upd.UserId = item.UserId;
                        upd.IsTrash = item.IsTrash;
                        upd.DateUpdate = DateTime.Now;
                    }

                _context.SaveChanges();
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
        */
    }
}