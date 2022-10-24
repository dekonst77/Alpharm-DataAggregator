angular
    .module('DataAggregatorModule')
    .controller('DeleteTransferController', ['$scope', '$http', '$uibModalInstance', 'dialogParams', DeleteTransferController]);

function DeleteTransferController($scope, $http, $modalInstance, dialogParams) {
    $scope.idsToDelete = dialogParams.join(", ");

    $scope.ok = function () {
        $modalInstance.close('ok');
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}
