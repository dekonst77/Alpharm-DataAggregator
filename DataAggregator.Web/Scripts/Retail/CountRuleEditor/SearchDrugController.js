angular
    .module('DataAggregatorModule')
    .controller('SearchDrugController', ['$scope', '$http', '$uibModalInstance', 'model', 'uiGridCustomService', 'formatConstants', '$translate', SearchDrugController]);

function SearchDrugController($scope, $http, $modalInstance, model, uiGridCustomService, formatConstants, $translate) {

    $scope.currentBrand = undefined;
    $scope.regionInfo = undefined;
    

    // Первоначальная загрузка формы при редактировании
    function initialize() {

        if (model.RegionCode) {
            $scope.regionInfo = model.Region;
            return;
        }

        $scope.regionInfo = '-';
    }

    initialize();

    // Поиск бренда
    $scope.searchBrand = function (value) {

        $scope.topCount = null;
        $scope.isOther = false;
        $scope.topCount = null;

        $scope.loading = $http.post('/Brand/SearchBrand/', JSON.stringify({ Value: value }))
            .then(function (response) {
            return response.data;
            });


        return $scope.loading;
    };



    $scope.$watch(function () { return $scope.topCount; },
        function (newValue) {
            if (newValue == null)
                return;

            $scope.isOther = false;
            $scope.currentBrand = null;
        },
        true);

    $scope.$watch(function () { return $scope.isOther; },
        function (newValue) {
            if (!newValue)
                return;

            $scope.topCount = null;
            $scope.currentBrand = null;
            searchOther();
        },
        true);

   

    function searchOther() {
        $scope.currentBrand = null;
        $scope.classifierId = null;
        $scope.topCount = null;

        var searchDrugParameters = {
            ClassifierId:0,
            BrandId: 0,
            Year: model.Year,
            Month: model.Month,
            RegionCode: model.RegionCode
        };

        $scope.loading = $http.post('/CountRuleEditor/SearchDrug/', JSON.stringify(searchDrugParameters))
            .then(function (response) {
                $scope.grid.Options.data = response.data;
                $scope.grid.clearSelection();
            }, function () { $scope.message = 'Unexpected Error'; });
    }


    $scope.setClassifier = function (value) {
        $scope.currentBrand = null;
        $scope.topCount = null;
        $scope.isOther = false;

        var searchDrugParameters = {
            ClassifierId: $scope.classifierId,
            BrandId: 0,
            Year: model.Year,
            Month: model.Month,
            RegionCode: model.RegionCode
        };

        $scope.loading = $http.post('/CountRuleEditor/SearchDrug/', JSON.stringify(searchDrugParameters))
            .then(function (response) {
                $scope.grid.Options.data = response.data;
                $scope.grid.clearSelection();
            }, function () { $scope.message = 'Unexpected Error'; });
    }
  

    $scope.setBrand = function (value) {
        $scope.classifierId = null;

        //Если ничего не задано ищем прочее
        if (!value || value.Id === undefined) {
            value = { Id : 0 };
        }
        
        var searchDrugParameters = {
            ClassifierId: 0,
            BrandId: value.Id,
            Year: model.Year,
            Month: model.Month,
            RegionCode : model.RegionCode

        };

        $scope.loading = $http.post('/CountRuleEditor/SearchDrug/', JSON.stringify(searchDrugParameters))
            .then(function (response) {
                $scope.grid.Options.data = response.data;
                $scope.grid.clearSelection();
            }, function() { $scope.message = 'Unexpected Error'; });
    };

    // Грид с результатами поиска DrugId, OwnerTradeMarkId, PackerId
    $scope.grid = uiGridCustomService.createGridClass($scope, 'CountRuleEditor_SearchDrugViewGrid');
    $scope.grid.Options.multiSelect = false;
    $scope.grid.Options.modifierKeysToMultiSelect = false;
    $scope.grid.Options.rowTemplate = '<div class="ui-grid-cell" ng-class=" {selected : row.isSelected, \'repeated\': row.entity.IsInCountRules }" ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" ui-grid-cell></div>';

    // Колонки грида
    $scope.grid.Options.columnDefs = [
        { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Описание', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
        { name: 'Правообладатель', field: 'OwnerTradeMark', width: 100, filter: { condition: uiGridCustomService.condition } },
        { name: 'Упаковщик', field: 'Packer', width: 100, filter: { condition: uiGridCustomService.condition } },
        { name: 'SSum', field: 'SellingSumNDS', width: 100, cellFilter: formatConstants.FILTER_PRICE, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'PSum', field: 'PurchaseSumNDS', width: 100, cellFilter: formatConstants.FILTER_PRICE, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'SCount', field: 'SellingCount', width: 100, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'PCount', field: 'PurchaseCount', width: 100, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'SSPart', field: 'SellingSumNDSPart', width: 70, cellFilter: formatConstants.FILTER_PRICE, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'PSPart', field: 'PurchaseSumNDSPart', width: 70, cellFilter: formatConstants.FILTER_PRICE, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'SCPart', field: 'SellingCountPart', width: 70, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'PCPart', field: 'PurchaseCountPart', width: 70, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
    ];

  

    $scope.Select = function () {
        var items = $scope.grid.getSelectedItem();

        var item = {};

        if (items != null) {
            item = items[0];
        }
      

        var result = { ClassifierId: item.ClassifierId, TradeName: item.TradeName, topCount: $scope.topCount, DrugDescription: item.DrugDescription}

        $modalInstance.close(result);
    };

    $scope.canSelect = function () {
        var selectedItems = $scope.grid.getSelectedItem();

        return selectedItems && selectedItems.length > 0 || $scope.topCount > 0;
    };

    // Закрыть форму
    $scope.Close = function () {
        $modalInstance.dismiss();
    };

}