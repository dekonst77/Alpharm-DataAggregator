angular
    .module('DataAggregatorModule')
    .controller('ExecutionTerminatedContractReportController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'formatConstants', 'errorHandlerService', ExecutionTerminatedContractReportController]);

function ExecutionTerminatedContractReportController(messageBoxService, $scope, $http, uiGridCustomService, formatConstants, errorHandlerService) {

    //Методы
    $scope.getReport = function () {
        getReport();
    }

    function getReport(filter) {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/ExecutionTerminatedContractReport/GetReport/",
            data: {
                DateStart: filter.dateStart.Value,
                DateEnd: filter.dateEnd.Value
            }
        }).then(function (response) {
              $scope.reportGrid.Options.data = response.data;
            if (response.data.length === 50000) {
                messageBoxService.showInfo("Показано 50 000 записей! Возможно, это не все данные.");
            }
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.objectChange = function (objectName) {
        $scope.reportGrid.Options.data = [];
        $scope.filter.showReportGrid = true;
    }

    var booleanCellTemplate = uiGridCustomService.getBooleanCellTemplate();
    var booleanCondition = uiGridCustomService.booleanCondition;

    //Отчет
        $scope.reportGrid = uiGridCustomService.createGridClass($scope, 'ExecutionTerminatedContractReport_Grid');
        $scope.reportGrid.Options.showGridFooter = true;
        $scope.reportGrid.Options.enableSorting = true,
        $scope.reportGrid.Options.columnDefs =
        [
            { name: 'Id закупки', field: 'PurchaseId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
            { name: 'Номер закупки', field: 'Number', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Номер контракта', field: 'ContractNumber', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Номер реестра', field: 'ReestrNumber', filter: { condition: uiGridCustomService.conditionSpace } },
            {
                name: 'Ссылка на контракт', enableCellEdit: false, field: 'Url', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="{{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { name: 'Сумма контракта', field: 'Sum', enableHiding: false, filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Дата начала подачи заявок', field: 'DateBegin', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } },
            {
                name: 'Примечание', field: 'Comment', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
            }
        ];

    //Фильтр
    var filterClass = function (loadFunction) {
        this.dateStart = new dateClass();
        this.dateStart.setTodayWithoutTime();
        this.dateEnd = new dateClass();
        this.dateEnd.setTodayWithoutTime();
        this.reportObject = "Report";
        this.showReportGrid = true;

        this.getReport = function () {
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
                ReportObject: this.reportObject
            }
            //return JSON.stringify({ filter: filterTransfer });
            return JSON.stringify({
                DateStart: this.dateStart.Value,
                DateEnd: this.dateEnd.Value,
                ReportObject: this.reportObject
            });
        }
    }

    //Объект фильтр
    $scope.filter = new filterClass(getReport);
}



