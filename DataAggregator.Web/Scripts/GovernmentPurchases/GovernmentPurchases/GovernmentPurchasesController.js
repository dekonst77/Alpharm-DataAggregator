angular
    .module('DataAggregatorModule')
    .controller('GovernmentPurchasesController', ['messageBoxService', '$scope', '$route', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', 'userService', 'uiGridConstants', 'errorHandlerService', GovernmentPurchasesController]);

function GovernmentPurchasesController(messageBoxService, $scope, $route, $http, $uibModal, uiGridCustomService, formatConstants, userService, uiGridConstants, errorHandlerService) {

    //для колонок с type='number', в которых значение должно быть >= 0
    var onlyPositiveEditableCellTemplate = "<div><form name=\"inputForm\"><input type=\"INPUT_TYPE\" ng-class=\"'colt' + col.uid\" ui-grid-editor ng-model=\"MODEL_COL_FIELD\" min=\"0\"></form></div>";

    $scope.format = 'dd.MM.yyyy';
    $scope.format2 = 'dd.MM.yyyy HH:mm';
    $scope.user = userService.getUser();

    $scope.selectedPurchaseIsEdited = false;
    $scope.PlanGCount = 'не доступно';
    $scope.selectedPurchase_DeliveryTimeInfo_start = 0;
    
    $scope.CurrentContractId = null;
    $scope.CurrentLotId = null;

    $scope.canGetPurchases = false;
    $scope.canSetPurchases = false;
    $scope.selectedPurchase_set = function (value) {
        $scope.selectedPurchase = value;
        if (value === null)
            $scope.selectedPurchase_Start = null;
        else
            $scope.selectedPurchase_Start = JSON.parse(JSON.stringify(value));
    };
    $scope.selectedPurchase_set(null);
    $scope.selectedLot_set = function (value) {
        $scope.selectedLot = value;
        if (value === null)
            $scope.selectedLot_Start = null;
        else {
            formFundingString();
            $scope.selectedLot_Start = JSON.parse(JSON.stringify(value));
        }
    };
    $scope.selectedLot_set(null);
    $scope.isInRoles = function (roles) {
        return userService.isInRoles(roles);
    }; 

    $scope.isMayEditPurObject = true;
    $scope.MayEditPurObject = function () {
        //http://s-dev1:8080/redmine/issues/471
        //Только для операторов-надомников: закупку и контракт можно редактировать только если в нем нет ни одного объекта, либо он ранее был отредактирован текущим пользователем.
        $scope.isMayEditPurObject = false;
        if (userService.isInRoles("GOperatorRemote")) {
            if ($scope.selectedLot !== null) {
                if ($scope.objectsReadyGrid.Options.data === null || $scope.objectsReadyGrid.Options.data.length === 0) {
                    $scope.isMayEditPurObject = true;
                }

                //if (
                //    ($scope.user.Name !== $scope.selectedLot.LastChangedUser_UserName || $scope.selectedLot.LastChangedUser_UserName === "")
                //    &&
                //    ($scope.user.Name === $scope.selectedLot.LastChangedObjectsUser_UserName || $scope.selectedLot.LastChangedObjectsUser_UserName === "")
                //) {
                //    $scope.isMayEditPurObject = true; return;
                //}

                if ($scope.user.Name === $scope.selectedLot.LastChangedObjectsUser_UserName || $scope.selectedLot.LastChangedObjectsUser_UserName === "")
                {
                    $scope.isMayEditPurObject = true; 
                }
            }
        }
        else {
            $scope.isMayEditPurObject = true;
        }
    };
    $scope.isMayEditContactObj = true;
    $scope.MayEditContactObj = function () {
        //http://s-dev1:8080/redmine/issues/471
        //Только для операторов-надомников: закупку и контракт можно редактировать только если в нем нет ни одного объекта, либо он ранее был отредактирован текущим пользователем.
        $scope.isMayEditContactObj = false;
        if (userService.isInRoles("GOperatorRemote")) {
            if ($scope.selectedContract !== null) {
                if ($scope.contractObjectsReadyGrid.Options.data === null || $scope.contractObjectsReadyGrid.Options.data.length === 0) {
                    $scope.isMayEditContactObj = true;
                }

                if ($scope.user.Name === $scope.selectedContract.LastChangedUser_UserName) {
                    $scope.isMayEditContactObj = true;
                }
            }
        }
        else {
            $scope.isMayEditContactObj = true;
        }
    };
   /* $scope.Purchase_DateBegin_DC = {
        DateOptions: {
            minDate: null
        },
        Opened: false
    };*/
    /*$scope.Purchase_DateEndFirstParts_DC = {
        DateOptions: {
            minDate: null
        },
        Opened: false
    };
    $scope.Purchase_DateEnd_DC = {
        DateOptions: {
            minDate: null
        },
        Opened: false
    };*/
    //==================== Справочники ==========================

    // список классов закупки
    $scope.purchaseClassList = [];

    // список этапов закупки
    $scope.stageList = [];
    $scope.SourceList = [];
    $scope.LawTypeList = [];
    $scope.MethodList = [];

    // список статусов лота
    $scope.lotStatusList = [];
    $scope.ContractKK = [];
   

    $scope.LoadSPR = function () {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadSPR/'
        }).then(function (response) {
            Array.prototype.push.apply($scope.ContractKK, response.data.ContractKK);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    // список категорий закупки
    $scope.categoryList = [];

    // список характеров закупки
    $scope.natureList = [];
    $scope.natureList_L2 = [];

    // список периодов поставки
    $scope.deliveryTimePeriodList = [];

    // список годов финансирования
    $scope.paymentYearList = [];
    
    $scope.ContractStatus = [];
    function ContractStatusList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetContractStatusList/'
        }).then(function (response) {
            $scope.ContractStatus = response.data;
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    ContractStatusList();

    function lotStatusList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetLotStatusList/'
        }).then(function (response) {
            $scope.lotStatusList = response.data;
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    lotStatusList();

    function loadStageList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetStageList/'
        }).then(function (response) {
            $scope.stageList = response.data;
            $scope.stageList.unshift({ Id: null, Name: '' });
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    loadStageList();

    function loadSourceList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetSourceList/'
        }).then(function (response) {
            $scope.SourceList = response.data;
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    loadSourceList();

    function loadLawTypeList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetLawTypeList/'
        }).then(function (response) {
            $scope.LawTypeList = response.data;
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }
    loadLawTypeList();

    function loadMethodList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetMethodList/'
        }).then(function (response) {
            $scope.MethodList = response.data;
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }
    loadMethodList();

    function loadCategoryList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetCategoryList/'
        }).then(function (response) {
            $scope.categoryList = response.data;
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }
    loadCategoryList();

    function loadNatureList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetNatureList/'
        }).then(function (response) {
            $scope.natureList = response.data;
            $scope.natureList.unshift({ Id: null, Name: '' });
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    loadNatureList();

    function loadNatureList_L2() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetNatureList_L2/'
        }).then(function (response) {
            $scope.natureList_L2 = response.data;
            $scope.natureList_L2.unshift({ Id: null, Name: '', Nature_L1Id:null });
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    $scope.natureList_L2_AR = function (filter) {
        var ret = [];
        $scope.natureList_L2.forEach(function (item, i, arr) {
            if (item.Nature_L1Id === null || item.Nature_L1Id === filter) {
                ret.push(item);
            }
        });
        return ret;
    };
    loadNatureList_L2();

    $scope.changeNature = function (fromchangeNature_L2) {
        $scope.selectedPurchase.Category = { Id: null, Name: '' };
        if (fromchangeNature_L2 === undefined) {
            $scope.selectedPurchase.Nature_L2 = { Id: null, Name: '', Nature_L1Id:null};
        }

        $scope.categoryList.forEach(function (item, i, arr) {
            if (item.Id === $scope.selectedPurchase.Nature.CategoryId) {
                $scope.selectedPurchase.Category = item;
            }
        });
    };

    $scope.changeNature_L2 = function () {
        $scope.selectedPurchase.Nature = { Id: null, Name: '', CategoryId:null };

        $scope.natureList.forEach(function (item, i, arr) {
            if (item.Id === $scope.selectedPurchase.Nature_L2.Nature_L1Id) {
                $scope.selectedPurchase.Nature = item;
                $scope.changeNature(1);
            }
        });
    };

    function deliveryTimePeriodList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetDeliveryTimePeriodList/'
        }).then(function (response) {
            $scope.deliveryTimePeriodList = response.data;
            $scope.deliveryTimePeriodList.unshift({ Id: null, Name: '' });
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    deliveryTimePeriodList();

    function loadPaymentYearList() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetPaymentYearList/'
        }).then(function (response) {
            $scope.paymentYearList = response.data;
            $scope.paymentYearList.unshift({ Id: null, Name: '' });
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    loadPaymentYearList();

    $scope.PaymentTypeList = [];
    function loadPaymentTypeList() {

        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetPaymentTypeList/'
        }).then(function (response) {
            $scope.PaymentTypeList = response.data;
            $scope.PaymentTypeList.unshift({ Id: null, Name: '' });
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    loadPaymentTypeList();

    //==================== Закупки в работе =====================

    $scope.purchasesInWorkLoading = null;

    $scope.purchasesGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_PurchasesGrid');
    $scope.purchasesGrid.Options.rowTemplate = '<div ng-class="{\'IsPriceByPart\' : row.entity[\'IsPriceByPart\']==true}"><div ng-repeat="col in colContainer.renderedColumns track by col.colDef.name" ng-class="{selected : row.isSelected}" class="ui-grid-cell" ui-grid-cell></div></div>';

    $scope.purchasesGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Номер Закупки', field: 'Number', filter: { condition: uiGridCustomService.condition } },
        { name: 'Наименование объекта закупки', field: 'Name', enableHiding: false, filter: { condition: uiGridCustomService.condition } },
        {
            name: 'Ссылка', field: 'URL', filter: { condition: uiGridCustomService.condition },
            cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.URL}}" target="_blank">{{COL_FIELD}}</a></div>'
        },
        { name: 'Признак на выигрыш цены', field: 'IsPriceByPart', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' }
    ];

    $scope.purchasesGrid.Options.multiSelect = false;
    $scope.purchasesGrid.Options.noUnselect = true;
    $scope.purchasesGrid.Options.modifierKeysToMultiSelect = false;
    $scope.purchasesGrid.Options.showGridFooter = true;
    //$scope.purchasesGrid.Options.enableGridMenu = false;
    

    $scope.purchasesGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.purchasesGridApi = gridApi;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, selectPurchase);
        gridApi.selection.on.rowSelectionChangedBatch($scope, selectPurchase);
    };
    $scope.selectPurchase_id = 0;
    function selectPurchase(row) {
        var selectedListId = $scope.purchasesGridApi.selection.getSelectedRows().map(function (value) {
            return value.Id;
        });

        // обрабатываем только если выбрана 1 закупка
        if (selectedListId.length !== 1 || selectedListId[0] === undefined) {
            return;
        }

        if ($scope.selectPurchase_id === selectedListId[0])
            return;
        var canSelect = !$scope.selectedPurchaseIsEdited && !$scope.selectedLotIsEdited && !$scope.objectsIsEdited && !$scope.selectedContractIsEdited && !$scope.contractObjectsIsEdited;

        if (!canSelect) {
            messageBoxService
                .showConfirm('Не все данные сохранены. Уйти со страницы редактирования закупки без сохранения?', 'Сохранение')
                .then(
                    function (result) {
                        $scope.selectedContractIsEdited = false;
                        $scope.contractObjectsIsEdited = false;
                        $scope.selectedPurchaseIsEdited = false;
                        $scope.selectedLotIsEdited = false;
                        $scope.objectsIsEdited = false;
                        changeSelectedPurchase(selectedListId[0]);
                    },
                    function (result) {
                        $scope.purchasesGrid.Options.data.forEach(function (item, i, arr) {
                            if (item.Id === $scope.selectPurchase_id) {
                                $scope.purchasesGridApi.selection.selectRow(item);
                                return;
                            }
                        });
                        /*if (result === 'no') {
                            $scope.Bricks_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }*/

                    });
        } else {
            changeSelectedPurchase(selectedListId[0]);
        }
    }

    $scope.getLog = function (PurchaseId) {
        window.open('/#/Global?name=gs_trigger_log&purchase_id=' + PurchaseId, '_blank');
    };
    function changeSelectedPurchase(selectedId) {
        $scope.selectPurchase_id = selectedId;
        clearPurchaseInfo();

        $scope.loadPurchaseDescription(selectedId);
    }

    function loadPurchases() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadPurchases/'
        }).then(function (response) {
            $scope.purchasesGridLoad(response.data);

        }, function () {
            $scope.purchasesGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
            messageBoxService.showError($scope.message);
        });
    }

    

    $scope.getIsLaterPurchases = function () {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetIsLaterPurchases/'
        }).then(function (response) {
            $scope.purchasesGridLoad(response.data);
        }, function () {
            $scope.purchasesGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
        });
    };

    $scope.getPurchasesSupplierResult = function () {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetPurchasesSupplierResult/'
        }).then(function (response) {
            $scope.purchasesGridLoad(response.data);
        }, function () {
            $scope.purchasesGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
        });
    };

    $scope.getPurchasesByFilter = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_GetPurchasesByFilterView.html',
            controller: 'GetPurchasesByFilterController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static'
        });

        modalInstance.result.then(function (v) {

            $scope.purchasesInWorkLoading = $http({
                method: 'POST',
                data: v,
                url: '/GovernmentPurchases/GetPurchasesByFilter/'
            }).then(function (response) {
                $scope.purchasesGridLoad(response.data);
            }, function () {
                $scope.purchasesGrid.Options.data = [];
                $scope.message = 'Unexpected error';
                messageBoxService.showError($scope.message);
                $scope.canGetPurchases = true;
                $scope.canSetPurchases = false;
            });
        }, function () {
        });
    };

    $scope.getPurchases = function () {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetPurchases/'
        }).then(function (response) {
            $scope.purchasesGridLoad(response.data);
        }, function () {
            $scope.purchasesGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
        });
    };

    $scope.getPurchasesWithContracts = function (KK) {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetPurchasesWithContracts/',
            data: JSON.stringify({ KK: KK })
        }).then(function (response) {
            $scope.purchasesGridLoad(response.data);
        }, function () {
            $scope.purchasesGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
        });
    };

    $scope.setPurchases = function () {
        var canSet = !$scope.selectedPurchaseIsEdited && !$scope.selectedLotIsEdited && !$scope.objectsIsEdited;

        if (!canSet) {
            messageBoxService.showConfirm('Не все данные сохранены. Вернуть данные без сохранения?', 'Сохранение')
                .then(function () {
                    setPurchases();
                    $scope.selectedPurchaseIsEdited = false;
                    $scope.selectedLotIsEdited = false;
                    $scope.objectsIsEdited = false;
                }, function () { });
        } else {
            setPurchases();
        }
    };

    function setPurchases() {
        //вопросы о сохранении
        var canSelect = !$scope.selectedContractIsEdited && !$scope.contractObjectsIsEdited;

        if (!canSelect) {
            messageBoxService.showConfirm('Не все данные сохранены. Уйти со страницы редактирования контракта без сохранения?', 'Сохранение')
                .then(function () {
                    $scope.selectedContractIsEdited = false;
                    $scope.contractObjectsIsEdited = false;
                    setPurchasesR();
                }, function () { });
        } else {
            setPurchasesR();
        }
    }

    function setPurchasesR() {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',            
            url: '/GovernmentPurchases/SetPurchases/'
            //,async:true
        }).then(function () {
            clearPurchases();
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
        }, function () {
            $scope.message = 'Unexpected error';
            $scope.canGetPurchases = false;
            $scope.canSetPurchases = true;
            messageBoxService.showError($scope.message);
            });         
    }

    //==================== Описание закупки =====================

    //Запрос на удаление закупки
    $scope.deletePurchaseAsk = function () {
        if ($scope.selectedPurchase === null)
            return;

        messageBoxService.showConfirm('Вы уверены, что хотите удалить закупку?', 'Удаление')
            .then(function () {
                deletePurchase($scope.selectedPurchase.Id);
            }, function () { });
    };

    //Удаляем закупку
    function deletePurchase(purchaseId) {

        if (purchaseId === null)
            return;

        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/DeletePurchase/',
            data: JSON.stringify({ id: purchaseId })
        }).then(function () {

            var item = $scope.purchasesGrid.Options.data.filter(function (object)
            {
                return object.Id === purchaseId;
            })[0];

            //Удаляем закупку из списка
            $scope.purchasesGrid.Options.data.removeitem(item);
            $scope.selectedPurchaseIsEdited = false;
            $scope.selectedLotIsEdited = false;
            $scope.objectsIsEdited = false;
            $scope.selectedPurchase_set(null);
            //Чистим поля
            clearPurchaseInfo();
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    }

    //Можем сохранить закупку?
    $scope.canSavePurchase = function () {

        var nullDeliveryEnd = $scope.selectedPurchase.DeliveryTimeInfo.filter(function (item) {
            return item.DateStart === null || item.DateEnd === null || item.DeliveryTimePeriod === null || item.DateEnd<=item.DateStart;
        });

        var mixed100 = $scope.selectedPurchase.PurchaseNatureMixed.Validate();

        return nullDeliveryEnd.length === 0 && mixed100;
    };



    $scope.loadPurchaseDescription = function (purchaseId) {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadPurchase/',
            data: JSON.stringify({ id: purchaseId })
        }).then(function (response) {
            $scope.selectedPurchase_set(response.data);

            $scope.selectedPurchase_DeliveryTimeInfo_start = $scope.selectedPurchase.DeliveryTimeInfo.length;

            //$scope.selectedPurchase.DateBegin_DT = parseISOAsUTC($scope.selectedPurchase.DateBegin);
            //$scope.selectedPurchase.DateEndFirstParts_DT = parseISOAsUTC($scope.selectedPurchase.DateEndFirstParts);
           // $scope.selectedPurchase.DateEnd_DT = parseISOAsUTC($scope.selectedPurchase.DateEnd);

            $scope.selectedPurchase.DateEndFormat = parseISOAsUTC($scope.selectedPurchase.DateEnd);
            $scope.selectedPurchase.PurchaseNatureMixed = new PurchaseNatureMixed($scope.selectedPurchase.PurchaseNatureMixed);



            response.data.DeliveryTimeInfo = response.data.DeliveryTimeInfo.map(function (value) {
                return new DeliveryTimeInfoClass(value);
            });
            $scope.loadRoleDependentPurchaseClassList();
            $scope.loadLots(purchaseId);
            $scope.LoadPlanG(purchaseId);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    //Справочник список разделов закупки
    $scope.loadRoleDependentPurchaseClassList = function () {
        $scope.purchasesFilterLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetRoleDependentPurchaseClassList/',
            data: JSON.stringify({ selectedPurchaseClassId: $scope.selectedPurchase.PurchaseClass.Id })
        }).then(function (response) {
            var data = response.data;
            $scope.purchaseClassList.DictionaryData = data;
            $scope.purchaseClassList.selected = $scope.purchaseClassList.DictionaryData[findSelectedPurchaseClass()];
        }, function () {
            $scope.message = 'Unexpected Error';
        });
    };
    
    function findSelectedPurchaseClass() {
        for(var i = 0; i < $scope.purchaseClassList.DictionaryData.length; i++) {
            if ($scope.purchaseClassList.DictionaryData[i].Id === $scope.selectedPurchase.PurchaseClass.Id) {
                return i;
            }
        }
        return 0;
    }

    $scope.editPurchaseInfo = function () {
        $scope.selectedPurchaseIsEdited = true;
    };

    $scope.savePurchaseInfo = function () {
        if (!validatePurchaseClassAndObjectsReady()) {
            return;
        }

        $scope.selectedPurchase.PurchaseClass.Id = $scope.purchaseClassList.selected.Id;


       // $scope.selectedPurchase.DateBegin = parse_to_datettime($scope.selectedPurchase.DateBegin_DT);
       // $scope.selectedPurchase.DateEndFirstParts = parse_to_datettime($scope.selectedPurchase.DateEndFirstParts_DT);
      //  $scope.selectedPurchase.DateEnd = parse_to_datettime($scope.selectedPurchase.DateEnd_DT);


        var data = JSON.stringify({ purchaseJson: $scope.selectedPurchase });
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/SavePurchaseInfo/',
            data: data
        }).then(function () {
            $scope.loadRoleDependentPurchaseClassList();
            $scope.selectedPurchaseIsEdited = false;
        }, function (response) {
                errorHandlerService.showResponseError(response);
        });
    };
    $scope.ObjectsStatus = function () {
        var ret = "";

        if ($scope.objectsGrid.Options.data !== undefined && $scope.objectsGrid.Options.data.length > 0) {
            ret += " [Есть Данные с сайта]";
        }
        else {
            ret += " [Нет Данных с сайта]";
        }

        if ($scope.objectsReadyGrid.Options.data !== undefined && $scope.objectsReadyGrid.Options.data.length > 0) {
            ret += " [Есть Техническое задание]";
        }
        else {
            ret += " [Нет Технического задания]";
        }
        return ret;
    }

    function validatePurchaseClassAndObjectsReady() {
        if(($scope.purchaseClassList.selected.Id === 1 || $scope.purchaseClassList.selected.Id === 3 || $scope.purchaseClassList.selected.Id === 9) 
        && $scope.objectsReadyGrid.Options.data.length > 0) {
            messageBoxService.showInfo('Измените Раздел или Удалите ТЗ в Объектах закупки.');
            return false;
        }

        if (($scope.purchaseClassList.selected.Id === 1 || $scope.purchaseClassList.selected.Id === 3 || $scope.purchaseClassList.selected.Id === 9)
            && $scope.objectsReadyGrid.Options.data.length > 0) {
            messageBoxService.showInfo('Измените Раздел или Удалите ТЗ в Объектах закупки.');
            return false;
        }
        return validateObjectsReadyUnits($scope.objectsReadyGrid.Options.data);
    }

    function validateObjectsReadyUnits(data) {
        for (var i = 0; i < data.length; i++) {
            var unit = data[i].Unit;
            if (unit === null || unit === undefined || unit === "") {
                messageBoxService.showInfo('Ед. измерения не может быть пустой');
                return false;
            }
            if (unit) {
                var isOnlyNumbers = true;
                for (var j = 0; j < unit.length; j++)
                    if (!(unit[j] >= '0' && unit[j] <= '9')) {
                        isOnlyNumbers = false;
                        break;
                    }

                if (isOnlyNumbers) {
                    messageBoxService.showInfo('Ед. измерения \'' + unit + '\' не может содержать только цифры');
                    return false;
                }
            }
            
        }

        return true;
    }

    $scope.getDateTime = function (dateStingddMMyyy) {
        if (dateStingddMMyyy.length > 0) {
            var dateF = dateStingddMMyyy.split('.');
            if (dateF.length === 3)
                var date = new Date(dateF[2], dateF[1] - 1, dateF[0]);
            date = new Date(date.getTime() - 60000 * date.getTimezoneOffset());
            return date;
        }
        return null;
    };

    $scope.getDateTime_YMD = function (val) {
        val = val.substr(0, 10);
        val = val.replace(new RegExp("-", 'g'), ".");

        if (val.length > 0) {
            var dateF = val.split('.');
            if (dateF.length === 3)
                var date = new Date(dateF[0], dateF[1]-1, dateF[2]);
            date = new Date(date.getTime() - 60000 * date.getTimezoneOffset());
            return date;
        }
        return null;
    };

    $scope.addDeliveryTimeInfo = function () {
        var deliveryTimeInfo = new DeliveryTimeInfoClass();
        if ($scope.selectedPurchase !== null && $scope.selectedPurchase.DateEnd_DT !== null) {

            var dateStart = $scope.getDateTime_YMD($scope.selectedPurchase.DateEnd);
            //var dateStart = new Date($scope.selectedPurchase.DateEnd);

            if (dateStart !== null) {
                var day_add = 10;
                if ($scope.selectedPurchase.Number.indexOf("C") > 0)
                    day_add = 0;
                dateStart.setDate(dateStart.getDate() + day_add);
                deliveryTimeInfo.DateStart = dateStart;
                deliveryTimeInfo.changeMinAndMaxDates();
            }
        }
        $scope.selectedPurchase.DeliveryTimeInfo.push(deliveryTimeInfo);
    };

    $scope.removeDeliveryTimeInfo = function (index) {
        $scope.selectedPurchase.DeliveryTimeInfo.splice(index, 1);
    };

    $scope.addPaymentInfo = function () {
        var payment = new PaymentClass();
        $scope.selectedPurchase.Payment.push(payment);
    };

    $scope.removePaymentInfo = function (index) {
        $scope.selectedPurchase.Payment.splice(index, 1);
    };

    $scope.changePurchaseCustomer = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_SearchOrganizationView.html',
            controller: 'SearchOrganizationController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static',
            resolve: {
                Is_Customer: true, Is_Recipient: false
            }
        });

        modalInstance.result.then(function (v) {
            $scope.selectedPurchase.Customer = v;
        }, function () {
        });
    };

    $scope.searchSupplier = function (item) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_SearchSupplierView.html',
            controller: 'SearchSupplierController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static'
        });

        modalInstance.result.then(function (selectedSupplier) {
            item.Supplier = selectedSupplier;
        }, function () {
        });
    };

    //==================== Лоты закупки =====================

    $scope.lotsLoading = null;

    $scope.lotsGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_LotsGrid');

    $scope.lotsGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' ,filter: { condition: uiGridCustomService.numberCondition }},
        { name: 'Номер лота', field: 'Number', type: 'number' },
        { name: 'Статус лота', field: 'LotStatus', filter: { condition: uiGridCustomService.condition }}
    ];

    $scope.lotsGrid.Options.enableFiltering = true;
    $scope.lotsGrid.Options.multiSelect = false;
    $scope.lotsGrid.Options.noUnselect = true;
    $scope.lotsGrid.Options.showGridFooter = true;
    //$scope.lotsGrid.Options.enableGridMenu = false;

    $scope.lotsGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.lotsGridApi = gridApi;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, selectLot);
        gridApi.selection.on.rowSelectionChangedBatch($scope, selectLot);
    };

    $scope.PlanGsGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_PlanGsGrid');

    $scope.PlanGsGrid.Options.columnDefs = [
        //{ name: 'Id', field: 'Id', type: 'number' },
        //{ name: 'expandrowid', field: 'Number', type: 'number' },
        { name: 'Начальная (максимальная) цена контракта', field: 'Sum', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_FLOAT_COUNT},
        { name: 'заказчик', field: 'Customer', filter: { condition: uiGridCustomService.condition } },
        { name: 'Идентификационный код закупки', field: 'IKZ', filter: { condition: uiGridCustomService.condition } },
        { name: 'Место доставки товара', field: 'Place', filter: { condition: uiGridCustomService.condition }},
        {
            name: 'Сведения о связи с позицией плана-графика', field: 'Plan_Id', filter: { condition: uiGridCustomService.condition },
            cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.Plan_Url}}" target="_blank">ссылка</a></div>'
        },
        { name: 'Customer_id', field: 'Customer_id',filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'ShortName', field: 'ShortName', filter: { condition: uiGridCustomService.condition }}
    ];

    $scope.PlanGsGrid.Options.enableFiltering = true;
    $scope.PlanGsGrid.Options.multiSelect = false;
    $scope.PlanGsGrid.Options.noUnselect = true;
    $scope.PlanGsGrid.Options.showGridFooter = true;

    $scope.PlanGsGrid.Options.onRegisterApi = function (gridApi) {
        $scope.PlanGsGridApi = gridApi;
    };

    function selectLot(row) {
        var selectedListId = $scope.lotsGridApi.selection.getSelectedRows().map(function (value) {
            return value.Id;
        });

        // обрабатываем только если выбран 1 лот
        if (selectedListId.length !== 1 || selectedListId[0] === undefined) {
            return;
        }

        var canSelect = !$scope.selectedLotIsEdited && !$scope.objectsIsEdited && !$scope.selectedSupplierResultIsEdited;

        if (!canSelect) {
            messageBoxService.showConfirm('Не все данные сохранены. Уйти со страницы редактирования лота без сохранения?', 'Сохранение')
                .then(function () {
                    changeSelectedLot(selectedListId[0]);
                    $scope.selectedLotIsEdited = false;
                    $scope.objectsIsEdited = false;
                    $scope.selectedSupplierResultIsEdited = false;
                }, function () { });
        } else {
            changeSelectedLot(selectedListId[0]);
        }
    }
    function changeSelectedLot(selectedId) {
        clearLotsInfo();
        $scope.loadLotDescription(selectedId);
        $scope.loadObjects(selectedId);
        $scope.loadObjectsReady(selectedId);
        $scope.loadSupplierResult(selectedId);
    }

    $scope.loadLots = function (purchaseId) {
        $scope.lotsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadLots/',
            data: JSON.stringify({ purchaseId: purchaseId })
        }).then(function (response) {
            $scope.lotsGrid.Options.data = response.data;
            if (response.data !== null && response.data.length > 0) {

                $scope.lotsGridApi.grid.modifyRows($scope.lotsGrid.Options.data);
                $scope.lotsGridApi.selection.selectRow($scope.lotsGrid.Options.data[0]);
            }

        }, function () {
            $scope.lotsGrid.Options.data = [];
            $scope.message = 'Unexpected error';
        });
    };
    
    $scope.LoadPlanG = function (purchaseId) {
        $scope.LoadPlanGs = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadPlanG/',
            data: JSON.stringify({ purchaseId: purchaseId })
        }).then(function (response) {
            $scope.PlanGsGrid.Options.data = response.data;
            $scope.PlanGCount = response.data.length;
            //if (response.data !== null && response.data.length > 0) {
            //}

        }, function () {
                $scope.PlanGsGrid.Options.data = [];
                $scope.PlanGCount = 'не доступно';
            $scope.message = 'Unexpected error';
        });
    };

    //==================== Описание лота =====================

    $scope.selectedLotIsEdited = false;

    $scope.loadLotDescription = function (lotId) {
        $scope.CurrentLotId = lotId;
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadLot/',
            data: JSON.stringify({ id: lotId })
        }).then(function (response) {
            $scope.selectedLot_set(response.data);
            
            $scope.loadContracts(lotId, "");
            $scope.MayEditPurObject();
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.editLotInfo = function () {
        $scope.selectedLotIsEdited = true;
    };

    $scope.saveLotInfo = function () {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/SaveLotInfo/',
            data: JSON.stringify({ lotJson: $scope.selectedLot })
        }).then(function () {
            $scope.selectedLotIsEdited = false;

            var getLot = $scope.lotsGrid.Options.data.filter(function (item) { return item.Id === $scope.selectedLot.Id; });

            if (getLot !== null && getLot.length === 1) {
                getLot[0].Sum = $scope.selectedLot.Sum;
            }


        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.selectedDeliveryIsEdited = function () {
        return $scope.selectedPurchaseIsEdited && ($scope.selectedPurchase_DeliveryTimeInfo_start === 0 || !userService.isInRoles('GOperatorRemote'));
    };
    $scope.editFunding = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_FundingView.html',
            controller: 'FundingController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                fundingList: function () {
                    return $scope.selectedLot.FundingList;
                },
                sourceOfFinancing: function () {
                    return $scope.selectedLot.SourceOfFinancing;
                }
            }
        });

        modalInstance.result.then(function (v) {
            $scope.selectedLot.FundingList = v;
            formFundingString();
        }, function () {
        });
    };

    function formFundingString() {
        var result = [];
        $scope.selectedLot.FundingList.forEach(function (item, i, arr) {
            var internalName = item.InternalName;
            item.CheckedList.forEach(function (item, i, arr) {
                if (item.Checked) {
                    result.push(internalName);
                }
            });
        });
        $scope.selectedLot.FundingString = result.join();
    }

    //==================== Объекты закупки =====================

    $scope.objectsIsEdited = false;

    $scope.objectsLoading = null;

    $scope.objectsGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_ObjectsGrid');
    $scope.objectsGrid.Options.noUnselect = true;
    $scope.objectsGrid.Options.columnDefs = [
        { name: 'Наименование', field: 'Name', enableHiding: false },
        { name: 'OKPD', field: 'OKPD', enableHiding: false, filter: { condition: uiGridCustomService.condition }},
        { name: 'Ед.измерения', field: 'Unit', enableHiding: false, filter: { condition: uiGridCustomService.condition }},
        { name: 'Количество', field: 'Amount', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, enableHiding: false },
        { name: 'Цена', field: 'Price', type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition }, enableHiding: false },
        { name: 'Сумма', field: 'Sum', type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition }, enableHiding: false }
    ];

    $scope.objectsGrid.Options.showGridFooter = true;
    //$scope.objectsGrid.Options.enableGridMenu = false;

    $scope.objectsGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.objectsGridApi = gridApi;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            var selectedListId = $scope.objectsGridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            });
        });

        //Что-то выделили
        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            var selectedListId = $scope.objectsGridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            });
        });
    };

    function canEditObjectsReadyGrid() {
        return $scope.objectsIsEdited;
    }

    $scope.objectsReadyGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_ObjectsReadyGrid');

    //$scope.objectsReadyGrid.Options.onRegisterApi = function (gridApi) {
    //    //set gridApi on scope
    //    $scope.objectsReadyGridApi = gridApi;
    //};
    $scope.objectsReadyGrid_Sum = function () {
        var ret = 0;
        if ($scope.objectsReadyGrid.Options.data !== undefined)
        $scope.objectsReadyGrid.Options.data.forEach(function (item) {
            ret += item.Sum;
        });
        return ret;
    };
    $scope.objectsReadyGrid.Options.columnDefs = [
        { name: 'Наименование', field: 'Name', enableHiding: false, enableCellEdit: true, cellEditableCondition : canEditObjectsReadyGrid },
        { name: 'Ед.измерения', field: 'Unit', enableHiding: false, enableCellEdit: true, cellEditableCondition: canEditObjectsReadyGrid },
        { name: 'Количество', field: 'Amount', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, enableHiding: false, enableCellEdit: true, cellEditableCondition: canEditObjectsReadyGrid },
        { name: 'Цена', field: 'Price', type: 'number', editableCellTemplate: onlyPositiveEditableCellTemplate, cellFilter: formatConstants.FILTER_PRICE, enableHiding: false, enableCellEdit: true, cellEditableCondition: canEditObjectsReadyGrid },
        {
            name: 'Сумма', 
            field: 'Sum', 
            aggregationType: uiGridConstants.aggregationTypes.sum, 
            //footerCellTemplate: '<div class="ui-grid-cell-contents" ng-class="{whiteOnRed: col.getAggregationValue() > 0 && col.getAggregationValue().toFixed(2) != grid.appScope.selectedLot.Sum.toFixed(2) }">Сумма: {{col.getAggregationValue() | number:2 }}, разница с лотом: {{col.getAggregationValue() - grid.appScope.selectedLot.Sum | number:2}} </div>',
            type: 'number', 
            editableCellTemplate: onlyPositiveEditableCellTemplate, 
            cellFilter: formatConstants.FILTER_PRICE, 
            enableHiding: false, 
            enableCellEdit: true, 
            cellEditableCondition: canEditObjectsReadyGrid
        },
        {
            name: 'Получатель', field: 'ReceiverShortName', enableHiding: false, enableCellEdit: false,
            cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/OrganizationsEditor?Id={{row.entity.ReceiverId}}" target="_blank">{{COL_FIELD}}</a></div>'
        },
        { name: 'грязный получатель', field: 'ReceiverRaw', enableHiding: false, enableCellEdit: true, cellEditableCondition: canEditObjectsReadyGrid }
    ];

    $scope.objectsReadyGrid.Options.showGridFooter = true;
    $scope.objectsReadyGrid.Options.showColumnFooter = false;
    $scope.objectsReadyGrid.Options.noUnselect = true;
    //$scope.objectsReadyGrid.Options.enableGridMenu = false;
    $scope.test = function () {
        var v = !$scope.objectsIsEdited || !$scope.objectsReadyGrid.getSelectedItem() || $scope.objectsReadyGrid.getSelectedItem() === 0;
        var selectedData = $scope.objectsReadyGrid.getSelectedItem();
    };
    $scope.loadObjects = function (lotId) {
        $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadObjects/',
            data: JSON.stringify({ lotId: lotId })
        }).then(function (response) {
            $scope.objectsGrid.Options.data = response.data;
        }, function () {
            $scope.objectsGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.loadObjectsReady = function (lotId) {
        $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadObjectsReady/',
            data: JSON.stringify({ lotId: lotId })
        }).then(function (response) {
            $scope.objectsReadyGrid.Options.data = response.data;
            $scope.MayEditPurObject();
        }, function () {
            $scope.objectsReadyGrid.Options.data = [];
            $scope.message = 'Unexpected error';
                messageBoxService.showError($scope.message);
                $scope.MayEditPurObject();
            });

        
    };

    //==================== Очистка ===========================

    function clearPurchases() {
        $scope.purchasesGrid.Options.data = [];
        clearPurchaseInfo();
    }

    function clearPurchaseInfo() {
        $scope.selectedPurchase_set(null);
        $scope.selectedPurchase_DeliveryTimeInfo_start = 0;
        $scope.PlanGsGrid.Options.data = [];
        $scope.PlanGCount = 'не доступно';
        
        clearLots();
    }

    function clearLots() {
        $scope.lotsGrid.Options.data = [];
        clearLotsInfo();
    }

    function clearLotsInfo() {
        $scope.selectedLot_set(null);
        $scope.objectsGrid.Options.data = [];
        $scope.objectsReadyGrid.Options.data = [];
        clearContracts();
        clearSupplierResult();
    }

    function clearSupplierResult() {
        $scope.supplierResult = null;
    }

    function clearContracts() {
        $scope.contractsGrid.Options.data = [];
        $scope.ContractPaymentStageGrid.Options.data = [];
        $scope.ContractPaymentStageGrid_new.Options.data = [];
        clearContractsInfo();
    }

    function clearContractsInfo() {
        $scope.selectedContract = null;
        $scope.contractObjectsGrid.Options.data = [];
        $scope.contractObjectsReadyGrid.Options.data = [];
        $scope.contractstageObjectsGrid.Options.data = [];
        $scope.contractObjectsReadyHistoryGrid.Options.data = [];
    }

    //========================================================
    $scope.Verification_Amount = function () {
        var Exists_point = false;
        $scope.objectsReadyGrid.Options.data.forEach(function (item) {
            var DR_1 = item.Amount % 1;
            if (DR_1 !== 0)
                Exists_point = true;
        });
        if (Exists_point === true) {
            messageBoxService.showConfirm('поле Количество имеет дробное значение. Всё равно сохранить?')
                .then(function () {
                    //Тут идти дальше
                    $scope.saveObjectsReady_Func();
                    //return true;
                }, function () {
                      //  return false;
                });
        }
        else
            $scope.saveObjectsReady_Func();
        //return true;
    };

    $scope.editObjectsReady = function () {
        $scope.objectsIsEdited = true;
    };

    $scope.saveObjectsReady = function () {
        $scope.Verification_Amount();        
    };
    $scope.saveObjectsReady_Func = function () {

        if (!validatePurchaseClassAndObjectsReady()) {
            return;
        }

        var data = JSON.stringify({ objects: $scope.objectsReadyGrid.Options.data, lotId: $scope.selectedLot.Id });

        $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/SavePurchaseObjectsReady/',
            data: data
        }).then(function (response) {
            $scope.objectsIsEdited = false;
            if (response.data === 2) {
                $scope.loadContractObjectsReady($scope.CurrentContractId);
            }
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.checkObject = function () {
        var result = $scope.objectsReadyGrid.Options.data.filter(function (item) {
            return item.ReceiverShortName === null && item.ReceiverId === null;
        });

        return result.length > 0;
    };

    //Загрузить шаблон с данными к себе
    $scope.downloadPurchaseTemplate = function () {
        var data = JSON.stringify({ objects: $scope.objectsReadyGrid.Options.data });

        $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/DownloadPurchaseTemplate/',
            data: data,
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'Шаблон закупка ' + $scope.selectedPurchase.Number + ' лот ' + $scope.selectedLot.Number + '.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.UploadPurchaseTemplate = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GovernmentPurchases/UploadPurchaseTemplate/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {

                $scope.objectsReadyGrid.Options.data = response.data;

                updateObjectReceiver(0);
            }).catch (function (error) {
                $scope.objectsReadyGrid.Options.data = [];
                messageBoxService.showError(error.data);
            });
        }
    };

    $scope.copyObjectsToReady = function () {
        $scope.objectsReadyGrid.Options.data = $scope.objectsGrid.Options.data;

        updateObjectReceiver(1);
    };

    function updateObjectReceiver(onlynull) {
        if ($scope.selectedPurchase !== null && $scope.selectedPurchase.Customer !== null) {

            $scope.objectsReadyGrid.Options.data.forEach(function (item) {
                if(onlynull===undefined || onlynull===1 || item.ReceiverId === null)
                {
                    
                    if ($scope.PlanGsGrid.Options.data.length === 1 && $scope.PlanGsGrid.Options.data[0].Customer_id > 0) {
                        item.ReceiverId = $scope.PlanGsGrid.Options.data[0].Customer_id;
                        item.ReceiverShortName = $scope.PlanGsGrid.Options.data[0].ShortName;
                    }
                    else {
                        item.ReceiverShortName = $scope.selectedPurchase.Customer.ShortName;
                        item.ReceiverId = $scope.selectedPurchase.Customer.Id;
                    }
                }
            });
        }
    }

    $scope.changeObjectReadyReceiver = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_SearchOrganizationView.html',
            controller: 'SearchOrganizationController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static',
            resolve: {
                Is_Customer: false, Is_Recipient: true
            }
        });

        modalInstance.result.then(function (v) {
            var selected = $scope.objectsReadyGrid.getSelectedItem();
            selected.forEach(function (item) {
                item.ReceiverId = v.Id;
                item.ReceiverShortName = v.ShortName;
            });

        }, function () {
        });
    };

    $scope.removeObjectReady = function () {
        var selectedData = $scope.objectsReadyGrid.getSelectedItem();

        selectedData.forEach(
            function (item) {
                $scope.objectsReadyGrid.Options.data.removeitem(item);
            });

        $scope.objectsReadyGrid.clearSelection();
    };

    $scope.copyObjectReady = function () {
        var selectedData = $scope.objectsReadyGrid.getSelectedItem();

        selectedData.forEach(
            function (item) {
                var newItem = {
                    Name: item.Name,
                    Unit: item.Unit,
                    Amount: item.Amount,
                    Price: item.Price,
                    Sum: item.Sum,
                    ReceiverId: item.ReceiverId,
                    ReceiverShortName: item.ReceiverShortName
                };

                $scope.objectsReadyGrid.Options.data.push(newItem);
            });

        $scope.objectsReadyGrid.clearSelection();
    };

    //------------- контракты ----------------------

    $scope.contractsLoading = null;

    $scope.contractsGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_ContractsGrid');
    $scope.ContractPaymentStageGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_ContractPaymentStageGrid');
    $scope.ContractPaymentStageGrid_new = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_ContractPaymentStageGrid_new');

    $scope.contractsGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Номер контракта', field: 'ReestrNumber', filter: { condition: uiGridCustomService.conditionSpace }, },
        { name: 'ссылка', field: 'Url', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents"><a href="{{row.entity.Url}}" target="_blank">ссылка</a></div>' },
        { name: 'Готов?', field: 'IsReady', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.IsReady  ? "Да" : "Нет"}}</div>' }
    ];

    $scope.contractsGrid.Options.enableFiltering = true;
    $scope.contractsGrid.Options.multiSelect = false;
    $scope.contractsGrid.Options.noUnselect = true;
    $scope.contractsGrid.Options.showGridFooter = true;
    //$scope.contractsGrid.Options.enableGridMenu = false;

    $scope.contractsGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.contractsGridApi = gridApi;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, selectContract);
        gridApi.selection.on.rowSelectionChangedBatch($scope, selectContract);
    };

    $scope.ContractPaymentStageGrid.Options.enableFiltering = true;
    $scope.ContractPaymentStageGrid.Options.multiSelect = false;
    $scope.ContractPaymentStageGrid.Options.noUnselect = true;
    $scope.ContractPaymentStageGrid.Options.showGridFooter = true;

    $scope.ContractPaymentStageGrid.Options.onRegisterApi = function (gridApi) {
        $scope.ContractPaymentStageGridApi = gridApi;
    };

    $scope.ContractPaymentStageGrid_new.Options.enableFiltering = true;
    $scope.ContractPaymentStageGrid_new.Options.multiSelect = false;
    $scope.ContractPaymentStageGrid_new.Options.noUnselect = true;
    $scope.ContractPaymentStageGrid_new.Options.showGridFooter = true;

    $scope.ContractPaymentStageGrid_new.Options.onRegisterApi = function (gridApi) {
        $scope.ContractPaymentStageGrid_newApi = gridApi;
    };

    function selectContract(row) {
        var selectedListId = $scope.contractsGridApi.selection.getSelectedRows().map(function (value) {
            return value.Id;
        });

        // обрабатываем только если выбран 1 контракт
        if (selectedListId.length !== 1 || selectedListId[0] === undefined) {
            return;
        }

        var canSelect = !$scope.selectedContractIsEdited && !$scope.contractObjectsIsEdited;

        if (!canSelect) {
            messageBoxService.showConfirm('Не все данные сохранены по контрактам. Сохранить?', 'Сохранение')
                .then(function () {//Сохранить и перейти
                        if ($scope.selectedContractIsEdited)
                            $scope.saveContractInfo();
                        if ($scope.contractObjectsIsEdited)
                            $scope.saveContractObjectsReady();
                    changeSelectedContract(selectedListId[0]);
                }, function () {//не сохранять
                    changeSelectedContract(selectedListId[0]);
                });
        } else {//не было изменений
            changeSelectedContract(selectedListId[0]);
        }
    }

    
    function changeSelectedContract(selectedId) {
        $scope.CurrentContractId = selectedId;
        $scope.selectedContractIsEdited = false;
        $scope.contractObjectsIsEdited = false;
        clearContractsInfo();
        $scope.loadContractDescription(selectedId);
        $scope.loadContractObjects(selectedId);
        $scope.loadContractObjectsReady(selectedId);
    }

    $scope.loadContracts = function (lotId, ReestrNumber) {
        $scope.contractsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadContracts/',
            data: JSON.stringify({ lotId: lotId, ReestrNumber: ReestrNumber })
        }).then(function (response) {
            $scope.contractsGrid.Options.data = response.data;
            if (response.data !== null && response.data.length > 0) {

                $scope.contractsGridApi.grid.modifyRows($scope.contractsGrid.Options.data);
                $scope.contractsGridApi.selection.selectRow($scope.contractsGrid.Options.data[0]);
            }

        }, function () {
                $scope.contractsGrid.Options.data = [];
                $scope.ContractPaymentStageGrid.Options.data = [];
                $scope.ContractPaymentStageGrid_new.Options.data = [];
            $scope.message = 'Unexpected error';
        });
    };

    //==================== Описание контракта =====================
    $scope.PaymentType = [];
    $scope.selectedContractIsEdited = false;

    $scope.loadContractDescription = function (contractId) {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadContract/',
            data: JSON.stringify({ id: contractId })
        }).then(function (response) {
            $scope.selectedContract = response.data.Contract;
           
            if ($scope.selectedContract.StatusDate === null) $scope.selectedContract.StatusDate=[];
            
            if ($scope.PaymentType.length === 0) {
                Array.prototype.push.apply($scope.PaymentType, response.data.PaymentType);
                $scope.ContractPaymentStageGrid.Options.columnDefs = [
                    { headerTooltip: true, cellTooltip: true, visible: false, enableCellEdit: false, width: 100, name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
                    { headerTooltip: true, cellTooltip: true, visible: false, enableCellEdit: false, width: 100, name: 'ContractId', field: 'ContractId', filter: { condition: uiGridCustomService.condition } },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 300, name: 'KBK', field: 'KBK', filter: { condition: uiGridCustomService.condition } },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 300, name: 'StageDate', field: 'StageDate', filter: { condition: uiGridCustomService.condition } },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, type: 'number', name: 'Sum', field: 'Sum', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_FLOAT_COUNT },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, type: 'number', name: 'Процент', field: 'calc_proc', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_FLOAT3_COUNT },
                    {
                        headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Тип', field: 'PaymentTypeId', filter: { condition: uiGridCustomService.condition },
                        editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                        editDropdownOptionsArray: $scope.PaymentType,
                        editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name'
                    },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 200, name: 'дата создания', field: 'Date', filter: { condition: uiGridCustomService.condition } },
                ];
                $scope.ContractPaymentStageGrid_new.Options.columnDefs = [
                    { headerTooltip: true, cellTooltip: true, visible: false, enableCellEdit: false, width: 100, name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
                    { headerTooltip: true, cellTooltip: true, visible: false, enableCellEdit: false, width: 100, name: 'ContractId', field: 'ContractId', filter: { condition: uiGridCustomService.condition } },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 300, name: 'KBK', field: 'KBK', filter: { condition: uiGridCustomService.condition } },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 300, name: 'StageDate', field: 'StageDate', filter: { condition: uiGridCustomService.condition } },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, type: 'number', name: 'Sum', field: 'Sum', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_FLOAT_COUNT },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, type: 'number', name: 'Процент', field: 'calc_proc', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_FLOAT3_COUNT },
                    {
                        headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Тип', field: 'PaymentTypeId', filter: { condition: uiGridCustomService.condition },
                        editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this',
                        editDropdownOptionsArray: $scope.PaymentType,
                        editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name'
                    },
                    { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 200, name: 'дата создания', field: 'Date', filter: { condition: uiGridCustomService.condition } },
                ];
            }
            response.data.ContractPaymentStage.forEach(function (item, i, arr) {
                item.calc_proc = $scope.GetProcentFromContractSum(item.Sum);
            });
            response.data.ContractPaymentStage_new.forEach(function (item, i, arr) {
                item.calc_proc = $scope.GetProcentFromContractSum(item.Sum);
            });
            $scope.ContractPaymentStageGrid.Options.data = response.data.ContractPaymentStage;
            $scope.ContractPaymentStageGrid_new.Options.data = response.data.ContractPaymentStage_new;

            $scope.calcLotContractCoefficient();
            $scope.MayEditContactObj();

          

        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.GetProcentFromContractSum = function (Sum_2) {
        if ($scope.selectedContract !== null) {
            return 100*Sum_2 / $scope.selectedContract.Sum;
        }
        return null;
    };
    $scope.editContractInfo = function () {
        $scope.selectedContractIsEdited = true;
    };

    $scope.saveContractInfo = function () {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/SaveContractInfo/',
            data: JSON.stringify({ contractJson: $scope.selectedContract, useContractData: $scope.selectedPurchase.UseContractData })
        }).then(function () {
            $scope.selectedContractIsEdited = false;
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.calcLotContractCoefficient = function () {
        var contractSum = $scope.contractsGrid.Options.data.map(d => d.Sum).reduce((a, b) => a + b, 0);
        var lotSum = $scope.selectedLot.Sum;

        $scope.selectedPurchase.LotContractCoefficient = Math.round(100 * contractSum / lotSum - 100).toFixed(2);
    };


    //==================== Объекты контракта =====================

    $scope.contractObjectsIsEdited = false;

    $scope.contractObjectsLoading = null;


    $scope.contractObjectsGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_ContractObjectsGrid');

    $scope.contractObjectsGrid.Options.columnDefs = [
        { name: 'Наименование', field: 'Name', enableHiding: false },
        { name: 'OKPD', field: 'OKPD', enableHiding: false },
        { name: 'Ед.измерения', field: 'Unit', enableHiding: false },
        { name: 'Количество', field: 'Amount', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, enableHiding: false },
        { name: 'Цена', field: 'Price', type: 'number', cellFilter: formatConstants.FILTER_PRICE, enableHiding: false },
        { name: 'Сумма', field: 'Sum', type: 'number', cellFilter: formatConstants.FILTER_PRICE, enableHiding: false }
    ];

    $scope.contractObjectsGrid.Options.showGridFooter = true;
    //$scope.contractObjectsGrid.Options.enableGridMenu = false;

    if (userService.isInRole("GOperatorRemote")) {
        $scope.contractObjectsGrid.Options.exporterMenuAllData = false;
        $scope.contractObjectsGrid.Options.exporterMenuCsv = false;
        $scope.contractObjectsGrid.Options.exporterMenuExcel = false;
        $scope.contractObjectsGrid.Options.exporterMenuPdf = false;

        $scope.objectsGrid.Options.exporterMenuAllData = false;
        $scope.objectsGrid.Options.exporterMenuCsv = false;
        $scope.objectsGrid.Options.exporterMenuExcel = false;
        $scope.objectsGrid.Options.exporterMenuPdf = false;

    }

    $scope.contractObjectsGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.contractObjectsGridApi = gridApi;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            var selectedListId = $scope.contractObjectsGridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            });
        });

        //Что-то выделили
        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            var selectedListId = $scope.contractObjectsGridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            });
        });
    };

    //==================Результат определения поставщика=========


    $scope.srDate = {
        DateOptions: {
            minDate: null
        },
        Opened: false
    };

    function parseISOAsUTC(s) {
        if (s === null)
            return null;
        var b = s.split(/\D/);
        return new Date(Date.UTC(b[0], --b[1], b[2], b[3], b[4], b[5], b[6] || 0));
    }

    function parse_to_datettime(s) {
        if (s === null)
            return null;
        return new Date(s).toISOString().substr(0, 19);
    }



    $scope.loadSupplierResult = function (lotId) {

        if (!userService.isInRoles('GSupplierResult'))
            return;

        $scope.contractObjectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadSupplierResult/',
            data: JSON.stringify({ lotId: lotId })
        }).then(function (response) {
            $scope.supplierResult = response.data;
            if (response.data.ProtocolDate !== null)
                $scope.supplierResult.ProtocolDate = parseISOAsUTC(response.data.ProtocolDate);
            $scope.srDate.DateOptions.minDate = new Date(response.data.DateBegin);

            //Внутренний идентификатор строки
            var number = 1;

            $scope.supplierResult.SupplierList.forEach(function (sl) {
                sl.Order = number;
                number = number + 1;
                sl.empty = empty;
            });

        }, function () {
            $scope.supplierResult = null;
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };
    $scope.supplierChangeSum = function (item) {
        //Сделать кнопочку (или галочку) «Проставить сумму». По идее сумма должна заполняться равной сумме лота для Участника 1. Вопрос как это реализовать. При загрузке протокола на простановку есть два поля с Участником, но не понятно кто из них первый, пока сами не выберем. Соответственно кнопка «Проставить сумму» может срабатывать только после того как выберем Участника 1. Либо при нажатии на кнопку «Проставить сумму» программа проставляет сумму в любое поле, а пользователь уже потом "подгоняет" Участника 1.
        item.Sum = $scope.selectedLot.Sum;
    };
    $scope.supplierChangeNumber = function (item) {

        var item2 = $scope.supplierResult.SupplierList.filter(function (sl) { return sl.Order !== item.Order; })[0];

        if (item.Number !== null) {
            item.Number = 3 - item.Number;
            item2.Number = 3 - item.Number;
        } else {
            if (item2.Number !== null) {
                item2.Number = 3 - item2.Number;
                item.Number = 3 - item2.Number;
            } else {
                item.Number = 1;
                item2.Number = 2;
            }
        }
    };


    $scope.selectedSupplierResultIsEdited = false;

    $scope.editSupplierResultInfo = function () {
        $scope.selectedSupplierResultIsEdited = true;
    };

    $scope.askCreateContract = function() {
        //Проверка
        if ($scope.supplierResult === null ||
            $scope.selectedPurchase === null ||
            !($scope.supplierResult.LotStatus.Id === 2 || $scope.supplierResult.LotStatus.Id === 3)) {
            messageBoxService.showInfo('Статус лота должен быть "Завершён" или "Завершён/один участник"!', 'Создание');
            return;
        } else {

            //Создаем контракт, объекты контракта и объекты тз на основе закупки.
            messageBoxService.showConfirm('Создать контракт?', 'Создание')
                .then($scope.createContract, function () { });
        }
    };

    $scope.createContract = function() {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_CreateContractView.html',
            controller: 'CreateContractController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static',
            resolve: { lotId: $scope.selectedLot.Id }
        });

        modalInstance.result.then(function (lotId) {
            changeSelectedLot(lotId);
        }, function () {
        });
    };

    function empty() {
        return this.Number === null && this.Supplier === null && this.Sum === null;
    }
    $scope.SupplierResult_setNumberDate = function () {
        //Сделать кнопочку (или галочку) «Проставить номер и дату». При её нажатии номер протокола будет заполняться равным номеру закупки, а дата протокола дате окончания процедуры.
        $scope.supplierResult.ProtocolDate = new Date($scope.selectedPurchase.DateEnd);
        $scope.supplierResult.ProtocolNumber = $scope.selectedPurchase.Number;
    };
    $scope.saveSupplierResultInfo = function () {
        if ($scope.supplierResult.ProtocolDate !== null) {
            $scope.supplierResult.ProtocolDate = new Date($scope.supplierResult.ProtocolDate).toUTCString();
        }
        $scope.supplierResult.Stage = $scope.selectedPurchase.Stage;

        var data = JSON.stringify({ supplierResult: $scope.supplierResult });

        $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/SaveSupplierResult/',
            data: data
        }).then(function () {


            var lotId = $scope.lotsGridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            })[0];
            $scope.selectedSupplierResultIsEdited = false;
            $scope.loadSupplierResult(lotId);
            //Теперь в лотах меняем значение

            var lot = $scope.lotsGrid.Options.data.filter(function (item) { return item.Id === lotId; })[0];

            if (lot !== null) {
                lot.LotStatus = $scope.supplierResult.LotStatus.Name;
            }
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });

    };

    $scope.changeLotStatus = function () {

        //long supplierResultId, long lotStatusId

        var data = JSON.stringify({ supplierResultId: $scope.supplierResult.Id, lotStatusId: $scope.supplierResult.LotStatus.Id });

        $scope.objectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/ChangeLotStatus/',
            data: data
        }).then(function (response) {
            if (response.data !== null) {
                $scope.selectedPurchase.Stage = $scope.stageList.filter(function (item) { return item.Id === response.data; })[0];
            }
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.clear = function (item) {
        item.Supplier = null;
        item.Number = null;
        item.Sum = null;
    };

    $scope.supplierResultError = "";
    $scope.canSaveSupplierResult = function () {
        $scope.supplierResultError = "";

        if ($scope.supplierResult === null) {
            $scope.supplierResultError = "пустой";
            return false;
        }



        if ($scope.supplierResult.LotStatus.Id === 1) {
            if ($scope.supplierResult.SupplierList.filter(function (item) { return item.Supplier !== null || item.Sum !== null && item.Sum > 0; }).length > 0) {
                $scope.supplierResultError = "проверить поставщик, сумма";
                return false;
            }

            if ($scope.supplierResult.ProtocolNumber !== null && $scope.supplierResult.ProtocolNumber.length > 0 || $scope.supplierResult.ProtocolDate !== null) {
                $scope.supplierResultError = "статус лота Открыт, а протокол есть";
                return false;
            }
        }




        //Номер и дата протокола обязательны, кроме статуса "Открыто"
        if ($scope.supplierResult.LotStatus.Id !== 1 && ($scope.supplierResult.ProtocolNumber === null || $scope.supplierResult.ProtocolNumber.length === 0 || $scope.supplierResult.ProtocolDate === null)) {
            $scope.supplierResultError = "Номер и дата протокола обязательны, кроме статуса Открыто";
            return false;
        }

        //Если статус  Завершен Завершен/один участник
        if ($scope.supplierResult.LotStatus.Id === 2 || $scope.supplierResult.LotStatus.Id === 3) {
            //Участник под номеро 1 должны быть и сумма а первого участника должна быть
            if ($scope.supplierResult.SupplierList.filter(function (item) { return item.Supplier !== null && item.Number === 1 && item.Sum !== null && item.Sum >= 0; }).length === 0) {
                $scope.supplierResultError = "Завершен Завершен/один участник, Участник под номеро 1 должны быть и сумма а первого участника должна быть";
                return false;
            }
        }

        //Если отправлено на проверку, то ничего не проверям
        if ($scope.supplierResult.ForCheck)
            return true;

        if (($scope.supplierResult.LotStatus.Id === 4 || $scope.supplierResult.LotStatus.Id === 5) && $scope.supplierResult.SupplierList.filter(function (item) { return !item.empty(); }).length > 0) {
            $scope.supplierResultError = "Не состоялся или Отменен, но есть список участников";
            return false;
        }








        return true;
    };


    //===========================================================


    function canEditContractObjectsReadyGrid() {
        return $scope.contractObjectsIsEdited;
    }
    $scope.ROPStatus = function () {
        var ret = "";
        //if ($scope.supplierResult.LotStatus.Id === 1) {
        //    ret = "Открыт";
        //}
        if ($scope.supplierResult !== undefined && $scope.supplierResult !== null && $scope.supplierResult.LotStatus !== undefined  && $scope.supplierResult.LotStatus !== null) {
            ret = "[" + $scope.supplierResult.LotStatus.Name + "]";
        }
        return ret;
    };
    $scope.ContractObjectsStatus =function() {
        var ret = "";

        if ($scope.contractObjectsGrid.Options.data !== undefined && $scope.contractObjectsGrid.Options.data.length > 0) {
            ret += " [Есть Данные с сайта]";
        }
        else {
            ret += " [Нет Данных с сайта]";
        }

        if ($scope.contractObjectsReadyGrid.Options.data !== undefined && $scope.contractObjectsReadyGrid.Options.data.length > 0) {
            ret += " [Есть Техническое задание]";
        }
        else {
            ret += " [Нет Технического задания]";
        }

        if ($scope.contractstageObjectsGrid.Options.data !== undefined && $scope.contractstageObjectsGrid.Options.data.length > 0) {
            ret += " [Есть Исполнение]";
        }
        else {
            ret += " [Нет Исполнения]";
        }
        return ret;
    }

    $scope.contractObjectsReadyGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_ContractObjectsReadyGrid');
    $scope.contractstageObjectsGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_contractstageObjectsGrid');

    $scope.contractObjectsReadyGrid_Sum = function () {
        var ret = 0;
        if ($scope.contractObjectsReadyGrid.Options.data !== undefined)
        $scope.contractObjectsReadyGrid.Options.data.forEach(function (item) {
            ret += item.Sum;
        });
        return ret;
    };
    $scope.contractObjectsReadyHistoryGrid_Sum = function () {
        var ret = 0;
        if ($scope.contractObjectsReadyHistoryGrid.Options.data !== undefined)
            $scope.contractObjectsReadyHistoryGrid.Options.data.forEach(function (item) {
                ret += item.Sum;
            });
        return ret;
    };
    $scope.contractstageObjectsGrid.Options.columnDefs = [
        { name: 'РЕКВИЗИТЫ ДОКУМЕНТА', field: 'doc' },
        { name: 'ДАТА ПОДПИСАНИЯ ДОКУМЕНТА О ПРИЕМКЕ ТРУ', field: 'date_end', type: 'date' },
        { name: 'СТОИМОСТЬ ИСПОЛНЕННЫХ ОБЯЗАТЕЛЬСТВ, ₽', field: 'sum_go', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { name: 'ФАКТИЧЕСКИ ОПЛАЧЕНО,  RUB', field: 'sum_pay', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { name: 'tovar', field: 'tovar' },
        { name: 'lek_form', field: 'lek_form' },
        { name: 'inn_name', field: 'inn_name' },
        { name: 'DrugDescription_grls', field: 'DrugDescription_grls' },
        { name: 'trade_name', field: 'trade_name' },
        { name: 'dosage', field: 'dosage' },
        { name: 'Цена за потребительскую упаковку с учетом НДС, Р', field: 'price', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { name: 'Серия лекарственного препарата', field: 'seria' },
        { name: 'Срок годности', field: 'date_expiration', type: 'date' },
        { name: 'Ед.измерения', field: 'EI' },
        { name: 'Количество ei', field: 'amount_ei_in', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { name: 'Количество', field: 'amount', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { name: 'СТРАНА ПРОИСХОЖДЕНИЯ ТОВАРА', field: 'country_reg' },
        { name: 'СУММА', field: 'sumObject', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT }
    ];
    $scope.contractstageObjectsGrid.Options.showGridFooter = true;
    $scope.contractstageObjectsGrid.Options.showColumnFooter = false;

    $scope.contractObjectsReadyGrid.Options.columnDefs = [
       { name: 'Наименование', field: 'Name', enableHiding: false, enableCellEdit: true, cellEditableCondition: canEditContractObjectsReadyGrid },
       { name: 'Ед.измерения', field: 'Unit', enableHiding: false, enableCellEdit: true, cellEditableCondition: canEditContractObjectsReadyGrid },
       { name: 'Количество', field: 'Amount', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, enableHiding: false, enableCellEdit: true, cellEditableCondition: canEditContractObjectsReadyGrid },
       { name: 'Цена', field: 'Price', type: 'number', editableCellTemplate: onlyPositiveEditableCellTemplate, cellFilter: formatConstants.FILTER_PRICE, enableHiding: false, enableCellEdit: true, cellEditableCondition: canEditContractObjectsReadyGrid },
        {
            name: 'Сумма', 
            field: 'Sum', 
            aggregationType: uiGridConstants.aggregationTypes.sum, 
            type: 'number', 
            editableCellTemplate: onlyPositiveEditableCellTemplate, 
            cellFilter: formatConstants.FILTER_PRICE, 
            enableHiding: false, 
            enableCellEdit: true, 
            cellEditableCondition: canEditContractObjectsReadyGrid
        }
    ];
    $scope.contractObjectsReadyGrid.Options.showGridFooter = true;
    $scope.contractObjectsReadyGrid.Options.showColumnFooter = false;

    $scope.contractObjectsReadyHistoryGrid = uiGridCustomService.createGridClass($scope, 'GovernmentPurchases_contractObjectsReadyHistoryGrid');
    $scope.contractObjectsReadyHistoryGrid.Options.columnDefs = [
        { name: 'ДатаВерсия', field: 'DT', enableHiding: false, enableCellEdit: false, type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME },
        { name: 'Наименование', field: 'Name', enableHiding: false, enableCellEdit: false },
        { name: 'Ед.измерения', field: 'Unit', enableHiding: false, enableCellEdit: false },
        { name: 'Количество', field: 'Amount', type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT, enableHiding: false, enableCellEdit: false },
        { name: 'Цена', field: 'Price', type: 'number', editableCellTemplate: onlyPositiveEditableCellTemplate, cellFilter: formatConstants.FILTER_PRICE, enableHiding: false, enableCellEdit: false},
        {
            name: 'Сумма',
            field: 'Sum',
            aggregationType: uiGridConstants.aggregationTypes.sum,
            type: 'number',
            editableCellTemplate: onlyPositiveEditableCellTemplate,
            cellFilter: formatConstants.FILTER_PRICE,
            enableHiding: false,
            enableCellEdit: false
        }
    ];
    $scope.contractObjectsReadyHistoryGrid.Options.showGridFooter = true;
    $scope.contractObjectsReadyHistoryGrid.Options.showColumnFooter = false;


    $scope.loadContractObjects = function (contractId) {
        $scope.contractObjectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadContractObjects/',
            data: JSON.stringify({ contractId: contractId })
        }).then(function (response) {
            $scope.contractObjectsGrid.Options.data = response.data;
        }, function () {
            $scope.contractObjectsGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    $scope.loadContractObjectsReady = function (contractId) {
        $scope.contractObjectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/LoadContractObjectsReady/',
            data: JSON.stringify({ contractId: contractId })
        }).then(function (response) {
            $scope.contractObjectsReadyGrid.Options.data = response.data.contractObjects;
            $scope.contractObjectsReadyHistoryGrid.Options.data = response.data.contractObjects_History;
            $scope.contractstageObjectsGrid.Options.data = response.data.contractstageObjects;
            $scope.MayEditContactObj();
        }, function () {
                $scope.contractObjectsReadyGrid.Options.data = [];
            $scope.contractObjectsReadyHistoryGrid.Options.data = [];
            $scope.contractstageObjectsGrid.Options.data = [];
                $scope.MayEditContactObj();
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
            });        
    };


    $scope.editContractObjectsReady = function () {
        $scope.contractObjectsIsEdited = true;
    };

    $scope.Verification_Contract_Amount = function () {
        var Exists_point = false;
        $scope.contractObjectsReadyGrid.Options.data.forEach(function (item) {
            var DR_1 = item.Amount % 1;
            if (DR_1 !== 0)
                Exists_point = true;
        });
        if (Exists_point === true) {
            messageBoxService.showConfirm('поле Количество имеет дробное значение. Всё равно сохранить?')
                .then(function () {
                    //Тут идти дальше
                    $scope.saveContractObjectsReady_Func();
                    //return true;
                }, function () {
                    //  return false;
                });
        }
        else
            $scope.saveContractObjectsReady_Func();
        //return true;
    };
    $scope.saveContractObjectsReady = function () {
        $scope.Verification_Contract_Amount();
    };
    $scope.saveContractObjectsReady_Func = function () {

        if (!validateObjectsReadyUnits($scope.contractObjectsReadyGrid.Options.data))
            return;

        var data = JSON.stringify({ objects: $scope.contractObjectsReadyGrid.Options.data, contractId: $scope.selectedContract.Id });

        $scope.contractObjectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/SaveContractObjectsReady/',
            data: data
        }).then(function (response) {
            $scope.contractObjectsIsEdited = false;
            if (response.data === 2) {
                $scope.loadObjectsReady($scope.CurrentLotId);
            }
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };

    //Загрузить шаблон с данными к себе
    $scope.downloadContractTemplate = function () {
        var data = JSON.stringify({ objects: $scope.contractObjectsReadyGrid.Options.data });

        $scope.contractObjectsLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/DownloadContractTemplate/',
            data: data,
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'Шаблон закупка ' + $scope.selectedPurchase.Number + ' контракт ' + $scope.selectedContract.Id + '.xlsx';
            saveAs(blob, fileName);
        }, function () {
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
        });
    };


    //Загрузить данные из готового шаблона
    $scope.UploadContractTemplate = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GovernmentPurchases/UploadContractTemplate/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.contractObjectsReadyGrid.Options.data = response.data;
            }).catch(function (error) {
                $scope.contractObjectsReadyGrid.Options.data = [];
                messageBoxService.showError(error.data);
            });
        }
    };

    $scope.copyContractObjectsToReady = function () {
        $scope.contractObjectsReadyGrid.Options.data = $scope.contractObjectsGrid.Options.data;
    };

    $scope.copyContractObjectsHistoryToReady = function () {
        $scope.contractObjectsReadyGrid.Options.data = $scope.contractObjectsReadyHistoryGrid.Options.data;
    };

    $scope.removeContractObjectReady = function () {
        var selectedData = $scope.contractObjectsReadyGrid.getSelectedItem();

        selectedData.forEach(
            function (item) {
                $scope.contractObjectsReadyGrid.Options.data.removeitem(item);
            });

        $scope.contractObjectsReadyGrid.clearSelection();
    };

    $scope.copyContractObjectReady = function () {
        var selectedData = $scope.contractObjectsReadyGrid.getSelectedItem();

        selectedData.forEach(
            function (item) {
                var newItem = {
                    Name: item.Name,
                    Unit: item.Unit,
                    Amount: item.Amount,
                    Price: item.Price,
                    Sum: item.Sum
                };

                $scope.contractObjectsReadyGrid.Options.data.push(newItem);
            });

        $scope.contractObjectsReadyGrid.clearSelection();
    };

    $scope.changeContractReceiver = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_SearchOrganizationView.html',
            controller: 'SearchOrganizationController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static',
            resolve: {
                Is_Customer: false, Is_Recipient:true
            }
        });

        modalInstance.result.then(function (v) {
            $scope.selectedContract.ReceiverId = v.Id;
            $scope.selectedContract.Receiver = v.ShortName;
        }, function () {
        });
    };

    //----------------------------------------------

    var PaymentClass = function () {
        this.Id = 0,
            this.KBK = '',
            this.KOSGU = '',
            this.PaymentYear = {
                Id: null,
                Name: null
            };
    };

    var PurchaseNatureMixed = function (model) {


        var PurchaseNatureMixed = {
            flPercentage: null,
            rlPercentage: null,
            blPercentage: null,
            rcpPercentage: null,
            fcpPercentage: null,
            bmpPercentage: null,
            palliativeCarePercentage: null,
            apuPercentage: null,
            fz223Percentage: null,

            FlNature_L2Id: null,
            RlNature_L2Id: null,
            BlNature_L2Id: null,
            RcpNature_L2Id: null,
            FcpNature_L2Id: null,
            BmpNature_L2Id: null,
            PalliativeCareNature_L2Id: null,
            apuNature_L2Id: null,
            fz223Nature_L2Id: null
        };


        Object.defineProperty(PurchaseNatureMixed, 'sum', {
            get: function () {
                return $scope.lotsGrid.Options.data.reduce(function (total, item) { return total + item.Sum; }, 0);
            }

        });


        Object.defineProperty(PurchaseNatureMixed, 'flSum', {
            get: function () {
                return (this.sum * this.flPercentage / 100).toFixed(3);
            },
            set: function (value) {
                this.flPercentage = (value / this.sum * 100).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'rlSum', {
            get: function () {
                return (this.sum * this.rlPercentage / 100).toFixed(3);
            },
            set: function (value) {
                this.rlPercentage = (value / this.sum * 100).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'blSum', {
            get: function () {
                return (this.sum * this.blPercentage / 100).toFixed(3);
            },
            set: function (value) {
                this.blPercentage = (value / this.sum * 100).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'rcpSum', {
            get: function () {
                return (this.sum * this.rcpPercentage / 100).toFixed(3);
            },
            set: function (value) {
                this.rcpPercentage = (value / this.sum * 100).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'fcpSum', {
            get: function () {
                return (this.sum * this.fcpPercentage / 100).toFixed(3);
            },
            set: function (value) {
                this.fcpPercentage = (value / this.sum * 100).toFixed(3);
            }
        });


        Object.defineProperty(PurchaseNatureMixed, 'bmpSum', {
            get: function () {
                return (this.sum * this.bmpPercentage / 100).toFixed(3);
            },
            set: function (value) {
                this.bmpPercentage = (value / this.sum * 100).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'palliativeCareSum', {
            get: function () {
                return (this.sum * this.palliativeCarePercentage / 100).toFixed(3);
            },
            set: function (value) {
                this.palliativeCarePercentage = (value / this.sum * 100).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'fz223Sum', {
            get: function () {
                return (this.sum * this.fz223Percentage / 100).toFixed(3);
            },
            set: function (value) {
                this.fz223Percentage = (value / this.sum * 100).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'apuSum', {
            get: function () {
                return (this.sum * this.apuPercentage / 100).toFixed(3);
            },
            set: function (value) {
                this.apuPercentage = (value / this.sum * 100).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'totalPercentage', {
            get: function () {
                return (parseFloat(this.flPercentage) +
                    parseFloat(this.rlPercentage) +
                    parseFloat(this.blPercentage) +
                    parseFloat(this.rcpPercentage) +
                    parseFloat(this.fcpPercentage) +
                    parseFloat(this.bmpPercentage) +
                    parseFloat(this.apuPercentage) +
                    parseFloat(this.fz223Percentage) +
                    parseFloat(this.palliativeCarePercentage)).toFixed(3);
            }
        });

        Object.defineProperty(PurchaseNatureMixed, 'totalSum', {
            get: function () {
                return (parseFloat(this.flSum) +
                    parseFloat(this.rlSum) +
                    parseFloat(this.blSum) +
                    parseFloat(this.rcpSum) +
                    parseFloat(this.fcpSum) +
                    parseFloat(this.bmpSum) +
                    parseFloat(this.apuSum) +
                    parseFloat(this.palliativeCareSum)).toFixed(3);
            }
        });


        PurchaseNatureMixed.Copy = function (model) {

            if (model === null || model === undefined)
                return;

            this.flPercentage = model.FlPercentage;
            this.rlPercentage = model.RlPercentage;
            this.blPercentage = model.BlPercentage;
            this.rcpPercentage = model.RcpPercentage;
            this.fcpPercentage = model.FcpPercentage;
            this.bmpPercentage = model.BmpPercentage;
            this.palliativeCarePercentage = model.PalliativeCarePercentage;
            this.apuPercentage = model.apuPercentage;
            this.fz223Percentage = model.fz223Percentage;

            this.FlNature_L2Id = model.FlNature_L2Id;
            this.RlNature_L2Id = model.RlNature_L2Id;
            this.BlNature_L2Id = model.BlNature_L2Id;
            this.RcpNature_L2Id = model.RcpNature_L2Id;
            this.FcpNature_L2Id = model.FcpNature_L2Id;
            this.BmpNature_L2Id = model.BmpNature_L2Id;
            this.PalliativeCareNature_L2Id = model.PalliativeCareNature_L2Id;
            this.apuNature_L2Id = model.apuNature_L2Id;
            this.fz223Nature_L2Id = model.fz223Nature_L2Id;
        };

        PurchaseNatureMixed.Copy(model);



        PurchaseNatureMixed.Validate = function () {
            var totalPercentage = parseFloat(this.totalPercentage);
            return totalPercentage === 0 || totalPercentage > 99.9 && totalPercentage < 100.1;
        };


        return PurchaseNatureMixed;

    };

    var DeliveryTimeInfoClass = function (object) {


        var deliveryTimeInfoClass = {};

        deliveryTimeInfoClass.Id = null;
        deliveryTimeInfoClass.Count = null;
        deliveryTimeInfoClass.DayPlus = 0;
        deliveryTimeInfoClass.DeliveryTimePeriod = null;
        deliveryTimeInfoClass.DateStart = null;
        deliveryTimeInfoClass.DateEnd = null;
        deliveryTimeInfoClass.StartOpened = false;
        deliveryTimeInfoClass.EndOpened = false;
        deliveryTimeInfoClass.DateStartOptions = { minDate: null, maxDate: null };
        deliveryTimeInfoClass.DateEndOptions = { minDate: null };


        deliveryTimeInfoClass.StartOpen = function () {
            this.StartOpened = !this.StartOpened;
        };

        deliveryTimeInfoClass.EndOpen = function () {
            this.EndOpened = !this.EndOpened;
        };
        deliveryTimeInfoClass.ChangeDays = function () {
            deliveryTimeInfoClass.DateEnd = new Date(deliveryTimeInfoClass.DateStart);
            deliveryTimeInfoClass.DateEnd.setDate(deliveryTimeInfoClass.DateEnd.getDate() + 1 * deliveryTimeInfoClass.DayPlus);

           // deliveryTimeInfoClass.DateEnd = new Date(object.DateEnd).getUTCTime();
        };


        if (object !== null && object !== undefined) {
            deliveryTimeInfoClass.Id = object.Id;
            deliveryTimeInfoClass.Count = object.Count;
            deliveryTimeInfoClass.DeliveryTimePeriod = object.DeliveryTimePeriod;
            deliveryTimeInfoClass.DateStart = new Date(object.DateStart).getUTCTime();
            deliveryTimeInfoClass.DateEnd = new Date(object.DateEnd).getUTCTime();
        }

        deliveryTimeInfoClass.changeMinAndMaxDates = function () {


            if (deliveryTimeInfoClass.DateEnd !== null) {
                deliveryTimeInfoClass.DateStartOptions.maxDate = new Date(deliveryTimeInfoClass.DateEnd).getUTCTime();
            } else {
                deliveryTimeInfoClass.DateStartOptions.maxDate = null;
            }


            if ($scope.selectedPurchase.DateEndFormat !== null) {
                deliveryTimeInfoClass.DateStartOptions.minDate = new Date($scope.selectedPurchase.DateEndFormat).getUTCTime();
            }

            if (deliveryTimeInfoClass.DateStart !== null) {
                deliveryTimeInfoClass.DateEndOptions.minDate = new Date(deliveryTimeInfoClass.DateStart).getUTCTime();
            } else {
                deliveryTimeInfoClass.DateEndOptions.minDate = null;
            }
        };

        deliveryTimeInfoClass.changeMinAndMaxDates();

        return deliveryTimeInfoClass;

    };

    Date.prototype.getUTCTime = function () {
        var userTimezoneOffset = this.getTimezoneOffset() * 60000;
        return new Date(this.getTime() - userTimezoneOffset);
    };




    //Загрузка
    $scope.Load = function () {
        loadPurchases();

        var p_ReestrNumber = $route.current.params["ReestrNumber"];
        var p_PurchaseNumber = $route.current.params["PurchaseNumber"];
        if (p_ReestrNumber !== undefined || p_PurchaseNumber !== undefined) {
            setPurchases();
            //sleep(1);
        }

        var filter = {
            Id: null,
            Number: null,
            ReestrNumber: null,
            LastChangedPurchaseUser: null,
            LastChangedLotUser: null,
            LastChangedObjectReadyUser: null,
            City: null,
            District: null,
            FederalDistrict: null,
            FederationSubject: null,
            DateStart: null,
            DateEnd: null,
            Name: null,
            NatureId: null,
            FundingId: null,
            PurchaseClassId: null
        };
        if (p_PurchaseNumber !== undefined) {
            filter.Number = p_PurchaseNumber;

        }
        if (p_ReestrNumber !== undefined) {
            filter.ReestrNumber = p_ReestrNumber;
        }

        if (p_ReestrNumber !== undefined || p_PurchaseNumber !== undefined) {
            $scope.LoadFilterurl(filter);
        }
    };

    $scope.purchasesGridLoad = function (value) {
        $scope.purchasesGrid.Options.data = value;
        if (value.length > 0) {
            $scope.canGetPurchases = false;
            $scope.canSetPurchases = true;
        } else {
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
        }
        if ($scope.purchasesGrid.Options.data.length > 0) {
            changeSelectedPurchase($scope.purchasesGrid.Options.data[0].Id);
        }
    };

    $scope.LoadFilterurl = function (ex_filter) {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            data: JSON.stringify({ filter: ex_filter }),
            url: '/GovernmentPurchases/GetPurchasesByFilter/'
        }).then(function (response) {
            $scope.purchasesGridLoad(response.data);
        }, function () {
            $scope.purchasesGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
        });
    };

    $scope.Load();

    $scope.CreateNewPurchase = function () {
        $scope.purchasesInWorkLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/CreateNewPurchase/'
        }).then(function (response) {
            $scope.purchasesGridLoad(response.data);
        }, function () {
            $scope.purchasesGrid.Options.data = [];
            $scope.message = 'Unexpected error';
            messageBoxService.showError($scope.message);
            $scope.canGetPurchases = true;
            $scope.canSetPurchases = false;
        });
    }
    $scope.KBK_set = function (isSet) {
        var ContractIds = [];        
        ContractIds.push($scope.CurrentContractId);

        var json = JSON.stringify({ model: ContractIds, isSet: isSet });

        $scope.classifierSave =
            $http({
                method: 'POST',
                url: '/CheckContract/Contract_check_ContractPaymentStage_KBK_Set/',
                data: json
            }).then(function (response) {
                changeSelectedContract($scope.CurrentContractId);                   

                alert("Сделал.");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.LinkOrganization = function (Id) {
        if (Id >= 0) {
            window.open('/#/GovernmentPurchases/OrganizationsEditor?Id=' + Id, '_blank');
        }
    };


}

