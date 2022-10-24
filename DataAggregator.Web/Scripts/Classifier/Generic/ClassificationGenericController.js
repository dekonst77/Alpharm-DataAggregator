angular
    .module('DataAggregatorModule')
    .controller('ClassificationGenericController', ['$scope', '$http', 'uiGridCustomService', ClassificationGenericController]);

function ClassificationGenericController($scope, $http, uiGridCustomService) {
    
    //Грид
    $scope.genericGrid = uiGridCustomService.createGridClass($scope, 'Generic_Grid');
    $scope.genericGrid.Options.showGridFooter = true;
    $scope.genericGrid.Options.enableSorting = true;
    $scope.genericGrid.Options.multiSelect = true;
    $scope.genericGrid.Options.modifierKeysToMultiSelect = true;
    $scope.genericGrid.Options.noUnselect = true;
    $scope.genericGrid.Options.columnDefs =
        [
            { displayName: 'TRADE_NAME', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
            { displayName: 'INN_GROUP', field: 'InnGroup',  filter: { condition: uiGridCustomService.conditionSpace } },
            { displayName: 'OWNER_TRADE_MARK', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
            { displayName: 'GENERIC', field: 'Generic', filter: { condition: uiGridCustomService.condition } },
            { displayName: 'COMMON.USER', field: 'User', filter: { condition: uiGridCustomService.condition } },
            { displayName: 'COMMON_GRID.EDIT_DATE', field: 'DateEdit', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'yyyy-MM-dd HH:mm\'' },

        ];

    var selectedRows = null;

    //События изменения грида
    $scope.genericGrid.Options.onRegisterApi = function (gridApi) {
        $scope.genericGridApi = gridApi;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            selectedRows = $scope.genericGridApi.selection.getSelectedRows();
        });

        //Что-то выделили
        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            selectedRows = $scope.genericGridApi.selection.getSelectedRows();
        });
    };
    
    $scope.genericList = null;
    $scope.generic = null;

    //Методы

    //Загрузка 
    function loadGeneric() {
        $scope.loading = $http({
            method: "POST",
            url: "/ClassificationGeneric/LoadGeneric"
        }).then(function (response) {
            $scope.genericList = response.data;
        }, function () {
                $scope.genericList = [];
        });
    }

    //Загрузка данных по фильтру
    function loadData() {
        $scope.loading = $http({
            method: "POST",
            url: "/ClassificationGeneric/LoadData/"
        }).then(function (response) {
            $scope.genericGrid.Options.data = response.data;
        }, function () {
            $scope.genericGrid.Options.data = [];
        });
    }

    $scope.canSetGeneric = function() {
        return selectedRows.length > 0 && $scope.generic !== null && $scope.generic.Id > 0;
    }

    //Установить класс
    $scope.setGeneric = function () {
        var ids = selectedRows.map(function (r) { return r.Id });
        var datajson = JSON.stringify({ ids: ids, genericId: $scope.generic.Id });

        $scope.loading = $http({
            method: "POST",
            data: datajson,
            url: "/ClassificationGeneric/SetGeneric/"
        }).then(function (response) {
            //Меняем показатели
            selectedRows.forEach(function (item, i, arr) {
                var currentId = item.Id;
                $scope.genericGrid.Options.data.forEach(function (item, i, arr) {
                    if (item.Id == currentId) {
                        item.Generic = $scope.generic.Value;
                        item.User = response.data.UserFullName;
                        item.DateEdit = response.data.DateUpdate;
                    }
                });
            });
        }, function () {
        });
    }

    //Инициализация

    loadGeneric();
    loadData();

}