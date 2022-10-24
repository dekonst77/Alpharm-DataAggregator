using System;
using System.Collections.Generic;
using System.Linq;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class PurchaseNatureMixedModel
    {

        public decimal FlPercentage { get; set; }
        public decimal RlPercentage { get; set; }
        public decimal BlPercentage { get; set; }
        public decimal RcpPercentage { get; set; }
        public decimal FcpPercentage { get; set; }
        public decimal BmpPercentage { get; set; }
        public decimal PalliativeCarePercentage { get; set; }
        public decimal apuPercentage { get; set; }
        public decimal fz223Percentage { get; set; }
        public Nature_L2Json FlNature_L2Id { get; set; }
        public Nature_L2Json RlNature_L2Id { get; set; }
        public Nature_L2Json BlNature_L2Id { get; set; }
        public Nature_L2Json RcpNature_L2Id { get; set; }
        public Nature_L2Json FcpNature_L2Id { get; set; }
        public Nature_L2Json BmpNature_L2Id { get; set; }
        public Nature_L2Json PalliativeCareNature_L2Id { get; set; }
        public Nature_L2Json apuNature_L2Id { get; set; }

        public Nature_L2Json fz223Nature_L2Id { get; set; }

        public PurchaseNatureMixedModel()
        {
            
        }

        public PurchaseNatureMixedModel(List<PurchaseNatureMixed> model)
        {

            if (model != null)
            {
                // ФЛ - Федеральная льгота 14
                var flPercentageModel = model.SingleOrDefault(m => m.NatureId == 14);
                //  РЛ - Региональная льгота 16
                var rlPercentageModel = model.SingleOrDefault(m => m.NatureId == 16);
                //  Больницы 3
                var blPercentageModel = model.SingleOrDefault(m => m.NatureId == 3);
                //РЦП - Региональные ЦП 8
                var rcpPercentageModel = model.SingleOrDefault(m => m.NatureId == 8);
                // ФЦП - Федеральные ЦП 7
                var fcpPercentageModel = model.SingleOrDefault(m => m.NatureId == 7);
             
                //ВМП 20
                var bmpPercentageModel = model.SingleOrDefault(m => m.NatureId == 20);

                //Паллиат.помощь 427, 428
                var palliativeCareModel = model.SingleOrDefault(m => m.NatureId == 22);

                var apuModel = model.SingleOrDefault(m => m.NatureId == 5);

                var fz223Model = model.SingleOrDefault(m => m.NatureId == 1);

                FlPercentage = flPercentageModel != null ? flPercentageModel.Percentage : 0;
                RlPercentage = rlPercentageModel != null ? rlPercentageModel.Percentage : 0;
                BlPercentage = blPercentageModel != null ? blPercentageModel.Percentage : 0;
                RcpPercentage = rcpPercentageModel != null ? rcpPercentageModel.Percentage : 0;
                FcpPercentage = fcpPercentageModel != null ? fcpPercentageModel.Percentage : 0;
                BmpPercentage = bmpPercentageModel != null ? bmpPercentageModel.Percentage : 0;
                PalliativeCarePercentage = palliativeCareModel != null ? palliativeCareModel.Percentage : 0;
                apuPercentage = apuModel != null ? apuModel.Percentage : 0;
                fz223Percentage = fz223Model != null ? fz223Model.Percentage : 0;

                FlNature_L2Id = new Nature_L2Json( flPercentageModel != null ? flPercentageModel.Nature_L2 : null);
                RlNature_L2Id = new Nature_L2Json(rlPercentageModel != null ? rlPercentageModel.Nature_L2 : null);
                BlNature_L2Id = new Nature_L2Json(blPercentageModel != null ? blPercentageModel.Nature_L2 : null);
                RcpNature_L2Id = new Nature_L2Json(rcpPercentageModel != null ? rcpPercentageModel.Nature_L2 : null);
                FcpNature_L2Id = new Nature_L2Json(fcpPercentageModel != null ? fcpPercentageModel.Nature_L2 : null);
                BmpNature_L2Id = new Nature_L2Json(bmpPercentageModel != null ? bmpPercentageModel.Nature_L2 : null);
                PalliativeCareNature_L2Id = new Nature_L2Json(palliativeCareModel != null ? palliativeCareModel.Nature_L2 : null);
                apuNature_L2Id = new Nature_L2Json(apuModel != null ? apuModel.Nature_L2 : null);
                fz223Nature_L2Id = new Nature_L2Json(fz223Model != null ? fz223Model.Nature_L2 : null);

            }


        }

        public List<PurchaseNatureMixed> GetPurchaseNatureMixed()
        {
            List<PurchaseNatureMixed> result = new List<PurchaseNatureMixed>();

            // ФЛ - Федеральная льгота 14
            if(FlPercentage > 0)
                result.Add(new PurchaseNatureMixed() {NatureId = 14, Percentage = FlPercentage,Nature_L2Id=FlNature_L2Id.Id});

            //  РЛ - Региональная льгота 16
            if (RlPercentage > 0)
                result.Add(new PurchaseNatureMixed() { NatureId = 16, Percentage = RlPercentage, Nature_L2Id =RlNature_L2Id.Id });

            //Больницы 3
            if (BlPercentage > 0)
                result.Add(new PurchaseNatureMixed() { NatureId = 3, Percentage = BlPercentage, Nature_L2Id = BlNature_L2Id.Id });

            //РЦП - Региональные ЦП 8
            if (RcpPercentage > 0)
                result.Add(new PurchaseNatureMixed() { NatureId = 8, Percentage = RcpPercentage, Nature_L2Id =RcpNature_L2Id.Id });

            // ФЦП - Федеральные ЦП 7
            if (FcpPercentage > 0)
                result.Add(new PurchaseNatureMixed() { NatureId = 7, Percentage = FcpPercentage, Nature_L2Id =FcpNature_L2Id.Id });
            
            //  ВМП 20
            if (BmpPercentage > 0)
                result.Add(new PurchaseNatureMixed() { NatureId = 20, Percentage = BmpPercentage, Nature_L2Id = BmpNature_L2Id.Id });

            //  ВМП 20
            if (PalliativeCarePercentage > 0)
                result.Add(new PurchaseNatureMixed() { NatureId = 22, Percentage = PalliativeCarePercentage, Nature_L2Id =PalliativeCareNature_L2Id.Id });

            if (apuPercentage > 0)
                result.Add(new PurchaseNatureMixed() { NatureId = 5, Percentage = apuPercentage, Nature_L2Id = apuNature_L2Id.Id });
            
            if (fz223Percentage > 0)
                result.Add(new PurchaseNatureMixed() { NatureId = 1, Percentage = fz223Percentage, Nature_L2Id = fz223Nature_L2Id.Id });
            return result;
        }
    }
}