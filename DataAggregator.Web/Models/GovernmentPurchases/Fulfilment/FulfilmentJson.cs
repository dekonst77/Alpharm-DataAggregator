using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.GovernmentPurchases.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DataAggregator.Web.Models.GovernmentPurchases.Fulfilment
{
    public class FulfilmentJson
    {
        public FulfilmentJson()
        {
        }
        public string Type { get; set; }
        public long Id { get; set; }
        public string Number { get; set; }
        public long ContractId { get; set; }
        public string ReestrNumber { get; set; }
        public decimal? ContractSum { get; set; }
        public decimal? ActuallyPaid { get; set; }
        public decimal? SumIsp { get; set; }
        public long ObjectId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        public decimal? Sum { get; set; }
        public long? ClassifierId { get; set; }
        public string INNGroup { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public string Corporation { get; set; }
        public string Packer { get; set; }
        public decimal? ObjectCalculatedAmount { get; set; }
        public decimal? ObjectCalculatedPrice { get; set; }
        public decimal? ObjectCalculatedSum { get; set; }
        public string Seria { get; set; }
        public string INNGroupIsp { get; set; }
        public string TradeNameIsp { get; set; }
        public string DrugDescriptionIsp { get; set; }
        public string ProvisorAction { get; set; }
        public long? contractQuantityId { get; set; }
        //элементы корректировки
        public long? uClassifierId { get; set; }
        public decimal? uObjectCalculatedAmount { get; set; }
        public decimal? uObjectCalculatedPrice { get; set; }
        public string uUserGuid { get; set; }
        public DateTime? uEditDate { get; set; }
        public int? uStatus { get; set; }
        //копия оригинальных значений
        public long? oClassifierId { get; set; }
        public decimal? oObjectCalculatedAmount { get; set; }
        public decimal? oObjectCalculatedPrice { get; set; }
        public decimal? oObjectCalculatedSum { get; set; }
        public string UserName { get; set; }
    }
}