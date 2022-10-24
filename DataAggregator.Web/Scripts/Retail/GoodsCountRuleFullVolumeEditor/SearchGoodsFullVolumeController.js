angular
    .module('DataAggregatorModule')
    .controller('SearchGoodsFullVolumeController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'formatConstants', SearchGoodsFullVolumeController]);

function SearchGoodsFullVolumeController($scope, $http, $modalInstance, uiGridCustomService, formatConstants) {

    $scope.currentBrand = undefined;

    // Поиск бренда
    $scope.searchBrand = function (value) {
        return $http.post('/GoodsBrand/SearchGoodsBrand/', JSON.stringify({ Value: value }))
            .then(function (response) {
            return response.data;
        });
    };

    $scope.setBrand = function (value) {
        if (!value || value.Id === undefined)
            return;

        var searchGoodsParameters = { BrandId: value.Id };

        $scope.loading = $http.post('/GoodsCountRuleFullVolumeEditor/SearchGoodsInRus/', JSON.stringify(searchGoodsParameters))
            .then(function (response) {
            $scope.grid.Options.data = response.data;
            $scope.grid.clearSelection();
        }, function () { $scope.message = 'Unexpected Error'; });
    };

    // Грид с результатами поиска GoodsId, OwnerTradeMarkId, PackerId
    $scope.grid = uiGridCustomService.createGridClass($scope, 'GoodsCountRuleFullValueEditor_SearchGoodsViewGrid');
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
        { name: 'SCPart', field: 'SellingCountPart', width: 70, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
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