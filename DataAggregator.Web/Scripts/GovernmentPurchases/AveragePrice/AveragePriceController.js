angular
    .module('DataAggregatorModule')
    .controller('AveragePriceController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', AveragePriceController]);

function AveragePriceController($scope, $http, $uibModal, uiGridCustomService, formatConstants) {

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    // Фильтр
    $scope.filter = {
        date: previousMonthDate
    };

    function getYearMonth() {
        return {
            year: $scope.filter.date.getFullYear(),
            month: $scope.filter.date.getMonth() + 1
        };
    }

    $scope.selectedRegion = {
        FederalDistrict: null,
        FederalDistrictId: null,
        FederationSubject: null,
        FederationSubjectId: null,
        District: null,
        DistrictId: null,
        City: null,
        CityId: null,
        SelectedRegionId: null
    };

    $scope.selectedDrug = {
        Drug: null,
        ClassifierId: null,
        OwnerTradeMark: null,
        Packer: null
    };

    $scope.averagePriceGrid = uiGridCustomService.createGridClass($scope, 'AveragePrice_Grid');

    $scope.averagePriceGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number', filter: { condition: uiGridCustomService.condition } },
        { name: 'Id региона', field: 'RegionId', filter: { condition: uiGridCustomService.condition }},
        { name: 'Федеральный округ', field: 'FederalDistrict', filter: { condition: uiGridCustomService.condition }},
        { name: 'Субъект федерации', field: 'FederationSubject', filter: { condition: uiGridCustomService.condition } },
        { name: 'Район', field: 'District', filter: { condition: uiGridCustomService.condition } },
        { name: 'Город', field: 'City', filter: { condition: uiGridCustomService.condition } },
        { name: 'Год', field: 'Year', filter: { condition: uiGridCustomService.condition }},
        { name: 'Месяц', field: 'Month', filter: { condition: uiGridCustomService.condition } },
        { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Торговое наименование', field: 'TradeName', filter: { condition: uiGridCustomService.condition }},
        { name: 'Описание', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition }},
        { name: 'Производитель', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition }},
        { name: 'Упаковщик', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
        { name: 'Средняя цена', field: 'Price', type: 'number', cellFilter: formatConstants.FILTER_PRICE, enableCellEdit: true }
    ];

    $scope.averagePriceGrid.Options.multiSelect = false;
    $scope.averagePriceGrid.Options.noUnselect = false;
    $scope.averagePriceGrid.Options.showGridFooter = true;

    $scope.averagePriceGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.averagePriceGridApi = gridApi;
    };

    $scope.$on('uiGridEventEndCellEdit', function (data) {
        var newValue = data.targetScope.row.entity;
        $scope.saveAveragePrice(newValue.Id, newValue.Price);
    });

    $scope.openFilterDialog = function () {
        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/AveragePrice/_AveragePriceFilterView.html',
            controller: 'AveragePriceFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: {
                    selectedRegion: $scope.selectedRegion,
                    selectedDrug: $scope.selectedDrug,
                    selectedPeriodDate: $scope.filter.date
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                $scope.selectedRegion = data.selectedRegion;
                $scope.selectedDrug = data.selectedDrug;
                $scope.selectedPeriodDate = data.selectedPeriodDate;
                $scope.loadAveragePrices();
            },
            // cancel
            function () {
            }
        );
    };

    $scope.loadAveragePrices = function () {
        var yearMonth = getYearMonth();

        var data = {
            selectedRegion: $scope.selectedRegion,
            selectedDrug: $scope.selectedDrug,
            year: yearMonth.year,
            month: yearMonth.month
        };

        $scope.loading = $http({
            method: 'POST',
            url: '/AveragePrice/GetAveragePrices',
            data: data
        }).then(function (response) {
            $scope.averagePriceGrid.Options.data = response.data;
        }, function () {
            $scope.averagePriceGrid.Options.data = [];
            alert(':(');
        });
    };

    $scope.saveAveragePrice = function(id, price) {
        $scope.loading = $http({
            method: 'POST',
            url: '/AveragePrice/SaveAveragePrice',
            data: { id: id, price: price }
        }).then(function () {
            
        }, function () {
            alert('Сохранить не получилось :(');
        });
    };
}