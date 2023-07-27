angular
    .module('DataAggregatorModule')
    .controller('SearchRawDataByDrugClearController', ['$scope', '$http', 'uiGridCustomService', 'formatConstants', SearchRawDataByDrugClearController]);

function SearchRawDataByDrugClearController($scope, $http, uiGridCustomService, formatConstants) {

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    $scope.format = 'MM.yyyy';

    // Фильтр
    $scope.filter = {
        startDate: previousMonthDate,
        endDate: previousMonthDate,
        drugClearIds: '',
        drugName: ''
    };

    $scope.rawDataByDrugClearGrid = {
        options: uiGridCustomService.createOptions('SearchRawDataByDrugClear_RawDataByDrugClearGrid')
    };

    $scope.rawDataByDrugClearGrid.options.columnDefs = [
        { displayName: 'PERIOD', field: 'getPeriod()', width: 100, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filterCellFiltered: true },
        { displayName: 'COMMON_GRID.SOURCE_NAME', field: 'SourceName', visible: false },
        { displayName: 'COMMON_GRID.FILE_INFO_ID', field: 'FileInfoId', width: 100, type: 'number', visible: false },
        { displayName: 'COMMON_GRID.FILE_PATH', field: 'Path' },
        { displayName: 'RETAIL.SEARCH_RAW_DATA_BY_DRUG_CLEAR.PHARMACY_NAMES', field: 'PharmacyNames' },
        { displayName: 'RETAIL.SEARCH_RAW_DATA_BY_DRUG_CLEAR.ORIGINAL_DRUG_NAME', field: 'Drug' },
        { displayName: 'RETAIL.SEARCH_RAW_DATA_BY_DRUG_CLEAR.ORIGINAL_MANUFACTURER_NAME', field: 'Manufacturer' },

        { displayName: 'COMMON_GRID.SELLING_PRICE', field: 'SellingPriceNds', width: 120, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { displayName: 'COMMON_GRID.SELLING_COUNT', field: 'SellingCount', width: 120, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { displayName: 'COMMON_GRID.SELLING_SUM', field: 'SellingSumNds', width: 120, type: 'number', cellFilter: formatConstants.FILTER_PRICE },

        { displayName: 'COMMON_GRID.PURCHASE_PRICE', field: 'PurchasePriceNds', width: 120, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { displayName: 'COMMON_GRID.PURCHASE_COUNT', field: 'PurchaseCount', width: 120, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { displayName: 'COMMON_GRID.PURCHASE_SUM', field: 'PurchaseSumNds', width: 120, type: 'number', cellFilter: formatConstants.FILTER_PRICE }

    ];

    $scope.rawDataByDrugClearGrid.options.customEnableCellSelection = true;

    $scope.search = function () {
        var data =
            {
                startYear: $scope.filter.startDate.getFullYear(),
                startMonth: $scope.filter.startDate.getMonth() + 1,
                endYear: $scope.filter.endDate.getFullYear(),
                endMonth: $scope.filter.endDate.getMonth() + 1,
                drugClearIds: $scope.filter.drugClearIds.split(','),
                drugName: $scope.filter.drugName
            };

        $scope.loading = $http.get('/SearchRawDataByDrugClear/GetData',
            { params: data }).then(function (response) {
                $scope.rawDataByDrugClearGrid.options.data = response.data;

                angular.forEach($scope.rawDataByDrugClearGrid.options.data, function (row) {
                    row.getPeriod = function () {
                        return new Date(this.Year, this.Month - 1, 1);
                    }
                });
            },
            function (response) {
                $scope.rawDataByDrugClearGrid.options.data = [];
                alert('Ошибка:\n' + JSON.stringify(response));
            });
    }

    // Обработка событий календаря
    $scope.popupStartDate = {
        opened: false
    };

    // Обработка событий календаря
    $scope.openStartDate = function () {
        $scope.popupStartDate.opened = true;
    };

    // Обработка событий календаря
    $scope.popupEndDate = {
        opened: false
    };

    // Обработка событий календаря
    $scope.openEndDate = function () {
        $scope.popupEndDate.opened = true;
    };

}