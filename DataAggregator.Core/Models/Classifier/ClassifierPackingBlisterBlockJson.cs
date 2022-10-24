using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Core.Models.Classifier
{
    public class ClassifierPackingBlisterBlockJson
    {
        public int Id { get; set; }
        public long ClassifierId { get; set; }
        public long? PrimaryPackingId { get; set; }
        public virtual Packing PrimaryPacking { get; set; }
        public int CountInPrimaryPacking { get; set; }
        public long? ConsumerPackingId { get; set; }
        public virtual Packing ConsumerPacking { get; set; }
        public int CountPrimaryPacking { get; set; }
        public string PackingDescription { get; set; }
        public bool? IsBlisterPacking { get; set; }
        public ClassifierPackingBlisterBlockJson()
        {
            this.Id = 0;
            this.ClassifierId = 0;
            this.PrimaryPackingId = null;
            this.PrimaryPacking = new Packing();
            this.CountInPrimaryPacking = 0;
            this.ConsumerPackingId = null;
            this.ConsumerPacking = new Packing();
            this.CountPrimaryPacking = 0;
            this.PackingDescription = String.Empty;
            this.IsBlisterPacking = false;
        }
        public ClassifierPackingBlisterBlockJson(ClassifierPacking packing, bool IsBlister)
        {
            this.Id = packing.Id;
            this.ClassifierId = packing.ClassifierId;
            //this.CI = packing?.CI;
            this.CountInPrimaryPacking = packing.CountInPrimaryPacking;
            this.CountPrimaryPacking = packing.CountPrimaryPacking;
            this.PackingDescription = packing?.PackingDescription;
            this.IsBlisterPacking = IsBlister;

            this.PrimaryPackingId = packing?.PrimaryPackingId;
            this.PrimaryPacking = packing?.PrimaryPacking;

            this.ConsumerPackingId = packing?.ConsumerPackingId;
            this.ConsumerPacking = packing?.ConsumerPacking;
        }

        public static implicit operator ClassifierPacking(ClassifierPackingBlisterBlockJson el)
        {
            if (el == null)
                return null;

            return new ClassifierPacking()
            {
                Id = el.Id,
                ClassifierId = el.ClassifierId,

                PrimaryPackingId = el?.PrimaryPackingId,
                PrimaryPacking = el?.PrimaryPacking,

                CountInPrimaryPacking = el.CountInPrimaryPacking,

                ConsumerPackingId = el?.ConsumerPackingId,
                ConsumerPacking = el?.ConsumerPacking,

                CountPrimaryPacking = el.CountPrimaryPacking,

                PackingDescription = el?.PackingDescription
            };
        }
    }
}

