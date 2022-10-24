angular
    .module('DataAggregatorModule')
    .controller('PurchasesAndContractsStatisticsController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', PurchasesAndContractsStatisticsController]);

function PurchasesAndContractsStatisticsController(messageBoxService, $scope, $http, uiGridCustomService, uiGridConstants, formatConstants) {
    $scope.format = 'yyyy-MM-dd';

    //Методы
    $scope.getStatistics = function () {
        getStatistics();
    }

    function getStatistics(filter) {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/PurchasesAndContractsStatistics/GetStatistics/",
            data: filter.getJson(),
        }).then(function (response) {
            $scope.statisticsGrid.Options.data = response.data.reportData;
            if (response.data.count == 50000) {
                messageBoxService.showInfo("Показано 50 000 записей! Возможно, это не все данные.");
            }
        }, function () {
            $scope.message = "Unexpected Error";
            messageBoxService.showError("Не удалось загрузить статистику!");
        });
    }    

    $scope.objectChange = function (objectName) {
        if (objectName === 'Purchases') {
            $scope.statisticsGrid.Options.columnDefs[3].visible = false;
        } else {
            $scope.statisticsGrid.Options.columnDefs[3].visible = true;
        }
        $scope.statisticsGrid.Options.data.length = 0;
        $scope.gridApi.grid.refresh();
    }

    //Статистика
    $scope.statisticsGrid = uiGridCustomService.createGridClass($scope, 'PurchasesAndContractsStatistics_Grid');
    $scope.statisticsGrid.Options.showGridFooter = true;
    $scope.statisticsGrid.Options.enableSorting = true,
    $scope.statisticsGrid.Options.columnDefs =
    [
        { name: 'Раздел', field: 'ClassName', filter: { condition: uiGridCustomService.condition }, sort: { direction: uiGridConstants.ASC } },
        { name: 'Статус', field: 'StatusName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Количество', field: 'Count', filter: { condition: uiGridCustomService.condition }, type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT },
        { name: 'КК', field: 'KK', filter: { condition: uiGridCustomService.condition }, visible: false, enableHiding: false },
        { name: 'PDF', field: 'PDF', filter: { condition: uiGridCustomService.condition }, visible: false, enableHiding: false }
    ];

    $scope.statisticsGrid.Options.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    };

    //Фильтр
    var filterClass = function (loadFunction) {

        this.dateStart = new dateClass();
        this.dateStart.setTodayWithoutTime();
        this.dateEnd = new dateClass();
        this.dateEnd.setTodayWithoutTime();
        this.statisticsObject = "Purchases";

        this.getStatistics = function () {
            if (!this.validate()) {
                return;
            }

            loadFunction(this);
        }

        this.validate = function () {
            if (this.dateStart.Value == undefined) {
                $scope.message = "User input error";
                messageBoxService.showError("Неверно указано начало периода!");
                return false;
            }
            if (this.dateEnd.Value == undefined) {
                $scope.message = "User input error";
                messageBoxService.showError("Неверно указан конец периода!");
                return false;
            }
            if (this.dateEnd.Value < this.dateStart.Value) {
                $scope.message = "User input error";
                messageBoxService.showError("Дата конца периода не может быть меньше даты начала!");
                return false;
            }
            return true;
        }

        this.getJson = function () {
            var filterTransfer = {
                DateStart: this.dateStart.Value,
                DateEnd: this.dateEnd.Value,
                StatisticsObject: this.statisticsObject,
            }
            return JSON.stringify({ filter: filterTransfer });
        }
    }

    //Объект фильтр
    $scope.filter = new filterClass(getStatistics);
}



