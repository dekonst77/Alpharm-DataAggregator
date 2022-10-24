angular
    .module('DataAggregatorModule')
    .controller('EditParameterViewController', ['$scope', '$http', '$uibModalInstance', 'item', EditParameterViewController]);

function EditParameterViewController($scope, $http, $modalInstance, item) {
    $scope.headerText = !item ? "Добавить" : "Редактировать";
    $scope.newItem = JSON.parse(JSON.stringify(item));

    $scope.save = function() {
        if ($scope.newItem && $scope.newItem.Value) {
            $modalInstance.close($scope.newItem);
        };
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
