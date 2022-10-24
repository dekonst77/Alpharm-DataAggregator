angular
    .module('DataAggregatorModule')
    .controller('OrganizationsMergeController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'gridData', OrganizationsMergeController]);

function OrganizationsMergeController($scope, $http, $modalInstance, uiGridCustomService, gridData) {

    $scope.organizationsToMergeGrid = uiGridCustomService.createGridClass($scope, 'organizationsToMergeGrid');

  

    $scope.organizationsToMergeGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'GosZakId', field: 'GosZakId', type: 'number' },
        { name: 'Полное наименование', field: 'FullName' },
        { name: 'Наименование', field: 'ShortName' },
        { name: 'ОГРН', field: 'OGRN' },
        { name: 'ИНН', field: 'INN' },
        { name: 'КПП', field: 'KPP' }
    ];
    $scope.organizationsToMergeGrid.Options.multiSelect = false;
    $scope.organizationsToMergeGrid.Options.noUnselect = false;
    $scope.organizationsToMergeGrid.Options.data = gridData;

    $scope.organizationsToMergeGrid.Options.onRegisterApi = function (gridApi) {

        $scope.organizationsToMergeGridApi = gridApi;

        //выделили 1 строку
        $scope.organizationsToMergeGridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedRows = $scope.organizationsToMergeGridApi.selection.getSelectedRows().map(function (value) {
                return value;
            });
        });
    }

    $scope.ok = function () {
        $modalInstance.close({
            ActualId: $scope.selectedRows[0].Id
    });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}