﻿angular
    .module('DataAggregatorModule')
    .controller('OFDController', [
        '$scope', '$route', '$http', '$uibModal', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'commonService', 'Upload', OFDController]);

function OFDController($scope, $route, $http, $uibModal, messageBoxService, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, commonService, Upload) {
    $scope.format = 'dd.MM.yyyy';
    $scope.log_dt = new dateClass();
    $scope.files_dt = new dateClass();
    // $scope.Title = "ОФД";
    $scope.OFD_job_ftp_upload_info = "";

    $scope.Log_show = function () {

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/Log/',
                data: JSON.stringify({ dts: $scope.log_dt.Value })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_log.SetData(data.Data);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Init_logs = function () {
        $scope.Grid_log = uiGridCustomService.createGridClassMod($scope, "Grid_log");
        $scope.Grid_log.Options.columnDefs = [
            //cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_P}}" target="_blank">ссылка</a></div>'
            //type: 'date'
            { name: 'Id', field: 'Id', type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'параметр1', field: 'param_s', filter: { condition: uiGridCustomService.condition } },
            { name: 'параметр2', field: 'param_i', filter: { condition: uiGridCustomService.condition } },
            { name: 'дата/время', field: 'dt', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME }

        ];

        $scope.dataLoading =
            $http({
                method: 'GET',
                url: '/OFD/Log/',
                data: JSON.stringify({})
            }).then(function (response) {
                var ViewBag = response.data;
                $scope.log_dt.Value = new Date(ViewBag.log_dt);
                $scope.Log_show();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Init_Action = function () {

    };
    $scope.Init_Files = function () {
        $scope.Grid_Files = uiGridCustomService.createGridClassMod($scope, "Grid_Files");
        $scope.Grid_Files.Options.columnDefs = [
            //cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_P}}" target="_blank">ссылка</a></div>'
            //type: 'date'
            { name: 'Id', field: 'Id' },
            { name: 'Имя файла', field: 'value', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { name: 'Загружен', field: 'dt_load', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME },
            { name: 'Период', field: 'period', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME },
            { name: 'Тип', field: 'ActionName', filter: { condition: uiGridCustomService.condition } },
            { name: 'Направление', field: 'SupplierNameNapr', filter: { condition: uiGridCustomService.condition } },
            {
                name: 'Удаление', field: 'value',
                cellTemplate: '<button class="btn btn-warning btn-sm" ng-click="grid.appScope.Files_Action(row, col,false)"><span class="glyphicon glyphicon-erase">Данные</span></button><button class="btn btn-danger btn-sm" ng-click="grid.appScope.Files_Action(row, col,true)"><span class="glyphicon glyphicon-fire">Файл</span></button>'
            }
        ];

        $scope.dataLoading =
            $http({
                method: 'GET',
                url: '/OFD/Files/',
                data: JSON.stringify({})
            }).then(function (response) {
                var ViewBag = response.data;
                $scope.files_dt.Value = new Date(ViewBag.files_dt);
                $scope.Files_show();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Files_show = function () {

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/Files/',
                data: JSON.stringify({ dts: $scope.files_dt.Value })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_Files.SetData(data.Data);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Files_Action = function (row, col, withFile) {
        var Filename = row.entity[col.field];
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/FilesAction/',
                data: JSON.stringify({ Filename: Filename, withFile: withFile })
            }).then(function (response) {

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Reports = [];
    $scope.report_text = "";
    $scope.period_start = new Date(new Date().getFullYear(), 0, 1);
    $scope.period_end = new Date();
    $scope.report_supplier = { Id: 0 };
    $scope.brick_3_Id = { Id: "%" };
    $scope.brick_3 = [];
    $scope.Init_Report = function () {
        $scope.SupplierGet(true);
        $scope.dataLoading =
            $http({
                method: 'GET',
                url: '/OFD/Report/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Reports = data.Data;
                    $scope.brick_3 = data.Data2;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.ReportGet = function (Id) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/Report/',
                data: JSON.stringify({ Id: Id, supplierID: $scope.report_supplier.Id, text: $scope.report_text, period_start: $scope.period_start, period_end: $scope.period_end, Brick_L3: $scope.brick_3_Id.Id })
            }).then(function (response) {
                alert('Ожидайте отчёт по почте');
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.SupplierList = [];
    $scope.SupplierGet = function (withDef) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/Suppliers/',
                data: JSON.stringify({ withDef: withDef })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.SupplierList = data.Data;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };



    $scope.Init_Periods = function () {
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, "Grid");

        $scope.Grid.Options.columnDefs = [
            //cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_P}}" target="_blank">ссылка</a></div>'
            //type: 'date'
            { name: 'Id', field: 'Id', type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'Поставщик', field: 'SupplierId', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'Период', field: 'period', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_YMD },
            {
                name: 'Активный', field: 'period_type', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==30}" ng-click="grid.appScope.Periods_type_Set(30)">Кв</button><button type="button" class="btn" ng-class="{\'btn-success\' :row.entity.period_type==20}" ng-click="grid.appScope.Periods_type_Set(20)">Мс</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==10}" ng-click="grid.appScope.Periods_type_Set(10)">Дн</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==0}" ng-click="grid.appScope.Periods_type_Set(0)">Блок</button></div>'
            },
            { name: 'sum Квартал', field: 'sum_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'amount Квартал', field: 'amount_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Квартал', field: 'count_brik_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Тран Квартал', field: 'count_tran_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'sum Мес', field: 'sum_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'amount Мес', field: 'amount_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Мес', field: 'count_brik_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Тран Мес', field: 'count_tran_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'sum Дни', field: 'sum_10', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'amount Дни', field: 'amount_10', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Дни', field: 'count_brik_10', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Тран Дни', field: 'count_tran_10', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE }
        ];

        $scope.dataLoading =
            $http({
                method: 'GET',
                url: '/OFD/Periods/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.SetData(data.Data);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Periods_type_Set = function (value) {
        $scope.Grid.selectedRows().forEach(function (item) {
            $scope.Grid.GridCellsMod(item, "period_type", value);
        });
    };

    $scope.Periods_save = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/Periods_save/',
                data: JSON.stringify({ array: $scope.Grid.GetArrayModify() })
            }).then(function (response) {
                $scope.Grid.ClearModify();
                alert("Сохранил");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    $scope.Init_PeriodsWK = function () {
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, "Grid");

        $scope.Grid.Options.columnDefs = [
            //cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_P}}" target="_blank">ссылка</a></div>'
            //type: 'date'
            { name: 'Id', field: 'Id', type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'Поставщик', field: 'SupplierId', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'Период', field: 'period_wk', filter: { condition: uiGridCustomService.condition } },
            { name: 'Период с', field: 'dt_start', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_YMD },
            { name: 'Период по', field: 'dt_end', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_YMD },
            {
                name: 'Активный', field: 'period_type', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==30}" ng-click="grid.appScope.Periods_type_Set(30)">Кв</button><button type="button" class="btn" ng-class="{\'btn-success\' :row.entity.period_type==20}" ng-click="grid.appScope.Periods_type_Set(20)">Мс</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==15}" ng-click="grid.appScope.Periods_type_Set(15)">Нед</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==0}" ng-click="grid.appScope.Periods_type_Set(0)">Блок</button></div>'
            },
            { name: 'amount Квартал', field: 'amount_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Квартал', field: 'count_brik_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'amount Мес', field: 'amount_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Мес', field: 'count_brik_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'amount Недели', field: 'amount_15', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Недели', field: 'count_brik_15', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE }
        ];
        $scope.dataLoading =
            $http({
                method: 'GET',
                url: '/OFD/PeriodsWK/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.SetData(data.Data);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.PeriodsWK_save = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/PeriodsWK_save/',
                data: JSON.stringify({ array: $scope.Grid.GetArrayModify() })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.ClearModify();
                    alert("Сохранил");
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Init_Periods_4SC = function () {
        $scope.Grid = uiGridCustomService.createGridClassMod($scope, "Grid");
        $scope.Grid.Options.columnDefs = [
            //cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.url_P}}" target="_blank">ссылка</a></div>'
            //type: 'date'
            { name: 'Id', field: 'Id', type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'ПоставщикИД', field: 'SupplierId', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'Поставщик', field: 'Supplier.value', filter: { condition: uiGridCustomService.condition } },
            { name: 'Клиент', field: 'OwnerAgr.Value', filter: { condition: uiGridCustomService.condition } },
            { name: 'Период', field: 'period', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_YMD },
            {
                name: 'Активный', field: 'period_type', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="btn-group"><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==30}" ng-click="grid.appScope.Periods_type_Set(30)">Кв</button><button type="button" class="btn" ng-class="{\'btn-success\' :row.entity.period_type==20}" ng-click="grid.appScope.Periods_type_Set(20)">Мс</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==10}" ng-click="grid.appScope.Periods_type_Set(10)">Дн</button><button type="button" class="btn" ng-class="{\'btn-success\' : row.entity.period_type==0}" ng-click="grid.appScope.Periods_type_Set(0)">Блок</button></div>'
            },
            { name: 'sum Квартал', field: 'sum_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'amount Квартал', field: 'amount_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Квартал', field: 'count_brik_30', type: 'number', headerCellClass: 'beige', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'sum Мес', field: 'sum_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'amount Мес', field: 'amount_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Мес', field: 'count_brik_20', type: 'number', headerCellClass: 'antiquewhite', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'sum Дни', field: 'sum_10', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'amount Дни', field: 'amount_10', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Брики Дни', field: 'count_brik_10', type: 'number', headerCellClass: 'bisque', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE }
        ];
        $scope.dataLoading =
            $http({
                method: 'GET',
                url: '/OFD/Periods_4SC/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.SetData(data.Data.Period_4SC);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Periods_4SC_save = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/Periods_4SC_save/',
                data: JSON.stringify({ array: $scope.Grid.GetArrayModify() })
            }).then(function (response) {
                $scope.Grid.ClearModify();
                alert("Сохранил");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };



    $scope.Agg_Init = function () {
        var SupplierList = [];
        $scope.classifierID = "0";
        $scope.supplier = [];
        $scope.periodStart = new Date();
        $scope.periodEnd = new Date();
        $scope.brickId = "1";

        $scope.selectedIds = [];

        $scope.Grid_Agg = uiGridCustomService.createGridClassMod($scope, "Grid_Agg");

        $scope.Grid_Agg.Options.enableSelectAll = true;
        $scope.Grid_Agg.Options.multiSelect = true;
        $scope.Grid_Agg.Options.enableRowSelection = true;
        $scope.Grid_Agg.Options.enableFullRowSelection = false;
        $scope.Grid_Agg.Options.enableRowHeaderSelection = true;
        $scope.Grid_Agg.Options.noUnselect = false;
        

        $scope.Grid_Agg.Options.columnDefs = [
            { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'FilenameId', enableCellEdit: false, field: 'FilenameId', filter: { condition: uiGridCustomService.condition } },
            { name: 'SupplierId', enableCellEdit: false, field: 'SupplierId', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'classifier_id_psevdo', enableCellEdit: false, field: 'classifier_id_psevdo', filter: { condition: uiGridCustomService.condition } },
            { name: 'BrickId', enableCellEdit: false, field: 'BrickId', filter: { condition: uiGridCustomService.condition } },
            { name: 'amount', enableCellEdit: true, field: 'amount', filter: { condition: uiGridCustomService.condition } },
            { name: 'summa', enableCellEdit: true, field: 'summa', filter: { condition: uiGridCustomService.condition } },
            { name: 'period', enableCellEdit: false, field: 'period', filter: { condition: uiGridCustomService.condition } },
            { name: 'amount_calc', enableCellEdit: false, field: 'amount_calc', filter: { condition: uiGridCustomService.condition } },
            { name: 'ClassifierId', enableCellEdit: false, field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'Periodw', enableCellEdit: false, field: 'Periodw', filter: { condition: uiGridCustomService.condition } },
            { name: 'period_type', enableCellEdit: false, field: 'period_type', filter: { condition: uiGridCustomService.condition } },
            { name: 'Min_price', enableCellEdit: false, field: 'Min_price', filter: { condition: uiGridCustomService.condition } },
            { name: 'Max_price', enableCellEdit: false, field: 'Max_price', filter: { condition: uiGridCustomService.condition } },
            { name: 'Avg_price', enableCellEdit: false, field: 'Avg_price', filter: { condition: uiGridCustomService.condition } },
            { name: 'Median_price', enableCellEdit: false, field: 'Median_price', filter: { condition: uiGridCustomService.condition } },
            { name: 'Mode_price', enableCellEdit: false, field: 'Mode_price', filter: { condition: uiGridCustomService.condition } }
        ];

        //События изменения грида
        $scope.Grid_Agg.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi_Grid_Agg = gridApi;
              //Что-то выделили
            $scope.gridApi_Grid_Agg.selection.on.rowSelectionChanged($scope, Grid_Agg_select);
            $scope.gridApi_Grid_Agg.selection.on.rowSelectionChangedBatch($scope, function (rows) {
                rows.forEach(x => Grid_Agg_select(x))
            });

            $scope.gridApi_Grid_Agg.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.Grid_Agg.NeedSave = true;
                    }
                }
            });
        };

        function Grid_Agg_select(row) {
            if (row.entity && row.entity.Id) {
                let Id = row.entity.Id;
                var index = $scope.selectedIds.indexOf(Id);
                if (index !== -1)
                    $scope.selectedIds.splice(index, 1);
                else
                    $scope.selectedIds.push(Id);
            }
        }

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/OFD/Agg_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.SupplierList, response.data.Data.Supplier);
            $scope.supplier = $scope.SupplierList[0];
            return 1;
        });
    };

    $scope.Agg_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/Agg_search/',
                data: JSON.stringify({ ClassifierId: $scope.classifierID, SupplierId: $scope.supplier.Id, periodStart: $scope.periodStart, periodEnd: $scope.periodEnd, BrickId: $scope.brickId })
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_Agg.SetData(response.data.Data.Agg);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Agg_search = function () {
        if ($scope.Grid_Agg.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Agg_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Agg_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Agg_search_AC();
        }

    };
    $scope.Agg_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/Agg_save/',
                data: JSON.stringify({
                    array: $scope.Grid_Agg.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.Agg_search_AC();
                    }
                    else {
                        $scope.Grid_Agg.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Agg_remove = function () {
        messageBoxService.showConfirm('Удалить выбранные агрегаторы?', 'Удаление').then(
            function (result) {
                $scope.dataLoading =
                    $http({
                        method: 'POST',
                        url: '/OFD/Agg_remove/',
                        data: JSON.stringify({
                            aggToDelete: $scope.selectedIds
                        })
                    }).then(function (response) {
                        var data = response.data;
                        if (data.Success) {
                            $scope.Agg_search_AC();
                            $scope.selectedIds = [];
                        }
                    }, function (response) {
                        errorHandlerService.showResponseError(response);
                    });
            });
    };

    $scope.D4SS_Init = function () {
        var SupplierList = [];
        $scope.classifierID = "0";
        $scope.supplier = [];
        $scope.period = new Date();
        $scope.brickId = "1";
        $scope.Grid_D4SS = uiGridCustomService.createGridClassMod($scope, "Grid_D4SS");

        $scope.Grid_D4SS.Options.columnDefs = [
            { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'FilenameId', enableCellEdit: false, field: 'FilenameId', filter: { condition: uiGridCustomService.condition } },
            { name: 'SupplierId', enableCellEdit: false, field: 'SupplierId', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'AgreementId', enableCellEdit: false, field: 'AgreementId', filter: { condition: uiGridCustomService.condition } },
            { name: 'period', enableCellEdit: false, field: 'period', filter: { condition: uiGridCustomService.condition } },
            { name: 'PharmacyId', enableCellEdit: false, field: 'PharmacyId', filter: { condition: uiGridCustomService.condition } },
            { name: 'OwnerId', enableCellEdit: false, field: 'OwnerId', filter: { condition: uiGridCustomService.condition } },
            { name: 'ReceiptId', enableCellEdit: false, field: 'ReceiptId', filter: { condition: uiGridCustomService.condition } },
            { name: 'stringid', enableCellEdit: false, field: 'stringid', filter: { condition: uiGridCustomService.condition } },
            { name: 'checkitem', enableCellEdit: false, field: 'checkitem', filter: { condition: uiGridCustomService.condition } },
            { name: 'DateTime', enableCellEdit: false, field: 'DateTime', filter: { condition: uiGridCustomService.condition } },
            { name: 'classifier_id_psevdo', enableCellEdit: false, field: 'classifier_id_psevdo', filter: { condition: uiGridCustomService.condition } },
            { name: 'SellingPrice', enableCellEdit: false, field: 'SellingPrice', filter: { condition: uiGridCustomService.condition } },
            { name: 'SellingCount', enableCellEdit: false, field: 'SellingCount', filter: { condition: uiGridCustomService.condition } },
            { name: 'SellingSum', enableCellEdit: false, field: 'SellingSum', filter: { condition: uiGridCustomService.condition } },
            { name: 'ClassifierId', enableCellEdit: false, field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'period_type', enableCellEdit: false, field: 'period_type', filter: { condition: uiGridCustomService.condition } },
            { name: 'ClassifierId_korr', enableCellEdit: false, field: 'ClassifierId_korr', filter: { condition: uiGridCustomService.condition } },
            { name: 'ClassifierId_hand', enableCellEdit: true, field: 'ClassifierId_hand', filter: { condition: uiGridCustomService.condition } },
            { name: 'SellingCountCorr', enableCellEdit: true, field: 'SellingCountCorr', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/OFD/D4SS_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.SupplierList, response.data.Data.Supplier);
            $scope.supplier = $scope.SupplierList[0];
            return 1;
        });
    };

    $scope.D4SS_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/D4SS_search/',
                data: JSON.stringify({})
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_D4SS.SetData(response.data.Data.D4SS);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.D4SS_search = function () {
        if ($scope.Grid_D4SS.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.D4SS_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.D4SS_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.D4SS_search_AC();
        }

    };
    $scope.D4SS_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/D4SS_save/',
                data: JSON.stringify({
                    array: $scope.Grid_D4SS.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.D4SS_search_AC();
                    }
                    else {
                        $scope.Grid_D4SS.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.D4SS_FromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/OFD/D4SS_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                alert('Загрузил')
            }, function (response) {
                $scope.Grid_D4SS.SetData([]);
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };

    $scope.Network_FromExcel = function (files) {
        if (files && files.length)
        {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });

            $scope.dataLoading = $http({
                method: 'POST',
                url: '/OFD/Network_FromExcel/',
                data: formData,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.Agg_search();
            }, function (response) {
                console.log(response);
                var responseData = response.data;
                messageBoxService.showError(responseData);
            });
        }
    };


    /*#12117 - Редактор Согласий 4СС*/
    $scope.D4SC_Agreement_Init = function () {
        $scope.supplier = [];

        $scope.selectedAgreementIds = [];

        $scope.NetworkNames = [];
        $scope.EntityINNs = [];
        $scope.AgreementList = [];

        $scope.filteredNetworkNames = [];
        $scope.filteredEntityINNs = [];

        $scope.periodStart = null;
        $scope.periodEnd = null;
        $scope.isCurrent = true;

        $scope.entityINN = [];
        $scope.networkName = null;

        $scope.Grid_D4SC_Agreement = uiGridCustomService.createGridClassMod($scope, "Grid_D4SC_Agreement");
        $scope.Grid_D4SC_Classifiers = uiGridCustomService.createGridClassMod($scope, "Grid_D4SC_Classifiers");

        $scope.Grid_D4SC_Agreement.Options.enableSelectAll = true;
        $scope.Grid_D4SC_Agreement.Options.multiSelect = true;
        $scope.Grid_D4SC_Agreement.Options.enableRowSelection = true;
        $scope.Grid_D4SC_Agreement.Options.enableFullRowSelection = false;
        $scope.Grid_D4SC_Agreement.Options.enableRowHeaderSelection = true;
        $scope.Grid_D4SC_Agreement.Options.noUnselect = false;


        $scope.Grid_D4SC_Agreement.Options.columnDefs = [
            { name: 'AgreementId', field: 'AgreementId', filter: { condition: uiGridCustomService.condition } },
            { name: 'EntityINN', enableCellEdit: false, field: 'EntityINN', filter: { condition: uiGridCustomService.condition } },
            { name: 'OwnerAgrId', enableCellEdit: false, field: 'OwnerAgrId', filter: { condition: uiGridCustomService.condition } },
            { name: 'SupplierId', enableCellEdit: false, field: 'SupplierId', filter: { condition: uiGridCustomService.condition } },
            { name: 'Name', enableCellEdit: false, field: 'Name', filter: { condition: uiGridCustomService.condition } },
            { name: 'NetworkName', enableCellEdit: false, field: 'NetworkName', filter: { condition: uiGridCustomService.condition } },
            { name: 'Date_begin', enableCellEdit: false, field: 'Date_begin', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } },
            { name: 'Date_end', enableCellEdit: false, field: 'Date_end', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } },
        ];

        $scope.Grid_D4SC_Agreement.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi_Grid_D4SC_Agreement = gridApi;
            $scope.gridApi_Grid_D4SC_Agreement.selection.on.rowSelectionChanged($scope, D4SC_Agreement_select);

            $scope.gridApi_Grid_D4SC_Agreement.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                if (colDef.field !== '@modify') {
                    if (newValue !== oldValue) {
                        rowEntity["@modify"] = true;
                        $scope.Grid_D4SC_Agreement.NeedSave = true;
                    }
                }
            });
        };

        $scope.Grid_D4SC_Classifiers.Options.columnDefs = [
            { name: 'AgreementId', enableCellEdit: false, field: 'AgreementId', filter: { condition: uiGridCustomService.condition } },
            { name: 'ClassifierId', enableCellEdit: false, field: 'ClassifierId', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/OFD/D4SC_Agreement_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.SupplierList, response.data.Data.Supplier);
            Array.prototype.push.apply($scope.NetworkNames, response.data.Data.NetworkNames);
            Array.prototype.push.apply($scope.EntityINNs, response.data.Data.EntityINNs);

            $scope.supplier = $scope.SupplierList[0];

            return 1;
        });
    };

    $scope.filterNetworks = function (supplierId) {
        let data = $scope.NetworkNames.filter((n) => n.SupplierId === supplierId);
        if (data && data[0] && data[0].Networks)
            $scope.filteredNetworkNames = data[0].Networks;
        else
            $scope.filteredNetworkNames = [];
    }

    $scope.filterEntityINN = function (supplierId, networkName) {
        let data = $scope.EntityINNs.filter((n) => n.SupplierId === supplierId && n.NetworkName == networkName);
        if (data && data[0] && data[0].INNs)
            $scope.filteredEntityINNs = data[0].INNs;
        else
            $scope.filteredEntityINNs = [];
    }

    function D4SC_Agreement_select(row) {
        if (row.entity && row.entity.AgreementId) {
            let AgreementId = row.entity.AgreementId;
            $scope.D4SC_GetClassifiers(AgreementId);

            var index = $scope.selectedAgreementIds.indexOf(AgreementId);
            if (index !== -1) 
                $scope.selectedAgreementIds.splice(index, 1);
            else
                $scope.selectedAgreementIds.push(AgreementId);
        }
    }

    $scope.D4SC_Agreement_search = function () {
        $scope.selectedAgreementIds = [];
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/D4SC_Agreement_search/',
                data: JSON.stringify({
                    SupplierId: $scope.supplier.Id, periodStart: $scope.periodStart, periodEnd: $scope.periodEnd, NetworkName: $scope.networkName
                    , EntityINNs: $scope.entityINN.length > 0 ? $scope.entityINN : null, isCurrent: $scope.isCurrent || false
                })
            }).then(function (response) {
                if (response.data.Success) {
                    if (response.data.Data.D4SC_Agreement) {
                        $scope.AgreementList = response.data.Data.D4SC_Agreement;
                        //let data = response.data.Data.D4SC_Agreement;
                        //data.forEach(x => {
                        //    x.Date_begin = new Date(x.Date_begin);
                        //    x.Date_end = new Date(x.Date_end);
                        //});
                        $scope.Grid_D4SC_Agreement.SetData($scope.AgreementList);
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.D4SC_GetClassifiers = function (AgreementId) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/D4SC_Agreement_Classifiers/',
                data: JSON.stringify({ AgreementId: AgreementId })
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_D4SC_Classifiers.SetData(response.data.Data.Data.Classifiers);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.D4SC_Agreement_import = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/OFD/4SC_Agreement_Import.html',
            size: 'lg',
            controller: 'AgreementImportController',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function () {
            $scope.D4SC_Agreement_search();
        });
    };

    $scope.D4SC_Agreement_editDates = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/OFD/4SC_Agreement_EditDates.html',
            size: 'lg',
            controller: 'AgreementEditDatesController',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                AgreementIds: function () {
                    return $scope.selectedAgreementIds;
                }
            }
        });

        modalInstance.result.then(function () {
            $scope.isCurrent = false;
            $scope.D4SC_Agreement_search();
        });
    }
}

angular
    .module('DataAggregatorModule')
    .controller('AgreementImportController', [
        '$scope', 'Upload', 'errorHandlerService', '$uibModalInstance', 'messageBoxService', AgreementImportController]);

function AgreementImportController($scope, Upload, errorHandlerService, $modalInstance, messageBoxService) {
    $scope.isForce = false;
    $scope.isLoading = false;
    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    $scope.import = function (event, file) {
        event.stopPropagation();
        $scope.isLoading = true;

        if (file == null)
            return;

        Upload.upload({
            url: '/OFD/ImportAgreements_from_Excel/',
            data: {
                uploads: file,
                force: $scope.isForce
            }
        }).then(function () {
            $scope.isLoading = false;
            messageBoxService.showError('Файл загружен', 'Успешно');
            $modalInstance.dismiss();
        }, function (response) {
            $scope.isLoading = false;
            errorHandlerService.showResponseError(response);
        });
    };
}

angular
    .module('DataAggregatorModule')
    .controller('AgreementEditDatesController', [
        '$scope', '$http', 'errorHandlerService', '$uibModalInstance', AgreementEditDatesController]);

function AgreementEditDatesController($scope, $http, errorHandlerService, $modalInstance) {
    $scope.dateBegin = new Date();
    $scope.dateEnd = new Date();
    $scope.stop = false;

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    $scope.save = function () {
        $http({
            method: 'POST',
            url: '/OFD/D4SC_Agreement_save/',
            data: JSON.stringify({
                array: $scope.$resolve.AgreementIds,
                dateBegin: $scope.dateBegin,
                dateEnd: $scope.dateEnd,
                stop: $scope.stop
            })
        }).then(function () {
            $modalInstance.close();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }
}