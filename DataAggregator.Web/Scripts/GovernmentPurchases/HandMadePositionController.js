angular
    .module('DataAggregatorModule')
    .controller('HandMadePositionController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', HandMadePositionController])
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
                /* else if (initial) {
                     return initial;
                 }*/
                return input;

            } catch (e) {
                // context.grid.appScope.log("Error: " + e);
            }
        };
    });

function HandMadePositionController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "Вакцинация";
    $scope.user = userService.getUser();

    $scope.RegionList = [];
    $scope.ProductList = [];
    $scope.selectedDataSource = null;
    var today = new Date();

    $scope.HandMadePosition_Init = function () {
        $scope.ColList = [];
        $scope.UsersAll = [];
        $scope.ColumnList = [];
        $scope.Period = new Date(today.getFullYear(), today.getMonth()-1, 1);   
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/HandMadePosition/HandMadePosition_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            //   Array.prototype.push.apply($scope.UsersAll, response.data.Data.UsersAll);
            //  $scope.filterList = data;
            //Array.prototype.push.apply($scope.RegionList, response.data.Data.RegionList);
            Array.prototype.push.apply($scope.RegionList, response.data.Data.RegionList.map(function (obj) {
                var rObj = { 'value': obj.Region_Id, 'label': obj.FederationSubject };
                return rObj;
            }));
            Array.prototype.push.apply($scope.ProductList, response.data.Data.ProductList);
            //  Array.prototype.push.apply($scope.ProductList, response.data.Data.ProductList);

            //   $scope.HandMadePosition_search();
            return null;
        });



        $scope.dataLoading = $http({
            method: 'POST',
            url: '/HandMadePosition/getColumnGrid/',
            data: JSON.stringify({})
        }).then(function (response) {           
            Array.prototype.push.apply($scope.ColumnList, response.data);     
            Array.prototype.push.apply($scope.ColList, response.data.map(function (obj) {
                var rObj = {};
                if (obj.ColName == 'Period') {
                    rObj = {
                        headerTooltip: true, cellTooltip: true, field: obj.ColName, name: obj.ColText, type: 'date', cellFilter: 'date:\'yyyy-MM-dd\''
                        , width: 100, enableCellEdit: false
                       // , filter: { condition: uiGridCustomService.condition }

                    };
                } else if (obj.ColName == 'Region_id') {
                    rObj = {
                        headerTooltip: true, cellTooltip: true,
                        enableCellEdit: false, width: 150, name: obj.ColText,
                        field: obj.ColName,
                        cellFilter: 'griddropdownSSA:this', editType: 'dropdown',

                        filter:
                        {
                            type: uiGridConstants.filter.SELECT,
                            selectOptions: $scope.RegionList
                        },
                        editableCellTemplate: 'ui-grid/dropdownEditor',
                        editDropdownOptionsArray: $scope.RegionList,
                        // editType: 'dropdown', 
                        editDropdownIdLabel: 'value', editDropdownValueLabel: 'label' //, editDropdownFilter: 'translate'


                    };
                } else
                    if (obj.IsEditable == 1) {
                        if (obj.ColName == 'InputFirst' || obj.ColName == 'InputSecond' || obj.ColName == 'InputRevaccinated' || obj.ColName == 'InputСhildren') {
                            rObj = { headerTooltip: true, cellTooltip: true, field: obj.ColName, name: obj.ColText, width: 150, enableCellEdit: obj.IsEditable, headerCellClass: 'Cellcolored'};
                        } else
                            rObj = { headerTooltip: true, cellTooltip: true, field: obj.ColName, name: obj.ColText, width: 150, enableCellEdit: obj.IsEditable, headerCellClass: 'Cellcolored2' };

                    } else rObj = { headerTooltip: true, cellTooltip: true, field: obj.ColName, name: obj.ColText, width: 150, enableCellEdit: obj.IsEditable };

                if (obj.IsNumForm == 1) {
                    rObj.type = 'number';
                    rObj.filter = { condition: uiGridCustomService.numberCondition };
                   rObj.cellFilter = formatConstants.FILTER_PRICE;
                }
                if (obj.IsTotal == 1) {
                    rObj.aggregationType = uiGridConstants.aggregationTypes.sum;
                    //  footerCellTemplate: '<div class="ui-grid-cell-contents" >Total: {{col.getAggregationValue() | number:2 }}</div>'
                    rObj.footerCellFilter = 'number:2';
                    //, aggregationType: uiGridConstants.aggregationTypes.sum
                    // rObj.apply( 'aggregationType: uiGridConstants.aggregationTypes.sum ');
                    //  rObj += ;
                    //  alert(rObj);
                }
            
                


                return rObj;
            }));
            return null;
        });
 
       
        
       
        
        $scope.filters = { IsNotReady: true };
        $scope.filtersClass = { isLS: true };
        $scope.Search_text = ""; 
       
        $scope.Grid_Raw = uiGridCustomService.createGridClassMod($scope, "Grid_Raw");
        $scope.Grid_Raw.Options.showGridFooter = true;

        $scope.Grid_Raw.Options.showColumnFooter = true;
        $scope.Grid_Raw.Options.enableFiltering = true;
        $scope.Grid_Raw.Options.columnDefs = $scope.ColList;

        $scope.HandMadePosition_search = function () {
            if ($scope.Grid_Raw.NeedSave === true) {
                messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                    .then(
                        function (result) {
                            $scope.HandMadePosition_save_q("search");
                            alert("Сохранено");
                        },
                        function (result) {
                            if (result === 'no') {
                                $scope.HandMadePosition_search_tbl();
                            }
                            else {
                                var d = "отмена";
                            }

                        });
            }
            else {
                $scope.HandMadePosition_search_tbl();
            }

        };

        $scope.HandMadePosition_search_tbl = function () {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/HandMadePosition/HandMadePosition_search/',
                    data: JSON.stringify({ Region_Id: 0 })
                }).then(function (response) {             
                  
                        $scope.Grid_Raw.Options.data = response.data;
                    
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        };

       
        $scope.clickMe = function () {
            
            $scope.columns.push({ field: 'company' });
        }
        $scope.addPeriod = function (action) {
          
          $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/HandMadePosition/addPeriod/',
                    data: JSON.stringify({
                        date: $scope.Period.toISOString()
                    })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.HandMadePosition_search_tbl();
                        }
                        else {
                            $scope.Grid_Raw.ClearModify();
                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
            
        };
        $scope.HandMadePosition_save = function (action) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/HandMadePosition/HandMadePosition_save/',
                    data: JSON.stringify({
                        array_Raw: $scope.Grid_Raw.GetArrayModify()
                    })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.HandMadePosition_search_tbl();
                        }
                        else {
                            $scope.Grid_Raw.ClearModify();
                            alert("Сохранил");
                            $scope.HandMadePosition_search_tbl();
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        };

        $scope.HandMadePosition_remove = function () {
            var items = $scope.Grid_Raw.selectedRows();
            var rowdelete = items.map(function (value) {
                return value.Id;
            });
          //  alert(rowdelete);
            if (window.confirm("Вы Действительно хотите удалить?")) {
                var json = JSON.stringify({ ids: rowdelete });

                $scope.sourceLoading = $http({
                    method: "POST",
                    url: "/HandMadePosition/HandMadePosition_delete/",
                    data: json
                }).then(function (response) {
                    /* var index = $scope.Grid_Raw_Price.data.indexOf(rowedit[0]["Id"]);
                     $scope.Grid_Raw_Price.data.splice(index, 1);
                     $scope.selectedRawPrice = null;*/
                    alert("Удалил");
                    $scope.HandMadePosition_search_tbl();
                }, function () {
                    $scope.message = "Unexpected Error";
                });


            }



        };

