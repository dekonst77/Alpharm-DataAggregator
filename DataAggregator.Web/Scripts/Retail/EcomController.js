angular
    .module('DataAggregatorModule')
    .controller('EcomController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', 'uiGridTreeViewConstants', EcomController]);

function EcomController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService, uiGridTreeViewConstants) {
    $scope.IsRowSelection = false;
    $scope.Title = "Ecom";
    $scope.user = userService.getUser();
    ///////////////////////////////ГС Старт
    $scope.Coefficients_Init = function () {        
        $scope.periods = [];
        $scope.format = 'dd.MM.yyyy';
        $scope.currentperiod = null;

        hotkeys.bindTo($scope).add({
            combo: 'shift+w',
            description: 'Одобрено',
            callback: function (event) {
                $scope.Grid_NetworkBrand.selectedRows().forEach(function (item) {
                    $scope.Grid_NetworkBrand.GridCellsMod(item, "Used", true);
                });
            }
        });
        $scope.Grid_RegionalCoefficients = uiGridCustomService.createGridClassMod($scope, "RegionalCoefficients", RegionalCoefficients_onSelectionChanged);
        $scope.Grid_CoefficientsCount = uiGridCustomService.createGridClassMod($scope, "CoefficientsCount");
        $scope.Grid_CoefficientsPrice = uiGridCustomService.createGridClassMod($scope, "CoefficientsPrice");
        $scope.Grid_Coefficients = uiGridCustomService.createGridClassMod($scope, "Coefficients");

        $scope.Grid_RegionalCoefficients.Options.columnDefs = [
            { name: 'Period', visible: false,field: 'Period', filter: { condition: uiGridCustomService.condition } },
            { name: 'RegionCode', visible: false,field: 'RegionCode', filter: { condition: uiGridCustomService.condition } },
            { name: 'Region', field: 'Region', filter: { condition: uiGridCustomService.condition } },
            { name: 'RegCoeff', width: 100, field: 'RegCoeff', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
        ];
        $scope.Grid_CoefficientsCount.Options.columnDefs = [
            { name: 'Period',visible:false, field: 'Period', filter: { condition: uiGridCustomService.condition } },
            { name: 'RegionCode', field: 'RegionCode', filter: { condition: uiGridCustomService.condition } },
            { name: 'CountCategory', field: 'CountCategory', filter: { condition: uiGridCustomService.condition } },
            { name: 'CountMin', width: 100, field: 'CountMin', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'CountMax', width: 100, field: 'CountMax', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
        ];
        $scope.Grid_CoefficientsPrice.Options.columnDefs = [
            { name: 'Period', visible: false,field: 'Period', filter: { condition: uiGridCustomService.condition } },
            { name: 'RegionCode', field: 'RegionCode', filter: { condition: uiGridCustomService.condition } },
            { name: 'PriceCategory', field: 'PriceCategory', filter: { condition: uiGridCustomService.condition } },
            { name: 'PriceMin', width: 100, field: 'PriceMin', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'PriceMax', width: 100, field: 'PriceMax', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
        ];
        $scope.Grid_Coefficients.Options.columnDefs = [
            { name: 'Period', visible: false,field: 'Period', filter: { condition: uiGridCustomService.condition } },
            { name: 'RegionCode', field: 'RegionCode', filter: { condition: uiGridCustomService.condition } },
            { name: 'PriceCategory', field: 'PriceCategory', filter: { condition: uiGridCustomService.condition } },
            { name: 'CountA', width: 100, field: 'CoefficientColsA', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
            ,{ name: 'CountB', width: 100, field: 'CoefficientColsB', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
            ,{ name: 'CountC', width: 100, field: 'CoefficientColsC', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
            ,{ name: 'CountD', width: 100, field: 'CoefficientColsD', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
        ];
        $scope.Grid_RegionalCoefficients.SetDefaults();
        $scope.Grid_CoefficientsCount.SetDefaults();
        $scope.Grid_CoefficientsPrice.SetDefaults();
        $scope.Grid_Coefficients.SetDefaults();

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Ecom/Coefficients_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.periods, response.data.Periods);
            $scope.currentperiod = $scope.periods[0];
            $scope.Coefficients_Search();
            return 1;
        });


    };
    function RegionalCoefficients_onSelectionChanged(row) {
        if (row !== undefined) {
            if (row.entity === undefined) {
                $scope.CurrentRowRegionalCoefficients = row;
            }
            else {
                $scope.CurrentRowRegionalCoefficients = row.entity;
            }
            $scope.Grid_CoefficientsCount.FilterSet("RegionCode", $scope.CurrentRowRegionalCoefficients.RegionCode);
            $scope.Grid_CoefficientsPrice.FilterSet("RegionCode", $scope.CurrentRowRegionalCoefficients.RegionCode);
            $scope.Grid_Coefficients.FilterSet("RegionCode", $scope.CurrentRowRegionalCoefficients.RegionCode);
            
        }        
    };
    $scope.Coefficients_Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Ecom/Coefficients_Search/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod})
            }).then(function (response) {
                    $scope.Grid_RegionalCoefficients.Options.data = response.data.RegionalCoefficients;
                    $scope.Grid_CoefficientsCount.Options.data = response.data.CoefficientsCount;
                    $scope.Grid_CoefficientsPrice.Options.data = response.data.CoefficientsPrice;
                $scope.Grid_Coefficients.Options.data = response.data.Coefficients;
                $scope.Grid_RegionalCoefficients.setFirstSelected();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Coefficients_Search = function () {
        if ($scope.Grid_RegionalCoefficients.NeedSave === true
            || $scope.Grid_CoefficientsCount.NeedSave === true
            || $scope.Grid_CoefficientsPrice.NeedSave === true
            || $scope.Grid_Coefficients.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Coefficients_Save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Coefficients_Search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Coefficients_Search_AC();
        }

    };
    $scope.Coefficients_Save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Ecom/Coefficients_Save/',
                data: JSON.stringify({
                    RegionalCoefficients: $scope.Grid_RegionalCoefficients.GetArrayModify(),
                    CoefficientsCount: $scope.Grid_CoefficientsCount.GetArrayModify(),
                    CoefficientsPrice: $scope.Grid_CoefficientsPrice.GetArrayModify(),
                    Coefficients: $scope.Grid_Coefficients.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                        $scope.Grid_RegionalCoefficients.ClearModify();
                        $scope.Grid_CoefficientsCount.ClearModify();
                        $scope.Grid_CoefficientsPrice.ClearModify();
                        $scope.Grid_Coefficients.ClearModify();
                        alert("Сохранил");
                            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Coefficients_Calc = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Ecom/Coefficients_Calc/',
                data: JSON.stringify({ currentperiod: $scope.currentperiod })
            }).then(function (response) {
                alert("Запущено");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Coefficients_from_Excel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
                formData.append('currentperiod', $scope.currentperiod);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/Ecom/Coefficients_from_Excel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.Coefficients_Search_AC();
            }, function (response) {
                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };
}