angular
    .module('DataAggregatorModule')
    .controller('DrugIdWithMinMaxPriceReportController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', DrugIdWithMinMaxPriceReportController]);

function DrugIdWithMinMaxPriceReportController(messageBoxService, $scope, $http, uiGridCustomService, uiGridConstants, formatConstants) {

    //Методы
    $scope.refresh = function () {
        getReport();
    };

    $scope.filter = {
        dateStart : new dateClass(),        
        dateEnd : new dateClass()
    };
    $scope.filter.dateStart.setTodayWithoutTime();
    $scope.filter.dateEnd.setTodayWithoutTime();

    function getReport() {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/DrugIdWithMinMaxPriceReport/GetReport/",
            data: JSON.stringify({
                DateStart: $scope.filter.dateStart.Value,
                DateEnd: $scope.filter.dateEnd.Value})
        }).then(function (response) {
            $scope.reportGrid.Options.data = response.data.reportData;
            if (response.data.count == 50000) {
                messageBoxService.showInfo("Показано 50 000 записей! Возможно, это не все данные.");
            }
        }, function () {
            $scope.message = "Unexpected Error";
            messageBoxService.showError("Не удалось загрузить отчёт!");
        });
    }    

    //Отчёт
    $scope.reportGrid = uiGridCustomService.createGridClass($scope, 'DrugIdWithMinMaxPriceReport_Grid');
    $scope.reportGrid.Options.showGridFooter = true;
    $scope.reportGrid.Options.enableSorting = true,
    $scope.reportGrid.Options.columnDefs =
    [
        { name: 'Источник цены', field: 'Source', filter: { condition: uiGridCustomService.condition } },
        { name: 'DrugId', field: 'DrugId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
        { name: 'Наименование', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Описание', field: 'DrugDescription', enableHiding: false, filter: { condition: uiGridCustomService.condition } },
        { name: 'MinPrice', field: 'MinPrice', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'MaxPrice', field: 'MaxPrice', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Коэффициент', field: 'Coeff', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_COEFFICIENT, sort: { direction: uiGridConstants.DESC } }
    ];

}



