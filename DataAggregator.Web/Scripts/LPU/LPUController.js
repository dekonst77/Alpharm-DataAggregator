﻿angular
    .module('DataAggregatorModule')
    .controller('LPUController', ['$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'errorHandlerService', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', 'userService', LPUController])
    .constant('DepartmentSource', {
        type: 'object',
        properties: {
            Name: { type: 'string', title: 'Подразденеие' },           
            }
        }
    ).filter('griddropdownSSS', function () {
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
function LPUController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, errorHandlerService, uiGridCustomService, uiGridConstants, formatConstants, userServic) {
     LPUID_Val = null;
    $scope.selectedDept = null;
    $scope.selectedDepartment = null;
    $scope.selectedKind = null;
    $scope.selectedType = null;
    /////Department
    //Dep_Grid
    $scope.LPUStatusLabel = [];
    $scope.LPUStatuss = [];
    $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'LPU_Grid');
    $scope.Grid.Options.showGridFooter = true;
    $scope.Grid.Options.multiSelect = true;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;

    $scope.DepartmentSource = [];
    $scope.DepartmentSourceLabel = [];

    $scope.LPUStatus = function () {
        $http({
            method: "POST",
            url: "/LPU/Get_LPU_Status/",

        }).then(function (response) {

            Array.prototype.push.apply($scope.LPUStatuss, response.data.Data);
            Array.prototype.push.apply($scope.LPUStatusLabel, $scope.LPUStatuss.map(function (obj) {
                var rObj = { 'value': obj.Id, 'label': obj.Name };
                return rObj;
            }));



        }, function () {
            $scope.message = "Unexpected Error";
        });
    };
    $scope.LPUStatus();

    $scope.LPUType = [];
   // $scope.LPUTypeFilter = [{ 'Id': -1, 'Name': 'Показать Все' }];
    $scope.LPUTypeLabel = [];
    $scope.LPUKind = [];
    //$scope.LPUKindFilter = [{ 'Id': -1, 'Name': 'Показать Все' }];
    $scope.LPUKindLabel = [];

    //#A3F06C
    $scope.Grid.Options.columnDefs = [
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'LPUId', field: 'LPUId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ParentId', field: 'ParentId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'ActualId', field: 'ActualId', filter: { condition: uiGridCustomService.numberCondition } },
        {
            headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Отделения', field: 'DepartmentCnt', filter: { condition: uiGridCustomService.numberCondition }
            , cellTemplate: '<button ng-disabled="{{row.entity.IsDepartment}} >=1 || {{row.entity.ParentId}}-1 >0"  style="width:100%; {{row.entity.DepartmentCnt >0 ? \'background-color: #A3F06C;\' : (row.entity.IsDepartment>0? \'background-color: #00BFFF;\'  :\'\') }} " class="btn btn-sm"  id="{{row.entity.LPUId}}"  onclick="LPUID_Val= this.id;GetDepartments();"  data-toggle="modal" data-target="#DepModal" ><b>{{COL_FIELD}}</b></button>'
            //  , cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm"  id="a{{row.entity.LPUId}}"  data-toggle="modal" data-target="#DepModal" data-whatever="{{row.entity.LPUId}}" >{{COL_FIELD}}</button>'
        },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Название Отделения', headerCellClass: 'Yellow', field: 'Department', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'OrganizationId', field: 'OrganizationId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'PointId', field: 'PointId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Коммент', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 300, name: 'ИНН юр. лица', field: 'EntityINN', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Organization?inn={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { headerTooltip: true, enableCellEdit: false, width: 300, name: 'ОГРН юр. лица', field: 'EntityOGRN', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 300, name: 'Юр. лицо', field: 'EntityName', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
        //{ headerTooltip: true, enableCellEdit: true, width: 300, name: 'Бренд аптеки', field: 'PharmacyBrand', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, name: 'Адрес из лицензии', width: 300, field: 'Address', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, name: 'ФО', width: 100, field: 'Bricks_FederalDistrict', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: true, width: 100, name: 'СФ', field: 'Address_region', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
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
        { headerTooltip: true, enableCellEdit: false, name: 'Дата Добавления', width: 100, field: 'Date_Create', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
        { headerTooltip: true, enableCellEdit: false, name: 'Лицензия', width: 100, field: 'licencesNumber', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
        { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Телефон', field: 'Phone', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: true, width: 100, name: 'Web', field: 'Website', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: true, width: 100, name: 'ФИО зав. аптеки', field: 'ContactPersonFullname', filter: { condition: uiGridCustomService.condition } },
        {
            enableCellEdit: true,
            width: 150,
            name: 'Подразделение',
            field: 'DepartmentId',
            headerCellClass: 'Edit',
            cellFilter: 'griddropdownSSS:this',
            editType: 'dropdown',
            filter:
            {
                type: uiGridConstants.filter.SELECT,
                selectOptions: $scope.DepartmentSourceLabel

            },
            editableCellTemplate: 'ui-grid/dropdownEditor',
            editDropdownOptionsArray: $scope.DepartmentSource,
            editDropdownIdLabel: 'Id',
            editDropdownValueLabel: 'Name'
        },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Это Отделение', field: 'IsDepartment', filter: { condition: uiGridCustomService.numberCondition } },
        { enableCellEdit: false, width: 100, name: 'Status', field: 'Status', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
        {
            enableCellEdit: true,
            width: 150,
            name: 'Тип',
            field: 'TypeId',
            headerCellClass: 'Edit',
            cellFilter: 'griddropdownSSS:this',
            editType: 'dropdown',
            filter:
            {
                type: uiGridConstants.filter.SELECT,
                selectOptions: $scope.LPUTypeLabel

            },
            editableCellTemplate: 'ui-grid/dropdownEditor',
            editDropdownOptionsArray: $scope.LPUType,
            editDropdownIdLabel: 'Id',
            editDropdownValueLabel: 'Name'
        },
        {
            enableCellEdit: true,
            width: 150,
            name: 'Вид отделения',
            field: 'KindId',
            headerCellClass: 'Edit',
            cellFilter: 'griddropdownSSS:this',
            editType: 'dropdown',
            filter:
            {
                type: uiGridConstants.filter.SELECT,
                selectOptions: $scope.LPUKindLabel

            },
            editableCellTemplate: 'ui-grid/dropdownEditor',
            editDropdownOptionsArray: $scope.LPUKind,
            editDropdownIdLabel: 'Id',
            editDropdownValueLabel: 'Name'
        },
    ];
    $scope.filter = {        
        IsActual: true
    };
  

    //row.entity.Id
    $scope.GetDepartmentSource = function () {
        $http({
            method: "POST",
            url: "/LPU/GetDepartmentSource/",

        }).then(function (response) {
            Array.prototype.push.apply($scope.DepartmentSource, response.data.Data);
            Array.prototype.push.apply($scope.DepartmentSourceLabel, $scope.DepartmentSource.map(function (obj) {
                var rObj = { 'value': obj.Id, 'label': obj.Name };
                return rObj;
            }));


        }, function () {
            $scope.message = "Unexpected Error";
        });
    };

    $scope.GetDepartmentSource();


    $scope.GetLPUType = function () {
        $http({
            method: "POST",
            url: "/LPUDictionaries/GetLPUType/",

        }).then(function (response) {
            Array.prototype.push.apply($scope.LPUType, response.data.Data);
          //  Array.prototype.push.apply($scope.LPUTypeFilter, response.data.Data);
            Array.prototype.push.apply($scope.LPUTypeLabel, $scope.LPUType.map(function (obj) {
                var rObj = { 'value': obj.Id, 'label': obj.Name };
                return rObj;
            }));


        }, function () {
            $scope.message = "Unexpected Error";
        });
    };

    $scope.GetLPUType();

    $scope.GetLPUKind = function () {
        $http({
            method: "POST",
            url: "/LPUDictionaries/GetLPUKind/",

        }).then(function (response) {
            Array.prototype.push.apply($scope.LPUKind, response.data.Data);
            Array.prototype.push.apply($scope.LPUKindLabel, $scope.LPUKind.map(function (obj) {
                var rObj = { 'value': obj.Id, 'label': obj.Name };
                return rObj;
            }));


        }, function () {
            $scope.message = "Unexpected Error";
        });
    };

    $scope.GetLPUKind();


    $scope.Search = function () {
   
       
        var SearchFilter = [];
         SearchFilter = $scope.filter;
        if ($scope.filter.TypeIdModel != null)
            SearchFilter["TypeId"] = $scope.filter.TypeIdModel.Id;        
        else SearchFilter["TypeId"] = null;
        if ($scope.filter.KindIdModel != null)
            SearchFilter["KindId"] = $scope.filter.KindIdModel.Id;
        else SearchFilter["KindId"] = null;

        if ($scope.filter.StatusModel != null)
            SearchFilter["Status"] = $scope.filter.StatusModel.Id;
        else SearchFilter["Status"] = null;
        var json = JSON.stringify(SearchFilter);

        $scope.classifierLoading =
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/LPU/LoadLPU/',
                data: json
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.Options.data = data.Data;
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    //loadLPU();


    $scope.Save = function () {
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/LPU/Save/',
                    data: JSON.stringify({ lpumodels: array_upd })
                }).then(function (response) {
                    if (response.data.Success) {
                        $scope.Grid.ClearModify();
                        alert("Сохранил");
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };

    $scope.ToExcel = function () {
        var array_upd = [];

        $scope.Grid.Options.data.forEach(function (item, i, arr) {
            array_upd.push(item.LPUId);
        });

        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/LPU/ToExcel/',
            data: JSON.stringify({ ids: array_upd }),
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'lpu.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.FromExcel = function (files) {

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/LPU/FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
            }).then(function (response) {
                $scope.Search();
            }, function (response) {
                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };

    $scope.AddLPU = function () {
        var json = JSON.stringify($scope.AddLPUForm);      
        if (json && json.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/LPU/AddLPU/',
                    data:  json 
                }).then(function (response) {
                    if (response.data.Success) {
                        $scope.Grid.ClearModify();
                       // $scope.Search();
                        alert("Добавил");

                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        } else alert('Вы не заполнили ни одного поля. ЛПУ не будет добавлено!');

    };

    
    //Блок Вид подразделения
    $scope.KindGrid = uiGridCustomService.createGridClassMod($scope, 'KindGrid');
 
    $scope.KindGrid.Options.multiSelect = false;
    $scope.KindGrid.Options.noUnselect = false;
    $scope.KindGrid.Options.showGridFooter = true;
    $scope.KindGrid.Options.columnDefs = [
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Вид отделения', field: 'Name', filter: { condition: uiGridCustomService.condition } }
    ];
    $scope.KindGrid.data = [];
   
    $scope.KindGrid.Options.onRegisterApi = function (gridApi) {
        $scope.KindGridApi = gridApi;
   };

    $('#KindModal').on('show.bs.modal', function (event) {
        getKindList();
    });



    function getKindList() {
        //очищаем выбор если был
        if ($scope.KindGridApi.selection.getSelectedRows() != undefined && $scope.KindGridApi.selection.getSelectedRows() != null && $scope.KindGridApi.selection.getSelectedRows().length > 0) {
            $scope.KindGridApi.selection.clearSelectedRows();
        }
        $scope.KindGrid.data = [];
        $scope.KindGrid.Options.data = [];
        $scope.KindGrid.Options.data = null;
        $scope.KindGrid.Options.data = $scope.LPUKind;
    };

    $scope.SelectKind = function () {
        if ($scope.Grid.selectedRows() === undefined || $scope.Grid.selectedRows().length < 1 || $scope.KindGridApi.selection.getSelectedRows() === undefined || $scope.KindGridApi.selection.getSelectedRows() === null || $scope.KindGridApi.selection.getSelectedRows().length < 1) {
            return;
        }
        var Id=  $scope.KindGridApi.selection.getSelectedRows()[0].Id
        var selectedRows = $scope.Grid.selectedRows();
        selectedRows.forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "KindId", Id);
        
        });

    };
 //Конец Блок Вид подразделения


 //Блок Тип подразделения
    $scope.TypeGrid = uiGridCustomService.createGridClassMod($scope, 'TypeGrid');

    $scope.TypeGrid.Options.multiSelect = false;
    $scope.TypeGrid.Options.noUnselect = false;
    $scope.TypeGrid.Options.showGridFooter = true;
    $scope.TypeGrid.Options.columnDefs = [
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Тип подразделения', field: 'Name', filter: { condition: uiGridCustomService.condition } }
    ];
    $scope.TypeGrid.data = [];

    $scope.TypeGrid.Options.onRegisterApi = function (gridApi) {
        $scope.TypeGridApi = gridApi;
    };

    $('#TypeModal').on('show.bs.modal', function (event) {
        getTypeList();
    });



    function getTypeList() {
        //очищаем выбор если был
        if ($scope.TypeGridApi.selection.getSelectedRows() != undefined && $scope.TypeGridApi.selection.getSelectedRows() != null && $scope.TypeGridApi.selection.getSelectedRows().length > 0) {
            $scope.TypeGridApi.selection.clearSelectedRows();
        }
        $scope.TypeGrid.data = [];
        $scope.TypeGrid.Options.data = [];
        $scope.TypeGrid.Options.data = null;
        $scope.TypeGrid.Options.data = $scope.LPUType;
    };

    $scope.SelectType = function () {
        if ($scope.Grid.selectedRows() === undefined || $scope.Grid.selectedRows().length < 1 || $scope.TypeGridApi.selection.getSelectedRows() === undefined || $scope.TypeGridApi.selection.getSelectedRows() === null || $scope.TypeGridApi.selection.getSelectedRows().length < 1) {
            return;
        }
        var Id = $scope.TypeGridApi.selection.getSelectedRows()[0].Id
        var selectedRows = $scope.Grid.selectedRows();
        selectedRows.forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "TypeId", Id);

        });

    };
 //Конец Блок Тип подразделения



    $scope.Dep_GridOptions = uiGridCustomService.createOptions('Dep_Grid');
    var Dep_GridOptions = {
        customEnableRowSelection: true,
        multiSelect: false,
        enableFullRowSelection: true,
        enableRowHeaderSelection: false,
        appScopeProvider: $scope,
        enableRowSelection: true,
        showGridFooter: false,
        noUnselect: true,
        columnDefs: [
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'LPUId', field: 'LPUId', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ParentId', field: 'ParentId', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ActualId', field: 'ActualId', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Название Отделения', headerCellClass: 'Yellow', field: 'Department', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'OrganizationId', field: 'OrganizationId', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'PointId', field: 'PointId', filter: { condition: uiGridCustomService.numberCondition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Коммент', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 300, name: 'ИНН юр. лица', field: 'EntityINN', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 300, name: 'ОГРН юр. лица', field: 'EntityOGRN', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 300, name: 'Юр. лицо', field: 'EntityName',  filter: { condition: uiGridCustomService.condition } },
            //{ headerTooltip: true, enableCellEdit: true, width: 300, name: 'Бренд аптеки', field: 'PharmacyBrand', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, name: 'Адрес из лицензии', width: 300, field: 'Address', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, name: 'ФО', width: 100, field: 'Bricks_FederalDistrict',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'СФ', field: 'Address_region', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, name: 'МР/ГО', width: 100, field: 'Bricks_City',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Индекс', field: 'Address_index',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'НП', field: 'Address_city',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, cellTooltip: true, width: 100, name: 'Адрес', field: 'Address_street',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Ориентир', field: 'Address_comment',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Этаж', field: 'Address_float',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: '№ пом.', field: 'Address_room',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'S пом., кв. м', field: 'Address_room_area',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, width: 100, name: 'Тип НП', field: 'Bricks_CityType',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Координаты', field: 'Address_koor',  filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Брик', field: 'BricksId',  filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Bricks?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { headerTooltip: true, enableCellEdit: false, name: 'Дата Добавления', width: 100, field: 'Date_Create', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { headerTooltip: true, enableCellEdit: false, name: 'Лицензия', width: 100, field: 'licencesNumber', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Телефон', field: 'Phone', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Web', field: 'Website', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, enableCellEdit: false, width: 100, name: 'ФИО зав. аптеки', field: 'ContactPersonFullname', filter: { condition: uiGridCustomService.condition } },
          //  { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: '100', name: 'DepartmentId', field: 'Id' },
         //   { headerTooltip: true, cellTooltip: true, enableCellEdit: false, name: 'LPUId', field: 'LPUId_Id'},
         //   { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: '500', name: 'Наименование', field: 'Name'},
         //   { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: '500', name: 'Коммент', field: 'Comments' }
        ]
    };
    $scope.Dep_GridOptions.data = [];
    angular.extend($scope.Dep_GridOptions, Dep_GridOptions);

    $scope.Dep_GridOptions.onRegisterApi = function (gridApi) {
       gridApi.selection.on.rowSelectionChanged($scope, selectDepartment);
       gridApi.edit.on.afterCellEdit($scope, editRowDepartment);
    };
   // $scope.Dep_Grid = uiGridCustomService.createGridClassMod($scope, 'Dep_Grid');
  //  $scope.Dep_Grid.Options.showGridFooter = true;
    //   $scope.Dep_Grid.Options.multiSelect = false;
    //  $scope.Dep_Grid.Options.modifierKeysToMultiSelect = true;
   // $scope.Dep_Grid.Options.rowHeight = 20;
   // $scope.Dep_Grid.Options.appScopeProvider = $scope;
   /* $scope.Dep_Grid.Options.columnDefs = [
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: '100', name: 'DepartmentId', field: 'Id', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, name: 'LPUId', field: 'LPUId_Id', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: '30%', name: 'Наименование', field: 'Name', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: '30%', name: 'Коммент', field: 'Comments', filter: { condition: uiGridCustomService.condition } },

    ];
    */
    
    function selectDepartment(row) {
        if (row.isSelected) {
            $scope.selectedDept = row.entity;            
        }
    };


    GetDepartments = function () {
        //  alert(LPUID_Val);
       // $scope.Dep_GridOptions.data = [];
        // $scope.Dep_Grid.Options.data = [];
       if (LPUID_Val == null)
            return;
        // $scope.Dep_Grid.Options.data = null;
        $http({
            method: "POST",
            url: "/LPU/Get_LPU_Department_by_LPUId/",
            data: JSON.stringify({ id: LPUID_Val }),
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                $scope.Dep_GridOptions.data = response.data.Data;
              
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }


        }, function () {
            $scope.message = "Unexpected Error";
        });
    };

 //Изменение департамента
    function editRowDepartment(rowEntity, colDef, newValue, oldValue) {
      //  str = JSON.stringify(rowEntity, null, 4); // (Optional) beautiful indented output.
      //  console.log(str);
        var json = JSON.stringify(rowEntity);
        $scope.editRowDepartmentLoading = $http({
            method: "POST",
            url: "/LPU/Edit_LPU_Department_by_LPUId/",
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

    };


    //добавляем департамент
    $scope.AddDept = function () {       
      $scope.sourceLoading = $http({
            method: "POST",
            url: "/LPU/Add_LPU_Department_by_LPUId/",
          data: JSON.stringify({ id: $scope.Grid.selectedRows()[0].LPUId, value: document.getElementById('DepartmentNameValue').value}),
        }).then(function (response) {
            // $scope.Dep_GridOptions.data = response.data.Data;
            GetDepartments();
        }, function () {
            $scope.message = "Unexpected Error";
        });
      
    };


    //Удаляем департамент
    $scope.removeDept = function () {

        if ($scope.selectedDept == null)
            return;

        var json = JSON.stringify({ id: $scope.selectedDept.LPUId });

        $scope.sourceLoading = $http({
            method: "POST",
            url: "/LPU/Remove_LPU_Department_by_LPUId/",
            data: json
        }).then(function (response) {
            //$scope.Dep_GridOptions.data = response.data.Data;
            GetDepartments();
            $scope.selectedDept = null;
        }, function () {
            $scope.message = "Unexpected Error";
        });



    };
    $('#DepModal').on('show.bs.modal', function (event) {
        GetDepartments();
    });

    /*

    $scope.LPU_Merge = function () {

        messageBoxService.showConfirm('Объединить LPU?', 'Объединение')
            .then(
                function () {
                    var array_LPUID = [];
                    var selectedRows = $scope.Grid.selectedRows();
                    if (selectedRows.length >= 5 || selectedRows.length<2) {
                        alert("Выделено больше 4 строк или меньше 2х, я так не буду объединять.");
                        return;
                    }
                    selectedRows.forEach(function (item) {
                        array_LPUID.push(item.LPUId);
                    });
                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/LPU/Merge/',
                            data: JSON.stringify({ LPUIds: array_LPUID })
                        }).then(function (response) {
                            var data = response.data;
                            $scope.Search();
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });

                },
                function (result) {

                }
            );
    };
    */

   


    $scope.LPUToMergeGrid = uiGridCustomService.createGridClassMod($scope, 'LPUToMergeGrid');
 
    $scope.LPUToMergeGrid.Options.multiSelect = false;
    $scope.LPUToMergeGrid.Options.noUnselect = false;
    $scope.LPUToMergeGrid.Options.showGridFooter = true;
    $scope.LPUToMergeGrid.Options.columnDefs = [
        { name: 'LPUId', field: 'LPUId', type: 'number' },
        { name: 'Отделения', field: 'DepartmentCnt', type: 'number' },
        { name: 'OrganizationId', field: 'OrganizationId', type: 'number' },
        { name: 'PointId', field: 'PointId', type: 'number' },
        { name: 'ИНН юр. лица', field: 'EntityINN' },
        { name: 'ОГРН юр. лица', field: 'EntityOGRN' },
        { name: 'Юр. лицо', field: 'EntityName' },
        { name: 'Адрес из лицензии', field: 'Address' },
        { name: 'Брик', field: 'BricksId' },
        { name: 'Дата Добавления', field: 'Date_Create' },
    ];
    $scope.LPUToMergeGrid.data = [];
   
    $scope.LPUToMergeGrid.Options.onRegisterApi = function (gridApi) {
        $scope.LPUToMergeGridApi = gridApi;
   };
   
    $('#MergeModal').on('show.bs.modal', function (event) {        
        getMergeList();
    });

 
    
    function getMergeList() {
        //очищаем выбор если был
        if ($scope.LPUToMergeGridApi.selection.getSelectedRows() != undefined && $scope.LPUToMergeGridApi.selection.getSelectedRows() != null && $scope.LPUToMergeGridApi.selection.getSelectedRows().length > 0) {
            $scope.LPUToMergeGridApi.selection.clearSelectedRows();       
        }
        $scope.LPUToMergeGrid.data = [];
        $scope.LPUToMergeGrid.Options.data = [];
        $scope.LPUToMergeGrid.Options.data = null;
        $scope.LPUToMergeGrid.Options.data = $scope.Grid.selectedRows();     
    };



    $scope.SelectActualMerge = function () {
        if ($scope.Grid.selectedRows() === undefined || $scope.Grid.selectedRows().length < 2 || $scope.LPUToMergeGridApi.selection.getSelectedRows() === undefined || $scope.LPUToMergeGridApi.selection.getSelectedRows() === null || $scope.LPUToMergeGridApi.selection.getSelectedRows().length < 1) {
            return;
        }
       // alert($scope.Grid.selectedRows()[0].LPUId);
       // alert($scope.LPUToMergeGridApi.selection.getSelectedRows()[0].LPUId);


        var array_LPUID = [];
        var selectedRows = $scope.Grid.selectedRows();
       
        selectedRows.forEach(function (item) {
            array_LPUID.push(item.LPUId);
        });
        var Actual_Id = $scope.LPUToMergeGridApi.selection.getSelectedRows()[0].LPUId

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/LPU/Merge/',
                data: JSON.stringify({ LPUIds: array_LPUID, Actual_Id: Actual_Id})
            }).then(function (response) {
              
                $scope.Search();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });






    };



  

}