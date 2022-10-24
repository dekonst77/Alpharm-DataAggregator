angular
    .module('DataAggregatorModule')
    .controller('SearchSupplierController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'messageBoxService', SearchSupplierController]);


function SearchSupplierController($scope, $http, $modalInstance, uiGridCustomService, messageBoxService) {
    var filter = $scope.filter = {
        Name: '',
        Id: '',
        INN: '',
        KPP: '',
        LocationAddress:'',
        canSearch : function() {
            return this.Name.length > 0 ||
                this.Id.length > 0 ||
                this.INN.length > 0 ||
                this.KPP.length > 0 ||
                this.LocationAddress.length > 0;
        }
    };
    $scope.supplierGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_SupplierGrid');

    $scope.supplierGrid.Options.columnDefs = [
      { name: 'Id', field: 'Id', enableHiding: false, type: 'number' },
      { name: 'Название', field: 'Name' },
      { name: 'ИНН', field: 'INN', type: 'number'},
      { name: 'КПП', field: 'KPP', type: 'number' },
      { name: 'Адрес', field: 'LocationAddress' }
    ];


    $scope.supplierGrid.Options.multiSelect = false;
    $scope.supplierGrid.Options.noUnselect = true;
    $scope.supplierGrid.Options.modifierKeysToMultiSelect = false;
    $scope.supplierGrid.Options.showGridFooter = true;
  

    $scope.search = function () {
        if (!filter.canSearch())
            return;

        $scope.searchLoading =
        $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetSupplierByFilter/',
            data: JSON.stringify({ filter: filter })
        }).then(function (response) {
            var data = response.data;
            if (data.length === 0) {
                messageBoxService.showInfo('Ничего не найдено', 'Поиск');
            };  
            $scope.supplierGrid.Options.data = data;
        },function () {
            $scope.message = 'Unexpected Error';
        });
    }


    $scope.canSave = function() {
        return $scope.supplierGrid.getSelectedItem() != null;
    }

    $scope.save = function () {
        $modalInstance.close($scope.supplierGrid.getSelectedItem()[0]);
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

   

}