angular
    .module('DataAggregatorModule')
    .controller('SearchGoodsController', ['$scope', '$http', '$uibModalInstance', 'model', 'uiGridCustomService', 'formatConstants', '$translate', SearchGoodsController]);

function SearchGoodsController($scope, $http, $modalInstance, model, uiGridCustomService, formatConstants, $translate) {

    $scope.currentBrand = undefined;
    $scope.regionInfo = undefined;

    var translatedRegionNames = {
        russia: $translate.instant('COUNTRIES.RUSSIA'),
        moscow: $translate.instant('CITIES.MOSCOW'),
        saintPetersburg: $translate.instant('CITIES.SAINT_PETERSBURG')
    };

    // Первоначальная загрузка формы при редактировании
    function initialize() {
        if (model.RegionRus) {
            $scope.regionInfo = translatedRegionNames.russia;
            return;
        }

        if (model.RegionMsk) {
            $scope.regionInfo = translatedRegionNames.moscow;
            return;
        }

        if (model.RegionSpb) {
            $scope.regionInfo = translatedRegionNames.saintPetersburg;
            return;
        }

        if (model.RegionCode) {
            $scope.regionInfo = model.Region;
            return;
        }

        $scope.regionInfo = '-';
    }

    initialize();

    // Поиск бренда
    $scope.searchBrand = function (value) {
        $scope.loading = $http.post('/GoodsBrand/SearchGoodsBrand/', JSON.stringify({ Value: value }))
            .then(function (response) {
            return response.data;
            });

        return $scope.loading;
    };

    $scope.setBrand = function(value) {
        if (!value || value.Id === undefined)
            return;

        var searchGoodsUrl;
        var searchGoodsParameters = {
            BrandId: value.Id,
            Year: model.Year,
            Month: model.Month
        };

        if (model.RegionCode) {
            searchGoodsUrl = '/GoodsCountRuleEditor/SearchGoodsInR12/';
            searchGoodsParameters.RegionCode = model.RegionCode;
        } else if (model.RegionSpb) {
            searchGoodsUrl = '/GoodsCountRuleEditor/SearchGoodsInR1/';
            searchGoodsParameters.RegionCode = '78';
        } else if (model.RegionMsk) {
            searchGoodsUrl = '/GoodsCountRuleEditor/SearchGoodsInR1/';
            searchGoodsParameters.RegionCode = '77';
        } else if (model.RegionRus) {
            searchGoodsUrl = '/GoodsCountRuleEditor/SearchGoodsInRus/';
        } else {
            searchGoodsUrl = '/GoodsCountRuleEditor/SearchGoods/';
        }

        $scope.loading = $http.post(searchGoodsUrl, JSON.stringify(searchGoodsParameters))
            .then(function (response) {
                $scope.grid.Options.data = response.data;
                $scope.grid.clearSelection();
            }, function() { $scope.message = 'Unexpected Error'; });
    };

    // Грид с результатами поиска GoodsId, OwnerTradeMarkId, PackerId
    $scope.grid = uiGridCustomService.createGridClass($scope, 'GoodsCountRuleEditor_SearchGoodsViewGrid');
    $scope.grid.Options.multiSelect = false;
    $scope.grid.Options.modifierKeysToMultiSelect = false;
    $scope.grid.Options.rowTemplate = '<div class="ui-grid-cell" ng-class=" {selected : row.isSelected, \'repeated\': row.entity.IsInCountRules }" ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" ui-grid-cell></div>';

    // Колонки грида
    $scope.grid.Options.columnDefs = [
        { name: 'G', field: 'GoodsId', width: 60, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'O', field: 'OwnerTradeMarkId', width: 60, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'P', field: 'PackerId', width: 60, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Описание', field: 'GoodsDescription', filter: { condition: uiGridCustomService.condition } },
        { name: 'Правообладатель', field: 'OwnerTradeMark', width: 100, filter: { condition: uiGridCustomService.condition } },
        { name: 'Упаковщик', field: 'Packer', width: 100, filter: { condition: uiGridCustomService.condition } },
        { name: 'SSum', field: 'SellingSumNDS', width: 100, cellFilter: formatConstants.FILTER_PRICE, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'SCount', field: 'SellingCount', width: 100, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'SSPart', field: 'SellingSumNDSPart', width: 70, cellFilter: formatConstants.FILTER_PRICE, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'SCPart', field: 'SellingCountPart', width: 70, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
    ];
    

    $scope.Select = function () {
        var items = $scope.grid.getSelectedItem();
        var item = items[0];

        $modalInstance.close(item);
    };

    $scope.canSelect = function () {
        var selectedItems = $scope.grid.getSelectedItem();

        return selectedItems && selectedItems.length > 0;
    };

    // Закрыть форму
    $scope.Close = function () {
        $modalInstance.dismiss();
    };

}