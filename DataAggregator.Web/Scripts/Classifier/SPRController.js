angular
    .module('DataAggregatorModule')
    .controller('SPRController', [
        '$scope', '$window', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', SPRController]);

function SPRController($scope, $window, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "Справочник";
    $scope.user = userService.getUser();

    $scope.SPR_Init = function () {
        $scope.type = $route.current.params["type"];
        $scope.name = $route.current.params["name"];
        $scope.db = $route.current.params["db"];
        $scope.shema = $route.current.params["shema"];
        $window.document.title = "Справочник " + $scope.name;
        $scope.Grid_SPR = uiGridCustomService.createGridClassMod($scope, $scope.name);

        if ($scope.type === "t1") {
            $scope.Grid_SPR.Options.columnDefs = [
                { name: 'Код', field: 'Id', filter: { condition: uiGridCustomService.condition } },
                { name: 'Значение', enableCellEdit: true, field: 'Value', filter: { condition: uiGridCustomService.condition } },
                { name: 'Value', enableCellEdit: true, field: 'Value_Eng', filter: { condition: uiGridCustomService.condition } }
            ];
        }
        if ($scope.type === "t4") {
            $scope.Grid_SPR.Options.columnDefs = [
                { name: 'Код', field: 'Id', filter: { condition: uiGridCustomService.condition } },
                { name: 'Значение', enableCellEdit: true, field: 'Value', filter: { condition: uiGridCustomService.condition } },
                { name: 'Value', enableCellEdit: true, field: 'Value_Eng', filter: { condition: uiGridCustomService.condition } },
                { name: 'Используется', enableCellEdit: true, field: 'UseClassifier', type: 'boolean' },
                { name: 'Используется в ДОП', enableCellEdit: true, field: 'UseGoodsClassifier', type: 'boolean' }
            ];
        }
        if ($scope.type === "t2") {
            $scope.Grid_SPR.Options.columnDefs = [
                { name: 'Код', field: 'Id', filter: { condition: uiGridCustomService.condition } },
                { name: 'Значение', enableCellEdit: false, field: 'Value', filter: { condition: uiGridCustomService.condition } },
                { name: 'Описание', enableCellEdit: true, field: 'Description', filter: { condition: uiGridCustomService.condition } },
                { name: 'Description', enableCellEdit: true, field: 'Description_Eng', filter: { condition: uiGridCustomService.condition } },
                { name: 'Используется', enableCellEdit: true, field: 'IsUse', type: 'boolean' }
            ];
            $scope.Grid_SPR.SetDefaults();
        }
        if ($scope.type === "t3") {
            $scope.Grid_SPR.Options.columnDefs = [
                { name: 'Код', field: 'Id', filter: { condition: uiGridCustomService.condition } },
                { name: 'Значение', enableCellEdit: false, field: 'Value', filter: { condition: uiGridCustomService.condition } },
                { name: 'Описание', enableCellEdit: true, field: 'Description', filter: { condition: uiGridCustomService.condition } }
            ];
        }
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/SPR/SPR_Init/',
            data: JSON.stringify({ db: $scope.db, shema: $scope.shema, name: $scope.name, type: $scope.type })
        }).then(function (response) {
            $scope.SPR_search();
            return 1;
        });
    };

    $scope.SPR_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/SPR/SPR_search/',
                data: JSON.stringify({ db: $scope.db, shema: $scope.shema, name: $scope.name, type: $scope.type })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_SPR.Options.data = data.Data.SPR;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.SPR_search = function () {
        if ($scope.Grid_SPR.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.SPR_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.SPR_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.SPR_search_AC();
        }

    };

    $scope.SPR_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/SPR/SPR_save/',
                data: JSON.stringify({
                    db: $scope.db, shema: $scope.shema,
                    name: $scope.name, type: $scope.type,
                    array_SPR: $scope.Grid_SPR.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.SPR_search_AC();
                    }
                    else {
                        $scope.Grid_SPR.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.SPR_delete = function () {
        var selectedAndFilteredRows = $scope.Grid_SPR.selectedRows();
        selectedAndFilteredRows.forEach(function (item) {
            item.Id = -1 * item.Id;
            item["@modify"] = true;
            $scope.Grid_SPR.NeedSave = true;
        });
    };

    $scope.IsUse = function (value) {
        var selectedAndFilteredRows = $scope.Grid_SPR.selectedRows();
        selectedAndFilteredRows.forEach(function (item) {
            item.IsUse = value;
            item["@modify"] = true;
            $scope.Grid_SPR.NeedSave = true;
        });
    };


    $scope.SPR_FromExcel = function (files) {
        /*    db: $scope.db, shema: $scope.shema,
                name: $scope.name, type: $scope.type,
    */
        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            formData.append('db', $scope.db);
            formData.append('shema', $scope.shema);
            formData.append('name', $scope.name);
            formData.append('type', $scope.type);

            $scope.dataLoading = $http({
                method: 'POST',
                url: '/SPR/SPR_FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.SPR_search_AC();
            }, function (response) {
                $scope.Grid_SPR.Options.data = [];
                errorHandlerService.showResponseError(response);
            });
        }
    };

    hotkeys.bindTo($scope).add({
        combo: 'shift+down',
        description: 'Следующая вниз пустая строка',
        callback: function (event) {
            $scope.searchEmptyDown();
        }
    }).add({
        combo: 'shift+up',
        description: 'Следующая вверх пустая строка',
        callback: function (event) {
            $scope.searchEmptyUp();
        }
    });

    //Поиск вниз следующей строки с пустым полем Value_Eng
    $scope.searchEmptyDown = function () {
        var visibleRows = $scope.Grid_SPR.gridApi.core.getVisibleRows().map(function (item) { return item.entity });        
        var selectedRows = $scope.Grid_SPR.selectedRows();

        var lastSelectedRowIndex = getLastRowIndex(visibleRows, selectedRows);

        //Найдем следующий за lastSelectedRowIndex index, где данные не привязаны
        var emptydata = visibleRows.filter((item, index) => { return index > lastSelectedRowIndex && item.Value_Eng == ""; })[0];
        if (emptydata == null)
            return;

        //Выделим её и проскролим до неё
        $scope.Grid_SPR.gridApi.selection.clearSelectedRows();
        $scope.Grid_SPR.gridApi.selection.selectRow(emptydata);
        $scope.Grid_SPR.gridApi.core.scrollTo(emptydata);

        //console.debug(selectedRows);
        //console.debug(visibleRows);
        //console.debug(lastSelectedRowIndex);
        //console.debug(emptydata);        

        return;       
    }

    //Поиск вверх следующей строки с пустым полем Value_Eng
    $scope.searchEmptyUp = function () {
        var visibleRows = $scope.Grid_SPR.gridApi.core.getVisibleRows().map(function (item) { return item.entity });
        var selectedRows = $scope.Grid_SPR.selectedRows();

        var firstSelectedRowIndex = getFirstRowIndex(visibleRows, selectedRows);

        //Найдем следующий за firstSelectedRowIndex index, где данные не привязаны
        var emptydata = visibleRows.findLast((item, index) => { return index < firstSelectedRowIndex && item.Value_Eng == ""; });
        if (emptydata == null)
            return;

        //Выделим её и проскролим до неё
        $scope.Grid_SPR.gridApi.selection.clearSelectedRows();
        $scope.Grid_SPR.gridApi.selection.selectRow(emptydata);
        $scope.Grid_SPR.gridApi.core.scrollTo(emptydata);

        //console.debug(selectedRows);
        //console.debug(visibleRows);
        //console.debug(firstSelectedRowIndex);
        //console.debug(emptydata);        

        return;
    }

    function getFirstRowIndex(visibleRows, selectedRows) {
        if (visibleRows.length == 0 || selectedRows.length == 0)
            return -1;

        return visibleRows.indexOf(selectedRows[0]);
    }

    function getLastRowIndex(visibleRows, selectedRows) {
        if (visibleRows.length == 0 || selectedRows.length == 0)
            return -1;

        return visibleRows.indexOf(selectedRows[selectedRows.length - 1]);
    }
}