angular
    .module('DataAggregatorModule')
    .controller('PriceRuleViewController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'model', 'editmode', 'errorHandlerService','formatConstants', PriceRuleViewController]);

function PriceRuleViewController($scope, $http, $modalInstance, uiGridCustomService, model, editmode, errorHandlerService, formatConstants) {

    ////Действия с Москвой и Питером
    //function changeArray(add, data) {
    //    if (add) {
    //        //Добавляем
    //        data.forEach(function (dataValue) {

    //            var exists = $scope.Region1.filter(function (regValue) { return regValue.RegionCode === dataValue.RegionCode });

    //            if (exists.length === 0)
    //                $scope.Region1.push(dataValue);

    //        });
    //    } else {
    //        //Удаляем
    //        data.forEach(function (value) { $scope.Region1.removeitem(value) });
    //    }

    //    unionRegion();
    //}

    //Регион уровня СанктПетербург или Москва
    $scope.Region1 = [];
    $scope.editmode = editmode;

    ////регионы Питера
    //$scope.IsSaintPetersburg = false;
    //$scope.changeSaintPetersburg = function () { changeArray($scope.IsSaintPetersburg, $scope.SaintPetersburg); };
    //$scope.SaintPetersburg = [];

    ////регионы Москвы
    //$scope.Moscow = [];
    //$scope.IsMoscow = false;
    //$scope.changeMoscow = function () { changeArray($scope.IsMoscow, $scope.Moscow); };

    $scope.noResults = false;

    // Модель формы
    $scope.model = {
        PriceRuleId: model.PriceRuleId,
        Region: {
            selectedItems: [],
            displayValue: '',
            search: searchRegion
        },
        RegionCode: model.RegionCode,
        Year: model.Year,
        Month: model.Month,
        ClassifierId: model.ClassifierId,
        DrugDescription: null,
        SellingPriceMin: model.SellingPriceMin,
        SellingPriceMax: model.SellingPriceMax,
        PurchasePriceMin: model.PurchasePriceMin,
        PurchasePriceMax: model.PurchasePriceMax,
        Regions: [],
        Comment: model.Comment,
        Validate: function () {
            return (this.RegionCode !== null || this.Regions.length > 0) &&
                this.ClassifierId !== null  &&
                (this.SellingPriceMin === null && this.SellingPriceMax === null || this.SellingPriceMin !== null && this.SellingPriceMax !== null) &&
                (this.PurchasePriceMin === null && this.PurchasePriceMax === null || this.PurchasePriceMin !== null && this.PurchasePriceMax !== null);
        }
    };

    //Остлеживаем изменение списка выбранных регионов
    $scope.$watchGroup(['model.Region.selectedItems']
      ,
        function (newValue, oldValue) {
            if (newValue === oldValue)
                return;

            unionRegion();

        });

    function unionRegion() {
        $scope.model.Regions = [];
        

        $scope.model.Region.selectedItems.forEach(function (dataValue) {

            var exists = $scope.model.Regions.filter(function (regValue) { return regValue.RegionCode === dataValue.RegionCode });

            if (exists.length === 0)
                $scope.model.Regions.push(dataValue);
        });


        $scope.Region1.forEach(function (dataValue) {

            var exists = $scope.model.Regions.filter(function (regValue) { return regValue.RegionCode === dataValue.RegionCode });

            if (exists.length === 0)
                $scope.model.Regions.push(dataValue);
        });
    }
   
  
    //Функция поиска региона
    function searchRegion(value) {

        var httpPromise = $http.post('/Region/SearchRegionPM01/', JSON.stringify({ Value: value }))
            .then(function (response) {
                var data = response.data;

                angular.forEach(data, function (item) {
                    item.displayValue = item.Region;
                });
                return data;
            });

        return httpPromise;
    }

    //Удалить регион
    $scope.removeRegion = function (value) {
        $scope.model.Regions.removeitem(value);
    };

    // Первоначальная загрузка формы при редактировании
    $scope.loading = $http.post('/PriceRuleEditor/Initialize/', JSON.stringify({ model: $scope.model }))
    .then(function (response) {
        //Инициализация региона


            if (response.data.Region != null) {
                $scope.model.Region.selectedItems = [response.data.Region];
                $scope.model.Region.displayValue = response.data.Region.Region;
            }

            //Инициализация грида
        $scope.grid.Options.data = response.data.GridData;
       // $scope.SaintPetersburg = response.data.SaintPetersburg;
       // $scope.Moscow = response.data.Moscow;
    }, function (response) {
        $scope.model.RegionCode = null;
        $scope.model.ClassifierId = null;
        $scope.model.SellingPriceMin = null;
        $scope.model.SellingPriceMax = null;
        $scope.model.PurchasePriceMin = null;
        $scope.model.PurchasePriceMax = null;
        $scope.model.Comment = null;
        errorHandlerService.showResponseError(response);
    });

    //Грид с разультатами поиска DrugId, OwnerTradeMarkId, PackerId
    $scope.grid = uiGridCustomService.createGridClass($scope, 'PriceRuleEditor_PriceRuleViewGrid');
    $scope.grid.Options.multiSelect = false;
    $scope.grid.Options.modifierKeysToMultiSelect = false;

    //Выделение элемента в гриде
    $scope.grid.selectionChanged = function () {
        $scope.items = $scope.grid.getSelectedItem();

        if ($scope.items.length === 0) {
            $scope.model.ClassifierId = null;
        } else {
            $scope.model.ClassifierId = $scope.items[0].ClassifierId;
        }
    };

    //Колонки грида
    $scope.grid.Options.columnDefs = [
        { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Описание', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
        { name: 'Правообладатель', field: 'OwnerTradeMark', width: 100, filter: { condition: uiGridCustomService.condition } },
        { name: 'Упаковщик', field: 'Packer', width: 100, filter: { condition: uiGridCustomService.condition } }, 
        { name: 'SSum', field: 'SellingSumNDS', width: 100, cellFilter: formatConstants.FILTER_PRICE, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'PSum', field: 'PurchaseSumNDS', width: 100, cellFilter: formatConstants.FILTER_PRICE, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'SCount', field: 'SellingCount', width: 100, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'PCount', field: 'PurchaseCount', width: 100, cellFilter: formatConstants.FILTER_FLOAT_COUNT, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        
    ];
 
    //Поиск DrugId, OwnerTradeMarkId, PackerId
    $scope.searchDrug = function () {
        $scope.items = null;

        $scope.loading = $http.post('/PriceRuleEditor/SearchDrug/', JSON.stringify({ Value: $scope.model.DrugDescription, Year: $scope.model.Year, Month: $scope.model.Month }))
        .then(function (response) {
            $scope.grid.Options.data = response.data;
            $scope.grid.clearSelection();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };
  
    //Все ли поля выбраны для сохранения
    $scope.canSave = function () {
        return $scope.model.Validate();
    };

  

    $scope.save = function () {
        $scope.loading = $http.post('/PriceRuleEditor/SaveRule/', JSON.stringify({ model: $scope.model }))
        .then(function () {
            $modalInstance.close();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };

    //Закрытие формы
    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

}