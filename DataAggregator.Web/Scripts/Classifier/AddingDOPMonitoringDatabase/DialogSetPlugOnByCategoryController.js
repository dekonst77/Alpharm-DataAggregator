angular
    .module('DataAggregatorModule')
    .controller('DialogSetPlugOnByCategoryController', ['$scope', '$uibModalInstance', 'PlugInfo', DialogSetPlugOnByCategoryController]);

function DialogSetPlugOnByCategoryController($scope, $modalInstance, PlugInfo) {

    // календарь --->>>
    $scope.today = function () {
        $scope.dt = new Date();
    };

    $scope.clear = function () {
        $scope.dt = null;
    };
    // календарь ---<<<

    $scope.CanClose = function () {
        return $scope.dt !== null;
    };

    $scope.ok = function () {
        $modalInstance.close($scope.dt);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('Отмена операции');
    };

    $scope.setDay = function (dateString) {
        $scope.dt = Date.parse(dateString);
    };
    
    $scope.PlugInfo = PlugInfo;
    $scope.dt = PlugInfo.EndDate ?? new Date(2099, 11, 31);

    console.log($scope.PlugInfo);
    console.log($scope.dt);
}