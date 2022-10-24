angular
    .module('DataAggregatorModule')
    .controller('DeleteGroupController', ['$scope', '$http', '$uibModalInstance', 'dialogParams', DeleteGroupController]);

function DeleteGroupController($scope, $http, $modalInstance, dialogParams) {
    $scope.idToDelete = dialogParams.Id;

    $scope.ok = function () {
        $modalInstance.close('ok');
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}
