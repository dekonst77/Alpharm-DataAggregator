angular
    .module('DataAggregatorModule')
    .controller('RetailCalculationController', ['$scope', '$http', '$uibModal', '$interval', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', RetailCalculationController]);

function RetailCalculationController($scope, $http, $uibModal, $interval, uiGridCustomService, errorHandlerService, formatConstants) {

    $scope.setTabIndex = function (index) {
        $scope.currentTabIndex = index;
    };

    //Выпускаемая дата
    var calculationPeriod = null;

    $scope.dataGridOptions = uiGridCustomService.createOptions('RetailCalculation_Grid');

    var options = {
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

    angular.extend($scope.dataGridOptions, options);

    $scope.dataGridOptions.columnDefs = [
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.FIELD.CHECK',
            field: 'Checked',
            width: 100,
            type: 'boolean',
            enableCellEdit: true,
            cellTemplate: '<input type="checkbox" ng-model="row.entity.Checked">'
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.FIELD.PROCESS',
            field: 'Process',
            width: 400,
            type: 'number'
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.FIELD.STATUS',
            field: 'Status',
            width: 200,
            type: 'number'
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.FIELD.START_TIME',
            field: 'StartTime',
            width: 200,
            type: 'date',
            cellFilter: formatConstants.FILTER_DATE_TIME
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.FIELD.END_TIME',
            field: 'EndTime',
            width: 200,
            type: 'date',
            cellFilter: formatConstants.FILTER_DATE_TIME
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.FIELD.COMMENT',
            field: 'Comment',
            width: 400,
            type: 'number'
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.FIELD.ID',
            field: 'Id',
            width: 100,
            type: 'number'
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.FIELD.USER',
            field: 'User',
            width: 400,
            type: 'number'
        },
    ];


    $scope.filter = {
        date: null,
    };

    function getLauncher() {

        var data =
        {
            year: $scope.filter.Year,
            month: $scope.filter.Month,
        }

        $scope.loading = $http({
            method: "POST",
            url: "/RetailCalculation/GetLauncher",
            data: JSON.stringify(data)
        }).then(function (response) {
            $scope.dataGridOptions.data = response.data;
        }, function () {

        });
    }

   

    //Определяем текущую дату и выпускаемый период
    function init() {
        $scope.loading = $http({
            method: "POST",
            url: "/RetailCalculation/Init"
        }).then(function (response) {

            if (response.data.Year !== null) {
                var year = response.data.Year;
                var month = response.data.Month;
                var date = new Date(year, month-1, 15);

                $scope.filter.date = date;
                calculationPeriod = date;
            } else {
                $scope.filter.date = null;
                calculationPeriod = null;
            }
          

          
        }, function () {
        });

        $scope.$watch(function () { return $scope.filter.date; },
            function () {
                if ($scope.filter.date) {
                    $scope.filter.Year = $scope.filter.date.getFullYear();
                    $scope.filter.Month = $scope.filter.date.getMonth() + 1;
                }

                if ($scope.filter.Year != null && $scope.filter.Month != null)
                    getLauncher();

            }, true);

    }

    $interval(getLauncher, 1000 * 60);


    init();

    //Выбрать активный период
    $scope.setCurrentPeriod = function () { };
    //Начать расчёт
    $scope.startPeriod = function () {

        var data =
        {
            year: $scope.filter.Year,
            month: $scope.filter.Month,
        }

        $scope.loading = $http({
            method: "POST",
            url: "/RetailCalculation/StartPeriod",
            data: JSON.stringify(data)
        }).then(function (response) {
            init();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };
    //Завершить расчёт
    $scope.endPeriod = function () {
        var data =
        {
            year: $scope.filter.Year,
            month: $scope.filter.Month,
        }

        $scope.loading = $http({
            method: "POST",
            url: "/RetailCalculation/EndPeriod",
            data: JSON.stringify(data)
        }).then(function (response) {
            init();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };
    //Поставить в очередь на выполнение
    $scope.runProcess = function () {

        var selectedProcessList = $scope.dataGridOptions.data.filter(d => d.Checked).map(function(obj) {
            return obj.ProcessId;
        });

        var data =
        {
            year: $scope.filter.Year,
            month: $scope.filter.Month,
            processList: selectedProcessList,
        }

        $scope.loading = $http({
            method: "POST",
            url: "/RetailCalculation/RunProcess",
            data: JSON.stringify(data)
        }).then(function (response) {
            init();
            getLauncher();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };

    //Поставить в очередь на выполнение
    $scope.stopProcess = function () {

        var selectedProcessList = $scope.dataGridOptions.data.filter(d => d.Checked).map(function (obj) {
            return obj.ProcessId;
        });

        var data =
        {
            year: $scope.filter.Year,
            month: $scope.filter.Month,
            processList: selectedProcessList,
        }

        $scope.loading = $http({
            method: "POST",
            url: "/RetailCalculation/StopProcess",
            data: JSON.stringify(data)
        }).then(function (response) {
            init();
            getLauncher();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };

    //Можно начать расчёт месяца
    $scope.canStartPeriod = function () {
        if (calculationPeriod === null)
            return true;
        return false;
    };
    //Можно завершить расчёт месяца
    $scope.canEndPeriod = function () {
        if (calculationPeriod === null)
            return false;
        if ($scope.filter.date.getFullYear() !== calculationPeriod.getFullYear() ||
            $scope.filter.date.getMonth() !== calculationPeriod.getMonth())
            return false;
        return true;
    };
    //Можно запустить выполнение
    $scope.canStart = function () {
        if (!$scope.dataGridOptions.data)
            return false;
        return $scope.dataGridOptions.data.some(d => d.Checked);
    };
    //Можно запустить выполнение
    $scope.canStop = function () {
        if (!$scope.dataGridOptions.data)
            return false;
        return $scope.dataGridOptions.data.some(d => d.Checked);
    };





}
















