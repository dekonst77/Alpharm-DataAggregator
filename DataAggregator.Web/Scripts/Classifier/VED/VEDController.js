angular
    .module('DataAggregatorModule')
    .controller('VEDController', ['$scope', '$http', '$uibModal', 'messageBoxService', 'uiGridCustomService', 'formatConstants', VEDController]);

function VEDController($scope, $http, $uibModal, messageBoxService, uiGridCustomService, formatConstants) {

    $scope.currentTabIndex = 0;

    $scope.setTabIndex = function(index) {
        $scope.currentTabIndex = index;
    };

    $scope.uibSize = {};
    $scope.gridStyle = {};

    $scope.$watch(function () { return $scope.uibSize; },
        function (newValue, oldValue) {
            if (newValue === oldValue)
                return;

            $scope.gridStyle = {
                'height': 'calc(100% - ' + newValue.height + ')'
            };
        },
        true);

    //Задаем свойства грида
    $scope.gridOptions = uiGridCustomService.createOptions('Ved_MnnGrid');

    var gridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 30,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: false,
        noUnselect: false
    };

    angular.extend($scope.gridOptions, gridOptions);

    //Задаем свойства грида
    $scope.gridTradeNameOptions = uiGridCustomService.createOptions('Ved_TradeNameGrid');

    var gridTradeNameOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 30,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: false,
        noUnselect: false
    };

    angular.extend($scope.gridTradeNameOptions, gridTradeNameOptions);

    var columns = [];
    var rows = null;

    $scope.ShowAll = false;

    $scope.Refresh = function () {
        Load();
        LoadTradeName();
    }


    $scope.ChangeData = function () {
        $scope.ShowAll = !$scope.ShowAll;
        ShowData();
    }

    $scope.openPeriod = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/VED/_PeriodView.html',
            controller: 'VEDPeriodController',
            size: 'giant',
            backdrop: 'static'
        });

        modalInstance.result.then(function () {
            Load();
        }, function () {
            Load();
        });
    }
    
    function ShowData() {
        if ($scope.ShowAll) {
            $scope.gridOptions.data = rows;
        }
        else {


            //Показываем данные ВЕД
            $scope.gridOptions.data = rows.filter(function (item) {
                return columns.some(function (column) { return item[column] === true });


            });

        }
    }

    var sortDate = function (a, b) {
        if (!a && b)
            return -1;

        if (a && !b)
            return 1;

        if (!a && !b)
            return 0;

        if (a == b) return 0;
        if (a > b) return 1;
        return -1;
    };


    //Загружаем VED отобранные по TradeName
    function LoadTradeName() {
        $scope.gridTradeNameOptions.data = null;

        $scope.vedTradeNameLoading =
        $http({
            method: "POST",
            url: "/VED/LoadTradeName/"
        }).then(function (response) {
            var data = response.data;

            $scope.gridTradeNameOptions.columnDefs = [
                { name: 'ТН', field: 'TradeName', width: 200 },
                { name: 'Форма выпуска', field: 'FormProduct' },
                {
                    name: 'Дата добавления', field: 'DateAdd',  type: 'date',
                    width: 200, cellFilter: formatConstants.FILTER_DATE_TIME,
                    sortingAlgorithm: sortDate
                }
            ];


            var tnColumns = [];

            //Генерим список колонок
            data.Columns.forEach(function (item) {
                var column = {};
                column.name = item.Name;
                column.field = 'Y' + item.Id;
                column.enableCellEdit = true;
                column.Type = 'boolean';
                column.width = '100';
                column.cellTemplate = '<input type="checkbox" ng-model="row.entity.' + column.field + '" ng-change="grid.appScope.ChangeTradeNameVed(row.entity, \'' + column.field + '\')">';

                tnColumns.push(column.field);

                $scope.gridTradeNameOptions.columnDefs.push(column);
            });

            $scope.gridTradeNameOptions.data = data.Rows;

        }, function (e) {
            $scope.message = "Unexpected Error";
        });
    }


    //Загружаем все VED
    function Load() {

        $scope.gridOptions.data = null;

        $scope.vedLoading =
        $http({
            method: "POST",
            url: "/VED/Load/"
        }).then(function (response) {
            var data = response.data;

            $scope.gridOptions.columnDefs = [
                { name: 'МНН', field: 'INNGroup', width: 200 },
                { name: 'Форма выпуска', field: 'FormProduct' },
                {
                    name: 'Дата добавления', field: 'DateAdd', 
                    type: 'date', width: 200, cellFilter: formatConstants.FILTER_DATE_TIME,
                    sortingAlgorithm:sortDate
                }
            ];



            var column = {};
            column.name = 'Проверено';
            column.field = 'Checked';
            column.enableCellEdit = true;
            column.Type = 'boolean';
            column.width = '100';
            column.cellTemplate = '<input type="checkbox" ng-model="row.entity.Checked" ng-change="grid.appScope.ChangeChecked(row.entity, row.entity.Checked)">';

            $scope.gridOptions.columnDefs.push(column);


            columns = [];

            //Генерим список колонок
            data.Columns.forEach(function (item) {
                var column = {};
                column.name = item.Name;
                column.field = 'Y' + item.Id;
                column.enableCellEdit = true;
                column.Type = 'boolean';
                column.width = '100';
                column.cellTemplate = '<input type="checkbox" ng-model="row.entity.' + column.field + '" ng-change="grid.appScope.ChangeVED(row.entity, \'' + column.field + '\')">';

                columns.push(column.field);

                $scope.gridOptions.columnDefs.push(column);
            });

            //Записываем данные
            rows = data.Rows;

            //Показываем данные ВЕД
            ShowData(false);


        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    $scope.ChangeTradeNameVed = function(item, column) {
        //Получаем идентификатор перида
        var VEDPeriodId = column.substr(1);

        $scope.vedLoading =
        $http({
            method: "POST",
            url: "/VED/ChangeTradeName/",
            data: JSON.stringify({ TradeNameId: item.TradeNameId, FormProductId: item.FormProductId, VEDPeriodId: VEDPeriodId })
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
            } else {
                messageBoxService.showError(data.Message);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    $scope.ChangeVED = function (item, column) {

        //Получаем идентификатор перида
        var VEDPeriodId = column.substr(1);

        $scope.vedLoading =
        $http({
            method: "POST",
            url: "/VED/Change/",
            data: JSON.stringify({ INNGroupId: item.INNGroupId, FormProductId: item.FormProductId, VEDPeriodId: VEDPeriodId })
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
            } else {
                messageBoxService.showError(data.Message);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    $scope.ChangeChecked = function (item, column) {

        $scope.vedLoading =
        $http({
            method: "POST",
            url: "/VED/ChangeChecked/",
            data: JSON.stringify({ INNGroupId: item.INNGroupId, FormProductId: item.FormProductId })
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
            } else {
                messageBoxService.showError(data.Message);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }


    //Загружаем
    Load();
    LoadTradeName();


}