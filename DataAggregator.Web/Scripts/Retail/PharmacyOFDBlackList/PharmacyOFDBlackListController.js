angular
    .module('DataAggregatorModule')
    .controller('PharmacyOFDBlackListController', ['$scope', '$http', 'uiGridCustomService', 'messageBoxService', 'errorHandlerService', PharmacyOFDBlackListController]);

function PharmacyOFDBlackListController($scope, $http, uiGridCustomService, messageBoxService, errorHandlerService) {

    /*фильтр*/
    var today = new Date();
    var previousMonthDate = new Date(today.getFullYear(), today.getMonth() - 1, 1);

    $scope.filter = {
        date: previousMonthDate
    };

    /*закупки*/
    $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'PharmacyBrandBlackList_Grid');
    $scope.Grid.Options.multiSelect = true;
    $scope.Grid.Options.noUnselect = false;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;
    $scope.Grid.Options.showGridFooter = true;

    $scope.Grid.Options.columnDefs = [
        { name: 'Id аптеки', field: 'TargetPharmacyId', type: 'number' },
        { name: 'Год', field: 'Year', type: 'number' },
        { name: 'Месяц', field: 'Month', type: 'number' }
    ];

    $scope.Grid.SetDefaults();

    $scope.getList = function () {
        $scope.loading = $http({
            method: 'POST',
            url: '/PharmacyOFDBlackList/GetList',
            data: JSON.stringify({
                year: $scope.filter.date.getFullYear(), month: $scope.filter.date.getMonth() + 1
            })
        }).then(function (response) {
            $scope.Grid.Options.data = response.data
        }, function () {
            $scope.Grid.Options.data = [];
        });
    }

    $scope.getList();

    $scope.UploadFromExcel = function (files) {
        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item) {
                formData.append('file', item);
            });
            formData.append('month', $scope.filter.date.getMonth() + 1);
            formData.append('year', $scope.filter.date.getFullYear());

            $scope.loading = $http({
                method: 'POST',
                url: '/PharmacyOFDBlackList/UploadFromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function () {
                messageBoxService.showInfo("Сохранено");
                $scope.getList();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        }
    };
}