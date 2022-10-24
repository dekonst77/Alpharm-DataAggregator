angular
    .module('DataAggregatorModule')
    .controller('DDDController', [
        '$scope', '$window', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', DDDController]);

function DDDController($scope, $window, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "Справочник";
    $scope.user = userService.getUser();

    $scope.DDD_Norma_Init = function () {
        $scope.ATCWho = [];
        $scope.RouteAdministration = [];
        $scope.DDD_Units = [];
        $window.document.title = "DDD Норма";
        $scope.Grid_DDD_Norma = uiGridCustomService.createGridClassMod($scope,"Grid_DDD_Norma");


        $scope.Grid_DDD_Norma.Options.columnDefs = [
            {
                enableCellEdit: true, width: 100, name: 'ATCWho', field: 'ATCWhoId', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.ATCWho,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            {
                enableCellEdit: true, width: 100, name: 'Способ введения', field: 'RouteAdministrationId', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.RouteAdministration,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            { name: 'DDD', enableCellEdit: true, field: 'DDD', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_FLOAT3_COUNT },
            {
                enableCellEdit: true, width: 100, name: 'Units', field: 'Units', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.DDD_Units,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            { name: 'примечание', enableCellEdit: true, field: 'Description', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DDD/DDD_Norma_Init/',
            data: JSON.stringify({ name: $scope.name, type: $scope.type })
        }).then(function (response) {
            Array.prototype.push.apply($scope.ATCWho, response.data.Data.ATCWho);
            Array.prototype.push.apply($scope.RouteAdministration, response.data.Data.RouteAdministration);
            Array.prototype.push.apply($scope.DDD_Units, response.data.Data.DDD_Units);
            $scope.DDD_Norma_search();
            return 1;
        });
    };
    $scope.DDD_Norma_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DDD/DDD_Norma_search/',
                data: JSON.stringify({ name: $scope.name, type: $scope.type })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_DDD_Norma.Options.data = data.Data.DDD_Norma;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.DDD_Norma_search = function () {
        if ($scope.Grid_DDD_Norma.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.DDD_Norma_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.DDD_Norma_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.DDD_Norma_search_AC();
        }

    };
    $scope.DDD_Norma_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DDD/DDD_Norma_save/',
                data: JSON.stringify({
                    name: $scope.name, type: $scope.type,
                    array_SPR: $scope.Grid_DDD_Norma.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.DDD_Norma_search_AC();
                    }
                    else {
                        $scope.Grid_DDD_Norma.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.DDD_Norma_Add = function () {
        var new_element = {
            ATCWhoId: $scope.ATCWho[0].Id,
            RouteAdministrationId: $scope.RouteAdministration[0].Id,
            DDD: 0.0,
            Units: $scope.DDD_Units[0].Id,
            Description: ""
        };
        $scope.Grid_DDD_Norma.Options.data.push(new_element);
    };


    $scope.Calculate = function (row_real) {
        var row = {
            DDD_Formula: row_real.DDD_Formula,
            ConsumerPackingCount: row_real.ConsumerPackingCount,
            inn_1_Count: row_real.inn_1_Count,
            inn_1_Unit: row_real.inn_1_Unit,
            inn_2_Count: row_real.inn_2_Count,
            inn_2_Unit: row_real.inn_2_Unit,
            main_Dos_In_Count: row_real.main_Dos_In_Count,
            main_Dos_In_Unit: row_real.main_Dos_In_Unit,
            main_Dos_Total_Count: row_real.main_Dos_Total_Count,
            main_Dos_Total_Unit: row_real.main_Dos_Total_Unit,
            DDD_Norma: row_real.DDD_Norma,
            DDD_Units: row_real.DDD_Units
        };
        if (row.inn_1_Unit === "%" && row.main_Dos_In_Unit === null) {
            row.inn_1_Count = 10 * row.inn_1_Count;
            row.main_Dos_In_Count = 1;
            row.main_Dos_In_Unit = row.main_Dos_Total_Unit;
            row.inn_1_Unit = "мг";
        };
        var res = 1000000.0;

        var formula = row.DDD_Formula;
        formula = formula.replace(/ /g, '');
        formula = formula.replace(/[*]/g, ' *');
        formula = formula.replace(/[/]/g, ' /');
        formula = formula.replace(/=/g, ' =');
        if (formula === "" || formula === null || formula === 'Неприменимо' || formula === undefined) {
            return 0;
        }
        var act = formula.split(' ');
        act.forEach(function (item) {
            if (item[0] === '=')
            {
                var v1 = $scope.Calculate_GetValue(row, item.replace('=', ''));
                res = res * v1;
            }
            if (item[0] === '*')
            {
                var v2 = $scope.Calculate_GetValue(row, item.replace('*', ''));
                res = res * v2;
            }
            if (item[0] === '/')
            {
                var v3 = 1000000*$scope.Calculate_GetValue(row, item.replace('/', ''));
                res = 1000000 * res / v3; 
            }
        });
        //Формула таблеток = INN1 * N / DDD_Norma
        //Формула сиропов=INN1/DosIn*DosTotal*N/DDD_Norma

        return res / 1000000.0;
    }
    $scope.Calculate_GetValue=function(row, item)
    {
        var ret = 0.0;
        if (item === 'N')
        {
            ret = 1.0 * row.ConsumerPackingCount;
        }
        else
        if (item === 'INN1') {
            ret = $scope.Calculate_GetUnits_Standart(1.0 * row.inn_1_Count, row.inn_1_Unit)
            }
        else
        if (item === 'INN2') {
            ret = $scope.Calculate_GetUnits_Standart(1.0 * row.inn_2_Count, row.inn_2_Unit)
        }
        else
            if (item === 'DosIn') {
                ret = $scope.Calculate_GetUnits_Standart(1.0 * row.main_Dos_In_Count, row.main_Dos_In_Unit)
            }
            else
                if (item === 'DosTotal') {
                    ret = $scope.Calculate_GetUnits_Standart(1.0 * row.main_Dos_Total_Count, row.main_Dos_Total_Unit)
                }
                else
                    if (item === 'DDD_Norma') {
                        ret = $scope.Calculate_GetUnits_Standart(1.0 * row.DDD_Norma, row.DDD_Units)
                    }
                    else
                        ret = 1.0 * item;
        return ret;
    };
    $scope.Calculate_GetUnits_Standart = function (ret, Units) {
        var kof = 0.0;
        $scope.DDD_Units_Standart.forEach(function (item) {
            if (item.Id === Units)
                kof = item.Value;
        });
        return kof * ret;
    };
    $scope.DDD_Init = function () {
        //$scope.ATCWho = [];
        //$scope.RouteAdministration = [];
        $scope.DDD_Units = [];
        $scope.DDD_Formulas = [];
        $scope.DDD_Units_Standart = [];
        $window.document.title = "DDD";
        $scope.Grid_DDD = uiGridCustomService.createGridClassMod($scope,"Grid_DDD");


        $scope.Grid_DDD.Options.columnDefs = [
            { name: 'ClassifierId', width: 100, enableCellEdit: false, field: 'ClassifierId', filter: { condition: uiGridCustomService.condition } },
            { name: 'DrugId', width: 100, enableCellEdit: false, field: 'DrugId', filter: { condition: uiGridCustomService.condition } },
            { name: 'OwnerTradeMarkId', width: 100, enableCellEdit: false, field: 'OwnerTradeMarkId', filter: { condition: uiGridCustomService.condition } },
            { name: 'PackerId', width: 100, enableCellEdit: false, field: 'PackerId', filter: { condition: uiGridCustomService.condition } },
            { name: 'Номер РУ', width: 100, enableCellEdit: false, field: 'RCNumber', filter: { condition: uiGridCustomService.condition } },
            { name: 'ТН', width: 100, enableCellEdit: false, field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
            { name: 'INNGroup', width: 100, enableCellEdit: false, field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
            { name: 'ФВ', cellTooltip: true,width: 100, enableCellEdit: false, field: 'FormProduct', filter: { condition: uiGridCustomService.condition } },
            { name: 'NFC', width: 100, enableCellEdit: false, field: 'nfc_Value', filter: { condition: uiGridCustomService.condition } },
            { name: 'NFC примечание', width: 100, enableCellEdit: false, field: 'nfc_Description', filter: { condition: uiGridCustomService.condition } },
            { name: 'Путь введения', width: 100, enableCellEdit: false, field: 'RouteAdministration', filter: { condition: uiGridCustomService.condition } },
            { name: 'ATCWho', width: 100, enableCellEdit: false, field: 'who_Value', filter: { condition: uiGridCustomService.condition } },
            { name: 'ATCWho примечание', width: 100, enableCellEdit: false, field: 'who_Description', filter: { condition: uiGridCustomService.condition } },
            { name: 'DosIn', width: 100, enableCellEdit: false, field: 'main_Dos_In_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'Dos Ед', width: 100, enableCellEdit: false, field: 'main_Dos_In_Unit', filter: { condition: uiGridCustomService.condition } },
            { name: 'DosTotal', width: 100, enableCellEdit: false, field: 'main_Dos_Total_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'DosTotal Ед', width: 100, enableCellEdit: false, field: 'main_Dos_Total_Unit', filter: { condition: uiGridCustomService.condition } },
            { name: 'К МНН', width: 100, enableCellEdit: false, field: 'count_INN', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'МНН1', width: 100, enableCellEdit: false, field: 'inn_1', filter: { condition: uiGridCustomService.condition } },
            { name: 'INN1', width: 100, enableCellEdit: false, field: 'inn_1_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'МНН1 Ед', width: 100, enableCellEdit: false, field: 'inn_1_Unit', filter: { condition: uiGridCustomService.condition } },
            { name: 'МНН2', width: 100, enableCellEdit: false, field: 'inn_2', filter: { condition: uiGridCustomService.condition } },
            { name: 'INN2', width: 100, enableCellEdit: false, field: 'inn_2_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'МНН2 Ед', width: 100, enableCellEdit: false, field: 'inn_2_Unit', filter: { condition: uiGridCustomService.condition } },
            { name: 'N', width: 100, enableCellEdit: false, field: 'ConsumerPackingCount', filter: { condition: uiGridCustomService.numberCondition } },
            { enableCellEdit: true, width: 100, name: 'Проверено', field: 'DDD_chek', type: 'boolean' },
            { name: 'DDD_Norma', width: 100, enableCellEdit: true, field: 'DDD_Norma', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            {
                enableCellEdit: true, width: 100, name: 'DDD_Units', field: 'DDD_Units', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.DDD_Units,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            { name: 'примечание', width: 100, enableCellEdit: true, field: 'DDD_Comment', filter: { condition: uiGridCustomService.condition } },
            { name: 'Формула', width: 100, enableCellEdit: true, field: 'DDD_Formula', filter: { condition: uiGridCustomService.condition } },
            { name: 'DDDs', width: 100, enableCellEdit: true, field: 'DDDs', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid_DDD.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DDD/DDD_Init/',
            data: JSON.stringify({ name: $scope.name, type: $scope.type })
        }).then(function (response) {
            //Array.prototype.push.apply($scope.ATCWho, response.data.Data.ATCWho);
            //Array.prototype.push.apply($scope.RouteAdministration, response.data.Data.RouteAdministration);
            Array.prototype.push.apply($scope.DDD_Units, response.data.Data.DDD_Units);
            Array.prototype.push.apply($scope.DDD_Formulas, response.data.Data.DDD_Formulas);
            Array.prototype.push.apply($scope.DDD_Units_Standart, response.data.Data.DDD_Units_Standart);
            $scope.DDD_search();
            return 1;
        });
    };
    $scope.DDD_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DDD/DDD_search/',
                data: JSON.stringify({ name: $scope.name, type: $scope.type })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_DDD.Options.data = data.Data.DDD;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.DDD_search = function () {
        if ($scope.Grid_DDD.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.DDD_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.DDD_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.DDD_search_AC();
        }

    };
    $scope.DDD_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DDD/DDD_save/',
                data: JSON.stringify({
                    name: $scope.name, type: $scope.type,
                    array_SPR: $scope.Grid_DDD.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.DDD_search_AC();
                    }
                    else {
                        $scope.Grid_DDD.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.DDD_Chek = function (value) {
        $scope.Grid_DDD.selectedRows().forEach(function (item) {
            $scope.Grid_DDD.GridCellsMod(item, "DDD_chek", value);

        });
    };
    $scope.DDD_FormulasSet = function (value) {
        var selectedAndFilteredRows = $scope.Grid_DDD.selectedRows();
        selectedAndFilteredRows.forEach(function (item) {
            if (value !== '=')//Пересчитать формулы
                $scope.Grid_DDD.GridCellsMod(item, "DDD_Formula", value);

            $scope.Grid_DDD.GridCellsMod(item, "DDDs", $scope.Calculate(item));
        });
    };

    $scope.open_formula = function () {
        $scope.formula_new = "";
        var sel = $scope.Grid_DDD.selectedRows();
        if (sel.length > 0) {
            $scope.formula_new = sel[0].DDD_Formula;
            $('#modal_formula').modal('show');
        }
    };
    $scope.DDD_FormulasSetCLC = function () {
        $scope.DDD_FormulasSet($scope.formula_new);
    };
    $scope.DDD_FormulasAdd = function (value) {
        $scope.formula_new += value;
    };

    $scope.StandardUnits_Init = function () {
        $scope.EI = [];
        $window.document.title = "StandardUnits";
        $scope.Grid_StandardUnits = uiGridCustomService.createGridClassMod($scope, "Grid_StandardUnits");


        $scope.Grid_StandardUnits.Options.columnDefs = [
            { name: 'ClassifierId', width: 100, enableCellEdit: false, field: 'ClassifierId', filter: { condition: uiGridCustomService.condition } },
            { name: 'DrugId', width: 100, enableCellEdit: false, field: 'DrugId', filter: { condition: uiGridCustomService.condition } },
            { name: 'Номер РУ', width: 100, enableCellEdit: false, field: 'RCNumber', filter: { condition: uiGridCustomService.condition } },
            { name: 'ТН', width: 100, enableCellEdit: false, field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
            { name: 'INNGroup', width: 100, enableCellEdit: false, field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
            { name: 'ФВ', cellTooltip: true, width: 100, enableCellEdit: false, field: 'FormProduct', filter: { condition: uiGridCustomService.condition } },
            { name: 'DosageGroup', width: 100, enableCellEdit: false, field: 'DosageGroup', filter: { condition: uiGridCustomService.condition } },
            { name: 'N', width: 100, enableCellEdit: false, field: 'ConsumerPackingCount', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'Доз1', width: 100, enableCellEdit: false, field: 'inn_1_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'Доз1 Ед', width: 100, enableCellEdit: false, field: 'inn_1_Unit', filter: { condition: uiGridCustomService.condition } },
            { name: 'Доз2', width: 100, enableCellEdit: false, field: 'inn_2_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'Доз2 Ед', width: 100, enableCellEdit: false, field: 'inn_2_Unit', filter: { condition: uiGridCustomService.condition } },
            { name: 'Доз3', width: 100, enableCellEdit: false, field: 'inn_3_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'Доз3 Ед', width: 100, enableCellEdit: false, field: 'inn_3_Unit', filter: { condition: uiGridCustomService.condition } },
           { name: 'DosIn', width: 100, enableCellEdit: false, field: 'main_Dos_In_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'Dos Ед', width: 100, enableCellEdit: false, field: 'main_Dos_In_Unit', filter: { condition: uiGridCustomService.condition } },
            { name: 'DosTotal', width: 100, enableCellEdit: false, field: 'main_Dos_Total_Count', filter: { condition: uiGridCustomService.condition } },
            { name: 'DosTotal Ед', width: 100, enableCellEdit: false, field: 'main_Dos_Total_Unit', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, width: 100, name: 'EI', field: 'EIId', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.EI,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            { name: 'SU', width: 100, enableCellEdit: false, field: 'StandardUnits', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'SU Проверено', width: 100, enableCellEdit: true, field: 'StandardUnits_Ckeck', type: 'boolean' },
            { name: 'РучнойSU', width: 100, enableCellEdit: true, field: 'StandardUnits_Hand', filter: { condition: uiGridCustomService.numberCondition } }

        ];
        $scope.Grid_StandardUnits.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DDD/StandardUnits_Init/',
            data: JSON.stringify({ name: $scope.name, type: $scope.type })
        }).then(function (response) {
            Array.prototype.push.apply($scope.EI, response.data.Data.EI);
            $scope.StandardUnits_search();
            return 1;
        });
    };
    $scope.StandardUnits_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DDD/StandardUnits_search/',
                data: JSON.stringify({ name: $scope.name, type: $scope.type })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_StandardUnits.Options.data = data.Data.StandardUnits;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.StandardUnits_search = function () {
        if ($scope.Grid_StandardUnits.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.StandardUnits_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.StandardUnits_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.StandardUnits_search_AC();
        }

    };
    $scope.StandardUnits_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DDD/StandardUnits_save/',
                data: JSON.stringify({
                    array_SPR: $scope.Grid_StandardUnits.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.StandardUnits_search_AC();
                    }
                    else {
                        $scope.Grid_StandardUnits.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.StandardUnitsSet = function (IdValue) {
        var selectedAndFilteredRows = $scope.Grid_StandardUnits.selectedRows();
        selectedAndFilteredRows.forEach(function (item) {
            $scope.Grid_StandardUnits.GridCellsMod(item, "EIId", IdValue);
        });
    };
}