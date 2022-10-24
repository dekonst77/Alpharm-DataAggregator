angular
    .module('DataAggregatorModule')
    .controller('OrganizationsFilterController', ['$scope', '$http', '$uibModalInstance', 'filter', OrganizationsFilterController]);

function OrganizationsFilterController($scope, $http, $modalInstance, filter) {

    $scope.filter = filter;

    $scope.clearAll = function () {
        $scope.filter.Id = null;
        $scope.filter.Inn = null;
        $scope.filter.OrganizationType = null;
        $scope.filter.FullName = null;
        $scope.filter.ShortName = null;
        $scope.filter.OnlyDrugsLinked = true;
        $scope.filter.OnlyEmptyType = false;
        $scope.filter.OnlyEmptyRegion = false;
        $scope.filter.is_LO = false;
        $scope.filter.is_CP = false;
        $scope.filter.is_Actual = false;
        $scope.filter.FederalDistrict.selectedItems = [];
        $scope.filter.FederalDistrict.displayValue = '';
        $scope.filter.FederationSubject.selectedItems = [];
        $scope.filter.FederationSubject.displayValue = '';
    }

    $scope.ok = function () {
        $modalInstance.close({
            filter: $scope.filter
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}