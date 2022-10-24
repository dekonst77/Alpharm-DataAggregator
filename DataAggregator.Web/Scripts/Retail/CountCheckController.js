angular
    .module('DataAggregatorModule')
    .controller('CountCheckController', ['$scope', '$http', 'uiGridCustomService', 'formatConstants', CountCheckController]);

function CountCheckController($scope, $http, uiGridCustomService, formatConstants) {
    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    // Фильтр
    $scope.filter = {
        date: previousMonthDate
    };

    $scope.countCheckGrid = {
        options: uiGridCustomService.createOptions('CountCheck_Grid')
    };

    $scope.countCheckGrid.options.columnDefs = [
        { displayName: 'PERIOD', field: 'getPeriod()', width: 100, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filterCellFiltered: true },
        { name: 'Код региона', field: 'RegionCode' },
        { name: 'Регион', field: 'RegionName' },
        { name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Торговое наименование', field: 'TradeName' },
        { name: 'ФВ', field: 'FormProduct' },
        { name: 'Производитель', field: 'OwnerTradeMark' },
        { name: 'Ср кол-во прод', field: 'AvgSellingCount', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { name: 'Ср кол-во прод 6 мес', field: 'AvgSellingCountHalfYear', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { name: 'Коэффициент', field: 'Coefficient', type: 'number', cellFilter: formatConstants.FILTER_COEFFICIENT, enableCellEdit: true }
    ];

    $scope.countCheckGrid.options.customEnableCellSelection = true;

    $scope.$on('uiGridEventEndCellEdit', function (data) {
        var newValue = data.targetScope.row.entity;
        $scope.editCountCheckRecord(newValue.Id, newValue.Coefficient);
    });

    $scope.$watch(function () { return $scope.filter.date; },
        function () {
            getCountCheckList();
        },
        true);

    function getCountCheckList() {
        var data =
        {
            year: $scope.filter.date.getFullYear(),
            month: $scope.filter.date.getMonth() + 1
        };

        $scope.loading = $http.get('/CountCheck/GetCountCheckList',  { params: data })
            .then(function (response) {
                $scope.countCheckGrid.options.data = response.data;

                var period = new Date($scope.filter.date.getFullYear(), $scope.filter.date.getMonth(), 1);

                angular.forEach($scope.countCheckGrid.options.data, function (row) {
                    row.getPeriod = function () {
                        return period;
                    }
                });

        },function (response) {
            $scope.countCheckGrid.options.data = [];
            alert('Ошибка:\n' + JSON.stringify(response));
        });
    }

    $scope.editCountCheckRecord = function (id, coefficient) {
        var request = {
            id: id,
            coefficient: coefficient
        };

        $scope.loading = $http({
            method: 'POST',
            url: '/CountCheck/EditCountCheckRecord',
            data: JSON.stringify(request)
        }).then(function (response) {
            var editedRow = $.grep($scope.countCheckGrid.options.data, function (r) {
                return r.Id == id;
            })[0];

            editedRow.Coefficient = response.data.Coefficient;
        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });
    }
}