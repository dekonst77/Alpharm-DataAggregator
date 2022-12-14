angular
    .module('DataAggregatorModule')
    .controller('RetailReportController', ['$scope', '$http', '$interval', 'uiGridCustomService', 'formatConstants', 'errorHandlerService', RetailReportController]);

function RetailReportController($scope, $http, $interval, uiGridCustomService, formatConstants, errorHandlerService) {

    $scope.filter = {};
    $scope.filter.reports = [];
    $scope.filter.selectedReports = [];
    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 15);

    $scope.filter.date = previousMonthDate;
    $scope.filter.selectedReports = [];

    var intervalPromise = $interval(refreshGrid, 1000 * 60);

    function refreshGrid() {
        $scope.loading = $http({
            method: "POST",
            url: "/RetailReport/GetReportLauncher"
        }).then(function (response) {
            $scope.reportGrid.Options.data = response.data;

        }, function (response) {
            $scope.filter.reports = null;
            errorHandlerService.showResponseError(response);
        });
    }

    var Init = function () {
        $scope.loading = $http({
            method: "POST",
            url: "/RetailReport/GetReports"
        }).then(function (response) {
            $scope.filter.reports = response.data;
        }, function (response) {
            $scope.filter.reports = null;
            errorHandlerService.showResponseError(response);
        });

        refreshGrid();
    }

    Init();

    $scope.add = function () {

        var launchers = $scope.filter.selectedReports.map(function (item) {
            var object =
            {
                ReportId: item,
                Year: $scope.filter.date.getFullYear(),
                Month: $scope.filter.date.getMonth() + 1,
                SendSelf: $scope.filter.SendSelf,
            };

            return object;
        });

        $scope.filter.selectedReports = [];


        $scope.loading = $http({
            method: "POST",
            url: "/RetailReport/AddReports",
            data: JSON.stringify({ models: launchers })
        }).then(function (response) {
            Init();
        }, function (response) {
            $scope.filter.reports = null;
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.stopReport = function () {
        var item = $scope.reportGrid.getSelected();

        $scope.loading = $http({
            method: "POST",
            url: "/RetailReport/StopReports",
            data: JSON.stringify({ reportId: item })
        }).then(function () {
            $scope.Init();
        }, function () {

        });
    }

    $scope.canRemove = function () {

        var item = $scope.reportGrid.getSelected();
        return item != null && item.length > 0;
    }

    $scope.canAdd = function () {
        return $scope.filter.date != null && $scope.filter.selectedReports.length > 0;
    }

    $scope.reportGrid = uiGridCustomService.createGridClass($scope, 'report_Grid');

    $scope.reportGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', width: 50, type: 'number' },
        { name: 'Отчет', field: 'ReportName', width: 500 },
        { name: 'Год', field: 'Year', width: 50 },
        { name: 'Месяц', field: 'Month', width: 50 },
        { name: 'Статус', field: 'StatusName', width: 200 },
        { name: 'Дата', field: 'Date', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, width: 100 },
        { name: 'Дата окончания', field: 'DateEnd', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, width: 100 },
        { name: 'Пользователь', field: 'UserFullName' },
        { name: 'Email', field: 'Email' },
        { name: 'Ошибка', field: 'ErrorMessage' },
    ];

    $scope.$on("$destroy", function handler() {
        $interval.cancel(intervalPromise);
    });

}

