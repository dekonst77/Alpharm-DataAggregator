angular
    .module('DataAggregatorModule')
    .controller('ClassifierEditorHistoryController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'filter','formatConstants', ClassifierEditorHistoryController]);

function ClassifierEditorHistoryController($scope, $http, $modalInstance, uiGridCustomService, filter, formatConstants) {
   
    $scope.historyGrid = uiGridCustomService.createGridClass($scope, 'ClassifierEditor_RegCertGrid');

    $scope.historyGrid.Options.columnDefs = [
        { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'DrugId', field: 'DrugId' },
        { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId' },
        { name: 'PackerId', field: 'PackerId' },
        { name: 'Who', field: 'Who' },
        { name: 'What', field: 'What' },
        { name: 'When', field: 'When' },
        { name: 'Flag', field: 'Flag' },
    ];

    $scope.historyGrid.Options.multiSelect = false;
    $scope.historyGrid.Options.modifierKeysToMultiSelect = false; 

    function load () {

        var json = JSON.stringify({ filter: filter });

        $scope.loading =
            $http({
                method: "POST",
                url: "/ClassifierEditor/LoadHistory/",
                data: json
            }).then(function (response) {
                $scope.historyGrid.Options.data = response.data.Data;

            },
                function () {

                    $scope.message = "Unexpected Error";
                    messageBoxService.showError("Произошла ошибка");
                });
    };

    load();

    $scope.ok = function () {
        $modalInstance.close();
    };

}
