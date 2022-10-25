angular
    .module('DataAggregatorModule')
    .controller('SalesSKUBySFController', ['$scope', '$http', '$q', '$cacheFactory', '$filter', '$timeout', 'userService', 'uiGridCustomService', 'errorHandlerService', 'messageBoxService', 'uiGridConstants', 'formatConstants', 'uiGridPaginationService', SalesSKUBySFController]);

function SalesSKUBySFController($scope, $http, $q, $cacheFactory, $filter, $timeout, userService, uiGridCustomService, errorHandlerService, messageBoxService, uiGridConstants, formatConstants, uiGridPaginationService) {
    $scope.Title = "Продажи СКЮ по субъектам федерации";
    $scope.user = userService.getUser();
    console.debug($scope.user.Name);

    //-----------AngularJS Dropdown Multiselect------------->      
    $scope.selectByGroupModel = []; // результаты выбора
    $scope.selectByDistrictData = []; // фед. округа

    // субъекты федерации, регионы
    $scope.selectByRegionData = [
        /*
        { id: 46, label: "Курская область", districtId: 1, orderby: 10},
        { id: 48, label: "Липецкая область", districtId: 1, orderby: 10 },
        { id: 77, label: "Москва", districtId: 1, orderby: 10 },
        { id: 91, label: 'Республика Крым', districtId: 2, orderby: 1 },
        { id: 83, label: 'Ненецкий автономный округ', districtId: 3, orderby: '2' },
        { id: 65, label: 'Сахалинская область', districtId: '4', orderby: '3' },
        { id: 17, label: 'Республика Тыва', districtId: '5', orderby: '' },
        { id: 66, label: 'Свердловская область', districtId: '6', orderby: ''},
        { id: 58, label: 'Пензенская область', districtId: '7', orderby: '' },
        { id: 7, label: 'Кабардино-Балкарская Республика', districtId: '8', orderby: '' }
        */
    ];

    $scope.selectByGroups = []; // группировка по id фед. округа

    $scope.selectByGroupSettings = {
        selectByGroups: $scope.selectByGroups,

        groupByTextProvider: function (groupValue) {
            /*
            switch (groupValue) {
                case '1': return '1 Центральный федеральный округ';
                case '2': return '2 Южный федеральный округ';
                case '8': return '8 Северо-Кавказский федеральный округ';
                default: return 'Другие...'
            }
            */

            var res = $scope.selectByDistrictData
                .find(
                    function callback(currentValue, index, array) {
                        //console.log(currentValue);
                        //console.log(-groupValue);

                        if (-groupValue == currentValue.id)
                            return true;
                    }
                )

            //console.log(res);

            if (res == undefined)
                return 'Другие...'
            else
                return res.id + '. ' + res.label;

        },
        groupBy: 'group_order_by',
        scrollable: true,
        scrollableHeight: '800px',
        enableSearch: true,
        styleActive: true,
        smartButtonMaxItems: 5,
        smartButtonTextConverter: function (itemText, originalItem) {
            return itemText;
        }
    };

    $scope.translation = {
        buttonDefaultText: 'Выбрать регионы...',
        searchPlaceholder: 'Выбрать...',
        checkAll: 'Выбрать все регионы...',
        uncheckAll: 'Убрать все регионы...',
        selectGroup: ''
    }
    $scope.selectByEvents = {
        onItemSelect: function (item) {
            //console.log(item);
        }
    }
    //-----------AngularJS Dropdown Multiselect-------------<  

    $scope.periods = [];
    $scope.districts = [];

    $scope.inactive = false;

    $scope.SalesSKUbySF_Init = function () {

        //******** Grid ******** ->
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'SalesSKUbySF_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.showColumnFooter = false;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.enableSelectAll = true;
        $scope.Grid.Options.enableFiltering = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Grid.Options.flatEntityAccess = true;
        $scope.Grid.Options.fastWatch = true;

        $scope.Grid.Options.paginationPageSizes = [10000, 25000, 50000, 75000];
        $scope.Grid.Options.paginationPageSize = 25000;

        let cellTemplateHint = '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
        let numbercellTemplateHint = '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" title="{{COL_FIELD}}">{{COL_FIELD | number:2}}</div>'

        $scope.Grid.Options.gridMenuCustomItems = [
            {
                title: 'Показать итоги таблицы',
                action: function ($event) {
                    $scope.Grid.Options.showGridFooter = !$scope.Grid.Options.showGridFooter;
                    $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
                },
                order: 1
            },
            {
                title: 'Показать итоги колонок',
                action: function ($event) {
                    $scope.Grid.Options.showColumnFooter = !$scope.Grid.Options.showColumnFooter;
                    $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
                },
                order: 2
            }
        ];
        $scope.Grid.Options.columnDefs = [
            { headerTooltip: true, name: 'Id', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Id', type: 'number', visible: false, nullable: true },
            { headerTooltip: true, name: 'ФО', enableCellEdit: false, width: 100, field: 'FederalDistrict', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'СФ', enableCellEdit: false, width: 100, field: 'FederationSubject', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Classifier Id', enableCellEdit: false, width: 130, cellTooltip: true, field: 'ClassifierId', type: 'number', cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'BrandId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BrandId', type: 'number', visible: false, nullable: true, filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Бренд', enableCellEdit: false, width: 300, field: 'Brand', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'дробить по МНН', enableCellEdit: false, field: 'ToSplitMNN', type: 'boolean' },
            { headerTooltip: true, name: 'INNGroupId', width: 100, displayName: "INNGroupId", field: 'INNGroupId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
            { headerTooltip: true, name: 'МНН', field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'TradeNameId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'TradeNameId', type: 'number', visible: false, nullable: true, filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Наименование ТН', enableCellEdit: false, width: 300, field: 'TradeName', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Описание ТН', enableCellEdit: false, width: 300, field: 'DrugDescription', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'OwnerTradeMarkId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'OwnerTradeMarkId', type: 'number', visible: false, nullable: true, filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Правообладатель', enableCellEdit: false, width: 300, field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint },

            { headerTooltip: true, name: 'СТМ', enableCellEdit: false, width: 80, field: 'IsSTM', type: 'boolean' },
            { headerTooltip: true, name: 'ПКУ', enableCellEdit: false, width: 80, field: 'IsPKU', type: 'boolean' },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Текущий период', field: 'PeriodCurr', visible: true, nullable: false, headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: true, name: 'коэф-т коррекции', width: 100, field: 'Correction_factor', headerCellClass: 'editable', type: 'number',
                filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_COEFFICIENT, cellTemplate: cellTemplateHint
            },

            // Расчётные данные
            // по упаковкам
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 150, name: 'уп. тек. Старт', field: 'CalculatedData_PackagesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'editable', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'уп. тек.', field: 'CalculatedData_PackagesNumber_Correction', type: 'number', visible: true, nullable: true, headerCellClass: 'editable', filter: { condition: uiGridCustomService.numberCondition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 120, name: 'уп. -1', field: 'CalculatedData_PackagesNumber_1', type: 'number', visible: true, nullable: true, headerCellClass: 'calculatedata', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 120, name: 'уп. -2', field: 'CalculatedData_PackagesNumber_2', type: 'number', visible: true, nullable: true, headerCellClass: 'calculatedata', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 120, name: 'уп. -3', field: 'CalculatedData_PackagesNumber_3', type: 'number', visible: true, nullable: true, headerCellClass: 'calculatedata', filter: { condition: uiGridCustomService.numberCondition } },
            // по аптекам
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: '∑точек тек.', field: 'CalculatedData_PharmaciesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'editable', cellFilter: formatConstants.FILTER_SUM, filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: '∑точек -1', field: 'CalculatedData_PharmaciesNumber_1', type: 'number', visible: true, nullable: true, headerCellClass: 'calculatedata', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: '∑точек -2', field: 'CalculatedData_PharmaciesNumber_2', type: 'number', visible: true, nullable: true, headerCellClass: 'calculatedata', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: '∑точек -3', field: 'CalculatedData_PharmaciesNumber_3', type: 'number', visible: true, nullable: true, headerCellClass: 'calculatedata', filter: { condition: uiGridCustomService.numberCondition } },
            // средняя цена
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'AVG цена тек.', field: 'CalculatedData_PriceAVG', type: 'number', visible: true, nullable: true, headerCellClass: 'editable', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'AVG цена -1', field: 'CalculatedData_PriceAVG_1', type: 'number', visible: true, nullable: true, headerCellClass: 'calculatedata', filter: { condition: uiGridCustomService.numberCondition } },

            // ОФД данные
            // по упаковкам
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД уп. тек.', field: 'OFDData_PackagesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД уп. -1', field: 'OFDData_PackagesNumber_1', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД уп. -2', field: 'OFDData_PackagesNumber_2', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД уп. -3', field: 'OFDData_PackagesNumber_3', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },

            // по аптекам
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД тек.', field: 'OFDData_PharmaciesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД -1', field: 'OFDData_PharmaciesNumber_1', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД -2', field: 'OFDData_PharmaciesNumber_2', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД -3', field: 'OFDData_PharmaciesNumber_3', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint
            },

            // средняя цена
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД AVG цена тек.', field: 'OFDData_PriceAVG', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ОФД AVG цена -1', field: 'OFDData_PriceAVG_1', type: 'number', visible: true, nullable: true, headerCellClass: 'ofddata', filter: { condition: uiGridCustomService.numberCondition } },

            // Исходные данные
            // по упаковкам
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. уп. тек.', field: 'InitialData_PackagesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. уп. -1', field: 'InitialData_PackagesNumber_1', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. уп. -2', field: 'InitialData_PackagesNumber_2', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. уп. -3', field: 'InitialData_PackagesNumber_3', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },

            // по аптекам
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. тек.', field: 'InitialData_PharmaciesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. -1', field: 'InitialData_PharmaciesNumber_1', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. -2', field: 'InitialData_PharmaciesNumber_2', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. -3', field: 'InitialData_PharmaciesNumber_3', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            // средняя цена
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. AVG цена тек.', field: 'InitialData_PriceAVG', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. AVG цена -1', field: 'InitialData_PriceAVG_1', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            // Исходные данные <<<<<

            // Исходные данные Sell In
            // по упаковкам
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. уп. тек. (Sell In)', field: 'InitialDataSellIn_PackagesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. уп. -1 (Sell In)', field: 'InitialDataSellIn_PackagesNumber_1', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. уп. -2 (Sell In)', field: 'InitialDataSellIn_PackagesNumber_2', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. уп. -3 (Sell In)', field: 'InitialDataSellIn_PackagesNumber_3', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn',
                filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum, footerCellFilter: 'number:2'
            },

            // по аптекам
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. тек. (Sell In)', field: 'InitialDataSellIn_PharmaciesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. -1 (Sell In)', field: 'InitialDataSellIn_PharmaciesNumber_1', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. -2 (Sell In)', field: 'InitialDataSellIn_PharmaciesNumber_2', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. -3 (Sell In)', field: 'InitialDataSellIn_PharmaciesNumber_3', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            // средняя цена
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. AVG цена тек. (Sell In)', field: 'InitialDataSellIn_PriceAVG', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'исх. AVG цена -1 (Sell In)', field: 'InitialDataSellIn_PriceAVG_1', type: 'number', visible: true, nullable: true, headerCellClass: 'initialdata_SellIn', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },
            // Исходные данные Sell In <<<<<<

            // Исходники e-com
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'E-com упак.', field: 'EcomData_PackagesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'ecomdata', filter: { condition: uiGridCustomService.numberCondition } },

            // Дистр. отчётность
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ДО', field: 'DistrReporting_PackagesNumber', type: 'number', visible: true, nullable: true, headerCellClass: 'distrdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint },

            // Сумма, руб. расчет
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: '∑ руб. расчет тек.', field: 'TotalSumm', type: 'number', visible: true, nullable: true, headerCellClass: 'totalsum',
                filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM, cellTemplate: numbercellTemplateHint,
                aggregationType: uiGridConstants.aggregationTypes.sum
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: '∑ руб. расчет -1', field: 'TotalSumm_1', type: 'number', visible: true, nullable: true, headerCellClass: 'totalsum', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM, cellTemplate: numbercellTemplateHint },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 150, name: 'Коммент. тек.', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, name: 'Коммент. -1', field: 'Comment_1', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, name: 'Коммент. -2', field: 'Comment_2', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, name: 'Коммент. -3', field: 'Comment_3', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'цена ЖНВЛП', field: 'LifeSavingDrugsPrice', type: 'number', visible: true, nullable: true, headerCellClass: 'totalsum', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM }
        ];
        $scope.Grid.SetDefaults();

        $scope.Grid.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.edit.on.afterCellEdit($scope, editRowDataSource);

            /*
            $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                if (sortColumns.length == 0) {
                    paginationOptions.sort = null;
                } else {
                    paginationOptions.sort = sortColumns[0].sort.direction;
                }
                getPage();
            });
            gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                paginationOptions.pageNumber = newPage;
                paginationOptions.pageSize = pageSize;5
                getPage();
            });
            */
        };

        // редактируемые поля: Comment
        function editRowDataSource(rowEntity, colDef, newValue, oldValue) {
            const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
            const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];
            const day = 15;
            const period = new Date(Date.UTC(year, month - 1, day));

            console.log('editRowDataSource() -> year = ' + year);
            console.log('editRowDataSource() -> month = ' + month);
            console.log('editRowDataSource() -> period = ' + period);

            // проверка на изменение
            if (newValue === oldValue || newValue === undefined)
                return;

            $scope.dataLoading = $http({
                method: "POST",
                url: "/SalesSKUbySF/Edit/",
                data: { record: rowEntity, fieldname: colDef.field }
            }).then(function (response) {

                var data = response.data.Data;
                if (data.Success) {
                    let record = data.Data.SalesSKUBySFRecord[0];
                    rowEntity[colDef.field] = record[colDef.field];

                    //console.log(colDef);
                    //console.log(record);

                    if ((colDef.field === "Correction_factor") || (colDef.field === "CalculatedData_PackagesNumber"))
                        rowEntity["CalculatedData_PackagesNumber_Correction"] = record["CalculatedData_PackagesNumber_Correction"];

                    //console.log(record[colDef.field]);
                } else {
                    console.error(data.ErrorMessage);
                    messageBoxService.showError(data.ErrorMessage);
                }

                return true;
            }, function (response) {
                rowEntity[colDef.field] = oldValue;
                errorHandlerService.showResponseError(response);
                return false;
            });

            return;
        }
        //******** Grid ******** <-

        // фед. районы ->
        var requestDistricts = $http({
            method: 'POST',
            url: '/SalesSKUbySF/FederalDistricts_Init/',
            data: JSON.stringify({})
        }).then(function (response) {

            $scope.districts.length = 0;
            Array.prototype.push.apply($scope.districts, response.data);
            //$scope.data.currentdistrict = response.data[0];
            //console.log($scope.districts);
            //console.log($scope.data.currentdistrict);            

            // фед. округа
            $scope.selectByDistrictData.length = 0;
            Array.prototype.push.apply($scope.districts, response.data.forEach(
                function callback(currentValue, index, array) {
                    $scope.selectByDistrictData[index] = {
                        id: currentValue.Id,
                        label: currentValue.Name
                    }
                }
            ));
            //console.log($scope.selectByDistrictData);

            // группировка по полю 'id' фед. округа
            $scope.selectByGroups.length = 0;
            Array.prototype.push.apply($scope.selectByGroups, response.data.forEach(
                function callback(currentValue, index, array) {
                    $scope.selectByGroups[index] = -currentValue.Id
                }
            ));
            //console.log($scope.selectByGroups);

            return true;
        });
        // фед. районы <-

        // регионы ->
        var requestRegions = $http({
            method: 'POST',
            url: '/SalesSKUbySF/Regions_Init/',
            data: JSON.stringify({})
        }).then(function (response) {

            // все регионы
            $scope.selectByRegionData.length = 0;
            Array.prototype.push.apply($scope.selectByRegionData, response.data.forEach(
                function callback(currentValue, index, array) {
                    $scope.selectByRegionData[index] = {
                        id: currentValue.Code,
                        label: currentValue.Code + '. ' + currentValue.Name,
                        districtId: currentValue.FederalDistrictId,
                        group_order_by: currentValue.orderby
                    }
                }
            ));

            // найдём выбранные регионы из localStorage
            $scope.selectByGroupModel.length = 0;
            var selregions = localStorage.getItem("selregions") == null ? [] : JSON.parse(localStorage.getItem("selregions"));
            if (selregions.length > 0) {

                selregions.forEach((item) => {
                    let index = $scope.selectByRegionData.findIndex(element => {
                        if (element.id == item.id)
                            return true
                        else
                            return false
                    })
                    if (index !== -1)
                        $scope.selectByGroupModel.push($scope.selectByRegionData[index]);
                })
            }
            //console.debug($scope.selectByGroupModel);

            return true;
        });
        // регионы <-

        // периоды ->
        var requestPeriods = $http({
            method: 'POST',
            url: '/GS/SummsPeriod_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.periods.length = 0;
            Array.prototype.push.apply($scope.periods, response.data.Data.periods);

            $scope.currentperiod = localStorage.getItem("currperiod") == null ? response.data.Data.periods[0] : localStorage.getItem("currperiod");

            //console.log($scope.periods);
            //console.log($scope.currentperiod);

            return true;
        });
        // периоды <-

        $scope.message = 'Пожалуйста, ожидайте... Запрос на округа, регионы, периоды';
        $scope.dataLoading = $q.all([requestDistricts, requestRegions, requestPeriods]).then(
            function (response) {
                console.debug("$scope.dataLoading success");

                return true;
            }, function (response) {
                console.error("$scope.dataLoading error");

                if (response.data == undefined)
                    messageBoxService.showError(response);
                else
                    errorHandlerService.showResponseError(response);

                return false;
            }
        );

        $scope.dataLoading.then(
            function (response) {
                console.debug(response);
            }
        ).catch(
            function (err) {
                console.error('Ошибка загрузки: ' + err);
            }
        );
    }

    $scope.canSearch = function () {
        if (($scope.selectByGroupModel === undefined) || ($scope.selectByGroupModel === null))
            return false;

        return ($scope.selectByGroupModel.length > 0) & $scope.IsSelectedPeriod();
    }
    $scope.IsSelectedPeriod = function () {
        if (($scope.currentperiod == undefined) || ($scope.currentperiod == null))
            return false
        else
            return true;
    }

    $scope.SalesSKUbySF_Search = function () {

        if ($scope.currentperiod == null)
            throw "Не определён текущий период"

        if ($scope.selectByGroupModel.length === 0)
            throw "Не выбраны регионы"

        // расчёт периода
        const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
        const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

        console.debug('$scope.Search -> year = ' + year);
        console.debug('$scope.Search -> month = ' + month);

        console.debug('$scope.Search -> region_model = ' + JSON.stringify($scope.selectByGroupModel));
        console.debug($scope.selectByGroupModel);

        var json_str = JSON.stringify({ year: year, month: month, region_model: JSON.stringify($scope.selectByGroupModel) });
        console.log('$scope.Search -> json_str = ' + json_str);

        $scope.message = 'Пожалуйста, ожидайте... Загрузка';
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/SalesSKUbySF/ViewSalesByGroupModel/',
            data: json_str
        }).then(function (response) {

            if (response.status === 200) {
                $scope.Grid.Options.data = response.data;

                localStorage.setItem("currperiod", $scope.currentperiod);
                localStorage.setItem("selregions", JSON.stringify($scope.selectByGroupModel));
            }
            else {
                console.error(response);
                messageBoxService.showError(response.statusText);
            }

        }, function (response) {
            console.error('errorHandlerService.showResponseError = ' + response);
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.RecalcDistrData = function () {
        messageBoxService.showConfirm('Вы уверены что хотите запустить расчёт данных дистрибьюторской отчётности?', 'Расчёт')
            .then(//да сохранить
                function (result) {
                    // расчёт периода
                    const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
                    const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/SalesSKUbySF/RecalcDistrData/',
                            data: JSON.stringify({ year: year, month: month })
                        }).then(function (response) {
                            var data = response.data;
                            if (data.Data.Success) {
                                $scope.SalesSKUbySF_Search();
                            }
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });
                },
                function (result) {
                    if (result === 'no') {//нет не сохранять
                    }
                    else {//отмена
                        var d = "отмена";
                    }

                });
    };

    $scope.RecalcInitialData = function () {
        messageBoxService.showConfirm('Вы уверены что хотите запустить расчёт исходных данных?', 'Расчёт')
            .then(//да сохранить
                function (result) {
                    // расчёт периода
                    const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
                    const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/SalesSKUbySF/RecalcInitialData/',
                            data: JSON.stringify({ year: year, month: month })
                        }).then(function (response) {
                            var data = response.data;
                            if (data.Data.Success) {
                                $scope.SalesSKUbySF_Search();
                            }
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });
                },
                function (result) {
                    if (result === 'no') {//нет не сохранять
                    }
                    else {//отмена
                        var d = "отмена";
                    }

                });
    };

    $scope.RecalcOFDData = function () {
        messageBoxService.showConfirm('Вы уверены что хотите запустить расчёт ОФД данных?', 'Расчёт')
            .then(//да сохранить
                function (result) {
                    // расчёт периода
                    const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
                    const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/SalesSKUbySF/RecalcOFDData/',
                            data: JSON.stringify({ year: year, month: month })
                        }).then(function (response) {
                            console.debug(response);
                            var data = response.data;
                            if (data.Data.Success) {
                                $scope.SalesSKUbySF_Search();
                            }
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });
                },
                function (result) {
                    if (result === 'no') {//нет не сохранять
                    }
                    else {//отмена
                        var d = "отмена";
                    }

                });
    };

    $scope.RecalcCalculatedData = function () {
        messageBoxService.showConfirm('Вы уверены что хотите запустить расчёт данных?', 'Расчёт')
            .then(//да сохранить
                function (result) {
                    // расчёт периода
                    const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
                    const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/SalesSKUbySF/RecalcCalculatedData/',
                            data: JSON.stringify({ year: year, month: month })
                        }).then(function (response) {
                            console.debug(response);
                            var data = response.data;
                            if (data.Data.Success) {
                                $scope.SalesSKUbySF_Search();
                            }
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });
                },
                function (result) {
                    if (result === 'no') {//нет не сохранять
                    }
                    else {//отмена
                        var d = "отмена";
                    }

                });
    };

    $scope.SalesSKUbySF_To_Excel = function () {
        const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
        const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

        var json_str = JSON.stringify({ year: year, month: month, region_model: JSON.stringify($scope.selectByGroupModel) });
        console.log('$scope.Search -> json_str = ' + json_str);

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/SalesSKUbySF/SalesSKUbySF_To_Excel/',
            data: json_str,
            headers: { 'Content-type': 'application/json' },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'SalesSKUbySF_' + $scope.currentperiod + '.xlsx';
            saveAs(blob, fileName);
        }, function (error) {
            messageBoxService.showError('Rejected:' + error);
        });
    }

    $scope.SalesSKUbySF_from_Excel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
                formData.append('currentperiod', $scope.currentperiod);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/SalesSKUbySF/SalesSKUbySF_from_Excel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.SalesSKUbySF_Search();
            }, function (response) {
                $scope.Grid.Options.data = [];
                let message = response.data.message;
                messageBoxService.showError(JSON.stringify(message));
            });
        }
    };

    $scope.Ratings_To_Excel = function () {

        if ($scope.currentperiod == null) {
            error = "Не определён текущий период";
            messageBoxService.showError(error);
            throw error
        }

        const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
        const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

        var json_str = JSON.stringify({ year: year, month: month });
        console.debug('$scope.Ratings_To_Excel -> json_str = ' + json_str);

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/SalesSKUbySF/Ratings_To_ExcelTask/',
            data: json_str,
            headers: { 'Content-type': 'application/json' },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'Рейтинги_' + $scope.currentperiod + '.xlsx';
            saveAs(blob, fileName);
        }, function (response) {
            console.error(response)

            let message = response.status + ' ' + response.statusText
            messageBoxService.showError(message);
        });
    }

}