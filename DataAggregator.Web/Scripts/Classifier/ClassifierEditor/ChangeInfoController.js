angular
    .module('DataAggregatorModule')
    .controller('ChangeInfoController', ['$scope', '$uibModalInstance', 'classifierInfo', 'canCancel', ChangeInfoController]);

function ChangeInfoController($scope, $modalInstance, classifierInfo, canCancel) {

    if (canCancel) {
        $scope.headerText = 'Что будет изменено';
    } else {
        $scope.headerText = 'Что изменено';
    }

    $scope.info = classifierInfo;

    $scope.canCancel = canCancel;

    $scope.ok = function () {
        $modalInstance.close($scope.info);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss();
    };
}