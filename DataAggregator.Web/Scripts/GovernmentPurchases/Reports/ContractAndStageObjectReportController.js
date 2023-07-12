angular
    .module('DataAggregatorModule')
    .controller('ContractAndStageObjectReportController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'formatConstants', 'errorHandlerService', ContractAndStageObjectReportController]);

function ContractAndStageObjectReportController(messageBoxService, $scope, $http, uiGridCustomService, formatConstants, errorHandlerService) {

    //Методы
    $scope.getReport = function () {
        getReport();
    }

    function getReport(filter) {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/ContractAndStageObjectReport/GetReport/",
            data: {
                PurchaseNumber: filter.purchaseNumber,
                ReestrNumber: filter.reestrNumber
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
        $scope.reportGrid = uiGridCustomService.createGridClass($scope, 'ContractAndStageObjectReport_Grid');
        $scope.reportGrid.Options.showGridFooter = true;
        $scope.reportGrid.Options.enableSorting = true,
        $scope.reportGrid.Options.columnDefs =
        [
            { name: 'Тип', field: 'Type', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Номер закупки', field: 'Number', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Номер контракта', field: 'ReestrNumber', filter: { condition: uiGridCustomService.conditionSpace } },

            { name: 'Сумма контракта', field: 'ContractSum', enableHiding: false, filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },

            { name: 'Фактически оплачено', field: 'ActuallyPaid', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Разница по оплате', field: 'DiffSum', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Сумма исполнения', field: 'SumIsp', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Id об', field: 'ObjectId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true },
            { name: 'Имя об', field: 'Name', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Единица об', field: 'Unit', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Кол-во об', field: 'Amount', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Цена об', field: 'Price', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Сумма об', field: 'Sum', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Classifier Id', field: 'ClassifierId', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'МНН', field: 'INNGroup', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Торг наимен', field: 'TradeName', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Описание', field: 'DrugDescription', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Производитель', field: 'Corporation', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Упаковщик', field: 'Packer', filter: { condition: uiGridCustomService.conditionSpace } },
            { name: 'Кол-во расч', field: 'ObjectCalculatedAmount', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Цена расч', field: 'ObjectCalculatedPrice', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Сумма расч', field: 'ObjectCalculatedSum', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Серия ЛП', field: 'Seria', filter: { condition: uiGridCustomService.conditionSpace } }
        ];

    //Фильтр
    var filterClass = function (loadFunction) {
        this.purchaseNumber = "";
        this.reestrNumber = "";
        this.reportObject = "Report";
        this.showReportGrid = true;

        this.getReport = function () {
            if (!this.validate()) {
                return;
            }

            loadFunction(this);
        }

        this.validate = function () {
            if (this.purchaseNumber.Value != undefined && this.purchaseNumber.Value.length > 250
                || this.reestrNumber.Value != undefined && this.reestrNumber.Value.length > 250) {
                $scope.message = "User input error";
                messageBoxService.showError("Длина списка не должна превышать 250 символов!");
                return false;
            }
            return true;
        }

        this.getJson = function () {
            var filterTransfer = {
                PurchaseNumber: this.purchaseNumber.Value,
                ReestrNumber: this.reestrNumber.Value
            }
            //return JSON.stringify({ filter: filterTransfer });
            return JSON.stringify({
                PurchaseNumber: this.purchaseNumber.Value,
                ReestrNumber: this.reestrNumber.Value
            });
        }
    }

    //Объект фильтр
    $scope.filter = new filterClass(getReport);
}



