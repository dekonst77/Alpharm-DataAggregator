angular
    .module('DataAggregatorModule')
    .controller('GoodsFilterController', ['$scope', '$http', '$uibModalInstance', 'hotkeys', 'uiGridCustomService', GoodsFilterController]);

function GoodsFilterController($scope, $http, $modalInstance, hotkeys, uiGridCustomService) {

    $scope.isCollapsed = true;
    $scope.additionalFilter = {};

    $scope.categoryStatGridOptions = uiGridCustomService.createOptions('GoodsSystematization_CategoryStatGrid');

    var categoryStatGridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: true,
        enableSelectAll: true,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableSelectionBatchEvent: true,
        enableHighlighting: true,
        multiSelect: true,
        noUnselect: false,
        showGridFooter: false,
        columnDefs: [
                        { name: 'Id категории', field: 'CategoryId', width: 50, filter: { condition: uiGridCustomService.condition } },
                        { name: 'Раздел', field: 'SectionName', width: 130, filter: { condition: uiGridCustomService.condition } },
                        { name: 'Категория', field: 'CategoryName', width: 130, filter: { condition: uiGridCustomService.condition } },
                        { name: 'В работу', field: 'ForWorkCount', width: 120, type: 'number', filter: { condition: uiGridCustomService.condition } },
                        { name: 'В работе', field: 'InWorkCount', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Готово', field: 'IsReadyCount', filter: { condition: uiGridCustomService.condition } }
        ],
        rowTemplate: '_rowGoodsFilterView.html',
        excessRows: 100
    };

    angular.extend($scope.categoryStatGridOptions, categoryStatGridOptions);

    $scope.categoryStatGridOptions.onRegisterApi = function (gridApi) {
        $scope.categoryStatGridApi = gridApi;
    };

    $scope.userStatGridOptions = uiGridCustomService.createOptions('GoodsSystematization_UserStatGrid');

    var userStatGridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: true,
        enableSelectAll: true,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableSelectionBatchEvent: true,
        enableHighlighting: true,
        multiSelect: true,
        noUnselect: false,
        showGridFooter: false,
        columnDefs: [
                        { name: 'Пользователь', field: 'UserName', width: 430, filter: { condition: uiGridCustomService.condition } },
                        { name: 'В работе', field: 'InWorkCount', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Готово', field: 'IsReadyCount', filter: { condition: uiGridCustomService.condition } }],
        excessRows: 100
    };

    angular.extend($scope.userStatGridOptions, userStatGridOptions);

    $scope.userStatGridOptions.onRegisterApi = function (gridApi) {
        $scope.userStatGridApi = gridApi;
    };

    hotkeys.bindTo($scope).add({
        combo: 'enter',
        description: 'Поиск',
        callback: function () {
            $scope.checkOk();
        }
    });

    $scope.canSearch = function() {
        if (!$scope.filter) {
            return false;
        }

        return $scope.categoryStatGridApi.selection.getSelectedGridRows().length > 0;
    };

    $scope.ok = function () {
        var forWorkCategoryIds = $scope.categoryStatGridApi.selection.getSelectedGridRows()
            .filter(function (obj) { return !obj.entity.ForAdding; })
            .map(function (obj) { return obj.entity.CategoryId });

        var forAddingCategoryIds = $scope.categoryStatGridApi.selection.getSelectedGridRows()
            .filter(function (obj) { return obj.entity.ForAdding; })
            .map(function (obj) { return obj.entity.CategoryId });

        var userGuids = $scope.userStatGridApi.selection.getSelectedGridRows().map(function (value) { return value.entity.UserId });

        var filterResult = {
            count: $scope.filter.Count,
            forWorkCategoryIds: forWorkCategoryIds,
            forAddingCategoryIds: forAddingCategoryIds,
            userGuids: userGuids,
            additional: $scope.additionalFilter
        };

        $modalInstance.close(filterResult);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

    function loadGoodsFilterStatistic() {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/GoodsSystematization/GetGoodsFilterStatistic/'
        }).then(function (response) {
            $scope.filter = response.data;

            $scope.categoryStatGridOptions.data = $scope.filter.CategoryStat;
            $scope.userStatGridOptions.data = $scope.filter.UserStat;

        }, function () {
            $scope.message = 'Unexpected Error';
        });
    }

    $scope.collapse = function() {
        $scope.isCollapsed = !$scope.isCollapsed;
    };

    $scope.сheckOk = function() {
        if ($scope.canSearch())
            $scope.ok();
    };

    $scope.update = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/GoodsSystematization/UpdateStatistic/'
        }).then(function (response) {
            $scope.filter = response.data;

            $scope.categoryStatGridOptions.data = $scope.filter.CategoryStat;
            $scope.userStatGridOptions.data = $scope.filter.UserStat;
        }, function () {
            $scope.message = 'Unexpected Error';
        });
    };

    loadGoodsFilterStatistic();
}