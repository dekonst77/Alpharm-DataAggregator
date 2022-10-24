angular
    .module('DataAggregatorModule')
    .controller('AddGoodsClassifierInfoController', ['$scope', '$http', '$uibModalInstance', 'classifierInfo', 'canCancel', AddGoodsClassifierInfoController]);

function AddGoodsClassifierInfoController($scope, $http, $modalInstance, classifierInfo, canCancel) {

    if (canCancel) {
        $scope.headerText = 'Что будет добавлено';
    } else {
        $scope.headerText = 'Что добавлено';
    }

    $scope.classifierInfo = classifierInfo;

    $scope.canCancel = canCancel;

    $scope.ok = function () {
        $modalInstance.close($scope.classifierFilter);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss();
    }
}