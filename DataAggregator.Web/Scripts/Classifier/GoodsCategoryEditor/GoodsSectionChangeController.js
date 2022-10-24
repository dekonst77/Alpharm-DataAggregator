angular
    .module('DataAggregatorModule')
    .controller('GoodsSectionChangeController', ['$scope', '$http', '$uibModalInstance', 'dialogParams', GoodsSectionChangeController]);

function GoodsSectionChangeController($scope, $http, $modalInstance, dialogParams) {
    $scope.sectionsList = dialogParams.sectionsList;
    $scope.newSection = dialogParams.selectedSection;

    $scope.save = function () {
        if ($scope.newSection === dialogParams.selectedSection) {
            $modalInstance.dismiss('cancel');
        } else {
            $modalInstance.close($scope.newSection.Id);
        }
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };
}
