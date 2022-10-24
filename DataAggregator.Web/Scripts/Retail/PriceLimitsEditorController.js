angular
    .module('DataAggregatorModule')
    .controller('PriceLimitsEditorController', ['$scope', '$http', 'uiGridCustomService', 'formatConstants', 'hotkeys', PriceLimitsEditorController]);

function PriceLimitsEditorController($scope, $http, uiGridCustomService, formatConstants, hotkeys) {
    $scope.gridApi = undefined;

    hotkeys.add({
        combo: 'del',
        callback: function () {
            $scope.clearPrices();
        }
    });

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    // Фильтр
    $scope.filter = {
        date: previousMonthDate
    };

    $scope.priceRangesGrid = {
        options: uiGridCustomService.createOptions('PriceLimitsEditor_PriceRangesGrid')
    };

    $scope.priceRangesGrid.options.columnDefs = [
        { displayName: 'PERIOD', field: 'getPeriod()', width: 100, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filterCellFiltered: true },
        { name: 'Код региона', field: 'RegionCode', visible: false },
        { name: 'Регион', field: 'RegionName' },
        { name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Торговое наименование', field: 'TradeName' },
        { name: 'ФВ, фас, доз', field: 'DrugDescription' },
        { name: 'Производитель', field: 'OwnerTradeMark' },
        { name: 'Ср цена прод', field: 'AvgSellingPrice', type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Ср цена зак', field: 'AvgPurchasePrice', type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Отклонение', field: 'PriceDeviation' },
        { name: 'PriceRangeId', field: 'PriceRangeId', type: 'number', visible: false },
        { name: 'Мин цена зак', field: 'PurchasePriceMin', type: 'number', cellFilter: formatConstants.FILTER_PRICE, enableCellEdit: true },
        { name: 'Макс цена зак', field: 'PurchasePriceMax', type: 'number', cellFilter: formatConstants.FILTER_PRICE, enableCellEdit: true },
        { name: 'Мин цена прод', field: 'SellingPriceMin', type: 'number', cellFilter: formatConstants.FILTER_PRICE, enableCellEdit: true },
        { name: 'Макс цена прод', field: 'SellingPriceMax', type: 'number', cellFilter: formatConstants.FILTER_PRICE, enableCellEdit: true }
    ];

    $scope.priceRangesGrid.options.customEnableCellSelection = true;

    $scope.$on('uiGridEventEndCellEdit', function (data) {
        var newValue = data.targetScope.row.entity;
        editRangeByEntity(newValue);
    });

    $scope.$watch(function () { return $scope.filter.date; },
        function () {
            getRanges();
        },
        true);

    function getRanges() {
        var data =
        {
            year: $scope.filter.date.getFullYear(),
            month: $scope.filter.date.getMonth() + 1
        };

        $scope.loading = $http.get('/PriceLimitsEditor/GetRanges', { params: data })
            .then(function (response) {
            $scope.priceRangesGrid.options.data = response.data;

            var period = new Date($scope.filter.date.getFullYear(), $scope.filter.date.getMonth(), 1);

            angular.forEach($scope.priceRangesGrid.options.data, function (row) {
                row.getPeriod = function () {
                    return period;
                }
            });
        }, function (response) {
            $scope.priceRangesGrid.options.data = [];
            alert('Ошибка:\n' + JSON.stringify(response));
        });
    }

    function editRangeByEntity(entity) {
        
        entity.PurchasePriceMin = entity.PurchasePriceMin ? entity.PurchasePriceMin : undefined;
        entity.PurchasePriceMax = entity.PurchasePriceMax ? entity.PurchasePriceMax : undefined;
        entity.SellingPriceMin = entity.SellingPriceMin ? entity.SellingPriceMin : undefined;
        entity.SellingPriceMax = entity.SellingPriceMax ? entity.SellingPriceMax : undefined;

        var request = {
            priceRangeId: entity.PriceRangeId,
            month: entity.Month,
            year: entity.Year,
            drugId: entity.DrugId,
            ownerTradeMarkId: entity.OwnerTradeMarkId,
            regionCode: entity.RegionCode,
            purchasePriceMin: entity.PurchasePriceMin,
            purchasePriceMax: entity.PurchasePriceMax,
            sellingPriceMin: entity.SellingPriceMin,
            sellingPriceMax: entity.SellingPriceMax
        };

        var promise = $http({
            method: 'POST',
            url: '/PriceLimitsEditor/EditRange',
            data: JSON.stringify(request)
        }).then(function(response) {
                entity.PriceRangeId = response.data.priceRangeId;
            },
            function(response) {
                alert('Ошибка:\n' + JSON.stringify(response));
            });

        if (!entity.PriceRangeId)
            $scope.loading = promise;
    };


    $scope.duplicatePrices = function () {
        var selectedRows = $scope.priceRangesGrid.options.customCellSelect.selectedRows;

        var firstRowEntity = selectedRows[0].entity;

        for (var i = 1; i < selectedRows.length; i++) {
            var rowEntity = selectedRows[i].entity;

            rowEntity.PurchasePriceMin = firstRowEntity.PurchasePriceMin;
            rowEntity.PurchasePriceMax = firstRowEntity.PurchasePriceMax;
            rowEntity.SellingPriceMin = firstRowEntity.SellingPriceMin;
            rowEntity.SellingPriceMax = firstRowEntity.SellingPriceMax;

            editRangeByEntity(rowEntity);
        }
    };

    $scope.clearPrices = function () {
        var selectedRows = $scope.priceRangesGrid.options.customCellSelect.selectedRows;
        var selectedCells = $scope.priceRangesGrid.options.customCellSelect.selectedCells;

        var availableFieldsToClear = ['PurchasePriceMin', 'PurchasePriceMax', 'SellingPriceMin', 'SellingPriceMax'];

        var i;
        var entities = {};
        var row;
        var entity;

        for (i = 0; i < selectedRows.length; i++) {
            row = selectedRows[i];
            entities[row.uid] = {
                value: row.entity,
                isChanged: false
            };
        }

        for (i = 0; i < selectedCells.length; i++) {
            var cell = selectedCells[i];

            entity = entities[cell.rowCol.row.uid];

            var fieldName = cell.rowCol.col.field;

            for (var j = 0; j < availableFieldsToClear.length; j++) {
                if (fieldName === availableFieldsToClear[j]) {
                    entity.value[fieldName] = undefined;
                    entity.isChanged = true;
                }
            }
        }

        for (var rowUid in entities) {
            if (!entities.hasOwnProperty(rowUid))
                continue;

            entity = entities[rowUid];

            if (entity.isChanged)
                editRangeByEntity(entity.value);
        }
    };
}