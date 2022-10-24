using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class Drug
    {
        public long Id { get; set; }       

        public long FormProductId { get; set; }

        public long? DosageGroupId { get; set; }


        public long DrugTypeId { get; set; }

        public int? ConsumerPackingCount { get; set; }

        public bool UseShortDescription { get; set; }

        public long? EquipmentId { get; set; }
        public Byte EIId { get; set; }
        public decimal StandardUnits { get; set; }
        public decimal StandardUnits_Hand { get; set; }
        public bool StandardUnits_Ckeck { get; set; }
        public virtual Equipment Equipment { get; set; }

        public long TradeNameId { get; set; }
        public virtual TradeName TradeName { get; set; }

        public long? INNGroupId { get; set; }
        public virtual INNGroup INNGroup { get; set; }

        public virtual IList<RealPacking> RealPacking { get; set; }

        public virtual FormProduct FormProduct { get; set; }

        public virtual DosageGroup DosageGroup { get; set; }

        public virtual IList<Systematization.DrugClassifier> DrugClassifier { get; set; }

        public virtual IList<Systematization.DrugClassifierInWork> DrugClassifierInWork { get; set; }

        public virtual ICollection<ProductionInfo> ProductionInfo { get; set; }

        public virtual DrugType DrugType { get; set; }
       
    }

}