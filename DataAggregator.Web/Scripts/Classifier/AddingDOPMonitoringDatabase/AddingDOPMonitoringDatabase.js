angular
    .module('DataAggregatorModule')
    .controller('AddingDOPMonitoringDatabaseController', ['$scope', '$http', '$q', '$cacheFactory', '$timeout', 'userService', 'uiGridCustomService', 'errorHandlerService', 'messageBoxService', 'uiGridConstants', 'formatConstants', AddingDOPMonitoringDatabaseController]);

function AddingDOPMonitoringDatabaseController($scope, $http, $q, $cacheFactory, $timeout, userService, uiGridCustomService, errorHandlerService, messageBoxService, uiGridConstants, formatConstants) {
    $scope.Title = "Модуль для добавления ДОП ассортимента в БД мониторинг";
    $scope.user = userService.getUser();

    console.log('Модуль для добавления ДОП ассортимента в БД мониторинг');


    $scope.AddingDOPMonitoringDatabase_Init = function () {

        $scope.message = 'Пожалуйста, ожидайте... Загрузка';

        //******** Grid ******** ->
        $scope.GridDOPMonitoringDatabase = uiGridCustomService.createGridClassMod($scope, 'GridDOPMonitoringDatabase');
        $scope.GridDOPMonitoringDatabase.Options.showGridFooter = true;
        $scope.GridDOPMonitoringDatabase.Options.multiSelect = true;
        $scope.GridDOPMonitoringDatabase.Options.enableFiltering = true;
        $scope.GridDOPMonitoringDatabase.Options.enableSelectAll = true;
        $scope.GridDOPMonitoringDatabase.Options.modifierKeysToMultiSelect = true;
        $scope.GridDOPMonitoringDatabase.Options.flatEntityAccess = true;
        $scope.GridDOPMonitoringDatabase.Options.enableGridMenu = true;

        $scope.GridDOPMonitoringDatabase.Options.columnDefs = [
            { headerTooltip: true, name: 'GoodsId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsId', type: 'number', visible: true, nullable: false },

            { headerTooltip: true, name: 'GoodsTradeNameId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsTradeNameId', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'GoodsTradeName', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsTradeName', visible: true },
            { headerTooltip: true, name: 'GoodsDescription', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsDescription', visible: true },
            { headerTooltip: true, name: 'OwnerTradeMarkId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'OwnerTradeMarkId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'OwnerTradeMarkKey', enableCellEdit: false, width: 100, cellTooltip: true, field: 'OwnerTradeMarkKey', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'OwnerTradeMark', enableCellEdit: false, width: 100, cellTooltip: true, field: 'OwnerTradeMark', visible: true },
            { headerTooltip: true, name: 'PackerId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'PackerId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'PackerKey', enableCellEdit: false, width: 100, cellTooltip: true, field: 'PackerKey', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'Packer', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Packer', visible: true },
            { headerTooltip: true, name: 'GoodsCategoryId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsCategoryId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'GoodsCategoryName', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsCategoryName', visible: true },
            { headerTooltip: true, name: 'BrandId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BrandId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'Brand', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Brand', visible: true },
            { headerTooltip: true, name: 'ClassifierId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'ClassifierId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'Status', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Status', type: 'boolean', visible: true, nullable: false },            
            { headerTooltip: true, name: 'StatusDesc', enableCellEdit: false, width: 100, cellTooltip: true, field: 'StatusDesc', visible: false },
            { headerTooltip: true, name: 'StartDate', enableCellEdit: false, width: 100, cellTooltip: true, field: 'StartDate', type: 'date', cellFilter: formatConstants.FILTER_DATE, visible: true, nullable: false },
            { headerTooltip: true, name: 'BlockTypeId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BlockTypeId', type: 'number', visible: false, nullable: true },
            { headerTooltip: true, name: 'BlockTypeName', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BlockTypeName', visible: false },
            { headerTooltip: true, name: 'BlockTypeDescription', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BlockTypeDescription', visible: false }            
        ];

        $scope.GridDOPMonitoringDatabase.SetDefaults();

        $scope.GridDOPMonitoringDatabase.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            //gridApi.edit.on.afterCellEdit($scope, editRowDataSource);
        };

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DOPMonitoringDatabase/Init/'
        }).then(function (response) {
            var data = response.data;

            if (data.Success) {
                $scope.GridDOPMonitoringDatabase.SetData(data.Data.DOPBlocking)

            } else {
                messageBoxService.showError(data.ErrorMessage);
            }

        }, function (response) {
            console.log(response);

            messageBoxService.showError(response.data);
        }).catch(error => alert(error.message));
    }

}