using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.OFD
{
    public class EtalonPriceViewData_SP_Result
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> ClassifierId { get; set; }
        public string TradeName { get; set; }
        public string INNGroup { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public Nullable<decimal> PriceVED { get; set; }
        public string Type { get; set; }
        public bool Used { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public string Comment { get; set; }
        // контракты
        public Nullable<decimal> Contract_PriceAVG { get; set; }
        //Комментарий
        public string CommentStatus { get; set; }
        public int? CommentStatusId { get; set; }
        //Комментарий - 1
        public string PrevCommentStatus { get; set; }
        public int? PrevCommentStatusId { get; set; }
        //исходные данные
        public Nullable<decimal> Rigla_PriceAVG { get; set; }
        public Nullable<decimal> Group366_PriceAVG { get; set; }
        public Nullable<decimal> Maksavit_PriceAVG { get; set; }
        public Nullable<decimal> Neopharm_PriceAVG { get; set; }
        public Nullable<decimal> Aloe_PriceAVG { get; set; }
        public Nullable<decimal> Vita_PriceAVG { get; set; }
        public Nullable<decimal> Farmaimpeks_PriceAVG { get; set; }
        //парсинг
        public Nullable<decimal> Aptekaru_PriceAVG { get; set; }
        public Nullable<decimal> Zdravcity_PriceAVG { get; set; }
        public Nullable<decimal> Uteka_PriceAVG { get; set; }
        public Nullable<decimal> Eapteka_PriceAVG { get; set; }
        //ОФД
        public Nullable<decimal> OFD1_PriceAVG { get; set; }
        public Nullable<decimal> Platformaofd_PriceAVG { get; set; }
        public Nullable<decimal> OFDYa_PriceAVG { get; set; }
        public Nullable<decimal> Taxcom_PriceAVG { get; set; }
        public Nullable<decimal> Kontur_PriceAVG { get; set; }
        public Nullable<decimal> Initpro_PriceAVG { get; set; }
        //расчётные данные
        //сумма, средняя цена, средняя цена с учётом выбросов, кол-во источников после выброса: Исходники
        public Nullable<decimal> Initial_Sum { get; set; }
        public Nullable<decimal> Initial_PriceAVG { get; set; }
        //public Nullable<int> Initial_SourceCount { get; set; }
        //сумма, средняя цена, средняя цена с учётом выбросов, кол-во источников после выброса: ОФД
        public Nullable<decimal> OFD_Sum { get; set; }
        public Nullable<decimal> OFD_PriceAVG { get; set; }
        //public Nullable<int> OFD_SourceCount { get; set; }
        //Цена sell in
        public Nullable<decimal> SellIn_PriceAVG { get; set; }
        //средняя цена по сайтам
        public Nullable<decimal> Downloaded_PriceAVG { get; set; }
        //расчетная цена sell out
        public Nullable<decimal> SellOut_PriceCalc { get; set; }
        public Nullable<decimal> PriceCalc { get; set; }
        public Nullable<decimal> PricePrev { get; set; }
        public Nullable<decimal> DeviationPercent { get; set; }
    }
}
