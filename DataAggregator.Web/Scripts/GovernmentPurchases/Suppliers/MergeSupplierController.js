angular
    .module('DataAggregatorModule')
    .controller('MergeSupplierController', ['$scope', '$uibModalInstance', 'mergeSupplierParameters', MergeSupplierController]);

function MergeSupplierController($scope, $modalInstance, mergeSupplierParameters) {

    $scope.supplierIdList = mergeSupplierParameters.supplierIdList;
    $scope.resultSupplierId = $scope.supplierIdList[0];

    $scope.save = function () {
        $modalInstance.close({
            resultSupplierId: $scope.resultSupplierId
        });
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

}