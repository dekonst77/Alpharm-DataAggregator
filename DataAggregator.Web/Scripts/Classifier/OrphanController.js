angular
    .module('DataAggregatorModule')
    .controller('OrphanController', ['$scope', '$http', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', OrphanController]);

function OrphanController($scope, $http, messageBoxService, uiGridCustomService, errorHandlerService, formatConstants) {

    $scope.message = `Пожалуйста, ожидайте. Загрузка данных...`;

    $scope.ShowOnlyOrphan = false;

    //---------------------
    //Задаём свойства грида
    //---------------------
    $scope.Grid_Orphan = uiGridCustomService.createGridClassMod($scope, "Grid_Orphan");

    $scope.Grid_Orphan.Options.columnDefs = [
        {
            name: 'Id',
            field: 'Id',
            type: 'number',
            width: '100'
        },
        {
            displayName: 'InnGroup Id',
            name: 'InnGroupId',
            field: 'INNGroupId',
            type: 'number',
            width: '100'
        },
        {
            name: 'МНН',
            field: 'INNGroup',
            width: '500'
        },
        {
            displayName: 'FormProduct Id',
            name: 'FormProductId',
            field: 'FormProductId',
            type: 'number',
            width: '100'
        },
        {
            name: 'Форма выпуска',
            field: 'FormProduct'
        },
        {
            displayName: 'DosageGroup Id',
            name: 'DosageGroupId',
            field: 'DosageGroupId',
            type: 'number',
            width: '100'
        },
        {
            name: 'Дозировка',
            field: 'DosageGroup'
        },

        {
            name: 'Орфанный',
            field: 'IsOrphan',
            enableCellEdit: false,
            type: 'boolean',
            width: '100'
        },
        {
            name: 'Постановление Правительства РФ',
            field: 'InDecreeRussianGovernment',
            enableCellEdit: true,
            type: 'boolean',
            width: '100',
            cellTemplate: '<input type="checkbox" ng-model="row.entity.InDecreeRussianGovernment" ng-change="grid.appScope.ChangeInDecreeRussianGovernment(row.entity)">'
        },
        {
            name: 'Перечень Минздрава',
            field: 'InListHealthMinistry',
            enableCellEdit: true,
            type: 'boolean',
            width: '100',
            cellTemplate: '<input type="checkbox" ng-model="row.entity.InListHealthMinistry" ng-change="grid.appScope.ChangeInListHealthMinistry(row.entity)">'
        },
        {
            name: 'ГРЛС',
            field: 'InGRLS',
            enableCellEdit: true,
            type: 'boolean',
            width: '100',
            cellTemplate: '<input type="checkbox" ng-model="row.entity.InGRLS" ng-change="grid.appScope.ChangeInGRLS(row.entity)">'
        },
        {
            name: 'Без регистрации',
            field: 'InWithoutReg',
            enableCellEdit: true,
            type: 'boolean',
            width: '100',
            cellTemplate: '<input type="checkbox" ng-model="row.entity.InWithoutReg" ng-change="grid.appScope.ChangeInWithoutReg(row.entity)">'
        }
    ];

    $scope.Grid_Orphan.SetDefaults();

    var gridOptions = {
        customEnableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        enableRowSelection: true,
        selectionRowHeaderWidth: 20,
        rowHeight: 30,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: false,
        noUnselect: false
    }

    angular.extend($scope.Grid_Orphan.Options, gridOptions);

    $scope.CurrentRow;
    $scope.Grid_Orphan.onSelectionChanged = function (row) {
        if (row !== undefined) {
            $scope.CurrentRow = row;
        }
    };
    //---------------------
    //Задаём свойства грида
    //---------------------

    function Load() {
        $scope.loading = $http({
            method: "POST",
            url: "/Orphan/Load/",
            data: JSON.stringify({ ShowOnlyOrphan: $scope.ShowOnlyOrphan })
        }).then(function (response) {
            $scope.Grid_Orphan.SetData(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }
    Load();

    $scope.Orphan_search = function () {
        if ($scope.Grid_Orphan.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранённые результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.OrphanListSave("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            Load();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            Load();
        }
    };

    // сохранение списка орфанных МНН
    $scope.OrphanListSave = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Orphan/Save/',
                data: JSON.stringify({
                    array_UPD: $scope.Grid_Orphan.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;

                if (data.Success) {
                    messageBoxService.showInfo("Сохранено записей: " + $scope.Grid_Orphan.GetArrayModify().length);

                    if (action === "search") {
                        Load();
                    } else {

                        var array_upd = $scope.Grid_Orphan.GetArrayModify(); // источник в таблице
                        var data = data.Data.OrphanRecords; // обновлённые записи

                        data.forEach(el => {
                            var index = array_upd.findIndex(item => item.Id === el.Id);

                            if (index >= 0) {
                                array_upd[index].IsOrphan = el.IsOrphan;
                            }
                        });
                    }

                    $scope.Grid_Orphan.ClearModify();
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.ChangeInDecreeRussianGovernment = function (entity) {
        $scope.Grid_Orphan.GridCellsMod($scope.CurrentRow, "InDecreeRussianGovernment", entity.InDecreeRussianGovernment);
    };

    $scope.ChangeInListHealthMinistry = function (entity) {
        $scope.Grid_Orphan.GridCellsMod($scope.CurrentRow, "InListHealthMinistry", entity.InListHealthMinistry);
    };

    $scope.ChangeInGRLS = function (entity) {
        $scope.Grid_Orphan.GridCellsMod($scope.CurrentRow, "InGRLS", entity.InGRLS);
    };

    $scope.ChangeInWithoutReg = function (entity) {
        $scope.Grid_Orphan.GridCellsMod($scope.CurrentRow, "InWithoutReg", entity.InWithoutReg);
    };
}