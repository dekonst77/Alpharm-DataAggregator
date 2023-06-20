angular
    .module('DataAggregatorModule')
    .controller('SourceBrandBlackListController', ['$scope', '$http', 'uiGridCustomService', 'messageBoxService', 'errorHandlerService', SourceBrandBlackListController]);

function SourceBrandBlackListController($scope, $http, uiGridCustomService, messageBoxService, errorHandlerService) {

    /*фильтр*/
    var today = new Date();
    var previousMonthDate = new Date(today.getFullYear(), today.getMonth() - 1, 1);

    $scope.filter = {
        date: previousMonthDate
    };

    /*Grid*/
    $scope.BlackList = uiGridCustomService.createGridClassMod($scope, 'PharmacyBrandBlackList_BlackList');
    $scope.BlackList.Options.multiSelect = true;
    $scope.BlackList.Options.noUnselect = false;
    $scope.BlackList.Options.modifierKeysToMultiSelect = true;
    $scope.BlackList.Options.showGridFooter = true;

    $scope.BlackList.Options.columnDefs = [
        { name: 'SourceId', field: 'SourceId', type: 'number' },
        { name: 'BrandId', field: 'BrandId', type: 'number' },
        { name: 'Год', field: 'Year', type: 'number' },
        { name: 'Месяц', field: 'Month', type: 'number' }
    ];

    $scope.BlackList.SetDefaults();
    

    $scope.getList = function () {
        $scope.loading = $http({
            method: 'POST',
            url: '/SourceBrandBlackList/GetList',
            data: JSON.stringify({
                year: $scope.filter.date.getFullYear(), month: $scope.filter.date.getMonth() + 1
            })
        }).then(function (response) {
            $scope.BlackList.Options.data = response.data;
        }, function () {
            $scope.BlackList.Options.data = [];
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
                url: '/SourceBrandBlackList/UploadFromExcel/',
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