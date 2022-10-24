angular
    .module('DataAggregatorModule')
    .controller('GoodsChangeInfoController', ['$scope', '$http', '$uibModalInstance', 'classifierInfo', 'canCancel', GoodsChangeInfoController]);

function GoodsChangeInfoController($scope, $http, $modalInstance, classifierInfo, canCancel) {

    if (canCancel) {
        $scope.headerText = 'Что будет изменено';
    } else {
        $scope.headerText = 'Что изменено';
    }

    $scope.info = classifierInfo;

    $scope.canCancel = canCancel;

    $scope.ok = function () {
        $modalInstance.close();
    };

    $scope.cancel = function() {
        $modalInstance.dismiss();
    }
}