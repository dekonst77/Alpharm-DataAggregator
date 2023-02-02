using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Classifier
{
    public static class ProductionInfoController
    {
        private class Change
        {
            public ProductionInfo From { get; set; }
            public ProductionInfo To { get; set; }
        }


        //Действия при изменении ProductionInfo
        public static void ChangeProductionInfo(ProductionInfo from, ProductionInfo to, Guid userId, DrugClassifierContext context)
        {
            //Список того, что на что поменяется

            //Добавляем текущую замену
            var changes = new List<Change> { new Change() { From = from, To = to } };

            //Добавляем список каскадных измненений
            changes.AddRange(CheckRegistrationCertificate(from, to, context, userId));

            //Для каждого изменения 
            foreach (var change in changes)
            {
                Action(change.From, change.To, userId, context);
            }

            //Так как ранее во всех изменениях в ProductionInfo поле ProductionStage уже обновлено, то новое значение можем взять оттуда, для каскадного изменения.
            //Field ProductionStage already updated early, and we can get new value this field from "to"
            ProductionInfoController.ChangeProductionStage(from ?? to, to.ProductionStageId, context);
        }

        //Обновляем ProductionStage для всех изменений Ру + Packer
        //Updated field ProductionStage for all combination Ru + Packer
        private static void ChangeProductionStage(ProductionInfo productionInfo, long? toProductionStageId, DrugClassifierContext context)
        {

            if (productionInfo.RegistrationCertificateId != null)
            {

                //Получаем список, где есть такие же Ru и Packer
                context.ProductionInfo.Where(p =>
                                                            p.PackerId == productionInfo.PackerId &&
                                                            p.RegistrationCertificateId == productionInfo.RegistrationCertificateId)
                                      .ToList()
                                      .ForEach(f => f.ProductionStageId = toProductionStageId);


               

            }
            else
            {
                //Для ProductionInfo без РУ обновляем как есть.
                productionInfo.ProductionStageId = toProductionStageId;
            }



            context.SaveChanges();

        }

        //Действия 
        private static void Action(ProductionInfo from, ProductionInfo to, Guid userId, DrugClassifierContext context)
        {
            //Изменяем таблицу ClassifierInfo
            ClassifierInfoController.Change(from, to, userId, context);

            //Учитвыаем изменения в DrugClassification
            DrugClassificationController.Change(from, to, context);

            ClassificationGenericController.Change(from, to, context);

            //Перепривязываем данные при изменении привязки
            ReClassifierController.ReClassifier(from, to, userId, context);
        }

        //Каскадное изменение OwnerTradeMark и Packer
        private static List<Change> CheckRegistrationCertificate(ProductionInfo from, ProductionInfo to, DrugClassifierContext context, Guid userId)
        {
            //Добавление
            if (from == null)
                return new List<Change>();

            //Тут должна быть провекрка и для каждого изменения опять все действия
            if (from.DrugId != to.DrugId || from.Id != to.Id)
                return new List<Change>();

            if (from.OwnerTradeMarkId == to.OwnerTradeMarkId && from.PackerId == to.PackerId)
                return new List<Change>();

            if (from.RegistrationCertificate == null)
                return new List<Change>();

            var registrationCertificate = from.RegistrationCertificate;

            var productionInfos = context.ProductionInfo.Where(p =>
                    p.RegistrationCertificate != null &&
                    p.RegistrationCertificate.Id == registrationCertificate.Id &&
                    (p.PackerId == from.PackerId || p.OwnerTradeMarkId == from.OwnerTradeMarkId) && p.Id != from.Id).ToList();

            var changes = new List<Change>();

            foreach (var productionInfo in productionInfos)
            {
                var change = new Change
                {
                    //Делаем копию старых значений
                    From = productionInfo.Copy(),
                    //Делаем 
                    To = productionInfo
                };

                if (productionInfo.PackerId == from.PackerId)
                    productionInfo.PackerId = to.PackerId;

                if (productionInfo.OwnerTradeMarkId == from.OwnerTradeMarkId)
                    productionInfo.OwnerTradeMarkId = to.OwnerTradeMarkId;

                //Логируем изменения
                LogAction.Log(context, productionInfo.Id, LogAction.ActionType.Change, userId);

                changes.Add(change);

            }

            return changes;
        }



    }


}
