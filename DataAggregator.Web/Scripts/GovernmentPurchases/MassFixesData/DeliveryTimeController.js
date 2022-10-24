angular
    .module('DataAggregatorModule')
    .controller('DeliveryTimeController', ['messageBoxService', '$scope', '$http', '$uibModal', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', '$window', DeliveryTimeController]);

function DeliveryTimeController(messageBoxService, $scope, $http, $uibModal, uiGridCustomService, uiGridConstants, formatConstants, $window) {

    $scope.OnClose = function () {
        $http({
            method: 'POST',
            url: '/DeliveryTime/ResetLock/',
            data: JSON.stringify({ typeLock: "DeliveryTime" })
        });
    };


    $scope.$on('$locationChangeStart', function (event) {
        $scope.OnClose();
        //event.preventDefault();
       /* var answer = confirm("Are you sure you want to leave this page?")
        if (!answer) {
            event.preventDefault();
        }*/
    });

    function isObject(obj) {
        var type = typeof obj;
        return type === 'function' || type === 'object' && !!obj;
    }
    function Copy(src) {
        let target = {};
        for (let prop in src) {
            if (src.hasOwnProperty(prop)) {
                // if the value is a nested object, recursively copy all it's properties
                if (isObject(src[prop])) {
                    target[prop] = Copy(src[prop]);
                } else {
                    target[prop] = src[prop];
                }
            }
        }
        return target;
    }
    $scope.ADD=function() {
        var selectedRows = $scope.gridApi_Grid.selection.getSelectedRows();
        var Id = 0;
        if (selectedRows.length >= 1) {
            $scope.NeedSave = true;

            var rowEntity = {
                PurchaseId: selectedRows[0].PurchaseId,
                PurchaseNumber: selectedRows[0].PurchaseNumber,
                URL: selectedRows[0].URL,
                DeliveryTime: selectedRows[0].DeliveryTime,
                DateBegin: selectedRows[0].DateBegin,
                DateEnd: selectedRows[0].DateEnd,      
                CustomerFederationSubject: selectedRows[0].CustomerFederationSubject,
                CustomerShortName: selectedRows[0].CustomerShortName,
                PurchaseName: selectedRows[0].PurchaseName,
                LotSum: selectedRows[0].LotSum,
                ContractDateBegin: selectedRows[0].ContractDateBegin,
                ContractDateEnd: selectedRows[0].ContractDateEnd,
                idDTI: null,
                DateStartDTI: selectedRows[0].DateStartDTI,
                DateEndDTI: null,
                DeliveryTimePeriodId: null,
            count_DTI: selectedRows[0].count_DTI + 1,
                DayDelta: 0
            };
            rowEntity["@modify"] = true;
            selectedRows[0].count_DTI = rowEntity.count_DTI;

            $scope.Grid.Options.data.push(rowEntity);

            $scope.gridApi_Grid.core.clearAllFilters();//++

            $scope.gridApi_Grid.grid.getColumn('Id закупки').filters[0] = {
                term: rowEntity.PurchaseId
            };
        }
    };
    $scope.Init = function () {
        $scope.NeedSave = false;
        $scope.Set_DateEndDTI = new dateClass();
        $scope.Set_DateStartDTI = new dateClass();
        $scope.format = "dd.MM.yyyy";
        $scope.DayPlus = 30;
        $scope.DeliveryTime = "";
        $scope.Grid = uiGridCustomService.createGridClass($scope, 'Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Grid.Options.noUnselect = true;//,\'ng-isolate-scope\':1==1
        $scope.Grid.Options.rowTemplate = '<div ng-class="{\'modify\' : row.entity[\'@modify\']==true, \'multi\' : row.entity[\'count_DTI\']>1, \'error\' : row.entity[\'DateStartDTI\']<row.entity[\'DateEnd\'], \'error\' : row.entity[\'DateEndDTI\']<row.entity[\'DateStartDTI\']}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';

        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Grid.Options.onRegisterApi = function (gridApi) {//если строка меняется тоей ставиться флажок modify
            $scope.gridApi_Grid = gridApi;
            $scope.gridApi_Grid.selection.on.rowSelectionChanged($scope, function (row) {
                $scope.DeliveryTime = "";   
                var selectedRows = $scope.gridApi_Grid.selection.getSelectedRows();
                if (selectedRows.length >= 1) {
                    $scope.DeliveryTime = row.entity.DeliveryTime;
                    $scope.IsRowSelection = true;
                }
                else {
                    $scope.IsRowSelection = false;
                }
            });

            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.NeedSave = true;
                    }
                }
                if (colDef.field === 'DateStartDTI') {
                    $scope.DateStartDTI_Logic(rowEntity);
                }
                if (colDef.field === 'DateEndDTI') {
                    $scope.DateEndDTI_Logic(rowEntity);
                }
                if (colDef.field === 'DateStartDTI' || colDef.field === 'DateEndDTI') {
                    var first = new Date(rowEntity["DateStartDTI"]);
                    var second = new Date(rowEntity["DateEndDTI"]);
                    rowEntity["DayDelta"] = Math.round((second - first) / (1000 * 60 * 60 * 24));
                }
            });
        };

        return $http({
            method: 'POST',
            url: '/DeliveryTime/InitMiniClassifiers/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.DeliveryTimePeriod = response.data;



        $scope.Grid.Options.columnDefs =
            [//'/#/Classifier/Manufacturer/Edit?id=0', '_blank'
                { name: 'Id закупки', field: 'PurchaseId', filter: { condition: uiGridCustomService.condition }, type: 'number' },
            { name: 'Номер закупки', field: 'PurchaseNumber', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
                { name: 'Ссылка на закупку', field: 'URL', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateURL },
                { name: 'Сроки поставки', field: 'DeliveryTime', filter: { condition: uiGridCustomService.condition } },
            { name: 'DateBegin', field: 'DateBegin', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'DateEnd', field: 'DateEnd', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
                { name: 'Регион Заказчика', field: 'CustomerFederationSubject', filter: { condition: uiGridCustomService.condition } },
                { name: 'Сокращенное наименование Заказчика', field: 'CustomerShortName', filter: { condition: uiGridCustomService.condition } },
                { name: 'Наименование закупки', field: 'PurchaseName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
                { name: 'Сумма лота', field: 'LotSum', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
                { name: 'ContractDateBegin', field: 'ContractDateBegin', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
                { name: 'ContractDateEnd', field: 'ContractDateEnd', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'idDTI', field: 'idDTI', filter: { condition: uiGridCustomService.condition }, type: 'number' },

            {
                enableCellEdit: true, headerCellClass: 'Edit', name: 'DateStart(из периода)', field: 'DateStartDTI', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE},
            { enableCellEdit: true, headerCellClass: 'Edit', name: 'DateEnd(из периода)', field: 'DateEndDTI', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },

            { enableCellEdit: true, headerCellClass: 'Edit', name: 'Count', field: 'Count', filter: { condition: uiGridCustomService.condition }, type: 'number' },
            {
                enableCellEdit: true, headerCellClass: 'Edit', width: 300, name: 'DeliveryTimePeriod', field: 'DeliveryTimePeriodId', filter: { condition: uiGridCustomService.SPRCondition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                cellFilter: 'griddropdownSSA:this', editType : 'dropdown',
                editDropdownOptionsArray: $scope.DeliveryTimePeriod,
                editDropdownIdLabel: 'code', editDropdownValueLabel: 'status'
            },
            { name: 'count_DTI', field: 'count_DTI', filter: { condition: uiGridCustomService.condition }, type: 'number' },
            { name: 'Дней', field: 'DayDelta', filter: { condition: uiGridCustomService.condition }, type: 'number' }
                ];
        });
    };
    function DateToString(value) {
        var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        ret = null;
        if (value !== null) {
            ret = value.toLocaleDateString("en-US", options);
        }
        return ret;
    }
    $scope.Save = function (action) {
        var array_upd = [];
        $scope.Grid.Options.data.forEach(function (item, i, arr) {
            if (item["@modify"] === true) {
                if (item.DateStartDTI < item.DateEnd) {
                    alert('DateStart не может быть ранее DateEnd (из закупки)');
                    return;
                }
                if (item.DateEndDTI < item.DateStartDTI) {
                    alert('DateEnd (из периода) не может быть ранее DateStart');
                    return;
                }
                itemClone = JSON.parse(JSON.stringify(item));

                itemClone.DateStartDTI = DateToString(item.DateStartDTI);
                itemClone.DateEndDTI = DateToString(item.DateEndDTI);

                array_upd.push(itemClone);
            }
        });
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/DeliveryTime/Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.getDataByFilter();
                        }
                        else {
                            $scope.NeedSave = false;
                            /*$scope.Grid.Options.data.forEach(function (item, i, arr) {
                                if (item["@modify"] === true) {
                                    item["@modify"] = false;
                                }
                            });*/
                            alert("Сохранил");
                            $scope.SetData(response.data.data);    
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.Search = function () {
        if ($scope.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.getDataByFilter();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.getDataByFilter();
        }
    };
    $scope.getDataByFilter = function () {
        //клонируем фильтр
        var filterToServer = JSON.parse(JSON.stringify($scope.filterwhere));

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DeliveryTime/GetDataByFilter',
            data: {
                filter: filterToServer
            }
        }).then(function (response) {
            $scope.SetData(response.data.data);            
            if (response.data.count === 50000) {
                messageBoxService.showInfo('Показано 50 000 записей! Возможно, это не все данные.');
            }
        }, function () {
            messageBoxService.showError('Не удалось загрузить отчёт!');
        });
    };
    $scope.ToDate = function (value) {
        var ret = new Date(value);
        ret = new Date(ret.getTime() - 60000 * ret.getTimezoneOffset());
        ret.setHours(0);
        ret.setMinutes(0);
        return ret;
    };

    $scope.SetData = function (data) {
        for (i = 0; i < data.length; i++) {
            if (data[i].DateStartDTI !== null)
                data[i].DateStartDTI = $scope.ToDate(data[i].DateStartDTI);
            if (data[i].DateEndDTI !== null)
                data[i].DateEndDTI = $scope.ToDate(data[i].DateEndDTI);
            if (data[i].DateEnd !== null)
                data[i].DateEnd = $scope.ToDate(data[i].DateEnd);
        }
        $scope.Grid.Options.data = data;
    };
    $scope.openFilterDialog = function () {
        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/filters/_filtersView.html',
            controller: 'filtersController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: {
                    filterall: $scope.filterall
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                $scope.filterwhere = data.filterwhere;
                $scope.filterall = data.filterall;
                $scope.Search();
            },
            // cancel
            function () {
            }
        );
    };   


    $scope.DateEndDTI_Set = function () {
        var selectedRows = $scope.gridApi_Grid.selection.getSelectedRows();
        selectedRows.forEach(function (item) {
            item.DateEndDTI = $scope.ToDate($scope.Set_DateEndDTI.Value);
            $scope.DateEndDTI_Logic(item);
            item.DayDelta = Math.round((item.DateEndDTI - item.DateStartDTI) / (1000 * 60 * 60 * 24));
            item["@modify"] = true;
            $scope.NeedSave = true;
        });        
    };
    $scope.DateStartDTI_Set = function () {
        var selectedRows = $scope.gridApi_Grid.selection.getSelectedRows();
        selectedRows.forEach(function (item) {
            item.DateStartDTI = $scope.ToDate($scope.Set_DateStartDTI.Value);
            $scope.DateStartDTI_Logic(item);
            item.DayDelta = Math.round((item.DateEndDTI - item.DateStartDTI) / (1000 * 60 * 60 * 24));
            item["@modify"] = true;
            $scope.NeedSave = true;
        });
    };
    $scope.DateStartDTI_Logic = function (row) {
        if (row.DateStartDTI < row.DateEnd) {
            row.DateStartDTI = row.DateEnd;
        }
    };
    $scope.DateEndDTI_Logic = function (row) {
        if (row.DateStartDTI > row.DateEndDTI) {
            //row.DateEndDTI = $scope.ToDate(row.DateStartDTI.getDate() + 10);
            row.DateEndDTI = null;
        }
    };
    $scope.DateEndDTI_SetDayp = function () {
        var selectedRows = $scope.gridApi_Grid.selection.getSelectedRows();
        selectedRows.forEach(function (item) {
            item.DateEndDTI = $scope.ToDate(item.DateStartDTI);
            item.DateEndDTI.setDate(item.DateEndDTI.getDate() + 1 * $scope.DayPlus);
            $scope.DateEndDTI_Logic(item);
            item.DayDelta = Math.round((item.DateEndDTI - item.DateStartDTI) / (1000 * 60 * 60 * 24));
            item["@modify"] = true;
            $scope.NeedSave = true;
        });
    };
    $scope.SetPeriodAuto = function () {
        var selectedRows = $scope.gridApi_Grid.selection.getSelectedRows();
        selectedRows.forEach(function (item) {
            if (item.DayDelta < 45) {
                item.DeliveryTimePeriodId = 10;
                item["@modify"] = true;
                $scope.NeedSave = true;
            }
            if (item.DayDelta >= 45) {
                item.DeliveryTimePeriodId = 1;
                item["@modify"] = true;
                $scope.NeedSave = true;
            }
            /*
1	1 раз в месяц
2	1 раз в неделю
3	1 раз в квартал
4	1 раз в полугодие
5	1 раз в год
6	1 раз в 2 месяца
7	2 раза в месяц
8	2 раза в неделю
9	2 раза в квартал
10	Одномоментно*/
        });
    };
}



