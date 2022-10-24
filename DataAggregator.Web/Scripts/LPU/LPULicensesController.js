angular
    .module('DataAggregatorModule')
    .controller('LPULicensesController', ['$scope', '$http', 'messageBoxService', 'errorHandlerService', 'uiGridCustomService', 'formatConstants', LPULicensesController]);

function LPULicensesController($scope, $http, messageBoxService, errorHandlerService, uiGridCustomService, formatConstants) {

    $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'LPU_Licenses_rid');
    $scope.Grid.Options.showGridFooter = true;
    $scope.Grid.Options.multiSelect = false;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;
    $scope.Grid.Options.enableCellEditOnFocus = true,
    $scope.Grid.Options.enableCellEdit = false,
    $scope.Grid.Options.cellEditableCondition = function ($scope) {       
        // put your enable-edit code here, using values from $scope.row.entity and/or $scope.col.colDef as you desire
      
        return $scope.row.entity.manualAdd; // in this example, we'll only allow active rows to be edited
    };
    $scope.Grid.cellEditableCondition = true,

        $scope.Grid.Options.columnDefs = [
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'EntityINN', field: 'EntityINN', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Organization?inn={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'full_name_licensee', field: 'full_name_licensee', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'Address', field: 'Address', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'EntityOGRN', field: 'EntityOGRN', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'abbreviated_name_licensee', field: 'abbreviated_name_licensee', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'form', field: 'form', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'OrganizationId', field: 'OrganizationId', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Organization?id={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'nameEnglish', field: 'nameEnglish', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'LPUPointId', field: 'LPUPointId', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'BricksId', field: 'BricksId', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Bricks?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'OKVED', field: 'OKVED', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'ContactPersonFullname', field: 'ContactPersonFullname', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'Phone', field: 'Phone', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'Email', field: 'Email', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'Website', field: 'Website', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'worksId', field: 'worksId', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'fias_code', field: 'fias_code', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'number', field: 'number', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'EntityName', field: 'EntityName', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'EntityType', field: 'EntityType', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'NetworkName', field: 'NetworkName', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'Brand', field: 'Brand', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'Тип ЛПУ', field: 'TypeOf', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'Вид ЛПУ', field: 'VidOf', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'date_register', field: 'date_register', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true,  width: 100, name: 'manualAdd', field: 'manualAdd', filter: { condition: uiGridCustomService.condition } },
        ];

    $scope.Search = function () {

        var json = JSON.stringify($scope.filter);

        $scope.classifierLoading =
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/LPULicenses/LoadLPULicenses/',
                data: json
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.Options.data = data.Data;
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.CanSave = function () {
        var array_upd = $scope.Grid.GetArrayModify();

    }


    $scope.Save = function () {
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/LPULicenses/Save/',
                    data: JSON.stringify({ lpumodels: array_upd })
                }).then(function (response) {
                    if (response.data.Success) {
                        $scope.Grid.ClearModify();
                        alert("Сохранил");
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };

    $scope.GS_delete = function () {
        messageBoxService.showConfirm('Удалить Лицензию?', 'Удаление')
            .then(
                function () {
                    var selectedRows = $scope.Grid.selectedRows();
                    if (selectedRows.length !== 1) {
                        alert("Выделено не 1 строка, я так не буду удалять.");
                        return;
                    }
                    var id = selectedRows[0].Id;

                    if (id > 0) {
                        $scope.dataLoading =
                            $http({
                                method: 'POST',
                                url: '/LPULicenses/Delete/',
                                data: JSON.stringify({ GSId: GSId })
                            }).then(function (response) {
                                var data = response.data;
                                $scope.Grid.Options.data.remove(selectedRows[0]);
                            }, function (response) {
                                errorHandlerService.showResponseError(response);
                            });
                    }
                    else {
                        $scope.Grid.Options.data.remove(selectedRows[0]);
                    }
                },
                function (result) {

                }
            );
    };


}