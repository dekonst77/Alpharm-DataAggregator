angular
    .module('DataAggregatorModule')
    .controller('ATCWhoController', ['$scope', '$http', 'messageBoxService', 'uiGridCustomService', ATCWhoController]);

function ATCWhoController($scope, $http, messageBoxService, uiGridCustomService) {

    //ATC
    function clear() {
        $scope.atc = {
            atc1: { Id: 0, Value: '', Description: '' },
            atc2: { Id: 0, Value: '', Description: '' },
            atc3: { Id: 0, Value: '', Description: '' },
            atc4: { Id: 0, Value: '', Description: '' },
            atc5: { Id: 0, Value: '', Description: '' }
        }
    }

    clear();

    //Задаем свойства грида
    $scope.gridOptions = uiGridCustomService.createOptions('AtcWho_Grid');

    var gridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 30,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: false,
        noUnselect: false,
        columnDefs: [
                        { name: 'Код 1', field: 'Atc1Value' },
                        { name: 'Описание 1', field: 'Atc1Description' },
                        { name: 'Код 2', field: 'Atc2Value' },
                        { name: 'Описание 2', field: 'Atc2Description' },
                        { name: 'Код 3', field: 'Atc3Value' },
                        { name: 'Описание 3', field: 'Atc3Description' },
                        { name: 'Код 4', field: 'Atc4Value' },
                        { name: 'Описание 4', field: 'Atc4Description' },
                        { name: 'Код 5', field: 'Atc5Value' },
                        { name: 'Описание 5', field: 'Atc5Description' }
        ]
    }

    angular.extend($scope.gridOptions, gridOptions);

    $scope.gridOptions.onRegisterApi = function (gridApi) {

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
           

            $scope.atc.atc1 = {
                Id: row.entity.Atc1Id,
                Value: row.entity.Atc1Value,
                Description: row.entity.Atc1Description
            };

            $scope.atc.atc2 = {
                Id: row.entity.Atc2Id,
                Value: row.entity.Atc2Value,
                Description: row.entity.Atc2Description
            };

            $scope.atc.atc3 = {
                Id: row.entity.Atc3Id,
                Value: row.entity.Atc3Value,
                Description: row.entity.Atc3Description
            };

            $scope.atc.atc4 = {
                Id: row.entity.Atc4Id,
                Value: row.entity.Atc4Value,
                Description: row.entity.Atc4Description
            };

            $scope.atc.atc5 = {
                Id: row.entity.Atc5Id,
                Value: row.entity.Atc5Value,
                Description: row.entity.Atc5Description
            };


        });


    };



    function Load() {

        $scope.loading = $http({
            method: "POST",
            url: "/ATCWho/Load/"
        }).then(function (response) {
            $scope.gridOptions.data = response.data;

        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    function reload(data) {
        if (data.Success)
            Load();
        else
            messageBoxService.showError(data.Message);
    }

    Load();

    $scope.add = function () {
        $http({
            method: "POST",
            url: "/ATCWho/Add/",
            data: JSON.stringify({ value: $scope.atc })
        }).then(function (response) {
            reload(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    $scope.change = function () {
        $http({
            method: "POST",
            url: "/ATCWho/Change/",
            data: JSON.stringify({ value: $scope.atc })
        }).then(function (response) {
            reload(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    $scope.delete = function () {
        $http({
            method: "POST",
            url: "/ATCWho/Delete/",
            data: JSON.stringify({ value: $scope.atc })
        }).then(function (response) {
            reload(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    $scope.clearField = function() {
        clear();
    }

    

}