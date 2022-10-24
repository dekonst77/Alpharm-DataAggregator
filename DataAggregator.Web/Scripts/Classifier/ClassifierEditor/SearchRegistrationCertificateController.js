angular
    .module('DataAggregatorModule')
    .controller('SearchRegistrationController', ['$scope', '$http', '$uibModalInstance', 'messageBoxService', 'regNumber', 'uiGridCustomService', 'formatConstants', SearchRegistrationController]);

function SearchRegistrationController($scope, $http, $modalInstance, messageBoxService, regNumber, uiGridCustomService, formatConstants) {

    $scope.filter = {
        Number: regNumber
    };

    $scope.regCertGrid = uiGridCustomService.createGridClass($scope, 'ClassifierEditor_RegCertGrid');

    $scope.regCertGrid.Options.columnDefs = [
        { name: 'Номер', field: 'Number', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата регистрации', type: 'date', cellFilter: formatConstants.FILTER_DATE, field: 'RegistrationDate', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата окончания', type: 'date', cellFilter: formatConstants.FILTER_DATE, field: 'ExpDate', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата перерег.', type: 'date', cellFilter: formatConstants.FILTER_DATE, field: 'ReissueDate', filter: { condition: uiGridCustomService.condition } },
        { name: 'Срок введ. в ГО', field: 'CirculationPeriod.Value', filter: { condition: uiGridCustomService.condition } },
        { name: 'Владелец Ру', field: 'OwnerRegistrationCertificate.Value', filter: { condition: uiGridCustomService.condition } }
    ];

    $scope.regCertGrid.Options.multiSelect = false;
    $scope.regCertGrid.Options.modifierKeysToMultiSelect = false;


    $scope.search = function() {

        var json = JSON.stringify({ filter: $scope.filter });

        $scope.loading =
            $http({
                method: "POST",
                url: "/ClassifierEditor/SearchRegistrationCertificate/",
                data: json
            }).then(function(response) {
                    $scope.regCertGrid.Options.data = response.data;

                },
                function() {

                    $scope.message = "Unexpected Error";
                    messageBoxService.showError("Произошла ошибка");
                });
    };

    $scope.canOk = function() {
        return $scope.regCertGrid.getSelectedItem() == null || $scope.regCertGrid.getSelectedItem().length == 0;
    };

    $scope.ok = function () {
        $modalInstance.close($scope.regCertGrid.getSelectedItem()[0]);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss();
    };
}
