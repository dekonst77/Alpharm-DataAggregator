angular
    .module('DataAggregatorModule')
    .controller('DialogSetPlugOffByCategoryController', ['$scope', '$uibModalInstance', 'PlugInfo', DialogSetPlugOffByCategoryController]);

function DialogSetPlugOffByCategoryController($scope, $modalInstance, PlugInfo) {

    // календарь
    $scope.today = function () {
        $scope.dt = new Date();
    };
    $scope.today();

    $scope.clear = function () {
        $scope.dt = null;
    };
    // календарь

    $scope.PlugInfo = PlugInfo;
    console.log($scope.PlugInfo);

    $scope.CanClose = function () {
        return $scope.dt !== null;
    };

    $scope.ok = function () {
        $modalInstance.close($scope.dt);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('Отмена операции');
    };
}