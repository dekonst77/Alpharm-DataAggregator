angular
    .module('DataAggregatorModule')
    .controller('SearchOrganizationController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'messageBoxService', 'Is_Customer','Is_Recipient', SearchOrganizationController]);


function SearchOrganizationController($scope, $http, $modalInstance, uiGridCustomService, messageBoxService, Is_Customer, Is_Recipient) {
    var filter = $scope.filter = {
        INN: '',
        Address: '',
        Name: '',
        Is_Customer: Is_Customer,
        Is_Recipient: Is_Recipient,
        canSearch : function() {
            return this.INN.length > 0 || this.Address.length > 0 || this.Name.length > 0;
        }
    };


    $scope.organizationGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_OrganizationGrid');

    $scope.organizationGrid.Options.columnDefs = [
      { name: 'Id', field: 'Id', enableHiding: false, type: 'number' },
      { name: 'Полное название', field: 'FullName' },
      { name: 'ИНН', field: 'INN', type: 'number'},
     // { name: 'КПП', field: 'KPP', type: 'number' },
       { name: 'Адрес', field: 'Address' },
       { name: 'Регион', field: 'Region' }
    ];


    $scope.organizationGrid.Options.multiSelect = false;
    $scope.organizationGrid.Options.noUnselect = true;
    $scope.organizationGrid.Options.modifierKeysToMultiSelect = false;
    $scope.organizationGrid.Options.showGridFooter = true;

    $scope.organization = null;

    $scope.organizationInfoString = null;

    $scope.search = function () {
        if (!filter.canSearch())
            return;

        $scope.searchLoading = 
        $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetOrganizationByFilter/',
            data: JSON.stringify({ filter: filter })
        }).then(function (response) {
            var data = response.data;
            if (data.length === 0) {
                messageBoxService.showInfo('Ничего не найдено', 'Поиск');
            };  
            $scope.organizationGrid.Options.data = data;
        },function () {
            $scope.message = 'Unexpected Error';
        });
    }


    $scope.canSave = function() {
        return $scope.organizationGrid.getSelectedItem() != null;
    }

    $scope.save = function () {
        $modalInstance.close($scope.organizationGrid.getSelectedItem()[0]);
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };
}