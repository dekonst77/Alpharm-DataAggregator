angular
    .module('DataAggregatorModule')
    .controller('FulfilmentController', ['messageBoxService', '$scope', '$http', '$timeout', 'uiGridCustomService', 'userService', 'uiGridConstants', 'formatConstants', 'errorHandlerService', FulfilmentController]);

function FulfilmentController(messageBoxService, $scope, $http, $timeout, uiGridCustomService, userService, uiGridConstants, formatConstants, errorHandlerService) {

    $scope.message = 'Пожалуйста, ожидайте. Загрузка данных...';
    var origdata = [];
    $scope.user = userService.getUser();
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

    // Грид с Fulfilment
    $scope.fulfilmentLoad = function () {
        fulfilmentLoad();
    }

    function fulfilmentLoad(filter) {
        $scope.fulfilmentLoading = $http({
            method: 'POST',
            url: '/Fulfilment/LoadFulfilmentList/',
            data: {
                ReestrNumber: filter.reestrNumber
            }
        }).then(function (response) {
            let data = response.data;
            $scope.fulfilmentGrid.Options.data = data;
            $scope.fulfilmentGrid.Options.data.forEach((value, index) => {
                origdata[index] = deepClone(value);
            });
            $scope.fulfilmentGrid.NeedSave = false;
        }, function () {
            $scope.fulfilmentGrid.Options.data = [];
            $scope.message = 'Unexpected Error';
            messageBoxService.showError($scope.message);
        });
    };

    function fulfilment_search(filter) {
        if ($scope.fulfilmentGrid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранённые результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        fulfilmentGrid_Save();
                    },
                    function (result) {
                        if (result === 'no') {
                            fulfilmentLoad(filter);
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            fulfilmentLoad(filter);
        }
    };

    $scope.objectChange = function (objectName) {
        $scope.fulfilmentGrid.Options.data = [];
    }

    var booleanCellTemplate = uiGridCustomService.getBooleanCellTemplate();
    var booleanCondition = uiGridCustomService.booleanCondition;

    $scope.fulfilmentGrid = uiGridCustomService.createGridClassMod($scope, 'Fulfilmen_Grid');
    $scope.fulfilmentGrid.Options.showGridFooter = true;
    $scope.fulfilmentGrid.Options.enableSorting = true,

        $scope.fulfilmentGrid.Options.multiSelect = false;
    $scope.fulfilmentGrid.Options.noUnselect = false;

    $scope.fulfilmentGrid.Options.enableFiltering = true;
    $scope.fulfilmentGrid.Options.enableSelectAll = false;
    $scope.fulfilmentGrid.Options.modifierKeysToMultiSelect = true;
    $scope.fulfilmentGrid.Options.flatEntityAccess = true;
    $scope.fulfilmentGrid.Options.enableGridMenu = true;



    $scope.fulfilmentGrid.Options.columnDefs = [
        { name: 'Тип', field: 'Type', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Номер закупки', field: 'Number', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Номер контракта', field: 'ReestrNumber', filter: { condition: uiGridCustomService.conditionSpace } },

        { name: 'Сумма контракта', field: 'ContractSum', enableHiding: false, filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE },

        { name: 'Фактически оплачено', field: 'ActuallyPaid', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Сумма исполнения', field: 'SumIsp', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Id об', field: 'ObjectId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true },
        { name: 'Имя об', field: 'Name', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Единица об', field: 'Unit', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Кол-во об', field: 'Amount', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Цена об', field: 'Price', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Сумма об', field: 'Sum', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE },
        {
            name: 'Classifier Id', field: 'ClassifierId', filter: { condition: uiGridCustomService.conditionSpace }, type: 'number', enableCellEdit: true, cellEditableCondition: function ($scope) { return $scope.row.entity.Type == 'Исполнение' },
            cellTemplate: '<div class="ui-grid-cell-contents" title="{{grid.appScope.formatChanging(row.entity, row.entity.ClassifierId, row.entity.oClassifierId)}}"><span ng-bind-html="grid.appScope.formatTheContent(row.entity, row.entity.ClassifierId, row.entity.oClassifierId)"></span></div>'
        },

        { name: 'МНН', field: 'INNGroup', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Торг наимен', field: 'TradeName', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Описание', field: 'DrugDescription', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Производитель', field: 'Corporation', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Упаковщик', field: 'Packer', filter: { condition: uiGridCustomService.conditionSpace } },

        {
            name: 'Кол-во расч', field: 'ObjectCalculatedAmount', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE, enableCellEdit: true, cellEditableCondition: function ($scope) { return $scope.row.entity.Type == 'Исполнение' },
            cellTemplate: '<div class="ui-grid-cell-contents" title="{{grid.appScope.formatChanging(row.entity, row.entity.ObjectCalculatedAmount, row.entity.oObjectCalculatedAmount)}}"><span ng-bind-html="grid.appScope.formatTheContent(row.entity, row.entity.ObjectCalculatedAmount, row.entity.oObjectCalculatedAmount)"></span></div>' },
        {
            name: 'Цена расч', field: 'ObjectCalculatedPrice', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE, enableCellEdit: true, cellEditableCondition: function ($scope) { return $scope.row.entity.Type == 'Исполнение' },
            cellTemplate: '<div class="ui-grid-cell-contents" title="{{grid.appScope.formatChanging(row.entity, row.entity.ObjectCalculatedPrice, row.entity.oObjectCalculatedPrice)}}"><span ng-bind-html="grid.appScope.formatTheContent(row.entity, row.entity.ObjectCalculatedPrice, row.entity.oObjectCalculatedPrice)"></span></div>' },

        {
            name: 'Сумма расч', field: 'ObjectCalculatedSum', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', visible: true, nullable: true, cellFilter: formatConstants.FILTER_PRICE,
            cellTemplate: '<div class="ui-grid-cell-contents" title="{{grid.appScope.formatChangingSumm(row.entity)}}"><span ng-bind-html="grid.appScope.formatTheContentSumm(row.entity)"></span></div>' },

        { name: 'Серия ЛП', field: 'Seria', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'МНН_Исполнение', field: 'INNGroupIsp', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'ТН_Исполнение', field: 'TradeNameIsp', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Описание_Исполнение', field: 'DrugDescriptionIsp', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Provisor Action', field: 'ProvisorAction', filter: { condition: uiGridCustomService.conditionSpace } },

        { name: 'contractQuantityId', field: 'contractQuantityId', type: 'number', visible: false },

        { name: 'uClassifierId', field: 'uClassifierId', type: 'number', visible: false, nullable: true },
        { name: 'uObjectCalculatedAmount', field: 'uObjectCalculatedAmount', type: 'number', visible: false, nullable: true },
        { name: 'uObjectCalculatedPrice', field: 'uObjectCalculatedPrice', type: 'number', visible: false, nullable: true },
        { name: 'uUserGuid', field: 'uUserGuid', type: 'number', visible: false, nullable: true },
        { name: 'uEditDate', field: 'uEditDate', type: 'date', visible: false, nullable: true, cellFilter: 'date:\'yyyy-MM-dd\'' },
        { name: 'uStatus', field: 'uStatus', type: 'number', visible: false, nullable: true },
        { name: 'oClassifierId', field: 'oClassifierId', type: 'number', visible: false, nullable: true },
        { name: 'oObjectCalculatedAmount', field: 'oObjectCalculatedAmount', type: 'number', visible: false, nullable: true },
        { name: 'oObjectCalculatedPrice', field: 'oObjectCalculatedPrice', type: 'number', visible: false, nullable: true }
    ];

    $scope.formatChanging = function (entity, New, Old) {
        if (entity.Type != 'Исполнение' || New === Old) return '';
        if (Old === '') Old = '0'
        var date = new Date(entity.uEditDate);
        return 'Оригинальное значение: ' + Old + '; Изменение: ' + entity.UserName + ' ' + date.toLocaleString("ru");
    };
    $scope.formatTheContent = function (entity, New, Old) {
        if (entity.Type != 'Исполнение' || New === Old) return New;
        return '<strong><em>' + New + '</em></strong>';
    };
    $scope.formatChangingSumm = function (entity) {
        if ((entity.Type != 'Исполнение')
            || (entity.ObjectCalculatedAmount === entity.oObjectCalculatedAmount && entity.ObjectCalculatedPrice === entity.oObjectCalculatedPrice)
            || (entity.ObjectCalculatedSum === entity.ObjectCalculatedAmount * entity.ObjectCalculatedPrice)
        )
            return '';
        var date = new Date(entity.uEditDate);
        return 'Оригинальное значение: ' + entity.ObjectCalculatedSum + '; Изменение: ' + entity.UserName + ' ' + date.toLocaleString("ru");
    };
    $scope.formatTheContentSumm = function (entity) {
        if ((entity.Type != 'Исполнение')
            || (entity.ObjectCalculatedAmount === entity.oObjectCalculatedAmount && entity.ObjectCalculatedPrice === entity.oObjectCalculatedPrice)
            || (entity.ObjectCalculatedSum === entity.ObjectCalculatedAmount * entity.ObjectCalculatedPrice)
        )
            return entity.ObjectCalculatedSum;
        return '<strong><em>' + entity.ObjectCalculatedAmount * entity.ObjectCalculatedPrice + '</em></strong>';
    };

    //Фильтр
    var filterClass = function (loadFunction) {
        this.reestrNumber = "";
        this.reportObject = "Fulfilment";
        this.showReportGrid = true;

        this.fulfilmentLoad = function () {
            if (!this.validate()) {
                return;
            }

            loadFunction(this);
        }
        this.fulfilment_search = function () {
            if (!this.validate()) {
                return;
            }

            fulfilment_search(this);
        }

        this.fulfilmentGrid_Save = function () {
            if (!this.validate()) {
                return;
            }

            fulfilmentGrid_Save(this);
        }

        this.validate = function () {
            if (this.reestrNumber.Value != undefined && this.reestrNumber.Value.length > 100) {
                $scope.message = "User input error";
                messageBoxService.showError("Длина не должна превышать 100 символов!");
                return false;
            }
            return true;
        }

        this.getJson = function () {
            var filterTransfer = {
                ReestrNumber: this.reestrNumber.Value
            }
            return JSON.stringify({
                ReestrNumber: this.reestrNumber.Value
            });
        }
    }

    //Объект фильтр
    $scope.filter = new filterClass(fulfilmentLoad);
    $scope.filter = new filterClass(fulfilment_search);
    $scope.filter = new filterClass(fulfilmentGrid_Save);

    //работа с изменениями
    $scope.fulfilmentGrid.Options.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        gridApi.edit.on.afterCellEdit($scope, editRowDataSource);
    };
    function editRowDataSource(rowEntity, colDef, newValue, oldValue) {
        if (colDef.field !== '@modify') {
            if (newValue !== oldValue) {
                if (colDef.field === 'ClassifierId') {
                    $scope.fulfilmentLoading = $http({
                        method: 'POST',
                        url: '/Fulfilment/CheсkClassifierId/',
                        data: {
                            ClassifierId: newValue
                        }
                    }).then(function (response) {
                        let data = response.data.Data.Ret;
                        let ret = response.data.status;
                        if (ret === '0') {
                            messageBoxService.showInfo(data);
                            var deepClone = JSON.parse(JSON.stringify(rowEntity));
                            deepClone[colDef.field] = oldValue;
                            $scope.fulfilmentGrid.GridLogger(deepClone);
                            rowEntity["@modify"] = true;
                            $scope.fulfilmentGrid.NeedSave = true;
                            rowEntity.uUserGuid = $scope.user.Id;
                            rowEntity.UserName = $scope.user.Fullname;
                            rowEntity.uEditDate = new Date();
                        } else {
                            rowEntity.ClassifierId = oldValue;
                            messageBoxService.showError(data);
                        }
                    }, function () {
                        $scope.fulfilmentGrid.Options.data = [];
                        $scope.message = 'Unexpected Error';
                        messageBoxService.showError($scope.message);
                    });
                } else {
                    messageBoxService.showInfo(data);
                    var deepClone = JSON.parse(JSON.stringify(rowEntity));
                    deepClone[colDef.field] = oldValue;
                    $scope.fulfilmentGrid.GridLogger(deepClone);
                    rowEntity["@modify"] = true;
                    $scope.fulfilmentGrid.NeedSave = true;
                    rowEntity.uUserGuid = $scope.user.Id;
                    rowEntity.UserName = $scope.user.Fullname;
                    rowEntity.uEditDate = new Date();
                }
            }
        }
    }
    function fulfilmentCheсkClassifierId(cId, oldValue) {
        $scope.fulfilmentLoading = $http({
            method: 'POST',
            url: '/Fulfilment/CheсkClassifierId/',
            data: {
                ClassifierId: cId,
                OldValue: oldValue
            }
        }).then(function (response) {
            let data = response.data.Data.Ret;
            let ret = response.data.status;
            if(ret === '0')
                messageBoxService.showInfo(data);
            else
                messageBoxService.showError(data);
        }, function () {
            $scope.fulfilmentGrid.Options.data = [];
            $scope.message = 'Unexpected Error';
            messageBoxService.showError($scope.message);
        });
    };




    $scope.$on('$destroy', function () {
        if ($scope.fulfilmentGrid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранённые результаты в Блоке блистеровки. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        fulfilmentGrid_Save();
                    });
        }
    });


    //$scope.$on(uiGridEditConstants.events.BEGIN_CELL_EDIT, function (data) {
    //    let newValue = data.targetScope.row.entity;
    //});
    //$scope.$on('uiGridEventEndCellEdit', function (data) {
    //    let newValue = data.targetScope.row.entity;
    //});




    function fulfilmentGrid_Save() {
        var array_upd = $scope.fulfilmentGrid.GetArrayModify();

        console.log(array_upd);

        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Fulfilment/Save/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {

                    if (response.status === 200) {
                        console.log('успешно записаны данные в БД');

                        $scope.fulfilmentGrid.ClearModify();
                        $scope.fulfilmentGrid.NeedSave = false;

                        var data = response.data.Data.FulfilmentRecord;
                        //console.log(data);

                        // корректируем оригинал
                        data.forEach(el => {
                            var index = origdata.findIndex(item => item.contractQuantityId === el.contractQuantityId);
                            if(index>=0) origdata.splice(index, 1, el);
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
        var array_upd = $scope.fulfilmentGrid.GetArrayModify(); // изменённые данные
        //console.log(array_upd);

        var griddata = $scope.fulfilmentGrid.Options.data; // grid данные       

        // корректируем grid данные
        array_upd.forEach(el => {
            var gridindex = griddata.findIndex(item => item.contractQuantityId === el.contractQuantityId);

            var origindex = origdata.findIndex(item => item.contractQuantityId === el.contractQuantityId);
            //console.log(origdata[origindex]);

            griddata.splice(gridindex, 1, deepClone(origdata[origindex]));
        });

        $scope.fulfilmentGrid.ClearModify();
    }

}