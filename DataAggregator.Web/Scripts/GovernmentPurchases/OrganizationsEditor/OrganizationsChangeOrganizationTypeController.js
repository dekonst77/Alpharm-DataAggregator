angular
    .module('DataAggregatorModule')
    .controller('OrganizationsChangeOrganizationTypeController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'gridData', OrganizationsChangeOrganizationTypeController]);

function OrganizationsChangeOrganizationTypeController($scope, $http, $modalInstance, uiGridCustomService, gridData) {

    $scope.organizationTypeGrid = uiGridCustomService.createGridClass($scope, 'organizationTypeGrid');
    
    $scope.organizationTypeGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Наименование', field: 'Name' }
    ];
    $scope.organizationTypeGrid.Options.multiSelect = false;
    $scope.organizationTypeGrid.Options.noUnselect = false;
    $scope.organizationTypeGrid.Options.data = gridData;
    $scope.organizationTypeGrid.Options.showGridFooter = true;
    //$scope.organizationTypeGrid.Options.excessRows = 20;//иначе почему-то при открытии модального окна рендерятся только первые 4 строчки

    $scope.organizationTypeGrid.Options.onRegisterApi = function (gridApi) {

        $scope.organizationTypeGridApi = gridApi;

        $scope.organizationTypeGridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedRows = $scope.organizationTypeGridApi.selection.getSelectedRows().map(function (value) {
                return value;
            });
        });
    }

    $scope.ok = function() {
        $modalInstance.close({
            OrganizationTypeId: $scope.selectedRows[0].Id, OrganizationTypeText: $scope.selectedRows[0].Name
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}