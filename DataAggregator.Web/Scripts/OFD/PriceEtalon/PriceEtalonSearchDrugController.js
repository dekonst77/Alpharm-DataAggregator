angular
    .module('DataAggregatorModule')
    .controller('PriceEtalonSearchDrugController', ['$scope', '$http', '$uibModalInstance',  'uiGridCustomService', 'formatConstants', '$translate', PriceEtalonSearchDrugController]);

function PriceEtalonSearchDrugController($scope, $http, $modalInstance,  uiGridCustomService, formatConstants, $translate) {

    


    $scope.setClassifier = function (value) {
        $scope.items = null;

        $scope.decription = null;

        $scope.loading = $http.post('/PriceEtalon/SearchDrug/', JSON.stringify({ Description: null, ClassifierId: $scope.classifierId }))
            .then(function (response) {
                $scope.grid.Options.data = response.data;
                $scope.grid.clearSelection();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.setDescription = function (value) { $scope.items = null;

        $scope.classifierId = null;

        $scope.loading = $http.post('/PriceEtalon/SearchDrug/', JSON.stringify({ Description: $scope.description, ClassifierId:null }))
            .then(function (response) {
                $scope.grid.Options.data = response.data;
                $scope.grid.clearSelection();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

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