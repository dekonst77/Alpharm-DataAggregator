using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataAggregator.Core.Models.Classifier
{
    public class ClassifierEditorModelJson
    {
        public long ClassifierId { get; set; }
        public long DrugId { get; set; } // код товара
        public long OwnerTradeMarkId { get; set; } // производитель-Правообладатель

        public long PackerId { get; set; }//упаковщик

        public long ProductionInfoId { get; set; }

        public DictionaryJson DrugType { get; set; }

        public ICollection<ClassifierPackingBlisterBlockJson> ClassifierPackings { get; set; }

        public DictionaryJson OwnerTradeMark { get; set; }

        public DictionaryJson Packer { get; set; }

        public DictionaryJson TradeName { get; set; }
        public DictionaryJson Brand { get; set; }

        public DictionaryJson FormProduct { get; set; }

        public DictionaryJson Nfc { get; set; }
        public DictionaryJson ATCWho { get; set; }
        public DictionaryJson ATCEphmra { get; set; }

        public DictionaryJson Generic { get; set; }
        public DictionaryJson ATCBaa { get; set; }
        public DictionaryJson FTG { get; set; }
        public DictionaryJson Equipment { get; set; }

        public DictionaryJson TotalVolume { get; set; }
        public string TotalVolumeCount { get; set; }
        public DictionaryJson DosageValue { get; set; }
        public string DosageValueCount { get; set; }

        public int? ConsumerPackingCount { get; set; }

        public string InnGroupDosageDescription { get; set; }

        public string DosageGroupDescription { get; set; }
        public string ShortDosageGroupDescription { get; set; }

        public List<INNGroupDosageJson> InnGroupDosage { get; set; }

        //Признак того, что МНН является составом
        public bool IsCompound { get; set; }

        //Признак того, что МНН является составом БАД
        public bool IsCompoundBAA { get; set; }

        public List<RealPackingCountJson> RealPackingList { get; set; }

        public RegistrationCertificateJson RegistrationCertificate { get; set; }

        /// <summary>
        /// Без РУ
        /// </summary>
        public bool WithoutRegistrationCertificate { get; set; }

        // public DictionaryJson OwnerRegistrationCertificate { get; set; }

        public bool Used { get; set; }
        public string Comment { get; set; }
        public bool IsOtc { get; set; }
        public bool IsRx { get; set; }

        public bool IsExchangeable { get; set; }
        public bool IsReference { get; set; }


        public bool UseShortDescription { get; set; }
        //Цена ЖНВЛП или Эталонная
        public string Price { get; set; }

        //1 - Цена ЖНВЛП 2 - Цена Эталонная 0 -Не задана
        public int? PriceSourceId { get; set; }

        public DictionaryJson ProductionStage { get; set; }
        public DictionaryJson ProductionLocalization { get; set; } // Локализация ЛС
        public DictionaryJson PackerLocalization { get; set; } // Локализация упаковщика

        public void Restore()
        {
            foreach (var packing in ClassifierPackings)
            {
                if (packing.ConsumerPacking == null)
                    packing.ConsumerPacking = new Packing() { Id = null, Value = null };
                if (packing.PrimaryPacking == null)
                    packing.PrimaryPacking = new Packing() { Id = null, Value = null };
                packing.ConsumerPacking.Value = RestoreString(packing.ConsumerPacking.Value);
                packing.PrimaryPacking.Value = RestoreString(packing.PrimaryPacking.Value);
            }

            if (InnGroupDosage != null)
            {
                foreach (var inn in InnGroupDosage)
                {
                    inn.INN.Value = RestoreString(inn.INN.Value);

                    if (inn.Dosage == null)
                        inn.Dosage = new DictionaryJson();

                    inn.Dosage.Value = RestoreString(inn.Dosage.Value);
                    inn.DosageCount = RestoreString(inn.DosageCount);

                }
            }

            TotalVolume.Value = RestoreString(TotalVolume.Value);
            TotalVolumeCount = RestoreString(TotalVolumeCount);
            DosageValue.Value = RestoreString(DosageValue.Value);
            DosageValueCount = RestoreString(DosageValueCount);
        }

        //Тут мы меняем ~ на пусто
        public void Clear()
        {
            if (RegistrationCertificate != null && RegistrationCertificate.IsNull())
            {
                RegistrationCertificate = null;
            }

            if (ProductionStage != null && ProductionStage.Id == 0)
            {
                ProductionStage = null;
            }

            Check();

            if (ClassifierPackings != null)
            {
                foreach (var packing in ClassifierPackings)
                {
                    packing.ConsumerPacking.Value = ClearString(packing.ConsumerPacking.Value);
                    packing.PrimaryPacking.Value = ClearString(packing.PrimaryPacking.Value);
                }
            }
            if (InnGroupDosage != null)
            {
                foreach (var inn in InnGroupDosage)
                {
                    inn.INN.Value = ClearString(inn.INN.Value);
                    inn.Dosage.Value = ClearString(inn.Dosage.Value);
                    inn.DosageCount = ClearString(inn.DosageCount);

                }
            }

            //Удаляем пустые МНН
            if (InnGroupDosage != null)
            {
                var innList = InnGroupDosage.Where(d => string.IsNullOrEmpty(d.INN.Value)).ToList();

                foreach (var inn in innList)
                {
                    InnGroupDosage.Remove(inn);
                }

            }

            if (RegistrationCertificate != null)
            {

                RegistrationCertificate.Number = ClearString(RegistrationCertificate.Number);
                if (RegistrationCertificate.CirculationPeriod != null)
                {
                    RegistrationCertificate.CirculationPeriod.Value = ClearString(RegistrationCertificate.CirculationPeriod.Value);
                }

                // RegistrationCertificate.url = ClearString(RegistrationCertificate.url);
            }

            InnGroupDosageDescription = ClearString(InnGroupDosageDescription);
            OwnerTradeMark.Value = ClearString(OwnerTradeMark.Value);
            Packer.Value = ClearString(Packer.Value);
            TradeName.Value = ClearString(TradeName.Value);
            FormProduct.Value = ClearString(FormProduct.Value);
            TotalVolume.Value = ClearString(TotalVolume.Value);
            TotalVolumeCount = ClearString(TotalVolumeCount);
            DosageValue.Value = ClearString(DosageValue.Value);
            DosageValueCount = ClearString(DosageValueCount);
            Brand.Value = ClearString(Brand.Value);
        }

        private string RestoreString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "~";

            return value;
        }

        private string ClearString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            //Замена не разрывных пробелов на обычные
            value = Regex.Replace(value, @"\u00A0", " ");
            //Удаление двойных пробелов
            while (value.Contains("  "))
            {
                value = value.Replace("  ", " ").Trim();
            }
            if (string.Equals(value, "~"))
                return null;

            return value.Trim();
        }

        private void Check()
        {
            if (DrugType == null)
                throw new ApplicationException("Должен быть указан Тип");

            if (DrugType.Id == 1 &&
                RegistrationCertificate == null &&
                OwnerTradeMark.Value != "Unknown" &&
                Packer.Value != "Unknown" && !WithoutRegistrationCertificate)
            {
                throw new ApplicationException("Для типа ЛС должен быть выбран Регистрационный сертификат");
            }

            if (DrugType.Id == 1 &&
                RegistrationCertificate != null &&
                OwnerTradeMark.Value == "Unknown" &&
                Packer.Value == "Unknown")
            {
                throw new ApplicationException("Для типа ЛС и Unknown недопустим Регистрационный сертификат");
            }

            if (DrugType.Id != 1 && DrugType.Id != 4 && RegistrationCertificate != null)
                throw new ApplicationException("Для типа не ЛС регистрационный сертификат не допустим");
        }
    }
}