///Period

        $('#PeriodModal').on('show.bs.modal', function (event) {
          //  getPriceGrid();

        })


////Period End


        ///////Price


        selectedRawPrice = null;
        $scope.Grid_Raw_Price = uiGridCustomService.createGridClassMod($scope, 'Grid_Raw_Price');
        $scope.Grid_Raw_Price.Options.appScopeProvider = $scope;
        $scope.Grid_Raw_Price.Options.showGridFooter = false;
        $scope.Grid_Raw_Price.Options.enableRowSelection = true;
        $scope.Grid_Raw_Price.Options.customEnableRowSelection = true;
        $scope.Grid_Raw_Price.Options.enableSelectionBatchEvent = true;
        $scope.Grid_Raw_Price.Options.enableHighlighting = true;
        $scope.Grid_Raw_Price.Options.noUnselect = false;
        $scope.Grid_Raw_Price.Options.modifierKeysToMultiSelect = true;
      /*  // $scope.Grid_Raw_Price.NeedSave = false;
        $scope.Grid_Raw_Price.filters = { IsNotReady: true };
        $scope.Grid_Raw_Price.filtersClass = { isLS: true };*/
      //  $scope.Grid_Raw.Options.showGridFooter = true;

       // $scope.Grid_Raw.Options.showColumnFooter = true;
        $scope.Grid_Raw.Options.enableFiltering = true;

        // $scope.Grid_Raw_Price.Options.modifierKeysToMultiSelect = true;
        //  $scope.Grid_Raw_Price.Options.rowTemplate = '<div ng-class="{\'modify\' : row.entity[\'@modify\']==true}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';


        // $scope.Grid_Raw_PriceOptions = uiGridCustomService.createOptions('Grid_Raw_PriceOptions');
        $scope.Grid_Raw_Price.Options.columnDefs = [
            { name: 'Id', field: 'Id', width: 70, type: 'number', enableCellEdit: false },
            { name: 'Classifer_Id', field: 'Classifer_Id', width: 70, type: 'number' },
            {
                enableCellEdit: true, width: 150, name: 'Продукт',
                field: 'Classifer_Id',
                cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.ProductList,
                // editType: 'dropdown', 
                editDropdownIdLabel: 'Classifer_Id', editDropdownValueLabel: 'DrugShort' //, editDropdownFilter: 'translate'
            },


            { name: 'Дата начала', field: 'DateBegin', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE, width: 180, enableCellEdit: true },
            {
                name: 'Дата конца', field: 'DateEnd', type: 'date', cellFilter: formatConstants.FILTER_DATE, width: 180, enableCellEdit: true
            },
            { name: 'Цена', field: 'Price', width: 150, type: 'number', enableCellEdit: true },
            { name: 'Пользователь', field: 'UserLastUpdate', width: 180, enableCellEdit: false },
            { name: 'Дата Изменения', field: 'UserLastUpdateDate', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, width: 180, enableCellEdit: false }
        ];


        //  angular.extend(Grid_Raw_Price.Options, Grid_Raw_Price.Options);
     /*  $scope.Grid_Raw_Price.Options.onRegisterApi = function (gridApi) {

            gridApi.selection.on.rowSelectionChanged($scope, selectRawPrice);
      

        };
        */
        /*
             gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                    if (colDef.field !== '@modify') {
                        if (newValue !== oldValue) {
                            rowEntity["@modify"] = true;
                            $scope.Grid_Raw_Price.NeedSave = true;
                        }
                    }
                });
         */

        function selectRawPrice(row) {

            if (row.isSelected) {
                $scope.selectedRawPrice = row.entity;

            } else $scope.selectedRawPrice = null;
        }



        $('#PriceModal').on('show.bs.modal', function (event) {
            getPriceGrid();

        })
        function getPriceGrid() {
            $scope.Grid_Raw_Price.data = [];
            // str = JSON.stringify($scope.filter, null, 4); // (Optional) beautiful indented output.
            //  console.log(str);

            $scope.dataLoading = $http({
                method: "POST",
                url: "/HandMadePosition/getPriceGrid/",
                data: JSON.stringify({})
            }).then(function (response) {
                $scope.Grid_Raw_Price.Options.data =// response.data;
                    response.data.map(function (obj) {
                        // if (obj.ColName == 'DateEnd') {
                        obj.DateEnd = new Date(obj.DateEnd);
                        obj.DateBegin = new Date(obj.DateBegin);
                        //  }
                        return obj;
                    });
                // str = JSON.stringify(response.data, null, 4); // (Optional) beautiful indented output.
                //  console.log(str);

            }, function () {
                $scope.Grid_Raw_Price.Options.data = null;
            });
        };
        $scope.RawPrice_save = function (action) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/HandMadePosition/RawPrice_save/',
                    data: JSON.stringify({
                        array_Raw: $scope.Grid_Raw_Price.GetArrayModify()
                    })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.getPriceGrid();
                        }
                        else {
                            $scope.Grid_Raw_Price.ClearModify();
                            alert("Сохранил");
                            getPriceGrid();
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        };



        $scope.addRawPrice = function () {
            $scope.sourceLoading = $http({
                method: "POST",
                url: "/HandMadePosition/addRawPrice/"
            }).then(function (response) {
                $scope.Grid_Raw_Price.data.push(response.data);
                getPriceGrid();
            }, function () {
                $scope.message = "Unexpected Error";
            });
        };
        $scope.removeRawPrice = function () {
            var rowedit = $scope.Grid_Raw_Price.selectedRows();
            if (rowedit.length != 1) {
                alert("выделена не 1 строка классификатора на обновление");
                return;
            }
            if (window.confirm("Вы Действительно хотите удалить?")) {
                        var json = JSON.stringify({ id: rowedit[0]["Id"]});
                      
                        $scope.sourceLoading = $http({
                            method: "POST",
                            url: "/HandMadePosition/RemoveRawPrice/",
                            data: json
                        }).then(function (response) {
                           /* var index = $scope.Grid_Raw_Price.data.indexOf(rowedit[0]["Id"]);
                            $scope.Grid_Raw_Price.data.splice(index, 1);
                            $scope.selectedRawPrice = null;*/
                            alert("Удалил");
                            getPriceGrid();
                        }, function () {
                            $scope.message = "Unexpected Error";
                        });

                  
             }

               

        };
