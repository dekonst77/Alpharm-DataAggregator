﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.ExternalDataRetail.external
{
    [Table("ExternalLog", Schema = "External")]
    public class ExternalLog
    {
        public long Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }

        public string Method { get; set; }

        public string PeriodList { get; set; }

        public string RegionCodeList { get; set; }

        public string DrugParametersList { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public bool IsError { get; set; }

        public long RequestId { get; set; }

        public string Value { get; set; }

        public long? TradeNameId { get; set; }

        public long? FormProductId { get; set; }

        public long? DosageId { get; set; }
    }

}