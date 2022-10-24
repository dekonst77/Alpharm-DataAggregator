angular
    .module('DataAggregatorModule')
    .controller('ProjectController', [
        '$scope', '$route', '$http', '$uibModal','$interval', 'commonService', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', ProjectController]);

function ProjectController($scope, $route, $http, $uibModal, $interval, commonService, messageBoxService, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.user = userService.getUser();
    $scope.currentProject = undefined;
    $scope.currentStep = undefined;
    $scope.InitGrid = function (Grid) {
        Grid.Options.showGridFooter = true;
        Grid.Options.multiSelect = true;
        Grid.Options.modifierKeysToMultiSelect = true;
        Grid.Options.rowTemplate = '<div ng-class="{\'modify\' : row.entity[\'@modify\']==true}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';
    };
    $scope.ToDate = function (value) {
        var ret = new Date(value);
        ret = new Date(ret.getTime() - 60000 * ret.getTimezoneOffset());
        ret.setHours(0);
        ret.setMinutes(0);
        return ret;
    };
    $scope.ToDateColumn = function (data, Name) {
        for (i = 0; i < data.length; i++) {
            if (data[i][Name] !== null)
                data[i][Name] = $scope.ToDate(data[i][Name]);
        }
    };
    ///////////////////////////////ГС Старт
    $scope.Project_Init = function () {
        $scope.Project_NeedSave = false;
        $scope.Project_MayEdit = true;
        $scope.Step_NeedSave = false;
        $scope.Project_IsRowSelection = false;
        $scope.Step_IsRowSelection = false;

        $scope.ProjectId = Number($route.current.params["projectid"]);
        $scope.Project = [];
        $scope.ProjectType = [];
        $scope.Users = [];
        $scope.StepStatus = [];
        $scope.StepTemplate = [];


        $scope.format = 'dd.MM.yyyy';
        $scope.currentperiod = null;

        $scope.Grid_Project = uiGridCustomService.createGridClass($scope, 'Grid_Project');
        $scope.Grid_Step = uiGridCustomService.createGridClass($scope, 'Grid_Step');
        $scope.Grid_History = uiGridCustomService.createGridClass($scope, 'Grid_History');
        $scope.InitGrid($scope.Grid_Project);
        $scope.InitGrid($scope.Grid_Step);
        $scope.InitGrid($scope.Grid_History);

        $scope.Grid_Project.Options.enableFiltering = false;
        $scope.Grid_Project.Options.enableSorting = false;

        $scope.Grid_Step.Options.enableFiltering = false;
        $scope.Grid_Step.Options.enableSorting = false;
        
        $scope.Grid_Project.Options.onRegisterApi = function (gridApi) {
            $scope.Grid_Project.IngridAPI = gridApi;

            $scope.Grid_Project.IngridAPI.selection.on.rowSelectionChanged($scope, Grid_Project_select);

            $scope.Grid_Project.IngridAPI.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.Project_NeedSave = true;
                    }
                }
            });
        };
        $scope.Grid_Project.Options.cellEditableCondition = function ($scope) {
            return IsEditProject($scope.row.entity);
        };
        //$scope.Grid_Project.Options.enableCellEdit = false;
        //$scope.Grid_Project.Options.enableCellEditOnFocus = true;
        $scope.Grid_Step.Options.onRegisterApi = function (gridApi) {
            $scope.Grid_Step.IngridAPI = gridApi;

            $scope.Grid_Step.IngridAPI.selection.on.rowSelectionChanged($scope, Grid_Step_select);

            $scope.Grid_Step.IngridAPI.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.Step_NeedSave = true;
                    }
                }
            });
        };
        $scope.Grid_Step.Options.cellEditableCondition = function ($scope) {
            return IsEditStep($scope.row.entity);
         };
        $scope.Grid_Project.Options.columnDefs = [
            { enableCellEdit: true,name: 'Период', field: 'PeriodYM', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true,name: 'Имя', field: 'Name', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true,  name: 'Тип', field: 'TypeId', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                editDropdownOptionsArray: $scope.ProjectType,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'},
            {
                enableCellEdit: true, name: 'Ответственный', field: 'ProjectManagerId', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                editDropdownOptionsArray: $scope.Users,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'FullName' }
        ];

        $scope.Grid_Step.Options.columnDefs = [
            //{ name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.FILTER_INT_COUNT }, type: 'number' },
           /* {
                name: 'Порядок', field: 'orderby', filter: { condition: uiGridCustomService.FILTER_INT_COUNT }, type: 'number',
                cellTemplate: '<div class="btn-group"><button type="button" class="btn btn-info" ng-click="grid.appScope.StepOrderBy(row.entity,-1)"><span class="glyphicon glyphicon-arrow-up"></span></button>  <button type="button" class="btn btn-info" ng-click="grid.appScope.StepOrderBy(row.entity,1)"><span class="glyphicon glyphicon-arrow-down"></span></button></div>'
            },*/
            {
                enableCellEdit: true,
                name: 'Тип шага', field: 'StepTemplateId', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                editDropdownOptionsArray: $scope.StepTemplate,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name' },

            {
                enableCellEdit: true, name: 'Начало', field: 'DateBeginPlan',
                filter: { condition: uiGridCustomService.condition },
                type: 'date',
                cellFilter: formatConstants.FILTER_DATE
                //editableCellTemplate: '<input ng-class="" datepicker-popup is-open="true" ng-model="MODEL_COL_FIELD" />'
                //editableCellTemplate: '<input datetime2 ng-model="MODEL_COL_FIELD" readonly>'
            },
            { enableCellEdit: true, name: 'Час Начала', field: 'DateBeginPlanHH', type: 'number', filter: { condition: uiGridCustomService.FILTER_INT_COUNT } },
            { enableCellEdit: true, name: 'Дней', field: 'DateDay', type: 'number', filter: { condition: uiGridCustomService.FILTER_INT_COUNT } },
            { enableCellEdit: true, name: 'Окончание', field: 'DateEndPlan', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { enableCellEdit: true, name: 'Час Окончания', field: 'DateEndPlanHH', type: 'number', filter: { condition: uiGridCustomService.FILTER_INT_COUNT } },
            { enableCellEdit: false, name: 'Начат', field: 'DateBeginReal', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME },
            { enableCellEdit: false, name: 'Завершён', field: 'DateEndReal', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME },
            {
                enableCellEdit: true, name: 'Ответственный', field: 'StepManagerId', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                editDropdownOptionsArray: $scope.Users,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'FullName'
             },
            {
                enableCellEdit: true, name: 'Статус', field: 'StepStatusId', filter: { condition: uiGridCustomService.condition },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                editDropdownOptionsArray: $scope.StepStatus,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value' }
        ];

        $scope.Grid_History.Options.columnDefs = [
            { name: 'Когда', field: 'DT', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME },
            { enableCellEdit: true, name: 'Где/Кто', field: 'Value1', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { enableCellEdit: true, name: 'Что', field: 'Value2', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint }
        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Project/ProjectList/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.ProjectType, response.data.Data.ProjectType);
            Array.prototype.push.apply($scope.StepStatus, response.data.Data.StepStatus);
            Array.prototype.push.apply($scope.StepTemplate, response.data.Data.StepTemplate);
            Array.prototype.push.apply($scope.Users, response.data.Data.Users);

            $scope.Grid_Project.Options.data = response.data.Data.Project;

            if ($scope.ProjectId > 0) {
                $scope.Grid_Project.Options.data.forEach(function (item, i, arr) {
                    if (item.Id === $scope.ProjectId) {

                        

                        $interval(function () { $scope.Grid_Project.IngridAPI.selection.selectRow(item); }, 0, 1);
                       // item.setSelected(true);
                    }
                });
            }
            return response.data;
        });
    };
    function Grid_Project_select(row) {
        var selectedRows = $scope.Grid_Project.IngridAPI.selection.getSelectedRows();
        if (selectedRows.length >= 1) {
            $scope.Project_IsRowSelection = true;
            $scope.currentProject = selectedRows[0];
            $scope.GetProject($scope.currentProject.Id);
        }
        else {
            $scope.currentProject = undefined;
            $scope.Project_IsRowSelection = false;
        }
    }

    function Grid_Step_select(row) {
        var selectedRows = $scope.Grid_Step.IngridAPI.selection.getSelectedRows();
        if (selectedRows.length >= 1) {
            $scope.Step_IsRowSelection = true;
            $scope.currentStep = selectedRows[0];
        }
        else {
            $scope.Step_IsRowSelection = false;
            $scope.currentStep = undefined;
        }
    }

    $scope.GetProject = function (ProjectId) {
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Project/Project/',
            data: JSON.stringify({ ProjectId:ProjectId})
        }).then(function (response) {
            $scope.ToDateColumn(response.data.Data.Step, "DateBeginPlan");
            $scope.ToDateColumn(response.data.Data.Step, "DateEndPlan");
            $scope.Grid_Step.Options.data = response.data.Data.Step;
            $scope.Grid_History.Options.data = response.data.Data.History;
            $scope.Grid_Step.IngridAPI.grid.sortColumn($scope.Grid_Step.IngridAPI.grid.getColumn("Порядок"), uiGridConstants.ASC);
            //$scope.Grid_Step.IngridAPI.grid.sortColumn($scope.Grid_Step.IngridAPI.grid.getColumn("DateBeginPlan"), uiGridConstants.ASC);
        });
    };
    $scope.ProjectNew = function (isClone) {

        var Id = 0;
        if (isClone) {
            var selectedRows = $scope.Grid_Project.IngridAPI.selection.getSelectedRows();
            Id = selectedRows.Id;
        }
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Project/ProjectNew/',
                data: JSON.stringify({ ProjectIdParent: Id })
            }).then(function (response) {
                if (response.data.Data.Success) {
                    var rowEntity = response.data.Data.NewRow;
                    $scope.NeedSave = true;
                    rowEntity["@modify"] = true;
                    $scope.Grid_Project.Options.data.push(rowEntity);
                    $scope.Grid_Project.IngridAPI.core.clearAllFilters();//++
                    $scope.Grid_Project.IngridAPI.grid.getColumn('Id').filters[0] = {
                        term: rowEntity.Id
                    };
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.ProjectSave = function (action) {
        var array_upd = [];
        $scope.Grid_Project.Options.data.forEach(function (item, i, arr) {
            if (item["@modify"] === true) {
                array_upd.push(item);
            }
        });
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Project/ProjectSave/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    if (response.data.Data.Success) {
                        if (action === "search") {
                            //$scope.Project_search_AC();
                        }
                        else {
                            $scope.Project_NeedSave = false;
                            $scope.Grid_Project.Options.data.forEach(function (item, i, arr) {
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
    function IsEditProject(row) {
        var ret = false;

        if (row !== undefined) {
            if (row.ProjectManagerId === $scope.user.Id)
                ret = true;
        }
        return ret;
    }
    $scope.IsEditProject = function (row) {       
        return IsEditProject(row);
    };

    $scope.StepNew = function (isClone) {
        if ($scope.currentProject.Id > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Project/StepNew/',
                    data: JSON.stringify({ ProjectId: $scope.currentProject.Id })
                }).then(function (response) {
                    if (response.data.Data.Success) {
                        $scope.GetProject($scope.currentProject.Id);
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.StepSave = function (action) {
        var array_upd = [];
        $scope.Grid_Step.Options.data.forEach(function (item, i, arr) {
            if (item["@modify"] === true) {
                array_upd.push(item);
            }
        });
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Project/StepSave/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    if (response.data.Data.Success) {
                        if (action === "search") {
                            //$scope.Project_search_AC();
                        }
                        else {
                            $scope.Step_NeedSave = false;
                            $scope.Grid_Step.Options.data.forEach(function (item, i, arr) {
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
    $scope.StepDelete = function () {
        var array_upd = [];
        var selectedRows = $scope.Grid_Step.IngridAPI.selection.getSelectedRows().forEach(function (item, i, arr) {
                array_upd.push(item);
        });
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Project/StepDelete/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    if (response.data.Data.Success) {
                        $scope.GetProject($scope.currentProject.Id);
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };
    $scope.StepOrderBy = function (row, napr) {
        if (row.orderby + napr === 0)
            return;
        if (row.orderby + napr > $scope.Grid_Step.Options.data.length)
            return;


        $scope.Step_NeedSave = true;
        row.orderby = row.orderby + napr;
        row["@modify"] = true;

        $scope.Grid_Step.Options.data.forEach(function (item, i, arr) {
            if (item.Id !== row.Id && item.orderby === row.orderby) {
                item.orderby = item.orderby - napr;
                item["@modify"] = true;
            }
        });
        $scope.Grid_Step.IngridAPI.grid.sortColumn($scope.Grid_Step.IngridAPI.grid.getColumn("Порядок"), uiGridConstants.ASC);
        $scope.Grid_Step.IngridAPI.grid.refresh();
    };

    function IsEditStep(row) {
        var ret = false;

        if (row !== undefined) {
            if (row.StepManagerId === $scope.user.Id)
                ret = true;
        }
        if ($scope.currentProject !== undefined) {
            if ($scope.currentProject.ProjectManagerId === $scope.user.Id)
                ret = true;
        }
        
        return ret;
    }
    $scope.IsEditStep = function (row) {
        return IsEditStep(row);
    };
 
}