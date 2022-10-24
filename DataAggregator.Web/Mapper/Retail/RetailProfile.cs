using AutoMapper;
using DataAggregator.Domain.Model.Retail.CTM;
using DataAggregator.Web.Models.Retail.CTM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Mapper.Retail
{

    public sealed class RetailProfile : Profile
    {
        public RetailProfile()
        {
            CreateMap<CTMView, CTMModel>();
        }
    }
}