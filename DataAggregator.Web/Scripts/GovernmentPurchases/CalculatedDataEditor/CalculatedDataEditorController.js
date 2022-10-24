angular
    .module('DataAggregatorModule')
    .controller('CalculatedDataEditorController', ['messageBoxService', '$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', CalculatedDataEditorController]);

function CalculatedDataEditorController(messageBoxService, $scope, $http, $uibModal, uiGridCustomService, formatConstants) {
    $scope.filter = {
        PurchaseId: null,
        ClassifierId : null,
        DrugId: null,
        ManufacturerList: null,
        OwnerTradeMark: null,
        OwnerTradeMarkId: null,
        Packer: null,
        PackerId: null,
        PurchaseNumber: null,
        INNGroup: null,
        PurchaseDateBeginStart: new dateClass(),
        PurchaseDateBeginEnd: new dateClass(),
        DrugTradeName: null,
        Category: {
            selectedItems: [],
            displayValue: '',
            availableItems: null
        },
        Nature: {
            selectedItems: [],
            displayValue: '',
            availableItems: null
        },
        DrugDescription: null,
        FederalDistrict: {
            selectedItems: [],
            displayValue: '',
            availableItems: null
        },
        FederationSubject: {
            selectedItems: [],
            displayValue: '',
            availableItems: null
        },
        includePurchases: true,
        includeContracts: true,
        VNC:false
    };

    $scope.filterUsed = false;

    $scope.requestString = '';

    getManufacturerList();
    getCategoryList();
    getNatureList();
    getFederalDistrictList();
    getFederationSubjectList();
    
    function getManufacturerList() {
        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/GetManufacturerList/'
        }).then(function (response) {
            $scope.filter.ManufacturerList = response.data;
        }, function () {
            messageBoxService.showError("Не удалось загрузить список категорий!");
        });
    }

    function getCategoryList() {
        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/GetCategoryList/'
        }).then(function (response) {
            $scope.filter.Category.availableItems = response.data;
        }, function () {
            messageBoxService.showError("Не удалось загрузить список категорий!");
        });
    }

    function getNatureList() {
        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/GetNatureList/'
        }).then(function (response) {
            $scope.filter.Nature.availableItems = response.data;
        }, function () {
            messageBoxService.showError("Не удалось загрузить список характеристик!");
        });
    }

    function getFederalDistrictList() {
        $scope.loading = $http({
            method: 'POST',
            data: { level: 1 },
            url: '/CalculatedDataEditor/GetRegionNames/'
        }).then(function (response) {
            $scope.filter.FederalDistrict.availableItems = response.data;
        }, function () {
            messageBoxService.showError("Не удалось загрузить список характеристик!");
        });
    }
    
    function getFederationSubjectList() {
        $scope.loading = $http({
            method: 'POST',
            data: { level: 2 },
            url: '/CalculatedDataEditor/GetRegionNames/'
        }).then(function (response) {
            $scope.filter.FederationSubject.availableItems = response.data;
        }, function () {
            messageBoxService.showError("Не удалось загрузить список характеристик!");
        });
    }

    $scope.calculatedDataEditorGrid = uiGridCustomService.createGridClass($scope, 'CalculatedDataEditor_Grid');

    $scope.contextData = { divideTo: 1, multipleOn: 1 };

    $scope.calculatedDataEditorGrid.Options.columnDefs = [
        { name: 'Id зак.', field: 'PurchaseId', width: 100, type: 'number', visible: false },
        { name: 'Тип', field: 'ObjectTypeRU', width: 100 },
        { name: 'Номер зак.', field: 'PurchaseNumber', width: 200, filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'закупка', field: 'PurchaseUrl', cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.PurchaseUrl}}" target="_blank">{{row.entity.PurchaseUrl}}</a></div>', width: 100 },
        { name: 'Наименование', field: 'PurchaseName', width: 300, visible: false },
        { name: 'Дата начала', field: 'PurchaseDateBegin', width: 200, type: 'date', cellFilter: formatConstants.FILTER_DATE },

        { name: 'Дата окончания', field: 'PurchaseDateEnd', width: 100, type: 'date', cellFilter: formatConstants.FILTER_DATE, visible: false },
        { name: 'Id органа осущ закупку', field: 'PurchaseCustomerId', width: 100, type: 'number', visible: false },
        { name: 'Сроки поставки', field: 'PurchaseDeliveryTime', width: 100, visible: false },
        { name: 'Id метода', field: 'MethodId', width: 100, type: 'number', visible: false },
        { name: 'Способ опред поставщика', field: 'MethodName', width: 100, visible: false },
        { name: 'Id раздела', field: 'PurchaseClassId', width: 100, type: 'number', visible: false },
        { name: 'Раздел', field: 'PurchaseClassName', width: 100, visible: false },
        { name: 'Id характера', field: 'NatureId', width: 100, type: 'number', visible: false },
        { name: 'Характер', field: 'NatureName', width: 100, visible: false },
        { name: 'Id категории', field: 'CategoryId', width: 100, type: 'number', visible: false },
        { name: 'Категория', field: 'CategoryName', width: 100, visible: false },
        { name: 'Сокращ наимен орг осущ зак', field: 'OrganizationShortName', width: 100, visible: false },
        { name: 'Id типа', field: 'OrganizationTypeId', width: 100, type: 'number', visible: false },
        { name: 'Тип орг осущ зак', field: 'OrganizationTypeName', width: 100, visible: false },

        { name: 'Id лота', field: 'LotId', width: 100, type: 'number', visible: false },
        { name: 'Сумма лота', field: 'LotSum', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Статус лота', field: 'LotStatus', width: 100, visible: false },

        { name: 'Используется для ГС', field: 'UseContractData', enableCellEdit: true, width: 100, type: 'boolean', filter: { condition: uiGridCustomService.booleanConditionX } },
        { name: 'номер Контракта', field: 'ReestrNumber', width: 100, filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?ReestrNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Сумма контракта', field: 'Contract_Sum', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE },
        { name: 'Статус контракта', field: 'ContractStatus', width: 100, visible: false },
        { name: 'контракт', field: 'ContractUrl', cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.ContractUrl}}" target="_blank">{{row.entity.ContractUrl}}</a></div>', width: 100 },

        { name: 'Номер лота', field: 'LotNumber', width: 100, type: 'number', visible: false },
        { name: 'Источник финансирования лота', field: 'LotSourceOfFinancing', width: 100, visible: false },

        { name: 'Id об.', field: 'ObjectReadyId', width: 100, type: 'number', visible: false },
        { name: 'Имя об.', field: 'ObjectReadyName', enableCellEdit: true, width: 300 },
        { name: 'Единицы об.', field: 'ObjectReadyUnit', enableCellEdit: true,  width: 100 },
        { name: 'Кол-во об.', field: 'ObjectReadyAmount', width: 100, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Цена об.', field: 'ObjectReadyPrice', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Сумма об.', field: 'ObjectReadySum', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },


        { name: 'Испр. кол-во об.', field: 'ObjectReadyAmountCorrected', enableCellEdit: true, width: 100, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Испр. цена об.', field: 'ObjectReadyPriceCorrected', enableCellEdit: true, width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Испр. сумма об.', field: 'ObjectReadySumCorrected', enableCellEdit: true, width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Торг. наимен.', field: 'DrugTradeName', width: 150 },
        { name: 'Опис.', field: 'DrugDescription', width: 300 },

        { name: 'Id расч.', field: 'ObjectCalculatedId', width: 100, visible: false },
        { name: 'Кол-во расч.', field: 'ObjectCalculatedAmount', width: 100, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Цена расч.', field: 'ObjectCalculatedPrice', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Сумма расч.', field: 'ObjectCalculatedSum', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Ср. цена по ФО', field: 'FDAveragePrice', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Коэф. ср. цены', field: 'FDCoefficient', width: 100, type: 'number', cellFilter: formatConstants.FILTER_COEFFICIENT, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Коэф.', field: 'ObjectCalculatedCoefficient', width: 100, type: 'number', cellFilter: formatConstants.FILTER_COEFFICIENT, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'ВНЦ', field: 'VNC', width: 100, enableCellEdit: true, type:'boolean'},
        {
            name: 'отклонение', field: 'kof_otkl', width: 100, type: 'number', cellFilter: formatConstants.FILTER_COEFFICIENT, filter: { condition: uiGridCustomService.numberCondition },
            cellTemplate: '<div class="ui-grid-cell-contents" ng-class="{\'back_Red\' : row.entity.kof_otkl<0}">{{COL_FIELD}}</div>'
        },
        { name: 'Зарег.цена', field: 'PriceClassifier', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Признак ЖНВЛП', field: 'IsVed', width: 100, type: 'boolean', filter: { condition: uiGridCustomService.booleanConditionX } },

        { name: 'Цена по протоколу', field: 'protocolPrice', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },

        { name: 'Восстановленная цена', field: 'ObjectCalculatedRecoveredPrice', width: 100, type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Тип восстановления', field: 'RecoveryType', width: 100 },
        { name: 'Сокращ тип восстановления', field: 'RecoveryTypeShortName', width: 100, visible: false },
        { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'DrugId', field: 'DrugId', width: 100, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Фасовка', field: 'DrugConsumerPackingCount', width: 100 },
        { name: 'Дозировка', field: 'DosageGroupDescription', width: 100 },
        { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', width: 100, type: 'number', visible: false },
        { name: 'Производитель', field: 'OwnerTradeMark', width: 100 },
        //{ name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', width: 100, visible: false },
        { name: 'PackerId', field: 'PackerId', width: 100, type: 'number', visible: false },
        { name: 'Упаковщик', field: 'Packer', width: 100 },
        //{ name: 'PackerId', field: 'PackerId', width: 100, visible: false },
        { name: 'МНН', field: 'InnGroup', width: 100 },
        { name: 'ProvisorAction', field: 'ProvisorAction', width: 100 },

        { name: 'Id получателя', field: 'ReceiverId', width: 100, type: 'number', visible: false },
        { name: 'Сокращ наимен получателя', field: 'ReceiverShortName', width: 100, visible: false },
        { name: 'Id типа получателя', field: 'ReceiverTypeId', width: 100, type: 'number', visible: false },
        { name: 'Тип получателя', field: 'ReceiverTypeName', width: 100, visible: false },
        { name: 'Id региона', field: 'RegionId', width: 100, type: 'number', visible: false },
        { name: 'Федеральный округ', field: 'RegionFederalDistrict', width: 100 },
        { name: 'Субъект федерации', field: 'RegionFederationSubject', width: 100 },
        { name: 'Район', field: 'RegionDistrict', width: 100, visible: false },
        { name: 'Город', field: 'RegionCity', width: 100 },
        { name: 'Код региона', field: 'RegionCode', width: 100, visible: false },
        { name: 'kofPriceGZotkl', field: 'kofPriceGZotkl', filter: { condition: uiGridCustomService.condition }, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT }
    ];

    $scope.calculatedDataEditorGrid.Options.multiSelect = true;
    $scope.calculatedDataEditorGrid.Options.noUnselect = false;
    $scope.calculatedDataEditorGrid.Options.showGridFooter = true;
    $scope.calculatedDataEditorGrid.Options.rowTemplate = '<div ng-class="{\'wrongCoefficient\' : row.entity.FDCoefficient >= row.entity.kofPriceGZotkl || row.entity.FDCoefficient <= 1.0/row.entity.kofPriceGZotkl,\'Green4333\':(row.entity.ObjectType == \'Purchase\' && row.entity.LotSum<row.entity.ObjectCalculatedSum ) || (row.entity.ObjectType == \'Contract\' && row.entity.Contract_Sum<row.entity.ObjectCalculatedSum ), \'purchase\' : row.entity.ObjectType == \'Purchase\' && !row.isSelected, \'purchaseSelected\' : row.entity.ObjectType == \'Purchase\' && row.isSelected,  \'contract\':row.entity.ObjectType == \'Contract\' && !row.isSelected, \'contractSelected\':row.entity.ObjectType == \'Contract\' && row.isSelected}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name"  class="ui-grid-cell" ui-grid-cell></div></div>';

    $scope.calculatedDataEditorGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.calculatedDataEditorGridApi = gridApi;

        gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
            if (colDef.field === 'ObjectReadyName' || colDef.field === 'ObjectReadyUnit') {
                if (oldValue !== newValue) {
                    var setCorrectedToNull = false;
                    if (rowEntity.ObjectReadyAmountCorrected !== null || rowEntity.ObjectReadyPriceCorrected !== null || rowEntity.ObjectReadySumCorrected !== null) {
                        messageBoxService.showConfirm('Удалить исправленные значения количества, суммы и цены?')
                            .then(
                                function() { //yes
                                    var scopeValue = $scope.calculatedDataEditorGrid.Options.data.find(function(item) {
                                        return item.ObjectType === rowEntity.ObjectType && item.ObjectReadyId === rowEntity.ObjectReadyId;
                                    });

                                    scopeValue.ObjectReadyAmountCorrected = null;
                                    scopeValue.ObjectReadyPriceCorrected = null;
                                    scopeValue.ObjectReadySumCorrected = null;
                                    setCorrectedToNull = true;
                                },
                                function() { //no
                                })
                            .finally(function() {
                                $scope.changeObjectReadyName(rowEntity.ObjectType, rowEntity.ObjectReadyId, rowEntity.ObjectReadyName, rowEntity.ObjectReadyUnit, setCorrectedToNull);
                            });
                    } else {
                        $scope.changeObjectReadyName(rowEntity.ObjectType, rowEntity.ObjectReadyId, rowEntity.ObjectReadyName, rowEntity.ObjectReadyUnit, setCorrectedToNull);
                    }
                }
            } else { //изменили ObjectReadyAmountCorrected, или ObjectReadyPriceCorrected, или ObjectReadySumCorrected
                $scope.saveCalculatedObject(rowEntity.ObjectType, rowEntity.PurchaseId, rowEntity.ObjectReadyId, rowEntity.ObjectReadyAmountCorrected, rowEntity.ObjectReadyPriceCorrected, rowEntity.ObjectReadySumCorrected, rowEntity.VNC, rowEntity.UseContractData);//, rowEntity.ObjectReadyName, false);
            }
        });
    };

    $scope.contextMenuTextBoxClick = function($event) {
        $event.stopPropagation();
    };

    $scope.getData = function () {
        $scope.filterUsed = false;
        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/GetData',
            data: {
                request: $scope.requestString
            }
        }).then(function(response) {
            $scope.calculatedDataEditorGrid.Options.data = response.data;
        }, function() {
            $scope.calculatedDataEditorGrid.Options.data = [];
            messageBoxService.showError('Ошибка! Не удалось загрузить данные.');
        });
    };

    $scope.getDataByFilter = function () {
        $scope.filterUsed = true;
        $scope.filter.PurchaseDateBeginStartValue = $scope.filter.PurchaseDateBeginStart.Value;
        $scope.filter.PurchaseDateBeginEndValue = $scope.filter.PurchaseDateBeginEnd.Value;
        $scope.filter.SelectedNatureIds = $scope.filter.Nature.selectedItems.map(function (nature) { return nature.Id });
        $scope.filter.SelectedCategoryIds = $scope.filter.Category.selectedItems.map(function (category) { return category.Id });
        $scope.filter.SelectedCategoryIds = $scope.filter.Category.selectedItems.map(function (category) { return category.Id });
        $scope.filter.SelectedFederalDistrictNames = $scope.filter.FederalDistrict.selectedItems.map(function (fd) { return fd.displayValue });
        $scope.filter.SelectedFederationSubjectNames = $scope.filter.FederationSubject.selectedItems.map(function (fs) { return fs.displayValue });

        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/GetDataByFilter',
            data: {
                filter: $scope.filter
            }
        }).then(function (response) {
            $scope.calculatedDataEditorGrid.Options.data = response.data;
            if (response.data.length === 10000) {
                messageBoxService.showInfo("Показано 10 000 записей! Возможно, это не все данные.");
            }
        }, function () {
            $scope.calculatedDataEditorGrid.Options.data = [];
            messageBoxService.showError('Ошибка! Не удалось загрузить данные.');
        });
    };
    $scope.recalculateObjectsEI = function () {
        var lotIds = $scope.calculatedDataEditorGrid.Options.data.map(function (object) {
            return object.LotId;
        });
        if (lotIds.length === 0)
            return;

        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/RecalculateObjectsEI',
            data: JSON.stringify({ lotIds: lotIds })
        }).then(function () {
            $scope.getData();
        }, function () {
            messageBoxService.showError('Ошибка! Не удалось пересчитать.');
        });
    };

    $scope.recalculateObjects = function () {
        var lotIds = $scope.calculatedDataEditorGrid.Options.data.map(function(object) {
            return object.LotId;
        });
        if (lotIds.length === 0)
            return;

        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/RecalculateObjects',
            data: JSON.stringify({ lotIds: lotIds })
        }).then(function () {
            $scope.getData();
        }, function () {
            messageBoxService.showError('Ошибка! Не удалось пересчитать.');
        });
    };

    $scope.saveCalculatedObject = function (objectType, PurchaseId, readyId, amount, price, sum, VNC, UseContractData) {
        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/SaveCalculatedObject',
            data: { objectType: objectType, PurchaseId: PurchaseId, readyId: readyId, amount: amount === 0 ? null : amount, price: price === 0 ? null : price, sum: sum === 0 ? null : sum, VNC: VNC, UseContractData: UseContractData }
        }).then(function() {

        }, function() {
            messageBoxService.showError('Ошибка! Не удалось сохранить.');
        });
    };

    $scope.openFilterDialog = function() {
        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/CalculatedDataEditor/_CalculatedDataFilterView.html',
            controller: 'CalculatedDataFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: {
                    filter: $scope.filter,
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                $scope.filter = data.filter;
                $scope.getDataByFilter();
            },
            // cancel
            function () {
            }
        );
    };

    $scope.ClearObjectReadyCorrected = function () {
        var selectedRows = $scope.calculatedDataEditorGridApi.selection.getSelectedRows();

        //ObjectReadyAmountCorrected
       // ObjectReadyPriceCorrected
       // ObjectReadySumCorrected
        var idsToEdit = selectedRows.map(function (value) {
            return { ObjectType: value.ObjectType, ObjectId: value.ObjectReadyId, DiviedTo: value.DrugConsumerPackingCount };
        });

        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/ClearObjectReadyCorrected',
            data: { idsToDivide: idsToEdit }
        }).then(function () {
            if ($scope.filterUsed) {
                $scope.getDataByFilter();
            } else {
                $scope.getData();
            }
        }, function () {
            messageBoxService.showError('Ошибка! Не удалось сохранить.');
            });
    };

    $scope.dividePackaging = function() {
        var selectedRows = $scope.calculatedDataEditorGridApi.selection.getSelectedRows();
        
        selectedRows = selectedRows.filter(function(item) {
            return item.DrugConsumerPackingCount !== null;
        });
       
        var idsToEdit = selectedRows.map(function (value) {
            return { ObjectType: value.ObjectType, ObjectId: value.ObjectReadyId, DiviedTo: value.DrugConsumerPackingCount };
        });

        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/DividePackaging',
            data: { idsToDivide: idsToEdit}
        }).then(function () {
            if ($scope.filterUsed) {
                $scope.getDataByFilter();
            } else {
                $scope.getData();
            }
        }, function () {
            messageBoxService.showError('Ошибка! Не удалось сохранить.');
        });
    };

    $scope.divide = function () {
        var selectedRows = $scope.calculatedDataEditorGridApi.selection.getSelectedRows();
        var idsToEdit = selectedRows.map(function (value) {
            return { ObjectType : value.ObjectType, ObjectId : value.ObjectReadyId };
        });
        
        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/Divide',
            data: { idsToDivide: idsToEdit, divider: $scope.contextData.divideTo }
        }).then(function () {
            if ($scope.filterUsed) {
                $scope.getDataByFilter();
            } else {
                $scope.getData();
            }
        }, function () {
            messageBoxService.showError('Ошибка! Не удалось сохранить.');
        });
    };

    $scope.multiple = function () {
        var selectedRows = $scope.calculatedDataEditorGridApi.selection.getSelectedRows();
        var idsToEdit = selectedRows.map(function (value) {
            return { ObjectType: value.ObjectType, ObjectId: value.ObjectReadyId };
        });
        
        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/Multiple',
            data: { idsToMultiple: idsToEdit, multiplier: $scope.contextData.multipleOn }
        }).then(function () {
            if ($scope.filterUsed) {
                $scope.getDataByFilter();
            } else {
                $scope.getData();
            }
        }, function () {
            messageBoxService.showError('Ошибка! Не удалось сохранить.');
        });
    };

    $scope.changeObjectReadyName = function (objectType, readyId, objectReadyName, objectReadyUnit, setCorrectedToNull) {
        $scope.loading = $http({
            method: 'POST',
            url: '/CalculatedDataEditor/ChangeObjectReadyName',
            data: {
                objectType: objectType,
                readyId: readyId,
                objectReadyName: objectReadyName,
                ObjectReadyUnit: objectReadyUnit,
                setCorrectedToNull: setCorrectedToNull
            }
        }).then(function () {

        }, function () {
            messageBoxService.showError('Ошибка! Не удалось сохранить.');
        });
    };

    $scope.clearAvgPrice = function () {
        var selectedRows = $scope.calculatedDataEditorGridApi.selection.getSelectedRows();

       selectedRows.forEach(function (item, i, arr) {
            $scope.loading = $http({
                method: 'POST',
                url: '/CalculatedDataEditor/clearAvgPrice',
                data: { ClassifierId: item.ClassifierId}
            }).then(function () {

            }, function () {
                messageBoxService.showError('Ошибка! Не удалось сохранить.');
            });
        });

    };
}