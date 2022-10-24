angular
    .module('DataAggregatorModule')
    .controller('HelpController', ['$scope', '$uibModalInstance', HelpController]);

function HelpController($scope, $modalInstance) {

    $scope.ok = function () {
        $modalInstance.close('ok');
    };

}