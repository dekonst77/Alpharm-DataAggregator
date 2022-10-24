angular
    .module('DataAggregatorModule')
    .controller('CreateContractController', ['messageBoxService', 'lotId', '$uibModal', '$scope', '$http', '$uibModalInstance', CreateContractController]);


function CreateContractController(messageBoxService, lotId, $uibModal, $scope, $http, $modalInstance) {
    
    $scope.contract = {};

    // список статусов контракта
    $scope.contractStatusList = [];

    function contractStatusList() {
        $scope.createLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetContractStatusList/'
        }).then(function (response) {
            $scope.contractStatusList = response.data;
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    contractStatusList();
    
    function generateContract() {
        $scope.createLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GenerateContract/',
            data: JSON.stringify({ lotId: lotId })
        }).then(function (response) {
            $scope.contract = response.data;
            if (response.data.ConclusionDate != null)
                $scope.contract.ConclusionDate = parseISOAsUTC(response.data.ConclusionDate);
            if (response.data.DateBegin != null)
                $scope.contract.DateBegin = parseISOAsUTC(response.data.DateBegin);
            if (response.data.DateEnd != null)
                $scope.contract.DateEnd = parseISOAsUTC(response.data.DateEnd);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    generateContract();

    $scope.changeContractReceiver = function () {
        var modalInstance2 = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_SearchOrganizationView.html',
            controller: 'SearchOrganizationController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static',
            resolve: {
                Is_Customer: false, Is_Recipient: false
            }
        });

        modalInstance2.result.then(function (v) {
            $scope.contract.ReceiverId = v.Id;
            $scope.contract.Receiver = v.ShortName;
        }, function () {
        });
    }

    $scope.save = function () {
        
        $scope.createLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/AddContractInfo/',
            data: JSON.stringify({ contractJson: $scope.contract, lotId: lotId })
        }).then(function (result) {
            if (result.data) {
                $modalInstance.close(lotId);
            } else {
                messageBoxService.showError('Контракт с Реестровым номером '+$scope.contract.ReestrNumber+' и Номером лота '+lotId+' уже существует!');
                //$modalInstance.close();
            }
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });

    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.searchSupplier = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_SearchSupplierView.html',
            controller: 'SearchSupplierController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static'
        });

        modalInstance.result.then(function (selectedSupplier) {
            $scope.contract.Supplier = selectedSupplier;
        }, function () {
        });
    }

    function parseISOAsUTC(s) {
        var b = s.split(/\D/);
        return new Date(Date.UTC(b[0], --b[1], b[2], b[3], b[4], b[5], (b[6] || 0)));
    }

}