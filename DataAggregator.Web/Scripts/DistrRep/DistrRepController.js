angular
    .module('DataAggregatorModule')
    .controller('DistrRepController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', DistrRepController])

    .constant('DataSource', {
        type: 'object',
        properties: {
            Name: { type: 'string', title: 'Источник данных' },
            NameFull: { type: 'string', title: 'Название(Рус)' },
            NameEng: { type: 'string', title: 'Название(Анг)' },
            DataSourceTypeId: {
                title: 'Тип источника данных',
                type: 'string',
                items: {
                    type: "string"
                }
            }
        }
    }).filter('griddropdownSSS', function () {
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
function DistrRepController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.user = userService.getUser();

    $scope.selectedDataSource = null;
    $scope.selectedTemplate = null;
    $scope.templateFieldName = null;

    $scope.DataSourceType = [];
    $scope.DataSourceTypeLabel = [];
    var DataSourceType2 = [];
    $scope.Project = [];
    $scope.ProjectLabel = [];
    $scope.DataType = [];
    $scope.DataTypeLabel = [];
    $scope.YN = [{ "Id": 0, "Name": "N" }, { "Id": 1, "Name": "Y" }];
    $scope.YNLabel = [{ 'value': 'False', 'label': "N" }, { 'value': 'True', 'label': "Y" }];

    $scope.dataLoading = $http({
        method: "POST",
        url: "/DistrRep/GetAllProject/",
        data: JSON.stringify({ param: "DistrRep_EditSource_Init" })
    }).then(function (response) {
        Array.prototype.push.apply($scope.Project, response.data);
        Array.prototype.push.apply($scope.ProjectLabel, response.data.map(function (obj) {
            var rObj = { 'value': obj.Id, 'label': obj.Name };
            return rObj;
        }));
        //  $scope.DataSourceType = response.data;

    }, function () {
        $scope.message = "Unexpected Error";
    });

    $scope.dataLoading = $http({
        method: "POST",
        url: "/DistrRep/GetAllDataType/",
        data: JSON.stringify({ param: "DistrRep_EditSource_Init" })
    }).then(function (response) {
        Array.prototype.push.apply($scope.DataType, response.data);
        Array.prototype.push.apply($scope.DataTypeLabel, response.data.map(function (obj) {
            var rObj = { 'value': obj.Id, 'label': obj.Name };
            return rObj;
        }));
        //  $scope.DataSourceType = response.data;

    }, function () {
        $scope.message = "Unexpected Error";
    });


    $scope.dataLoading = $http({
        method: "POST",
        url: "/DistrRep/GetAllDataSourceType/",
        data: JSON.stringify({ param: "DistrRep_EditSource_Init" })
    }).then(function (response) {
        Array.prototype.push.apply($scope.DataSourceType, response.data);
        Array.prototype.push.apply($scope.DataSourceTypeLabel, response.data.map(function (obj) {
            var rObj = { 'value': obj.Id, 'label': obj.Name };
            return rObj;
        }));
        //  $scope.DataSourceType = response.data;

    }, function () {
        $scope.message = "Unexpected Error";
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
    function mapDataSourceType() {

        if (DataSourceType2.length < 1) {

            DataSourceType2 = [];
            $http({
                method: "POST",
                url: "/DistrRep/GetAllDataSourceType/",
                data: JSON.stringify({ param: "DistrRep_EditSource_Init2" })
            }).then(function (response) {
                Array.prototype.push.apply(DataSourceType2, response.data);
                //  $scope.DataSourceType = response.data;

            }, function () {
                $scope.message = "Unexpected Error";
            });
            str11 = JSON.stringify($scope.DataSourceType, null, 4);
            console.log(str11);
        }
        str11 = JSON.stringify(DataSourceType2, null, 4);
        console.log(str11);
        return DataSourceType2.map(function (obj) {
            var rObj = { 'value': obj.Id, 'label': obj.Name };
            return rObj;
        });
    }



    // $scope.DataSourceType2 = $scope.DataSourceType;
    // $scope.DataSourceType2 = renameKey($scope.DataSourceType2, 'Id', 'value');

    //  $scope.DataSourceType2 = renameKey($scope.DataSourceType2, 'Name', 'label');


    $scope.Relation = [];

    $http({
        method: "POST",
        url: "/DistrRep/GetRelation/",
        data: JSON.stringify({ param: "DistrRep_EditSource_Init" })
    }).then(function (response) {
        Array.prototype.push.apply($scope.Relation, response.data);
        //  $scope.DataSourceType = response.data;

    }, function () {
        $scope.message = "Unexpected Error";
    });

    $scope.TemplatesMethod = [];

    $http({
        method: "POST",
        url: "/DistrRep/GetTemplatesMethod/",
        data: JSON.stringify({ param: "DistrRep_EditSource_Init" })
    }).then(function (response) {
        Array.prototype.push.apply($scope.TemplatesMethod, response.data);
        //  $scope.DataSourceType = response.data;

    }, function () {
        $scope.message = "Unexpected Error";
    });

    $scope.TemplatesFieldName = [];

    $http({
        method: "POST",
        url: "/DistrRep/GetTemplatesFieldName/",
        data: JSON.stringify({ param: "DistrRep_EditSource_Init" })
    }).then(function (response) {
        Array.prototype.push.apply($scope.TemplatesFieldName, response.data);
        //  $scope.DataSourceType = response.data;

    }, function () {
        $scope.message = "Unexpected Error";
    });

    this.typeLookup = function (val, arr) {
        var result = arr.filter(function (v) {
            return v.id === val;
        })[0].Name;

        return result;
    };

    //Источник Данных
    $scope.gridDataSourceOptions = uiGridCustomService.createOptions('DistrRep_EditSource_DataSourcesGrid');
    var gridDataSourceOptions = {
        customEnableRowSelection: true,
        multiSelect: false,
        enableFullRowSelection: true,
        enableRowHeaderSelection: false,
        appScopeProvider: $scope,


        enableRowSelection: true,
        showGridFooter: false,
        noUnselect: true,
        columnDefs: [
            {
                headerTooltip: true, field: 'Id', name: 'ИД', width: 34, type: 'number', enableCellEdit: false
            },
            {
                enableCellEdit: true, width: 150, name: 'Проект',
                field: 'ProjectId',
                headerCellClass: 'Edit',
                cellFilter: 'griddropdownSSS:this',
                //  cellFilter:'mapDataSourceType:this',
                editType: 'dropdown',
                filter:
                {
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: $scope.ProjectLabel
                },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.Project,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name' //   
                //  , editDropdownFilter: 'translate'
            },

            {
                headerTooltip: true, name: 'Источник данных', field: 'Name', enableCellEdit: true
            }
            , {
                headerTooltip: true, name: 'Полное название', field: 'NameFull', enableCellEdit: true, visible: false
            },
            {
                enableCellEdit: true, width: 150, name: 'Тип источника',
                field: 'DataSourceTypeId',
                headerCellClass: 'Edit',
                cellFilter: 'griddropdownSSS:this',
                //  cellFilter:'mapDataSourceType:this',
                editType: 'dropdown',
                filter:
                {
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: $scope.DataSourceTypeLabel

                },
                //  cellTemplate : '<div class="ui-grid-cell-contents">{{ grid.appScope.Main.typeLookup(COL_FIELD,' + JSON.stringify($scope.DataSourceType) + ') }}</div>',
                //filter: {                 //type: uiGridConstants.filter.SELECT                     selectOptions: mapDataSourceType()         },

                // { type: uiGridConstants.filter.SELECT, condition: uiGridConstants.filter.EXACT },

                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.DataSourceType,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name' //   
                //  , editDropdownFilter: 'translate'
            }

            , { headerTooltip: true, name: 'Тип источника(описание)', field: 'DataSourceType.Name', enableCellEdit: false, visible: false }
            /*,
             {
                enableCellEdit: true, width: 150, name: 'Тип источника2',
                field: 'DataSourceTypeId',          
               //  cellFilter: 'griddropdownSSS:this',
                 editType: 'dropdown',
                 filter:
                 {
                     type: uiGridConstants.filter.SELECT,
                     selectOptions: $scope.DataSourceTypeLabel
                     
                 },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                 editDropdownOptionsArray: $scope.DataSourceTypeLabel,
                 editDropdownIdLabel: 'value', editDropdownValueLabel: 'label' //
                //  , editDropdownFilter: 'translate'



            }*/

        ]
        //, exporterHeaderFilter: function (displayName) { return 'col: ' + displayName; }
        /* , exporterFieldCallback : function (grid, row, col, value) {
             if (col.name === 'ИД') {
                 value = 0;
             }
             return value;
         }*/
    };



    angular.extend($scope.gridDataSourceOptions, gridDataSourceOptions);
    $scope.gridDataSourceOptions.onRegisterApi = function (gridApi) {
        gridApi.selection.on.rowSelectionChanged($scope, selectDataSource);
        //  gridApi.edit.on.beginCellEdit($scope, editRowDataSource)
        gridApi.edit.on.afterCellEdit($scope, editRowDataSource);


    };

    function selectDataSource(row) {
        if (row.isSelected) {
            $scope.selectedDataSource = row.entity;
            $scope.selectedTemplate = null;
            str = JSON.stringify($scope.DataSourceType, null, 4); // (Optional) beautiful indented output.
            //    console.log(str); // Logs output to dev tools console.
            //   alert(str);

            /* str123 = JSON.stringify($scope.DataSourceType.map(function (obj) {
                 var rObj = { "value": obj.Id, "label": obj.Name };
                 return rObj;
             }), null, 4);
             console.log(str123);
 
             str11 = JSON.stringify($scope.DataSourceType, null, 4);
             console.log(str11);
             */


            loadTemplates();
            loadTemplate();
        }
    }


    function loadDataSource() {
        $scope.gridDataSourceOptions.data = [];
        $scope.sourceLoading = $http({
            method: "POST",
            url: "/DistrRep/GetAllDataSource/",
            data: JSON.stringify({ param: "DistrRep_EditSource_Init" })
        }).then(function (response) {

            $scope.gridDataSourceOptions.data = response.data;

            //str11 = JSON.stringify(response.data, null, 4);
            // console.log(str11);
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }
    loadDataSource();


    $scope.DataSource = "";

    //Добавляем источник
    $scope.addDataSource = function () {


        $scope.sourceLoading = $http({
            method: "POST",
            url: "/DistrRep/AddDataSource/"
        }).then(function (response) {
            $scope.gridDataSourceOptions.data.push(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }
    function editRowDataSource(rowEntity, colDef, newValue, oldValue) {
        str = JSON.stringify(rowEntity, null, 4); // (Optional) beautiful indented output.
        console.log(str);
        var json = JSON.stringify(rowEntity);
        $scope.DataSourceLoading = $http({
            method: "POST",
            url: "/DistrRep/EditDataSource/",
            data: json
        }).then(function () {
            return true;
        }, function () {
            $scope.message = "Unexpected Error";
            rowEntity[colDef.field] = oldValue;
            $scope.$apply();
            return false;
        });

        //     $scope.DataSource = row.entity;
        //  alert(row.entity);
        //  $('#modal_DataSource_Header').val(row.entity);
        //      $('#modal_DataSource').modal('show');

    }

    //Удаляем источник
    $scope.removeDataSource = function () {

        if ($scope.selectedDataSource == null)
            return;

        var json = JSON.stringify({ id: $scope.selectedDataSource.Id });

        $scope.sourceLoading = $http({
            method: "POST",
            url: "/DistrRep/RemoveDataSource/",
            data: json
        }).then(function (response) {
            var index = $scope.gridDataSourceOptions.data.indexOf($scope.selectedDataSource);
            $scope.gridDataSourceOptions.data.splice(index, 1);
            $scope.selectedDataSource = null;
            $scope.selectedTemplate = null;
            loadTemplates();
            //   loadTemplate();
            alert(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });



    }

    //Конец Источник данных

    //Шаблоны

    //свойства грида
    $scope.gridTemplatesOptions = uiGridCustomService.createOptions('DistrRep_EditSource_TemplatesGrid');

    var gridTemplatesOptions = {
        customEnableRowSelection: true,
        multiSelect: false,
        enableFullRowSelection: true,
        enableRowHeaderSelection: false,
        appScopeProvider: $scope,
        enableRowSelection: true,
        showGridFooter: false,
        noUnselect: true,
        columnDefs: [
            {
                headerTooltip: true, field: 'Id', name: 'ИД', width: 34, type: 'number', enableCellEdit: false
            }
            , { name: 'Шаблоны', field: 'Name', enableCellEdit: true }
            // , { name: 'Тип Шаблона', field: 'TemplateMethod', enableCellEdit: true, visible: false }
            , {
                enableCellEdit: true, width: 150, name: 'Тип Шаблона',
                field: 'TemplatesMethodId',
                cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.TemplatesMethod,
                // editType: 'dropdown', 
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'NameRus' //, editDropdownFilter: 'translate'
            }
            , {
                enableCellEdit: true, width: 150, name: 'Лист: Тип фильтра',
                field: 'SheetRelationId',
                cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.Relation,
                // editType: 'dropdown', 
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'NameRus' //, editDropdownFilter: 'translate'
            }

            , { name: 'Лист: Значение', field: 'Sheet', enableCellEdit: true }
            , {
                enableCellEdit: true, width: 150, name: 'Тип Данных',
                field: 'DataTypeId',
                headerCellClass: 'Edit',
                cellFilter: 'griddropdownSSS:this',
                //  cellFilter:'mapDataSourceType:this',
                editType: 'dropdown',
                filter:
                {
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: $scope.DataTypeLabel

                },
                //  cellTemplate : '<div class="ui-grid-cell-contents">{{ grid.appScope.Main.typeLookup(COL_FIELD,' + JSON.stringify($scope.DataSourceType) + ') }}</div>',
                //filter: {                 //type: uiGridConstants.filter.SELECT                     selectOptions: mapDataSourceType()         },

                // { type: uiGridConstants.filter.SELECT, condition: uiGridConstants.filter.EXACT },

                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.DataType,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name' //   
                //  , editDropdownFilter: 'translate'
            }
        ]
    };

    angular.extend($scope.gridTemplatesOptions, gridTemplatesOptions);
    $scope.gridTemplatesOptions.onRegisterApi = function (gridApi) {
        gridApi.selection.on.rowSelectionChanged($scope, selectTemplates);

        gridApi.edit.on.afterCellEdit($scope, editRowTemplates);
    };

    function selectTemplates(row) {
        if (row.isSelected) {
            $scope.selectedTemplate = row.entity;

            //    str = JSON.stringify($scope.DataSourceType, null, 4); // (Optional) beautiful indented output.
            //    console.log(str); // Logs output to dev tools console.
            //   alert(str);


            // loadTemplates();
            loadTemplate();
        }
    }

    ///Загрузить все шаблоны выбранного источника
    function loadTemplates() {

        $scope.gridTemplatesOptions.data = [];

        if ($scope.selectedDataSource == null)
            return;

        $scope.templatesLoading = $http({
            method: "POST",
            url: "/DistrRep/GetTemplates/",
            data: JSON.stringify({ id: $scope.selectedDataSource.Id })
        }).then(function (response) {

            $scope.gridTemplatesOptions.data = response.data;
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    //Добавляем Шаблон
    $scope.addTemplate = function () {

        if ($scope.selectedDataSource == null)
            return;

        $scope.sourceLoading = $http({
            method: "POST",
            url: "/DistrRep/AddTemplate/",
            data: JSON.stringify({ id: $scope.selectedDataSource.Id })
        }).then(function (response) {
            $scope.gridTemplatesOptions.data.push(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }
    function editRowTemplates(rowEntity, colDef, newValue, oldValue) {
        str = JSON.stringify(rowEntity, null, 4); // (Optional) beautiful indented output.
        console.log(str);
        var json = JSON.stringify(rowEntity);
        $scope.DataSourceLoading = $http({
            method: "POST",
            url: "/DistrRep/EditTemplates/",
            data: json
        }).then(function () {
            return true;
        }, function () {
            $scope.message = "Unexpected Error";
            rowEntity[colDef.field] = oldValue;
            $scope.$apply();
            return false;
        });

        //     $scope.DataSource = row.entity;
        //  alert(row.entity);
        //  $('#modal_DataSource_Header').val(row.entity);
        //      $('#modal_DataSource').modal('show');

    }



  //  $scope.gridTemplatesFieldOptions = uiGridCustomService.createGridClassMod($scope, 'DistrRep_EditSource_TemplatesGrid_gridTemplatesFieldOptions');
    //createGridClassMod($scope, "Grid_Substance", onSelectionChanged);
    // $scope.Grid_CompanyPeriod.Options.columnDefs = [
    /*
    $scope.gridTemplatesFieldOptions.Options.columnDefs = [
        {headerTooltip: true, field: 'Id', name: 'ИД', width: 34, type: 'number', enableCellEdit: false}
        , {
            enableCellEdit: true, width: 150, name: 'Поле',
            field: 'TemplatesFieldNameId',
            cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
            filter: { condition: uiGridCustomService.condition },
            editableCellTemplate: 'ui-grid/dropdownEditor',
            editDropdownOptionsArray: $scope.TemplatesFieldName,
            // editType: 'dropdown', 
            editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Description' //, editDropdownFilter: 'translate'
        }
        , {
            enableCellEdit: true, width: 150, name: 'Тип фильтра',
            field: 'RelationId',
            cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
            filter: { condition: uiGridCustomService.condition },
            editableCellTemplate: 'ui-grid/dropdownEditor',
            editDropdownOptionsArray: $scope.Relation,
            // editType: 'dropdown', 
            editDropdownIdLabel: 'Id', editDropdownValueLabel: 'NameRus' //, editDropdownFilter: 'translate'
        }
        , { name: 'Название в Файле', field: 'Value', enableCellEdit: true }
        //, { name: 'Справочник', field: 'DicMappping', enableCellEdit: true, type: "boolean", headerTooltip: true }
        , {
            enableCellEdit: true, width: 150, name: 'Справочник',
            field: 'DicMappping',
            headerCellClass: 'Edit',
            cellFilter: 'griddropdownSSS:this',
            //  cellFilter:'mapDataSourceType:this',
            editType: 'dropdown',
            filter:
            {
                type: uiGridConstants.filter.SELECT,
                selectOptions: $scope.YNLabel

            },
            //  cellTemplate : '<div class="ui-grid-cell-contents">{{ grid.appScope.Main.typeLookup(COL_FIELD,' + JSON.stringify($scope.DataSourceType) + ') }}</div>',
            //filter: {                 //type: uiGridConstants.filter.SELECT                     selectOptions: mapDataSourceType()         },

            // { type: uiGridConstants.filter.SELECT, condition: uiGridConstants.filter.EXACT },

            editableCellTemplate: 'ui-grid/dropdownEditor',
            editDropdownOptionsArray: $scope.YN,
            editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name' //   
            //  , editDropdownFilter: 'translate'
        }

       
        , { name: 'Отступ X', field: 'ColOffSet', enableCellEdit: true }
        , { name: 'Отступ Y', field: 'RowOffSet', enableCellEdit: true }
        , {
            headerTooltip: true, name: 'TemplateId', field: 'TemplateId', enableCellEdit: false, visible: false
        }
    ];
    $scope.gridTemplatesFieldOptions.SetDefaults();
    $scope.gridTemplatesFieldOptions.afterCellEdit = editRowTemplate;

*/
    $scope.gridTemplatesFieldOptions = uiGridCustomService.createOptions('DistrRep_EditSource_TemplatesGrid_gridTemplatesFieldOptions');
    var gridTemplatesFieldOptions = {
        customEnableRowSelection: true,
        multiSelect: false,
        enableFullRowSelection: true,
        enableRowHeaderSelection: false,
        appScopeProvider: $scope,
        enableRowSelection: true,
        showGridFooter: false,
        noUnselect: true,
        columnDefs: [
            {
                headerTooltip: true, field: 'Id', name: 'ИД', width: 34, type: 'number', enableCellEdit: false
            }
            , {
                enableCellEdit: true, width: 150, name: 'Поле',
                field: 'TemplatesFieldNameId',
                cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.TemplatesFieldName,
                // editType: 'dropdown', 
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Description' //, editDropdownFilter: 'translate'
            }
            , {
                enableCellEdit: true, width: 150, name: 'Тип фильтра',
                field: 'RelationId',
                cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.Relation,
                // editType: 'dropdown', 
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'NameRus' //, editDropdownFilter: 'translate'
            }
            , { name: 'Название в Файле', field: 'Value', enableCellEdit: true }
            , {
                enableCellEdit: true, width: 150, name: 'Справочник',
                field: 'DicMappping',
                headerCellClass: 'Edit',
                cellFilter: 'griddropdownSSS:this',
                //  cellFilter:'mapDataSourceType:this',
                editType: 'dropdown',
                filter:
                {
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: $scope.YN


                },
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: $scope.YN,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name' //   
                //  , editDropdownFilter: 'translate'
            }
            , { name: 'Отступ X', field: 'ColOffSet', enableCellEdit: true }
            , { name: 'Отступ Y', field: 'RowOffSet', enableCellEdit: true }
            , {
                headerTooltip: true, name: 'TemplateId', field: 'TemplateId', enableCellEdit: false, visible: false
            }
        ]
    };
   // $scope.Grid_CompanyPeriod.Options.columnDefs = [
    angular.extend($scope.gridTemplatesFieldOptions, gridTemplatesFieldOptions);
    $scope.gridTemplatesFieldOptions.onRegisterApi = function (gridApi) {
      //  gridApi.selection.on.rowSelectionChanged($scope, selectTemplates);

        gridApi.edit.on.afterCellEdit($scope, editRowTemplate);
    };
    
    function loadTemplate() {

        $scope.gridTemplatesFieldOptions.data = [];

        if ($scope.selectedTemplate == null)
            return;

        $scope.templatesLoading = $http({
            method: "POST",
            url: "/DistrRep/GetTemplate/",
            data: JSON.stringify({ id: $scope.selectedTemplate.Id })
        }).then(function (response) {

            $scope.gridTemplatesFieldOptions.data = response.data;
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }
    function editRowTemplate(rowEntity, colDef, newValue, oldValue) {
        str = JSON.stringify(rowEntity, null, 4); // (Optional) beautiful indented output.
        console.log(str);
        var json = JSON.stringify(rowEntity);
        $scope.DataSourceLoading = $http({
            method: "POST",
            url: "/DistrRep/EditTemplate/",
            data: json
        }).then(function () {
            return true;
        }, function () {
            $scope.message = "Unexpected Error";
            rowEntity[colDef.field] = oldValue;
            $scope.$apply();
            return false;
        });

        //     $scope.DataSource = row.entity;
        //  alert(row.entity);
        //  $('#modal_DataSource_Header').val(row.entity);
        //      $('#modal_DataSource').modal('show');

    }


    $scope.removeTemplate = function () {

        if ($scope.selectedTemplate == null)
            return;

        var json = JSON.stringify({ id: $scope.selectedTemplate.Id });

        $scope.sourceLoading = $http({
            method: "POST",
            url: "/DistrRep/RemoveTemplate/",
            data: json
        }).then(function (response) {
            var index = $scope.gridTemplatesOptions.data.indexOf($scope.selectedTemplate);
            $scope.gridTemplatesOptions.data.splice(index, 1);
           // $scope.selectedDataSource = null;
            $scope.selectedTemplate = null;
            loadTemplates();
            //   loadTemplate();
            alert(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });



    }


//Конец Шаблоны




    


    $scope.DistrRep_TovarActions_Init = function () {
        $scope.Title = "Корректировка товародвижения";
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/DistrRep_Main/',
            data: JSON.stringify({ param:"DistrRep_TovarActions_Init"})
        }).then(function (response) {
           
            return response.data;
        });
    };

    $scope.DistrRep_HistoryGS_Init = function () {
        $scope.Title = "Справочник Аптек";
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/DistrRep_Main/',
            data: JSON.stringify({ param: "DistrRep_HistoryGS_Init" })
        }).then(function (response) {

            return response.data;
        });
    };


//Загрузка файлов File_info
    $scope.Companylist = [];
    $scope.AllCompanylist = [];
    $scope.DistrRep_DataFiles_Init = function () {
        $scope.Title = "Загрузка Файлов";
 
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/DataFiles/',
            data: JSON.stringify({ param: "DistrRep_DataFiles_Init" })
        }).then(function (response) {
            var data = response.data;
            $scope.filterList = data;
            $scope.filter = data.Filter;
            $scope.AllCompanylist = data.CompanyList;
            $scope.filter.date = new Date(data.Filter.Year, data.Filter.Month - 1, 15);          
            //Отслеживаем изменения поисковой формы
            $scope.$watch(function () { return $scope.filter.date; },
                function () {
                    if ($scope.filter.date) {
                        $scope.filter.Year = $scope.filter.date.getFullYear();
                        $scope.filter.Month = $scope.filter.date.getMonth() + 1;
                    }

                    if (!$scope.fileInfoForm.$invalid)
                        $scope.getInfo();

                }, true);

            $scope.$watch(function () { return $scope.filter.DataSource; },
                function () {
                    if (!$scope.fileInfoForm.$invalid)
                        $scope.getInfo();
                }, true);
            $scope.$watch(function () { return $scope.filter.Company; },
                function () {
                    if (!$scope.fileInfoForm.$invalid)
                        $scope.getInfo();
                }, true);
            $scope.$watch(function () { return $scope.filter.Project; },
                function () {

                    $scope.Companylist = $scope.AllCompanylist.filter(function (c) {
                       return c.ProjectId == $scope.filter.Project.Id;
                    });
                    if (!$scope.fileInfoForm.$invalid) {
                        $scope.filter.DataSource.Id = null;
                        $scope.filter.Company.Id = null;
                        $scope.getInfo();
                    }
                }, true);
            //return response.data;
        });
    };
   
    


    $scope.DataFiles_filterList = null;
    $scope.DataFiles_filter = {};



    $scope.CompanyFilterList = null;

    $scope.CompanyFilterList = function () {

        if ($scope.filter.Project.id.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/GetCompany/",
            data: JSON.stringify({ ProjectId: $scope.filter.Project.id })
        }).then(function (response) {
            response.data;
        }, function () {
            $scope.FileInfo_Grid.Options.data = null;
        });
    };

     


    $scope.FileInfo_Grid = uiGridCustomService.createGridClass($scope, 'FileInfo_Grid');
    $scope.FileInfo_Grid.Options.columnDefs = [
        { name: 'Id', field: 'Id', width: 50, type: 'number' },
        { name: 'Год', field: 'Year', width: 50, type: 'number' },
        { name: 'Месяц', field: 'Month', width: 50, type: 'number' },        
        { name: 'Дата добавления', field: 'DateInsert', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, width: 180 },
    //    { name: 'Описание ошибки', field: 'Description' },
        { name: 'Путь', field: 'FilePath' },
        { name: 'Статус', field: 'FileStatusT', width: 200 }
    ];


    $scope.FileInfo_Grid.Options.customEnableRowSelection = true;
 
    $scope.FileInfo_Grid.Options.multiSelect= false;
    $scope.FileInfo_Grid.Options.enableFullRowSelection = true;
    $scope.FileInfo_Grid.Options.enableRowHeaderSelection = false;
    $scope.FileInfo_Grid.Options.enableRowSelection= true;
    $scope.FileInfo_Grid.Options.noUnselect= true;
    $scope.FileInfo_Grid.Options.appScopeProvider= $scope;
    $scope.FileInfo_Grid.Options.showGridFooter = true;

    $scope.getInfo = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/GetFileInfo/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            $scope.FileInfo_Grid.Options.data = response.data;
        }, function () {
                $scope.FileInfo_Grid.Options.data = null;
        });
    };

    //Отправить файлы на удаление
    $scope.deleteFiles = function () {

        var r = confirm("Вы уверены что хотите Удалить выбранные файлы?");
        if (r == true) {
            txt = "You pressed OK!";

            var item = $scope.selectedFileInfo_Grid_Detail ;

            
            $scope.dataLoading = $http({
                method: "POST",
                url: "/DistrRep/DeleteFile/",
                data: JSON.stringify({ id: item.Id })
            }).then(function (response) {
                alert(response.data);
                $scope.getInfo();
            }, function () {

            });
        }
    }
    $scope.canDelete = function () {
        var item = $scope.selectedFileInfo_Grid_Detail;
       // alert(item);
        if (item == null)// || item.length === 0)
            return false;

       
        return true
    }

    //Отправить файлы на перезагрузку
    $scope.reloadFiles = function () {

        var item = $scope.selectedFileInfo_Grid_Detail;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/ReloadFile/",
            data: JSON.stringify({ id: item.Id })
        }).then(function (response) {
            alert(response.data);
            $scope.getInfo();
        }, function () {
            $scope.getInfo();
        });
    }
    //Возможность отправить файлы на перезагрузку
    $scope.canReload = function () {
        var item = $scope.selectedFileInfo_Grid_Detail ;

        if (item == null)// || item.length === 0)
            return false;


        return true
    }

    $scope.selectedFileInfo_Grid_Detail = null;

    $scope.FileInfo_Grid_Detail = uiGridCustomService.createGridClass($scope, 'FileInfo_Grid_Detail');
    $scope.FileInfo_Grid_Detail.Options.columnDefs = [
        { name: 'Id', field: 'FileInfoId', width: 50, type: 'number' },
        { name: 'Лист', field: 'Sheet'},
        { name: 'К-во строк', field: 'Cnt', width: 120, type: 'number' },
        { name: 'Кол-во закупок', field: 'PurchaseCount', width: 120, type: 'number' },
        { name: 'Сумма закупок', field: 'PurchaseSum', width: 120, type: 'number' },
        { name: 'Кол-во продаж', field: 'SellingCount', width: 120, type: 'number' },
        { name: 'Сумма продаж', field: 'SellingSum', width: 120, type: 'number' },
        { name: 'Кол-во остатка', field: 'StockCount', width: 120, type: 'number' },
        { name: 'Сумма остатка', field: 'StockSum', width: 120, type: 'number' },   
        { name: 'Статус', field: 'SheetStatusT' }       
    ];


    function getInfoDetail() {
        $scope.FileInfo_Grid_Detail.data = [];
        if ($scope.selectedFileInfo_Grid_Detail  == null)
            return;
        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/GetFileInfoDetail/",
            data: JSON.stringify({ id: $scope.selectedFileInfo_Grid_Detail.Id  })
        }).then(function (response) {
            $scope.FileInfo_Grid_Detail.Options.data = response.data;
        }, function () {
                $scope.FileInfo_Grid_Detail.Options.data = null;
        });
    };

 //   $scope.selectedFileInfo_Grid_Detail_6FP = null;
    $scope.FileInfo_Grid_Detail_6FP = uiGridCustomService.createGridClass($scope, 'FileInfo_Grid_Detail_6FP');
    $scope.FileInfo_Grid_Detail_6FP.Options.columnDefs = [
        { name: 'Id', field: 'FileInfoId', width: 50, type: 'number' },
        { name: 'Лист', field: 'Sheet' },
        { name: 'К-во строк', field: 'Cnt', width: 120, type: 'number' },
        { name: 'Кол-во упаковок', field: 'PurchaseCount', width: 120, type: 'number' },
        { name: 'Сумма чеков', field: 'PurchaseSum', width: 120, type: 'number' },
        { name: 'К-во Е+ в [FN]', field: 'SellingCount', width: 120, type: 'number' },
        { name: 'Статус', field: 'SheetStatusT' }
    ];
    function getInfoDetail_6FP() {
        $scope.FileInfo_Grid_Detail_6FP.data = [];
        if ($scope.selectedFileInfo_Grid_Detail == null)
            return;
        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/GetFileInfoDetail/",
            data: JSON.stringify({ id: $scope.selectedFileInfo_Grid_Detail.Id })
        }).then(function (response) {
            $scope.FileInfo_Grid_Detail_6FP.Options.data = response.data;
        }, function () {
                $scope.FileInfo_Grid_Detail_6FP.Options.data = null;
        });
    };


    $scope.FileInfo_Grid.Options.onRegisterApi = function (gridApi) {
        gridApi.selection.on.rowSelectionChanged($scope, selectFileInfo);
    };
   
    function selectFileInfo(row) {
        $scope.selectedFileInfo_Grid_Detail = null;
        if (row.isSelected) {
            $scope.selectedFileInfo_Grid_Detail = row.entity;
           // alert($scope.filter.Project.Id);
            if ($scope.filter.Project.Id == 2) {
                getInfoDetail_6FP()
            } else {
                getInfoDetail();
            }
        }
    }

    $scope.checkFiles = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/CheckNewFileInfo/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
           // $scope.FileInfo_Grid.Options.data = response.data;
            alert(response.data);
        }, function () {
                $scope.FileInfo_Grid.Options.data = null;
        });
    }

    $scope.uploadFiles = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/UploadFiles/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            // $scope.FileInfo_Grid.Options.data = response.data;
            alert(response.data);
        }, function () {
            $scope.FileInfo_Grid.Options.data = null;
        });
    }
    
    $scope.sendToClassification = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/SendToClassification/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            // $scope.FileInfo_Grid.Options.data = response.data;
            alert(response.data);
        }, function () {
            $scope.FileInfo_Grid.Options.data = null;
        });
    }
    $scope.fromAPI = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/fromAPI/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            // $scope.FileInfo_Grid.Options.data = response.data;
            alert(response.data);
        }, function () {
            $scope.FileInfo_Grid.Options.data = null;
        });
    }
    $scope.toFtp = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/toFtp/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            // $scope.FileInfo_Grid.Options.data = response.data;
            alert(response.data);
        }, function () {
            $scope.FileInfo_Grid.Options.data = null;
        });
    }
    
    $scope.toQlik = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/ToQlik/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            // $scope.FileInfo_Grid.Options.data = response.data;
            alert(response.data);
        }, function () {
            $scope.FileInfo_Grid.Options.data = null;
        });
    }
    $scope.toQlikProd = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/ToQlikProd/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            // $scope.FileInfo_Grid.Options.data = response.data;
            alert(response.data);
        }, function () {
            $scope.FileInfo_Grid.Options.data = null;
        });
    }
    $scope.getErrorInfo = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/GetErrorInfo/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            $scope.FileInfo_Grid.Options.data = response.data;
        }, function () {
            $scope.FileInfo_Grid.Options.data = null;
        });
    }

    $scope.canShowError = function () {
        return $scope.filter.Year != null && $scope.filter.Month != null && $scope.filter.Year != undefined && $scope.filter.Month != undefined;
    }


    $scope.TaskList_Grid = uiGridCustomService.createGridClass($scope, 'TaskList_Grid');
    $scope.TaskList_Grid.Options.appScopeProvider = $scope;
    $scope.TaskList_Grid.Options.columnDefs = [
        { name: 'Id', field: 'Id', width: 50, type: 'number' },
        { name: 'Компания', field: 'Company' },
        { name: 'Тип', field: 'Type'},
        { name: 'Задача', field: 'Name'},
        { name: 'Статус', field: 'Status'},
        { name: 'Дата последнего запуска', field: 'DateLastStart', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, width: 180 },
        { name: 'Дата последнего выполнения', field: 'DateLastComplete', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, width: 180 }    

    ];
   
    $('#StatusModal').on('show.bs.modal', function (event) {
        getTaskList();
    })

    function getTaskList() {
        $scope.TaskList_Grid.data = [];
        str = JSON.stringify($scope.filter, null, 4); // (Optional) beautiful indented output.
        console.log(str);

        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/GetTaskList",
            data: JSON.stringify({ filter: $scope.filter  })
        }).then(function (response) {
            $scope.TaskList_Grid.Options.data = response.data;
            str = JSON.stringify(response.data, null, 4); // (Optional) beautiful indented output.
            console.log(str);

        }, function () {
                $scope.TaskList_Grid.Options.data = null;
        });
    };

    $scope.GetReportSheet_ToExcel = function () {
        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/DistrRep/GetReportSheet_ToExcel/',
            data: JSON.stringify({ filter: $scope.filter }),
            responseType: 'arraybuffer'         

        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var now = new Date();
            var fileName = 'ReportSheet.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.GetReportSheet_top15_ToExcel = function () {
        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/DistrRep/GetReportSheet_top15_ToExcel/',
            data: JSON.stringify({ filter: $scope.filter }),
            responseType: 'arraybuffer'

        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var now = new Date();
            var fileName = 'ReportSheet_top15.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.DistrRep_Rules_Init = function () {
        $scope.Title = "Правила определения региона";


        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/DistrRep_Main/',
            data: JSON.stringify({ param: "DistrRep_Rules_Init" })
        }).then(function (response) {

            return response.data;
        });


        



    };

    //Правила
   
    selectedRules_Clients = null;

    $scope.Rules_Clients_Grid = uiGridCustomService.createGridClass($scope, 'Rules_Clients_Grid');
    $scope.Rules_Clients_Grid.Options.appScopeProvider = $scope;
    $scope.Rules_Clients_Grid.Options.columnDefs = [
        { name: 'Id', field: 'Id', width: 50, type: 'number', enableCellEdit: false  },       
        { name: 'Название', field: 'Name', enableCellEdit: true  },
        { name: 'ИНН', field: 'INN', enableCellEdit: true },
        { name: 'Адрес исх.', field: 'Region_Before', enableCellEdit: true },
        { name: 'Регион Клиента', field: 'Region_After', enableCellEdit: true }
    ];
    $scope.Rules_Clients_Grid.Options.multiSelect = false;
    $scope.Rules_Clients_Grid.Options.enableFullRowSelection = true;
    $scope.Rules_Clients_Grid.Options.enableRowHeaderSelection= false;
  //  $scope.Rules_Clients_Grid.Options.appScopeProvider = $scope;
    $scope.Rules_Clients_Grid.Options.enableRowSelection= true;
    $scope.Rules_Clients_Grid.Options.showGridFooter = false;
    $scope.Rules_Clients_Grid.Options.noUnselect= true;
    $scope.Rules_filterList = null;
    $scope.Rules_filter = {};

    /// angular.extend($scope.Rules_Clients_Grid.Options, Rules_Clients_Grid.Options);
    $scope.Rules_Clients_Grid.Options.onRegisterApi = function (gridApi) {
        gridApi.selection.on.rowSelectionChanged($scope, selectRules_Clients);
        //  gridApi.edit.on.beginCellEdit($scope, editRowDataSource)
        gridApi.edit.on.afterCellEdit($scope, EditRules_Clients);
    };


    function selectRules_Clients(row) {
        if (row.isSelected) {
            $scope.selectedRules_Clients = row.entity;          
         //   str = JSON.stringify($scope.DataSourceType, null, 4); // (Optional) beautiful indented output.
            //    console.log(str); // Logs output to dev tools console.
            //   alert(str);

            /* str123 = JSON.stringify($scope.DataSourceType.map(function (obj) {
            /* str123 = JSON.stringify($scope.DataSourceType.map(function (obj) {
                 var rObj = { "value": obj.Id, "label": obj.Name };
                 return rObj;
             }), null, 4);
             console.log(str123);
 
             str11 = JSON.stringify($scope.DataSourceType, null, 4);
             console.log(str11);
             */

        }
    }

   
    $scope.AddRules_Clients = function () {


        $scope.sourceLoading = $http({
            method: "POST",
            url: "/DistrRep/AddRules_Clients/",
              data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            $scope.Rules_Clients_Grid.Options.data.push(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }
    function EditRules_Clients(rowEntity, colDef, newValue, oldValue) {
        str = JSON.stringify(rowEntity, null, 4); // (Optional) beautiful indented output.
        console.log(str);
        var json = JSON.stringify(rowEntity);
        $scope.DataSourceLoading = $http({
            method: "POST",
            url: "/DistrRep/EditRules_Clients/",
            data: json
        }).then(function () {
            return true;
        }, function () {
            $scope.message = "Unexpected Error";
            rowEntity[colDef.field] = oldValue;
            $scope.$apply();
            return false;
        });

        //     $scope.DataSource = row.entity;
        //  alert(row.entity);
        //  $('#modal_DataSource_Header').val(row.entity);
        //      $('#modal_DataSource').modal('show');

    }
     //Удаляем источник
    $scope.removeRules_Clients = function () {

        if ($scope.selectedRules_Clients == null)
            return;

        var json = JSON.stringify({ id: $scope.selectedRules_Clients.Id });

        $scope.sourceLoading = $http({
            method: "POST",
            url: "/DistrRep/RemoveRules_Clients/",
            data: json
        }).then(function (response) {
            var index = $scope.Rules_Clients_Grid.Options.data.indexOf($scope.selectedRules_Clients);
            $scope.Rules_Clients_Grid.Options.data.splice(index, 1);
            $scope.selectedRules_Clients = null;        
           
         
            alert(response.data);
        }, function () {
            $scope.message = "Unexpected Error";
        });



    }
    function getRules_Clients() {
        $scope.Rules_Clients_Grid.data = [];
        if ($scope.RulesForm.$invalid)
            return;
        $scope.dataLoading = $http({
            method: "POST",
            url: "/DistrRep/GetRules_Clients/",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            $scope.Rules_Clients_Grid.Options.data = response.data;
        }, function () {
                $scope.Rules_Clients_Grid.Options.data = null;
        });
    };
    $scope.DistrRep_Rules_Init = function () {
        $scope.Title = "Rules";

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/Rules/',
            data: JSON.stringify({ param: "DistrRep_Rules_Init" })
        }).then(function (response) {
            var data = response.data;
            $scope.filterList = data;
            $scope.filter = data.Filter;          
           // $scope.filter.date = new Date(data.Filter.Year, data.Filter.Month - 1, 15);
            //Отслеживаем изменения поисковой формы
            $scope.$watch(function () { return $scope.filter.date; },
                function () {
                    if ($scope.filter.date) {
                        $scope.filter.Year = $scope.filter.date.getFullYear();
                        $scope.filter.Month = $scope.filter.date.getMonth() + 1;
                    }

                    if (!$scope.RulesForm.$invalid)
                        getRules_Clients();

                }, true);

            $scope.$watch(function () { return $scope.filter.Company; },
                function () {
                    if (!$scope.RulesForm.$invalid)
                        getRules_Clients();
                }, true);
            //return response.data;
            str = JSON.stringify($scope.filter, null, 4); // (Optional) beautiful indented output.
            console.log(str);

        });
    };
//Конец загрузка файлов
    $scope.DistrRep_EditSource_Init = function () {
        $scope.Title = "Редактор источников и шаблонов";
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/DistrRep_Main/',
            data: JSON.stringify({ param: "DistrRep_EditSource_Init" })
        }).then(function (response) {

            return response.data;
        });
    };


