angular
    .module('DataAggregatorModule')
    .controller('LPUPointController', ['$scope', '$http', 'messageBoxService', 'errorHandlerService', 'uiGridCustomService', 'formatConstants', LPUPointController]);

function LPUPointController($scope, $http, messageBoxService, errorHandlerService, uiGridCustomService, formatConstants) {

    $scope.GridLPUPoint = uiGridCustomService.createGridClassMod($scope, 'LPUPoint_Grid', LPUPoint_onSelectionChanged);
    $scope.GridLPUPoint.Options.showGridFooter = true;
    $scope.GridLPUPoint.Options.multiSelect = true;
    $scope.GridLPUPoint.Options.modifierKeysToMultiSelect = true;
    $scope.filter = {
        IsActual: true
    };
    $scope.GridLPUPoint.Options.columnDefs = [
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'PointId', field: 'PointId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ActualId', field: 'ActualId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 100, name: 'Коммент', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
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
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Кол-во ЛПУ', field: 'LPUcnt', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Дубль Pointid', field: 'DoubleMinPoint', filter: { condition: uiGridCustomService.numberCondition } },

        

    ];


    function LPUPoint_onSelectionChanged(row) {
        if (row !== undefined) {

            var json = JSON.stringify({ PointId: row.PointId })

            $scope.classifierLoading =
                $scope.dataLoading = $http({
                    method: 'POST',
                    url: '/LPU/LoadLPUByPointId/',
                    data: json
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        $scope.GridLPU.Options.data = data.Data;
                    } else {
                        messageBoxService.showError(data.ErrorMessage);
                    }

                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });

        }
    };

    //LPU

    $scope.GridLPU = uiGridCustomService.createGridClassMod($scope, 'LPU_Grid');
    $scope.GridLPU.Options.showGridFooter = true;
    $scope.GridLPU.Options.multiSelect = true;
    $scope.GridLPU.Options.modifierKeysToMultiSelect = true;

    $scope.GridLPU.Options.columnDefs = [
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'LPUId', field: 'LPUId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'ActualId', field: 'ActualId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'OrganizationId', field: 'OrganizationId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'PointId', field: 'PointId', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Коммент', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 300, name: 'ИНН юр. лица', field: 'EntityINN', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Organization?inn={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { headerTooltip: true, enableCellEdit: false, width: 300, name: 'ОГРН юр. лица', field: 'EntityOGRN', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 300, name: 'Юр. лицо', field: 'EntityName', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, name: 'Адрес из лицензии', width: 300, field: 'Address', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, name: 'ФО', width: 100, field: 'Bricks_FederalDistrict', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'СФ', field: 'Address_region', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, name: 'МР/ГО', width: 100, field: 'Bricks_City', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Индекс', field: 'Address_index', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'НП', field: 'Address_city', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, cellTooltip: true, width: 100, name: 'Адрес', field: 'Address_street', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Ориентир', field: 'Address_comment', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Этаж', field: 'Address_float', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: '№ пом.', field: 'Address_room', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'S пом., кв. м', field: 'Address_room_area', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, width: 100, name: 'Тип НП', field: 'Bricks_CityType', headerCellClass: 'LightCyan', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Координаты', field: 'Address_koor', headerCellClass: 'Yellow', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Брик', field: 'BricksId', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GS/Bricks?ids={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { headerTooltip: true, enableCellEdit: false, name: 'Дата Добавления', width: 100, field: 'Date_Create', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
        { headerTooltip: true, enableCellEdit: false, name: 'Лицензия', width: 100, field: 'licencesNumber', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Телефон', field: 'Phone', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'Web', field: 'Website', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, enableCellEdit: false, width: 100, name: 'ФИО зав. аптеки', field: 'ContactPersonFullname', filter: { condition: uiGridCustomService.condition } },
    ];

    $scope.Search = function () {

        var json = JSON.stringify($scope.filter);

        $scope.classifierLoading =
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/LPUPoint/LoadLPUPoint/',
                data: json
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.GridLPUPoint.Options.data = data.Data;
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.Save = function () {
        var array_upd = $scope.GridLPUPoint.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/LPUPoint/Save/',
                    data: JSON.stringify({ lpupointmodels: array_upd })
                }).then(function (response) {
                    if (response.data.Success) {
                        $scope.GridLPUPoint.ClearModify();
                        alert("Сохранил");
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };

    $scope.FromExcel = function (files) {     

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/LPUPoint/FromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
            }).then(function (response) {
                $scope.Search();
            }, function (response) {
                messageBoxService.showError(JSON.stringify(response));
            });
        }
    };

    $scope.ToExcel = function () {
        var array_upd = [];

        $scope.GridLPUPoint.Options.data.forEach(function (item, i, arr) {
            array_upd.push(item.PointId);
        });

        $scope.dataLoading = $scope.objectsLoading = $http({
            method: 'POST',
            url: '/LPUPoint/ToExcel/',
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


    $scope.Merge = function () {

        messageBoxService.showConfirm('Объединить Point?', 'Объединение')
            .then(
                function () {
                    var array_LPUPointID = [];
                    var selectedRows = $scope.GridLPUPoint.selectedRows();
                    if (selectedRows.length >= 5) {
                        alert("Выделено больше 4 строк, я так не буду объединять.");
                        return;
                    }
                    selectedRows.forEach(function (item) {
                        array_LPUPointID.push(item.PointId);
                    });
                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/LPUPoint/Merge/',
                            data: JSON.stringify({ LPUPointIds: array_LPUPointID })
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

   

}