angular
    .module('DataAggregatorModule')
    .controller('RightClassifierFilterController', ['$scope', '$http', '$uibModalInstance', 'dialogData', RightClassifierFilterController]);

function RightClassifierFilterController($scope, $http, $modalInstance, dialogData) {
    $scope.dialogData = dialogData;

    $scope.ok = function () {
        $modalInstance.close($scope.dialogData);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}