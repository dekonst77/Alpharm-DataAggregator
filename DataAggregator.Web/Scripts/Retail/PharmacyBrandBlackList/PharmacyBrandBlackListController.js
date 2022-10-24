angular
    .module('DataAggregatorModule')
    .controller('PharmacyBrandBlackListController', ['$scope', '$http', '$uibModal', '$window', 'uiGridCustomService', PharmacyBrandBlackListController]);

function PharmacyBrandBlackListController($scope, $http, $uibModal, $window, uiGridCustomService) {

    $scope.UploadFromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('file', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/PharmacyBrandBlackList/UploadFromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function () {
                $scope.getBlackList();
            }, function () {
                alert('Ошибка при импортировании файла');
            });
        }
    };
    $scope.exportToExcel = function () {
        $window.location.href = '/PharmacyBrandBlackList/ExportToExcel';
    }

    $scope.blackListGrid = uiGridCustomService.createGridClass($scope, 'PharmacyBrandBlackList_BlackListGrid');

    $scope.blackListGrid.Options.columnDefs = [
        { name: 'Id аптеки', field: 'TargetPharmacyId', type: 'number' },
        { name: 'Id бренда', field: 'BrandId', type: 'number' },
        { name: 'Бренд', field: 'Brand' }
    ];

    $scope.blackListGrid.Options.multiSelect = true;
    $scope.blackListGrid.Options.noUnselect = false;
    $scope.blackListGrid.Options.modifierKeysToMultiSelect = true;
    $scope.blackListGrid.Options.showGridFooter = true;

    $scope.blackListGrid.Options.onRegisterApi = function (gridApi) {
        $scope.blackListGridApi = gridApi;
    };

    $scope.getBlackList = function() {
        $scope.loading = $http({
            method: 'POST',
            url: '/PharmacyBrandBlackList/GetBlackList'
        }).then(function (response) {
            $scope.blackListGrid.Options.data = response.data;
        }, function () {
            $scope.blackListGrid.Options.data = [];
        });
    }

    $scope.openDeleteDialog = function () {
        var modalDeleteDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/PharmacyBrandBlackList/_DeletePositionsView.html',
            controller: 'DeletePositionsController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalDeleteDialog.result.then(
            // ok
            function () {
                $scope.deletePositions();
            },
            // cancel
            function () {
            }
        );
    }

    $scope.deletePositions = function() {
        var positionsToDelete = $scope.blackListGridApi.selection.getSelectedRows().map(function (value) {
            return { TargetPharmacyId: value.TargetPharmacyId, BrandId: value.BrandId };
        });

        $scope.loading = $http({
            method: 'POST',
            url: '/PharmacyBrandBlackList/DeletePositions',
            data: JSON.stringify(positionsToDelete)
        }).then(function () {
            $scope.getBlackList();
        }, function () {
            alert('Ошибка!');
        });
    }

    $scope.getBlackList();
}