angular
    .module('DataAggregatorModule')
    .controller('GlobalController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', GlobalController])
    .filter('griddropdownSPR2', function () {
        return function (input, map, idField, valueField, initial) {
            if (typeof map !== "undefined") {
                for (var i = 0; i < map.length; i++) {
                    if (map[i][idField] == input) {
                        return map[i][valueField];
                    }
                }
            } else if (initial) {
                return initial;
            }
            return input;
        };
    });

function GlobalController($scope, $route, $http, $uibModal, commonService, messageBoxService, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.user = userService.getUser();
    $scope.Name = "нет данных";
    $scope.format = 'dd.MM.yyyy';
    ///////////////////////////////Старт
    $scope.Init = function () {
        $scope.NeedSave = false;
        $scope.Name = $route.current.params["name"];
        $scope.Fields = [];
        $scope.Filters = [];
        $scope.CMD = [];
        $scope.SPR = [];
        $scope.gridApi__Grid = null;
        $scope.IsActivReport = true;

        $scope.format = 'dd.MM.yyyy';
        $scope.currentperiod = null;

        $scope.Grid = uiGridCustomService.createGridClass($scope, 'Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Grid.Options.rowTemplate = '<div ng-class="{\'modify\' : row.entity[\'@modify\']==true}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';
        $scope.Grid.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi__Grid = gridApi;

            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.NeedSave = true;
                    }
                }
            });
        };
        $scope.params= JSON.stringify({ model: $route.current.params });
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Global/Init/',
            data: JSON.stringify({ name: $scope.Name, param: $scope.params})
        }).then(function (response) {
            document.title = response.data.Data.title;
            $scope.InitArrays(response.data.Data);    
            
            if ($scope.RunningProcess != undefined) {
                messageBoxService.showInfo($scope.RunningProcess, 'Отчеты');
                return;
            }
            if (response.data.Data.Search_now === true || $scope.Filters.length===0) {
                $scope.Search();
            }

            if (response.data.Data.Data !== undefined) {
                var ddd = JSON.parse(response.data.Data.Data);  
                document.title = response.data.Data.Name;
                $scope.IsActivReport = false;
                $scope.SetData(ddd);
            }

        }, function (response) {
            console.log(response);
            messageBoxService.showError(response.data.message);
        });

        return 0;
    };
    $scope.InitArrays = function (Data) {
        if (Data.CMD !== undefined && Data.CMD.length > 0) {
            $scope.CMD.splice(0, $scope.CMD.length);
            Array.prototype.push.apply($scope.CMD, Data.CMD);
        }
        if (Data.SPR !== undefined && Data.SPR.length > 0) {
            $scope.SPR.splice(0, $scope.SPR.length);
            Array.prototype.push.apply($scope.SPR, Data.SPR);
        }

        if (Data.Filters !== undefined && Data.Filters.length>0) {
            $scope.Filters.splice(0, $scope.Filters.length);
            Array.prototype.push.apply($scope.Filters, Data.Filters);

            $scope.Filters.forEach(function (item, i, arr) {
                if (item.sType === 'date') {
                    item.ValueDT = new dateClass();
                    item.ValueDT.setToday();
                }
            });
        }

        if (Data.Fields !== undefined && Data.Fields.length > 0) {
            $scope.Fields.splice(0, $scope.Fields.length);
            Array.prototype.push.apply($scope.Fields, Data.Fields);
            $scope.Grid.Options.columnDefs = new Array();
            $scope.Fields.forEach(function (item, i, arr) {

                var item_new = [];
                item_new = {
                    name: item.DisplayName, field: item.Name,
                    enableCellEdit: item.IsEdit,
                    filter: { condition: uiGridCustomService.condition }
                };
                if (item.Name === 'PurchaseNumber') {
                    item_new.cellTemplate = '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>';
                }
                if (item.Name === 'ReestrNumber') {
                    item_new.cellTemplate = '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?ReestrNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>';
                }
                if (item.sType === 'bool') {
                    item_new.type = 'boolean';
                    item_new.cellFilter = formatConstants.FILTER_DATE;
                    item_new.filter.condition = uiGridCustomService.condition;
                }
                if (item.sType === 'int') {
                    item_new.type = 'number';
                    item_new.cellFilter = formatConstants.FILTER_INT_COUNT;
                    item_new.filter.condition = uiGridCustomService.numberCondition;
                }
                if (item.sType === 'double') {
                    item_new.type = 'number';
                    item_new.cellFilter = formatConstants.FILTER_PRICE;
                    item_new.filter.condition = uiGridCustomService.numberCondition;
                }
                if (item.sType === 'date') {
                    item_new.type = 'date';
                    item_new.cellFilter = formatConstants.FILTER_DATE;
                    item_new.filter.condition = uiGridCustomService.condition;
                }
                if (item.sType === 'datetime') {
                    item_new.type = 'date';
                    item_new.cellFilter = formatConstants.FILTER_DATE_TIME;
                    item_new.filter.condition = uiGridCustomService.condition;
                }
                if (item.sType === 'SPR') {
                    //item_new.type = uiGridConstants.filter.SELECT;
                    item_new.editType = 'dropdown';
                    item_new.editableCellTemplate = 'ui-grid/dropdownEditor';
                    item_new.editDropdownIdLabel = 'Id';
                    item_new.editDropdownValueLabel = 'Value';
                    //item_new.editDropdownFilter = 'translate';
                    item_new.cellFilter = 'griddropdownSSA:this';
                    item_new.filterCellFiltered = true;
                    ///item_new.cellFilter = 'griddropdownSPR2:editDropdownOptionsArray:editDropdownIdLabel:editDropdownValueLabel:row.entity.' + item.Name+'';
                    $scope.SPR.forEach(function (itemspr, ispr, arrspr) {
                        if (itemspr.Name === item.SPR) {
                            item_new.editDropdownOptionsArray = itemspr.Data;
                        }
                    });
                    item_new.filter = { condition: uiGridCustomService.SPRCondition };
                }
                if (item.IsEdit === true) {
                    item_new.headerCellClass = 'IsEdit';
                }
                $scope.Grid.Options.columnDefs.push(item_new);
            });
        }
        if (Data.RunningProcess != undefined) {
            $scope.RunningProcess = Data.RunningProcess;
        }
    };
    $scope.SetData = function (data) {
        $scope.Fields.forEach(function (item, i, arr) {
            if (item.sType === 'date') {
                for (i = 0; i < data.length; i++) {
                    if (data[i][item.Name] !== null)
                        data[i][item.Name] = $scope.ToDate(data[i][item.Name],0);
                }
            }
            if (item.sType === 'datetime') {
                for (i = 0; i < data.length; i++) {
                    if (data[i][item.Name] !== null)
                        data[i][item.Name] = $scope.ToDate(data[i][item.Name],1);
                }
            }
        });

        $scope.Grid.Options.data = data;
    };
    $scope.Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Global/Load/',
                data: JSON.stringify({ name: $scope.Name, param: $scope.params, Filters:$scope.Filters })
            }).then(function (response) {
                var data = response.data.Data.Data;
                $scope.InitArrays(response.data.Data);
                if ($scope.RunningProcess != undefined) {
                    messageBoxService.showInfo($scope.RunningProcess, 'Отчеты');
                    return;
                }
                if (response.data.Data.TypeData === "string")
                {
                    data = JSON.parse(data);
                }
                $scope.SetData(data);
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.ToDate = function (value,wHM) {
        var ret = new Date(value);        
        if (wHM === 0) {
            ret = new Date(ret.getTime() - 60000 * ret.getTimezoneOffset());
            ret.setHours(0);
            ret.setMinutes(0);
        }
        return ret;
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
                            $scope.Search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Search_AC();
        }

    };
    $scope.SearchExcel = function () {
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Global/LoadExcel/',
            data: JSON.stringify({ name: $scope.Name, param: $scope.params, Filters: $scope.Filters }),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            if (response.data.byteLength == 0) {
                messageBoxService.showError("Данные для загрузки файла отсутствуют!");
                return
            }
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'Отчёт.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.GCommand = function (action) {
        if (action.typec === 'post') {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Global/GCommand/',
                    data: JSON.stringify({ name: $scope.Name, action: action.command })
                }).then(function (response) {
                    var data = response.data;
                });
        }
        if (action.typec === 'href') {
            window.open(action.command, '_blank');
        }
        if (action.typec === 'AddNew') {
            
            if (action.command === 'AutoCorrectAmountInfo') {
                var rowEntity = {
                Unit :"",
                 Type: 0
                };
                $scope.Grid.Options.data.push(rowEntity);
            }
            
        }
    };

    $scope.Save = function (action) {
        var array_upd = [];
        $scope.Grid.Options.data.forEach(function (item, i, arr) {
            if (item["@modify"] === true) {
                array_upd.push(item);
            }
        });
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Global/Save' + $scope.Name+'/',
                    data: JSON.stringify({ name: $scope.Name, data: array_upd})
                }).then(function (response) {
                    if (response.data.Data.Success===true) {
                        if (action === "search") {
                            $scope.Search_AC();
                        }
                        else {
                            $scope.NeedSave = false;
                            $scope.Grid.Options.data.forEach(function (item, i, arr) {
                                if (item["@modify"] === true) {
                                    item["@modify"] = false;
                                }
                            });
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.setDate = function (item, event) {
        item.Value = item.ValueDT.Value;
    };


    $scope.query_Init = function () {
        $scope.IsRowSelection = false;
        $scope.user = userService.getUser();
        $scope.CurrentRow;
        $scope.Area = [];
        $scope.CurrentArea = "";
        $scope.Grid_query = uiGridCustomService.createGridClassMod($scope, "Grid_query");
        $scope.Grid_query.onSelectionChanged = onSelectionChanged;

        $scope.Grid_query.Options.columnDefs = [
            { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, name: 'Server', field: 'Server', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, name: 'Query', field: 'Query', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, name: 'Area', field: 'Area', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, name: 'Roles', field: 'Roles', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, name: 'Name', field: 'Name', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, name: 'Filters', field: 'Filters', filter: { condition: uiGridCustomService.condition } }

        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Global/query_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.Area, response.data.Data.Area);

            $scope.query_Search();
            return response.data;
        });
    };
    function onSelectionChanged(row) {
        if (row !== undefined) {
            $scope.CurrentRow = row;
        }
    }
    textChange = function () {
        if ($scope.CurrentRow !== undefined) {
            $scope.CurrentRow.entity["@modify"] = true;
            $scope.Grid_query.NeedSave = true;
        }
    };
    $scope.query_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Global/query_search/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_query.Options.data = data.Data.query;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.query_Search = function () {
        if ($scope.Grid_query.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.query_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.query_Search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.query_Search_AC();
        }

    };
    $scope.query_Save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Global/query_save/',
                data: JSON.stringify({
                    array: $scope.Grid_query.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.query_Search_AC();
                    }
                    else {

                        $scope.Grid_query.ClearModify();
                        $scope.query_Search_AC();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.query_Add = function () {
        var new_element = {
            Id: -1,
            Area: $scope.CurrentArea,
            Server: "",
            Roles: "",
            Filters: "",
            Query: "",
            Name: ""
        };
        $scope.Grid_query.Options.data.push(new_element);
    };
    $scope.query_SetFilterArea = function (Name) {
        $scope.CurrentArea = Name;
        $scope.Grid_query.gridApi.grid.columns[3].filters[0].term = Name;

    };
}
