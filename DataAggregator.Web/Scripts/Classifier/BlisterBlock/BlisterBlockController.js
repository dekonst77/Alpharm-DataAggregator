angular
    .module('DataAggregatorModule')
    .controller('BlisterBlockController', ['$scope', '$http', '$q', '$cacheFactory', '$timeout', 'userService', 'uiGridCustomService', 'errorHandlerService', 'messageBoxService', 'uiGridConstants', 'formatConstants', BlisterBlockController]);

function BlisterBlockController($scope, $http, $q, $cacheFactory, $timeout, userService, uiGridCustomService, errorHandlerService, messageBoxService, uiGridConstants, formatConstants) {
    $scope.user = userService.getUser();
    var origdata = [];

    // глубокое копирование массива
    function deepClone(obj) {
        const clObj = {};
        for (const i in obj) {
            if (obj[i] instanceof Object) {
                clObj[i] = deepClone(obj[i]);
                continue;
            }
            clObj[i] = obj[i];
        }
        return clObj;
    }

    $scope.BlisterBlock_Init = function () {

        //******** Grid ******** ->
        $scope.Grid_BlisterBlock = uiGridCustomService.createGridClassMod($scope, 'Grid_BlisterBlock');
        $scope.Grid_BlisterBlock.Options.showGridFooter = true;
        $scope.Grid_BlisterBlock.Options.multiSelect = true;
        $scope.Grid_BlisterBlock.Options.enableFiltering = true;
        $scope.Grid_BlisterBlock.Options.enableSelectAll = true;
        $scope.Grid_BlisterBlock.Options.modifierKeysToMultiSelect = true;
        $scope.Grid_BlisterBlock.Options.flatEntityAccess = true;
        $scope.Grid_BlisterBlock.Options.enableGridMenu = true;

        let cellTemplateHint = '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'

        $scope.packing_array = null;
        $scope.packing_selected = null;

        $scope.Grid_BlisterBlock.Options.columnDefs = [
            { headerTooltip: true, name: 'OFD Sum последний месяц', enableCellEdit: false, width: 130, cellTooltip: true, field: 'OFD_Sum_LastMonth', type: 'number', cellFilter: formatConstants.FILTER_PRICE },
            { headerTooltip: true, name: 'Audit Sum последний месяц', enableCellEdit: false, width: 130, cellTooltip: true, field: 'Audit_Sum_LastMonth', type: 'number', cellFilter: formatConstants.FILTER_PRICE },
            { headerTooltip: true, name: 'Classifier Id', enableCellEdit: false, width: 130, cellTooltip: true, field: 'ClassifierId', type: 'number', cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'ClassifierPackingId', enableCellEdit: false, width: 200, cellTooltip: true, field: 'ClassifierPackingId', type: 'number', visible: false, nullable: true, filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: cellTemplateHint },

            {
                name: 'CountPrimaryPacking', displayName: 'Коэффициент деления',
                editableCellTemplate: 'ui-grid/dropdownEditor',
                width: '150',
                field: 'CountPrimaryPacking',
                enableCellEdit: true,
                cellTemplate: cellTemplateHint,

                editDropdownIdLabel: 'label',
                editDropdownValueLabel: 'label',
                //editDropdownOptionsArray: [
                //    { value: 1, label: 'male' },
                //    { value: 2, label: 'female' },
                //    { value: 66, label: '556' }
                //],
                editDropdownOptionsFunction: function (rowEntity, colDef) {

                    //console.log(rowEntity);
                    //console.log(colDef);

                    //var res = [{ value: 1, label: 4 }, { value: 2, label: 5 }]
                    //return res;                    

                    $scope.dataLoading = $http({
                        method: 'POST',
                        url: '/BlisterBlock/GetPrimaryPacking/',
                        data: { ClassifierId: rowEntity.ClassifierId }
                    }).then(function (response) {
                        //console.log(response);

                        if (response.status === 200) {
                            //console.log(response.data.Data);

                            $scope.packing_array = response.data.Data;

                            return response.data.Data;
                        }
                        else {
                            console.log(response);
                            return false;
                        }

                    }, function (response) {
                        console.log('errorHandlerService.showResponseError = ' + response);
                        errorHandlerService.showResponseError(response);
                    });
                    return $scope.dataLoading;
                }
            },
            { headerTooltip: true, name: 'Существует ли на рынке', enableCellEdit: true, width: 100, field: 'IsExist', type: 'boolean' },
            { headerTooltip: true, name: 'Номер РУ', enableCellEdit: false, width: 200, field: 'Number', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Drug Id', enableCellEdit: false, width: 130, cellTooltip: true, field: 'DrugId', type: 'number', cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Trade Name', enableCellEdit: false, width: 250, cellTooltip: true, field: 'Trade_Name', cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Drug Description', enableCellEdit: false, cellTooltip: true, width: 300, field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'INN Group', cellTooltip: true, width: 300, enableCellEdit: false, field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'OwnerTradeMarkId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'OwnerTradeMarkId', type: 'number', visible: false, nullable: true, filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'Правообладатель', enableCellEdit: false, width: 300, field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint },
            { headerTooltip: true, name: 'PackerId', field: 'PackerId', filter: { condition: uiGridCustomService.condition } },
            { headerTooltip: true, name: 'Packer', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
            { name: 'Used', field: 'Used', type: 'boolean', enableCellEdit: userService.isInRole("SBoss") },
            { name: 'IsOther', field: 'IsOther', type: 'boolean' },
            { name: 'Комментарий', enableCellEdit: true, field: 'Comment', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.Grid_BlisterBlock.SetDefaults();

        $scope.Grid_BlisterBlock.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.edit.on.afterCellEdit($scope, editRowDataSource);
        };
        /*
        function editRowDataSource(rowEntity, colDef, newValue, oldValue) {

            // проверка на изменение
            if (newValue === oldValue || newValue === undefined)
                return;

            var json = null;
            switch (colDef.field) {
                case 'CountPrimaryPacking':
                    $scope.packing_selected = $scope.packing_array.find(function (element, index, array) {
                        return element.label == newValue;
                    });

                    json = { ClassifierId: rowEntity.ClassifierId, FieldName: "ClassifierPackingId", newValue: $scope.packing_selected.value };
                    //console.log(json);
                    break;

                case 'Comment':
                    json = { ClassifierId: rowEntity.ClassifierId, FieldName: "Comment", newValue: newValue };
                    break;

                case 'IsExist':
                    json = { ClassifierId: rowEntity.ClassifierId, FieldName: "IsExist", newValue: newValue };
                    break;
                default: return;
            }

            $scope.dataLoading = $http({
                method: "POST",
                url: "/BlisterBlock/SaveField/",
                data: JSON.stringify(json)
            }).then(function (response) {
                var data = response.data.Data;

                if (data.Success) {
                    let BlisterBlockRecord = data.Data.BlisterBlockRecord;

                    console.log(BlisterBlockRecord);

                    rowEntity.ClassifierPackingId = BlisterBlockRecord.ClassifierPackingId;
                    rowEntity.CountPrimaryPacking = BlisterBlockRecord.CountPrimaryPacking;
                    rowEntity.IsExist = BlisterBlockRecord.IsExist;
                    rowEntity.Comment = BlisterBlockRecord.Comment;

                    console.log(rowEntity);

                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
                return true;
            }, function (response) {
                rowEntity[colDef.field] = oldValue;
                errorHandlerService.showResponseError(response);
                return false;
            });
        }
        */
        function editRowDataSource(rowEntity, colDef, newValue, oldValue) {

            if (colDef.field !== '@modify') {
                if (newValue !== oldValue) {
                    var deepClone = JSON.parse(JSON.stringify(rowEntity));
                    deepClone[colDef.field] = oldValue;
                    $scope.Grid_BlisterBlock.GridLogger(deepClone);
                    rowEntity["@modify"] = true;
                    $scope.Grid_BlisterBlock.NeedSave = true;
                }
            }

            // проверка на изменение
            if (newValue === oldValue || newValue === undefined)
                return;

            if (colDef.field === 'CountPrimaryPacking') {
                $scope.packing_selected = $scope.packing_array.find(function (element, index, array) {
                    return element.label == newValue;
                });
                rowEntity.ClassifierPackingId = $scope.packing_selected.value;
            }
        }
        //******** Grid ******** <-

        var json_str = null;
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/BlisterBlock/BlisterBlockView/',
            data: json_str
        }).then(function (response) {
            var data = response.data;

            if (data.Success) {
                $scope.Grid_BlisterBlock.Options.data = data.Data.BlisterBlock;

                $scope.Grid_BlisterBlock.Options.data.forEach((value, index) => {
                    origdata[index] = deepClone(value);
                });
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }

        }, function (response) {
            console.log(response);

            /*
            if (response.status in [401, 404]) {
                messageBoxService.showError("У вас нет доступа к данному ресурсу. Обратитесь к администратору системы.");

                return
            }
            */
            messageBoxService.showError(response.data);
        }).catch(error => alert(error.message));
    }

    $scope.BlisterBlock_Save = function () {
        var array_upd = $scope.Grid_BlisterBlock.GetArrayModify();

        console.log(array_upd);

        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/BlisterBlock/Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {

                    if (response.status === 200) {
                        console.log('успешно записаны данные в БД');

                        $scope.Grid_BlisterBlock.ClearModify();

                        var data = response.data.Data.Data.BlisterBlockRecord;
                        //console.log(data);

                        // корректируем оригинал
                        data.forEach(el => {
                            var index = origdata.findIndex(item => item.ClassifierId === el.ClassifierId);
                            origdata.splice(index, 1, el);
                        });

                        //alert("Сохранено");
                        messageBoxService.showInfo("Сохранено записей: " + data.length);
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };

    $scope.BlisterBlock_Undo = function () {
        var array_upd = $scope.Grid_BlisterBlock.GetArrayModify(); // изменённые данные
        //console.log(array_upd);

        var griddata = $scope.Grid_BlisterBlock.Options.data; // grid данные       

        // корректируем grid данные
        array_upd.forEach(el => {
            var gridindex = griddata.findIndex(item => item.ClassifierId === el.ClassifierId);

            var origindex = origdata.findIndex(item => item.ClassifierId === el.ClassifierId);
            //console.log(origdata[origindex]);

            griddata.splice(gridindex, 1, deepClone(origdata[origindex]));
        });

        $scope.Grid_BlisterBlock.ClearModify();
    }

    $scope.$on('$destroy', function () {
        if ($scope.Grid_BlisterBlock.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранённые результаты в Блоке блистеровки. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.BlisterBlock_Save();
                    });
        }
    });

}