angular
    .module('DataAggregatorModule')
    .controller('DeletePharmaciesController', ['$scope', '$http', '$uibModalInstance', 'dialogParams', DeletePharmaciesController]);

function DeletePharmaciesController($scope, $http, $modalInstance, dialogParams) {
    $scope.idsToDelete = dialogParams.map(function(value) {
        return value.Id;
    }).join(", ");

    $scope.ok = function () {
        $modalInstance.close('ok');
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}
