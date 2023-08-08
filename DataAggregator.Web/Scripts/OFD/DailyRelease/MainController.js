angular
    .module('DataAggregatorModule')
    .controller('MainController', ['$scope', '$http', 'uiGridCustomService', 'messageBoxService', 'errorHandlerService', 'formatConstants', '$uibModal', MainController]);

function MainController($scope, $http, uiGridCustomService, messageBoxService, errorHandlerService, formatConstants, $uibModal) {
    $scope.currentTabIndex = 0;
    $scope.SupplierList = [];

    $scope.setTabIndex = function (index) {
        $scope.currentTabIndex = index;
    };

    $scope.Init = function () {
        $scope.loading =
            $http({
                method: 'POST',
                url: '/OFD/Suppliers/',
                data: JSON.stringify({ withDef: true })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.SupplierList = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.Init();

   
}