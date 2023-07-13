angular
    .module('DataAggregatorModule')
    .controller('GTINController', [
        '$scope', '$route', '$http', '$uibModal', '$interval', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', '$q', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', 'Upload', 'cfpLoadingBar', 'messageBoxService' , GTINController])
    .filter('griddropdownSSS', function () {
        return function (input, context) {

            try {

                var map = context.col.colDef.editDropdownOptionsArray;
                var idField = context.col.colDef.editDropdownIdLabel;
                var valueField = context.col.colDef.editDropdownValueLabel;
                //var initial = context.row.entity[context.col.field];
                if (typeof map !== "undefined") {
                    for (var i = 0; i < map.length; i++) {
                        if (map[i][idField] == input) {
                            return map[i][valueField];
                        }
                    }
                }
                return input;

            } catch (e) {

            }
        };
    });
function GTINController($scope, $route, $http, $uibModal, $interval, commonService, messageBoxService, hotkeys, $timeout, $q, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, Upload, cfpLoadingBar, messageBoxService, userService) {
    $scope.Source = [];
    $scope.SourceLabel = [];
    $scope.DrugTypeLabel = [];
    $scope.selectedCount = 0;
    $scope.IsRowSelection = false;
    $scope.GTIN_Init = function () {
       
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GTIN//GTIN_Init/'
            }).then(function (response) {
                if (response.data.Success) {               
                    Array.prototype.push.apply($scope.Source, response.data.Data.Source);
                    Array.prototype.push.apply($scope.SourceLabel, response.data.Data.Source.map(function (obj) {
                        var rObj = { 'value': obj.Id, 'label': obj.Name };
                        return rObj;
                    }));
                    for (var i = 0; i < response.data.Data.DrugType.length; i++) {
                        Array.prototype.push.apply($scope.DrugTypeLabel, [{ 'value': response.data.Data.DrugType[i], 'label': response.data.Data.DrugType[i] }]);                      
                    };                                       
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        $scope.GTIN_Grid = uiGridCustomService.createGridClassMod($scope, 'GTIN_Grid');
        $scope.GTIN_Grid.Options.showGridFooter = true;
        $scope.GTIN_Grid.Options.multiSelect = true;
        $scope.GTIN_Grid.Options.modifierKeysToMultiSelect = true;
        $scope.GTIN_Grid.Options.columnDefs = [
            { enableCellEdit: false, name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: false, width: 150, name: 'Source',
                field: 'SourceId',
                headerCellClass: 'Edit',
                cellFilter: 'griddropdownSSS:this',
                //  cellFilter:'mapDataSourceType:this',
                editType: 'dropdown',
                filter:
                {
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: $scope.SourceLabel
                },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.Source,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name' //   
                //  , editDropdownFilter: 'translate'
            },
            { name: 'GTIN', field: 'GTIN', filter: { condition: uiGridCustomService.condition } },
            { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, name: 'IsActive', field: 'IsActive', filter: {
                    condition: uiGridCustomService.booleanConditionX
                    , type: uiGridConstants.filter.SELECT
                    , selectOptions: [{ value: '1', label: 'Да' }, { value: '0', label: 'Нет' }, { value: '-', label: 'Не проставлено' }]
                }, type: 'bit'
            },
            {
                enableCellEdit: false, name: 'IsValid', field: 'IsValid', filter: {
                    condition: uiGridCustomService.booleanConditionX
                    , type: uiGridConstants.filter.SELECT
                    , selectOptions: [{ value: '1', label: 'Да' }, { value: '0', label: 'Нет' }, { value: '-', label: 'Не определено' }]
                }, type: 'bit'
            },
          //  { name: 'Status', field: 'Status', filter: { condition: uiGridCustomService.condition } },
            {  enableCellEdit: false, name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }},
            {  enableCellEdit: false, name: 'GoodsId', field: 'GoodsId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }},
            {  enableCellEdit: false, name: 'TradeName', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
            {  enableCellEdit: false, name: 'DrugDescription', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
            {  enableCellEdit: false, name: 'Manufacturer', field: 'Manufacturer', filter: { condition: uiGridCustomService.condition } },
            {  enableCellEdit: false, name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            {  enableCellEdit: false, name: 'OwnerTradeMark', field: 'OwnerTradeMark', width: 300, filter: { condition: uiGridCustomService.condition } },
            {  enableCellEdit: false, name: 'PackerId', field: 'PackerId', filter: { condition: uiGridCustomService.condition } },
            {  enableCellEdit: false, name: 'Packer', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
            {  enableCellEdit: false, name: 'Пользователь', field: 'UserName', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: false, name: 'Used', field: 'Used', filter: {
                    condition: uiGridCustomService.booleanConditionX
                    ,type: uiGridConstants.filter.SELECT
                    , selectOptions: [{ value: '1', label: 'Да' }, { value: '0', label: 'Нет' }]
                }, type: 'bit'
            },
            {
                enableCellEdit: false,
                name: 'DrugType', field: 'DrugType', filter: {
                    condition: uiGridCustomService.condition,
                     type: uiGridConstants.filter.SELECT
                    , selectOptions: $scope.DrugTypeLabel

                }
            },
            { name: 'OperatorComment', field: 'OperatorComment', filter: { condition: uiGridCustomService.condition }, enableCellEdit: true },
            {
                enableCellEdit: false, width: 100, name: 'Search', field: 'Search', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="{{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>'
            },

        ];
        $scope.GTIN_Grid.SetDefaults();



    };
   

    $scope.IsActive = function (idss) {
        var ref_this = $(idss).hasClass('active');
        return ref_this;

    };
    $scope.GTINs_search_AC = function () {
         $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GTIN//GTINs_search/',
                data: JSON.stringify({ searchText: null })
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.GTIN_Grid.Options.data = response.data.Data.GTINs_View;
                
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };



    $scope.GTINs_search = function () {
        if ($scope.GTIN_Grid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.GTINs_save();
                    },
                    function (result) {
                        if (result === 'no') {                            
                            $scope.GTINs_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.GTINs_search_AC();
        }

    };



    $scope.GTINs_save = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GTIN//GTINs_save/',
                data: JSON.stringify({
                    array_GTINs: $scope.GTIN_Grid.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                        $scope.GTIN_Grid.ClearModify();                                   
                        $scope.GTINs_search_AC();
                        alert("Сохранил");                  
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SetIsActive = function (val) {
        var selectedRows = $scope.GTIN_Grid.selectedRows();      
        if (selectedRows != null) {
       
            selectedRows.forEach(function (item) {             
                $scope.GTIN_Grid.GridCellsMod(item, "IsActive", val);              
            });
        };
    };

    $scope.Import_GTIN_NEW_from_Excel = function (file) {

        if (file == null)
            return;
        var formData = new FormData();
      
        formData.append('uploads', file);
      
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GTIN//Import_GTIN_NEW_from_Excel/',
                data: formData ,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                if (response.data.Success) {                   
                    alert('Файл загружен');
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });

    };





}