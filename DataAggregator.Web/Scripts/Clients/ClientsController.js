angular
    .module('DataAggregatorModule')
    .controller('ClientsController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', ClientsController]);

function ClientsController($scope, $route, $http, $uibModal, commonService, messageBoxService, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
 //компании--------------------------------------------------------
    $scope.CompaniesInit = function () {
        $scope.currentCompany = undefined;
        $scope.filter_Companies = {
            common: "",
            id:""
        };
        $scope.HasNew = false;
        $scope.Grid_Companies = uiGridCustomService.createGridClass($scope, 'Grid_Companies');
        $scope.Grid_Companies.Options.showGridFooter = true;
        $scope.Grid_Companies.Options.multiSelect = true;
        $scope.Grid_Companies.Options.modifierKeysToMultiSelect = true;
        $scope.Grid_Companies.Options.noUnselect = false;
        $scope.Grid_Companies.Options.rowTemplate = '<div ng-class="{\'modify\' : row.entity[\'@modify\']==true}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';

        $scope.Grid_Companies.Options.onRegisterApi = function (gridApi) {//если строка меняется тоей ставиться флажок modify
            $scope.gridApi_Grid_Companies = gridApi;
            $scope.gridApi_Grid_Companies.selection.on.rowSelectionChanged($scope, Grid_Companies_select);
            $scope.gridApi_Grid_Companies.selection.on.rowSelectionChangedBatch($scope, Grid_Companies_select);

            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.NeedSave = true;
                    }
                }
            });

        };
        $scope.WorkersInit();
        return $http({
            method: 'POST',
            url: '/Clients/CompaniesInit/',
            data: JSON.stringify({})
        }).then(function (response) {
            $scope.Grid_Companies.Options.columnDefs = [
                { name: 'Id', width: 100, field: 'Id' },
                { enableCellEdit: true, width: 400, name: 'Название', field: 'Value', filter: { condition: uiGridCustomService.condition } }
            ];
            var id = $route.current.params["id"];
            if (id !== undefined) {
                $scope.filter_Companies.id = id;
                $scope.filter_Companies.common = "";               
            }            
            
            $scope.Companies_search_AC();
            return response.data;
        });
    };
    $scope.Companies_search_AC = function () {
        $scope.NeedSave = false;
        $scope.currentCompany = undefined;
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Clients/CompaniesGet/',
                data: JSON.stringify({ filter: $scope.filter_Companies })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.HasNew = false;
                    $scope.Grid_Companies.Options.data = data.Data;

                    if ($scope.Grid_Companies.Options.data.length > 0) {
                        $scope.gridApi_Grid_Companies.selection.selectRow($scope.Grid_Companies.Options.data[0]);
                        Grid_Companies_select($scope.Grid_Companies.Options.data[0]);
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Companies_search = function () {
        $scope.filter_Companies.id = "";
        if ($scope.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Companies_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Companies_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Companies_search_AC();
        }
    };
    $scope.Companies_New = function () {
        $scope.HasNew = true;
        var Element_new = {
            Id: 0,
            Value: "новый"
        };
        Element_new["@modify"] = true;
        $scope.Grid_Companies.Options.data.push(Element_new);

    };
    $scope.Companies_save = function (action) {
        var array_upd = [];
        $scope.Grid_Companies.Options.data.forEach(function (item, i, arr) {
            if (item["@modify"] === true) {
                array_upd.push(item);
            }
        });
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Clients/Companies_Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.Companies_search_AC();
                        }
                        else {
                            $scope.NeedSave = false;
                            $scope.Grid_Companies.Options.data.forEach(function (item, i, arr) {
                                if (item["@modify"] === true) {
                                    item["@modify"] = false;
                                }
                            });                            
                            alert("Сохранил");
                            if ($scope.HasNew === true)
                                $scope.Companies_search_AC();
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    function Grid_Companies_select(row) {
            var selectedRows = $scope.gridApi_Grid_Companies.selection.getSelectedRows();
            if (selectedRows.length >= 1) {
                $scope.Grid_Companies_IsRowSelection = true;
                $scope.currentCompany = selectedRows[0];
                $scope.Workers_search();
            }
            else {
                $scope.Grid_Companies_IsRowSelection = false;
            }
        }
 //сотрудники--------------------------------------------------------
    $scope.WorkersInit = function () {
        $scope.NeedSave_Worker = false;
        $scope.Grid_Workers = uiGridCustomService.createGridClass($scope, 'Grid_Workers');
        $scope.Grid_Workers.Options.showGridFooter = true;
        $scope.Grid_Workers.Options.multiSelect = true;
        $scope.Grid_Workers.Options.modifierKeysToMultiSelect = true;
        $scope.Grid_Workers.Options.noUnselect = false;
        $scope.Grid_Workers.Options.rowTemplate = '<div ng-class="{\'modify\' : row.entity[\'@modify\']==true}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';

        $scope.Grid_Workers.Options.onRegisterApi = function (gridApi) {//если строка меняется тоей ставиться флажок modify
            $scope.gridApi_Grid_Workers = gridApi;
            //$scope.gridApi_Grid_Workers.selection.on.rowSelectionChanged($scope, Grid_Workers_select);
            //$scope.gridApi_Grid_Workers.selection.on.rowSelectionChangedBatch($scope, Grid_Workers_select);

            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.NeedSave_Worker = true;
                    }
                }
            });
        };
        $scope.Grid_Workers.Options.columnDefs = [
                { name: 'Id', width: 100, field: 'Id' },
            { enableCellEdit: true, width: 300, name: 'Email', field: 'Email', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true, width: 300, name: 'Name', field: 'Name', filter: { condition: uiGridCustomService.condition } },
            {
                width: 100, name: 'Отчёты', field: 'calc1', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="Отчёты"><a href="/#/Clients/Reports?w_id={{row.entity.Id}}&c_id={{row.entity.CompanyId}}" target="_blank">Отчёты</a></div>'
            }
        ];

    };
    $scope.Workers_search_AC = function () {
        $scope.NeedSave_Worker = false;
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Clients/WorkerGet/',
                data: JSON.stringify({ CompanyId: $scope.currentCompany.Id })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.HasNew = false;
                    $scope.Grid_Workers.Options.data = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Workers_search = function () {
        if ($scope.NeedSave_Worker === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Workers_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Workers_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Workers_search_AC();
        }
    };
    $scope.Workers_New = function () {
        if ($scope.currentCompany !== undefined && $scope.currentCompany.Id>0)
        $scope.HasNew = true;
        var Element_new = {
            Id: 0,
            Email: "",
            CompanyId: $scope.currentCompany.Id,
            Name: ""
        };
        Element_new["@modify"] = true;
        $scope.Grid_Workers.Options.data.push(Element_new);
    };
    $scope.Workers_save = function (action) {
        var array_upd = [];
        $scope.Grid_Workers.Options.data.forEach(function (item, i, arr) {
            if (item["@modify"] === true) {
                array_upd.push(item);
            }
        });
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Clients/Worker_Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.Workers_search_AC();
                        }
                        else {
                            $scope.NeedSave_Worker = false;
                            $scope.Grid_Workers.Options.data.forEach(function (item, i, arr) {
                                if (item["@modify"] === true) {
                                    item["@modify"] = false;
                                }
                            });
                            alert("Сохранил");
                            if ($scope.HasNew === true)
                                $scope.Workers_search_AC();
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
 //отчёты--------------------------------------------------------
    $scope.ReportInit = function () {
        $scope.currentReport = undefined;
        $scope.list_Workers = [];
        $scope.list_INN = [];
        $scope.list_Regions = [];
        $scope.list_TN = [];
        $scope.list_Rep_Type = [];
        $scope.HasNew = false;
        $scope.Grid_Report = uiGridCustomService.createGridClass($scope, 'Grid_Report');
        $scope.Grid_Report.Options.showGridFooter = true;
        $scope.Grid_Report.Options.multiSelect = true;
        $scope.Grid_Report.Options.modifierKeysToMultiSelect = true;
        $scope.Grid_Report.Options.noUnselect = false;
        $scope.Grid_Report.Options.rowTemplate = '<div ng-class="{\'modify\' : row.entity[\'@modify\']==true}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';

        var id = $route.current.params["w_id"];
        if (id !== undefined) {
            $scope.filter_Report_w_id = id;
        }
        id = $route.current.params["c_id"];
        if (id !== undefined) {
            $scope.filter_Report_c_ip = id;
        }

        $scope.Grid_Report.Options.onRegisterApi = function (gridApi) {//если строка меняется тоей ставиться флажок modify
            $scope.gridApi_Grid_Report = gridApi;
            $scope.gridApi_Grid_Report.selection.on.rowSelectionChanged($scope, Grid_Report_select);
            $scope.gridApi_Grid_Report.selection.on.rowSelectionChangedBatch($scope, Grid_Report_select);

            gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.NeedSave_Report = true;
                    }
                }
            });

        };
        return $http({
            method: 'POST',
            url: '/Clients/ReportInit/',
            data: JSON.stringify({ c_id: $scope.filter_Report_c_ip, w_id: $scope.filter_Report_w_id })
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                $scope.list_Workers = data.Workers;
                $scope.list_INN = data.INN;
                $scope.list_Regions = data.Regions;
                $scope.list_TN = data.TN;
                $scope.list_Rep_Type = data.Rep_Type;



                $scope.Grid_Report.Options.columnDefs = [
                    { name: 'Id', width: 100, field: 'Id' },
                    {
                        enableCellEdit: true, width: 100, name: 'Тип', field: 'Rep_TypeId', filter: { condition: uiGridCustomService.condition },
                        editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                        editDropdownOptionsArray: $scope.list_Rep_Type,
                        editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value' },
                    {
                        enableCellEdit: true, width: 100, name: 'Сотрудник', field: 'WorkerId', filter: { condition: uiGridCustomService.condition },
                        editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                        editDropdownOptionsArray: $scope.list_Workers,
                        editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'},
                    { enableCellEdit: true, width: 400, name: 'Название', field: 'Name', filter: { condition: uiGridCustomService.condition } },
                    {
                        enableCellEdit: true, width: 100, name: 'Периодичность', field: 'Period', filter: { condition: uiGridCustomService.condition },
                        cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Period_Show(row.entity)" class=""></span>{{row.entity.Period}}</button>'
                    },
                    { enableCellEdit: true, width: 100, name: 'Активен', field: 'IsActive', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },
                    {
                        enableCellEdit: true, width: 400, name: 'Фильтр', field: 'Param_word', filter: { condition: uiGridCustomService.condition },
                        cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Text_Show(row.entity,\'Param_word\')" class=""></span>{{row.entity.Param_word}}</button>' },
                    {
                        enableCellEdit: true, width: 400, name: 'МНН', field: 'Param_INN', filter: { condition: uiGridCustomService.condition },
                        cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Text_Show(row.entity,\'Param_INN\')" class=""></span>{{row.entity.Param_INN}}</button>' },
                    {
                        enableCellEdit: true, width: 400, name: 'ТН', field: 'Param_TN', filter: { condition: uiGridCustomService.condition },
                        cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Text_Show(row.entity,\'Param_TN\')" class=""></span>{{row.entity.Param_TN}}</button>' },
                    {
                        enableCellEdit: true, width: 400, name: 'ATCEphmra', field: 'Param_ATCEphmra', filter: { condition: uiGridCustomService.condition },
                        cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Text_Show(row.entity,\'Param_ATCEphmra\')" class=""></span>{{row.entity.Param_ATCEphmra}}</button>' },
                    {
                        enableCellEdit: true, width: 400, name: 'ИНН Заказчика', field: 'Param_Customer_INN', filter: { condition: uiGridCustomService.condition },
                        cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Text_Show(row.entity,\'Param_Customer_INN\')" class=""></span>{{row.entity.Param_Customer_INN}}</button>' },
                    {
                        enableCellEdit: true, width: 400, name: 'Регион Заказчика', field: 'Param_Region_Customer', filter: { condition: uiGridCustomService.condition },
                        cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Text_Show(row.entity,\'Param_Region_Customer\')" class=""></span>{{row.entity.Param_Region_Customer}}</button>'
                    },                   {
                        enableCellEdit: true, width: 400, name: 'Регион получателя', field: 'Param_Region_Receiver', filter: { condition: uiGridCustomService.condition },
                        cellTemplate: '<button ng-class="" style="width:100%" class="btn btn-sm" ng-click="grid.appScope.Text_Show(row.entity,\'Param_Region_Receiver\')" class=""></span>{{row.entity.Param_Region_Receiver}}</button>' },
                    { width: 100, name: 'Отправлен', field: 'LastSend', filter: { condition: uiGridCustomService.condition } },
                    { width: 100, name: 'Создан', field: 'Create', filter: { condition: uiGridCustomService.condition } }
                ];

                $scope.Report_search_AC();
            }
            return response.data;
        });
    };
    $scope.Report_search_AC = function () {
        $scope.NeedSave_Report = false;
        $scope.currentReport = undefined;
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Clients/ReportsGet/',
                data: JSON.stringify({ c_id: $scope.filter_Report_c_ip, w_id: $scope.filter_Report_w_id })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.HasNew = false;
                    $scope.Grid_Report.Options.data = data.Data;

                    if ($scope.Grid_Report.Options.data.length > 0) {
                        Grid_Report_select($scope.Grid_Report.Options.data[0]);
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Report_search = function () {
        if ($scope.NeedSave_Report === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Report_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Report_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Report_search_AC();
        }
    };
    $scope.Report_New = function () {
        $scope.HasNew = true;
        var Element_new = {
            Id: 0,
            Rep_TypeId: $scope.list_Rep_Type[0].Id,
            WorkerId: $scope.filter_Report_w_id,
            Name: "",
            Period: "",
            IsActive: true,
            Param_word: "",
            Param_INN: "",
            Param_ATCEphmra: "",
            Param_Region_Customer: "",
            Param_Region_Receiver: "",
            Param_Customer_INN: "",
            Param_TN: ""
        };
        Element_new["@modify"] = true;
        $scope.Grid_Report.Options.data.push(Element_new);
        $scope.NeedSave_Report = true;
    };

    $scope.Report_save = function (action) {
        var array_upd = [];
        $scope.Grid_Report.Options.data.forEach(function (item, i, arr) {
            if (item["@modify"] === true) {
                array_upd.push(item);
            }
        });
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Clients/Report_Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.Report_search_AC();
                        }
                        else {
                            $scope.NeedSave_Report = false;
                            $scope.Grid_Report.Options.data.forEach(function (item, i, arr) {
                                if (item["@modify"] === true) {
                                    item["@modify"] = false;
                                }
                            });
                            alert("Сохранил");
                            if ($scope.HasNew === true)
                                $scope.Report_search_AC();
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    function Grid_Report_select(row) {
        var selectedRows = $scope.gridApi_Grid_Report.selection.getSelectedRows();
        if (selectedRows.length >= 1) {
            $scope.Grid_Report_IsRowSelection = true;
            $scope.currentReport = selectedRows[0];
        }
        else {
            $scope.Grid_Report_IsRowSelection = false;
        }
    }


    $scope.rep_param_cur = undefined;
    $scope.week = [false, false, false, false, false, false, false];
    $scope.weekP = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];

    $scope.month = [false, false, false, false, false, false, false];
    $scope.monthP = ['[1]', '[2]', '[3]', '[10]', '[15]', '[20]', '[последний]'];

    $scope.Period_Show  = function (Value) {       
        $scope.rep_param_cur = Value;
        $scope.m_value = $scope.rep_param_cur.Period;

        for (i = 0; i < $scope.week.length; i++) {
            if ($scope.m_value.indexOf($scope.weekP[i])>=0)
                $scope.week[i] = true;
            else
                $scope.week[i] = false;
        }

        for (i = 0; i < $scope.month.length; i++) {
            if ($scope.m_value.indexOf($scope.monthP[i]) >= 0)
                $scope.month[i] = true;
            else
                $scope.month[i] = false;
        }

        $('#modal_Period').modal('show');
    };
    $scope.modal_Period_Save = function () {
        $scope.m_value = "";
        for (i = 0; i < $scope.week.length; i++) {
            if ($scope.week[i] === true)
                $scope.m_value += $scope.weekP[i];
        }
        for (i = 0; i < $scope.month.length; i++) {
            if ($scope.month[i] === true)
                $scope.m_value += $scope.monthP[i];
        }
        $scope.rep_param_cur.Period = $scope.m_value;
        $scope.rep_param_cur["@modify"] = true;
        $scope.NeedSave_Report = true;
        $('#modal_Period').modal('hide');
    };

    $scope.Text_Show= function (Value,param) {
        $scope.rep_param_cur = Value;
        $scope.rep_param_cur_param = param;
        $scope.m_value = $scope.rep_param_cur[$scope.rep_param_cur_param];

        $('#modal_Text').modal('show');
    };
    $scope.modal_Text_Save = function () {
        //$scope.m_value = $scope.m_value.replace(new RegExp("\\r?\\n", "g"), "\x01");


        $scope.rep_param_cur[$scope.rep_param_cur_param] = $scope.m_value;

        $scope.rep_param_cur["@modify"] = true;
        $scope.NeedSave_Report = true;
        $('#modal_Text').modal('hide');
    };
}