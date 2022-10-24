angular
    .module('DataAggregatorModule')
    .controller('GoodsPriceRuleViewController', ['$scope', '$http', '$uibModalInstance', 'uiGridCustomService', 'model', 'editmode', 'errorHandlerService', GoodsPriceRuleViewController]);

function GoodsPriceRuleViewController($scope, $http, $modalInstance, uiGridCustomService, model, editmode, errorHandlerService) {

    //Действия с Москвой и Питером
    function changeArray(add, data) {
        if (add) {
            //Добавляем
            data.forEach(function (dataValue) {

                var exists = $scope.model.Regions.filter(function (regValue) { return regValue.RegionCode === dataValue.RegionCode });

                if (exists.length === 0)
                    $scope.model.Regions.push(dataValue);
            });
        } else {
            //Удаляем
            data.forEach(function (value) { $scope.model.Regions.removeitem(value) });
        }
    }

    $scope.editmode = editmode;

    //регионы Питера
    $scope.IsSaintPetersburg = false;
    $scope.changeSaintPetersburg = function () { changeArray($scope.IsSaintPetersburg, $scope.SaintPetersburg); };
    $scope.SaintPetersburg = [];

    //регионы Москвы
    $scope.Moscow = [];
    $scope.IsMoscow = false;
    $scope.changeMoscow = function () { changeArray($scope.IsMoscow, $scope.Moscow); };

    $scope.noResults = false;

    // Модель формы
    $scope.model = {
        PriceRuleId: model.PriceRuleId,
        Region: null,
        RegionCode: model.RegionCode,
        Year: model.Year,
        Month: model.Month,
        GoodsId: model.GoodsId,
        GoodsDescription : null,
        OwnerTradeMarkId: model.OwnerTradeMarkId,
        PackerId: model.PackerId,
        SellingPriceMin: model.SellingPriceMin,
        SellingPriceMax: model.SellingPriceMax,
        Regions : [],
        Comment: model.Comment,
        Validate: function () {
            return (this.RegionCode !== null || this.Regions.length > 0) &&
                this.GoodsId !== null &&
                this.OwnerTradeMarkId !== null &&
                this.PackerId !== null &&
                (this.SellingPriceMin === null && this.SellingPriceMax === null || this.SellingPriceMin !== null && this.SellingPriceMax !== null);
        }
    };

    // Первоначальная загрузка формы при редактировании
    $scope.loading = $http.post('/GoodsPriceRuleEditor/Initialize/', JSON.stringify({ model: $scope.model }))
    .then(function (response) {
        //Инициализация региона
        $scope.model.Region = response.data.Region;

        //Инициализация грида
        $scope.grid.Options.data = response.data.GridData;

        $scope.SaintPetersburg = response.data.SaintPetersburg;
        $scope.Moscow = response.data.Moscow;
    }, function (response) {
        $scope.model.RegionCode = null;
        $scope.model.GoodsId = null;
        $scope.model.OwnerTradeMarkId = null;
        $scope.model.PackerId = null;
        $scope.model.SellingPriceMin = null;
        $scope.model.SellingPriceMax = null;
        $scope.model.Comment = null;
        errorHandlerService.showResponseError(response);
    });

    //Грид с разультатами поиска GoodsId, OwnerTradeMarkId, PackerId
    $scope.grid = uiGridCustomService.createGridClass($scope, 'GoodsPriceRuleEditor_PriceRuleViewGrid');
    $scope.grid.Options.multiSelect = false;
    $scope.grid.Options.modifierKeysToMultiSelect = false;

    //Выделение элемента в гриде
    $scope.grid.selectionChanged = function () {
        $scope.items = $scope.grid.getSelectedItem();

        if ($scope.items.length === 0) {
            $scope.model.GoodsId = null;
            $scope.model.OwnerTradeMarkId = null;
            $scope.model.PackerId = null;
        } else {
            $scope.model.GoodsId = $scope.items[0].GoodsId;
            $scope.model.OwnerTradeMarkId = $scope.items[0].OwnerTradeMarkId;
            $scope.model.PackerId = $scope.items[0].PackerId;
        }
    };

    //Колонки грида
    $scope.grid.Options.columnDefs = [
        { name: 'GoodsId', field: 'GoodsId', width:80, type: 'number', filter: { condition: uiGridCustomService.condition } },
        { name: 'OtmId', field: 'OwnerTradeMarkId', width: 80, type: 'number', filter: { condition: uiGridCustomService.condition } },
        { name: 'PackerId', field: 'PackerId', width: 80, type: 'number', filter: { condition: uiGridCustomService.condition } },
        { name: 'Описание', field: 'GoodsDescription', filter: { condition: uiGridCustomService.condition } },
        { name: 'Правообладатель', field: 'OwnerTradeMark', width: 100, filter: { condition: uiGridCustomService.condition } },
        { name: 'Упаковщик', field: 'Packer', width: 100, filter: { condition: uiGridCustomService.condition } }
    ];

    //Обнуляем результат для пустого значения
    $scope.clear = function () {
        if ($scope.model.Region !== null && $scope.model.Region.length === 0) {
            $scope.model.Region = null;
            $scope.model.RegionCode = null;
        }
    };

    //Задаем идентификатор региона, при выборе из выпадающего списка
    $scope.setId = function (item) {
        $scope.model.RegionCode = item.RegionCode;
    };

    //Поиск GoodsId, OwnerTradeMarkId, PackerId
    $scope.searchGoods = function () {

        $scope.items = null;

        $scope.loading = $http.post('/GoodsPriceRuleEditor/SearchGoods/', JSON.stringify({ Value: $scope.model.GoodsDescription, Year: $scope.model.Year, Month: $scope.model.Month }))
        .then(function (response) {
            $scope.grid.Options.data = response.data;
            $scope.grid.clearSelection();
        },function (response) { errorHandlerService.showResponseError(response); });
    };

    //Поиск региона
    $scope.searchRegion = function(value) {

        //Очищаем Id
        $scope.model.RegionCode = null;

        return $http.post('/Region/SearchRegion/', JSON.stringify({ Value: value }))
            .then(function(response) {
                return response.data;
            });
    };


    $scope.addRegion = function() {

        var item = { RegionCode: $scope.model.RegionCode, Region: $scope.model.Region };

        $scope.model.Regions.push(item);
    };

    //Все ли поля выбраны для сохранения
    $scope.canSave = function() {
        return $scope.model.Validate();
    };

    $scope.removeRegion = function(value) {
        $scope.model.Regions.removeitem(value);
    };


    $scope.canAddRegion = function() {
        var exists = $scope.model.Regions.filter(function(val) { return val.RegionCode === $scope.model.RegionCode });
        return !editmode && $scope.model.RegionCode != null && exists.length === 0;
    };

    $scope.save = function () {
        $scope.loading = $http.post('/GoodsPriceRuleEditor/SaveRule/', JSON.stringify({ model: $scope.model }))
            .then(function() {
                $modalInstance.close();
            }, function(response) {
                errorHandlerService.showResponseError(response);
            });
    };

    //Закрытие формы
    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };
}