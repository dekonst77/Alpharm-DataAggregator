angular
    .module('DataAggregatorModule')
    .controller('LPUMergeController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'gridData', LPUMergeController]);

function LPUMergeController($scope, $http, $modalInstance, uiGridCustomService, gridData) {

    $scope.LPUToMergeGrid = uiGridCustomService.createGridClass($scope, 'LPUToMergeGrid');



    $scope.LPUToMergeGrid.Options.columnDefs = [

        { name: 'LPUId', field: 'LPUId', type: 'number' },
        { name: 'Отделения', field: 'DepartmentCnt', type: 'number' },
        { name: 'OrganizationId', field: 'OrganizationId', type: 'number' },
        { name: 'PointId', field: 'PointId', type: 'number' },
        { name: 'ИНН юр. лица', field: 'EntityINN' },
        { name: 'ОГРН юр. лица', field: 'EntityOGRN' },
        { name: 'Юр. лицо', field: 'EntityName' },
        { name: 'Адрес из лицензии', field: 'Address' },
        { name: 'Брик', field: 'BricksId' },
        { name: 'Дата Добавления', field: 'Date_Create' }
    ];
    $scope.LPUToMergeGrid.Options.multiSelect = false;
    $scope.LPUToMergeGrid.Options.noUnselect = false;
    $scope.LPUToMergeGrid.Options.data = gridData;

    $scope.LPUToMergeGrid.Options.onRegisterApi = function (gridApi) {

        $scope.LPUToMergeGridApi = gridApi;

        //выделили 1 строку
        $scope.LPUToMergeGridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedRows = $scope.LPUToMergeGridApi.selection.getSelectedRows().map(function (value) {
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