angular
    .module('DataAggregatorModule')
    .controller('RulesCommitController', ['$scope', '$http', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', '$q', 'messageBoxService', RulesCommitController]);

function RulesCommitController($scope, $http, uiGridCustomService, errorHandlerService, formatConstants, $q, messageBoxService) {

    $scope.RulesCommit_Init = function () {

        $scope.filter = {
            PeriodFrom: null,
            PeriodTo: null
        };

        $scope.isRunning = false;

        $scope.RulesCommitHistory_Grid = uiGridCustomService.createGridClassMod($scope, "RulesCommitHistory_Grid");

        $scope.RulesCommitHistory_Grid.Options.columnDefs = [
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
    }

    $scope.checkFilter = function() {
        if ($scope.filter.PeriodFrom == null || $scope.filter.PeriodTo == null) {
            messageBoxService.showError('Не выбраны Период с и Период по', 'Ошибка');
            return false;
        }

        return true;
    }

    $scope.History_search = function () {
        if (!$scope.checkFilter())
            return false;

        var data = $http({
            method: 'POST',
            url: '/RulesCommit/Log_search/',
            data: JSON.stringify({
                PeriodFrom: $scope.filter.PeriodFrom, PeriodTo: $scope.filter.PeriodTo
            })
        }).then(function (response) {
            if (response.data.Data)
                $scope.RulesCommitHistory_Grid.Options.data = response.data.Data;
        });

        $scope.dataLoading = $q.all([data]);
    };

    $scope.runProcess = function () {
        if (!$scope.checkFilter())
            return false;

        $scope.isRunning = true;
        var data = $http({
            method: 'POST',
            url: '/RulesCommit/RunProcess/',
            data: JSON.stringify({
                PeriodFrom: $scope.filter.PeriodFrom, PeriodTo: $scope.filter.PeriodTo
            })
        }).then(function (response) {
            $scope.isRunning = false;
            if (response.data.Data)
                $scope.History_search();
        }, function (response) {
            $scope.isRunning = false;
            errorHandlerService.showResponseError(response);
        });

        $scope.dataLoading = $q.all([data]);
    };
}