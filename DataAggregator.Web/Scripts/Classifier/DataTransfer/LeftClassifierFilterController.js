angular
    .module('DataAggregatorModule')
    .controller('LeftClassifierFilterController', ['$scope', '$http', '$uibModalInstance', 'dialogData', LeftClassifierFilterController]);

function LeftClassifierFilterController($scope, $http, $modalInstance, dialogData) {
    $scope.dialogData = dialogData;

    $scope.ok = function () {
        $modalInstance.close($scope.dialogData);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}