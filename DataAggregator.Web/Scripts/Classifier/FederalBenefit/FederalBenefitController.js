angular
    .module('DataAggregatorModule')
    .controller('FederalBenefitController', ['$scope', '$http', '$uibModal', 'messageBoxService', 'uiGridCustomService', 'formatConstants', FederalBenefitController]);

function FederalBenefitController($scope, $http, $uibModal, messageBoxService, uiGridCustomService, formatConstants) {

    //Задаем свойства грида
    $scope.gridOptions = uiGridCustomService.createOptions('FederalBenefit_Grid');

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
   

    var columns = [];
    var rows = null;

    $scope.ShowAll = false;

    $scope.Refresh = function () {
        Load();
    }


    $scope.ChangeData = function () {
        $scope.ShowAll = !$scope.ShowAll;
        ShowData();
    }

    $scope.openPeriod = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/FederalBenefit/_PeriodView.html',
            controller: 'FederalBenefitPeriodController',
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
    

    //Загружаем все FederalBenefit
    function Load() {

        $scope.gridOptions.data = null;

        $scope.FederalBenefitLoading =
        $http({
            method: "POST",
            url: "/FederalBenefit/Load/"
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
                column.cellTemplate = '<input type="checkbox" ng-model="row.entity.' + column.field + '" ng-change="grid.appScope.ChangeFederalBenefit(row.entity, \'' + column.field + '\')">';

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

    

    $scope.ChangeFederalBenefit = function (item, column) {

        //Получаем идентификатор перида
        var FederalBenefitPeriodId = column.substr(1);

        $scope.FederalBenefitLoading =
        $http({
            method: "POST",
            url: "/FederalBenefit/Change/",
            data: JSON.stringify({ INNGroupId: item.INNGroupId, FormProductId: item.FormProductId, FederalBenefitPeriodId: FederalBenefitPeriodId })
        }).then(function (response) {
            var responseData = response.data;
            if (responseData.Success) {
            } else {
                messageBoxService.showError(responseData.Message);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    $scope.ChangeChecked = function (item, column) {

        $scope.FederalBenefitLoading =
        $http({
            method: "POST",
            url: "/FederalBenefit/ChangeChecked/",
            data: JSON.stringify({ INNGroupId: item.INNGroupId, FormProductId: item.FormProductId })
        }).then(function (response) {
            var responseData = response.data;

            if (responseData.Success) {
            } else {
                messageBoxService.showError(responseData.Message);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }


    //Загружаем
    Load();


}