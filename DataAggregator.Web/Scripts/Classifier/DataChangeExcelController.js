angular
    .module('DataAggregatorModule')
    .controller('DataChangeExcelController', [
        '$scope', '$window', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', DataChangeExcelController]);

function DataChangeExcelController($scope, $window, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.user = userService.getUser();

    $scope.DataChangeExcel_Init = function () {


        $scope.DataChangeExcel_ATCWhoLinkMKB_Update_Get = function () {

            $scope.dataLoading = $scope.objectsLoading = $http({
                method: 'POST',
                url: '/DataChangeExcel/ATCWhoLinkMKB_Update_Get/',
                data: {},
                headers: {
                    'Content-type': 'application/json'
                },
                responseType: 'arraybuffer'
            }).then(function (response) {
                var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                var fileName = 'ATCWhoLinkMKB_Update.xlsx';
                saveAs(blob, fileName);
            }, function () {
                $scope.message = 'Unexpected error';
                messageBoxService.showError($scope.message);
            });
        };

        $scope.DataChangeExcel_ATCWhoLinkMKB_Update_Save = function (files) {
            var stop = 1;

            if (files && files.length) {
                var formData = new FormData();
                files.forEach(function (item, i, arr) {
                    formData.append('uploads', item);
                });
                $scope.dataLoading = $http({
                    method: 'POST',
                    url: '/DataChangeExcel/ATCWhoLinkMKB_Update_Save/',
                    data: formData,
                    headers: {
                        'Content-Type': undefined
                    },
                }).then(function (response) {
                    alert("Загружен.")
                }, function (response) {
                    messageBoxService.showError(JSON.stringify(response));
                });
            }
        };
    };
}