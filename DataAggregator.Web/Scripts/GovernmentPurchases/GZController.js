angular
    .module('DataAggregatorModule')
    .controller('GZController', [
        '$scope', '$window', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', GZController]);

function GZController($scope, $window, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.user = userService.getUser();

    $scope.AutoNature_Init = function () {
        $scope.Bricks_L3 = [];
        $scope.Nature = [];
        $scope.Nature_L2 = [];
        $scope.Category = [];
        $scope.Funding = [];


        $scope.Grid_AutoNature = uiGridCustomService.createGridClassMod($scope, "Grid_AutoNature");
        $scope.Grid_AutoNature.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.numberCondition }},
            { name: 'Value',width:100, enableCellEdit: true,  field: 'Value', filter: { condition: uiGridCustomService.condition } },
            { name: 'Наименование', width: 50, enableCellEdit: true,  field: 'IsInName', type: 'boolean' },
            { name: 'Комментарий', width: 100, enableCellEdit: true,  field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, width: 100, name: 'Регион', field: 'Customer_Bricks_L3', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Bricks_L3,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            {
                enableCellEdit: true, width: 100, name: 'Характер', field: 'NatureId', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Nature,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name'
            },
            {
                enableCellEdit: true, width: 100, name: 'ПодХарактер', field: 'Nature_L2Id', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Nature_L2,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name'
            },
            {
                enableCellEdit: true, width: 100, name: 'Финансирование', field: 'FundingId', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Funding,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name'
            }


        ];
        $scope.Grid_AutoNature.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/GZ/AutoNature_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.Bricks_L3, response.data.Data.Bricks_L3);
            Array.prototype.push.apply($scope.Nature, response.data.Data.Nature);
            Array.prototype.push.apply($scope.Nature_L2, response.data.Data.Nature_L2);
            Array.prototype.push.apply($scope.Category, response.data.Data.Category);
            Array.prototype.push.apply($scope.Funding, response.data.Data.Funding);

             $scope.AutoNature_search();
            return 1;
        });
    };
    $scope.AutoNature_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GZ/AutoNature_search/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_AutoNature.Options.data = data.Data.AutoNature_Text;
                    $scope.Grid_AutoNature.ClearModify();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.AutoNature_search = function () {
        if ($scope.Grid_AutoNature.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.AutoNature_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.AutoNature_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.AutoNature_search_AC();
        }

    };
    $scope.AutoNature_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/GZ/AutoNature_save/',
                data: JSON.stringify({
                    array_UPD: $scope.Grid_AutoNature.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.AutoNature_search_AC();
                    }
                    else {
                        $scope.Grid_AutoNature.ClearModify();
                        alert("Сохранил");
                        $scope.AutoNature_search_AC();//Авто перегрузка т.к. нужно чтобы 0 применился иначе будет вечно добавляться
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.AutoNature_NatureSet = function (value) {
        $scope.Grid_AutoNature.selectedRows().forEach(function (item) {
            $scope.Grid_AutoNature.GridCellsMod(item, "NatureId", value);
        });
    };
    $scope.AutoNature_Nature_L2Set = function (value1,calue2) {
        $scope.Grid_AutoNature.selectedRows().forEach(function (item) {
            $scope.Grid_AutoNature.GridCellsMod(item, "Nature_L2Id", value);
        });
    };
    $scope.AutoNature_FundingSet = function (value) {
        $scope.Grid_AutoNature.selectedRows().forEach(function (item) {
            $scope.Grid_AutoNature.GridCellsMod(item, "FundingId", value);
        });
    };

    $scope.AutoNature_Add = function () {
        $scope.Grid_AutoNature.Options.data.push({
            Id: 0, Customer_Bricks_L3: "1.0.0", Value: "", NatureId: null, Nature_L2Id: null, FundingId: null, Comment:"", IsInName:true
        });
    };
    $scope.AutoNature_Delete = function () {
        $scope.Grid_AutoNature.selectedRows().forEach(function (item) {
            $scope.Grid_AutoNature.GridCellsMod(item, "Id", -1 * item.Id);
        });
    };
}