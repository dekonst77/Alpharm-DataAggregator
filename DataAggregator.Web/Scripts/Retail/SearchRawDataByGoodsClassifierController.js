angular
    .module('DataAggregatorModule')
    .controller('SearchRawDataByGoodsClassifierController', ['$scope', '$http', 'uiGridCustomService', '$translate', 'uiGridConstants', 'formatConstants', SearchRawDataByGoodsClassifierController]);

function SearchRawDataByGoodsClassifierController($scope, $http, uiGridCustomService, $translate, uiGridConstants, formatConstants) {

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    $scope.detailingList =
    [
        { Id: 0, Name: $translate.instant('TRADE_NAME') },
        { Id: 1, Name: $translate.instant('BRAND') },
        { Id: 2, Name: $translate.instant('GOP') }
    ];

    $scope.periodDetailingList =
    [
        { Id: 0, Name: $translate.instant('RETAIL.SEARCH_RAW_DATA_BY_CLASSIFIER.PERIOD_DETAILING.NONE') },
        { Id: 1, Name: $translate.instant('RETAIL.SEARCH_RAW_DATA_BY_CLASSIFIER.PERIOD_DETAILING.YEAR') },
        { Id: 2, Name: $translate.instant('RETAIL.SEARCH_RAW_DATA_BY_CLASSIFIER.PERIOD_DETAILING.MONTH') }
    ];

    // Фильтр
    $scope.filter = {
        dateStart: previousMonthDate,
        dateEnd: previousMonthDate,
        region: {
            selectedItems: [],
            displayValue: '',
            search: searchRegion
        },
        tradeName: {
            selectedItems: [],
            displayValue: '',
            search: searchTradeName
        },
        brand: {
            selectedItems: [],
            displayValue: '',
            search: searchBrand
        },
        classifier: {
            selectedItems: [],
            displayValue: '',
            search: searchClassifier
        },
        detailing: $scope.detailingList[0],
        periodDetailing: $scope.periodDetailingList[0],
    };

    $scope.rawDataByClassifierGrid = {
        options: uiGridCustomService.createOptions('SearchRawDataByGoodsClassifier_RawDataByGoodsClassifierGrid')
    };

    var gridOptions = {
        gridVersion: '1.3',
        paginationPageSizes: [500, 1000, 5000, 10000, 50000],
        paginationPageSize: 500,
        customEnablePagination: true,
        customEnableCellSelection: true,
        onRegisterApi: function(gridApi) {
            $scope.gridApi = gridApi;
        }
    };

    var booleanCellTemplate = uiGridCustomService.getBooleanCellTemplate();
    var booleanCondition = uiGridCustomService.booleanCondition;

    $scope.rawDataByClassifierGrid.options.columnDefs = [
        { displayName: 'PERIOD', name:'PeriodYearMonth', field: 'getPeriodYearMonth()', width: 100, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filterCellFiltered: true, enableHiding: false },
        { displayName: 'PERIOD', name: 'PeriodYear', field: 'getPeriodYear()', width: 100, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_YEAR, filterCellFiltered: true, enableHiding: false },
        { displayName: 'COMMON_GRID.SOURCE_NAME', field: 'SourceName', visible: false },
        { displayName: 'COMMON_GRID.FILE_INFO_ID', field: 'FileInfoId', width: 100, type: 'number', visible: false },
        { displayName: 'COMMON_GRID.FILE_PATH', field: 'Path' },
        { displayName: 'RETAIL.SEARCH_RAW_DATA_BY_CLASSIFIER.TARGET_PHARMACY_ID', field: 'TargetPharmacyId', width: 100, type: 'number' },
        { displayName: 'REGION', name: 'RegionName', field: 'RegionName', visible: true, enableHiding: false },
        { displayName: 'RETAIL.SEARCH_RAW_DATA_BY_CLASSIFIER.CLASSIFICATION_NAME_OF_DETAILING', field: 'Name' },

        { displayName: 'ЧС по TargetPharmacy', field: 'IsTpBlackList', width: 120, cellTemplate: booleanCellTemplate, filter: { condition: booleanCondition } },
        { displayName: 'ЧС по TargetPharmacyBrand', field: 'IsTpBrandBlackList', width: 120, cellTemplate: booleanCellTemplate, filter: { condition: booleanCondition } },
        { displayName: 'ЧС по SourcePharmacy', field: 'IsSprBlackList', width: 120, cellTemplate: booleanCellTemplate, filter: { condition: booleanCondition } },

        { displayName: 'COMMON_GRID.SELLING_PRICE', field: 'SellingPriceNds', width: 120, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { displayName: 'COMMON_GRID.SELLING_COUNT', field: 'SellingCount', width: 120, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { displayName: 'COMMON_GRID.SELLING_SUM', field: 'SellingSumNds', width: 120, type: 'number', cellFilter: formatConstants.FILTER_PRICE },

        { displayName: 'COMMON_GRID.PURCHASE_PRICE', field: 'PurchasePriceNds', width: 120, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { displayName: 'COMMON_GRID.PURCHASE_COUNT', field: 'PurchaseCount', width: 120, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { displayName: 'COMMON_GRID.PURCHASE_SUM', field: 'PurchaseSumNds', width: 120, type: 'number', cellFilter: formatConstants.FILTER_PRICE }
    ];

    angular.extend($scope.rawDataByClassifierGrid.options, gridOptions);

    var periodYearMonthColumn = $scope.rawDataByClassifierGrid.options.columnDefs.find(function (item) { return item.name === 'PeriodYearMonth'; });
    var periodYearColumn = $scope.rawDataByClassifierGrid.options.columnDefs.find(function (item) { return item.name === 'PeriodYear'; });

    $scope.search = function () {

        function multiFieldToIdArray(field) {
            return field.selectedItems.map(function (r) { return r.Id; });
        }

        var filter = $scope.filter;

        var data =
        {
            yearStart: filter.dateStart.getFullYear(),
            monthStart: filter.dateStart.getMonth() + 1,
            yearEnd: filter.dateEnd.getFullYear(),
            monthEnd: filter.dateEnd.getMonth() + 1,

            regionCodes: filter.region.selectedItems.map(function(r) { return r.RegionCode; }),
            tradeNameIds: multiFieldToIdArray(filter.tradeName),
            brandIds: multiFieldToIdArray(filter.brand),
            classifierIds: multiFieldToIdArray(filter.classifier),

            detailingType: filter.detailing.Id,
            periodDetailingType: filter.periodDetailing.Id
        };

        $scope.loading = $http.post('/SearchRawDataByGoodsClassifier/GetData', JSON.stringify(data)).then(function(response) {
                $scope.rawDataByClassifierGrid.options.data = response.data;

                var getPeriodYearMonth = function() {
                    if (!this.Year || !this.Month)
                        return undefined;

                    return new Date(this.Year, this.Month - 1, 1);
                };

                var getPeriodYear = function () {
                    if (!this.Year || this.Month)
                        return undefined;

                    return new Date(this.Year, 0, 1);
                };

                angular.forEach($scope.rawDataByClassifierGrid.options.data, function (row) {
                    row.getPeriodYearMonth = getPeriodYearMonth;
                    row.getPeriodYear = getPeriodYear;
                });
            },
            function (response) {
                $scope.rawDataByClassifierGrid.options.data = [];
                alert('Ошибка:\n' + JSON.stringify(response));
            });
    }

    function searchRegion(value) {

        var httpPromise = $http.post('/Region/SearchRegion/', JSON.stringify({ Value: value }))
            .then(function (response) {
            var data = response.data;

            angular.forEach(data, function (item) {
                item.displayValue = item.Region;
            });
            return data;
        });

        return httpPromise;
    }

    $scope.$watch(
        function () { return $scope.filter.periodDetailing; },
        function () {
            checkPeriodVisibility();
        });

    function checkPeriodVisibility() {
        var periodDetailingId = $scope.filter.periodDetailing.Id;

        switch(periodDetailingId) {
            // NONE
            case 0:
                periodYearMonthColumn.visible = false;
                periodYearColumn.visible = false;
                break;
            // YEAR
            case 1:
                periodYearMonthColumn.visible = false;
                periodYearColumn.visible = true;
                break;
            // MONTH
            case 2:
                periodYearMonthColumn.visible = true;
                periodYearColumn.visible = false;
                break;
        }

        if ($scope.gridApi)
            $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
    }

    // Поиск торгового наименования
    function searchTradeName(value) {
        var httpPromise = $http.post('/GoodsTradeName/SearchGoodsTradeName/', JSON.stringify({ Value: value }))
            .then(function (response) {

                return prepareDictionary(response.data);
        });

        return httpPromise;
    };

    // Поиск бренда
    function searchBrand(value) {
        var httpPromise = $http.post('/GoodsBrand/SearchGoodsBrand/', JSON.stringify({ Value: value }))
            .then(function (response) {

                return prepareDictionary(response.data);
        });

        return httpPromise;
    };

    // Поиск Classifier
    function searchClassifier(value) {
        var httpPromise = $http.post('/GoodsExternalView/SearchGoodsExternalView/', JSON.stringify({ Value: value }))
            .then(function(response) {

                return prepareDictionary(response.data);
            });

        return httpPromise;
    };

    function prepareDictionary(dictionary) {
        angular.forEach(dictionary, function (item) {
            item.displayValue = item.Value;
        });

        return dictionary;
    }

    $scope.canSearch = function (filterForm) {
        if (filterForm.$invalid)
            return false;

        return true;
    }
}