///////Price End





  
    





///////Product
    selectedRawProduct = null;
        $scope.Grid_Raw_Product = uiGridCustomService.createGridClassMod($scope, 'Grid_Raw_Product');
        $scope.Grid_Raw_Product.Options.appScopeProvider = $scope;
        $scope.Grid_Raw_Product.Options.showGridFooter = false;
        $scope.Grid_Raw_Product.Options.enableRowSelection = true;
        $scope.Grid_Raw_Product.Options.customEnableRowSelection = true;
        $scope.Grid_Raw_Product.Options.enableSelectionBatchEvent = true;
        $scope.Grid_Raw_Product.Options.enableHighlighting = true;
        $scope.Grid_Raw_Product.Options.noUnselect = false;
        $scope.Grid_Raw_Product.Options.modifierKeysToMultiSelect = true;
    $scope.Grid_Raw_Product.Options.enableFiltering = true;
    $scope.Grid_Raw_Product.Options.columnDefs = [
        { name: 'Classifer_Id', field: 'Classifer_Id', width: 70, type: 'number' ,enableCellEdit: true},
        { name: 'Препарат(сокр.)', field: 'DrugShort', enableCellEdit: true },
        { name: 'Препарат', field: 'Drug', enableCellEdit: true},       
        { name: 'Упаковки', field: 'Package', width: 70, type: 'number', enableCellEdit: true}      
    ];

  /* $scope.Grid_Raw_Product.Options.onRegisterApi = function (gridApi) {

        gridApi.selection.on.rowSelectionChanged($scope, selectRawProduct);

    };*/
    function selectRawProduct(row) {
        if (row.isSelected) {
            $scope.selectedRawProduct = row.entity;
        } else $scope.selectedRawProduct = null;
    }



    $('#ProductModal').on('show.bs.modal', function (event) {
        getProductGrid();
    })

    function getProductGrid() {
        $scope.Grid_Raw_Product.data = [];
       // str = JSON.stringify($scope.filter, null, 4); // (Optional) beautiful indented output.
      //  console.log(str);

        $scope.dataLoading = $http({
            method: "POST",
            url: "/HandMadePosition/getProductGrid/",
            data: JSON.stringify({ })
        }).then(function (response) {
            $scope.Grid_Raw_Product.Options.data = response.data;
           // str = JSON.stringify(response.data, null, 4); // (Optional) beautiful indented output.
          //  console.log(str);

        }, function () {
                $scope.Grid_Raw_Product.Options.data = null;
        });
    };


    $scope.RawProduct_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/HandMadePosition/RawProduct_save/',
                data: JSON.stringify({
                    array_Raw: $scope.Grid_Raw_Product.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        getProductGrid();
                    }
                    else {
                        $scope.Grid_Raw_Product.ClearModify();
                        alert("Сохранил");
                        getProductGrid();
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };




    $scope.addRawProduct = function () {
        $scope.sourceLoading = $http({
            method: "POST",
            url: "/HandMadePosition/addRawProduct/"
        }).then(function (response) {
          //  $scope.Grid_Raw_Product.data.push(response.data);
            getProductGrid();
        }, function () {
            $scope.message = "Unexpected Error";
        });
    };


    $scope.removeRawProduct = function () {
        var rowedit = $scope.Grid_Raw_Product.selectedRows();
        if (rowedit.length != 1) {
            alert("выделена не 1 строка классификатора на обновление");
            return;
        }
        if (window.confirm("Вы Действительно хотите удалить?")) {
            var json = JSON.stringify({ id: rowedit[0]["Id"] });

            $scope.sourceLoading = $http({
                method: "POST",
                url: "/HandMadePosition/RemoveRawProduct/",
                data: json
            }).then(function (response) {
                /* var index = $scope.Grid_Raw_Price.data.indexOf(rowedit[0]["Id"]);
                 $scope.Grid_Raw_Price.data.splice(index, 1);
                 $scope.selectedRawPrice = null;*/
                alert("Удалил");
                getProductGrid();
            }, function () {
                $scope.message = "Unexpected Error";
            });


        }



    };
        ///////Product End


    };


  /*  messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
        .then(
            function (result) {
                $scope.HandMadePosition_save_q("search");
                alert("Сохранено");
            },
            function (result) {
                if (result === 'no') {
                    $scope.HandMadePosition_search_tbl();
                }
                else {
                    var d = "отмена";
                }

            });

*/













    /*
    $scope.HandMadePosition_Init = function () {
        $scope.UsersAll = [];
        $scope.filters = { IsNotReady: true };
        $scope.filtersClass = { isLS: true };
        $scope.Search_text = "";
        $scope.Grid_Raw = uiGridCustomService.createGridClassMod($scope, "Grid_Raw");
        $scope.Grid_SPR = uiGridCustomService.createGridClassMod($scope, "Grid_SPR", null, "Classifier_dblClick");

        hotkeys.bindTo($scope).add({
            combo: 'shift+f',
            description: 'Поиск по справочнику',
            callback: function (event) {
                var value = commonService.getSelectionText();
                $scope.Organization_search_AC(value);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+x',
            description: 'Мусор',
            callback: function (event) {
                $scope.HandMadePosition_IsTrashSet(true);
            }
        });

        $scope.Grid_Raw.Options.columnDefs = [
            { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'Value', field: 'Value', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'IsTrash', field: 'IsTrash', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, width: 100, name: 'User', field: 'UserId', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                editDropdownOptionsArray: $scope.UsersAll,
                editDropdownIdLabel: 'UserId', editDropdownValueLabel: 'FullName'
            },
            { cellTooltip: true, name: 'DateUpdate', field: 'DateUpdate', filter: { condition: uiGridCustomService.condition }, type: 'date' },
            { cellTooltip: true, name: 'OrganizationId', field: 'OrganizationId', filter: { condition: uiGridCustomService.condition }, type: 'number' },
            { cellTooltip: true, name: 'Название', field: 'Organization.ShortName', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'ИНН', field: 'Organization.INN', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'Адрес', field: 'Organization.LocationAddress', filter: { condition: uiGridCustomService.condition } }
        ];


        $scope.Grid_SPR.Options.columnDefs = [
            { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            //{ name: 'ActualId', field: 'ActualId', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'К.Название', field: 'ShortName', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'П.Название', field: 'FullName', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'ИНН', field: 'INN', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'Адрес', field: 'LocationAddress', filter: { condition: uiGridCustomService.condition } }
        ];


        $scope.dataLoading = $http({
            method: 'POST',
            url: '/HandMadePosition/HandMadePosition_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.UsersAll, response.data.Data.UsersAll);
            $scope.HandMadePosition_search();
            return response.data;
        });
    };

    $scope.Classifier_dblClick = function (field, rowEntity) {
        $scope.Grid_Raw.selectedRows().forEach(function (item) {
            var orgId = rowEntity["Id"];
            if (rowEntity["ActualId"] > 0)
                orgId = rowEntity["ActualId"];
            $scope.Grid_Raw.GridCellsMod(item, "IsTrash", false);
            $scope.Grid_Raw.GridCellsMod(item, "OrganizationId", orgId);
            $scope.Grid_Raw.GridCellsMod(item, "UserId", $scope.user.UserId);
            $scope.Grid_Raw.GridCellsMod(item, "Organization", { ShortName: rowEntity["ShortName"], INN: rowEntity["INN"], LocationAddress: rowEntity["LocationAddress"] });
        });
    };
    $scope.HandMadePosition_IsTrashSet = function (value) {
        var selectedRows = $scope.Grid_Raw.selectedRows();
        selectedRows.forEach(function (item) {
            $scope.Grid_Raw.GridCellsMod(item, "IsTrash", value);
            $scope.Grid_Raw.GridCellsMod(item, "OrganizationId", null);
            $scope.Grid_Raw.GridCellsMod(item, "UserId", $scope.user.UserId);
            $scope.Grid_Raw.GridCellsMod(item, "Organization", { ShortName: '', INN: '', LocationAddress: '' });
        });
    };
    $scope.HandMadePosition_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/HandMadePosition/HandMadePosition_search/',
                data: JSON.stringify({ IsNotReady: $scope.filters.IsNotReady })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_Raw.Options.data = data.Data.Raw;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.HandMadePosition_search = function () {
        if ($scope.Grid_Raw.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.HandMadePosition_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.HandMadePosition_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.HandMadePosition_search_AC();
        }

    };
    $scope.HandMadePosition_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/HandMadePosition/HandMadePosition_save/',
                data: JSON.stringify({
                    array_Raw: $scope.Grid_Raw.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.HandMadePosition_search_AC();
                    }
                    else {
                        $scope.Grid_Raw.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Organization_search_AC = function (Value) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OrganizationsEditor/GetOrganizations',
                data: JSON.stringify({
                    filter: {
                        Id: null,
                        Inn: null,
                        OrganizationType: null,
                        FullName: null,
                        Text: Value,
                        ShortName: null,
                        OnlyDrugsLinked: $scope.filtersClass.isLS,
                        OnlyEmptyType: false,
                        OnlyEmptyRegion: false,
                        is_LO: false,
                        is_CP: false,
                        is_Actual: false
                    }
                })
            }).then(function (response) {
                var data = response.data;
                $scope.Grid_SPR.Options.data = data;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });


    };

 <div class="col-md-2 padding1">
                    <label for="Region">Субъект федерации</label>
                    <select name="Region"
                            id="Region"
                            class="form-control btn-sm"
                            ng-options="item as item.label for item in RegionList track by item.label"
                            required
                            ng-model="RegionId"></select>

                </div>
    */
}