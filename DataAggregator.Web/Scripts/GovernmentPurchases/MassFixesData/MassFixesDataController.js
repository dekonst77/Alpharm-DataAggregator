angular
    .module('DataAggregatorModule')
    .controller('MassFixesDataController', ['messageBoxService', '$scope', '$http', '$uibModal', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', MassFixesDataController]);

function MassFixesDataController(messageBoxService, $scope, $http, $uibModal, uiGridCustomService, uiGridConstants, formatConstants) {

    $scope.level = false;
    $scope.massFixesDataGrid = uiGridCustomService.createGridClassMod($scope, 'MassFixesData_Grid', onSelectionChanged_massFixesDataGrid);
    $scope.massFixesDataGrid.Options.showGridFooter = true;
    $scope.massFixesDataGrid.Options.enableSorting = true;
    $scope.massFixesDataGrid.Options.noUnselect = true;
    $scope.massFixesDataGrid.Options.multiSelect = true;
    $scope.massFixesDataGrid.Options.modifierKeysToMultiSelect = true;
    $scope.massFixesDataGrid.Options.columnDefs =
    [//'/#/Classifier/Manufacturer/Edit?id=0', '_blank'
{ name: 'Id закупки', field: 'PurchaseId', filter: { condition: uiGridCustomService.condition }, type: 'number' },
 { name: 'Номер закупки', field: 'PurchaseNumber', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
//{ name: 'LotId', field: 'LotId', filter: { condition: uiGridCustomService.condition }, type: 'number' , visible: false},
        { name: 'Номер лота', field: 'LotNumber', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { name: '∑ лота', field: 'LotSum', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM , type: 'number' },

{ name: 'Наименование закупки', field: 'PurchaseName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
{ name: 'Date_begin', field: 'DateBegin', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
{ name: 'Date_end', field: 'DateEnd', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
{ name: 'Тип ФЗ', field: 'LawTypeName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Характер.Подраздел', field: 'Nature_L2Name', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
        { name: 'Характер', field: 'NatureName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
{ name: 'Категория', field: 'CategoryName', filter: { condition: uiGridCustomService.condition } },
{ name: 'Источник финансирования', field: 'SourceOfFinancing', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
{ name: 'Финансирование', field: 'FundingNames', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
{ name: 'Сроки поставки', field: 'DeliveryTime', filter: { condition: uiGridCustomService.condition } },
{ name: 'Периоды поставки и периодичность', field: 'DeliveryTimePeriod_text', filter: { condition: uiGridCustomService.condition } },
{ name: 'Ссылка на закупку', field: 'URL', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateURL },
{ name: 'Статус лота', field: 'LotStatusName', filter: { condition: uiGridCustomService.condition } },

        { name: 'Примечание', field: 'Comment', filter: { condition: uiGridCustomService.condition } },


        { name: 'Id Заказчика', field: 'CustomerId', filter: { condition: uiGridCustomService.condition }, type: 'number', cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/OrganizationsEditor?Id={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
{ name: 'Полное наименование Заказчика', field: 'CustomerFullName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
{ name: 'Сокращенное наименование Заказчика', field: 'CustomerShortName', filter: { condition: uiGridCustomService.condition } },
        { name: 'ИНН Заказчика', field: 'CustomerINN', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/OrganizationsEditor?Inn={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
{ name: 'Тип учреждения Заказчика', field: 'CustomerOrganizationType', filter: { condition: uiGridCustomService.condition } },
{ name: 'Регион Заказчика', field: 'CustomerFederationSubject', filter: { condition: uiGridCustomService.condition } },

        { name: 'Id Получателя', field: 'ReceiverId', filter: { condition: uiGridCustomService.condition }, type: 'number', cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/OrganizationsEditor?Id={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
{ name: 'Полное наименование Получателя', field: 'ReceiverFullName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
{ name: 'Сокращенное наименование Получателя', field: 'ReceiverShortName', filter: { condition: uiGridCustomService.condition } },
        { name: 'ИНН Получателя', field: 'ReceiverINN', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/OrganizationsEditor?Inn={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
{ name: 'Тип учреждения Получателя', field: 'ReceiverOrganizationType', filter: { condition: uiGridCustomService.condition } },
{ name: 'Регион Получателя', field: 'ReceiverFederationSubject', filter: { condition: uiGridCustomService.condition } },

{ name: 'КБК из закупки', field: 'KBKs', filter: { condition: uiGridCustomService.condition } },
{ name: 'КБК из контракта', field: 'contract_KBKs', filter: { condition: uiGridCustomService.condition } },

{ name: 'Раздел', field: 'PurchaseClassName', filter: { condition: uiGridCustomService.condition } },
{ name: 'Пользователь, последний изменивший «Описание закупки»', field: 'LastChangedUser_Purchase', filter: { condition: uiGridCustomService.condition } },
{ name: 'Пользователь, последний изменивший «Описание лота»', field: 'LastChangedUser_Lot', filter: { condition: uiGridCustomService.condition } },
{ name: 'Пользователь, последний изменивший «Объекты закупки»', field: 'LastChangedUser_PurchaseObjectReady', filter: { condition: uiGridCustomService.condition } },

{ name: 'Id контракта', field: 'contractId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
{ name: 'id объекта лота', field: 'porID', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false }
        ];

    $scope.massFixesDataGrid.SetDefaults();

    $scope.getDataByFilter = function () {
        //клонируем фильтр
        var filterToServer = JSON.parse(JSON.stringify($scope.filterwhere));

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/MassFixesData/GetDataByFilter',
            data: {
                filter: filterToServer,level:$scope.level
            }
        }).then(function (response) {
            $scope.massFixesDataGrid.Options.data = response.data.data;
            if (response.data.count === 50000) {
                messageBoxService.showInfo('Показано 50 000 записей! Возможно, это не все данные.');
            }
        }, function () {
            messageBoxService.showError('Не удалось загрузить отчёт!');
        });
    };

    $scope.openFilterDialog = function () {
        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/filters/_filtersView.html',
            controller: 'filtersController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: {
                    filterall: $scope.filterall
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                $scope.filterwhere = data.filterwhere;
                $scope.filterall = data.filterall;
                $scope.getDataByFilter();
            },
            // cancel
            function () {
            }
        );
    };

    $scope.openChangeDialog = function () {
        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/MassFixesData/_ChangeView.html',
            controller: 'MassChangeController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: {
                    change: $scope.change
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                /*
                            PurchaseIds: [],
            LotIds: [],
            PurchaseObjectReadyIds:[]
                */
                $scope.change = data.change;
                var selectedList = $scope.massFixesDataGrid.selectedItem.map(function (value) {
                    if (value.PurchaseId > 0 && $scope.change.PurchaseIds.indexOf(value.PurchaseId) === -1) {
                        $scope.change.PurchaseIds.push(value.PurchaseId);
                    }
                    if (value.LotId > 0 && $scope.change.LotIds.indexOf(value.LotId) === -1) {
                        $scope.change.LotIds.push(value.LotId);
                    }
                    if (value.porID > 0 && $scope.change.PurchaseObjectReadyIds.indexOf(value.porID) === -1) {
                        $scope.change.PurchaseObjectReadyIds.push(value.porID);
                    }
                });
                var message = "\r\nБудет изменено: ";
                message +="\r\nЗакупок: " +$scope.change.PurchaseIds.length;
                message += "\r\nЛотов: " + $scope.change.LotIds.length;
                message += "\r\nОбъектов закупки: " + $scope.change.PurchaseObjectReadyIds.length;

                if (confirm("Вы уверены что хотите применить изменения?" + message)) {
                    
                    
                    

                    $scope.setchange();                    
                } else {                    
                    //alert("Вы нажали кнопку отмена")                    
                }


            },
            // cancel
            function () {
            }
        );
    };
    $scope.setchange = function () {
        //клонируем фильтр
        var changeToServer = JSON.parse(JSON.stringify($scope.change));

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/MassFixesData/setchange',
            data: {
                change: changeToServer
            }
        }).then(function (response) {
            if (confirm("Обновил. Обновить выборку?")) {
                $scope.getDataByFilter();
            } else {
                //alert("Вы нажали кнопку отмена")                    
            }
        }, function () {
            messageBoxService.showError('Ошибка!');
        });
    };
    $scope.superText = "";
    function onSelectionChanged_massFixesDataGrid(row) {
        if (row !== undefined) {
            $scope.superText = row["PurchaseName"] + " " + row["SourceOfFinancing"];
        }
        else {
            $scope.superText = "";}
    };
}



