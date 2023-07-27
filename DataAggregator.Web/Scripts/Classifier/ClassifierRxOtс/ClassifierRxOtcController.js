angular
    .module('DataAggregatorModule')
    .controller('ClassifierRxOtcController', ['$scope', '$route', '$http', '$uibModal', '$timeout', 'userService', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', ClassifierRxOtcController]);

function ClassifierRxOtcController($scope, $route, $http, $uibModal, $timeout, userService, messageBoxService, uiGridCustomService, errorHandlerService, formatConstants) {

    $scope.message = 'Пожалуйста, ожидайте... Загрузка';
    $scope.Title = "Модуль для простановки RX OTC";
    $scope.user = userService.getUser();
    $scope.canSearch = function () { return true; }
    $scope.filter = {
        Used: true, // в использовании
        Excluded: false // исключённые
    }

    $scope.ClassifierRxOtc_Init = function () {

        //******** Grid ******** ->
        $scope.RxOtcGrid = uiGridCustomService.createGridClassMod($scope, 'RxOtcGrid', null, "ExceptionGrid_dblClick");
        $scope.RxOtcGrid.Options.showGridFooter = true;
        $scope.RxOtcGrid.Options.showColumnFooter = false;
        $scope.RxOtcGrid.Options.multiSelect = false;
        $scope.RxOtcGrid.Options.enableSelectAll = false;
        $scope.RxOtcGrid.Options.enableFiltering = true;
        $scope.RxOtcGrid.Options.modifierKeysToMultiSelect = false;

        $scope.RxOtcGrid.Options.columnDefs = [
            { headerTooltip: true, name: 'Used', field: 'Used', type: 'boolean', cellTemplate: '_icon.html', width: 25, visible: false, nullable: false },

            { headerTooltip: true, name: 'RegistrationCertificateId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'RegistrationCertificateId', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'РУ', enableCellEdit: false, width: 300, field: 'RegistrationCertificateNumber', visible: true, nullable: false, filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, name: 'SKU', enableCellEdit: false, width: 100, cellTooltip: true, field: 'DrugId', visible: true, nullable: false },
            { headerTooltip: true, name: 'classId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'ClassifierInfoId', visible: true, nullable: false, filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, name: 'ТН + ФВ + Д + Ф + МНН', enableCellEdit: false, width: 600, field: 'TN_FP_D_F_INN', visible: true, nullable: false, filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, name: 'Rx', enableCellEdit: true, field: 'Rx', type: 'boolean', nullable: false },
            { headerTooltip: true, name: 'Otc', enableCellEdit: true, field: 'Otc', type: 'boolean', nullable: false },
            { headerTooltip: true, name: 'RURx', displayName: 'RURx', enableCellEdit: false, field: 'RURx', type: 'boolean', nullable: false },
            { headerTooltip: true, name: 'IsChecked', enableCellEdit: true, field: 'IsChecked', type: 'boolean' }
        ];

        $scope.RxOtcGrid.SetDefaults();

        $scope.RxOtcGrid.afterCellEdit = function editRowDataSource(rowEntity, colDef, newValue, oldValue) {

            // проверка на изменение
            if (newValue === oldValue || newValue === undefined)
                return;

            if (colDef.field === 'Rx') {
                rowEntity.Otc = !newValue;
            }

            if (colDef.field === 'Otc') {
                rowEntity.Rx = !newValue;
            }
        };
        //******** Grid ******** <-

        $scope.ClassifierRxOtc_Search();
    }

    $scope.ClassifierRxOtc_Search = function () {

        $scope.message = 'Пожалуйста, ожидайте... Загрузка данных.';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/ClassifierRxOtc/Init/',
            data: JSON.stringify($scope.filter)
        }).then(function (response) {

            if (response.status === 200) {
                $scope.RxOtcGrid.Options.data = response.data.Data.RxOtc;
            }
            else {
                console.error(response);
                messageBoxService.showError(response.statusText);
            }

        }, function (response) {
            console.error('errorHandlerService.showResponseError = ' + response);
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.ClassifierRxOtc_Save = function () {
        var array_upd = $scope.RxOtcGrid.GetArrayModify();

        console.log(array_upd);

        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/ClassifierRxOtc/Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {

                    if (response.status === 200) {
                        console.log('успешно записаны данные в БД');

                        $scope.RxOtcGrid.ClearModify();

                        var data = response.data.Data.Data.ClassifierRxOtcRecord;
                        //console.log(data);

                        // корректируем оригинал
                        data.forEach(el => {
                            var index = origdata.findIndex(item => item.Classifierid === el.Classifierid);
                            origdata.splice(index, 1, el);
                        });

                        //alert("Сохранено");
                        messageBoxService.showInfo("Сохранено записей: " + data.length);
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
}