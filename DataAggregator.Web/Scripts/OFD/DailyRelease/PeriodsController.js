angular
    .module('DataAggregatorModule')
    .controller('PeriodsController', ['$scope', '$http', 'uiGridCustomService', 'messageBoxService', 'errorHandlerService', 'formatConstants', '$uibModal', PeriodsController]);

function PeriodsController($scope, $http, uiGridCustomService, messageBoxService, errorHandlerService, formatConstants, $uibModal) {

    /*Grid*/
    $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'Periods_Grid');
    $scope.Grid.Options.showGridFooter = true;
    $scope.Grid.Options.multiSelect = true;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;
    $scope.Grid.Options.enableSelectAll = true;
    $scope.Grid.Options.enableFiltering = true;

    $scope.Grid.Options.columnDefs = [
        {
            cellTooltip: true, enableCellEdit: false, width: 80, visible: true, nullable: false, name: 'ClassifierId',
            field: 'ClassifierId', type: 'number', filter: { condition: uiGridCustomService.condition }
        }
    ];

    $scope.Grid.SetDefaults();

    $scope.Init = function () {
       
    }

    $scope.Init();
}