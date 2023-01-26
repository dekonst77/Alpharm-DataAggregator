angular
    .module('DataAggregatorModule')
    .controller('CheckContractController', [
        '$scope', '$route', '$http', '$uibModal', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', CheckContractController]);


function CheckContractController($scope, $route, $http, $uibModal, messageBoxService, uiGridCustomService, errorHandlerService, formatConstants) {
    $scope.contract_Init = function () {
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.noUnselect = true;
    $scope.Grid.Options.columnDefs = [
        
    { name: 'Статус', field: 'ContractStatus', filter: { condition: uiGridCustomService.condition } },
        { name: 'ссылка закупка', field: 'url_P', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_P}}" target="_blank">ссылка</a></div>' },
        { name: 'номер закупки', field: 'Number', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'},
        { name: 'Дата размещения закупки', field: 'DateBegin', type: 'date', filter: { condition: uiGridCustomService.condition } },
        { name: 'ссылка Контракт', field: 'url_C', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_C}}" target="_blank">ссылка</a></div>' },
        { name: 'номер Контракта', field: 'ReestrNumber', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?ReestrNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'KK', field: 'KKName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Причина изменения условий контракта', field: 'change_reason', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
        { name: 'Поставщик', field: 'Supplier_Name', filter: { condition: uiGridCustomService.condition } },
        { name: 'Поставщик ИНН', field: 'Supplier_INN', filter: { condition: uiGridCustomService.condition } },
        { name: 'Сумма ТЗ', field: 'SUM_TZ', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Сумма', field: 'Sum', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Сумма новая', field: 'Sum_new', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Разница Сумм', field: 'Sum_delta', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
        { name: '%Сумм', field: 'Sum_delta_proc', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent },
       { name: 'Фактически оплачено', field: 'ActuallyPaid', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
        //{ name: 'Фактически оплачено новая', field: 'ActuallyPaid_new', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
        //{ name: 'Разница Фактически оплачено', field: 'ActuallyPaid_delta', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Разница Сумма новая-Фактически оплачено', field: 'Sum_new_ActuallyPaid', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
        { name: '%Сумма новая-Фактически оплачено', field: 'Sum_new_ActuallyPaid_proc', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent },
        { name: 'ТЗ', field: 'TZ', filter: { condition: uiGridCustomService.condition } },
        { name: 'Пользователь (Фамилия, Имя), последний изменивший Объекты контракта', field: 'LastChangedObjectsUser', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата последнего изменения Объектов контракта', field: 'LastChangedObjectsDate', type: 'date', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'dd.MM.yyyy\'' },
        { name: 'Дата последнего скачивания контракта', field: 'LastChangedContract', type: 'date', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'dd.MM.yyyy\''  },
        { name: 'количество позиций', field: 'COR_Count', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },

    ];
    $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Load_Contract_check();
    };
    
    $scope.Load_Contract_check = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/CheckContract/Load_Contract_check_view/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
               // if (data.Success) {
                data.Data.forEach(function (item, i, arr) {
                    if (item.Sum > 0)
                        item.Sum_delta_proc = 100 * item.Sum_delta / item.Sum;
                    if (item.Sum_new > 0)
                        item.Sum_new_ActuallyPaid_proc = 100 * item.Sum_new_ActuallyPaid / item.Sum_new;
                });
                $scope.Grid.Options.data = data.Data;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }
   
    //применить параметры контракта, если IsSet=1 то применить, если isSet=0, то удалить найденые изменения
    $scope.Contract_check_Set_check = function (type,isSet)
    {
        var retList = [];
        $scope.Grid.selectedRows().forEach(function (value) {
            retList.push(value.ReestrNumber);
            value["@modify"] = true;
        });
        var json = JSON.stringify({ model: retList,type:type, isSet: isSet });
       // return;
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/CheckContract/Contract_check_Set_check/',
                data: json
            }).then(function (response) {
                alert("Сделал.");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }
    $scope.ContractPaymentStage_Init = function () {
        $scope.Nature = [];
        $scope.Funding = [];
        $scope.spr_Nature = [];
        $scope.spr_Funding = [];
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.columnDefs = [
            { name: 'PurchaseId', visible: false, field: 'PurchaseId' },
            { name: 'NatureId', visible: false, field: 'NatureId' },
            { name: 'LotId', visible: false, field: 'LotId' },
            { name: 'ContractId', visible: false, field: 'ContractId' },

            { name: 'номер закупки', field: 'PurchaseNumber', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'номер Контракта', field: 'ReestrNumber', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?ReestrNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'Дата размещения закупки', field: 'DateBegin', type: 'date', filter: { condition: uiGridCustomService.condition } },
            { name: 'Характер (было)', field: 'NatureName', filter: { condition: uiGridCustomService.condition } },
            { name: 'Регион Получателя', field: 'ReceiverFederationSubject', filter: { condition: uiGridCustomService.condition } },
            { name: 'КБК (было)', field: 'KBKs', filter: { condition: uiGridCustomService.condition } },
            { name: 'Финансирование (было)', field: 'FundingNames', filter: { condition: uiGridCustomService.condition } },
            { name: 'Суммы по КБК (было)', field: 'KBK_sum', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'КБК (стало)', field: 'KBKs_new', filter: { condition: uiGridCustomService.condition } },
            { name: 'Суммы по КБК (стало)', field: 'KBK_sum_new', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Ссылка на контракт', field: 'Url', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.Url}}" target="_blank">ссылка</a></div>' },
            { name: 'Статус', field: 'Status', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Load_ContractPaymentStage();

    };
    $scope.KBK_set = function (isSet) {
        var ContractIds = [];
        $scope.Grid.selectedRows().forEach(function (value) {
            ContractIds.push(value.ContractId);
        });
        var json = JSON.stringify({ model: ContractIds, isSet: isSet });

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/CheckContract/Contract_check_ContractPaymentStage_KBK_Set/',
                data: json
            }).then(function (response) {
                alert("Сделал.");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SetNature = function () {
        var PurchaseIds = [];
        $scope.Grid.selectedRows().forEach(function (value) {
            PurchaseIds.push(value.PurchaseId);
        });
        var json = JSON.stringify({ model: PurchaseIds, NatureId: $scope.Nature.Id });

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/CheckContract/SetNature/',
                data: json
            }).then(function (response) {
                alert("Сделал.");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SetFunding = function () {
        var LotIds = [];
        $scope.Grid.selectedRows().forEach(function (value) {
            LotIds.push(value.LotId);
        });
        var json = JSON.stringify({ model: LotIds, FundingId: $scope.Funding.Id });

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/CheckContract/SetFunding/',
                data: json
            }).then(function (response) {
                alert("Сделал.");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Load_ContractPaymentStage = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/CheckContract/Load_ContractPaymentStage/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                // if (data.Success) {
                $scope.Grid.Options.data = data.Data;
                if ($scope.spr_Nature.length === 0) {
                    Array.prototype.push.apply($scope.spr_Nature, response.data.Nature);
                }
                if ($scope.spr_Funding.length === 0) {
                    Array.prototype.push.apply($scope.spr_Funding, response.data.Funding);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }
}