////////////////////////////////// DistrRep_RawData_Init Начало
    $scope.DistrRep_RawData_Init = function () {
        $scope.format = 'dd.MM.yyyy';
        $scope.UsersCL = "";
        //для поиска в данных
        $scope.filter = {
            top: 300,
            CompanyId: 0,
            DataSourceId: 0,
            DataSourceTypeId: 0,
            //spr_DistributionType: 0,
            Spec: "",
            Search_text: ""            
        };
        $scope.spr_Comp = [];//
        $scope.spr_Tops = [];//
        $scope.spr_DataSource = [];//
        $scope.spr_DataSourceType = [];//
        $scope.spr_DistributionType = [];
        $scope.spr_Spec = [];



        $scope.NeedSave = false;
        $scope.HaveData = false;
        hotkeys.bindTo($scope).add({
            combo: 'shift+f',
            description: 'Поиск по справочнику',
            callback: function (event) {
                $scope.DistrRep_RawData_shiftF(event);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+c',
            description: 'Копировать',
            callback: function (event) {
                $scope.DistrRep_RawData_Copy();
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+v',
            description: 'Вставить',
            callback: function (event) {
                $scope.DistrRep_RawData_Paste();
            }
        });
        
        $scope.Grid = uiGridCustomService.createGridClass($scope, 'GS_Grid');
        $scope.Grid.Options.showGridFooter = true;
        $scope.Grid.Options.multiSelect = true;
        $scope.Grid.Options.modifierKeysToMultiSelect = true;
        $scope.Grid.Options.rowTemplate = '<div ng-class="{\'modify\' : row.entity[\'@modify\']==true}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';
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
        $scope.Grid.Options.noUnselect = false;

        $scope.Grid.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.NeedSave = true;
                    }
                }
            });

            //gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            //    onSelectionChanged(row);
            //});

            //gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            //    onSelectionChanged(rows[0]);
            //});
        };

        $scope.Grid.Options.columnDefs = [
            { visible: false, headerTooltip: true, name: 'Id', width: 100, field: 'Id', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, name: 'Год', width: 100, field: 'Year', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, name: 'Месяц', width: 100, field: 'Month', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, name: 'DataSourceType', width: 100, field: 'DataSourceType', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'DataSource', width: 100, field: 'DataSource', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'PharmacyName', width: 100, field: 'PharmacyName', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'LegalName', width: 100, field: 'LegalName', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'INN', width: 100, field: 'INN', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Region', width: 100, field: 'Region', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'City', width: 100, field: 'City', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Address', width: 100, field: 'Address', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'GSId', width: 100, field: 'GSId', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, name: 'PharmacyId', width: 100, field: 'PharmacyId', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, name: 'EntityINN', width: 100, field: 'EntityINN', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'EntityName', width: 100, field: 'EntityName', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'NetworkName', width: 100, field: 'NetworkName', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Address_region', width: 100, field: 'Address_region', hasCustomWidth: false, headerCellClass: 'NavajoWhiteRed', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Address_city', width: 100, field: 'Address_city', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Address_street', width: 100, field: 'Address_street', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'PharmacyBrand', width: 100, field: 'PharmacyBrand', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Comments', width: 100, field: 'Comments', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Spark', width: 100, field: 'Spark', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Наименование поставщика', width: 100, field: 'Distributor', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Филиал поставщика', width: 100, field: 'DistributorBranch', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },

            { headerTooltip: true, name: 'Остатки уп', width: 100, field: 'StockCount', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { headerTooltip: true, name: 'Остатки руб', width: 100, field: 'StockSum', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { headerTooltip: true, name: 'Закупки уп', width: 100, field: 'PurchaseCount', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { headerTooltip: true, name: 'Закупки руб', width: 100, field: 'PurchaseSum', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { headerTooltip: true, name: 'Продажи уп', width: 100, field: 'SellingCount', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { headerTooltip: true, name: 'Продажи руб', width: 100, field: 'SellingSum', headerCellClass: 'NavajoWhite', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },

            { headerTooltip: true, name: 'Тип Получателя', width: 100, field: 'DistributionType', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Тип Получателя Кто', width: 100, field: 'DistributionTypeId_UserFullName', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Тип Получателя Когда', width: 100, field: 'DistributionTypeId_Date', hasCustomWidth: false, headerCellClass: 'NavajoWhite', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/DistrRep_RawData_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.spr_Comp, response.data.spr_Comp);
            Array.prototype.push.apply($scope.spr_Tops, response.data.spr_Tops);
            Array.prototype.push.apply($scope.spr_DataSource, response.data.spr_DataSource);
            Array.prototype.push.apply($scope.spr_DataSourceType, response.data.spr_DataSourceType);
            Array.prototype.push.apply($scope.spr_DistributionType, response.data.spr_DistributionType);
            Array.prototype.push.apply($scope.spr_Spec, response.data.spr_Spec);

            $scope.filter.CompanyId = $scope.spr_Comp[0].code;
            $scope.filter.top = $scope.spr_Tops[0].code;
            $scope.filter.DataSourceId = $scope.spr_DataSource[0].code;
            $scope.filter.DataSourceTypeId = $scope.spr_DataSourceType[0].code;
            $scope.filter.Spec = $scope.spr_Spec[0].code;

        });
    };
    $scope.DistrRep_RawData_ShowData = function () {
        if ($scope.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.DistrRep_RawData_SetData(1);
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.DistrRep_RawData_ShowDataAC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.DistrRep_RawData_ShowDataAC();
        }

    };
    $scope.DistrRep_RawData_ShowDataAC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DistrRep/DistrRep_RawData_GetData/',
                data: JSON.stringify({ filter: $scope.filter})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.NeedSave = false;
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
    $scope.DistrRep_RawData_SetData = function (level) {
        var array_upd = [];
        $scope.Grid.Options.data.forEach(function (item, i, arr) {
            if (item["@modify"] === true) {
                array_upd.push(item);
            }
        });
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DistrRep/DistrRep_RawData_SetData/',
                data: JSON.stringify({ array: array_upd})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.NeedSave = false;
                    $scope.Grid.Options.data.forEach(function (item, i, arr) {
                        if (item["@modify"] === true) {
                            item["@modify"] = false;
                        }
                    });
                    if (level === 1) {
                        $scope.DistrRep_RawData_ShowDataAC();
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
                return;
            });

    };
    $scope.DistrRep_RawData_Set_DistributionType = function (DistributionTypeId, DistributionType) {
        //alert('Запрещено');//Считаем что этот тип является образующим и тут не устанавливается.
        setIsShift(false);
        var selectedRows = $scope.gridApi.selection.getSelectedRows();
        selectedRows.forEach(function (item) {
            item.DistributionTypeId = DistributionTypeId;
            item.DistributionType = DistributionType;

            item.DistributionTypeId_UserFullName = $scope.user.Fullname;

            item["@modify"] = true;
            $scope.NeedSave = true;
        });
    };
////////////////////////////////// DistrRep_RawData_Init Начало
    ////////////////////////////// CompanyPeriod для Старт
    $scope.CompanyPeriod_Init = function () {
        $scope.Grid_CompanyPeriod = uiGridCustomService.createGridClassMod($scope, "Grid_CompanyPeriod");
        $scope.Grid_CompanyPeriod.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id' },
            { headerTooltip: true, name: 'Компания', field: 'Company.Company' },
            { headerTooltip: true, name: 'Проект', field: 'Company.Project.Name' },
            { headerTooltip: true, name: 'Период', field: 'period', type: "number" },
            { headerTooltip: true, enableCellEdit: true, name: 'Клиентам', field: 'toProd', type: "boolean" }
        ];
        $scope.Grid_CompanyPeriod.SetDefaults();

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/CompanyPeriod_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.CompanyPeriod_Search();
            return 1;
        });
    };
    $scope.CompanyPeriod_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DistrRep/CompanyPeriod_Search/',
                data: JSON.stringify({})
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_CompanyPeriod.Options.data = response.data.Data.CompanyPeriod;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.CompanyPeriod_Search = function () {
        if ($scope.Grid_CompanyPeriod.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.CompanyPeriod_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.CompanyPeriod_Search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.CompanyPeriod_Search_AC();
        }

    };
    $scope.CompanyPeriod_Save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DistrRep/CompanyPeriod_Save/',
                data: JSON.stringify({
                    array: $scope.Grid_CompanyPeriod.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.CompanyPeriod_Search_AC();
                    }
                    else {
                        $scope.Grid_CompanyPeriod.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
   ////////////////////////////// CompanyPeriod для Окончание
}
