angular
    .module('DataAggregatorModule')
    .controller('FundingController', ['$scope', '$uibModalInstance', 'fundingList', 'sourceOfFinancing', FundingController]);

function FundingController($scope, $modalInstance, fundingList, sourceOfFinancing) {

    $scope.fundingList = fundingList;
    
    $scope.sourceOfFinancing = sourceOfFinancing;

    $scope.save = function () {
        $modalInstance.close($scope.fundingList);
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

}