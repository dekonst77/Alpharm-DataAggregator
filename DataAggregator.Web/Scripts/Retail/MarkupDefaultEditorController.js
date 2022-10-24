angular
    .module('DataAggregatorModule')
    .controller('MarkupDefaultEditorController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', MarkupDefaultEditorController]);

function MarkupDefaultEditorController(messageBoxService, $scope, $http, uiGridCustomService, uiGridConstants, formatConstants) {

    $scope.refresh = function () {
        getReport();
    }

    function getReport() {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/MarkupDefaultEditor/GetData/"
        }).then(function (response) {
            $scope.grid.Options.data = response.data;
        }, function () {
            messageBoxService.showError("Не удалось загрузить данные!");
        });
    }

    getReport();
    
    $scope.grid = uiGridCustomService.createGridClass($scope, 'MarkupDefaultEditor_Grid');
    $scope.grid.Options.enableSorting = true,
    $scope.grid.Options.columnDefs =
    [
        {
            name: 'Цена от',
            field: 'PriceMin',
            filter: { condition: uiGridCustomService.condition },
            type: 'number',
            sort: {
                direction: uiGridConstants.ASC,
                priority: 1
            }
        },
        { name: 'Цена до', field: 'PriceMax', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        {
            name: 'Код региона',
            field: 'Code',
            filter: { condition: uiGridCustomService.condition },
            type: 'number',
            sort: {
                direction: uiGridConstants.ASC,
                priority: 0
            }
        },
        { name: 'Регион', field: 'FullName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Наценка', field: 'Markup', enableFiltering: false, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, enableCellEdit: true }
    ];

    $scope.$on('uiGridEventEndCellEdit', function (data) {
        var newValue = data.targetScope.row.entity;
        $scope.saveMarkupDefault(newValue.Id, newValue.Markup);
    });

    $scope.saveMarkupDefault = function (id, markup) {
        $scope.loading = $http({
            method: "POST",
            url: "/MarkupDefaultEditor/SaveMarkupDefault",
            data: { id: id, markup: markup }
        }).then(function () {

        }, function () {
            messageBoxService.showError("Не удалось сохранить изменения!");
        });
    };
}



