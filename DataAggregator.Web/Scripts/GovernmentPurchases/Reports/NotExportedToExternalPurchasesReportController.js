angular
    .module('DataAggregatorModule')
    .controller('NotExportedToExternalPurchasesReportController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'formatConstants', 'errorHandlerService', NotExportedToExternalPurchasesReportController]);

function NotExportedToExternalPurchasesReportController(messageBoxService, $scope, $http, uiGridCustomService, formatConstants, errorHandlerService) {

    //Методы
    $scope.getReport = function () {
        getReport();
    }

    function getReport(filter) {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/NotExportedToExternalPurchasesReport/GetReport/",
            data: {
                DateStart: filter.dateStart.Value,
                DateEnd: filter.dateEnd.Value,
                ReportObject: filter.reportObject
            }
        }).then(function (response) {
            if ($scope.filter.showPurchasesGrid) {
                $scope.purchasesGrid.Options.data = response.data;
            } else {
                $scope.contractsGrid.Options.data = response.data;
            }
            if (response.data.length === 50000) {
                messageBoxService.showInfo("Показано 50 000 записей! Возможно, это не все данные.");
            }
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.objectChange = function (objectName) {
        $scope.purchasesGrid.Options.data = [];
        $scope.contractsGrid.Options.data = [];

        if (objectName === 'Purchases') {
            $scope.filter.showPurchasesGrid = true;
        } else {
            $scope.filter.showPurchasesGrid = false;
        }
    }

    var booleanCellTemplate = uiGridCustomService.getBooleanCellTemplate();
    var booleanCondition = uiGridCustomService.booleanCondition;

    //Закупки
    $scope.purchasesGrid = uiGridCustomService.createGridClass($scope, 'NotExportedToExternalPurchasesReport_Grid');
    $scope.purchasesGrid.Options.showGridFooter = true;
    $scope.purchasesGrid.Options.enableSorting = true,
    $scope.purchasesGrid.Options.columnDefs =
    [
        { name: 'Id закупки', field: 'PurchaseId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
        { name: 'Номер закупки', field: 'PurchaseNumber', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Название закупки', field: 'PurchaseName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Id лота', field: 'LotId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
        { name: 'Номер лота', field: 'LotNumber', filter: { condition: uiGridCustomService.condition } },
        { name: 'Сумма лота', field: 'LotSum', enableHiding: false, filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Нет количества', field: 'BadCount', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Нет суммы лота', field: 'BadLotSum', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Плохие объекты', field: 'BadObjects', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Нет Категории или Характера', field: 'BadNature', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Нет периода поставки', field: 'BadDeliveryTimeInfo', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Не указано финансирование хотя бы у 1 лота', field: 'BadLotFunding', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Коэфф. хотя бы 1 объекта выходит за пределы 0,1 < k < 10', field: 'BadCoefficient', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Sрасч > Sлота', field: 'BadObjSum', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Дата начала подачи заявок', field: 'PurchaseDateBegin', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата окончания процедуры', field: 'PurchaseDateEnd', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } }
    ];

    //Контракты
    $scope.contractsGrid = uiGridCustomService.createGridClass($scope, 'NotExportedToExternalPurchasesReport_Grid');
    $scope.contractsGrid.Options.showGridFooter = true;
    $scope.contractsGrid.Options.enableSorting = true,
    $scope.contractsGrid.Options.columnDefs =
    [
        { name: 'Id закупки', field: 'PurchaseId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
        { name: 'Номер закупки', field: 'PurchaseNumber', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Id контаркта', field: 'ContractId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
        { name: 'Номер контракта', field: 'ContrReestrNumber', filter: { condition: uiGridCustomService.condition } },
        { name: 'Сумма контракта', field: 'ContractSum', enableHiding: false, filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Нет количества', field: 'BadCount', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Плохие объекты', field: 'BadObjects', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Нет Категории или Характера', field: 'BadNature', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Нет периода поставки', field: 'BadDeliveryTimeInfo', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Не указано финансирование хотя бы у 1 лота', field: 'BadLotFunding', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Sрасч > Sконтракта', field: 'BadObjSum', filter: { condition: booleanCondition }, cellTemplate: booleanCellTemplate },
        { name: 'Дата начала подачи заявок', field: 'PurchaseDateBegin', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата окончания процедуры', field: 'PurchaseDateEnd', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } }
    ];

    //Фильтр
    var filterClass = function (loadFunction) {
        this.dateStart = new dateClass();
        this.dateStart.setTodayWithoutTime();
        this.dateEnd = new dateClass();
        this.dateEnd.setTodayWithoutTime();
        this.reportObject = "Purchases";
        this.showPurchasesGrid = true;

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



