angular
    .module('DataAggregatorModule')
    .controller('PriceRuleEditorController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', 'messageBoxService', 'errorHandlerService', PriceRuleEditorController]);

function PriceRuleEditorController($scope, $http, $uibModal, uiGridCustomService, formatConstants, messageBoxService, errorHandlerService) {

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    $scope.items = null;

    // Фильтр
    $scope.filter = {
        date: previousMonthDate
    };

    function getYearMonth() {
        return {
            year: $scope.filter.date.getFullYear(),
            month: $scope.filter.date.getMonth() + 1
        };
    }

    //Модель формы
    $scope.model = {};

    $scope.priceRuleListGrid = {
        options: uiGridCustomService.createOptions('PriceRuleEditor_PriceRuleListGrid')
    };

    $scope.priceRuleListGrid.options.customEnableCellSelection = true;
    $scope.priceRuleListGrid.options.customCellSelect =
    {
        selectedChanged: selectedChanged
    };

    //Выделение элемента в гриде
    function selectedChanged() {
        var selectedRows = $scope.priceRuleListGrid.options.customCellSelect.selectedRows;

        var selectedRowsEntities = selectedRows.map(function (gridRow) { return gridRow.entity;});
        $scope.items = selectedRowsEntities;

        if ($scope.items.length === 0)
            $scope.model = {};
        else
            $scope.model = $scope.items[0];
    }

    $scope.priceRuleListGrid.options.columnDefs = [
        { name: 'Id', field: 'PriceRuleId', type: 'number' },
        { displayName: 'PERIOD', field: 'getPeriod()', width: 100, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filterCellFiltered: true },
        { name: 'Дата изменения', field: 'Date', width: 100, type: 'date', cellFilter: formatConstants.FILTER_DATE, filterCellFiltered: true, visible: false },
        { name: 'Код региона', field: 'RegionCode' },
        { name: 'Регион', field: 'RegionFullName' },
        { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Продажи от', field: 'SellingPriceMin', type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Продажи до', field: 'SellingPriceMax', type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Закупки от', field: 'PurchasePriceMin', type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Закупки до', field: 'PurchasePriceMax', type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Торговое наименование', field: 'TradeName' },
        { name: 'Бренд', field: 'Brand' },
        { name: 'DrugDescription', field: 'DrugDescription' },
        { name: 'Производитель', field: 'OwnerTradeMark' },
        { name: 'Упаковщик', field: 'Packer' },
        { name: 'Пользователь', field: 'Surname' },
        { name: 'Комментарий', field: 'Comment' },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.OUT_USED', field: 'OutUsed', width: 25,
            cellTemplate: "<div class='ui-grid-cell-contents'>{{row.entity.OutUsed ? 'Да' : 'Нет'}}</div>"
        },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.IN_USED', field: 'InUsed', width: 25,
            cellTemplate: "<div class='ui-grid-cell-contents'>{{row.entity.InUsed ? 'Да' : 'Нет'}}</div>"
        }
    ];

    $scope.priceRuleListGrid.options.gridVersion = '1.1';

    function getPriceRuleList() {
        $scope.loading = $http.get('/PriceRuleEditor/GetPriceRuleList', { params: getYearMonth() })
            .then(function(response) {
                    $scope.priceRuleListGrid.options.data = response.data;

                    var period = new Date($scope.filter.date);

                    angular.forEach($scope.priceRuleListGrid.options.data, function (row) {
                        row.getPeriod = function() {
                            return period;
                        };
                    });

                    $scope.items = null;
                },
            function(response) {
                $scope.priceRuleListGrid.options.data = [];
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.addPriceRule = function () {

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/PriceRuleEditor/_PriceRuleView.html',
            controller: 'PriceRuleViewController',
            size: 'lg',
            windowClass: 'center-modal wide-dialog',
            backdrop: 'static',
            resolve: {
                model: function () {
                    var data = getYearMonth();
                    return { Year: data.year, Month: data.month };
                },
                editmode: function () { return false; }
            }
        });

        modalInstance.result.then(function (v) {
            getPriceRuleList();
        }, function () {
        });
    };

    $scope.canEdit = function() {
        return $scope.items !== null && $scope.items.length === 1 && $scope.filter.date !== null && $scope.model.PriceRuleId;
    };

    $scope.editPriceRule = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/PriceRuleEditor/_PriceRuleView.html',
            controller: 'PriceRuleViewController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                model: function () {
                    var data = getYearMonth();

                    var model = JSON.parse(JSON.stringify($scope.model));

                    model.Year = data.year;
                    model.Month = data.month;

                    return model;
                },
                editmode: function() { return true; }
            }
        });

        modalInstance.result.then(function () {
            getPriceRuleList();
        }, function () {
        });
    };

    $scope.deletePriceRule = function() {
        messageBoxService.showConfirm('Удалить выбранное правило?', 'Удаление')
            .then(function() {
                    $scope.loading = $http.post('/PriceRuleEditor/DeletePriceRule', JSON.stringify({ priceRuleId: $scope.model.PriceRuleId }))
                    .then(function() {
                            $scope.model = {};
                            $scope.priceRuleListGrid.options.customCellSelect.clearSelection();
                            getPriceRuleList();
                        },
                        function(response) {
                            errorHandlerService.showResponseError(response);
                        });
                },
                function() {});
    };

    $scope.$watch(
        function() { return $scope.filter.date; },
        function (newValue, oldValue) {
            if (newValue === oldValue && newValue === undefined)
                return;

            getPriceRuleList();
        },
        true
    );
}