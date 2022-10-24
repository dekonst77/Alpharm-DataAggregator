angular
    .module('DataAggregatorModule')
    .controller('PurchaseLinkAddController', ['messageBoxService', '$scope', '$uibModalInstance', 'dialogParams', PurchaseLinkAddController]);

function PurchaseLinkAddController(messageBoxService, $scope, $modalInstance, dialogParams) {
    var editParameters = dialogParams.editParameters;

    $scope.objectType = editParameters.objectType;
    $scope.lawTypesList = editParameters.lawTypesList;

    $scope.save = function () {
        if (purchaseUrlIsCorrect()) {
            $modalInstance.close({
                purchaseNumber: $scope.purchaseNumber,
                lawTypeId: $scope.lawTypeId,
                reestrNumber: $scope.reestrNumber,
                purchaseUrl: $scope.purchaseUrl
            });
        } else {
            messageBoxService.showError('Ссылка не содержит адрес сайта госзакупок (https://zakupki.gov.ru/)!');
            return;
        }
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

    function purchaseUrlIsCorrect() {
        return $scope.objectType === 'Contracts' || $scope.purchaseUrl.startsWith('https://zakupki.gov.ru/');
    }
}



