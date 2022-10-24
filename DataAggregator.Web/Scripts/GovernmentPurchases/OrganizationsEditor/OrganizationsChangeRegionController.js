angular
    .module('DataAggregatorModule')
    .controller('OrganizationsChangeRegionController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'data', OrganizationsChangeRegionController]);

function OrganizationsChangeRegionController($scope, $http, $modalInstance, uiGridCustomService, data) {

    $scope.headerText = data.headerText;

    $scope.regionGrid = uiGridCustomService.createGridClass($scope, 'regionGrid');
    
    $scope.regionGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Федеральный округ', field: 'FederalDistrict' },
        { name: 'Субъект федерации', field: 'FederationSubject' },
        { name: 'Район', field: 'District' },
        { name: 'Город', field: 'City' },
        { name: 'Код', field: 'Code' }
    ];
    $scope.regionGrid.Options.multiSelect = false;
    $scope.regionGrid.Options.noUnselect = false;
    $scope.regionGrid.Options.data = data.gridData;
    $scope.regionGrid.Options.showGridFooter = true;
    $scope.regionGrid.Options.excessRows = 20;//иначе почему-то при открытии модального окна рендерятся только первые 4 строчки

    $scope.regionGrid.Options.onRegisterApi = function (gridApi) {

        $scope.regionGridApi = gridApi;

        $scope.regionGridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedRows = $scope.regionGridApi.selection.getSelectedRows().map(function (value) {
                return value;
            });
        });
    }

    $scope.ok = function () {

        $modalInstance.close({
            RegionId: $scope.selectedRows[0].Id, FederalDistrict:$scope.selectedRows[0].FederalDistrict, FederationSubject:$scope.selectedRows[0].FederationSubject
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}