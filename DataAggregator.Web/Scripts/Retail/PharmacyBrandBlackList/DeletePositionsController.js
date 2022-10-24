angular
    .module('DataAggregatorModule')
    .controller('DeletePositionsController', ['$scope', '$http', '$uibModalInstance', DeletePharmaciesController]);

function DeletePharmaciesController($scope, $http, $modalInstance) {
    $scope.ok = function () {
        $modalInstance.close('ok');
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}
