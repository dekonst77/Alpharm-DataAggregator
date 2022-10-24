angular
    .module('DataAggregatorModule')
    .controller('PriceEtalonController', ['$scope', '$http', '$uibModal','uiGridCustomService', 'formatConstants', PriceEtalonController]);

function PriceEtalonController($scope, $http, $uibModal, uiGridCustomService, formatConstants) {


    $scope.model = { ClassifierId: null, Price: null, Id:null};

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);
     
    // Фильтр
    $scope.filter = {
        date: previousMonthDate,
    };

    $scope.priceGrid = uiGridCustomService.createGridClass($scope, 'priceEtalon_Grid');

    $scope.priceGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id' },
        { name: 'Год', field: 'Year' },
        { name: 'Месяц', field: 'Month' },
        {
            name: 'ClassifierId',
            field: 'ClassifierId',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
        },
        {
            name: 'DrugId',
            field: 'DrugId',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition }
        },
        {
            name: 'OwnerTradeMarkId',
            field: 'OwnerTradeMarkId',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition }
        },
        { name: 'PackerId', field: 'PackerId' },
        { name: 'Цена', field: 'Price', enableCellEdit: true },
        { name: 'Описание', field: 'DrugDescription' },
        { name: 'ТН', field: 'TradeName' },
        { name: 'Бренд', field: 'Brand' },
        { name: 'Производитель', field: 'OwnerTradeMark' },
        { name: 'Упаковщик', field: 'Packer' },
    ];

    $scope.priceGrid.Options.multiSelect = false;
    $scope.priceGrid.Options.modifierKeysToMultiSelect = false;


    $scope.priceGrid.Options.onRegisterApi = function (gridApi) {
        
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.model.Id = row.entity.Id;
            $scope.model.ClassifierId = row.entity.ClassifierId;
            $scope.model.Price = row.entity.Price;

        });

    };

    //$scope.$on('uiGridEventEndCellEdit', function (data) {
    //    var newValue = data.targetScope.row.entity;
    //    $scope.edit(newValue.Id, newValue.Price);
    //});


    function load() {

        var request = {
            Date: $scope.filter.date
        };
        $scope.loading = $http({
            method: 'POST',
            url: '/PriceEtalon/Load',
            data: JSON.stringify(request)
        }).then(function (response) {
            $scope.priceGrid.Options.data = response.data.Data;
        }, function (response) {
            $scope.priceGrid.Options.data = null;
            alert('Ошибка:\n' + JSON.stringify(response));
        });


    }

    $scope.canEdit = function() {
        return $scope.model.Id > 0;
    }

    $scope.edit = function () {

        var request = {
            model: {
                Id: $scope.model.Id,
                Price: $scope.model.Price,
                ClassifierId: $scope.model.ClassifierId,
                Year: $scope.filter.date.getFullYear(),
                Month: $scope.filter.date.getMonth() + 1,
            }
        };

        $scope.loading = $http({
            method: 'POST',
            url: '/PriceEtalon/Edit',
            data: JSON.stringify(request)
        }).then(function (response) {                    
        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });

    }

    $scope.add = function () {

        var request = {
            model: {
                Id: $scope.model.Id,
                Price: $scope.model.Price,
                ClassifierId: $scope.model.ClassifierId,
                Year: $scope.filter.date.getFullYear(),
                Month: $scope.filter.date.getMonth() + 1,
            }
        };

        $scope.loading = $http({
            method: 'POST',
            url: '/PriceEtalon/Add',
            data: JSON.stringify(request)
        }).then(function (response) {
            load();
        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });

    }

    $scope.delete = function () {

        var request = {
            model: {
                Id: $scope.model.Id,
                Price: $scope.model.Price,
                ClassifierId: $scope.model.ClassifierId,
                Year: $scope.filter.date.getFullYear(),
                Month: $scope.filter.date.getMonth() + 1,
            }
        };

        $scope.loading = $http({
            method: 'POST',
            url: '/PriceEtalon/Delete',
            data: JSON.stringify(request)
        }).then(function (response) {
            load();
        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });

    }

    $scope.search = function () {
        load();
    }

    //$scope.searchClassifier = function () {
    //    var data =
    //    {
    //        DrugId: $scope.filter.DrugId,
    //        OwnerTradeMarkId: $scope.filter.OwnerTradeMarkId,
    //        PackerId: $scope.filter.PackerId,
    //        TradeName: $scope.filter.TradeName,
    //        Brand: $scope.filter.Brand
    //    };

    //    $scope.loading = $http.get('/PriceEtalon/LoadClassifier', { params: data })
    //        .then(function (response) {
    //            $scope.priceGrid.Options.data = response.data.Data;

    //        }, function (response) {
    //            $scope.countCheckGrid.options.data = [];
    //            alert('Ошибка:\n' + JSON.stringify(response));
    //        });
    //}

    // Форма поиска препарата
    $scope.searchDrug = function () {
        var rule = $scope.rule;

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/OFD/PriceEtalon/_SearchDrugView.html',
            controller: 'PriceEtalonSearchDrugController',
            size: 'full',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function (classifierId) {
            $scope.model.ClassifierId = classifierId;
        }, function () { });
    };

}