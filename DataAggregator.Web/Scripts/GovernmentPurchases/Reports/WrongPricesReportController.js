angular
    .module('DataAggregatorModule')
    .controller('WrongPricesReportController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'formatConstants', WrongPricesReportController]);

function WrongPricesReportController(messageBoxService, $scope, $http, uiGridCustomService, formatConstants) {

    //Методы
    $scope.getReport = function () {
        getReport();
    };

    function getReport(filter) {
        filter.lessCoeff = Number(filter.lessCoeff);//для корректного преобразования числа 
        filter.moreCoeff = Number(filter.moreCoeff);//с запятой в decimal

        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/WrongPricesReport/GetReport/",
            data: filter.getJson()
        }).then(function (response) {
            $scope.reportGrid.Options.data = response.data.reportData;
            if (response.data.count === 50000) {
                messageBoxService.showInfo("Показано 50 000 записей! Возможно, это не все данные.");
            }
        }, function () {
            $scope.message = "Unexpected Error";
            messageBoxService.showError("Не удалось загрузить отчёт!");
        });
    }    

    //Отчёт
    $scope.reportGrid = uiGridCustomService.createGridClass($scope, 'WrongPricesReport_Grid');
    $scope.reportGrid.Options.showGridFooter = true;
    $scope.reportGrid.Options.enableSorting = true,
    $scope.reportGrid.Options.columnDefs =
    [
        { name: 'Тип объекта', field: 'ObjectType', filter: { condition: uiGridCustomService.condition } },
        { name: 'Id закупки', field: 'PurchaseId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
        { name: 'Номер закупки', field: 'PurchaseNumber', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Дата начала подачи заявок', field: 'DateBegin', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } },
        { name: 'Характер', field: 'NatureName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Сумма лота', field: 'LotSum', enableHiding: false, filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'DrugId', field: 'DrugId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
        { name: 'МНН', field: 'InnGroup', filter: { condition: uiGridCustomService.condition } },
        { name: 'Коэффициент отклонения', field: 'FDPriceCoefficient', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_COEFFICIENT },
        { name: 'Цена рассч.', field: 'ObjectCalculatedPrice', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Ср. цена по ФО', field: 'FDAveragePrice', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Ед. измер.', field: 'ObjectReadyUnit', filter: { condition: uiGridCustomService.condition } },
        { name: 'Количество', field: 'ObjectReadyAmount', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { name: 'Наименование', field: 'ObjectReadyName', filter: { condition: uiGridCustomService.condition } },
        { name: 'ВНЦ', field: 'VNC', filter: { condition: uiGridCustomService.condition } },
        { name: 'kofPriceGZotkl', field: 'kofPriceGZotkl', filter: { condition: uiGridCustomService.condition }, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT}

    ];

    //Фильтр
    var filterClass = function (loadFunction) {

        this.dateStart = new dateClass();
        this.dateStart.setTodayWithoutTime();
        this.dateEnd = new dateClass();
        this.dateEnd.setTodayWithoutTime();
        this.includePurchases = false;
        this.includeContracts = false;
        this.lessCoeff = 0.2;
        this.moreCoeff = 5;

        this.getReport = function () {
            if (!this.validate()) {
                return;
            }

            loadFunction(this);
        };

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
            if (this.lessCoeff == undefined || this.lessCoeff === "") {
                $scope.message = "User input error";
                messageBoxService.showError("Укажите значение коэффициента \"меньше\"!");
                return false;
            }
            if (this.moreCoeff == undefined || this.moreCoeff === "") {
                $scope.message = "User input error";
                messageBoxService.showError("Укажите значение коэффициента \"больше\"!");
                return false;
            }
            if (this.lessCoeff > this.moreCoeff) {
                $scope.message = "User input error";
                messageBoxService.showError("Коэффициент \"меньше\" должен быть меньше коэффициента \"больше\"");
                return false;
            }
            if (!this.includeContracts && !this.includePurchases) {
                $scope.message = "User input error";
                messageBoxService.showError("Выберите хотя бы один тип объектa!");
                return false;
            }

            return true;
        };

        this.getJson = function () {
            var filterTransfer = {
                DateStart: this.dateStart.Value,
                DateEnd: this.dateEnd.Value,
                IncludePurchases: this.includePurchases,
                IncludeContracts: this.includeContracts,
                LessCoeff: this.lessCoeff,
                MoreCoeff: this.moreCoeff
            };
            return JSON.stringify({ filter: filterTransfer });
        };
    };

    //Объект фильтр
    $scope.filter = new filterClass(getReport);
}



