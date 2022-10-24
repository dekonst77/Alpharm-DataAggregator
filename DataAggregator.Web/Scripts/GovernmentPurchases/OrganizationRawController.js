angular
    .module('DataAggregatorModule')
    .controller('OrganizationRawController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', OrganizationRawController]);

function OrganizationRawController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "Организации Обработка";
    $scope.user = userService.getUser();

    $scope.OrganizationRaw_Init = function () {
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
                $scope.OrganizationRaw_IsTrashSet(true);
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
            { cellTooltip: true, name: 'DateUpdate', field: 'DateUpdate', filter: { condition: uiGridCustomService.condition }, type:'date' },
            { cellTooltip: true, name: 'OrganizationId', field: 'OrganizationId', filter: { condition: uiGridCustomService.condition }, type:'number' },            
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
            url: '/OrganizationRaw/OrganizationRaw_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.UsersAll, response.data.Data.UsersAll);
            $scope.OrganizationRaw_search();
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
    $scope.OrganizationRaw_IsTrashSet = function (value) {
        var selectedRows = $scope.Grid_Raw.selectedRows();
        selectedRows.forEach(function (item) {
            $scope.Grid_Raw.GridCellsMod(item, "IsTrash", value);
            $scope.Grid_Raw.GridCellsMod(item, "OrganizationId", null);
            $scope.Grid_Raw.GridCellsMod(item, "UserId", $scope.user.UserId);
            $scope.Grid_Raw.GridCellsMod(item, "Organization", { ShortName: '', INN: '', LocationAddress: '' });
        });
    };
    $scope.OrganizationRaw_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OrganizationRaw/OrganizationRaw_search/',
                data: JSON.stringify({ IsNotReady: $scope.filters.IsNotReady})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_Raw.Options.data = data.Data.Raw;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.OrganizationRaw_search = function () {
        if ($scope.Grid_Raw.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.OrganizationRaw_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.OrganizationRaw_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.OrganizationRaw_search_AC();
        }

    };
    $scope.OrganizationRaw_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OrganizationRaw/OrganizationRaw_save/',
                data: JSON.stringify({
                    array_Raw: $scope.Grid_Raw.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.OrganizationRaw_search_AC();
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
                        Id:null,
                        Inn:null,
                        OrganizationType:null,
                        FullName: null,
                        Text: Value,
                        ShortName:null,
                        OnlyDrugsLinked: $scope.filtersClass.isLS,
                        OnlyEmptyType:false,
                        OnlyEmptyRegion:false,
                        is_LO:false,
                        is_CP:false,
                        is_Actual:false
                    }
                })
            }).then(function (response) {
                var data = response.data;
                    $scope.Grid_SPR.Options.data = data;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

}