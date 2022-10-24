angular
    .module('DataAggregatorModule')
    .controller('ReportsController', ['messageBoxService', '$scope', '$http', '$uibModal', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', ReportsController]);

function ReportsController(messageBoxService, $scope, $http, $uibModal, uiGridCustomService, uiGridConstants, formatConstants) {
    $scope.superText = "";

    $scope.Grid = uiGridCustomService.createGridClass($scope, 'Grid');
    $scope.Grid.Options.showGridFooter = true;
    $scope.Grid.Options.enableSorting = true;

    $scope.Grid.Options.columnDefs =
        [//'/#/Classifier/Manufacturer/Edit?id=0', '_blank'
            //{ name: 'Id закупки', field: 'PurchaseId', filter: { condition: uiGridCustomService.condition }, type: 'number' },
            //{ name: 'Номер закупки', field: 'PurchaseNumber', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            ////{ name: 'LotId', field: 'LotId', filter: { condition: uiGridCustomService.condition }, type: 'number' , visible: false},
            //{ name: 'Номер лота', field: 'LotNumber', filter: { condition: uiGridCustomService.condition }, type: 'number' },
            //{ name: 'Наименование закупки', field: 'PurchaseName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            //{ name: 'Date_begin', field: 'DateBegin', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            //{ name: 'Date_end', field: 'DateEnd', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            //{ name: 'Тип ФЗ', field: 'LawTypeName', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Характер', field: 'NatureName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            //{ name: 'Категория', field: 'CategoryName', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Источник финансирования', field: 'SourceOfFinancing', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            //{ name: 'Финансирование', field: 'FundingNames', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            //{ name: 'Сроки поставки', field: 'DeliveryTime', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Периоды поставки и периодичность', field: 'DeliveryTimePeriod_text', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Ссылка на закупку', field: 'URL', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateURL },
            //{ name: 'Статус лота', field: 'LotStatusName', filter: { condition: uiGridCustomService.condition } },

            //{ name: 'Id Заказчика', field: 'CustomerId', filter: { condition: uiGridCustomService.condition }, type: 'number' },
            //{ name: 'Полное наименование Заказчика', field: 'CustomerFullName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            //{ name: 'Сокращенное наименование Заказчика', field: 'CustomerShortName', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'ИНН Заказчика', field: 'CustomerINN', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Тип учреждения Заказчика', field: 'CustomerOrganizationType', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Регион Заказчика', field: 'CustomerFederationSubject', filter: { condition: uiGridCustomService.condition } },

            //{ name: 'Id Получателя', field: 'ReceiverId', filter: { condition: uiGridCustomService.condition }, type: 'number' },
            //{ name: 'Полное наименование Получателя', field: 'ReceiverFullName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            //{ name: 'Сокращенное наименование Получателя', field: 'ReceiverShortName', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'ИНН Получателя', field: 'ReceiverINN', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Тип учреждения Получателя', field: 'ReceiverOrganizationType', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Регион Получателя', field: 'ReceiverFederationSubject', filter: { condition: uiGridCustomService.condition } },

            //{ name: 'Найденный заказчик', field: 'CustomerSource', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'КБК из закупки', field: 'KBKs', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'КБК из контракта', field: 'contract_KBKs', filter: { condition: uiGridCustomService.condition } },

            //{ name: 'Раздел', field: 'PurchaseClassName', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Пользователь, последний изменивший «Описание закупки»', field: 'LastChangedUser_Purchase', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Пользователь, последний изменивший «Описание лота»', field: 'LastChangedUser_Lot', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'Пользователь, последний изменивший «Объекты закупки»', field: 'LastChangedUser_PurchaseObjectReady', filter: { condition: uiGridCustomService.condition } },

            //{ name: 'Id контракта', field: 'contractId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
            //{ name: 'id объекта лота', field: 'porID', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false }
        ];
    $scope.Grid.Options.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.onSelectionChanged(row);
        });
    };
    $scope.Grid.Options.multiSelect = true;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;

    $scope.TEST = function (o) {
        var t = o;
    };
    $scope.onSelectionChanged = function (row) {
        $scope.superText = "";
        //var selectedRows = $scope.gridApi.selection.getSelectedGridRows();
        //if (selectedRows.length == 1)
        //{
        //$scope.superText=selectedRows[0].PurchaseName + " " + SourceOfFinancing;
        //}   
        $scope.superText = row.entity.PurchaseName + " " + row.entity.SourceOfFinancing;
    };

    $scope.getDataByFilter = function () {
        //клонируем фильтр
        var filterToServer = JSON.parse(JSON.stringify($scope.filterwhere));

        $scope.loading = $http({
            method: 'POST',
            url: '/MassFixesData/GetDataByFilter',
            data: {
                filter: filterToServer
            }
        }).then(function (response) {
            $scope.Grid.Options.data = response.data.data;
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
}



