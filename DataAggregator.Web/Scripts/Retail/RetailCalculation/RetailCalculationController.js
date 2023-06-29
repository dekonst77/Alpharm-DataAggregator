angular
    .module('DataAggregatorModule')
    .controller('RetailCalculationController', ['$scope', '$http', '$uibModal', '$interval', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', '$q', 'messageBoxService', RetailCalculationController]);

function RetailCalculationController($scope, $http, $uibModal, $interval, $timeout, uiGridCustomService, errorHandlerService, formatConstants, $q, messageBoxService) {

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

    //история
    $scope.RetailCalculatioHistory_Grid = uiGridCustomService.createGridClassMod($scope, "RetailCalculatioHistory_Grid");
    $scope.RetailCalculatioHistory_Grid.Options.columnDefs = [
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.LOG_FIELD.STEP',
            field: 'Step',
            width: 400,
            enableCellEdit: false,
            filter: { condition: uiGridCustomService.condition }
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.LOG_FIELD.YEAR',
            field: 'Year',
            width: 100,
            type: 'number'
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.LOG_FIELD.MONTH',
            field: 'Month',
            width: 100,
            type: 'number'
        },
        {
            displayName: 'RETAIL.RETAIL_CALCULATION.LOG_FIELD.DATE',
            field: 'Date',
            width: 200,
            type: 'date',
            cellFilter: formatConstants.FILTER_DATE_TIME
        }
    ];

    $scope.filter = {
        date: null,
        PeriodFrom: null,
        PeriodTo: null
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
                var date = new Date(year, month - 1, 15);

                $scope.filter.date = date;
                calculationPeriod = date;
            } else {
                $scope.filter.date = null;
                calculationPeriod = null;
            }
        }, function () {

        }).then(function () {
            $scope.loadSourceList();
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
            sourceId: $scope.sources.Source.Id
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

    //Показ выпадающиего блока поставщиков у процесса "Отвязать аптеки"
    $scope.showSource = function () {
        if (!$scope.dataGridOptions.data)
            return false;
        return $scope.dataGridOptions.data.some(d => d.Checked && d.ProcessId == 1);
    };

    $scope.loadSourceList = function (selecedItem, event) {

        $http({
            method: 'POST',
            url: '/RetailTemplates/GetAllSources'
        }).then(function (response) {
            $scope.sources = response.data;
            $scope.sources.Source = {}
            $scope.sources.Source.Id = null;
            $scope.sources.Source.Name = null;
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };

    $scope.getListSource = function (value) {
        if (value == "" || value == null)
            return $scope.sources.slice(0, 20);

        return $scope.sources.filter(function (item) {
            return item.Name.toLowerCase().includes(value.toLowerCase())
        }).slice(0, 20);
    };

    $scope.setId = function (dictionaryItem, item) {
        item.Id = dictionaryItem.Id;
        item.Name = dictionaryItem.Name;
    };

    $scope.clearId = function (item, value) {
        if (item != null)
            item.Id = null;
    };

    //Для выпадающего списка при клике на Поставщика
    $scope.onFocus = function (e) {
        $timeout(function () {
            $(e.target).trigger('input');
            $(e.target).trigger('change'); // for IE
        });
    }

    

    $scope.isCalcHistoryRunning = false;
    $scope.checkFilter = function () {
        if ($scope.filter.PeriodFrom == null || $scope.filter.PeriodTo == null) {
            messageBoxService.showError('Не выбраны Период с и Период по', 'Ошибка');
            return false;
        }

        return true;
    }

    $scope.LoadHistory = function () {
        if (!$scope.checkFilter())
            return false;

        var data = $http({
            method: 'POST',
            url: '/RetailCalculation/Log_search/',
            data: JSON.stringify({
                PeriodFrom: $scope.filter.PeriodFrom, PeriodTo: $scope.filter.PeriodTo
            })
        }).then(function (response) {
            if (response.data.Data)
                $scope.RetailCalculatioHistory_Grid.Options.data = response.data.Data;
        });

        $scope.dataLoading = $q.all([data]);
    };

    $scope.CalcHistory = function () {
        if (!$scope.checkFilter())
            return false;

        messageBoxService.showConfirm('Вы уверены, что хотите сделать пересчёт истории?', 'Подтверждение')
            .then(
                function (result) {
                    $scope.isCalcHistoryRunning = true;
                    var data = $http({
                        method: 'POST',
                        url: '/RetailCalculation/HistoryCalculation/',
                        data: JSON.stringify({
                            PeriodFrom: $scope.filter.PeriodFrom, PeriodTo: $scope.filter.PeriodTo
                        })
                    }).then(function (response) {
                        $scope.isCalcHistoryRunning = false;
                        if (response.data.Data)
                            $scope.LoadHistory();
                    }, function (response) {
                        $scope.isCalcHistoryRunning = false;
                        errorHandlerService.showResponseError(response);
                    });

                    $scope.loading = $q.all([data]);
                },
                function (result) {
                    if (result === 'no') {
                      
                    }
                });

       
    };
}
















