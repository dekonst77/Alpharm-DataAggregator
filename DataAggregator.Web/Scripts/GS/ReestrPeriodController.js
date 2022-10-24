angular
    .module('DataAggregatorModule')
    .controller('ReestrPeriodController', [
        '$scope', '$uibModalInstance', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', 'dialogParams', 
function ReestrPeriodController($scope, $uibModalInstance, uiGridCustomService, errorHandlerService, formatConstants, dialogParams) {
    var reestr_line_Grid_Api = undefined;
    $scope.reestr_line__header = dialogParams.Id;

    $scope.reestr_line_Grid = uiGridCustomService.createGridClass($scope, 'reestr_line_Grid');
    /*$scope.reestr_line_Grid.Options.showGridFooter = true;
    $scope.reestr_line_Grid.Options.multiSelect = true;
    $scope.reestr_line_Grid.Options.modifierKeysToMultiSelect = true;*/

    $scope.reestr_line_Grid.Options.customEnableRowSelection = true;
        $scope.reestr_line_Grid.Options.enableColumnResizing = true;
        $scope.reestr_line_Grid.Options.enableGridMenu = true;
        $scope.reestr_line_Grid.Options.enableSorting = true;
        $scope.reestr_line_Grid.Options.enableFiltering = true;
        $scope.reestr_line_Grid.Options.enableSelectAll = true;
        $scope.reestr_line_Grid.Options.enableRowSelection = true;
        $scope.reestr_line_Grid.Options.enableRowHeaderSelection = true;
        $scope.reestr_line_Grid.Options.enableCellEdit = false;
        $scope.reestr_line_Grid.Options.appScopeProvider = $scope;
        $scope.reestr_line_Grid.Options.enableFullRowSelection = true;
        $scope.reestr_line_Grid.Options.enableSelectionBatchEvent = true;
        $scope.reestr_line_Grid.Options.enableHighlighting = true;
        $scope.reestr_line_Grid.Options.multiSelect = true;
        $scope.reestr_line_Grid.Options.excessRows = 100;


    $scope.reestr_line_Grid.Options.onRegisterApi = function (gridApi) {
        reestr_line_Grid_Api = gridApi;
    };
    $scope.reestr_line_Grid.Options.columnDefs = [
        { name: 'Период', field: 'period'},
        { name: 'Существование', field: 'isExists'},
        { name: 'Имя Сети', field: 'NetworkName'},
        { name: 'Сумма', field: 'Summa'}
    ];
    //$scope.reestr_line_Grid.Options.columnDefs = [
    //    { name: 'Период', field: 'period', filter: { condition: uiGridCustomService.condition }, type: 'date' },
    //    { enableCellEdit: true, name: 'Существование', field: 'isExists', filter: { condition: uiGridCustomService.condition }, type: 'boolean' },
    //    { enableCellEdit: true, name: 'Имя Сети', field: 'NetworkName', filter: { condition: uiGridCustomService.condition } },
    //    { enableCellEdit: true, name: 'Сумма', field: 'Summa', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE }
    //];
    $scope.reestr_line_Grid.Options.data = dialogParams.Data;


    $scope.save = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
        } 


    ]);