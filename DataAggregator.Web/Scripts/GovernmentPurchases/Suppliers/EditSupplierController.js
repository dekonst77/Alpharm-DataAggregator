angular
    .module('DataAggregatorModule')
    .controller('EditSupplierController', ['$scope', '$http', '$uibModalInstance', 'editSupplierParameters', EditSupplierController]);

function EditSupplierController($scope, $http, $modalInstance, editSupplierParameters) {

    $scope.header = editSupplierParameters.header;
    $scope.selectedSupplier = editSupplierParameters.selectedSupplier;

    $scope.save = function () {
        //add
        if ($scope.selectedSupplier.Id == undefined) {
            $http({
                method: 'POST',
                url: '/Suppliers/AddSupplier/',
                data: JSON.stringify({ supplier: $scope.selectedSupplier })
            }).then(function (response) {
                $scope.selectedSupplier.Id = response.data;
                $modalInstance.close({
                    supplier: $scope.selectedSupplier
                });
            }, function () {
                alert('Ошибка');
                $scope.message = 'Unexpected Error';
            });
        }
        // edit
        else
        {
            $http({
                method: 'POST',
                url: '/Suppliers/EditSupplier/',
                data: JSON.stringify({ supplier: $scope.selectedSupplier })
            }).then(function () {
                $modalInstance.close({
                    supplier: $scope.selectedSupplier
                });
            }, function () {
                alert('Ошибка');
                $scope.message = 'Unexpected Error';
            });
        }
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

}