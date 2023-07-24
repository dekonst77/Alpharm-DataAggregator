angular
    .module('DataAggregatorModule')
    .controller('CheckReportDialogController', ['$scope', '$http', '$uibModalInstance', 'ReportFilter', CheckReportDialogController]);

function CheckReportDialogController($scope, $http, $modalInstance, ReportFilter) {

    // --- инициализация ---
    $scope.ReportFilter = {
        RegistrationCertificateNumber: null, // РУ
        CheckClassifireReport: null // Отчёт
    }

    $scope.ReportFilterList = ReportFilter.ReportFilterList;

    switch (ReportFilter.Action) {
        case 'add':
            $scope.Add = true;
            break;

        case 'edit':
            $scope.Add = false;
            break;

        default:
            $scope.Add = null;
    }
    
    $scope.ReportFilter = {
        Id: ReportFilter.CurrentReport.Id,
        RegistrationCertificateNumber: ReportFilter.CurrentReport.RegistrationCertificateNumber, // РУ
        CheckClassifireReport: ReportFilter.CurrentReport.CheckClassifireReport // Отчёт
    }

    // --- инициализация ---

    // поиск РУ
    $scope.searchRegistrationNumber = function (value) {

        return $http({
            method: "POST",
            url: "/CheckClassifireReport/SearchRegistrationNumber/",
            data: JSON.stringify({ Value: value })
        }).then(function (response) {
            return response.data;
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    $scope.CanOK = function () {
        if (($scope.ReportFilter.RegistrationCertificateNumber == null) || ($scope.ReportFilter.CheckClassifireReport == null))
            return false;

        if (($scope.ReportFilter.RegistrationCertificateNumber.Id == null) || ($scope.ReportFilter.RegistrationCertificateNumber.Value == null))
            return false;

        if (($scope.ReportFilter.CheckClassifireReport.Id == null) || ($scope.ReportFilter.CheckClassifireReport.Value == null))
            return false;

        return true;
    };

    $scope.ok = function () {
        $modalInstance.close($scope.ReportFilter);
    };

    $scope.setId = function (dictionaryItem, field) {
        if (field === 'RegistrationCertificateNumber')
            $scope.ReportFilter.RegistrationCertificateNumber = dictionaryItem;
    };

    $scope.clear = function () {
        $scope.ReportFilter = null;
    };
}