angular
    .module('DataAggregatorModule')
    .controller('SQAController', ['$scope', '$http', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', SQAController]);

function SQAController($scope, $http, messageBoxService, uiGridCustomService, errorHandlerService, formatConstants) {

    

    //Задаем свойства грида
    $scope.gridOptions = uiGridCustomService.createOptions('SQA_Grid');

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
        noUnselect: false,
        columnDefs: [
            {
                name: 'Id',
                field: 'Id'
            },
            {
                name: 'InnGroupId',
                field: 'INNGroupId'
            },
            {
                name: 'МНН',
                field: 'INNGroup'
            },
            {
                name: 'FormProductId',
                field: 'FormProductId'
            },
            {
                name: 'Форма выпуска',
                field: 'FormProduct'
            },
            {
                name: 'DosageGroupId',
                field: 'DosageGroupId'
            },
            {
                name: 'Дозировка',
                field: 'DosageGroup'
            },
            {
              name: 'ПКУ',
              field: 'IsSQA',
              enableCellEdit: true,
              type: 'boolean',
              width: '100',
              cellTemplate: '<input type="checkbox" ng-model="row.entity.IsSQA" ng-change="grid.appScope.ChangeSQA(row.entity)">'
            }
        ]
    };

    angular.extend($scope.gridOptions, gridOptions);

    $scope.gridOptions.onRegisterApi = function (gridApi) {

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {

            $scope.atc.atc1 = {
                Id: row.entity.Atc1Id,
                Value: row.entity.Atc1Value,
                Description: row.entity.Atc1Description
            };
            


        });


    };

    $scope.ChangeSQA = function (row) {

        $http({
            method: "POST",
            url: "/SQA/Change/",
            data: JSON.stringify({ Id: row.Id, value: row.IsSQA })
        }).then(function (response) {
            if (!response.data.Success)
                row.IsSQA = !row.IsSQA;
        }, function () {
            $scope.message = "Unexpected Error";
        });
        
    }

    function Load() {

        $scope.loading = $http({
            method: "POST",
            url: "/SQA/Load/"
        }).then(function (response) {
            $scope.gridOptions.data = response.data;

        }, function () {
            $scope.message = "Unexpected Error";
        });
    }
    Load();

    
    
}