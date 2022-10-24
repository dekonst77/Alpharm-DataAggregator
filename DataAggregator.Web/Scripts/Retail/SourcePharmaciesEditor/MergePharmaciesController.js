angular
    .module('DataAggregatorModule')
    .controller('MergePharmaciesController', ['$scope', '$http', '$uibModalInstance', 'dialogParams', 'uiGridCustomService', MergePharmaciesController]);

function MergePharmaciesController($scope, $http, $modalInstance, dialogParams, uiGridCustomService) {
    $scope.pharmaciesToMerge = dialogParams;

    $scope.mergePharmaciesGrid = uiGridCustomService.createGridClass($scope, 'SourcePharmaciesEditor_MergePharmaciesGrid');

    $scope.mergePharmaciesGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Адрес', field: 'Address' },
        { name: 'Имена файлов', field: 'FileNames' }
    ];

    $scope.mergePharmaciesGrid.Options.multiSelect = false;
    $scope.mergePharmaciesGrid.Options.enableFiltering = false;

    $scope.mergePharmaciesGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.mergePharmaciesGridApi = gridApi;
    };

    $scope.mergePharmaciesGrid.Options.data = $scope.pharmaciesToMerge;

    $scope.canCommitMerge = function() {
        var selectedPharmacies = $scope.mergePharmaciesGridApi.selection.getSelectedRows();
        return selectedPharmacies.length != 1;
    };

    $scope.ok = function () {
        var selectedPharmacies = $scope.mergePharmaciesGridApi.selection.getSelectedRows();
        $modalInstance.close(selectedPharmacies[0].Id);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}
