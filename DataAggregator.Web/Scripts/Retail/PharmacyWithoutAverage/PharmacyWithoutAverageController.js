angular
    .module('DataAggregatorModule')
    .controller('PharmacyWithoutAverageController', ['$scope', '$http', 'uiGridCustomService', 'messageBoxService', 'errorHandlerService', PharmacyWithoutAverageController]);

function PharmacyWithoutAverageController($scope, $http, uiGridCustomService, messageBoxService, errorHandlerService) {

    /*фильтр*/
    var today = new Date();
    var previousMonthDate = new Date(today.getFullYear(), today.getMonth() - 1, 1);

    $scope.filter = {
        date: previousMonthDate,
        isGenerated: false
    };

    /*закупки*/
    $scope.inListGrid = uiGridCustomService.createGridClassMod($scope, 'PharmacyBrandBlackList_inListGrid');
    $scope.inListGrid.Options.multiSelect = true;
    $scope.inListGrid.Options.noUnselect = false;
    $scope.inListGrid.Options.modifierKeysToMultiSelect = true;
    $scope.inListGrid.Options.showGridFooter = true;

    $scope.inListGrid.Options.columnDefs = [
        { name: 'Id аптеки', field: 'TargetPharmacyId', type: 'number' },
        { name: 'Экстраполировать', field: 'IsGenerated', type: 'boolean' },
        { name: 'Год', field: 'Year', type: 'number' },
        { name: 'Месяц', field: 'Month', type: 'number' }
    ];

    $scope.inListGrid.SetDefaults();

    /*продажи*/
    $scope.outListGrid = uiGridCustomService.createGridClassMod($scope, 'TargetPharmacyWithoutAverage_OutListGrid');
    $scope.outListGrid.Options.multiSelect = true;
    $scope.outListGrid.Options.noUnselect = false;
    $scope.outListGrid.Options.modifierKeysToMultiSelect = true;
    $scope.outListGrid.Options.showGridFooter = true;

    $scope.outListGrid.Options.columnDefs = [
        { name: 'Id аптеки', field: 'TargetPharmacyId', type: 'number' },
        { name: 'Экстраполировать', field: 'IsGenerated', type: 'boolean' },
        { name: 'Год', field: 'Year', type: 'number' },
        { name: 'Месяц', field: 'Month', type: 'number' }
    ];

    $scope.outListGrid.SetDefaults();

    $scope.getList = function() {
        $scope.loading = $http({
            method: 'POST',
            url: '/PharmacyWithoutAverage/GetList',
            data: JSON.stringify({
                year: $scope.filter.date.getFullYear(), month: $scope.filter.date.getMonth() + 1, isGenerated: $scope.filter.isGenerated
            })
        }).then(function (response) {
            $scope.inListGrid.Options.data = response.data.In;
            $scope.outListGrid.Options.data = response.data.Out;
        }, function () {
            $scope.inListGrid.Options.data = [];
            $scope.outListGrid.Options.data = [];
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
                url: '/PharmacyWithoutAverage/UploadFromExcel/',
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