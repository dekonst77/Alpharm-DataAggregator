angular
    .module('DataAggregatorModule')
    .controller('NFCController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', NFCController]);

function NFCController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "NFC";
    $scope.user = userService.getUser();

    $scope.NFC_Init = function () {
        $scope.RouteAdministration = [];
        $scope.Grid_NFC = uiGridCustomService.createGridClassMod($scope,"Grid_NFC");

        $scope.Grid_NFC.Options.columnDefs = [
            { name: 'Код 1', field: 'Nfc1Value', filter: { condition: uiGridCustomService.condition } },
            { name: 'Описание 1', field: 'Nfc1Description', filter: { condition: uiGridCustomService.condition } },
            { name: 'Код 2', field: 'Nfc2Value', filter: { condition: uiGridCustomService.condition } },
            { name: 'Описание 2', field: 'Nfc2Description', filter: { condition: uiGridCustomService.condition } },
            { name: 'Код 3', field: 'Nfc3Value', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, name: 'Описание 3', field: 'Nfc3Description', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, width: 100, name: 'Способ введения', field: 'RouteAdministrationId', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.RouteAdministration,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            }
        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/NFC/NFC_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            var data = response.data;

            Array.prototype.push.apply($scope.RouteAdministration, response.data.Data.RouteAdministration);

            $scope.NFC_search();
            return response.data;
        });
    };

    $scope.NFC_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/NFC/NFC_search/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_NFC.Options.data = data.Data.NFC;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.NFC_search = function () {
        if ($scope.Grid_NFC.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.NFC_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.NFC_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.NFC_search_AC();
        }

    };
     $scope.NFC_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/NFC/NFC_save/',
                data: JSON.stringify({
                    array_NFC: $scope.Grid_NFC.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.NFC_search_AC();
                    }
                    else {
                        $scope.Grid_NFC.NeedSave = false;
                        $scope.Grid_NFC.ClearModify();

                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

}