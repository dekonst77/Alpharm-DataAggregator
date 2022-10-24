using DataAggregator.Domain.Utils;
using Newtonsoft.Json;
using System;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class DistributionWork
    {
        /// <summary>
        /// Идентификатор закупки
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Номер закупки
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Наименование объекта закупки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата объявления
        /// </summary>
        public DateTime DateBegin { get; set; }

        /// <summary>
        /// Дата аукциона
        /// </summary>
         [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Идентификатор класса закупки
        /// </summary>
        public Byte? PurchaseClassId { get; set; }

        /// <summary>
        /// Дата создания закупки 
        /// </summary>
        public DateTime PurchaseDateCreate { get; set; }

        /// <summary>
        /// Пользователь, изменивший Описание закупки последним
        /// </summary>
        public string LastChangedUser { get; set; }

        /// <summary>
        /// Дата последнего изменения Описания закупки 
        /// </summary>
        public DateTime? LastChangedDate { get; set; }

        /// <summary>
        /// Класс закупки (Нет/ЛС/на проверку/на удаление)
        /// </summary>
        public string PurchaseClass { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому назначена закупка
        /// </summary>
        public Guid? AssignedToUserId { get; set; }

        /// <summary>
        /// Пользователь, которому назначена закупка
        /// </summary>
        public string AssignedToUser { get; set; }

        /// <summary>
        /// Идентификатор пользователя, установившего класс закупки
        /// </summary>
        public Guid? PurchaseClassUserId { get; set; }

        /// <summary>
        /// Пользователь, установивший класс закупки
        /// </summary>
        public string PurchaseClassUser { get; set; }

        /// <summary>
        /// URL закупки на zakupki.gov.ru
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Сумма всех лотов закупки
        /// </summary>
        public decimal? LotSum { get; set; }

        /// <summary>
        /// Статус закупки
        /// </summary>
        public Int16 StatusId { get; set; }

        /// <summary>
        /// Пользователь который делает закупку
        /// </summary>
        public string UserInWork { get; set; }

        /// <summary>
        /// Тип ФЗ
        /// </summary>
        public string LawTypeName { get; set; }

        public string ConclusionReason { get; set; }

        /// <summary>
        /// Этап закупки
        /// </summary>
        public string StageName { get; set; }

        /// <summary>
        /// Способ определения поставщика
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Сокращенное наименование организации, осуществляющей закупку
        /// </summary>
        public string CustomerShortName { get; set; }

        /// <summary>
        /// Id наименование организации, осуществляющей закупку
        /// </summary>
        public long? CustomerId { get; set; }

        /// <summary>
        /// Региональный уровень организации, осуществляющей закупку
        /// </summary>
        public int? CustomerRegionLevel { get; set; }

        /// <summary>
        /// Регион организации, осуществляющей закупку - ФО
        /// </summary>
        public string CustomerRegionFederalDistrict { get; set; }

        /// <summary>
        /// Регион организации, осуществляющей закупку - СФ
        /// </summary>
        public string CustomerRegionFederationSubject { get; set; }

        /// <summary>
        /// Регион организации, осуществляющей закупку - район
        /// </summary>
        public string CustomerRegionDistrict { get; set; }

        /// <summary>
        /// Регион организации, осуществляющей закупку - город
        /// </summary>
        public string CustomerRegionCity { get; set; }

        /// <summary>
        /// Регион организации, осуществляющей закупку - код
        /// </summary>
        public string CustomerRegionCode { get; set; }

        /// <summary>
        /// Регион организации, осуществляющей закупку
        /// </summary>
        public string ConcatinatedCustomerRegion { get; set; }

        /// <summary>
        /// Кто осуществляет закупку
        /// </summary>
        public string WhoIsPurchasing { get; set; }

        /// <summary>
        /// Тип организации
        /// </summary>
        public string OrganizationType { get; set; }

        public bool ExistsNature { get; set; }
        public bool ExistsDeliveryTimeInfo { get; set; }
        public bool ExistsLotFunding { get; set; }
        public bool ExistsPurchaseObject { get; set; }
        public bool ToProtokol { get; set; }
        public string SiteURL { get; set; }


        public string NatureName { get; set; }

        public int? LotCount { get; set; }
        public int? LotCountOpen { get; set; }

        public bool ForCheck { get; set; }
    }
}


