angular.module('ui.grid.module').
    factory('uiGridCustomService', ['$translate', 'hotkeys', function ($translate, hotkeys) {

        var yes = $translate.instant('YES');
        var no = $translate.instant('NO');


        function setFileName(options, fileName) {
            options.exporterCsvFilename = fileName + '.csv';
            options.exporterPdfFilename = fileName + '.pdf';
            options.exporterExcelFilename = fileName + '.xlsx';
            if (fileName !== undefined && fileName!==null)
                options.exporterExcelSheetName = fileName;
        }

        function setGridName(options, gridName) {
            if (gridName) {
                options.gridName = gridName;
                setFileName(options, gridName);
            }
        }

        function postInit(options, gridName) {
            setGridName(options, gridName);

            if (options.customEnablePagination === undefined)
                options.customEnablePagination = false;

            if (options.customEnableRowSelection === undefined)
                options.customEnableRowSelection = false;

            if (options.customEnableCellSelection === undefined)
                options.customEnableCellSelection = false;
        }

        function CommonGridClassMod($scope, ondblclick) {


            this.selectedItem = null;
            this.gridApi = null;

            this.NeedSave = false;
            this.afterCellEdit = null;
            this.onSelectionChanged = null;
            var ArrayUndo = [];

            var selectionChanged = null;

            this.getSelectedItem = function () {
                return this.selectedItem;
            }

            this.setFirstSelected = function () {
                this.gridApi.grid.modifyRows(this.Options.data);
                this.gridApi.selection.selectRow(this.Options.data[0]);

            }

            this.notifyDataChange = function (COLUMN) {
                this.gridApi.core.notifyDataChange(COLUMN);
            }

            this.Options = {
                customEnableRowSelection: true,
                enableColumnResizing: true,
                enableGridMenu: true,
                enableSorting: true,
                enableFiltering: true,
                enableSelectAll: false,
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                enableCellEdit: false,
                appScopeProvider: $scope,
                enableFullRowSelection: true,
                enableSelectionBatchEvent: true,
                enableHighlighting: true,
                modifierKeysToMultiSelect: true,
                multiSelect: true,
                noUnselect: true,
                rowTemplate: '/Views/Static/GridRow.html'
            };

            this.Options.gridMenuCustomItems=[
                {
                    title: 'Отменить',
                    action: GridUndo,
                    order: 0
                }
            ]
            this.clearSelection = function () {
                // selectedItem = null;
                this.gridApi.selection.clearSelectedRows();
            }
            this.setSelected = function (item) {
                this.gridApi.grid.modifyRows(this.Options.data);
                this.gridApi.selection.selectRow(item);

            }
            Object.defineProperty(this, "selectionChanged", {
                get: function () {
                    return selectionChanged;
                },
                set: function (value) {
                    selectionChanged = value;
                },
                enumerable: true

            });

            
            this.Options.showGridFooter = true;
            this.Options.multiSelect = true;
            this.Options.modifierKeysToMultiSelect = true;
            var s_ondblclick = "";
            if (ondblclick !== null && ondblclick !== undefined) {
                s_ondblclick = 'ng-dblclick="grid.appScope.' + ondblclick +'(col.colDef.field, row.entity)"';
            }
            this.Options.rowTemplate = '<div ng-class="{\'ui-grid-modify\' : row.entity[\'@modify\']==true}"><div ' + s_ondblclick+' ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';

            
            this.SetData = function ( Data) {
                this.Options.data = Data;
                this.NeedSave = false;
                ArrayUndo = [];
            };
            this.Rows = function () {
                return this.Options.data;
            };
            this.GetArrayModify = function () {
                var array_upd = [];
                this.Options.data.forEach(function (item, i, arr) {
                    if (item["@modify"] === true) {
                        array_upd.push(item);
                    }
                });
                return array_upd;
            };
            this.ClearModify = function () {
                this.NeedSave = false;
                ArrayUndo = [];
                this.Options.data.forEach(function (item, i, arr) {
                    if (item["@modify"] === true) {
                        item["@modify"] = false;
                    }
                });
            };
            this.getSelected = function (columnName) {
                if (columnName === undefined || columnName === null)
                    columnName = "Id";
                var ret = [];
                if (this.selectedRows() !== null) {
                    this.selectedRows().forEach(function (item, i, arr) {
                        ret.push(item[columnName])
                    });
                }
                return ret;
            };
            this.selectedRows = function () {
                return this.selectedItem;
            };
            this.IsRowSelection = function () {
                if (this.selectedItem.length > 0)
                    return true;
                return false;
            }
            this.GridCellsMod = function (row, ColumnName, newValue) {
                this.GridLogger(row);//нет смысла редактировать строку по кусочкам нужно её сохранять всю 1 раз
                var oldValue = row[ColumnName];
                var colDef = { field: ColumnName };
                row[ColumnName] = newValue;
                row["@modify"] = true;
                this.NeedSave = true;
                if (this.afterCellEdit !== null) {
                    this.afterCellEdit(row, colDef, newValue, oldValue);
                }
            };
            this.GridLogger = function (row) {
                ArrayUndo.push(row);
            };
            this.SetDefaults = function () {
                var uiGridCustomService = this.parent;
                this.Options.columnDefs.forEach(function (item, i, arr) {
                    item.width = (item.width === undefined) ? 100 : item.width;
                    switch (item.type) {
                        case "boolean":
                            item.filter = { condition: uiGridCustomService.booleanConditionX };
                            if (item.cellTemplate === null || item.cellTemplate === undefined)
                                item.cellTemplate = '<div class="ui-grid-cell-contents"><span  class="glyphicon" ng-class="COL_FIELD===true ? \'ui-grid-true glyphicon-check\' : COL_FIELD===false ? \'ui-grid-false glyphicon-unchecked\' : \'ui-grid-null glyphicon-question-sign\'"></span></div>';
                            if (item.enableCellEdit === true) {
                                item.editableCellTemplate = '<div><form name="inputForm"><label  class="ui-grid-center"><input type="checkbox" style="margin:0px;" ng-class="" ui-grid-editor ng-model="MODEL_COL_FIELD"></label></form></div>';
                                //item.editableCellTemplate = '<div><form name="inputForm" ui-grid-editor ng-model="MODEL_COL_FIELD">' +
                                //    '<div class="glyphicon glyphicon-question-sign" ng-click="MODEL_COL_FIELD=null;"></div>' +
                                //    '<div class="glyphicon glyphicon-unchecked"></div>' +
                                //    '<div class="glyphicon glyphicon-check"></div>' +
                                //    '</form></div>';
                            }
                            //item.cellTemplate = item.editableCellTemplate;
                            break;
                        case "bit":
                            item.filter = (item.filter === undefined) ? { condition: uiGridCustomService.booleanConditionX } : item.filter;
                            if (item.cellTemplate === null || item.cellTemplate === undefined)
                                item.cellTemplate = '<div class="ui-grid-cell-contents"><span  class="glyphicon" ng-class="COL_FIELD===true ? \'ui-grid-true glyphicon-check\' : COL_FIELD===false ? \'ui-grid-false glyphicon-unchecked\' : \'ui-grid-null glyphicon-question-sign\'"></span></div>';
                            if (item.enableCellEdit === true) {
                                item.editableCellTemplate = '<div><form name="inputForm"><label  class="ui-grid-center"><input type="checkbox" style="margin:0px;" ng-class="" ui-grid-editor ng-model="MODEL_COL_FIELD"></label></form></div>';
                                //item.editableCellTemplate = '<div><form name="inputForm" ui-grid-editor ng-model="MODEL_COL_FIELD">' +
                                //    '<div class="glyphicon glyphicon-question-sign" ng-click="MODEL_COL_FIELD=null;"></div>' +
                                //    '<div class="glyphicon glyphicon-unchecked"></div>' +
                                //    '<div class="glyphicon glyphicon-check"></div>' +
                                //    '</form></div>';
                            }
                            //item.cellTemplate = item.editableCellTemplate;
                            break;
                        case "number":
                            item.filter = (item.filter === undefined) ? { condition: uiGridCustomService.numberCondition } : item.filter;
                            break;
                        case undefined:
                            item.filter = (item.filter === undefined) ? { condition: uiGridCustomService.condition } : item.filter;
                            break;

                    }
                });
            };
            this.FiltersClear = function () {
                this.gridApi.grid.columns.forEach(function (col, col_i, col_arr) {
                    col.filters.forEach(function (filter, filter_i, filter_arr) {
                        filter.term = "";
                    });
                });
            };
            this.FilterSet = function (columnName, value) {
                this.gridApi.grid.columns.forEach(function (col, col_i, col_arr) {
                    if (col.field === columnName) {
                        col.filters[0].term = value;
                    }
                });
            };
            this.Refresh = function () {
                this.gridApi.core.refresh();
            };


            this.SortColumn = function (columnName, isASC, isAdd) {
                this.gridApi.grid.columns.forEach(function (col, col_i, col_arr) {
                    if (col.field === columnName) {
                        $scope.gridApi.grid.sortColumn(col, isASC === true ? uiGridConstants.ASC: uiGridConstants.DESC, isAdd);
                        return;
                    }
                });
               
            };


            function GridUndo() {
                if (ArrayUndo.length > 0) {
                    var row = ArrayUndo.pop();
                    this.grid.options.data.forEach(function (item, i, arr) {
                        if (item.$$hashKey === row.$$hashKey) {
                            arr[i] = row;
                            //item = row;
                            //return false;
                        }
                    });
                    if (ArrayUndo.lenght === 0)
                        this.NeedSave = false;
                }
            };

            hotkeys.bindTo($scope).add({
                combo: 'shift+a',
                description: 'Выбрать все строки',
                callback: function (event) {
                    alert('A');
                }
            });
        }

        return {
            createOptions: function (gridName) {
                var options =
                    {
                        enableGridMenu: true,
                        enableColumnResizing: true,
                        enableFiltering: true,
                        enableRowHeaderSelection: false,
                        enableCellEdit: false,
                        enableRowSelection: false,
                        showGridFooter: true,
                        exporterFieldApplyFilters: false,
                        exporterHeaderFilter: $translate.instant,
                        gridMenuTitleFilter: $translate.instant
                    };

                postInit(options, gridName);

                return options;
            },
            createGridClass: function (scope, gridName) {
                var gridClass = new CommonGridClass(scope);

                postInit(gridClass.Options, gridName);
                
                return gridClass;
            },
            createGridClassMod: function (scope, gridName, onSelectionChanged,ondblclick) {
                var gridClass = new CommonGridClassMod(scope, ondblclick);
                gridClass.parent = this;
                gridClass.onSelectionChanged = onSelectionChanged;
                gridClass.Options.GridClassMod = gridClass;
              //  I = gridClass;
                postInit(gridClass.Options, gridName);
                gridClass.Options.onRegisterApi = function (api) {

                    //set gridApi on scope
                    this.GridClassMod.gridApi = api;
                    api.GridClassMod = this.GridClassMod;
                    //Что-то выделили
                    
                    api.selection.on.rowSelectionChanged(this.GridClassMod.Options.appScopeProvider, function (row) {
                        this.GridClassMod.selectedItem = this.selection.getSelectedRows();
                        //this.GridClassMod.selected = this.selection.getSelectedRows().map(function (value) {
                        //    return value.Id;
                        //});
                        if (this.GridClassMod.onSelectionChanged !== null && this.GridClassMod.onSelectionChanged !== undefined)
                            this.GridClassMod.onSelectionChanged(row.entity);

                    });

                    api.edit.on.afterCellEdit(this.GridClassMod.Options.appScopeProvider, function (rowEntity, colDef, newValue, oldValue) {
                        if (colDef.field !== '@modify') {
                            if (newValue !== oldValue) {
                                var deepClone = JSON.parse(JSON.stringify(rowEntity));
                                deepClone[colDef.field] = oldValue;
                                this.GridClassMod.GridLogger(deepClone);
                                rowEntity["@modify"] = true;
                                this.GridClassMod.NeedSave = true;
                                if (this.GridClassMod.afterCellEdit !== null) {
                                    this.GridClassMod.afterCellEdit(rowEntity, colDef, newValue, oldValue);
                                }
                            }
                        }
                    });
                    //Что-то выделили
                    api.selection.on.rowSelectionChangedBatch(this.GridClassMod.Options.appScopeProvider, function () {

                        this.GridClassMod.selectedItem = this.selection.getSelectedRows();
                        if (this.GridClassMod.onSelectionChanged !== null && this.GridClassMod.onSelectionChanged !== undefined)
                            this.GridClassMod.onSelectionChanged();
                    });

                    //Очищает выделенное когда изменяется фильтр
                    api.core.on.filterChanged(this.GridClassMod.Options.appScopeProvider, function () {
                        this.GridClassMod.selectedItem = null;
                        this.GridClassMod.gridApi.selection.clearSelectedRows();
                    });
                };
                return gridClass;
            },
            getBooleanCellTemplate: function() {
                return '<div class="text-center">{{COL_FIELD ? "'+yes+'" : "'+no+'"}}</div>';
            },
            booleanConditionX: function (searchTerm, cellValue) {
                switch (searchTerm) {
                    case '1':
                        searchTerm = true;
                        break;
                    case '0':
                        searchTerm = false;
                        break;
                    case "\\-":
                        searchTerm = null;
                        break;
                }
                if (cellValue === searchTerm) {
                    return true;
                }
                return false;
            },
            SPRCondition: function (searchTerm, cellValue) {

                searchTerm = searchTerm.toString().toLowerCase().replace(/\\/g, '');

                //if ((cellValue === null || cellValue === undefined) && searchTerm ==="\-")
                //   return true;

                if (cellValue === null || cellValue === undefined)
                    return false;

                cellValue = cellValue.toString().toLowerCase().replace(/\\/g, '');

                var terms = searchTerm.split('*');

                for (var j = 0; j < terms.length; ++j) {
                    if (terms[j][0] !== '!') {
                        if (cellValue.indexOf(terms[j]) < 0)
                            return false;
                    } else {
                        if (cellValue.indexOf(terms[j].substring(1)) >= 0)
                            return false;
                    }
                }

                return true;
            },
            booleanCondition: function (searchTerm, cellValue) {                
                if (searchTerm === "1" && cellValue === true)
                    return true;
                if (searchTerm === "0" && cellValue === false)
                    return true;
                if ((cellValue === '' || cellValue === null || cellValue === undefined) && searchTerm === "\\-")
                    return true;

                searchTerm = searchTerm.toString().toLowerCase();

                if (cellValue === null || cellValue === undefined)
                    return false;

                cellValue = cellValue ? yes : no;

                cellValue = cellValue.toLowerCase();

                if (cellValue.indexOf(searchTerm) < 0) {
                    return false;
                }
                
                return true;
            },
            condition: function (searchTerm, cellValue) {

                searchTerm = searchTerm.toString().toLowerCase().replace(/\\/g, '');
                //до2020-02-18 было заблокировано почему?
                //дик*амп/гель
                if (searchTerm === "-") {
                    if (cellValue === '' || cellValue === null || cellValue === undefined)//выбрать пустые
                        return true;
                    else
                        return false;
                }
                if (searchTerm === "!") {
                    if (cellValue !== '' && cellValue !== null && cellValue !== undefined)//выбрать не пустые
                        return true;
                    else
                        return false;
                }
                if (cellValue === null || cellValue === undefined)
                    return false;

                cellValue = cellValue.toString().toLowerCase().replace(/\\/g, '');

                var terms = searchTerm.split('*');

                for (var j = 0; j < terms.length; ++j) {
                    if (terms[j].indexOf('_') >= 0) {
                        var termsOR = terms[j].split('_');
                        var ret_OR = false;
                        for (var j_or = 0; j_or < termsOR.length; ++j_or) {                            
                            if (cellValue.indexOf(termsOR[j_or]) >= 0)
                                ret_OR = true;
                        }
                        if (ret_OR === false)
                            return false;
                    }
                    else {
                        if (terms[j][0] !== '!') {
                            if (cellValue.indexOf(terms[j]) < 0)
                                return false;
                        } else {
                            if (cellValue.indexOf(terms[j].substring(1)) >= 0)
                                return false;
                        }
                    }
                }

                return true;
            },
            conditionSpace: function (searchTerm, cellValue) {

                searchTerm = searchTerm.toString().toLowerCase().replace(/\\/g, '');
                if ((cellValue === '' || cellValue === null || cellValue === undefined) && searchTerm === "-")
                    return true;

                if (cellValue === null || cellValue === undefined)
                    return false;

                cellValue = cellValue.toString().toLowerCase().replace(/\\/g, '');

                var terms = searchTerm.split('*');

                for (var j = 0; j < terms.length; ++j) {
                    if (terms[j].indexOf(' ') >= 0) {
                        var termsOR = terms[j].split(' ');
                        var ret_OR = false;
                        for (var j_or = 0; j_or < termsOR.length; ++j_or) {
                            if (cellValue.indexOf(termsOR[j_or]) >= 0)
                                ret_OR = true;
                        }
                        if (ret_OR === false)
                            return false;
                    }
                    else {
                        if (terms[j][0] !== '!') {
                            if (cellValue.indexOf(terms[j]) < 0)
                                return false;
                        } else {
                            if (cellValue.indexOf(terms[j].substring(1)) >= 0)
                                return false;
                        }
                    }
                }

                return true;
            },
            conditionList: function (searchTerm, cellValueId, Row,Column) {

                var cellValue = "";
                Column.colDef.editDropdownOptionsArray.forEach(function (item, i, arr) {
                    if (cellValueId === item[Column.colDef.editDropdownIdLabel]) {
                        cellValue = item[Column.colDef.editDropdownValueLabel];
                        return false;
                    }
                });

                searchTerm = searchTerm.toString().toLowerCase().replace(/\\/g, '');

                if ((cellValue === '' || cellValue === null || cellValue === undefined) && searchTerm === "-")
                    return true;

                if (cellValue === null || cellValue === undefined)
                    return false;

                cellValue = cellValue.toString().toLowerCase().replace(/\\/g, '');

                var terms = searchTerm.split('*');

                for (var j = 0; j < terms.length; ++j) {
                    if (terms[j].indexOf('_') >= 0) {
                        var termsOR = terms[j].split('_');
                        var ret_OR = false;
                        for (var j_or = 0; j_or < termsOR.length; ++j_or) {
                            if (cellValue.indexOf(termsOR[j_or]) >= 0)
                                ret_OR = true;
                        }
                        if (ret_OR === false)
                            return false;
                    }
                    else {
                        if (terms[j][0] !== '!') {
                            if (cellValue.indexOf(terms[j]) < 0)
                                return false;
                        } else {
                            if (cellValue.indexOf(terms[j].substring(1)) >= 0)
                                return false;
                        }
                    }
                }

                return true;
            },
            numberCondition: function (searchTerm, cellValue) {
                searchTerm = searchTerm.toString().toLowerCase().replace(/\\/g, '');


                if (cellValue === null || cellValue === undefined)
                    if (searchTerm === "-")
                        return true;
                    else
                        return false;
               

                var terms = searchTerm.split(' ');//Можно значения перечислять через пробел

                for (var j = 0; j < terms.length; ++j) {
                    searchTerm = terms[j];
                    var FS = searchTerm[0];//Ищем первый символ он может быть коммандным
                    switch (FS) {
                        case "!"://Условие НЕ
                            FS_a = parseFloat(searchTerm.substring(1));
                            if (FS_a !== isNaN && FS_a !== cellValue)
                                return true;
                            break;
                        case ">"://Условие больше
                            FS_a = parseFloat(searchTerm.substring(1));
                            if (FS_a !== isNaN && FS_a < cellValue)
                                return true;
                            break;
                        case "<"://Условие Меньше
                            FS_a = parseFloat(searchTerm.substring(1));
                            if (FS_a !== isNaN && FS_a > cellValue)
                                return true;
                            break;
                        case "~"://Условие между
                            searchTerm = searchTerm.substring(1);
                            var ind1 = searchTerm.indexOf("~");
                            if (ind1 > 0) {
                                var s1 = searchTerm.substring(0, ind1);
                                var s2 = searchTerm.substring(ind1+1);
                                FS_a1 = parseFloat(s1);
                                FS_a2 = parseFloat(s2);
                                if (FS_a1 !== isNaN && FS_a2 !== isNaN) {
                                    if (cellValue >= FS_a1 && cellValue <= FS_a2)
                                        return true;
                                }
                            }
                            break;
                        default://условие полного равенства
                            if (parseFloat(searchTerm) === cellValue)
                                return true;
                            break;
                    }
                }               

                return false;
        }
        };
    }]);