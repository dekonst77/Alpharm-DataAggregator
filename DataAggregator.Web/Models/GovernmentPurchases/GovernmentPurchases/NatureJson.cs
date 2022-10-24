using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class NatureJson
    {
        public NatureJson()
        {
            
        }

        public NatureJson(Nature nature)
        {
            if (nature == null)
            {
                throw new ArgumentNullException("nature");
            }

            Id = nature.Id;
            CategoryId = nature.CategoryId;
            Name = Name;
        }

        public Byte? Id { get; set; }

        public Byte? CategoryId { get; set; }

        public string Name { get; set; }
    }
    public class Nature_L2Json
    {
        public Nature_L2Json()
        {

        }

        public Nature_L2Json(Nature_L2 nature)
        {
            if (nature == null)
            {
                Id =null;
                Nature_L1Id = null;
                Name ="[пустой]";
            }
            else
            {
                Id = nature.Id;
                Nature_L1Id = nature.Nature_L1Id;
                Name = nature.Name + " [" + nature.Nature_L1.Name+"]";
            }
        }

        public Int16? Id { get; set; }

        public Byte? Nature_L1Id { get; set; }

        public string Name { get; set; }
    }
}