angular
    .module('DataAggregatorModule')
    .controller('KBKController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', KBKController]);

function KBKController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "KBK";
    $scope.user = userService.getUser();

    $scope.NeedSave = function () {
        return $scope.Grid_KBK.NeedSave || $scope.Grid_KBK_Main_Rasp.NeedSave || $scope.Grid_KBK_ZS.NeedSave || $scope.Grid_KBK_Razdel.NeedSave || $scope.Grid_KBK_Razdel2.NeedSave || $scope.Grid_KBK_KodVidRashod.NeedSave;
    };
    $scope.KBK_Init = function () {
        $scope.Bricks_L3 = [];
        $scope.Nature = [];
        $scope.Nature_L2 = [];
        $scope.Category = [];
        $scope.Funding = [];
        $scope.Grid_KBK = uiGridCustomService.createGridClassMod($scope,"Grid_KBK");
        $scope.Grid_KBK_Main_Rasp = uiGridCustomService.createGridClassMod($scope,"Grid_KBK_Main_Rasp");
        $scope.Grid_KBK_ZS = uiGridCustomService.createGridClassMod($scope,"Grid_KBK_ZS");
        $scope.Grid_KBK_Razdel = uiGridCustomService.createGridClassMod($scope,"Grid_KBK_Razdel");
        $scope.Grid_KBK_Razdel2 = uiGridCustomService.createGridClassMod($scope,"Grid_KBK_Razdel2");
        $scope.Grid_KBK_KodVidRashod = uiGridCustomService.createGridClassMod($scope,"Grid_KBK_KodVidRashod");

        $scope.Grid_KBK.Options.columnDefs = [
            { name: 'КБК', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: false, width: 100, name: 'КБКрегион', field: 'Customer_Bricks_L3', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                editType: 'dropdown',
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
                cellTooltip: true, name: 'Финансирование', field: 'KBK_FundingView.Value', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div style="width:100%;height:100%" ng-click="grid.appScope.editFunding(row.entity,-1,null)">{{COL_FIELD}}</div>'
            },
            { cellTooltip: true,enableCellEdit: true, name: 'Комментарий', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Код главного распорядителя', field: 'Main_Rasp', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'Код главного распорядителя описание', field: 'KBK_Main_Rasp.Value', filter: { condition: uiGridCustomService.condition } },
            {
                cellTooltip: true,
                enableCellEdit: false, width: 100, name: 'Код главного распорядителя регион', field: 'KBK_Main_Rasp.Bricks_L3', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Bricks_L3,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            { name: 'Код раздела', field: 'Razdel', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, headerTooltip: true, name: 'Код раздела описание', field: 'KBK_Razdel.Value', filter: { condition: uiGridCustomService.condition } },
            { name: 'Код подраздела', field: 'Razdel2', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,headerTooltip: true, name: 'Код подраздела описание', field: 'KBK_Razdel2.Value', filter: { condition: uiGridCustomService.condition } },
            { name: 'Целевая Статья', field: 'ZS', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Описание Программное (непрограммное) направление расходов', field: 'KBK_ZS.ZS_M1_Value', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Описание Подпрограмма', field: 'KBK_ZS.ZS_M2_Value', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Описание Основное мероприятие', field: 'KBK_ZS.ZS_MM_Value', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Описание Направление расходов', field: 'KBK_ZS.ZS_Napr_Value', filter: { condition: uiGridCustomService.condition } },

            { name: 'Код вида расходов', field: 'KodVidRashod', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,headerTooltip: true, name: 'Код вида расходов описание', field: 'KBK_KodVidRashod.Value', filter: { condition: uiGridCustomService.condition } }
        ];


        $scope.Grid_KBK_Main_Rasp.Options.columnDefs = [
            { name: 'Код главного распорядителя', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: false, width: 100, name: 'КБКрегион', field: 'Customer_Bricks_L3', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Bricks_L3,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            { cellTooltip: true,name: 'Значение', enableCellEdit: true, field: 'Value', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, width: 100, name: 'Регион', field: 'Bricks_L3', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Bricks_L3,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            }
        ];
        $scope.Grid_KBK_ZS.Options.columnDefs = [
            { name: 'Код главного распорядителя', field: 'Main_Rasp', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: false, width: 100, name: 'КБКрегион', field: 'Customer_Bricks_L3', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Bricks_L3,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },
            { name: 'Целевая Стаья', enableCellEdit: true, field: 'ZS', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Программное (непрограммное) направление расходов', field: 'ZS_M1', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Описание Программное (непрограммное) направление расходов', enableCellEdit: true, field: 'ZS_M1_Value', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Подпрограмма', field: 'ZS_M2', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Описание Подпрограмма', enableCellEdit: true, field: 'ZS_M2_Value', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Основное мероприятие', field: 'ZS_MM', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Описание Основное мероприятие', enableCellEdit: true, field: 'ZS_MM_Value', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Направление расходов', field: 'ZS_Napr', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Описание Направление расходов', enableCellEdit: true, field: 'ZS_Napr_Value', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid_KBK_Razdel.Options.columnDefs = [
            { name: 'Код раздела', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true, name: 'Значение', enableCellEdit: true,field: 'Value', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid_KBK_Razdel2.Options.columnDefs = [
            { name: 'Код подраздела', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Значение', enableCellEdit: true,field: 'Value', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid_KBK_KodVidRashod.Options.columnDefs = [
            { name: 'Код вида расходов', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { cellTooltip: true,name: 'Значение', enableCellEdit: true,field: 'Value', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/KBK/KBK_Init/',
            data: JSON.stringify({})
        }).then(function (response) {

            Array.prototype.push.apply($scope.Bricks_L3, response.data.Data.Bricks_L3);
            Array.prototype.push.apply($scope.Nature, response.data.Data.Nature);
            Array.prototype.push.apply($scope.Nature_L2, response.data.Data.Nature_L2);
            Array.prototype.push.apply($scope.Category, response.data.Data.Category);
            Array.prototype.push.apply($scope.Funding, response.data.Data.Funding);

            //$scope.Category.push({"Name": "пусто" });
            //$scope.Nature.push({ "Id": 0, "NameMini": "пусто", "CategoryName": "пусто" });
            //$scope.Nature_L2.push({ "Id": 0, "Name": "пусто" });

            $scope.KBK_search();
            return response.data;
        });
    };

    $scope.KBK_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/KBK/KBK_search/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_KBK.Options.data = data.Data.KBK;
                    $scope.Grid_KBK_Main_Rasp.Options.data = data.Data.KBK_Main_Rasp;
                    $scope.Grid_KBK_ZS.Options.data = data.Data.KBK_ZS;
                    $scope.Grid_KBK_Razdel.Options.data = data.Data.KBK_Razdel;
                    $scope.Grid_KBK_Razdel2.Options.data = data.Data.KBK_Razdel2;
                    $scope.Grid_KBK_KodVidRashod.Options.data = data.Data.KBK_KodVidRashod;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.KBK_search = function () {
        if ($scope.NeedSave() === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.KBK_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.KBK_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.KBK_search_AC();
        }

    };
    $scope.KBK_save = function (action) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/KBK/KBK_save/',
                    data: JSON.stringify({
                        array_KBK: $scope.Grid_KBK.GetArrayModify(),
                        array_KBK_Main_Rasp: $scope.Grid_KBK_Main_Rasp.GetArrayModify(),
                        array_KBK_ZS: $scope.Grid_KBK_ZS.GetArrayModify(),
                        array_KBK_Razdel: $scope.Grid_KBK_Razdel.GetArrayModify(),
                        array_KBK_Razdel2: $scope.Grid_KBK_Razdel2.GetArrayModify(),
                        array_KBK_KodVidRashod: $scope.Grid_KBK_KodVidRashod.GetArrayModify()
                    })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        if (action === "search") {
                            $scope.KBK_search_AC();
                        }
                        else {
                            $scope.Grid_KBK.ClearModify();
                            $scope.Grid_KBK_Main_Rasp.ClearModify();
                            $scope.Grid_KBK_ZS.ClearModify();
                            $scope.Grid_KBK_Razdel.ClearModify();
                            $scope.Grid_KBK_Razdel2.ClearModify();
                            $scope.Grid_KBK_KodVidRashod.ClearModify();

                            alert("Сохранил");
                        }
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
    };
    $scope.KBK_NatureSet = function (value) {
        var selectedRows = $scope.Grid_KBK.selectedRows();
        selectedRows.forEach(function (item) {
            $scope.Grid_KBK.GridCellsMod(item, "NatureId", value);
        });
    };
    $scope.KBK_Nature_L2Set = function (value) {
        var selectedRows = $scope.Grid_KBK.selectedRows();
        selectedRows.forEach(function (item) {
            $scope.Grid_KBK.GridCellsMod(item, "Nature_L2Id", value);
        });
    };
    $scope.editFunding = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_FundingView.html',
            controller: 'FundingController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                fundingList: function () {
                    $scope.Funding.forEach(function (item) {
                        item.CheckedList = [{ Checked:false }];
                    });
                    if (row !== null) {
                        row.KBK_Funding.forEach(function (item) {
                            if (item.KBKId === row.Id && item.Customer_Bricks_L3 === row.Customer_Bricks_L3) {
                                $scope.Funding.forEach(function (Fundingitem) {
                                    if (Fundingitem.Id === item.FundingId) {
                                        Fundingitem.CheckedList[0].Checked = true;
                                    }
                                });
                            }
                        });
                    }
                    return $scope.Funding;
                },
                sourceOfFinancing: function () {
                    if (row===null || row.KBK_FundingView === null)
                        return "";
                    else
                        return row.KBK_FundingView.Value;
                }
            }
        });

        modalInstance.result.then(function (fundingList) {
            //$scope.KBK_FundingMod.push({ KBKId: fundingList.KBKId, Customer_Bricks_L3: fundingList.Customer_Bricks_L3, FundingId: 0 });//чтобы удалить все что касалось этого элемента
            var rows = [];
            if (row === null) {
                rows = $scope.Grid_KBK.selectedRows();
            }
            else {
                rows.push(row);
            }
            rows.forEach(function (itemRow) {
                 
                //for (var i = 0; i < itemRow.KBK_Funding.length; i++) {
                    itemRow.KBK_Funding.splice(0, itemRow.KBK_Funding.length);
                    //if (itemRow.KBK_Funding[i].KBKId === fundingList.KBKId && itemRow.KBK_Funding[i].Customer_Bricks_L3 === fundingList.Customer_Bricks_L3) {
                    //    itemRow.KBK_Funding.splice(i, 1);
                    //    i--;
                    //}
                //}
                var result = [];
                fundingList.forEach(function (item) {
                    if (item.CheckedList[0].Checked === true) {
                        itemRow.KBK_Funding.push({ KBKId: itemRow.Id, Customer_Bricks_L3: itemRow.Customer_Bricks_L3, FundingId: item.Id });
                        result.push(item.Name);
                    }
                });
                itemRow.KBK_FundingView = { Value: "" };
                $scope.Grid_KBK.GridCellsMod(itemRow, "@modify", true);
                itemRow.KBK_FundingView.Value = result.join(", ");
            });
        }, function () {
        });
    };
}