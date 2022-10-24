angular
    .module('DataAggregatorModule')
    .controller('CalculatedDataFilterController', ['$scope', '$uibModalInstance', 'dialogParams', CalculatedDataFilterController]);

function CalculatedDataFilterController($scope, $modalInstance, dialogParams) {
    $scope.filter = dialogParams.filter;

    $scope.dateEndIsMoreThanDateBegin = function() {
        return $scope.filter.PurchaseDateBeginEnd.Value !== null && $scope.filter.PurchaseDateBeginEnd.Value < $scope.filter.PurchaseDateBeginStart.Value;
    };

    $scope.clearFilter = function () {
        $scope.filter.ClassifierId = null;
        $scope.filter.PurchaseId = null;
        $scope.filter.DrugId = null;
        $scope.filter.ManufacturerList = null;
        $scope.filter.OwnerTradeMark = null;
        $scope.filter.OwnerTradeMarkId = null;
        $scope.filter.Packer = null;
        $scope.filter.PackerId = null;
        $scope.filter.PurchaseNumber = null;
        $scope.filter.INNGroup = null;
        $scope.filter.PurchaseDateBeginStart = new dateClass();
        $scope.filter.PurchaseDateBeginEnd = new dateClass();
        $scope.filter.DrugTradeName = null;
        $scope.filter.Category.selectedItems = [];
        $scope.filter.Category.displayValue = '';
        $scope.filter.Nature.selectedItems = [];
        $scope.filter.Nature.displayValue = '';
        $scope.filter.DrugDescription = null;
        $scope.filter.ObjectReadyName = null;
        $scope.filter.GoodsCategoryName = null;
        $scope.filter.FederalDistrict.selectedItems = [];
        $scope.filter.FederalDistrict.displayValue = '';
        $scope.filter.FederationSubject.selectedItems = [];
        $scope.filter.FederationSubject.displayValue = '';
        $scope.filter.includePurchases = true;
        $scope.filter.includeContracts = true;
        $scope.filter.VNC = false;
    };

    $scope.ok = function () {
        $modalInstance.close({
            filter: $scope.filter
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}