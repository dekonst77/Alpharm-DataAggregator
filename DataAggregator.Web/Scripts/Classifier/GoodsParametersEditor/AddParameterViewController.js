angular
    .module('DataAggregatorModule')
    .controller('AddParameterViewController', ['$scope', '$http', '$uibModalInstance', AddParameterViewController]);

function AddParameterViewController($scope, $http, $modalInstance) {
    $scope.headerText = "Добавить";
    $scope.test = function() {
        console.log($scope.newValue);
    }
    $scope.save = function() {
        if ($scope.newValue) {
            $modalInstance.close($scope.newValue);
        };
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
