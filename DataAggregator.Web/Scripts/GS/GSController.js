angular
    .module('DataAggregatorModule')
    .controller('GSController', [
        '$scope', '$route', '$http', '$q', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', 'uiGridTreeViewConstants', 'Upload', 'cfpLoadingBar', GSController]);

function GSController($scope, $route, $http, $q, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService, uiGridTreeViewConstants, Upload, cfpLoadingBar) {
    $scope.IsRowSelection = false;
    $scope.Title = "ГС";
    $scope.user = userService.getUser();
    ///////////////////////////////ГС Старт
    $scope.GS_Init = function () {
        $scope.format = 'dd.MM.yyyy';
        $scope.periods = [];
        $scope.spr_FormatLayout = [];
        $scope.spr_PharmacySellingPlaceType = [];
        $scope.spr_PointCategory = [];
        $scope.spr_WorkFormat = [];
        $scope.currentperiod = null;

        $scope.bool_select = [{ "Id": true, "name": "1" }, { "Id": false, "name": "0" }, { "Id": null, "name": "пусто" }];

        $scope.reestr_line_Grid = uiGridCustomService.createGridClassMod($scope, 'reestr_line_Grid');
        $scope.reestr_line_Grid.Options.showGridFooter = true;
        $scope.reestr_line_Grid.Options.multiSelect = true;
        $scope.reestr_line_Grid.Options.modifierKeysToMultiSelect = true;


        $scope.Calls_Grid = uiGridCustomService.createGridClassMod($scope, 'Calls_Grid');
        $scope.Calls_Grid.Options.showGridFooter = true;
        $scope.Calls_Grid.Options.multiSelect = true;
        $scope.Calls_Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Calls_Grid.Options.columnDefs = [
            { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'Инициатор', field: 'Creator_User', filter: { condition: uiGridCustomService.condition } },
            { name: 'Дата', field: 'Creator_Date', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { enableCellEdit: true, name: 'Результат', field: 'Result_text', filter: { condition: uiGridCustomService.condition } },
            { name: 'Дата результата', field: 'Result_Date', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Пользователь', field: 'Result_User', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Calls_Grid.SetDefaults();
        $scope.reestr_line_Grid.Options.columnDefs = [
            { name: 'Период', field: 'period', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            {
                enableCellEdit: true, name: 'Существование', field: 'isExists', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists==null}" ng-click="grid.appScope.isExists(row.entity,-1,null)">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists==false}" ng-click="grid.appScope.isExists(row.entity,-1,false)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists==true}" ng-click="grid.appScope.isExists(row.entity,-1,true)">1</button></div>'
            },
            { enableCellEdit: true, name: 'Имя Сети', field: 'NetworkName', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, name: '∑', field: 'Summa', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { enableCellEdit: false, name: 'Лицензии', field: 'periods_text', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: false, name: 'ЛицензииН', field: 'licenses_numbers_text', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.reestr_line_Grid.SetDefaults();

        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Grid.Options.columnDefs = [
            {
                headerTooltip: true, name: 'GSId', width: 100, field: 'Id', hasCustomWidth: false, headerCellClass: 'Red', filter: { condition: uiGridCustomService.numberCondition },
                cellTemplate: '<button ng-class="{\'btn-danger\' : row.entity.isExists==false,\'btn-info\' : row.entity.isExists==null,\'btn-success\' : row.entity.isExists==true}" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.GS_periods(row.entity.Id)"><span ng-class="{\'glyphicon glyphicon-usd\' : row.entity.Summa>0}" class=""></span><span ng-class="{\'glyphicon glyphicon-random\' : row.entity.isExists!=row.entity.isExists_p1}" class=""></span>{{row.entity.Id}}</button>'
            },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'PharmacyId', field: 'PharmacyId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, cellTooltip: true, width: 100, name: 'Прозвон', field: 'Calls', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Calls_show(row.entity.Id)" class=""></span>{{row.entity.Calls}}</button>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Коммент', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 300, name: 'ИНН юр. лица', field: 'EntityINN', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Organization?inn={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { headerTooltip: true, enableCellEdit: true, width: 300, name: 'Юр. лицо', field: 'EntityName', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 300, name: 'Бренд аптеки', field: 'PharmacyBrand', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, name: 'Адрес из лицензии', width: 300, field: 'Address', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'ФО', width: 100, field: 'Bricks_FederalDistrict', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'СФ', field: 'Address_region', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            //{ headerTooltip: true, name: 'СФ', width: 100, field: 'Bricks_FederationSubject', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'МР/ГО', width: 100, field: 'Bricks_City', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Индекс', field: 'Address_index', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'НП', field: 'Address_city', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, cellTooltip: true, width: 100, name: 'Адрес', field: 'Address_street', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Ориентир', field: 'Address_comment', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Этаж', field: 'Address_float', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: '№ пом.', field: 'Address_room', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'S пом., кв. м', field: 'Address_room_area', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, width: 100, name: 'Тип НП', field: 'Bricks_CityType', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Координаты', field: 'Address_koor', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Брик', field: 'BricksId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Bricks?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { headerTooltip: true, name: 'Текущий период', width: 100, field: 'period', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            {
                headerTooltip: true, enableCellEdit: true, name: '-3м', width: 100, field: 'isExists_p3', headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                //cellTemplate: '<select class="form-control" ng-class="{\'btn-info\' : row.entity.isExists_p3==null,\'btn-danger\' : row.entity.isExists_p3==false,\'btn-success\' : row.entity.isExists_p3==true}" ng-options="item as item.name for item in grid.appScope.bool_select track by item.Id" ng-model="row.entity.isExists_p3"></select>'
                //item as item.name for item in bool_seect track by item.value
                //cellTemplate: '<div ng-class="{\'btn-info\' : row.entity.isExists_p3==null,\'btn-danger\' : row.entity.isExists_p3==false,\'btn-success\' : row.entity.isExists_p3==true}">{{COL_FIELD}}</div>'
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists_p3==null}" ng-click="grid.appScope.isExists(row.entity,3,null)">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists_p3==false}" ng-click="grid.appScope.isExists(row.entity,3,false)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists_p3==true}" ng-click="grid.appScope.isExists(row.entity,3,true)">1</button></div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '-2м', width: 100, field: 'isExists_p2', headerCellClass: 'PaleGoldenrod', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists_p2==null}" ng-click="grid.appScope.isExists(row.entity,2,null)">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists_p2==false}" ng-click="grid.appScope.isExists(row.entity,2,false)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists_p2==true}" ng-click="grid.appScope.isExists(row.entity,2,true)">1</button></div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '-1м', width: 100, field: 'isExists_p1', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists_p1==null}" ng-click="grid.appScope.isExists(row.entity,1,null)">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists_p1==false}" ng-click="grid.appScope.isExists(row.entity,1,false)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists_p1==true}" ng-click="grid.appScope.isExists(row.entity,1,true)">1</button></div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Работает', width: 100, field: 'isExists', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists==null}" ng-click="grid.appScope.isExists(row.entity,0,null)">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists==false}" ng-click="grid.appScope.isExists(row.entity,0,false)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists==true}" ng-click="grid.appScope.isExists(row.entity,0,true)">1</button></div>'
            },


            { headerTooltip: true, enableCellEdit: true, name: '-3м Сеть', width: 100, field: 'NetworkName_p3', headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: '-2м Сеть', width: 100, field: 'NetworkName_p2', headerCellClass: 'PaleGoldenrod', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: '-1м Сеть', width: 100, field: 'NetworkName_p1', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Сеть', width: 100, field: 'NetworkName', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, name: 'Дата Добавления', width: 100, field: 'Date_Create', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { headerTooltip: true, enableCellEdit: false, name: 'Min Лицензия', width: 100, field: 'Lic_Period_First', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { headerTooltip: true, enableCellEdit: false, name: 'Max Лицензия', width: 100, field: 'Lic_Period', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            {
                headerTooltip: true, enableCellEdit: true, width: 100, name: 'Тип АУ', field: 'PharmacySellingPlaceType', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.spr_PharmacySellingPlaceType,
                editDropdownIdLabel: 'code', editDropdownValueLabel: 'status', editDropdownFilter: 'translate'
            },
            { headerTooltip: true, enableCellEdit: true, width: 300, name: 'Номер аптеки', field: 'PharmacyNumber', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Телефон', field: 'Phone', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Web', field: 'Website', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Режим работы', field: 'OperationMode', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, width: 100, name: 'Формат выкладки', field: 'FormatLayout', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.spr_FormatLayout,
                editDropdownIdLabel: 'code', editDropdownValueLabel: 'status', editDropdownFilter: 'translate'
            },
            {
                headerTooltip: true, enableCellEdit: true, width: 100, name: 'Формат работы', field: 'WorkFormat', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.spr_WorkFormat,
                editDropdownIdLabel: 'code', editDropdownValueLabel: 'status', editDropdownFilter: 'translate'
            },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'ФИО зав. аптеки', field: 'ContactPersonFullname', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, width: 100, name: 'Категория', field: 'PointCategoryId', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.spr_PointCategory,
                editDropdownIdLabel: 'code', editDropdownValueLabel: 'status'
            },
            { headerTooltip: true, enableCellEdit: true, width: 80, name: 'ПКУ', field: 'PKU', type: 'boolean' },
            { headerTooltip: true, enableCellEdit: true, width: 80, name: 'ECom', field: 'ECom', type: 'boolean' },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'ECom_License', field: 'ECom_License', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'сайты для e-com торговли', field: 'ECOM_WWW', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'приложение e-com', field: 'ECOM_mobileApp', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid.SetDefaults();
        return $http({
            method: 'POST',
            url: '/GS/GS_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.periods, response.data.Data.periods);
            Array.prototype.push.apply($scope.spr_FormatLayout, response.data.Data.spr_FormatLayout);
            Array.prototype.push.apply($scope.spr_PointCategory, response.data.Data.spr_PointCategory);
            Array.prototype.push.apply($scope.spr_PharmacySellingPlaceType, response.data.Data.spr_PharmacySellingPlaceType);
            Array.prototype.push.apply($scope.spr_WorkFormat, response.data.Data.spr_WorkFormat);
            $scope.currentperiod = response.data.Data.periods[0];

            //$scope.Manufacturer[dictionary + 'Id'] = null;

            var isSearch = false;
            var ids = $route.current.params["ids"];
            if (ids !== undefined) {
                $scope.filter.IDS = ids;
                isSearch = true;
            }
            var OperationMode = $route.current.params["OperationMode"];
            if (OperationMode !== undefined) {
                $scope.filter.OperationMode = OperationMode;
                isSearch = true;
            }
            var PHids = $route.current.params["PHids"];
            if (PHids !== undefined) {
                $scope.filter.PHids = PHids;
                isSearch = true;
            }
            var PharmacyBrand = $route.current.params["PharmacyBrand"];
            if (PharmacyBrand !== undefined) {
                $scope.filter.PharmacyBrand = PharmacyBrand;
                isSearch = true;
            }
            var NetworkName = $route.current.params["NetworkName"];
            if (NetworkName !== undefined) {
                $scope.filter.NetworkName = NetworkName;
                isSearch = true;
            }
            if (isSearch === true)
                $scope.GS_search();
            return response.data;
        });
    };
    $scope.isExists = function (item, type, value, column) {
        if (column !== undefined && column.colDef !== undefined) {
            if (column.colDef.enableCellEdit !== true)
                return;
        }
        if (type === -1) {
            $scope.Grid.GridCellsMod(item, "isExists", value);
            return;
        }
        if (type === 0 && item.isExists !== undefined)
            $scope.Grid.GridCellsMod(item, "isExists", value);
        if (type === 0 && item.isExists_p0 !== undefined)
            $scope.Grid.GridCellsMod(item, "isExists_p0", value);
        if (type === 1)
            $scope.Grid.GridCellsMod(item, "isExists_p1", value);
        if (type === 2)
            $scope.Grid.GridCellsMod(item, "isExists_p2", value);
        if (type === 3)
            $scope.Grid.GridCellsMod(item, "isExists_p3", value);
        if (type === 4)
            $scope.Grid.GridCellsMod(item, "isExists_p4", value);
        if (type === 5)
            $scope.Grid.GridCellsMod(item, "isExists_p5", value);
        if (type === 6)
            $scope.Grid.GridCellsMod(item, "isExists_p6", value);
        if (type === 7)
            $scope.Grid.GridCellsMod(item, "isExists_p7", value);
        if (type === 8)
            $scope.Grid.GridCellsMod(item, "isExists_p7", value);
        if (type === 9)
            $scope.Grid.GridCellsMod(item, "isExists_p9", value);
        if (type === 10)
            $scope.Grid.GridCellsMod(item, "isExists_p10", value);
        if (type === 11)
            $scope.Grid.GridCellsMod(item, "isExists_p11", value);
    };
    $scope.filter = {
        common: "",
        isNotChecked: false,
        BrickError: false,
        OperationMode: "",
        isNew: false,
        IDS: "",
        PHids: "",
        adress: "",
        BrickId: "",
        NetworkName: "",
        PharmacyBrand: "",
        isCloseOFD: false,
        isCloseAlphaBit: false,
        isDoubleA: false,
        isLicExists: false,
        isCall: false,
        dt: null,
        isDateAddLic: false,
        isSameAddressDiffCoords: false,
        isSameCoordsDiffAddress: false,
    };
    $scope.DT_filter = new dateClass();
    $scope.Get_currentperiod = function (val, inc) {

        var msUTC = new Date(Date.parse(val + '-15'));
        msUTC.setMonth(msUTC.getMonth() + inc);
        var ret = msUTC.toISOString().slice(0, 7);
        return ret;
    };
    $scope.GS_search_AC = function () {
        $scope.filter.dt = $scope.DT_filter.Value;
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/GS_search/',
                data: JSON.stringify({ filter: $scope.filter, currentperiod: $scope.currentperiod })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.Options.data = data.Data;
                    if ($scope.filter.isNew === true) {
                        $scope.Grid.SortColumn('Address_city', false, false);
                        $scope.Grid.SortColumn('Address_street', false, true);
                        $scope.Grid.SortColumn('Address_region', false, true);
                    }
                    $scope.GS_IsExists_Count();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.GS_search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.GS_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.GS_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.GS_search_AC();
        }

    };
    $scope.GS_SetEmpty = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/GS_SetEmpty/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.GS_search_AC();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });

    };
    $scope.GS_save = function (action) {
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/GS_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.GS_search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.GS_periods = function (Id) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/GS_periods/',
                data: JSON.stringify({ Id: Id })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.open_Periods(data.Data, Id);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.GS_delete = function () {
        messageBoxService.showConfirm('Удалить GSid?', 'Удаление')
            .then(
                function () {
                    var selectedRows = $scope.Grid.selectedRows();
                    if (selectedRows.length !== 1) {
                        alert("Выделено не 1 строка, я так не буду удалять.");
                        return;
                    }
                    selectedRows.forEach(function (item) {
                        var GSId = 0;
                        GSId = item.Id;
                        if (GSId > 0) {
                            $scope.dataLoading =
                                $http({
                                    method: 'POST',
                                    url: '/GS/GS_delete/',
                                    data: JSON.stringify({ GSId: GSId })
                                }).then(function (response) {
                                    var data = response.data;
                                    if (data.Success) {
                                        var PH = data.Data;
                                    }
                                }, function (response) {
                                    errorHandlerService.showResponseError(response);
                                });
                        }
                    });
                },
                function (result) {

                }
            );
    };
   
    $scope.YandexAddress = function () {
        var array_GSID = [];
        var selectedRows = $scope.Grid.selectedRows();
        if (selectedRows.length > 1) {
            alert("Выделено больше 1 строк, я так не буду показывать.");
            return;
        }
        var address = selectedRows[0].Address_region + " " + selectedRows[0].Bricks_City + " " + selectedRows[0].Address_city + " " + selectedRows[0].Address_street;
        //address = address + " Аптека";
        window.open('https://yandex.ru/maps/?text=' + address, '_blank');
    };
    $scope.YandexKoor = function () {
        var array_GSID = [];
        var selectedRows = $scope.Grid.selectedRows();
        if (selectedRows.length > 1) {
            alert("Выделено больше 1 строк, я так не буду показывать.");
            return;
        }
        var Address_koor = selectedRows[0].Address_koor;
        window.open('https://yandex.ru/maps/?text=' + Address_koor, '_blank');
    };
    $scope.GS_Merge = function () {
        messageBoxService.showConfirm('Объединить GSid?', 'Объединение')
            .then(
                function () {
                    var array_GSID = [];
                    var selectedRows = $scope.Grid.selectedRows();
                    if (selectedRows.length >= 5) {
                        alert("Выделено больше 4 строк, я так не буду объединять.");
                        return;
                    }
                    selectedRows.forEach(function (item) {
                        array_GSID.push(item.Id);
                    });
                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/GS/GS_Merge/',
                            data: JSON.stringify({ GSIds: array_GSID })
                        }).then(function (response) {
                            var data = response.data;
                            if (data.Success) {
                                var PH = data.Data;
                            }
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });

                },
                function (result) {

                }
            );
    };
    $scope.GS_Control = function () {
        var selectedRows = $scope.Grid.selectedRows();
        selectedRows.forEach(function (item) {
            if (item.UserControl_Name === "" || item.UserControl_Name === null) {
                $scope.Grid.GridCellsMod(item, "UserControl_Name", $scope.user.Name);
            }
        });
    };
    $scope.GS_IsExists = function (Per, value) {
        var selectedRows = $scope.Grid.selectedRows();
        selectedRows.forEach(function (item) {
            if (Per === 0)
                $scope.Grid.GridCellsMod(item, "isExists", value);
            if (Per === -1)
                $scope.Grid.GridCellsMod(item, "isExists_p1", value);
            if (Per === -2)
                $scope.Grid.GridCellsMod(item, "isExists_p2", value);
            if (Per === -3)
                $scope.Grid.GridCellsMod(item, "isExists_p3", value);
        });
        $scope.GS_IsExists_Count();
    };
    $scope.AGG = {
        isExists: 0,
        isExists_p1: 0,
        isExists_p2: 0,
        isExists_p3: 0
    };
    $scope.GS_IsExists_Count = function () {
        $scope.AGG.isExists = 0;
        $scope.AGG.isExists_p1 = 0;
        $scope.AGG.isExists_p2 = 0;
        $scope.AGG.isExists_p3 = 0;

        $scope.Grid.Options.data.forEach(function (item, i, arr) {
            if (item.isExists === true)
                $scope.AGG.isExists++;
            if (item.isExists_p1 === true)
                $scope.AGG.isExists_p1++;
            if (item.isExists_p2 === true)
                $scope.AGG.isExists_p2++;
            if (item.isExists_p3 === true)
                $scope.AGG.isExists_p3++;
        });
    };
    $scope.PharmacyId_new = function () {
        messageBoxService.showConfirm('Добавить новый PharmacyId?', 'Добавить')
            .then(
                function () {
                    var selectedRows = $scope.Grid.selectedRows();
                    selectedRows.forEach(function (item) {
                        var GSId = 0;
                        GSId = item.Id;
                        if (GSId > 0) {
                            $scope.dataLoading =
                                $http({
                                    method: 'POST',
                                    url: '/GS/PharmacyId_new/',
                                    data: JSON.stringify({ GSId: GSId })
                                }).then(function (response) {
                                    var data = response.data;
                                    if (data.Success) {
                                        var PH = data.Data;
                                        $scope.Grid.Options.data.forEach(function (item, i, arr) {
                                            if (item.Id === PH.GSId_first) {
                                                $scope.Grid.GridCellsMod(item, "PharmacyId", PH.PharmacyId);
                                            }
                                        });
                                    }
                                }, function (response) {
                                    errorHandlerService.showResponseError(response);
                                });
                        }
                    });
                },
                function (result) {

                }
            );
    };
    $scope.Calls_Add = function () {
        messageBoxService.showConfirm('Добавить в очередь Прозвона?', 'Добавить')
            .then(
                function () {
                    var array_GSID = [];
                    var selectedRows = $scope.Grid.selectedRows();
                    selectedRows.forEach(function (item) {
                        array_GSID.push(item.Id);
                    });
                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/GS/Calls_Add/',
                            data: JSON.stringify({ GSIds: array_GSID })
                        }).then(function (response) {
                            var data = response.data;
                            if (data.Success) {
                                var PH = data.Data;
                            }
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });

                },
                function (result) {

                }
            );
    };
    $scope.GS_ToExcel = function () {
        var array_upd = [];
        $scope.Grid.Options.data.forEach(function (item, i, arr) {
            array_upd.push(item.Id);
        });

        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GS/GS_ToExcel/',
            data: JSON.stringify({ ids: array_upd, currentperiod: $scope.currentperiod }),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'ГС.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.GS_FromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/GS_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
            }).then(function (response) {
                $scope.GS_search();
            }, function (response) {
                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };
    $scope.GS_Clone = function () {
        var array_GSID = [];
        var selectedRows = $scope.Grid.selectedRows();
        if (selectedRows.length === 1) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/GS_Clone/',
                    data: JSON.stringify({ GSId: selectedRows[0].Id, currentperiod: $scope.currentperiod })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        Array.prototype.push.apply($scope.Grid.Options.data, data.Data);
                        $scope.GS_IsExists_Count();
                    }

                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
        else { alert("Что-то не так выделено!"); }
    };
    $scope.GS_restore_From_changelog = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/GS_restore_From_changelog/',
                data: JSON.stringify({ GSIds: $scope.filter.IDS })
            }).then(function (response) {
                if (data.Success) {
                    $scope.GS_search();
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    //BEGIN Модальное окно параметров периодичности
    $scope.open_Periods = function (Data, Id) {
        $scope.reestr_line__header = Id;
        $scope.reestr_line_Grid.Options.data = Data;

        $('#modal_periods').modal('show');
    };
    $scope.GS_periods_Save = function () {
        var array_upd = $scope.reestr_line_Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/GS_periods_Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        alert("Сохранил");
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    //END Модальное окно параметров периодичности

    //BEGIN Модальное окно Прозвона
    $scope.Calls_show = function (Id) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Calls_show/',
                data: JSON.stringify({ Id: Id })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Calls_Apply(data.Data, Id);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Calls_Apply = function (Data, Id) {
        $scope.calls_header = Id;
        $scope.Calls_Grid.Options.data = Data;

        $('#modal_Calls').modal('show');
    };
    $scope.Calls_Save = function () {
        var array_upd = $scope.Calls_Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/Calls_Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        alert("Сохранил");
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    //END Модальное окно Прозвона

    //BEGIN Модальное окно Редактирование
    $scope.GS_Edit = undefined;
    $scope.Calls_show = function (Id) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Calls_show/',
                data: JSON.stringify({ Id: Id })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Calls_Apply(data.Data, Id);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Calls_Apply = function (Data, Id) {
        $scope.calls_header = Id;
        $scope.Calls_Grid.Options.data = Data;

        $('#modal_Calls').modal('show');
    };
    $scope.modal_GS_Edit_Save = function () {
        var array_upd = $scope.Calls_Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/Calls_Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        alert("Сохранил");
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    //END Модальное окно Редактирование
    ///////////////////////////////ГС Конец

    ///////////////////////////////Брики Старт
    $scope.FilterBricks = {
        ids: "",
        common: "",
        post_index: ""
    };
    $scope.Bricks_Init = function () {
        $scope.Bricks_Grid = uiGridCustomService.createGridClassMod($scope, 'Bricks_Grid');
        $scope.Bricks_Grid.Options.showGridFooter = true;
        $scope.Bricks_Grid.Options.multiSelect = true;
        $scope.Bricks_Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Bricks_Grid.Options.noUnselect = false;//,\'ng-isolate-scope\':1==1

        return $http({
            method: 'POST',
            url: '/GS/Bricks_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.Bricks_list_L7_label2 = response.data.L7_label2;
            $scope.Bricks_list_CityType = response.data.CityType;

            $scope.Bricks_list3 = [{ value: '~', label: '~' }];
            $scope.Bricks_list4 = [{ value: '~', label: '~' }];
            $scope.Bricks_list5 = [{ value: '~', label: '~' }];
            $scope.Bricks_list6 = [{ value: '~', label: '~' }];

            $scope.Bricks_Grid.Options.columnDefs = [
                { name: 'Id', width: 100, field: 'Id' },
                { enableCellEdit: true, width: 300, name: 'Комментарий', field: 'comment', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: true, width: 100, name: 'Индекс', field: 'post_index', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: false, visible: false, width: 50, name: 'Код МТЕ', field: 'L7_id', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: true, width: 300, name: 'Прав. МТЕ', field: 'L7_label', filter: { condition: uiGridCustomService.condition } },
                {
                    enableCellEdit: true, width: 300, name: 'Вид адреса', field: 'L7_label2', filter: { condition: uiGridCustomService.condition },
                    editableCellTemplate: 'ui-grid/dropdownEditor',
                    editDropdownOptionsArray: $scope.Bricks_list_L7_label2,
                    editDropdownIdLabel: 'code', editDropdownValueLabel: 'status', editDropdownFilter: 'translate'
                },
                { enableCellEdit: false, visible: false, width: 50, name: 'Код ТЕ', field: 'L6_id', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: true, width: 300, name: 'ТЕ (для Москвы, Севастополь уровень - районы, для СПб - округа)', field: 'L6_label', filter: { type: uiGridConstants.filter.SELECT, selectOptions: $scope.Bricks_list6 } },
                {
                    enableCellEdit: true, width: 300, name: 'Тип нас.пункта', field: 'CityType', filter: { condition: uiGridCustomService.condition },
                    editableCellTemplate: 'ui-grid/dropdownEditor',
                    editDropdownOptionsArray: $scope.Bricks_list_CityType,
                    editDropdownIdLabel: 'code', editDropdownValueLabel: 'status', editDropdownFilter: 'translate'
                },
                { enableCellEdit: false, visible: false, width: 50, name: 'Код БРИКа', field: 'L5_id', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: true, width: 300, name: 'Населенные пункты (для Москвы,  Севастополь уровень - округа, для СПб - районы)', field: 'L5_label', filter: { type: uiGridConstants.filter.SELECT, selectOptions: $scope.Bricks_list5 } },
                { enableCellEdit: false, visible: false, width: 50, name: 'Код района/округа', field: 'L4_id', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: true, width: 300, name: 'Муниципальный район/городской округ', field: 'L4_label', filter: { type: uiGridConstants.filter.SELECT, selectOptions: $scope.Bricks_list4 } },
                { enableCellEdit: false, visible: false, width: 50, name: 'Код региона', field: 'L3_id', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: false, width: 300, name: 'Субъект федерации', field: 'L3_label', filter: { type: uiGridConstants.filter.SELECT, selectOptions: $scope.Bricks_list3 } },
                { enableCellEdit: false, visible: false, width: 50, name: 'Код округа', field: 'L2_id', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: false, width: 300, name: 'Федеральный округ', field: 'L2_label', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: false, visible: false, width: 50, name: 'L1_id', field: 'L1_id', filter: { condition: uiGridCustomService.condition } },
                { enableCellEdit: false, width: 300, name: 'L1_label', field: 'L1_label', filter: { condition: uiGridCustomService.condition } }
            ];

            $scope.Bricks_Grid.SetDefaults();
            var ids = $route.current.params["ids"];
            if (ids !== undefined) {
                $scope.FilterBricks.ids = ids;
                $scope.FilterBricks.common = "";
                $scope.FilterBricks.post_index = "";
                $scope.Bricks_search();
            }

            return response.data;
        });

    };
    $scope.Bricks_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Bricks_search/',
                data: JSON.stringify({ filter: $scope.FilterBricks })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Bricks_Grid.Options.data = data.Data;
                    //
                    $scope.Bricks_list6.splice(0, $scope.Bricks_list6.length);
                    $scope.Bricks_list5.splice(0, $scope.Bricks_list5.length);
                    $scope.Bricks_list4.splice(0, $scope.Bricks_list4.length);
                    $scope.Bricks_list3.splice(0, $scope.Bricks_list3.length);

                    Array.prototype.push.apply($scope.Bricks_list6, data.L6_list);
                    Array.prototype.push.apply($scope.Bricks_list5, data.L5_list);
                    Array.prototype.push.apply($scope.Bricks_list4, data.L4_list);
                    Array.prototype.push.apply($scope.Bricks_list3, data.L3_list);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Bricks_search = function () {
        if ($scope.Bricks_Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Bricks_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Bricks_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Bricks_search_AC();
        }
    };
    $scope.Bricks_New = function (level) {

        var selectedRows = $scope.Bricks_Grid.selectedRows();
        var Id = 0;
        if (selectedRows.length >= 1) {
            Id = selectedRows[0].Id;
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/Bricks_new/',
                    data: JSON.stringify({ level: level, Id: Id })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        var rowEntity = data.Data;
                        $scope.Bricks_Grid.Options.data.push(rowEntity);
                        $scope.Bricks_Grid.FiltersClear();//++
                        $scope.Bricks_Grid.FilterSet('Id', rowEntity.Id);

                        $scope.Grid.GridCellsMod(rowEntity, "@modify", true);
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.Bricks_Delete = function () {
        var selectedRows = $scope.Bricks_Grid.selectedRows();
        if (selectedRows.length >= 1) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/Bricks_Delete/',
                    data: JSON.stringify({ array: selectedRows })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        $scope.Bricks_search_AC();
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.Bricks_save = function (action) {
        var array_upd = $scope.Bricks_Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/Bricks_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.Bricks_search_AC();
                        }
                        else {
                            $scope.Bricks_Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.Bricks_ToExcel = function () {
        /* var array_upd = [];
         $scope.Grid.Options.data.forEach(function (item, i, arr) {
             array_upd.push(item.Id);
         });*/

        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GS/Bricks_ToExcel/',
            data: JSON.stringify({ /*ids: array_upd, currentperiod: $scope.currentperiod*/ }),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'Брики.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    ///////////////////////////////Брики Окончание
    ///////////////////////////////БрикиTree Старт
    $scope.BrickRegion_Init = function () {
        $scope.Bricks_L3 = [];
        //$scope.Bricks_L3_Current = {};
        $scope.Bricks_L3_model = {};
        return $http({
            method: 'POST',
            url: '/GS/BrickRegion_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.Bricks_L3, response.data.Data.L3);


            var Id = $route.current.params["Id"];
            if (Id !== undefined) {
                $scope.BrickRegion_Change(Id);
            }
            else {
                $scope.Bricks_L3_model = $scope.Bricks_L3[0];
            }

            return response.data;
        });

    };
    $scope.BrickRegion_Change = function (Id) {
        $scope.Bricks_L3.forEach(function (item, index, array) {
            if (item.Id === Id) {
                $scope.Bricks_L3_model = item;
            }
        });
    };

    $scope.BrickRegion_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/BrickRegion_save/',
                data: JSON.stringify({ array: $scope.Bricks_L3 })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        // $scope.Bricks_search_AC();
                    }
                    else {
                        //$scope.Grid.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    ///////////////////////////////Брики Окончание
    ////////////////////////////// Лицензии Старт
    $scope.Licenses_Init = function () {
        $scope.Title = "Лицензии";
        $scope.filter_Licenses = {
            common: "",
            activity_type: "",
            adress: "",
            works: "<work>06.",
            date: Date((new Date()).getFullYear() - 1, 1, 1),
            isNew: false,
            withAdress: true
        };
        $scope.Grid_Licenses = uiGridCustomService.createGridClassMod($scope, 'Grid_Licenses');
        $scope.Grid_Licenses.Options.showGridFooter = true;
        $scope.Grid_Licenses.Options.multiSelect = true;
        $scope.Grid_Licenses.Options.modifierKeysToMultiSelect = true;
        $scope.Grid_Licenses.Options.columnDefs = [
            //cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_P}}" target="_blank">ссылка</a></div>'
            //type: 'date'
            { visible: false, name: 'Id', field: 'Id' },
            { width: 100, name: 'Наименование лицензирующего органа', field: 'name', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Лицензируемый вид деятельности', field: 'activity_type', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Полное наименование лицензиата', field: 'full_name_licensee', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Сокращенное наименование лицензиата', field: 'abbreviated_name_licensee', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Фирменное наименование лицензиата', field: 'brand_name_licensee', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Организационно-правовая форма', field: 'form', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Адрес места нахождения юридического лица либо адрес места жительства индивидуального предпринимателя', field: 'address', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'ОГРН/ОГРНИП лицензиата', field: 'ogrn', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'ИНН лицензиата', field: 'inn', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Номер лицензии', field: 'number', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Дата выдачи лицензии', field: 'date', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { width: 100, name: 'Номер приказа (распоряжения) о предоставлении лицензии', field: 'number_orders', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Дата приказа (распоряжения) о предоставлении лицензии', field: 'date_order', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { width: 100, name: 'Дата внесения записи в реестр', field: 'date_register', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { width: 100, name: 'Номер дубликата лицензии', field: 'number_duplicate', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Дата выдачи дубликата лицензии', field: 'date_duplicate', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { width: 100, name: 'Основание прекращения действия лицензии', field: 'termination', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Дата прекращения действия лицензии', field: 'date_termination', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { width: 100, name: 'Сведения о проведенных проверках', field: 'information', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Сведения о выданных постановлениях по результатам проверок.', field: 'information_regulations', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Сведения о приостановлении/возобновлении действия лицензии', field: 'information_suspension_resumption', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Сведения об аннулировании лицензии', field: 'information_cancellation', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Сведения о переоформлении лицензии', field: 'information_reissuing', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Адрес Точки', field: 'address_point', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Индекс', field: 'index', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Регион', field: 'region', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Город', field: 'city', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'Улица', field: 'street', filter: { condition: uiGridCustomService.condition } },
            { width: 100, name: 'оказываемые услуги', field: 'works', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.Grid_Licenses.SetDefaults();
        /*
         *       ,[address_point]
      ,[index]
      ,[region]
      ,[city]
      ,[street]
      ,[works]
         * return $http({
            method: 'POST',
            url: '/GS/Licenses/',
            data: JSON.stringify({})
        }).then(function (response) {
            //return response.data;
        });*/
        var searchtext = $route.current.params["searchtext"];
        if (searchtext !== undefined) {
            $scope.filter_Licenses.activity_type = "";
            $scope.filter_Licenses.common = searchtext;
            $scope.filter_Licenses.works = "";
            $scope.filter_Licenses.date = Date(2000, 1, 1);
            $scope.filter_Licenses.isNew = false;
            $scope.Licenses_search();
        }

    };
    $scope.Licenses_search = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Licenses_search/',
                data: JSON.stringify({ filter: $scope.filter_Licenses })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_Licenses.Options.data = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    ////////////////////////////// Лицензии Окончание

    $(window).keydown(function (event) {
        if (event.keyCode === 16) {
            setIsShift(true);
        }
    });

    $(window).keyup(function (event) {
        if (event.keyCode === 16) {
            setIsShift(false);
        }
    });

    function setIsShift(value) {
        if (value && !$('#grid-block').hasClass('disableSelection')) {
            $scope.selectedText = commonService.getSelectionText();
        }

        if (value) {
            $('#grid-block').addClass('disableSelection');
        } else {
            $('#grid-block').removeClass('disableSelection');
        }
    }

    ////////////////////////////// Адреса для Старт
    $scope.base_address_Init = function () {
        $scope.Title = "Адреса для базы";
        $scope.count_to_work = 0;
        $scope.count_not_in_GS = 0;
        $scope.count_in_GS = 0;
        $scope.count_isUse = 0;
        $scope.count_in_LPU = 0;
        $scope.count_in_DO = 0;
        $scope.count_in_NN = 0;
        $scope.filter_base_address = {
            common: "",
            toWork: true,
            in_GS: "-1",
            isUse: false
        };
        $scope.Grid_base_address = uiGridCustomService.createGridClassMod($scope, 'Grid_base_address');
        $scope.Grid_base_address.Options.showGridFooter = true;
        $scope.Grid_base_address.Options.multiSelect = true;
        $scope.Grid_base_address.Options.modifierKeysToMultiSelect = true;
        $scope.Grid_base_address.Options.customEnableRowSelection = true;
        $scope.Grid_base_address.Options.enableRowSelection = true;
        $scope.Grid_base_address.Options.enableRowHeaderSelection = false;
        $scope.Grid_base_address.Options.enableSelectAll = false;
        $scope.Grid_base_address.Options.selectionRowHeaderWidth = 20;
        $scope.Grid_base_address.Options.rowHeight = 20;
        $scope.Grid_base_address.Options.appScopeProvider = $scope;
        $scope.Grid_base_address.Options.enableFullRowSelection = true;
        $scope.Grid_base_address.Options.enableSelectionBatchEvent = true;
        $scope.Grid_base_address.Options.enableHighlighting = true;
        $scope.Grid_base_address.Options.noUnselect = false;

        $scope.Grid_base_address.Options.columnDefs = [
            //cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_P}}" target="_blank">ссылка</a></div>'
            //type: 'date'
            { enableCellEdit: false, visible: false, name: 'Id', field: 'Id' },
            { enableCellEdit: false, width: 100, name: 'Номер лицензии', field: 'number', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Licenses?searchtext={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { enableCellEdit: false, width: 100, name: 'ИНН лицензиата', field: 'inn', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Organization?inn={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { enableCellEdit: false, width: 100, name: 'Полное наименование лицензиата', field: 'full_name_licensee', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },

            { enableCellEdit: true, width: 100, name: 'Адрес', field: 'address', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, width: 100, name: 'Индекс', field: 'index', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, width: 100, name: 'Регион', field: 'region', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, width: 100, name: 'Город', field: 'city', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, width: 100, name: 'Улица', field: 'street', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, width: 100, name: 'BricksId', field: 'BricksId', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Bricks?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { enableCellEdit: false, width: 100, name: 'Дата выдачи лицензии', field: 'date', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { enableCellEdit: false, width: 100, name: 'Дата обнаружения', field: 'date_add', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { enableCellEdit: false, width: 100, name: 'works', field: 'works', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },

            { enableCellEdit: true, width: 100, name: 'IsUse', field: 'IsUse', type: 'boolean' },
            { enableCellEdit: true, width: 100, name: 'PharmacyId', field: 'PharmacyId', filter: { condition: uiGridCustomService.condition }, type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT },
            { enableCellEdit: false, width: 100, name: 'сотр в ГС', field: 'UserAppendToGS', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: false, width: 100, name: 'Дата в ГС', field: 'DateAppendToGS', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE }
        ];
        $scope.Grid_base_address.SetDefaults();

        var filter = $route.current.params["filter"];
        if (filter !== undefined) {
            $scope.filter_base_address.common = filter;
            $scope.filter_base_address.toWork = false;
            $scope.filter_base_address.in_GS = "-1";
            $scope.filter_base_address.isUse = false;
        }
        $scope.base_address_search();
    };
    $scope.base_address_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/base_address_search/',
                data: JSON.stringify({ filter: $scope.filter_base_address })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_base_address.Options.data = data.Data;
                    $scope.count_to_work = data.count_to_work;
                    $scope.count_not_in_GS = data.count_not_in_GS;
                    $scope.count_in_GS = data.count_in_GS;
                    $scope.count_isUse = data.count_isUse;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.base_address_search = function () {
        if ($scope.Grid_base_address.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.base_address_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.base_address_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.base_address_search_AC();
        }

    };

    $scope.base_address_To_GS = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/base_address_To_GS/'
            }).then(function (response) {
                $scope.base_address_search();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.CreateLPUId = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/CreateLPUId/'
            }).then(function (response) {
                $scope.base_address_search();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.base_address_IsUse = function (value) {
        $scope.Grid_base_address.selectedRows().forEach(function (item) {
            $scope.Grid_base_address.GridCellsMod(item, "IsUse", value);
        });
    };
    $scope.base_address_save = function (action) {
        var array_upd = $scope.Grid_base_address.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/base_address_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    $scope.Grid_base_address.ClearModify();

                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.base_address_search_AC();
                        }
                        else {
                            $scope.Grid_base_address.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.base_address_inn = function () {
        var array_upd = [];
        $scope.Grid_base_address.selectedRows.forEach(function (item) {
            array_upd.push(item.inn);
        });
        if (array_upd.length > 0) {
            $scope.Organizaion_block(array_upd, false);
        }
    };

    $scope.licenses_to_Use_BrickIdSet = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/licenses_to_Use_BrickIdSet/'
            }).then(function (response) {
                $scope.base_address_search();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.base_address_ToExcel = function () {
        var array_upd = [];
        $scope.Grid_base_address.Options.data.forEach(function (item, i, arr) {
            array_upd.push(item.Id);
        });

        $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GS/base_address_ToExcel/',
            data: JSON.stringify({ ids: array_upd }),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'База адресов.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.base_address_FromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/base_address_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.base_address_search();
            }, function () {
                $scope.Grid_base_address.Options.data = [];
                $scope.message = 'Unexpected error';
                messageBoxService.showError($scope.message);
            });
        }
    };
    ////////////////////////////// Адреса для базы Окончание

    ////////////////////////////// Организации для Старт
    $scope.OrganizationInit = function () {
        $scope.Title = "Организации";
        $scope.IsRowSelection = false;
        $scope.count_to_work = 0;
        $scope.count_in_GS = 0;
        $scope.count_not_in_GS = 0;
        $scope.count_withNull = 0;
        $scope.count_IsNotCheck = 0;
        $scope.count_IsErrors = 0;
        $scope.FilterOrganization = {
            ids: "",
            inn: "",
            common: "",
            toWork: true,
            withNull: false,
            IsNotCheck: false,
            IsErrors: false,
            in_GS: "0"
        };
        $scope.Grid_Organization = uiGridCustomService.createGridClassMod($scope, 'Grid_Organization');
        $scope.Grid_Organization.Options.columnDefs = [
            { enableCellEdit: false, visible: false, name: 'Id', field: 'Id' },
            {
                enableCellEdit: false, width: 100, name: 'ИНН', field: 'inn', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/base_address?filter={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { enableCellEdit: true, width: 100, name: 'ActualId', field: 'ActualId', filter: { condition: uiGridCustomService.numberCondition } },
            { enableCellEdit: false, width: 100, name: 'Полное наименование', field: 'full_name', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Короткое наименование', field: 'name', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'BricksId', field: 'BricksId', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Наименование ГС', field: 'EntityName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },

            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Описание', field: 'description', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Адрес', field: 'address', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'ogrn', field: 'ogrn', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: false, width: 100, name: 'Форма', field: 'form', filter: { condition: uiGridCustomService.condition } },
            { allowCellFocus: true, enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'ТипФС', field: 'EntityType', filter: { condition: uiGridCustomService.condition } },

            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Проверено', field: 'IsCheck', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },

            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'можно в ГС', field: 'IsUseGS', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'можно в МУ', field: 'IsUseLPU', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Наименование сети', field: 'NetWorkName', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Тип ЛПУ', field: 'TypeOf', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Вид ЛПУ', field: 'VidOf', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Бренд', field: 'Brand', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: false, width: 100, name: 'FIO', field: 'FIO', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Boss_Name', field: 'Boss_Name', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Boss_Position', field: 'Boss_Position', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Phone', field: 'Phone', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Date_registration', field: 'Date_registration', type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { enableCellEdit: false, width: 100, name: 'Status', field: 'Status', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Date_licvidation', field: 'Date_licvidation', type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { enableCellEdit: false, width: 100, name: 'Vid_Action', field: 'Vid_Action', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Info', field: 'Info', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint }
        ];
        $scope.Grid_Organization.SetDefaults();
        var searchtext = $route.current.params["searchtext"];
        var searchinn = $route.current.params["inn"];
        if (searchtext !== undefined)
            $scope.FilterOrganization.common = searchtext;
        if (searchinn !== undefined)
            $scope.FilterOrganization.inn = searchinn;
        if (searchtext !== undefined || searchinn !== undefined) {
            $scope.FilterOrganization.toWork = false;
            $scope.FilterOrganization.withNull = false;
            $scope.FilterOrganization.in_GS = "10";
        }
        $scope.Organization_search();
    };
    $scope.Organization_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Organization_search/',
                data: JSON.stringify({ filter: $scope.FilterOrganization })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_Organization.Options.data = data.Data;

                    //ui-i18n="ru"
                    $scope.count_to_work = data.count_to_work;
                    $scope.count_in_GS = data.count_in_GS;
                    $scope.count_in_LPU = data.count_in_LPU;
                    $scope.count_in_DO = data.count_in_DO;
                    $scope.count_in_NN = data.count_in_NN;
                    $scope.count_not_in_GS = data.count_not_in_GS;
                    $scope.count_withNull = data.count_withNull;
                    $scope.count_IsNotCheck = data.count_IsNotCheck;
                    $scope.count_IsErrors = data.count_IsErrors;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Organizaion_block = function (inns, value) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Organization_BlockGS/',
                data: JSON.stringify({ inns: inns, value: value })
            }).then(function (response) {
                var data = response.data;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Organization_search = function () {
        if ($scope.Grid_Organization.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Organization_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Organization_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Organization_search_AC();
        }

    };
    $scope.Organization_save = function (action) {
        var array_upd = $scope.Grid_Organization.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/Organization_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.Organization_search_AC();
                        }
                        else {
                            $scope.Grid_Organization.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.Organization_FromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/Organization_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
            }).then(function (response) {
                $scope.Organization_search();
            }, function (response) {
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };
    $scope.Organization_IsUseGS = function (value) {
        //alert('Запрещено');//Считаем что этот тип является образующим и тут не устанавливается.
        setIsShift(false);
        $scope.Grid_Organization.selectedRows().forEach(function (item) {
            $scope.Grid_Organization.GridCellsMod(item, "IsUseGS", value);
        });
    };
    $scope.Organization_IsCheck = function (value) {
        //alert('Запрещено');//Считаем что этот тип является образующим и тут не устанавливается.
        setIsShift(false);
        $scope.Grid_Organization.selectedRows().forEach(function (item) {
            $scope.Grid_Organization.GridCellsMod(item, "IsCheck", value);
        });
    };
    $scope.Spark_To = function () {
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/Spark_To/',
            data: JSON.stringify({ TP: $scope.FilterOrganization.in_GS }),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/zip' });
            var fileName = 'В спарк.zip';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.Spark_From = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/Spark_From/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {//Все хорошо

            }, function (response) {//Все плохо
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };

    ////////////////////////////// Организации для Окончание
    ////////////////////////////// Организации без ИНН для Старт
    $scope.Organization_without_INNInit = function () {
        $scope.Title = "Организации";
        $scope.IsRowSelection = false;
        $scope.count_to_work = 0;
        $scope.count_in_GS = 0;
        $scope.count_not_in_GS = 0;
        $scope.count_withNull = 0;
        $scope.count_IsNotCheck = 0;
        $scope.count_IsErrors = 0;
        $scope.FilterOrganization = {
            ids: "",
            inn: "",
            common: "",
            toWork: true,
            withNull: false,
            IsNotCheck: false,
            IsErrors: false,
            in_GS: "0"
        };
        $scope.Grid_Organization_without_INN = uiGridCustomService.createGridClassMod($scope, 'Grid_Organization_without_INN');
        $scope.Grid_Organization_without_INN.Options.columnDefs = [
            { enableCellEdit: false, visible: false, name: 'Id', field: 'Id' },
            { enableCellEdit: true, width: 100, name: 'ИНН', field: 'inn', headerCellClass: 'cssEdit', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, width: 100, name: 'ActualId', field: 'ActualId', filter: { condition: uiGridCustomService.numberCondition } },
            { enableCellEdit: false, width: 100, name: 'Полное наименование', field: 'full_name', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Короткое наименование', field: 'name', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'BricksId', field: 'BricksId', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Наименование ГС', field: 'EntityName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },

            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Описание', field: 'description', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, width: 100, name: 'Адрес', field: 'address', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'ogrn', field: 'ogrn', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Форма', field: 'form', filter: { condition: uiGridCustomService.condition } },
            { allowCellFocus: true, enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'ТипФС', field: 'EntityType', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Проверено', field: 'IsCheck', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'можно в МУ', field: 'IsUseLPU', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Наименование сети', field: 'NetWorkName', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Тип ЛПУ', field: 'TypeOf', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Вид ЛПУ', field: 'VidOf', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Бренд', field: 'Brand', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'FIO', field: 'FIO', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Boss_Name', field: 'Boss_Name', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Boss_Position', field: 'Boss_Position', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Phone', field: 'Phone', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Date_registration', field: 'Date_registration', type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Status', field: 'Status', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: false, headerCellClass: 'cssEdit', width: 100, name: 'WWW', field: 'WWW', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Date_licvidation', field: 'Date_licvidation', type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Vid_Action', field: 'Vid_Action', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, headerCellClass: 'cssEdit', width: 100, name: 'Info', field: 'Info', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint }
        ];
        $scope.Grid_Organization_without_INN.SetDefaults();
        var searchtext = $route.current.params["searchtext"];
        var searchinn = $route.current.params["inn"];
        if (searchtext !== undefined)
            $scope.FilterOrganization.common = searchtext;
        if (searchinn !== undefined)
            $scope.FilterOrganization.inn = searchinn;
        if (searchtext !== undefined || searchinn !== undefined) {
            $scope.FilterOrganization.toWork = false;
            $scope.FilterOrganization.withNull = false;
            $scope.FilterOrganization.in_GS = "10";
        }
        $scope.Organization_without_INN_search();
    };
    $scope.Organization_without_INN_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Organization_without_INN_search/',
                data: JSON.stringify({ filter: $scope.FilterOrganization })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_Organization_without_INN.Options.data = data.Data;

                    //ui-i18n="ru"
                    $scope.count_to_work = data.count_to_work;
                    $scope.count_in_GS = data.count_in_GS;
                    $scope.count_in_LPU = data.count_in_LPU;
                    $scope.count_in_DO = data.count_in_DO;
                    $scope.count_in_NN = data.count_in_NN;
                    $scope.count_not_in_GS = data.count_not_in_GS;
                    $scope.count_withNull = data.count_withNull;
                    $scope.count_IsNotCheck = data.count_IsNotCheck;
                    $scope.count_IsErrors = data.count_IsErrors;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Organization_without_INN_search = function () {
        if ($scope.Grid_Organization_without_INN.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Organization_without_INN_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Organization_without_INN_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Organization_without_INN_search_AC();
        }

    };
    $scope.Organization_without_INN_save = function (action) {
        var array_upd = $scope.Grid_Organization_without_INN.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/Organization_without_INN_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.Organization_without_INN_search_AC();
                        }
                        else {
                            $scope.Grid_Organization_without_INN.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.Organization_without_INN_FromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/Organization_without_INN_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
            }).then(function (response) {
                $scope.Organization_without_INN_search();
            }, function (response) {
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };




    ////////////////////////////// Организации без ИНН для Окончание

    ////////////////////////////// SummsAlphaBit для Старт
    $scope.SummsAlphaBit_Init = function () {
        $scope.format = 'dd.MM.yyyy';
        $scope.periods = [];
        $scope.currentperiod = null;
        $scope.isDoubles = false;
        $scope.isDoublesAdd = false;

        $scope.bool_select = [{ "Id": true, "name": "1" }, { "Id": false, "name": "0" }, { "Id": null, "name": "пусто" }];

        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Grid.afterCellEdit = function (item, colDef, newValue, oldValue) {
            if (colDef.field === 'Comment') {
                if (newValue !== '' && newValue !== null) {
                    $scope.Grid.GridCellsMod(item, "IsUse", false);
                }
            }
        };


        $scope.Grid.Options.columnDefs = [
            {
                visible: false,
                headerTooltip: true, name: 'Id', width: 100, field: 'Id', hasCustomWidth: false, headerCellClass: 'Red', filter: { condition: uiGridCustomService.numberCondition },
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Поставщик', field: 'Supplier', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'PharmacyId', field: 'PharmacyId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.numberCondition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?PHids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Коммент', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, periodIndex: 1, periodName: 'Коммент', name: '-1м Коммент', field: 'Comment_p1', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, periodIndex: 2, periodName: 'Коммент', name: '-2м Коммент', field: 'Comment_p2', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, enableCellEdit: true, name: 'IsUse', width: 100, field: 'IsUse', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.IsUse==false}" ng-click="grid.appScope.SummsAlphaBit_IsUse(row.entity,false,0)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.IsUse==true}" ng-click="grid.appScope.SummsAlphaBit_IsUse(row.entity,true,0)">1</button></div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'IsUse_p1', periodIndex: 1, periodName: 'IsUse', width: 100, field: 'IsUse_p1', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.IsUse_p1==false}" ng-click="grid.appScope.SummsAlphaBit_IsUse(row.entity,false,1)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.IsUse_p1==true}" ng-click="grid.appScope.SummsAlphaBit_IsUse(row.entity,true,1)">1</button></div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'IsUse_p2', periodIndex: 2, periodName: 'IsUse', width: 100, field: 'IsUse_p2', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.IsUse_p2==false}" ng-click="grid.appScope.SummsAlphaBit_IsUse(row.entity,false,2)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.IsUse_p2==true}" ng-click="grid.appScope.SummsAlphaBit_IsUse(row.entity,true,2)">1</button></div>'
            },
            { headerTooltip: true, name: 'Текущий период', width: 100, field: 'Period', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'СФ', field: 'Address_region', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'НП', field: 'Address_city', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, cellTooltip: true, width: 100, name: 'Адрес', field: 'Address_street', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },

            //cellTemplate: '<div style="height:100%;text-align:center" ng-class="{\'btn-danger\' : row.entity.RealSellingSum<row.entity.RealSellingSum_p1,\'btn-warning\' : row.entity.RealSellingSum_p1===null,\'btn-success\' : row.entity.RealSellingSum>row.entity.RealSellingSum_p1 && row.entity.RealSellingSum_p1>0}">{{row.entity.RealSellingSum}} ({{(100*(row.entity.RealSellingSum-row.entity.RealSellingSum_p1)/row.entity.RealSellingSum).toFixed()}}%)</div > '

            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, periodIndex: 2, periodName: 'Сеть', name: '-2м Сеть', width: 100, field: 'NetworkName_p2', headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition }
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, periodIndex: 1, periodName: 'Сеть', name: '-1м Сеть', width: 100, field: 'NetworkName_p1', headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, periodIndex: 0, periodName: 'Сеть', width: 100, field: 'NetworkName', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Поставщик префикс', field: 'Supplier_Add', filter: { condition: uiGridCustomService.condition } },

            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 2, periodName: 'Работает', name: '-2м Работает', width: 100, field: 'isExists_p2', headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists_p2==null}">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists_p2==false}">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists_p2==true}">1</button></div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 1, periodName: 'Работает', name: '-1м Работает', width: 100, field: 'isExists_p1', headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists_p1==null}">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists_p1==false}">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists_p1==true}">1</button></div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 0, periodName: 'Работает', name: '-0м Работает', width: 100, field: 'isExists', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists==null}">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists==false}">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists==true}">1</button></div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 12, periodName: '∑', name: '-12м ∑', width: 100, field: 'RealSellingSum_p12', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p12==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 11, periodName: '∑', name: '-11м ∑', width: 100, field: 'RealSellingSum_p11', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p11==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 10, periodName: '∑', name: '-10м ∑', width: 100, field: 'RealSellingSum_p10', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p10==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 9, periodName: '∑', name: '-9м ∑', width: 100, field: 'RealSellingSum_p9', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p9==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 8, periodName: '∑', name: '-8м ∑', width: 100, field: 'RealSellingSum_p8', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p8==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 7, periodName: '∑', name: '-7м ∑', width: 100, field: 'RealSellingSum_p7', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p7==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 6, periodName: '∑', name: '-6м ∑', width: 100, field: 'RealSellingSum_p6', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p6==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 5, periodName: '∑', name: '-5м ∑', width: 100, field: 'RealSellingSum_p5', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p5==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 4, periodName: '∑', name: '-4м ∑', width: 100, field: 'RealSellingSum_p4', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p4==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 3, periodName: '∑', name: '-3м ∑', width: 100, field: 'RealSellingSum_p3', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p3==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 2, periodName: '∑', name: '-2м ∑', width: 100, field: 'RealSellingSum_p2', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p2==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 1, periodName: '∑', name: '-1м ∑', width: 100, field: 'RealSellingSum_p1', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile_p1==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, periodIndex: 0, periodName: '∑', name: '-0м ∑', width: 100, field: 'RealSellingSum', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents"> <div title="Из файла" ng-class="{\'triangle-topright\' : row.entity.RealSellingSumFromFile==1}"></div>{{COL_FIELD | number : 0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, name: 'Дней недозагруза', width: 100, field: 'TotalUnloadDays', headerCellClass: 'PapayaWhip'
                , type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM
            },
            { headerTooltip: true, enableCellEdit: false, name: 'КОФ', width: 100, field: 'KOF', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'Исп.Послед ∑', width: 100, field: 'LastSellingSum_IsUse', headerCellClass: 'LightCyan', type: 'boolean', filter: { condition: uiGridCustomService.booleanConditionX } },
            { headerTooltip: true, enableCellEdit: false, name: 'Послед ∑', width: 100, field: 'LastSellingSum', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'delta ∑', width: 100, field: 'DeltaSellingSum', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'ОФД ∑', width: 100, field: 'OFD_Sum', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'ОФД тран', width: 100, field: 'OFD_Tran', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'ОФД %', width: 100, field: 'OFD_delta', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM }
        ];
        $scope.Grid.SetDefaults();
        return $http({
            method: 'POST',
            url: '/GS/SummsAlphaBit_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.periods, response.data.Data.periods);
            $scope.currentperiod = response.data.Data.periods[0];

            var ids = $route.current.params["ids"];
            if (ids !== undefined) {
                $scope.filter.IDS = ids;
                $scope.SummsAlphaBit_search();
            }
            return response.data;
        });
    };
    $scope.SetColumnPeriod = function (Data1) {
        $scope.Grid.Options.columnDefs.forEach(function (item, i, arr) {
            if (item.periodIndex >= 0) {
                item.displayName = Data1[item.periodIndex] + ' ' + item.periodName;
            }
        });
        if ($scope.Grid.notifyDataChange !== undefined)
            $scope.Grid.notifyDataChange(uiGridConstants.dataChange.COLUMN);
    };
    $scope.SummsAlphaBit_IsUse = function (item, value, period) {
        if (item !== null) {
            if (period === 0 && item.IsUse !== null) {
                $scope.Grid.GridCellsMod(item, "IsUse", value);
            }
            if (period === 1 && item.IsUse_p1 !== null) {
                $scope.Grid.GridCellsMod(item, "IsUse_p1", value);
            }
            if (period === 2 && item.IsUse_p2 !== null) {
                $scope.Grid.GridCellsMod(item, "IsUse_p2", value);
            }
        }
        else {

            $scope.Grid.selectedRows().forEach(function (item) {
                $scope.Grid.GridCellsMod(item, "IsUse", value);
            });
        }
    };
    $scope.SummsAlphaBit_Comment = function (value) {
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Comment", value);
            if (value !== '')
                $scope.Grid.GridCellsMod(item, "IsUse", false);
        });
    };

    $scope.SummsAlphaBit_SetLastSellingSum = function (value) {
        $scope.Grid.selectedRows().forEach(function (item) {
            if (value == true) {
                $scope.Grid.GridCellsMod(item, "RealSellingSum", item.LastSellingSum);
                $scope.Grid.GridCellsMod(item, "LastSellingSum_IsUse", true);
            }
            else if (value == false) {
                $scope.Grid.GridCellsMod(item, "RealSellingSum", null);
                $scope.Grid.GridCellsMod(item, "LastSellingSum_IsUse", false);
            }
        });
    };

    $scope.SummsAlphaBit_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsAlphaBit_search/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod, isDoubles: $scope.isDoubles, isDoublesAdd: $scope.isDoublesAdd })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.SetColumnPeriod(data.Data2);
                    $scope.Grid.Options.data = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SummsAlphaBit_search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.SummsAlphaBit_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.SummsAlphaBit_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.SummsAlphaBit_search_AC();
        }

    };
    $scope.SummsAlphaBit_recalc = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsAlphaBit_recalc/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod })
            }).then(function (response) {
                $scope.SummsAlphaBit_search_AC();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SummsAlphaBit_save = function (action) {
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/SummsAlphaBit_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.SummsAlphaBit_search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };

    $scope.NeoFarm_from_Excel = function (file) {

        if (file == null)
            return;

        cfpLoadingBar.start();

        var upload = Upload.upload({
            url: '/GS/AlphaBitSums_from_Excel/',
            data: {
                uploads: file,
                supplier: 'Нео-Фарм (Москва)',
                currentperiod: $scope.currentperiod
            }
        }).then(function (resp) {
            console.log('Success ' + resp.config.data.uploads.name + 'uploaded. Response: ' + resp.data);
            cfpLoadingBar.complete();
            $scope.message = "Загрузка"
        }, function (resp) {
            errorHandlerService.showResponseError(resp);
            cfpLoadingBar.complete();
            $scope.message = "Загрузка"
        }, function (evt) {
            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            cfpLoadingBar.set(progressPercentage / 100);
            $scope.message = "Загрузка файла " + progressPercentage + "%"
            if (progressPercentage >= 100)
                $scope.message = "Обработка файла"
        });

        $scope.dataLoading = $q.all([upload])
    };

    ////////////////////////////// SummsAlphaBit для Окончание

    ////////////////////////////////// History_Init Начало
    $scope.History_Init = function () {
        $scope.format = 'dd.MM.yyyy';
        $scope.UsersCL = "";
        //для поиска в данных
        $scope.filter = {
            spr_top: 300,
            spr_Source_client: "",
            spr_Category: 0,
            spr_Comments: "",
            spr_DataSource: "",
            spr_Spec: "",
            spr_Status: -5,
            top: 300,
            Source_client: "",

            DataSourceType: "",
            spr_DataSourceType: "",

            Category: 10,
            Comments: "",
            DataSource: "",
            Spec: "",
            NetworkName: "",
            Status: 255,
            text: "",
            INN: "",
            Address: "",
            GSIDs: "",
            Ids: "",
            PharmacyIDs: ""
        };
        $scope.spr_Category = [];
        $scope.spr_DataSource = [];
        $scope.spr_Spec = [];
        $scope.spr_Comments = [];
        $scope.spr_Status = [];
        $scope.spr_Source_client = [];
        $scope.spr_DataSourceType = [];
        $scope.spr_Tops = [];
        $scope.spr_Users = [];
        $scope.HaveData = false;
        $scope.IsGSwork = false;
        $scope.IsTypeClient = false;
        $scope.IsOnline = false;

        if ($route.current.params["TypeClient"] === "1") {
            $scope.IsTypeClient = true;
            $scope.IsOnline = true;
        }
        else {
            $scope.IsGSwork = true;
        }

        //для поиска в справочнике
        $scope.Search_text = "";
        $scope.Source_client = "";
        $scope.GSId = 0;
        $scope.PharmacyId = 0;
        $scope.Category = 0;
        $scope.CopyObjectRow = null;
        $scope.NetworkName = "";

        hotkeys.bindTo($scope).add({
            combo: 'shift+f',
            description: 'Поиск по справочнику',
            callback: function (event) {
                $scope.History_shiftF(event);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+z',
            description: 'Заведение',
            callback: function (event) {
                $scope.History_Set_Status(2);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+x',
            description: 'Мусор',
            callback: function (event) {
                $scope.History_Set_Status(5);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+c',
            description: 'Копировать',
            callback: function (event) {
                $scope.History_Copy();
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+v',
            description: 'Вставить',
            callback: function (event) {
                $scope.History_Paste();
            }
        });
        $scope.Grid_SPR = uiGridCustomService.createGridClassMod($scope, 'Grid_SPR', null, "Grid_SPR_dblClick");

        $scope.Grid_SPR.Options.showGridFooter = false;
        $scope.Grid_SPR.Options.multiSelect = true;
        $scope.Grid_SPR.Options.modifierKeysToMultiSelect = true;
        $scope.Grid_SPR.Options.customEnableRowSelection = true;
        $scope.Grid_SPR.Options.enableRowSelection = true;
        $scope.Grid_SPR.Options.enableRowHeaderSelection = false;
        $scope.Grid_SPR.Options.enableSelectAll = false;
        $scope.Grid_SPR.Options.selectionRowHeaderWidth = 20;
        $scope.Grid_SPR.Options.rowHeight = 20;
        $scope.Grid_SPR.Options.appScopeProvider = $scope;
        $scope.Grid_SPR.Options.enableFullRowSelection = true;
        $scope.Grid_SPR.Options.enableSelectionBatchEvent = true;
        $scope.Grid_SPR.Options.enableHighlighting = true;
        $scope.Grid_SPR.Options.noUnselect = false;

        $scope.Grid_SPR.Options.columnDefs = [
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'GSId', field: 'GSId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'LPUId', field: 'LPUId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/LPU?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'DistrId', field: 'DistrId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Distr?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'PharmacyId', field: 'PharmacyId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?PHids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Адрес из лицензии', field: 'Address', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ИНН юр. лица', field: 'EntityINN', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'LightCyan', name: 'Юр. лицо', field: 'EntityName', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'СФ', field: 'Address_region', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'НП', field: 'Address_city', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Индекс', field: 'Address_index', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Адрес', field: 'Address_street', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Ориентир', field: 'Address_comment', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Этаж', field: 'Address_float', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: '№ пом.', field: 'Address_room', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Имя Сети', field: 'NetworkName', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Номер аптеки', field: 'PharmacyNumber', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Бренд аптеки', field: 'PharmacyBrand', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Брик', field: 'BricksId', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, name: 'Существование', field: 'isExists', filter: { condition: uiGridCustomService.booleanCondition }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><div class="btn btn-xs" ng-class="{\'btn-info\' : row.entity.isExists==null}">x</div><div class="btn btn-xs" ng-class="{\'btn-danger\' : row.entity.isExists==false}">0</div><div class="btn btn-xs" ng-class="{\'btn-success\' : row.entity.isExists==true}">1</div></div>'
            }
        ];
        $scope.Grid_SPR.SetDefaults();


        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Grid.Options.customEnableRowSelection = true;
        $scope.Grid.Options.enableRowSelection = true;
        $scope.Grid.Options.enableRowHeaderSelection = false;
        $scope.Grid.Options.enableSelectAll = false;
        $scope.Grid.Options.selectionRowHeaderWidth = 20;
        $scope.Grid.Options.rowHeight = 20;
        $scope.Grid.Options.appScopeProvider = $scope;
        $scope.Grid.Options.enableFullRowSelection = true;
        $scope.Grid.Options.enableSelectionBatchEvent = true;
        $scope.Grid.Options.enableHighlighting = true;
        $scope.Grid.Options.noUnselect = true;

        $scope.Grid.afterCellEdit = function (rowEntity, colDef, newValue, oldValue) {
            if (colDef.field === 'NetworkName' || colDef.field === 'Address_region' || colDef.field === 'Address_city' || colDef.field === 'Address_street' || colDef.field === 'EntityINN'
                || colDef.field === 'EntityName' || colDef.field === 'PharmacyBrand' || colDef.field === 'BricksId') {
                if (rowEntity["GSId"] > 1000000000)
                    $scope.Grid.GridCellsMod(rowEntity, "GSId", -55);
            }
        };

        $scope.Grid.Options.columnDefs = [
            {
                visible: false,
                headerTooltip: true, name: 'Id', width: 100, field: 'Id', hasCustomWidth: false, headerCellClass: 'Red', filter: { condition: uiGridCustomService.numberCondition },
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'DataSourceType', field: 'DataSourceType', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'DataSource', field: 'DataSource', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'PharmacySourceCode', field: 'PharmacySourceCode', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'PharmacyName', field: 'PharmacyName', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'LegalName', field: 'LegalName', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'АБЛ', field: 'Category', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'INN', field: 'INN', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.History_classifier_Search(null, 0, 0,0,0, null, row.entity.INN, null)">{{COL_FIELD}}</label></div>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'Region', field: 'Region', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'City', field: 'City', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'Address', field: 'Address', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, enableCellEdit: true, width: 100, name: 'GSId', field: 'GSId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                //cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.History_classifier_Search(null, row.entity.GSId,0,0, 0, null, null, null)">{{COL_FIELD}}</label></div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, width: 100, name: 'LPUId', field: 'LPUId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                //cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.History_classifier_Search(null,null, row.entity.LPUId,0, 0, null, null, null)">{{COL_FIELD}}</label></div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, width: 100, name: 'DistrId', field: 'DistrId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                //cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.History_classifier_Search(null, 0,0,row.entity.DistrId, 0, null, null, null)">{{COL_FIELD}}</label></div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, width: 100, name: 'PharmacyId', field: 'PharmacyId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                //cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?PHids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.History_classifier_Search(null, 0,0,0, row.entity.PharmacyId, null, null, null)">{{COL_FIELD}}</label></div>'
            },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'LightCyan', name: '1/0', field: 'GS_IsExists', filter: { condition: uiGridCustomService.booleanCondition }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><div class="btn btn-xs" ng-class="{\'btn-info\' : row.entity.GS_IsExists==null}">x</div><div class="btn btn-xs" ng-class="{\'btn-danger\' : row.entity.GS_IsExists==false}">0</div><div class="btn btn-xs" ng-class="{\'btn-success\' : row.entity.GS_IsExists==true}">1</div></div>'
            },

            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'EntityINN', field: 'EntityINN', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.History_classifier_Search(null, 0, 0,0, 0,null, row.entity.EntityINN, null)">{{COL_FIELD}}</label></div>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'EntityName', field: 'EntityName', filter: { condition: uiGridCustomService.condition } },

            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'Имя Сети', field: 'NetworkName', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.History_classifier_Search(null, 0, 0,0,0, null, null,row.entity.NetworkName)">{{COL_FIELD}}</label></div>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'Address_region', field: 'Address_region', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'Address_city', field: 'Address_city', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'Address_street', field: 'Address_street', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'Бренд аптеки', field: 'PharmacyBrand', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'Comments', field: 'Comments', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'Spark', field: 'Spark', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: $scope.IsGSwork, width: 100, name: 'Spark2', field: 'Spark2', filter: { condition: uiGridCustomService.condition } },

            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'BricksId', field: 'BricksId', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.History_classifier_Search(null, 0, 0, 0, 0, row.entity.BricksId, null, null)">{{COL_FIELD}}</label></div>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Status', field: 'Status', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Добавлено', field: 'Date_Add', type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'CheckStat', field: 'CheckStat', filter: { condition: uiGridCustomService.condition } }
        ];
        if ($scope.IsTypeClient === true) {
            var TypeClients = [];
            TypeClients = { headerTooltip: true, cellTooltip: true, name: 'Тип Получателя', width: 100, field: 'TypeClients', enableCellEdit: false, filter: { condition: uiGridCustomService.condition } };
            var TypeClients_WhoT = [];
            TypeClients_WhoT = { headerTooltip: true, cellTooltip: true, name: 'Тип Получателя Кто', width: 100, field: 'TypeClients_WhoT', enableCellEdit: false, filter: { condition: uiGridCustomService.condition } };
            var TypeClients_When = [];
            TypeClients_When = { headerTooltip: true, cellTooltip: true, name: 'Тип Получателя Когда', width: 100, field: 'TypeClients_When', type: 'date', filter: { condition: uiGridCustomService.FILTER_DATE } };

            $scope.Grid.Options.columnDefs.push(TypeClients);
            $scope.Grid.Options.columnDefs.push(TypeClients_WhoT);
            $scope.Grid.Options.columnDefs.push(TypeClients_When);

        }
        $scope.Grid.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/History_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.spr_Category, response.data.Data.spr_Category);
            Array.prototype.push.apply($scope.spr_Status, response.data.Data.spr_Status);
            Array.prototype.push.apply($scope.spr_Source_client, response.data.Data.spr_Source_client);
            Array.prototype.push.apply($scope.spr_DataSourceType, response.data.Data.spr_DataSourceType);
            Array.prototype.push.apply($scope.spr_Tops, response.data.Data.spr_Tops);
            Array.prototype.push.apply($scope.spr_Users, response.data.Data.spr_Users);
            Array.prototype.push.apply($scope.spr_Comments, response.data.Data.spr_Comments);
            Array.prototype.push.apply($scope.spr_DataSource, response.data.Data.spr_DataSource);
            Array.prototype.push.apply($scope.spr_Spec, response.data.Data.spr_Spec);

            $scope.filter.spr_top = $scope.spr_Tops[0];
            $scope.filter.spr_Category = $scope.spr_Category[1];


            $scope.spr_Status.forEach(function (item) {
                if (item.code === -5)
                    $scope.filter.spr_Status = item;
            });

            $scope.filter.spr_Source_client = $scope.spr_Source_client[0];
            $scope.filter.spr_DataSourceType = $scope.spr_DataSourceType[0];
            $scope.filter.spr_Comments = $scope.spr_Comments[0];
            $scope.filter.spr_DataSource = $scope.spr_DataSource[0];
            $scope.filter.spr_Spec = $scope.spr_Spec[0];

            var route_ex = 0;
            if ($route.current.params["Top"] !== undefined) {
                $scope.filter.spr_top = $scope.GetFromSPR($scope.spr_Tops, 1 * $route.current.params["Top"], $scope.filter.spr_top);
                route_ex = 1;
            }
            if ($route.current.params["DataSourceType"] !== undefined) {
                $scope.filter.spr_DataSourceType = $scope.GetFromSPR($scope.spr_DataSourceType, $route.current.params["DataSourceType"], $scope.filter.spr_DataSourceType);
                route_ex = 1;
            }
            if ($route.current.params["DataSource"] !== undefined) {
                $scope.filter.spr_DataSource = $scope.GetFromSPR($scope.spr_DataSource, $route.current.params["DataSource"], $scope.filter.spr_DataSource);
                route_ex = 1;
            }

            if ($scope.IsOnline === true) {
                $scope.History_ShowData();
            }
            else {
                if (route_ex === 1) {
                    $scope.History_GetData();
                }
                else { $scope.History_ShowData(); }
            }
        });






    };
    $scope.GetFromSPR = function (SPR, value, def) {
        SPR.forEach(function (item, i, arr) {
            if (item.code === value) {
                def = item;
                return;
            }
        });
        return def;
    };
    $scope.History_GetData = function () {
        $scope.filter.top = $scope.filter.spr_top.code;
        $scope.filter.Category = $scope.filter.spr_Category.code;
        $scope.filter.Comments = $scope.filter.spr_Comments.code;
        $scope.filter.DataSource = $scope.filter.spr_DataSource.code;
        $scope.filter.Spec = $scope.filter.spr_Spec.code;
        $scope.filter.Status = $scope.filter.spr_Status.code;
        $scope.filter.Source_client = $scope.filter.spr_Source_client.code;
        $scope.filter.DataSourceType = $scope.filter.spr_DataSourceType.code;
        if ($scope.IsOnline === true) {
            if ($scope.Grid.NeedSave === true) {
                messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                    .then(
                        function (result) {
                            $scope.History_SetData(1);
                        },
                        function (result) {
                            if (result === 'no') {
                                $scope.History_ShowData();
                            }
                            else {
                                var d = "отмена";
                            }

                        });
            }
            else {
                $scope.History_ShowData();
            }
        }
        else {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/History_GetData/',
                    data: JSON.stringify({ filter: $scope.filter, IsOnline: $scope.IsOnline })
                }).then(function (response) {
                    $scope.History_ShowData();
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.History_ShowData = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/History_ShowData/',
                data: JSON.stringify({ filter: $scope.filter, IsOnline: $scope.IsOnline })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.NeedSave = false;
                    if (data.Data.length > 0) {
                        $scope.HaveData = true;
                        if (data.Data.length < response.data.count) {
                            alert("Внимание показано не все.");
                        }
                    }
                    $scope.Grid.Options.data = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };



    ///Grid для заморозки
    $scope.History_FreezePermission = 0;
    $scope.Grid_Freeze = uiGridCustomService.createGridClass($scope, 'Grid_Freeze');
    $scope.Grid_Freeze.Options.showGridFooter = false;
    $scope.Grid_Freeze.Options.multiSelect = false;
    $scope.Grid_Freeze.Options.modifierKeysToMultiSelect = true;
    $scope.Grid_Freeze.Options.customEnableRowSelection = true;
    $scope.Grid_Freeze.Options.enableRowSelection = false;
    $scope.Grid_Freeze.Options.enableRowHeaderSelection = false;
    $scope.Grid_Freeze.Options.enableSelectAll = false;
    $scope.Grid_Freeze.Options.selectionRowHeaderWidth = 20;
    $scope.Grid_Freeze.Options.rowHeight = 20;
    $scope.Grid_Freeze.Options.appScopeProvider = $scope;
    $scope.Grid_Freeze.Options.enableFullRowSelection = true;
    $scope.Grid_Freeze.Options.enableSelectionBatchEvent = true;
    $scope.Grid_Freeze.Options.enableHighlighting = true;
    $scope.Grid_Freeze.Options.noUnselect = false;
    $scope.Grid_Freeze.Options.appScopeProvider = $scope;
    $scope.Grid_Freeze.Options.columnDefs = [
         { headerTooltip: true, name: 'Id', width: 100, field: 'Id'},
         { headerTooltip: true, cellTooltip: true, enableCellEdit: false, name: 'Клиент', field: 'Source_client', filter: { condition: uiGridCustomService.condition } },
         { headerTooltip: true, enableCellEdit: false, name: 'Заморожено', field: 'IsFreeze', filter: { condition: uiGridCustomService.booleanCondition }, width: 95,
             cellTemplate:

                 ' <button ng-show="{{row.entity.IsFreeze==null||row.entity.IsFreeze<1  ?\'true\':\'false\'}}" class= "btn btn-xs btn-success"  ng-disabled="{{row.entity.Permission<1  ?\'true\':\'false\'}}" style = "width:90px;" ng-click="grid.appScope.History_ToFreeze(row.entity.Source_client);" >Заморозить</button> <button ng-show="{{row.entity.IsFreeze==null||row.entity.IsFreeze<1  ?\'false\':\'true\'}}"  class= "btn btn-xs btn-danger"  ng-disabled="{{row.entity.Permission<1  ?\'true\':\'false\'}}" style = "width:90px;" ng-click="grid.appScope.History_UnFreeze(row.entity.Source_client);">Разморозить</button>'

                   },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Кол-во записей в работе', field: 'CountInWork', filter: { condition: uiGridCustomService.condition } },
       
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, name: 'У кого в работе', field: 'UsersWork', filter: { condition: uiGridCustomService.condition } },
        { visible:false, headerTooltip: true, cellTooltip: true, enableCellEdit: false, name: 'Permission', field: 'Permission', filter: { condition: uiGridCustomService.condition } }

    ];

   
  /* 
   * '<button class="btn btn-xs {{(row.entity.IsFreeze==null||row.entity.IsFreeze<1  ?\'btn-success\':\'btn-danger\')}}"  ng-disabled="{{row.entity.Permission<1  ?\'true\':\'false\'}}" style="width:90px;" ng-click="{{row.entity.Permission < 1 ? \' \': \' History_ToFreeze(\'test\'); \'}}"> {{(row.entity.IsFreeze == null || row.entity.IsFreeze < 1 ?\'Заморозить\':\'Разморозить\')}}</button>'
    */
    

    
    $('FreezeModal').on('show.bs.modal', function (event) {
    
        History_FreezeList();
    });



    $scope.History_ToFreeze = function (Source_client) {        
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/History_ToFreeze/',
                data: JSON.stringify({ Source_client: Source_client })
            }).then(function (response) {

                var data = response.data;
                if (data.Success) {
                    alert(Source_client + ' заблокирован');
                    $scope.History_FreezeList();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.History_UnFreeze = function (Source_client) {     
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/History_UnFreeze/',
                data: JSON.stringify({ Source_client: Source_client })
            }).then(function (response) {

                var data = response.data;
                if (data.Success) {
                    alert(Source_client + ' разблокирован');
                    $scope.History_FreezeList();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    $scope.History_FreezeList = function () {

        $scope.Grid_Freeze.Options.data = [];
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/History_FreezeGetSource/',             
            }).then(function (response) {
              
                var data = response.data;
                if (data.Success) {
                    //$scope.Grid.NeedSave = false;
                    //if (data.Data.length > 0) {
                   //     $scope.Grid_Freeze.Options.data = data.Data;
                     //   $scope.HaveData = true;
                       // if (data.Data.length < response.data.count) {
                       //     alert("Внимание показано не все.");
                     //   }
                   // }
                    
                    $scope.Grid_Freeze.Options.data = data.Data;
                    if (data.Data.length > 0) {
                        if (data.Data[0].Permission != null) {
                            $scope.History_FreezePermission =  data.Data[0].Permission;
                         //   alert($scope.History_FreezePermission)
                        }
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    $scope.History_SetData = function (level) {
        var array_upd = $scope.Grid.GetArrayModify();
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/History_SetData/',
                data: JSON.stringify({ array: array_upd, level: level, IsOnline: $scope.IsOnline, IsGSwork: $scope.IsGSwork, IsTypeClient: $scope.IsTypeClient })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.ClearModify();
                    if (level === 1) {
                        $scope.HaveData = false;
                        $scope.History_ShowData();
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
                return;
            });

    };
    $scope.History_ToExcel = function () {

        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GS/History_ToExcel/',
            data: JSON.stringify({}),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'History_coding.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.History_FromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/History_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.History_ShowData();
            }, function (response) {
                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };
    $scope.History_SetOtherData = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/History_SetOtherData/',
                data: JSON.stringify({ UsersCL: $scope.UsersCL.code })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    alert("Сбросил");
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
                return;
            });
    };
    $scope.History_classifier_Search = function (text, GSId, LPUId, DistrId, PharmacyId, BricksId, INN, NetworkName) {
        $scope.Grid_SPR.Options.data = [];
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Category = item.Category;
        });
        $scope.FilterDescription = [];
        $scope.GSId = GSId;
        $scope.LPUId = LPUId;
        $scope.DistrId = DistrId;
        $scope.PharmacyId = PharmacyId;
        $scope.INN = INN;
        $scope.BricksId = BricksId;
        $scope.Search_text = text;
        $scope.NetworkName = NetworkName;
        if ($scope.GSId === null) $scope.GSId = 0;
        if ($scope.LPUId === null) $scope.LPUId = 0;
        if ($scope.DistrId === null) $scope.DistrId = 0;
        if ($scope.PharmacyId === null) $scope.PharmacyId = 0;
        if ($scope.filter.spr_Spec.code === 'noBrick')
            $scope.Category = 201;
        $scope.dataLoading = $http.post('/GS/History_GetClassifier/',
            JSON.stringify({
                Category: $scope.Category, text: $scope.Search_text, GSId: $scope.GSId, LPUId: $scope.LPUId, DistrId: $scope.DistrId, PharmacyId: $scope.PharmacyId,
                BricksId: $scope.BricksId, INN: $scope.INN, NetworkName: $scope.NetworkName
            }))
            .then(function (response) {
                $scope.Grid_SPR.Options.data = response.data;
            }, function () {
                $scope.message = 'Unexpected Error';
            });
        //if (event) {
        //    event.preventDefault();
        //}
    };
    $scope.History_shiftF = function (event) {
        setIsShift(false);
        var text = commonService.getSelectionText();

        if (!text)
            text = $scope.selectedText;

        if (!text)
            return;

        $scope.History_classifier_Search(text, 0, 0, 0, 0, '', '', '');
    };
    $scope.History_classifier_Search_btn = function () {

        $scope.History_classifier_Search($scope.Search_text, 0, 0, 0, 0, '', '', '');
    };
    $scope.History_Set_Status = function (Status) {
        setIsShift(false);
        $scope.Grid.selectedRows().forEach(function (item) {
            if (Status !== 5 && Status !==110 ) {
                $scope.Grid.GridCellsMod(item, "GSId", null);
                $scope.Grid.GridCellsMod(item, "PharmacyId", null);
            }
            $scope.Grid.GridCellsMod(item, "Status", Status);
        });
    };
    $scope.History_Set_Comment = function (Comment) {
        setIsShift(false);
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Comments", Comment);
        });
    };
    $scope.History_Set_TypeClients = function (TypeClients) {
        //alert('Запрещено');//Считаем что этот тип является образующим и тут не устанавливается.
        setIsShift(false);
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "TypeClients", TypeClients);
            $scope.Grid.GridCellsMod(item, "TypeClients_WhoT", $scope.user.Fullname);
            $scope.Grid.GridCellsMod(item, "TypeClients_Who", $scope.user.UserId);
        });
    };
    $scope.History_Set_CheckStat = function (CheckStat) {
        //alert('Запрещено');//Считаем что этот тип является образующим и тут не устанавливается.
        setIsShift(false);
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "CheckStat", CheckStat);
        });
    };
    $scope.History_Set_Category = function (Category) {
        setIsShift(false);
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Category", Category);
        });
    };
    $scope.Grid_SPR_dblClick = function (field, row) {
        if ($scope.IsGSwork === true) {
            var Status = 0;
            if (row.DistrId === null && row.LPUId === null && row.GSId === null && row.PharmacyId === null) {
                $scope.Grid.selectedRows().forEach(function (item) {
                    $scope.Grid.GridCellsMod(item, "BricksId", row.BricksId);
                });
            }
            else {
                if (row.DistrId > 0 || row.LPUId > 0 || (row.GSId > 0 && row.PharmacyId > 0))
                    Status = 30;
                $scope.Grid.selectedRows().forEach(function (item) {
                    $scope.Grid.GridCellsMod(item, "GSId", row.GSId);
                    $scope.Grid.GridCellsMod(item, "LPUId", row.LPUId);
                    $scope.Grid.GridCellsMod(item, "DistrId", row.DistrId);
                    $scope.Grid.GridCellsMod(item, "PharmacyId", row.PharmacyId);
                    $scope.Grid.GridCellsMod(item, "EntityINN", row.EntityINN);
                    $scope.Grid.GridCellsMod(item, "EntityName", row.EntityName);
                    $scope.Grid.GridCellsMod(item, "NetworkName", row.NetworkName);
                    $scope.Grid.GridCellsMod(item, "Address_region", row.Address_region);
                    $scope.Grid.GridCellsMod(item, "Address_city", row.Address_city);
                    $scope.Grid.GridCellsMod(item, "Address_street", row.Address_street);
                    $scope.Grid.GridCellsMod(item, "PharmacyBrand", row.PharmacyBrand);
                    //$scope.Grid.GridCellsMod(item, "Comments",row.);
                    //$scope.Grid.GridCellsMod(item, "Spark",row.);
                    //$scope.Grid.GridCellsMod(item, "Spark2",row.);
                    //$scope.Grid.GridCellsMod(item, "Channel",row.);
                    $scope.Grid.GridCellsMod(item, "BricksId", row.BricksId);
                    $scope.Grid.GridCellsMod(item, "Status", Status);
                });

            }
        }
    };
    $scope.History_Copy = function () {
        $scope.CopyObjectRow = null;
        var selectedRows = $scope.Grid.selectedRows();
        if (selectedRows.length === 1) {
            $scope.CopyObjectRow = selectedRows[0];
        }
    };
    $scope.History_Paste = function () {
        if ($scope.IsGSwork === true) {
            if ($scope.CopyObjectRow !== null) {
                $scope.Grid.selectedRows().forEach(function (item) {
                    $scope.Grid.GridCellsMod(item, "GSId", $scope.CopyObjectRow.GSId);
                    $scope.Grid.GridCellsMod(item, "LPUId", $scope.CopyObjectRow.LPUId);
                    $scope.Grid.GridCellsMod(item, "DistrId", $scope.CopyObjectRow.DistrId);
                    $scope.Grid.GridCellsMod(item, "PharmacyId", $scope.CopyObjectRow.PharmacyId);
                    $scope.Grid.GridCellsMod(item, "EntityINN", $scope.CopyObjectRow.EntityINN);
                    $scope.Grid.GridCellsMod(item, "EntityName", $scope.CopyObjectRow.EntityName);
                    $scope.Grid.GridCellsMod(item, "NetworkName", $scope.CopyObjectRow.NetworkName);
                    $scope.Grid.GridCellsMod(item, "Address_region", $scope.CopyObjectRow.Address_region);
                    $scope.Grid.GridCellsMod(item, "Address_city", $scope.CopyObjectRow.Address_city);
                    $scope.Grid.GridCellsMod(item, "Address_street", $scope.CopyObjectRow.Address_street);
                    $scope.Grid.GridCellsMod(item, "PharmacyBrand", $scope.CopyObjectRow.PharmacyBrand);
                    $scope.Grid.GridCellsMod(item, "Comments", $scope.CopyObjectRow.Comments);
                    $scope.Grid.GridCellsMod(item, "Spark", $scope.CopyObjectRow.Spark);
                    $scope.Grid.GridCellsMod(item, "Spark2", $scope.CopyObjectRow.Spark2);
                    $scope.Grid.GridCellsMod(item, "BricksId", $scope.CopyObjectRow.BricksId);

                    $scope.Grid.GridCellsMod(item, "Status", $scope.CopyObjectRow.Status);
                });
            }
        }
    };
    $scope.History_SetGSId = function (GSId) {
        setIsShift(false);
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "GSId", GSId);
        });
    };
    $scope.GS_add_from_History_coding = function () {
        $scope.Grid.selectedRows().forEach(function (item, i, arr) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/GS_add_from_History_coding/',
                    data: JSON.stringify({ History_codingId: item.Id })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        $scope.Grid.GridCellsMod(item, "GSId", data.Data);
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                    return;
                });
        });


    };
    $scope.LPU_add_from_History_coding = function () {
        $scope.Grid.selectedRows().forEach(function (item, i, arr) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/LPU_add_from_History_coding/',
                    data: JSON.stringify({ History_codingId: item.Id })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        $scope.Grid.GridCellsMod(item, "LPUId", data.Data);
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                    return;
                });
        });


    };


    $scope.History_Clear = function () {
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "GSId", null);
            $scope.Grid.GridCellsMod(item, "LPUId", null);
            $scope.Grid.GridCellsMod(item, "DistrId", null);
            $scope.Grid.GridCellsMod(item, "PharmacyId", null);
            $scope.Grid.GridCellsMod(item, "EntityINN", "");
            $scope.Grid.GridCellsMod(item, "EntityName", "");
            $scope.Grid.GridCellsMod(item, "NetworkName", "");
            $scope.Grid.GridCellsMod(item, "Address_region", "");
            $scope.Grid.GridCellsMod(item, "Address_city", "");
            $scope.Grid.GridCellsMod(item, "Address_street", "");
            $scope.Grid.GridCellsMod(item, "PharmacyBrand", "");
            $scope.Grid.GridCellsMod(item, "Comments", "");
            $scope.Grid.GridCellsMod(item, "Spark", "");
            $scope.Grid.GridCellsMod(item, "Spark2", "");
            $scope.Grid.GridCellsMod(item, "BricksId", null);

            $scope.Grid.GridCellsMod(item, "Status", 0);

            $scope.Grid.GridCellsMod(item, "CheckStat", 0);
        });
    };
    $scope.History_ReSync = function () {
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/History_ReSync/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.History_ShowData();
        }, function (response) {
            $scope.Grid.Options.data = [];
            messageBoxService.showError(JSON.stringify(response));
        });
    };
    ////////////////////////////////// History_Init Окончание

    ////////////////////////////////// SummsPeriod_Init Начало
    $scope.SummsPeriod_Init = function () {
        $scope.periods = [];
        $scope.IsNoSumms = false;
        $scope.IsNeed = false;
        $scope.FilterP = "";
        $scope.NetworkName = "";
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Grid.afterCellEdit = function (rowEntity, colDef, newValue, oldValue) {
            if (colDef.field === 'Summa_p0') {
                $scope.Grid.GridCellsMod(rowEntity, "SourceData_p0", "Fix");
            }
            if (colDef.field === 'Summa_p1') {
                $scope.Grid.GridCellsMod(rowEntity, "SourceData_p1", "Fix");
            }
            if (colDef.field === 'Summa_p2') {
                $scope.Grid.GridCellsMod(rowEntity, "SourceData_p2", "Fix");
            }
        };



        $scope.Grid.Options.columnDefs = [
            {
                headerTooltip: true, name: 'GSId', width: 100, field: 'GSId', hasCustomWidth: false, headerCellClass: 'Red', filter: { condition: uiGridCustomService.numberCondition },
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'PharmacyId', field: 'PharmacyId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.numberCondition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?PHids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { headerTooltip: true, enableCellEdit: true, width: 300, name: 'ИНН юр. лица', field: 'EntityINN', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Organization?inn={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { headerTooltip: true, enableCellEdit: true, width: 300, name: 'Юр. лицо', field: 'EntityName', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 300, name: 'Бренд аптеки', field: 'PharmacyBrand', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'СФ', field: 'Address_region', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'НП', field: 'Address_city', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, cellTooltip: true, width: 100, name: 'Адрес', field: 'Address', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Ориентир', field: 'Address_comment', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Режим работы', field: 'OperationMode', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, name: 'Дата Добавления', width: 100, field: 'Date_Create', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { headerTooltip: true, name: 'Текущий период', width: 100, field: 'Period', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE }
        ];
        for (var p = 0; p < 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, cellTooltip: true, enableCellEdit: true, periodIndex: p, periodName: 'Сеть', width: 100, field: 'NetworkName_p' + p, headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition }
            });
        }
        for (p = 0; p < 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, name: 'isExists_p' + p, periodIndex: p, periodName: 'isExists', width: 100, field: 'isExists_p' + p, filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-info\' : row.entity.isExists_p' + p + '==null}" ng-click="grid.appScope.isExists(row.entity,' + p + ',null,col)">x</button><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.isExists_p' + p + '==false}" ng-click="grid.appScope.isExists(row.entity,' + p + ',false,col)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.isExists_p' + p + '==true}" ng-click="grid.appScope.isExists(row.entity,' + p + ',true,col)">1</button></div>'
            });
        }
        for (p = 0; p < 1; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: 'С.старт', name: 'С.старт' + p, width: 100, field: 'Summa_Start_p' + p, headerCellClass: 'PeachPuff', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.SourceData_p' + p + '}}" title="{{COL_FIELD}}">{{COL_FIELD | number:2}}</div>'
            });
        }
        for (p = 0; p < 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: '∑', name: '∑' + p, width: 100, field: 'Summa_p' + p, headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.SourceData_p' + p + '}}" title="{{COL_FIELD}}">{{COL_FIELD | number:2}}</div>'
            });
        }
        for (p = 0; p < 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, cellTooltip: true, enableCellEdit: true, periodIndex: p, periodName: 'Источник', width: 100, field: 'SourceData_p' + p, headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.SourceData_p' + p + '}}" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
            });
        }
        $scope.Grid.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/SummsPeriod_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.periods, response.data.Data.periods);
            $scope.currentperiod = response.data.Data.periods[0];
            $scope.SummsPeriod_Search();
        });
    };
    $scope.SummsPeriod_AlphaBitSums_Set = function (paramret) {
        messageBoxService.showConfirm('Вы уверены что хотите запустить применение АльфаБит Сумм???', 'Рассчёт')
            .then(//да сохранить
                function (result) {
                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/GS/SummsPeriod_AlphaBitSums_Set/',
                            data: JSON.stringify({ currentperiod: $scope.currentperiod })
                        }).then(function (response) {
                            var data = response.data;
                            if (data.Success) {
                                if (paramret === "SummsPeriod")
                                    $scope.SummsPeriod_Search_AC();
                                if (paramret === "SummsAnket")
                                    $scope.SummsAnket_Search_AC();
                                if (paramret === "SummsRegion")
                                    $scope.SummsRegion_Search_AC();
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
    $scope.SummsPeriod_OFD_Set = function () {
        messageBoxService.showConfirm('Вы уверены что хотите запустить применение ОФД Сумм???', 'Рассчёт')
            .then(//да сохранить
                function (result) {
                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/GS/SummsPeriod_OFD_Set/',
                            data: JSON.stringify({ currentperiod: $scope.currentperiod })
                        }).then(function (response) {
                            $scope.SummsOFD_search_AC();
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
    $scope.SummsPeriod_Recalc = function (paramret) {
        //messageBoxService.showConfirm('Вы уверены что хотите запустить Пересчёт Сумм???', 'Рассчёт')
        //  .then(//да сохранить
        //function (result) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsPeriod_Recalc/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (paramret === "SummsPeriod")
                        $scope.SummsPeriod_Search_AC();
                    if (paramret === "SummsAnket")
                        $scope.SummsAnket_Search_AC();
                    if (paramret === "SummsRegion")
                        $scope.SummsRegion_Search_AC();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        //},
        //function (result) {
        //    if (result === 'no') {//нет не сохранять
        //    }
        //    else {//отмена
        //        var d = "отмена";
        //    }

        //});
    };
    $scope.count_period_edit = 0;
    $scope.SummsPeriod_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsPeriod_Search/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod, IsNoSumms: $scope.IsNoSumms, IsNeed: $scope.IsNeed, FilterP: $scope.FilterP, NetworkName: $scope.NetworkName })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.SetColumnPeriod(data.Data2);
                    $scope.count_period_edit = 0;
                    if ($scope.currentperiod.indexOf("-Q") > 0)
                        $scope.count_period_edit = 2;
                    $scope.Grid.Options.columnDefs.forEach(function (item, i, arr) {
                        if (item.periodIndex !== undefined) {
                            if (item.periodIndex > $scope.count_period_edit) {
                                item.enableCellEdit = false;
                            }
                            else {
                                item.enableCellEdit = true;
                            }
                        }
                    });
                    $scope.Grid.Options.data = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SummsPeriod_Search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(//да сохранить
                    function (result) {
                        $scope.SummsPeriod_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {//нет не сохранять
                            $scope.SummsPeriod_Search_AC();
                        }
                        else {//отмена
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.SummsPeriod_Search_AC();
        }

    };
    $scope.SummsPeriod_Save = function (action) {
        //alert('Не работает');        
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/SummsPeriod_Save/',
                    data: JSON.stringify({ array: array_upd, count_period_edit: $scope.count_period_edit })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.SummsPeriod_search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.SummsPeriod_To_Excel = function () {
        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GS/SummsPeriod_To_Excel/',
            data: JSON.stringify({ currentperiod: $scope.currentperiod, IsNoSumms: $scope.IsNoSumms, IsNeed: $scope.IsNeed, FilterP: $scope.FilterP, NetworkName: $scope.NetworkName }),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'SummsPeriod_' + $scope.currentperiod + '.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.SummsPeriod_from_Excel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
                formData.append('currentperiod', $scope.currentperiod);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/SummsPeriod_from_Excel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.SummsPeriod_Search();
            }, function (response) {
                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };
    $scope.SummsPeriod_SetOUT = function () {
        messageBoxService.showConfirm('Вы уверены что хотите применить выбранные периоды??', 'Рассчёт')
            .then(//да сохранить
                function (result) {
                    alert('Не работает');
                },
                function (result) {
                    if (result === 'no') {//нет не сохранять
                    }
                    else {//отмена
                        var d = "отмена";
                    }

                });
    };
    $scope.SummsPeriod_Clear = function () {
        var selectedRows = $scope.Grid.selectedRows;
        selectedRows.forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Summa_p0", 0.0);
            $scope.Grid.GridCellsMod(item, "SourceData_p0", 0.0);
            if (count_period_edit > 1) {
                $scope.Grid.GridCellsMod(item, "Summa_p1", 0.0);
                $scope.Grid.GridCellsMod(item, "SourceData_p1", 0.0);

                $scope.Grid.GridCellsMod(item, "Summa_p2", 0.0);
                $scope.Grid.GridCellsMod(item, "SourceData_p2", 0.0);
            }
        });
    };
    ////////////////////////////////// SummsPeriod_Init Окончание

    ////////////////////////////////// SummsRegion Начало
    $scope.SummsRegion_Init = function () {
        $scope.periods = [];
        $scope.K_otkl = 0.0;
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;


        $scope.Grid.Options.columnDefs = [
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'ФО', field: 'Fed_ok', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'СФ', field: 'Region', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
            },
            {
                headerTooltip: true, name: 'Текущий период', width: 100, field: 'Period', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition },
                type: 'date', cellFilter: formatConstants.FILTER_DATE
            },
            {
                enableCellEdit: false, name: 'доля прошлый год', width: 100, field: 'Kof_dol_prev_year', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}">{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: false, name: 'прошлый год', width: 100, field: 'Year_prev', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                enableCellEdit: false, periodIndex: 12, periodName: '∑', name: '∑12', width: 100, field: 'Month_12', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                enableCellEdit: false, name: 'доля текущий год', width: 100, field: 'Kof_dol_now_year', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}">{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: false, name: 'разница доли регионов', width: 100, field: 'delta_Kof_dol', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}">{{COL_FIELD | number:3}}</div>'
            },
            {
                enableCellEdit: false, name: 'точки', width: 100, field: 'Count_GSId_0', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
                cellTemplate: '<div class="ui-grid-cell-contents">{{COL_FIELD | number:0}}</div>'
            },
            {
                enableCellEdit: false, name: 'текущий год', width: 100, field: 'Year_now', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents font_bold {{row.entity.ClassStyle}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                enableCellEdit: false, periodIndex: 3, periodName: '∑', name: '∑3', width: 100, field: 'Month_3', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:0}}</div>'
            },
            {
                enableCellEdit: false, periodIndex: 2, periodName: '∑', name: '∑2', width: 100, field: 'Month_2', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:0}}</div>'
            },
            {
                enableCellEdit: false, periodIndex: 1, periodName: '∑', name: '∑1', width: 100, field: 'Month_1', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:0}}</div>'
            },
            {
                enableCellEdit: false, periodIndex: 0, periodName: '∑ текущая', name: '∑0', width: 100, field: 'Month_0', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents font_bold {{row.entity.ClassStyle}}" >{{COL_FIELD | number:0}}</div>'
            },

            {
                enableCellEdit: false, name: 'прирост к прошлому месяцу', width: 100, field: 'Kof_rost_month', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: false, name: 'прирост к прошлому году', width: 100, field: 'Kof_rost_month12', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: true, name: 'коэф-т коррекции', width: 100, field: 'Kof', headerCellClass: 'Green', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents" >{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: false, name: 'Расчёткоэф-т коррекции', width: 100, field: 'Kof_calc', headerCellClass: 'Green', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents" >{{COL_FIELD | number:2}}</div>'
            },
            //АльфаБит
            {
                enableCellEdit: false, name: 'АльБ прирост пересекающихся к прошлому месяцу', width: 100, field: 'Kof_rost_common_prev_month', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: false, name: 'АльБ дельта', width: 100, field: 'Kof_delta', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: false, name: 'АльБ кол пересекающихся', width: 100, field: 'Count_common_prev_month', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" ">{{COL_FIELD | number:2}}</div>'
            },
            //ОФД
            {
                enableCellEdit: false, name: 'ОФД прирост пересекающихся к прошлому месяцу', width: 100, field: 'Kof_rost_common_prev_month_ofd', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: false, name: 'ОФД дельта', width: 100, field: 'Kof_delta_ofd', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" >{{COL_FIELD | number:2}}</div>'
            },
            {
                enableCellEdit: false, name: 'ОФД кол пересекающихся', width: 100, field: 'Count_common_prev_month_ofd', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" ">{{COL_FIELD | number:2}}</div>'
            },
            // Сумма, руб. расчет
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: '∑ руб. расчет', field: 'TotalSumm', type: 'number', visible: true, nullable: true, headerCellClass: 'row_total',
                filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents font_bold {{row.entity.ClassStyle}}" ">{{COL_FIELD | number:2}}</div>'
            }
        ];
        $scope.Grid.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/SummsRegion_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.periods, response.data.Data.periods);
            $scope.currentperiod = response.data.Data.periods[0];
            $scope.SummsRegion_Search_AC();
        });
    };
    $scope.SummsRegion_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsRegion_Search/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod, K_otkl: $scope.K_otkl })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.K_otkl = data.Data[0].K_otkl;
                    $scope.SetColumnPeriod(data.Data2);
                    $scope.Grid.Options.data = data.Data;
                    $scope.count_period_edit = 0;
                    if ($scope.currentperiod.indexOf("-Q") > 0)
                        $scope.count_period_edit = 2;
                    $scope.Grid.Options.columnDefs.forEach(function (item, i, arr) {
                        if (item.periodIndex !== undefined && item.periodIndex > $scope.count_period_edit) {
                            item.enableCellEdit = false;
                        }
                    });
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SummsRegion_Search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(//да сохранить
                    function (result) {
                        $scope.SummsRegion_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {//нет не сохранять
                            $scope.SummsRegion_Search_AC();
                        }
                        else {//отмена
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.SummsRegion_Search_AC();
        }

    };
    $scope.SummsRegion_Save = function (action) {
        //alert('Не работает');        
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/SummsRegion_Save/',
                    data: JSON.stringify({ array: array_upd, count_period_edit: $scope.count_period_edit })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.SummsRegion_Search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.SummsRegion_Clear = function () {
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Kof", 0.0);
        });
    };
    $scope.SummsRegion_Copy = function () {
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Kof", item.Kof_calc);
        });
    };
    ////////////////////////////////// SummsRegion Окончание
    ////////////////////////////////// SummsAnket Начало
    $scope.SummsAnket_Init = function () {
        $scope.periods = [];
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Grid.Options.columnDefs = [
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'Имя Сети', field: 'NetworkName', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'СФ', field: 'Region', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
            },
            {
                headerTooltip: true, name: 'Текущий период', width: 100, field: 'Period', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition },
                type: 'date', cellFilter: formatConstants.FILTER_DATE
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'к корр', width: 100, field: 'Kof', headerCellClass: 'Green', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:8}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'к расчт', width: 100, field: 'Kof_calc', headerCellClass: 'Green', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:8}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '∑ Альфабита', width: 100, field: 'Summa_AlphaBitSumsStartYear', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '∑ нарастающая', width: 100, field: 'SummaStartYear', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Анкета ∑', width: 100, field: 'anketSum', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Анкета точки' + p, width: 100, field: 'anketPoint', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Отклонение в % расчетных продаж от анкетных', width: 100, field: 'Otkl_proc', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:2}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Отклонение в руб.', width: 100, field: 'Otkl_sum', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Точек Текущий', width: 100, field: 'Count_GSId_0', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            }
            //,
            //{
            //    headerTooltip: true, enableCellEdit: true, name: '∑ Текущий', width: 100, field: 'Summa_0', headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
            //    cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            //}
        ];
        var p = 12;
        for (p = 12; p >= 1; p--) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: '∑', name: '∑ M' + p, width: 100, field: 'Summa_' + p, headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            });
            //$scope.Grid.Options.columnDefs.push({
            //    headerTooltip: true, enableCellEdit: true, name: 'регионов M' + p, width: 100, field: 'Count_Region_' + p, headerCellClass: headerCellClass, type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
            //    cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            //});
            //$scope.Grid.Options.columnDefs.push({
            //    headerTooltip: true, enableCellEdit: true, name: 'NetworkType M' + p, width: 100, field: 'NetworkType_' + p, headerCellClass: headerCellClass, filter: { condition: uiGridCustomService.numberCondition }
            //});
        }
        //p = 0;
        //while (p < 12) {
        //    headerCellClass = 'PaleGoldenrod';
        //    if (p % 2 === 0.0)
        //        headerCellClass = 'NavajoWhite';
        //    $scope.Grid.Options.columnDefs.push({
        //        headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: 'точек', name: 'точек M' + p, width: 100, field: 'Count_GSId_' + p, headerCellClass: headerCellClass, type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
        //        cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
        //    });
        //    p++;
        //}
        $scope.Grid.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/SummsAnket_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.periods, response.data.Data.periods);
            $scope.currentperiod = response.data.Data.periods[0];
            $scope.SummsAnket_Search_AC();
        });
    };
    $scope.SummsAnket_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsAnket_Search/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.SetColumnPeriod(data.Data2);
                    $scope.Grid.Options.data = data.Data;
                    $scope.count_period_edit = 0;
                    if ($scope.currentperiod.indexOf("-Q") > 0)
                        $scope.count_period_edit = 2;
                    $scope.Grid.Options.columnDefs.forEach(function (item, i, arr) {
                        if (item.periodIndex !== undefined && item.periodIndex > $scope.count_period_edit) {
                            item.enableCellEdit = false;
                        }
                    });
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SummsAnket_Search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(//да сохранить
                    function (result) {
                        $scope.SummsAnket_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {//нет не сохранять
                            $scope.SummsAnket_Search_AC();
                        }
                        else {//отмена
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.SummsAnket_Search_AC();
        }

    };
    $scope.SummsAnket_Save = function (action) {
        //alert('Не работает');        
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/SummsAnket_Save/',
                    data: JSON.stringify({ array: array_upd, count_period_edit: $scope.count_period_edit })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.SummsAnket_Search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.SummsAnket_ToTemplate = function () {
        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GS/SummsAnket_ToTemplate/',
            data: JSON.stringify({ currentperiod: $scope.currentperiod }),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'SummsAnket_ToTemplate.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.SummsAnket_FromTemplate = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/SummsAnket_FromTemplate/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
            }).then(function (response) {
                $scope.SummsAnket_Search();
            }, function (response) {
                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };

    $scope.SummsAnket_Clear = function () {
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Kof", 1.0);
        });
    };
    $scope.SummsAnket_Copy = function () {
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Kof", item.Kof_calc);
        });
    };
    ////////////////////////////////// SummsAnket Окончание

    ////////////////////////////////// SummsNetwork Начало
    $scope.SummsNetwork_Init = function () {
        $scope.periods = [];
        $scope.IsWithAnket = false;
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Grid.Options.columnDefs = [
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'Id', field: 'Id', headerCellClass: 'Yellow', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'Имя Сети', field: 'NetworkName', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.ClassStyle}}" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
            },
            {
                headerTooltip: true, name: 'Текущий период', width: 100, field: 'Period', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition },
                type: 'date', cellFilter: formatConstants.FILTER_DATE
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '∑', width: 100, field: 'Summa', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Доля в РФ', width: 100, field: 'ProcentInCountry', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:4}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '∑ нарастающая', width: 100, field: 'SummaStartYear', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '∑ ОФД', width: 100, field: 'SummOFD', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Анкета ∑' + p, width: 100, field: 'anketSum', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Анкета точки' + p, width: 100, field: 'anketPoint', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'Период точки' + p, width: 100, field: 'periodPoint', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            }
            ,
            {
                headerTooltip: true, enableCellEdit: true, name: 'дельта анткеты' + p, width: 100, field: 'SummaStartYearA', headerCellClass: 'LightCyan', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '-Q ∑ на точку', width: 100, field: 'SumByPoint_QP', headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE
            },
            {
                headerTooltip: true, enableCellEdit: true, name: '-Q Доля в РФ', width: 100, field: 'Dol_3M_P', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:4}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: '-Q рейтинг', field: 'RTNG_3M_P', headerCellClass: 'Yellow', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'дельта ∑ на точку', width: 100, field: 'delta_SumByPoint_QP', headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'дельта Доля в РФ', width: 100, field: 'delta_Dol_3M_P', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:4}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'дельта рейтинг', field: 'delta_RTNG_3M_P', headerCellClass: 'Yellow', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent
            }
        ];

        var p = 1;
        for (p = 1; p <= 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: '∑', name: '∑ M' + p, width: 100, field: 'Summa_' + p, headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            });
        }
        for (p = 1; p <= 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: 'точек', name: 'точек M' + p, width: 100, field: 'Count_GSId_' + p, headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            });
        }
        for (p = 1; p <= 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: '∑ на точку', name: '∑ на точку ' + p, width: 100, field: 'SumByPointM_' + p, headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE
            });
        }
        $scope.Grid.Options.columnDefs.push({
            headerTooltip: true, enableCellEdit: true, name: 'Итог ∑ на точку', width: 100, field: 'SumByPointM_С', headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE
        });
        for (p = 1; p <= 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: 'регионов', name: 'регионов M' + p, width: 100, field: 'Count_Region_' + p, headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT,
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            });
        }
        for (p = 1; p <= 12; p++) {
            $scope.Grid.Options.columnDefs.push({
                headerTooltip: true, enableCellEdit: true, periodIndex: p, periodName: 'NetworkType', name: 'NetworkType M' + p, width: 100, field: 'NetworkType_' + p, headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.numberCondition }
            });
        }
        $scope.Grid.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/SummsNetwork_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.periods, response.data.Data.periods);
            $scope.currentperiod = response.data.Data.periods[0];
            $scope.SummsNetwork_Search_AC();
        });
    };
    $scope.SummsNetwork_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsNetwork_Search/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod, IsWithAnket: $scope.IsWithAnket })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.SetColumnPeriod(data.Data2);
                    $scope.Grid.Options.data = data.Data;
                    $scope.count_period_edit = 0;
                    if ($scope.currentperiod.indexOf("-Q") > 0)
                        $scope.count_period_edit = 2;
                    $scope.Grid.Options.columnDefs.forEach(function (item, i, arr) {
                        if (item.periodIndex !== undefined && item.periodIndex > $scope.count_period_edit) {
                            item.enableCellEdit = false;
                        }
                    });
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SummsNetwork_Search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(//да сохранить
                    function (result) {
                        $scope.SummsNetwork_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {//нет не сохранять
                            $scope.SummsNetwork_Search_AC();
                        }
                        else {//отмена
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.SummsNetwork_Search_AC();
        }

    };
    $scope.SummsNetwork_Save = function (action) {
        alert('Не работает');
        return;
        //var array_upd = $scope.Grid.GetArrayModify();        
        //if (array_upd.length > 0) {
        //    $scope.dataLoading =
        //        $http({
        //            method: 'POST',
        //            url: '/GS/SummsNetwork_Save/',
        //            data: JSON.stringify({ array: array_upd, count_period_edit: $scope.count_period_edit })
        //        }).then(function (response) {
        //            var data = response.data;
        //            if (data.Success) {
        //                if (action === "search") {
        //                    $scope.SummsNetwork_Search_AC();
        //                }
        //                else {
        //                    $scope.Grid.ClearModify();
        //                    alert("Сохранил");
        //                }
        //            }
        //        }, function (response) {
        //            errorHandlerService.showResponseError(response);
        //        });
        //}
    };
    ////////////////////////////////// SummsNetwork Окончание

    //#region Point Начало
    $scope.Point_Init = function () {
        $scope.IsNoKoord = false;
        $scope.PHids = "";
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Grid.Options.enableRowSelection = true;

        $scope.Grid.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            //gridApi.selection.on.rowSelectionChanged($scope, doSelection);
        };

        //function doSelection(row) {
        //    _.each($scope.gridApi.selection.getSelectedRows(), function (row) {
        //        console.log(row)
        //    });
        //}

        $scope.rightClick = function (event) {
            var scope = angular.element(event.target).scope();
            //$scope.gridApi.selection.clearSelectedRows();
            $scope.gridApi.grid.modifyRows($scope.Grid.Options.data);
            $scope.gridApi.selection.selectRow(scope.row.entity); 
        };

        //$scope.YandexAddress
        $scope.Grid.Options.columnDefs = [
            {
                headerTooltip: true, name: 'PharmacyId', width: 100, field: 'PharmacyId', hasCustomWidth: false, headerCellClass: 'Red', filter: { condition: uiGridCustomService.numberCondition }
            },
            { visible: false, headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'date_add', field: 'date_add', filter: { condition: uiGridCustomService.condition } },
            { visible: false, headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'koor_DT', field: 'koor_DT', filter: { condition: uiGridCustomService.condition } },
            { visible: false, headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'GSId_first', field: 'GSId_first', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'Address', field: 'Address', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'СФ', field: 'Address_region', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'НП', field: 'Address_city', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Индеск', field: 'Address_index', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Адрес', field: 'Address_street', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Ориентир', field: 'Address_comment', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'Этаж', field: 'Address_float', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: '№ пом.', field: 'Address_room', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'Yellow', name: 'S пом., кв.м.', field: 'Address_room_area', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Брик', field: 'BricksId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Bricks?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'koor_широта', field: 'koor_широта_string', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'koor_долгота', field: 'koor_долгота_string', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'Address_koor_lat', field: 'Address_koor_lat_string', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'Address_koor_long', field: 'Address_koor_long_string', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'Post_Index', field: 'Post_Index', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'PapayaWhip', name: 'fias_id', field: 'fias_id', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'LightCyan', name: 'fias_id_manual', field: 'fias_id_manual', filter: { condition: uiGridCustomService.condition } },
            //{ headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, headerCellClass: 'LightCyan', name: 'fias_code_manual', field: 'fias_code_manual', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'LightCyan', name: 'geo_lat_manual', field: 'geo_lat_manual_string', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, headerCellClass: 'LightCyan', name: 'geo_lon_manual', field: 'geo_lon_manual_string', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, name: 'Проверено', enableCellEdit: true, width: 100, field: 'IsChecked', type: 'boolean' },
            { headerTooltip: true, name: 'Комментарий', enableCellEdit: false, width: 100, field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { visible: false, headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ГАР Guid', field: 'HOUSEGUID', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 500, headerCellClass: 'Yellow', name: 'ГАР СФ', field: 'GAR_Address', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.Grid.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/Point_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            var PHids = $route.current.params["PHids"];
            if (PHids !== undefined) {
                $scope.PHids = PHids;
                $scope.Point_search();
            }
            // $scope.Point_Search_AC();
        });
    };

    $scope.Point_Search_AC = function () {
        
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Point_Search/',
                data: JSON.stringify({ IsNoKoord: $scope.IsNoKoord, PHids: $scope.PHids })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {

                    $scope.Grid.Options.data = data.Data;

                    $scope.comments = [
                        "центр НП",
                        "привязка верна"
                    ];
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    $scope.Point_ColumnCheck_Set = function () {
        var selectedRows = $scope.gridApi.selection.getSelectedRows()
        selectedRows.forEach(function (item) {         
            $scope.Grid.GridCellsMod(item, "IsChecked", true);
        });
    };
    $scope.Point_ColumnCheck_UnSet = function () {
        var selectedRows = $scope.gridApi.selection.getSelectedRows()
        selectedRows.forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "IsChecked", false);
        });
    };

    $scope.Point_Comment = function (comment) {
        var selectedRows = $scope.gridApi.selection.getSelectedRows()
        selectedRows.forEach(function (item) {
            var finalComment = "";
            if (item.Comment && item.Comment.indexOf(comment) >= 0) {
                finalComment = item.Comment.replace(comment, "")
                finalComment = finalComment.replace(";;", ";")
                if (finalComment.startsWith(";"))
                    finalComment = finalComment.substring(1, finalComment.length);
                if (finalComment.endsWith(";"))
                    finalComment = finalComment.substring(0, finalComment.length - 1);
            }
            else {
                if (item.Comment)
                    finalComment = item.Comment + ";" + comment;
                else
                    finalComment = comment;
            }
            $scope.Grid.GridCellsMod(item, "Comment", finalComment);
        });
    };

    $scope.Point_Search = function () {

        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(//да сохранить
                    function (result) {
                        $scope.Point_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {//нет не сохранять
                            $scope.Point_Search_AC();
                        }
                        else {//отмена
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Point_Search_AC();
        }

    };
    $scope.Point_Save = function (action) {
        //alert('Не работает');
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/Point_Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.Point_Search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };

    //#region Point Окончание

    $scope.CopyToBuffer = function (value) {
        navigator.clipboard.writeText(value)
            .then(() => {
                // Получилось!
            })
            .catch(err => {
                console.log('Something went wrong', err);
            });
    }
    ////////////////////////////// SummsOFD для Старт
    $scope.SummsOFD_Init = function () {
        hotkeys.bindTo($scope).add({
            combo: 'shift+1',
            description: 'Вставить',
            callback: function (event) {
                $scope.SummsOFD_IsUse(null, true, 0);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+0',
            description: 'Вставить',
            callback: function (event) {
                $scope.SummsOFD_IsUse(null, false, 0);
            }
        });
        $scope.format = 'dd.MM.yyyy';
        $scope.periods = [];
        $scope.currentperiod = null;

        $scope.bool_select = [{ "Id": true, "name": "1" }, { "Id": false, "name": "0" }, { "Id": null, "name": "пусто" }];

        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Grid.afterCellEdit = function (rowEntity, colDef, newValue, oldValue) {
            if (colDef.field === 'Comment') {
                if (newValue !== '' && newValue !== null) {
                    $scope.Grid.GridCellsMod(rowEntity, "IsUse", false);
                }
            }
        };


        $scope.Grid.Options.columnDefs = [
            {
                visible: false,
                headerTooltip: true, name: 'Id', width: 100, field: 'Id', hasCustomWidth: false, headerCellClass: 'Red', filter: { condition: uiGridCustomService.numberCondition },
            },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'PharmacyId', field: 'PharmacyId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.numberCondition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?PHids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, periodIndex: 0, periodName: 'EntityName', width: 100, field: 'EntityName', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, periodIndex: 0, periodName: 'Сеть', width: 100, field: 'NetworkName', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'BrickId', field: 'BrickId', type: 'number', filter: { condition: uiGridCustomService.numberCondition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{Phids}}" ng-click="grid.appScope.CopyToBuffer(row.entity.Phids)">{{COL_FIELD}}</div>'
            },

            { headerTooltip: true, enableCellEdit: false, name: 'C-2 Tran', width: 100, field: 'CountTran_M2', headerCellClass: 'PeachPuff', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'C-1 Tran', width: 100, field: 'CountTran_M1', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'C Tran', width: 100, field: 'CountTran', headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'C SKU', width: 100, field: 'CountClassifierId', headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'C Days', width: 100, field: 'CountDays', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'АПТчасов', width: 100, field: 'HH_inMonth', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: '∑-2 Brick', width: 100, field: 'SummaBrick_M2', headerCellClass: 'PeachPuff', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: '∑-1 Brick', width: 100, field: 'SummaBrick_M1', headerCellClass: 'PapayaWhip', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: '∑ Brick', width: 100, field: 'SummaBrick', headerCellClass: 'PaleGoldenrod', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: '∑ ГС Brick', width: 100, field: 'MonthlyTurnover_G', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'kof', width: 100, field: 'kof', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Kof_A1', field: 'Kof_A1', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent },
            //{ headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Kof_Brick', field: 'Kof_Brick', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent },
            { headerTooltip: true, enableCellEdit: false, name: '∑расчёт', width: 100, field: 'NewSUM', headerCellClass: '', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            {
                headerTooltip: true, enableCellEdit: false, name: '∑ ГС', width: 100, field: 'MonthlyTurnover', headerCellClass: '', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.SourceData}}" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            {
                headerTooltip: true, enableCellEdit: false, name: '∑ ГС прошлый', width: 100, field: 'MonthlyTurnover_M01', headerCellClass: '', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM,
                cellTemplate: '<div class="ui-grid-cell-contents {{row.entity.SourceData_M01}}" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>'
            },
            { headerTooltip: true, enableCellEdit: false, name: '∑АльфаБит', width: 100, field: 'AL_RealSellingSum', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            {
                headerTooltip: true, enableCellEdit: false, name: 'Средний чек', width: 100, field: 'Check_AVG', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent
            },
            {
                headerTooltip: true, enableCellEdit: false, name: 'Расчёт/ГС', width: 100, field: 'kof_Calc_Gs', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_Procent
            },
            {
                headerTooltip: true, enableCellEdit: true, name: 'IsUse', width: 100, field: 'IsUse', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-danger\' : row.entity.IsUse==false}" ng-click="grid.appScope.SummsOFD_IsUse(row.entity,false,0)">0</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.IsUse==true}" ng-click="grid.appScope.SummsOFD_IsUse(row.entity,true,0)">1</button></div>'
            },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Коммент', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Режим работы', field: 'OperationMode', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { visible: false, headerTooltip: true, enableCellEdit: false, width: 100, name: 'СФ', field: 'Address_region', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'НП', field: 'Address_city', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, cellTooltip: true, width: 100, name: 'Адрес', field: 'Address_street', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'SourceData', field: 'SourceData', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },

            { visible: false, headerTooltip: true, name: 'Текущий период', width: 100, field: 'Period', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { visible: true, headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Поставщик', field: 'SupplierId', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'SourceData прошлый', field: 'SourceData_M01', headerCellClass: '', filter: { condition: uiGridCustomService.condition } },

            { visible: false, headerTooltip: true, enableCellEdit: false, name: 'РежимРаботы Days', width: 100, field: 'DD_inMonth', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { visible: false, headerTooltip: true, enableCellEdit: false, name: 'БРИКчасов', width: 100, field: 'HH_inMonth_G', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { visible: false, headerTooltip: true, enableCellEdit: false, name: '∑АПТ в час', width: 100, field: 'SChek_inMonth', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { visible: false, headerTooltip: true, enableCellEdit: false, name: '∑БРИК в час', width: 100, field: 'SChek_inMonth_G', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'КомАльфаБит', field: 'AL_Comment', headerCellClass: '', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid.SetDefaults();
        return $http({
            method: 'POST',
            url: '/GS/SummsOFD_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.periods, response.data.Data.periods);
            $scope.currentperiod = response.data.Data.periods[0];

            var ids = $route.current.params["ids"];
            if (ids !== undefined) {
                $scope.filter.IDS = ids;
                $scope.SummsOFD_search();
            }
            return response.data;
        });
    };
    $scope.SummsOFD_IsUse = function (GItem, value, period) {
        if (GItem !== null) {
            if (period === 0 && GItem.IsUse !== null) {
                var BrickId = GItem.BrickId;
                $scope.Grid.Options.data.forEach(function (item) {
                    if (item.BrickId === BrickId) {
                        $scope.Grid.GridCellsMod(item, "IsUse", value);
                    }
                });
            }
        }
        else {
            $scope.Grid.selectedRows().forEach(function (item) {
                $scope.SummsOFD_IsUse(item, value, 0);
            });
        }
    };
    $scope.SummsOFD_Comment = function (value) {
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "Comment", value);

            if (value !== '')
                $scope.Grid.GridCellsMod(item, "IsUse", false);
        });
    };
    $scope.SummsOFD_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsOFD_search/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.SetColumnPeriod(data.Data2);
                    $scope.Grid.Options.data = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SummsOFD_search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.SummsOFD_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.SummsOFD_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.SummsOFD_search_AC();
        }

    };
    $scope.SummsOFD_recalc = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/SummsOFD_recalc/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod })
            }).then(function (response) {
                $scope.SummsOFD_search_AC();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SummsOFD_save = function (action) {
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/SummsOFD_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.SummsOFD_search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.SummsOFD_FromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/SummsOFD_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.SummsOFD_search_AC();
            }, function (response) {
                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };
    ////////////////////////////// SummsOFD для Окончание

    ////////////////////////////// DistributorBranch для Старт
    $scope.DistributorBranch_Init = function () {
        $scope.format = 'dd.MM.yyyy';

        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Grid.afterCellEdit = function (rowEntity, colDef, newValue, oldValue) {
            if (colDef.field === 'Comment') {
                if (newValue !== '' && newValue !== null) {
                    $scope.Grid.GridCellsMod(rowEntity, "IsUse", false);
                }
            }
        };


        $scope.Grid.Options.columnDefs = [
            {
                visible: false,
                headerTooltip: true, name: 'Id', width: 100, field: 'Id', hasCustomWidth: false, headerCellClass: 'Red', filter: { condition: uiGridCustomService.numberCondition },
            },
            { headerTooltip: true, enableCellEdit: true, name: 'EntityINN', width: 100, field: 'EntityINN', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'EntityName', width: 100, field: 'EntityName', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'DistributorBrand', width: 100, field: 'DistributorBrand', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Name_Short', width: 100, field: 'Name_Short', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Address_city', width: 100, field: 'Address_city', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Address_city_All', width: 100, field: 'Address_city_All', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Email', width: 100, field: 'Email', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Phone', width: 100, field: 'Phone', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Address_street', width: 100, field: 'Address_street', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Web', width: 100, field: 'Web', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Comment', width: 100, field: 'Comment', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid.SetDefaults();
        return $http({
            method: 'POST',
            url: '/GS/DistributorBranch_Init/',
            data: JSON.stringify({})
        }).then(function (response) {

        });
    };
    $scope.DistributorBranch_save = function (action) {
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/DistributorBranch_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.DistributorBranch_search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.DistributorBranch_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/DistributorBranch_search/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.SetColumnPeriod(data.Data2);
                    $scope.Grid.Options.data = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.DistributorBranch_search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.DistributorBranch_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.DistributorBranch_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.DistributorBranch_search_AC();
        }

    };
    $scope.DistributorBranch_To_Excel = function () {
        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GS/DistributorBranch_To_Excel/',
            data: JSON.stringify({}),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'DistributorBranch.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.DistributorBranch_from_Excel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
                //formData.append('currentperiod', $scope.currentperiod);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/DistributorBranch_from_Excel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.DistributorBranch_Search();
            }, function (response) {
                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };
    ////////////////////////////// DistributorBranch для Окончание

    ////////////////////////////// OperationMode для Старт
    $scope.spr_OperationMode_Init = function () {
        $scope.format = 'dd.MM.yyyy';

        $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Grid.afterCellEdit = function (rowEntity, colDef, newValue, oldValue) {
            if (colDef.field === 'Comment') {
                if (newValue !== '' && newValue !== null) {
                    $scope.Grid.GridCellsMod(rowEntity, "IsUse", false);
                }
            }
        };


        $scope.Grid.Options.columnDefs = [
            {
                headerTooltip: true, name: 'OperationMode', width: 100, field: 'OperationMode', hasCustomWidth: false, headerCellClass: 'Red', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?OperationMode={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { headerTooltip: true, enableCellEdit: true, name: 'Понедельник', width: 100, field: 'Monday', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Вторник', width: 100, field: 'Tuesday', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Среда', width: 100, field: 'Wednesday', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Четверг', width: 100, field: 'Thursday', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Пятница', width: 100, field: 'Friday', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Суббота', width: 100, field: 'Saturday', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: true, name: 'Воскресенье', width: 100, field: 'Sunday', headerCellClass: 'PeachPuff', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid.SetDefaults();
        return $http({
            method: 'POST',
            url: '/GS/spr_OperationMode_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.spr_OperationMode_search();
        });
    };
    $scope.spr_OperationMode_save = function (action) {
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/GS/spr_OperationMode_save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.spr_OperationMode_search_AC();
                        }
                        else {
                            $scope.Grid.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.spr_OperationMode_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/spr_OperationMode_search/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.Options.data = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.spr_OperationMode_search = function () {
        if ($scope.Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.spr_OperationMode_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.spr_OperationMode_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.spr_OperationMode_search_AC();
        }

    };
    ////////////////////////////// OperationMode для Окончание

    ////////////////////////////// NetworkBrand для Старт
    $scope.NetworkBrand_Init = function () {
        hotkeys.bindTo($scope).add({
            combo: 'shift+w',
            description: 'Одобрено',
            callback: function (event) {
                $scope.Grid_NetworkBrand.selectedRows().forEach(function (item) {
                    $scope.Grid_NetworkBrand.GridCellsMod(item, "Used", true);
                });
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+d',
            description: 'Не правильно',
            callback: function (event) {
                $scope.Grid_NetworkBrand.selectedRows().forEach(function (item) {
                    $scope.Grid_NetworkBrand.GridCellsMod(item, "Used", false);
                });
            }
        });
        $scope.Grid_NetworkBrand = uiGridCustomService.createGridClassMod($scope, "Grid_NetworkBrand");
        $scope.Grid_NetworkBrand.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'Имя Сети', field: 'NetworkName', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?NetworkName={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { headerTooltip: true, enableCellEdit: false, name: '∑ Сети', width: 100, field: 'MonthlyTurnover', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'Кол-во брендов', width: 100, field: 'PharmacyBrand_Count', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            {
                headerTooltip: true, enableCellEdit: true, width: 100, name: 'Бренд аптеки', field: 'PharmacyBrand', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?NetworkName={{row.entity.NetworkName}}&PharmacyBrand={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { name: 'Одобрено', width: 100, field: 'Used', enableCellEdit: true, type: 'boolean' },
            { name: 'Комментарий', width: 100, cellTooltip: true, enableCellEdit: true, field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { name: 'Ассоциации', width: 100, cellTooltip: true, enableCellEdit: true, field: 'Associations', filter: { condition: uiGridCustomService.condition } },
            { name: 'Франшиза', width: 100, cellTooltip: true, enableCellEdit: true, field: 'Franchise', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid_NetworkBrand.SetDefaults();

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/NetworkBrand_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.NetworkBrand_Search();
            return 1;
        });
    };
    $scope.NetworkBrand_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/NetworkBrand_Search/',
                data: JSON.stringify({})
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_NetworkBrand.Options.data = response.data.Data.NetworkBrand;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.NetworkBrand_Search = function () {
        if ($scope.Grid_NetworkBrand.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.NetworkBrand_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.NetworkBrand_Search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.NetworkBrand_Search_AC();
        }

    };
    $scope.NetworkBrand_Save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/NetworkBrand_Save/',
                data: JSON.stringify({
                    array: $scope.Grid_NetworkBrand.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.NetworkBrand_Search_AC();
                    }
                    else {
                        $scope.Grid_NetworkBrand.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    ////////////////////////////// NetworkBrand для Окончание

    ////////////////////////////// Network для Старт
    $scope.Network_Init = function () {
        hotkeys.bindTo($scope).add({
            combo: 'shift+e',
            description: 'заменить Ассоциацию',
            callback: function (event) {
                navigator.clipboard.readText()
                    .then(text => {
                        $scope.Grid_Network.selectedRows().forEach(function (item) {
                            $scope.Grid_Network.GridCellsMod(item, "Associations", text);
                        });
                        $scope.Grid_Network.Refresh();
                    }).catch(err => {
                        // возможно, пользователь не дал разрешение на чтение данных из буфера обмена
                        console.log('Something went wrong', err);
                    });
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+v',
            description: 'добавить Ассоциацию',
            callback:
                function (event) {
                    navigator.clipboard.readText()
                        .then(text => {
                            $scope.Grid_Network.selectedRows().forEach(function (item) {
                                var text1 = text;
                                if (item.Associations.length > 1)
                                    text1 = item.Associations + ', ' + text;
                                $scope.Grid_Network.GridCellsMod(item, "Associations", text1);
                            });
                            $scope.Grid_Network.Refresh();
                        }).catch(err => {
                            // возможно, пользователь не дал разрешение на чтение данных из буфера обмена
                            console.log('Something went wrong', err);
                        });
                }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+d',
            description: 'убрать Ассоциацию',
            callback: function (event) {
                $scope.Grid_Network.selectedRows().forEach(function (item) {
                    $scope.Grid_Network.GridCellsMod(item, "Associations", "");
                });
            }
        });

        hotkeys.bindTo($scope).add({
            combo: 'shift+r',
            description: 'добавить Франшизу',
            callback:
                function (event) {
                    navigator.clipboard.readText()
                        .then(text => {
                            $scope.Grid_Network.selectedRows().forEach(function (item) {
                                $scope.Grid_Network.GridCellsMod(item, "Franchise", text);
                            });
                            $scope.Grid_Network.Refresh();
                        }).catch(err => {
                            // возможно, пользователь не дал разрешение на чтение данных из буфера обмена
                            console.log('Something went wrong', err);
                        });
                }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+f',
            description: 'убрать Франшизу',
            callback: function (event) {
                $scope.Grid_Network.selectedRows().forEach(function (item) {
                    $scope.Grid_Network.GridCellsMod(item, "Franchise", "");
                });
            }
        });
        $scope.Grid_Network = uiGridCustomService.createGridClassMod($scope, "Grid_Network");

        $scope.Grid_Network.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, pinnedLeft: true, enableCellEdit: false, width: 100, name: 'Имя Сети', field: 'Value', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/GS?NetworkName={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },
            { name: 'EntityInn', pinnedLeft: true, width: 100, cellTooltip: true, enableCellEdit: false, field: 'EntityInn', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Текущий период', width: 100, field: 'Period', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { headerTooltip: true, enableCellEdit: false, name: '∑ Сети', width: 100, field: 'MonthlyTurnover', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: false, name: 'Кол-во точек', width: 100, field: 'CountPharmacy', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'Ассоциации', width: 100, cellTooltip: true, enableCellEdit: true, headerCellClass: 'Green', field: 'Associations', filter: { condition: uiGridCustomService.condition } },
            { name: 'Франшиза', width: 100, cellTooltip: true, enableCellEdit: true, headerCellClass: 'Green', field: 'Franchise', filter: { condition: uiGridCustomService.condition } },
            { name: 'Описание компании', width: 100, cellTooltip: true, enableCellEdit: true, headerCellClass: 'Green', field: 'CompanyDescription', filter: { condition: uiGridCustomService.condition } },
            { name: 'Бренд', width: 100, cellTooltip: true, enableCellEdit: false, field: 'Brand', filter: { condition: uiGridCustomService.condition } },
            { name: 'Год регистрации', width: 100, cellTooltip: true, enableCellEdit: true, headerCellClass: 'Green', field: 'RegistrationYear', filter: { condition: uiGridCustomService.condition } },
            { name: 'Сайт', width: 100, cellTooltip: true, enableCellEdit: false, field: 'Website', filter: { condition: uiGridCustomService.condition } },
            { name: 'Должность руководителя', width: 100, cellTooltip: true, enableCellEdit: true, field: 'TopManagerPosition', filter: { condition: uiGridCustomService.condition } },
            { name: 'Ф.И.О. руководителя', width: 100, cellTooltip: true, enableCellEdit: true, field: 'TopManagerName', filter: { condition: uiGridCustomService.condition } },
            { name: 'Ф.И.О. владельца/совладельца', width: 100, cellTooltip: true, enableCellEdit: true, field: 'OwnerName', filter: { condition: uiGridCustomService.condition } },
            { name: 'Юр. адрес головного офиса', width: 100, cellTooltip: true, enableCellEdit: true, field: 'HeadOfficeLegalAddress', filter: { condition: uiGridCustomService.condition } },
            { name: 'Факт. адрес головного офиса', width: 100, cellTooltip: true, enableCellEdit: true, field: 'HeadOfficeActualAddress', filter: { condition: uiGridCustomService.condition } },
            { name: 'Телефон', width: 100, cellTooltip: true, enableCellEdit: true, field: 'Phone', filter: { condition: uiGridCustomService.condition } },
            { name: 'E-mail', width: 100, cellTooltip: true, enableCellEdit: true, field: 'Email', filter: { condition: uiGridCustomService.condition } },
            { name: 'Наименование дистрибьюторов, ТОП 5', width: 100, cellTooltip: true, enableCellEdit: true, field: 'Top5Distributors', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition } },
            { name: 'СТМ, наименование брендов', width: 100, cellTooltip: true, enableCellEdit: true, field: 'STM_Brands', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition } },
            { name: 'Прочая информация', width: 100, cellTooltip: true, enableCellEdit: true, field: 'OtherInformation', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition } },
            { name: 'Комментарий', width: 100, cellTooltip: true, enableCellEdit: true, field: 'Comment', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, enableCellEdit: true, name: '∑Объем отпуска препаратов по льготным рецептам', width: 100, field: 'PreferentialRecipesSalesSum', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: 'Средний чек, руб. (по анкете)', width: 100, field: 'AverageReceipt', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: 'кол-во SKU, суммарно продающихся в сетевой/несетевой аптеке', width: 100, field: 'SKUTotalCount', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: '%Rx', width: 100, field: 'Rx_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: '%ОТС', width: 100, field: 'OTC_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: '%БАД', width: 100, field: 'BAD_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: '%ДОП', width: 100, field: 'Other_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: '%СТМ', width: 100, field: 'STM_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: '%ECom', width: 100, field: 'Ecom_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
            { headerTooltip: true, enableCellEdit: true, name: '∑Общий объем продаж', width: 100, field: 'TotalSalesSum', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM }


        ];
        $scope.Grid_Network.SetDefaults();

        $scope.Grid_Network.Options.showTreeExpandNoChildren = false;
        //        $scope.Grid_Network.Options.enableRowSelection = true;
        //        $scope.Grid_Network.Options.enableRowHeaderSelection = true;
        /*
                $scope.Grid_Network.Options.expandableRowTemplate = '<div ui-grid="row.entity.subGridOptions" style="height: 150px;"></div>';
                $scope.Grid_Network.Options.expandableRowHeight = 120;
                $scope.Grid_Network.Options.expandableRowScope = {
                    subGridVariable: 'subGridScopeVariable'
                };
        */
        //enableRowSelection: true,
        //expandableRowTemplate: 'expandableRowTemplate.html'

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GS/Network_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.Network_Search();
            return 1;
        });
    };
    $scope.Network_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Network_Search/',
                data: JSON.stringify({})
            }).then(function (response) {
                if (response.data.Success) {
                    var data = response.data.Data.spr_NetworkNameView;

                    for (i = 0; i < data.length; i++) {
                        if (data[i].treeLevel >= 0)
                            data[i].$$treeLevel = data[i].treeLevel;
                        //                        data[i].subGridOptions = {
                        //                            columnDefs: [{ name: 'Текущий период', width: 100, field: 'period', headerCellClass: 'PapayaWhip', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
                        //                                { enableCellEdit: false, name: '∑Объем отпуска препаратов по льготным рецептам', width: 100, field: 'PreferentialRecipesSalesSum', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: 'Средний чек, руб. (по анкете)', width: 100, field: 'AverageReceipt', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: 'кол-во SKU, суммарно продающихся в сетевой/несетевой аптеке', width: 100, field: 'SKUTotalCount', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: '%Rx', width: 100, field: 'Rx_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: '%ОТС', width: 100, field: 'OTC_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: '%БАД', width: 100, field: 'BAD_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: '%ДОП', width: 100, field: 'Other_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: '%СТМ', width: 100, field: 'STM_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: '%ECom', width: 100, field: 'Ecom_Share', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM },
                        //                                { enableCellEdit: false, name: '∑Общий объем продаж', width: 100, field: 'TotalSalesSum', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_SUM }
                        //],
                        //                            data: data[i].spr_NetworkName_Periods,
                        //                            showGridFooter:false,
                        //                            disableRowExpandable: !(data[i].spr_NetworkName_Periods.length>0)
                        //                        };
                    }
                    $scope.Grid_Network.Options.data = data;

                    $scope.Grid_Network.gridApi.expandable.expandAllRows();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Network_Search = function () {
        if ($scope.Grid_Network.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Network_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Network_Search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Network_Search_AC();
        }

    };
    $scope.Network_Save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GS/Network_Save/',
                data: JSON.stringify({
                    array: $scope.Grid_Network.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.Network_Search_AC();
                    }
                    else {
                        $scope.Grid_Network.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Network_FromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GS/Network_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.Network_Search_AC();
            }, function (response) {
                $scope.Grid_Network.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };
    ////////////////////////////// Network для Окончание

}
