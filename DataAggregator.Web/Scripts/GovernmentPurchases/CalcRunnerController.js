angular
    .module('DataAggregatorModule')
    .controller('CalcRunnerController', ['$scope', '$http', '$interval', 'messageBoxService', CalcRunnerController]);

function CalcRunnerController($scope, $http, $interval, messageBoxService) {

    $scope.calcStatus = {};
    $scope.CreateExternalShipmentDatabase_startNight_value = "";
    $scope.GovernmentPurchases_Start_Full = "";
    $scope.GovernmentPurchases_CreateExternalShipmentDatabase = "";

    $scope.updateCalcStatus = function () {
        //$scope.loading = $http({
        //    method: 'POST',
        //    url: '/CalcRunner/GetCalcStatus'
        //}).then(function (response) {
        //    $scope.calcStatus = response.data;
        //}, function () {
        //    $scope.calcStatus = {};

        //});


        $scope.loading = $http({
            method: 'POST',
            url: '/CalcRunner/GetExternalGovernmentPurchasesStatus'
        }).then(function (response) {
            var data = response.data;
            $scope.TransferData = data.Step + ':' + data.Date;
        }, function () {
            $scope.TransferData = {};

        });


        $scope.loading = $http({
            method: 'POST',
            url: '/CalcRunner/GetExternalGovernmentPurchasesShipmentStatus'
        }).then(function (response) {
            var data = response.data;
            $scope.ShipmentStatusLog = data.Step + ':' + data.Date;
            $scope.ShipmentCreated = data.Created;
            $scope.ShipmentChecked = data.Checked;
        }, function () {
            $scope.ShipmentStatusLog = {};
            $scope.ShipmentCreated = null;
        });


        $scope.loading = $http({
            method: 'POST',
            url: '/CalcRunner/GetStatuses'
        }).then(function (response) {
            $scope.CreateExternalShipmentDatabase_startNight_value = response.data.CreateExternalShipmentDatabase_startNight_value;
            $scope.GovernmentPurchases_Start_Full = response.data.GovernmentPurchases_Start_Full;
            $scope.GovernmentPurchases_CreateExternalShipmentDatabase = response.data.GovernmentPurchases_CreateExternalShipmentDatabase;
        }, function () {
            $scope.CreateExternalShipmentDatabase_startNight_value = "";
            $scope.GovernmentPurchases_Start_Full = "";
            $scope.GovernmentPurchases_CreateExternalShipmentDatabase = "";
        });
    }

    $scope.SetChecked = function (value) {

        var json = JSON.stringify({ value: value });

        $http({
            method: 'POST',
            url: '/CalcRunner/SetChecked',
            data: json
        }).then(function (response) {
            $scope.ShipmentChecked = response.data;
        }, function () {
        });
    };

    $scope.calcAveragePrice = function () {
        $http({
            method: 'POST',
            url: '/CalcRunner/CalcAveragePrice'
        }).then(function () {
            $scope.updateCalcStatus();
        }, function () {
            $scope.updateCalcStatus();
        });
    };

    $scope.copyToCalculatedContractObject = function () {
        $http({
            method: 'POST',
            url: '/CalcRunner/CopyToCalculatedContractObject'
        }).then(function () {
            $scope.updateCalcStatus();
        }, function () {
            $scope.updateCalcStatus();
        });
    };

    $scope.copyToCalculatedPurchaseObject = function () {
        $http({
            method: 'POST',
            url: '/CalcRunner/CopyToCalculatedPurchaseObject'
        }).then(function () {
            $scope.updateCalcStatus();
        }, function () {
            $scope.updateCalcStatus();
        });
    };

    $scope.CreateExternal = function () {
        messageBoxService.showConfirm('Запустить выгрузку базы?', 'Выгрузка базы')
            .then(runCreateExternal, function () { });
    };

    function runCreateExternal() {
        $http({
            method: 'POST',
            url: '/CalcRunner/CreateExternal'
        }).then(function (response) {
            alert(response.data);
            $scope.updateCalcStatus();
        }, function () {
            $scope.updateCalcStatus();
        });
    }


    $scope.QlikJob = function() {
        messageBoxService.showConfirm('Запустить выкладку БД госсегмента в Qlik?', 'Выкладка базы гос сегмента')
            .then(runQlikJob, function() {});
    };

    function runQlikJob() {
        $http({
            method: 'POST',
            url: '/CalcRunner/QlikJob'
        }).then(function () {
            $scope.updateCalcStatus();
        }, function () {
            $scope.updateCalcStatus();
        });
    }

    $scope.CreateExternalShipment = function() {
        messageBoxService.showConfirm('Запустить выгрузку базы гос сегмента?', 'Выгрузка базы гос сегмента')
            .then(runCreateExternalShipment, function() {});
    };

    function runCreateExternalShipment() {
        $http({
            method: 'POST',
            url: '/CalcRunner/CreateExternalShipment'
        }).then(function (response) {
            alert(response.data);
            $scope.updateCalcStatus();
        }, function () {
            $scope.updateCalcStatus();
        });
    }

    $scope.CreateExternalShipmentDatabase_startNight = function () {
        $scope.loading = $http({
            method: 'POST',
            url: '/CalcRunner/CreateExternalShipmentDatabase_startNight'
        }).then(function (response) {
            $scope.updateCalcStatus();
        }, function () {
            $scope.updateCalcStatus();
        });


    };

    $scope.Action = function (name) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/CalcRunner/Action/',
                data: JSON.stringify({ name: name })
            }).then(function (response) {
                var data = response.data;
                $scope.Init_Action();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Init_Action = function () {
        $scope.dataLoading =
            $http({
                method: 'GET',
                url: '/CalcRunner/Action/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                $scope.GovernmentPurchases_ToProvizor_status = data.GovernmentPurchases_ToProvizor_status;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.updateCalcStatus();
   // $interval($scope.updateCalcStatus, 15000);
    $scope.Init_Action();
}