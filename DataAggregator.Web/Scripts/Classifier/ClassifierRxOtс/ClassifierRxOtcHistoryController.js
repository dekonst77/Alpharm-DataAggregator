angular
    .module('DataAggregatorModule')
    .controller('ClassifierRxOtcHistoryController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'filter', 'formatConstants', ClassifierRxOtcHistoryController]);

function ClassifierRxOtcHistoryController($scope, $http, $modalInstance, uiGridCustomService, filter, formatConstants) {

    $scope.historyGrid = uiGridCustomService.createGridClass($scope, 'ClassifierRxOtcHistoryGrid');

    $scope.historyGrid.Options.columnDefs = [
        { name: 'Id', displayName: 'Log Id', field: 'Id', enableCellEdit: false, width: 100, cellTooltip: true, visible: false, nullable: false },
        { name: 'ClassifierId', field: 'ClassifierId', width: 200, filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Who', field: 'Who', width: 500 },
        { name: 'What', field: 'What', width: 500 },
        { name: 'When', field: 'When', width: 500 },
        { name: 'Flag', field: 'Flag', width: 100 },
    ];

    $scope.historyGrid.Options.multiSelect = false;
    $scope.historyGrid.Options.modifierKeysToMultiSelect = false;

    function load() {

        var json = JSON.stringify({ filter: filter });

        $scope.loading =
            $http({
                method: "POST",
                url: "/ClassifierRxOtc/LoadHistory/",
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
