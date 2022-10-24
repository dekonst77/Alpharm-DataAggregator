angular
    .module('DataAggregatorModule')
    .controller('GoodsCategorySelectorController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', GoodsCategorySelectorController]);

function GoodsCategorySelectorController($scope, $http, $modalInstance, uiGridCustomService) {

    $scope.goodsCategories = [];
    $scope.categoriesGridApi = undefined;

    $scope.categoriesGridOptions = uiGridCustomService.createOptions('GoodsSystematization_CategoriesGrid');

    var categoriesGridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableSelectAll: false,
        enableFullRowSelection: true,
        multiSelect: false,
        showGridFooter: false,
        excessRows: 100,
        appScopeProvider: $scope,
        columnDefs: [
            { name: 'Id', field: 'Id', width: 50, type: 'number' },
            { name: 'Раздел', field: 'SectionName', width: 150 },
            { name: 'Категория', field: 'Name' }
        ],
        rowTemplate: '_rowCategoryTemplate.html'
    };

    angular.extend($scope.categoriesGridOptions, categoriesGridOptions);

    $scope.categoriesGridOptions.onRegisterApi = function (gridApi) {
        $scope.categoriesGridApi = gridApi;
    };

    $scope.canChangeCategory = function () {
        return $scope.categoriesGridApi ? $scope.categoriesGridApi.selection.getSelectedRows().length > 0 : false;
    };

    $scope.ok = function () {
        var selectedCategory = $scope.categoriesGridApi.selection.getSelectedRows()[0];
        $modalInstance.close(selectedCategory);
    };

    $scope.onDblClick = function (selectedCategory) {
        $modalInstance.close(selectedCategory);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    (function() {
        $scope.categoriesLoading = $http.post('/GoodsSystematization/LoadGoodsCategories/')
            .then(function (response) {
                var data = response.data;
                $scope.goodsCategories = data;
                $scope.categoriesGridOptions.data = data;
            }, function () {
                $scope.message = 'Unexpected Error';
            });
    })();
}