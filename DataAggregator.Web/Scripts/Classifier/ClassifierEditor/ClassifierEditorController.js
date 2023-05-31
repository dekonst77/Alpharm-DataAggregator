angular
    .module('DataAggregatorModule')
    .controller('ClassifierEditorController', [
        '$scope', '$route', '$http', '$uibModal', '$timeout', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', ClassifierEditorController]);


function ClassifierEditorController($scope, $route, $http, $uibModal, $timeout, messageBoxService, uiGridCustomService, errorHandlerService, formatConstants) {
    $scope.InnerCods = { OwnerTradeMarkId: '', PackerId: '', DrugId: '', OnwerRegistrationCertificateId: '' };

    $scope.InnerCodsShow = {

        isNewID: function (id) {
            if (id > 0)
                return id;
            else
                return "новый";
        },


        get OwnerTradeMarkId() {
            return this.isNewID($scope.InnerCods.OwnerTradeMarkId);
        },
        get PackerId() {
            return this.isNewID($scope.InnerCods.PackerId);
        },
        get DrugId() {
            return this.isNewID($scope.InnerCods.DrugId);
        },
        get OnwerRegistrationCertificateId() {
            return this.isNewID($scope.InnerCods.OnwerRegistrationCertificateId);

        }
    };

    $scope.isEditor = $route.current.isEditor;


    var InnGroupDosage = function () {

        this.Dosage = { Value: null, Id: 0 };

        this.INN = { Value: null, Id: 0 };

        this.DosageCount = null;
    };

    $scope.decimalPattern = '^\\d{0,9}(\\.\\d{1,9})?$';
    $scope.integerPattern = '^\\d{0,9}$';

    //Изменить регистрационный сертификат

    $scope.changeUse = function () {
        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/ChangeUse/',
                data: json
            }).then(function (response) {
                var responseData = response.data;

                if (!responseData.Success) {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

                loadClassifier();

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });


    }

    $scope.changeCert = function () {

        var json = JSON.stringify({ certificate: $scope.classifier.RegistrationCertificate });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/EditRegistrationCertificate/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (!responseData.Success) {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    //Очистить поля регистрационного сертификата
    $scope.clearCert = function (item) {
        //$scope.classifier.DrugId = 0;
        //$scope.classifier.DrugType = null;
        $scope.classifier.RegistrationCertificate = new RegistrationCertificateClass();
        $scope.classifier.IsExchangeable = false;
        $scope.classifier.IsReference = false;
    };


    var RealPacking = function () {

        this.realPackingCount = { RealPackingCount: null, Id: 0 };
    };


    var RegistrationCertificateClass = function (object) {

        this.DatePickerRegistrationDate = new dateClass();
        this.DatePickerReissueDate = new dateClass();
        this.DatePickerExpDate = new dateClass();
        this.Number = null;
        this.CirculationPeriod = { Id: null, Value: null };
        this.Id = 0;
        this.OwnerRegistrationCertificate = { Id: null, Value: null };
        this.IsBlocked = true;


        if (object != null) {
            this.Id = object.Id;
            this.Number = object.Number;
            this.CirculationPeriod = object.CirculationPeriod;
            this.OwnerRegistrationCertificate = object.OwnerRegistrationCertificate;
            this.OwnerRegistrationCertificateId = object.OwnerRegistrationCertificateId;
            if (object.RegistrationDate != null)
                this.DatePickerRegistrationDate.Value = new Date(object.RegistrationDate);
            if (object.ReissueDate != null)
                this.DatePickerReissueDate.Value = new Date(object.ReissueDate);
            if (object.ExpDate != null)
                this.DatePickerExpDate.Value = new Date(object.ExpDate);
            this.IsBlocked = object.IsBlocked;
            this.StorageLife = object.StorageLife;
        } else {
            this.Id = null;
        }

        Object.defineProperty(this, 'RegistrationDate', {
            get: function () {
                return this.DatePickerRegistrationDate.Value;
            },
            set: function (value) {
                this.DatePickerRegistrationDate.Value = value;
            },
            enumerable: true
        });

        Object.defineProperty(this, 'ReissueDate', {
            get: function () {
                return this.DatePickerReissueDate.Value;
            },
            set: function (value) {
                this.DatePickerReissueDate.Value = value;
            },
            enumerable: true

        });

        Object.defineProperty(this, 'ExpDate', {
            get: function () {
                return this.DatePickerExpDate.Value;
            },
            set: function (value) {
                this.DatePickerExpDate.Value = value;
            },
            enumerable: true
        });

        this.isNull = function () {
            return !this.Number ||
                //!this.CirculationPeriod.Value &&
                //!this.DatePickerRegistrationDate.Value &&
                //!this.DatePickerReissueDate.Value &&
                //!this.DatePickerExpDate.Value ||
                !this.OwnerRegistrationCertificate.Value;

        };
    };



    $scope.classifierGrid = uiGridCustomService.createGridClass($scope, 'ClassifierEditor_ClassifierGrid');

    $scope.classifierGrid.Options.rowHeight = 20;
    $scope.classifierGrid.Options.columnDefs = [
        { name: 'Used', cellTemplate: '_icon.html', enableFiltering: false, enableSorting: false, width: 25 },
        { name: 'IsBlockedRC', cellTemplate: '_iconIsBlockedRC.html', enableFiltering: false, enableSorting: false, width: 25 },
        { name: 'РУ', field: 'RegistrationCertificateNumber', cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/Certificates?searchText={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Код препарата', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Торговое наименование', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
        { name: 'МНН', field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
        { name: 'ФВ + Ф + Д + К', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
        { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Правообладатель', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
        { name: 'PackerId', field: 'PackerId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Упаковщик', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
        { name: 'Цена', field: 'Price', filter: { condition: uiGridCustomService.condition } },
        { name: 'PackerLocalizationId', field: 'PackerLocalizationId', visible: false, filter: { condition: uiGridCustomService.condition } },
        { name: 'Локализация упаковщика', field: 'PackerLocalizationValue', visible: false, filter: { condition: uiGridCustomService.condition } },
        { name: 'ProductionLocalizationId', field: 'ProductionLocalizationId', visible: false, filter: { condition: uiGridCustomService.condition } },
        { name: 'Локализация', field: 'ProductionLocalizationValue', visible: false, filter: { condition: uiGridCustomService.condition } }
    ];

    $scope.classifierGrid.Options.multiSelect = false;
    $scope.classifierGrid.Options.modifierKeysToMultiSelect = false;

    $scope.classifierGrid.Options.rowTemplate = '_rowClassifierTemplate.html';
    $scope.classifierGrid.selectionChanged = function () {
        var item = $scope.classifierGrid.getSelectedItem();
        if (item.length === 1)
            loadSelectedClassifier(item[0]);
    };

    function copyClassifierPacking(pack) {
        var newPack = {
            Id: 0,
            ClassifierId: pack.ClassifierId,
            ConsumerPacking: { Id: pack.ConsumerPacking.Id, Value: pack.ConsumerPacking.Value },
            PrimaryPacking: { Id: pack.PrimaryPacking.Id, Value: pack.PrimaryPacking.Value },
            CountPrimaryPacking: pack.CountPrimaryPacking,
            CountInPrimaryPacking: pack.CountInPrimaryPacking,
            PackingDescription: pack.PackingDescription,
            IsBlisterPacking: false
        };

        return newPack;
    }

    // $scope.classifierPackingGrid ->
    $scope.classifierPackingGrid = uiGridCustomService.createGridClassMod($scope, 'ClassifierEditor_ClassifierPackingGrid');

    $scope.classifierPackingGrid.Options.rowHeight = 20;
    $scope.classifierPackingGrid.Options.multiSelect = false;
    $scope.classifierPackingGrid.Options.modifierKeysToMultiSelect = false;

    $scope.classifierPackingGrid.Options.columnDefs = [
        { name: 'Classifier Id', cellTooltip: true, field: 'ClassifierId', type: 'number', visible: false, nullable: true, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Drug packing ID', field: 'Id', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, width: '18%' },
        { name: 'В первичной упаковке', field: 'CountInPrimaryPacking', filter: { condition: uiGridCustomService.condition }, width: '18%' },
        { name: 'Кол-во первичных упаковок', field: 'CountPrimaryPacking', filter: { condition: uiGridCustomService.condition }, width: '18%' },
        { name: 'Пользовательская упаковка', field: 'ConsumerPacking.Value', filter: { condition: uiGridCustomService.condition }, width: '18%' },
        { name: 'Первичная упаковка', field: 'PrimaryPacking.Value', filter: { condition: uiGridCustomService.condition }, width: '18%' }
    ];

    if ($scope.isEditor)
        $scope.classifierPackingGrid.Options.columnDefs.push({ name: 'Блистеровка', field: 'IsBlisterPacking', type: 'boolean', width: 100, enableCellEdit: true, filter: { condition: uiGridCustomService.booleanConditionX }, width: '10%' });

    $scope.classifierPackingGrid.SetDefaults();

    $scope.classifierPackingGrid.Options.rowTemplate = '_rowClassifierTemplate.html';

    $scope.classifierPackingGrid.Options.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi; //set gridApi on scope

        // Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            var item = $scope.gridApi.selection.getSelectedRows();

            //console.log('$scope.classifierPackingGrid.selectionChanged()');
            //console.log(item);

            if (item.length === 1) {
                $scope.selectedClassifierPacking = item[0];

                $scope.currentPacking = copyClassifierPacking($scope.selectedClassifierPacking);
            }
        });

        gridApi.edit.on.afterCellEdit($scope, editRowDataSource);
    };

    // редактируемые поля: IsBlisterPacking
    function editRowDataSource(rowEntity, colDef, newValue, oldValue) {
        if (colDef.field !== 'IsBlisterPacking')
            return;

        if (newValue === oldValue || newValue === undefined)
            return;

        var json =
        {
            id: rowEntity.Id,
            ClassifierId: rowEntity.ClassifierId,
            newValue: newValue
        }
        //console.log(json);
        $scope.dataLoading = $http({
            method: "POST",
            url: "/ClassifierEditor/ClassifierPackingEdit/",
            data: JSON.stringify(json)
        }).then(function (response) {

            if (response.status === 200) {
                //console.log(response);

                $scope.classifierPackingGrid.Options.data = response.data;
            }
            else {
                console.error(data.ErrorMessage);
                messageBoxService.showError(data.ErrorMessage);
            }

            return true;
        }, function (response) {
            rowEntity[colDef.field] = oldValue;
            errorHandlerService.showResponseError(response);
            return false;
        });
    }
    // $scope.classifierPackingGrid <-

    $scope.classifier = {
        RegistrationCertificate: new RegistrationCertificateClass(),
        InnGroupDosage: [],
        RealPackingList: [],
        ClassifierPackings: [],
        DosageValue: { Id: null, Value: null },
        TotalVolume: { Id: null, Value: null },
        TradeName: { Id: null, Value: null },
        OwnerTradeMark: { Id: null, Value: null },
        Packer: { Id: null, Value: null },
        СlassifierId: null,
        DrugType: null,
        Used: true,
        Comment: "",
        Nfc: null,
        ATCWho: null,
        Generic: null,
        ATCEphmra: null,
        ATCBaa: null,
        FTG: null,
        IsOtc: null,
        IsRx: null,
        Brand: null,
        ProductionStage: null,
        ProductionLocalization: null,
        PackerLocalization: null
    };

    $scope.checkRx = function (type_otc) {
        if (type_otc === "Otc") {
            if ($scope.classifier.IsOtc === true) {
                $scope.classifier.IsRx = false;
            }
        }
        if (type_otc === "Rx") {
            if ($scope.classifier.IsRx === true) {
                $scope.classifier.IsOtc = false;
            }
        }
    }

    $scope.checkExRef = function (type) {
        if (type === "IsExchangeable") {
            if ($scope.classifier.IsExchangeable === true) {
                $scope.classifier.IsReference = false;
            }
        }
        if (type === "IsReference") {
            if ($scope.classifier.IsReference === true) {
                $scope.classifier.IsExchangeable = false;
            }
        }
    }


    $scope.removeInn = function (item) {
        $scope.classifier.InnGroupDosage.removeitem(item);
    };

    $scope.addInn = function (item) {
        $scope.classifier.InnGroupDosage.push(new InnGroupDosage());
    };

    $scope.addRealPacking = function (item) {

        if ($scope.loadConsumerPackingCount != null && $scope.loadConsumerPackingCount != $scope.classifier.ConsumerPackingCount) {
            $scope.classifier.RealPackingList = [];
        }

        $scope.classifier.RealPackingList.push(new RealPacking());
    };

    $scope.removeRealPacking = function (item) {
        $scope.classifier.RealPackingList.removeitem(item);
    };

    //Для выпадающего списка при клике на ATCWho, ATCEpmra, FTG
    $scope.onFocus = function (e) {
        $timeout(function () {
            $(e.target).trigger('input');
            $(e.target).trigger('change'); // for IE
        });
    }

    $scope.getListATCBaa = function (value) {
        if (value == "" || value == null)
            return $scope.ATCBaaList.slice(0, 20);
        return $scope.ATCBaaList.filter(item => item.Value.toLowerCase().includes(value.toLowerCase())).slice(0, 20);
    }

    $scope.getListFTG = function (value) {
        if (value == "" || value == null)
            return $scope.FTGList.slice(0, 20);
        return $scope.FTGList.filter(item => item.Value.toLowerCase().includes(value.toLowerCase())).slice(0, 20);
    }

    $scope.getListATCWho = function (value) {
        if (value == "" || value == null)
            return $scope.ATCWhoList.slice(0, 20);
        return $scope.ATCWhoList.filter(item => item.Value.toLowerCase().includes(value.toLowerCase())).slice(0, 20);
    }

    $scope.getListATCEphmra = function (value) {
        if (value == "" || value == null)
            return $scope.ATCEphmraList.slice(0, 20);
        return $scope.ATCEphmraList.filter(item => item.Value.toLowerCase().includes(value.toLowerCase())).slice(0, 20);
    }

    $scope.setAtcWho = function (dictionaryItem) {
        if ($scope.classifier.ATCWho == null)
            return;
        $scope.classifier.ATCWho.Value = dictionaryItem.Value;
        $scope.classifier.ATCWho.Id = dictionaryItem.Id;



        //$scope.classifier.AtcWho = { Id: dictionaryItem.Id, Value: dictionaryItem.Value }
    }

    $scope.searchDictionary = function (value, dictionary) {
        //$scope.classifier[dictionary + 'Id'] = null;
        return $http({
            method: 'POST',
            url: '/Dictionary/GetDictionary/',
            data: JSON.stringify({ Value: value, Dictionary: dictionary })
        }).then(function (response) {
            return response.data;
        });
    };

    $scope.setId = function (dictionaryItem, item) {
        item.Id = dictionaryItem.Id;
        item.Value = dictionaryItem.Value;
    };

    $scope.setLocalization = function (Packeritem) {
        //console.debug('обновим локализацию упаковщика');
        //console.debug(Packeritem);

        // обновим локализацию упаковщика
        $scope.message = 'обновим локализацию упаковщика';
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Dictionary/GetLocalizationByManufacturer/',
            data: JSON.stringify({ Id: Packeritem.Id })
        }).then(function (response) {
            //console.debug(response);

            $scope.classifier.PackerLocalization = response.data;

            $scope.LoadFilterLocalization();

            return response.data;
        });
    };

    $scope.clearId = function (item, value) {
        if (item != null)
            item.Id = null;
    };

    $scope.classifierFilter = new Object;

    function clearForm() {
        $scope.selectedClassifierPacking = null;
        $scope.currentPacking = null;
        $scope.InnerCods = { OwnerTradeMarkId: '', PackerId: '', DrugId: '', OnwerRegistrationCertificateId: '' };
    }

    // результат фильтра - получить список данных классификатора
    function loadClassifier(newItem) {

        clearForm();

        if (isFilterEmpty()) {
            $scope.classifierGrid.Options.data = [];
            return;
        }

        var json = JSON.stringify($scope.classifierFilter);

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/GetClassifierEditorView/',
                data: json
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.classifierGrid.Options.data = data.Data;
                    if (newItem == null && $scope.classifierGrid.Options.data.length > 0) {
                        loadSelectedClassifier($scope.classifierGrid.Options.data[0]);
                    }
                    else {
                        //Добавим новый элемент в список, если его там нету
                        if (newItem != null && !$scope.classifierGrid.Options.data.some((i) => {
                            return i.ClassifierId == newItem.ClassifierId
                        })) {
                            $scope.classifierGrid.Options.data.push(newItem);
                        };

                        loadSelectedClassifier(newItem);
                    }

                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    function isFilterEmpty() {
        return $scope.classifierFilter.tradeName == null &&
            $scope.classifierFilter.ownerTradeMark == null &&
            $scope.classifierFilter.ownerTradeMark == null &&
            $scope.classifierFilter.innGroup == null &&
            $scope.classifierFilter.packer == null &&
            $scope.classifierFilter.formProduct == null &&
            $scope.classifierFilter.registrationNumber == null &&
            $scope.classifierFilter.dosageGroup == null &&
            $scope.classifierFilter.consumerPackingCount == null &&
            $scope.classifierFilter.DrugId == null &&
            $scope.classifierFilter.ownerTradeMarkId == null &&
            $scope.classifierFilter.classifierId == null;
    }


    function canAdd3() {




        ///Без Ру может быть только у типа 1
        if ($scope.classifier.WithoutRegistrationCertificate && $scope.classifier.DrugType.Id != 1) {
            return false;
        }



        //У Субстанций проверка не осуществляется
        if ($scope.classifier.DrugType.Id == 4) {
            return true;
        }


        //У Бадов и других не может быть регистрационного сертификата
        if ($scope.classifier.DrugType.Id != 1) {
            //return $scope.classifier.RegistrationCertificate == null;
            return $scope.classifier.RegistrationCertificate.isNull();
        } else {

            //Если это ЛС то сертификат должен быть кроме случая когда Производитель и упаковщик Unknown
            if ($scope.classifier.OwnerTradeMark.Value == 'Unknown' && $scope.classifier.Packer.Value == 'Unknown') {
                return $scope.classifier.RegistrationCertificate.isNull() && !$scope.classifier.WithoutRegistrationCertificate;
            } else {

                //Должно быть выбрано или Ру или Без Ру
                if ($scope.classifier.RegistrationCertificate.isNull() && !$scope.classifier.WithoutRegistrationCertificate) {
                    return false;
                }

                //Должно быть выбрано или Ру или Без Ру
                if (!$scope.classifier.RegistrationCertificate.isNull() && $scope.classifier.WithoutRegistrationCertificate) {
                    return false;
                }

                return true;
            }
        }
    }


    $scope.checkCertificate = function () {

        if ($scope.classifier.DrugType == null)
            return false;

        return canAdd3();
    };

    $scope.canAdd = function () {
        if ($scope.classifier.DrugType == null)
            return false;

        var canAdd = $scope.classifier.ClassifierPackings != null && $scope.classifier.ClassifierPackings.length > 0;

        var canAddRegOwner = $scope.classifier.RegistrationCertificate.isNull() ||
            ($scope.classifier.RegistrationCertificate.OwnerRegistrationCertificate.Value);

        return canAdd && canAdd3() && canAddRegOwner;
    };
    $scope.changeListATS = function () {
        $scope.loadATCWhoList();
        $scope.loadGeneric();
        $scope.loadATCEphmraList();
        $scope.loadATCBaaList();
        $scope.loadFTGList();
    };

    $scope.loadConsumerPackingCount = null;

    //Загружаем выбранный классификатор
    function loadSelectedClassifier(item) {

        clearForm();

        if (item == null || item.DrugId == null || item.ClassifierId == null)
            return;

        var json = JSON.stringify({ ClassifierId: item.ClassifierId });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/LoadClassifier/',
                data: json
            }).then(function (response) {
                var data = response.data;
                $scope.classifier = data;

                $scope.classifier.RegistrationCertificate = new RegistrationCertificateClass(data.RegistrationCertificate);

                if ($scope.classifier.RegistrationCertificate.OwnerRegistrationCertificate != null)
                    $scope.InnerCods.OnwerRegistrationCertificateId = $scope.classifier.RegistrationCertificate.OwnerRegistrationCertificate.Id;

                $scope.loadConsumerPackingCount = $scope.classifier.ConsumerPackingCount;

                $scope.classifier.Nfc = { Id: data.Nfc.Id };
                $scope.classifier.ATCWho = data.ATCWho;
                $scope.classifier.Generic = { Id: data.Generic.Id };
                $scope.classifier.ATCEphmra = data.ATCEphmra;
                $scope.classifier.ATCBaa = data.ATCBaa;
                $scope.classifier.FTG = data.FTG;
                $scope.classifier.IsOtc = data.IsOtc;
                $scope.classifier.IsRx = data.IsRx;
                $scope.classifier.Brand = data.Brand;
                $scope.classifier.ProductionStage = data.ProductionStage;
                $scope.classifier.ProductionLocalization = data.ProductionLocalization.Id == 0 ? null : data.ProductionLocalization;
                $scope.classifier.PackerLocalization = data.PackerLocalization;

                $scope.InnerCods.DrugId = $scope.classifier.DrugId;
                $scope.InnerCods.OwnerTradeMarkId = $scope.classifier.OwnerTradeMarkId;
                $scope.InnerCods.PackerId = $scope.classifier.PackerId;

                $scope.classifierPackingGrid.Options.data = $scope.classifier.ClassifierPackings;

                if ($scope.classifier.ClassifierPackings != null && $scope.classifier.ClassifierPackings.length > 0) {

                    $timeout(function () {
                        if ($scope.gridApi.selection.selectRow) {
                            $scope.gridApi.selection.selectRow($scope.classifierPackingGrid.Options.data[0]);
                        }
                    });

                }

                changeDescription();

                $scope.loadNfcList();
                $scope.changeListATS();
                $scope.LoadFilterLocalization();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.addClassifierPacking = function () {
        var newClassifierPacking = copyClassifierPacking($scope.currentPacking);

        //console.log(newClassifierPacking);

        var newConsumerPackingCount = newClassifierPacking.CountInPrimaryPacking * newClassifierPacking.CountPrimaryPacking;

        if ($scope.classifier.ConsumerPackingCount == newConsumerPackingCount) {
            $scope.classifier.ClassifierPackings.push(newClassifierPacking);
        } else {
            $scope.classifier.ConsumerPackingCount = newConsumerPackingCount;
            $scope.classifier.ClassifierPackings = [newClassifierPacking];
        }

        $scope.classifierPackingGrid.Options.data = $scope.classifier.ClassifierPackings;

        //console.log($scope.classifier.ClassifierPackings);
    };

    $scope.removeClassifierPacking = function () {
        //console.log('до удаления:');
        //console.log($scope.selectedClassifierPacking);

        $scope.classifier.ClassifierPackings = $scope.classifier.ClassifierPackings.filter(val => val.Id !== $scope.selectedClassifierPacking.Id);

        //console.log('после удаления:');
        //console.log($scope.selectedClassifierPacking);

        $scope.classifierPackingGrid.Options.data = $scope.classifier.ClassifierPackings;
    };

    $scope.canShowHistory = function () {
        return $scope.classifier.DrugId > 0;
    }

    //Поиск регистрационного удостоверения
    $scope.ShowHistory = function () {
        $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/ClassifierEditor/_ClassifierEditorHistory.html',
            controller: 'ClassifierEditorHistoryController',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                filter: {
                    ClassifierId: $scope.classifier.ClassifierId,
                    DrugId: $scope.classifier.DrugId,
                    OwnerTradeMarkId: $scope.classifier.OwnerTradeMarkId,
                    PackerId: $scope.classifier.PackerId
                },
            }
        });
    };



    //Поиск регистрационного удостоверения
    $scope.searchRegCert = function () {

        var modalClassifieInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/ClassifierEditor/_SearchRegistrationCertificate.html',
            controller: 'SearchRegistrationController',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                regNumber: function () {
                    if ($scope.classifier.RegistrationCertificate != null)
                        return $scope.classifier.RegistrationCertificate.Number;

                    return null;
                }
            }
        });

        modalClassifieInstance.result.then(function (regCert) {

            $scope.classifier.RegistrationCertificate = new RegistrationCertificateClass(regCert);

            if ($scope.classifier.RegistrationCertificate.OwnerRegistrationCertificate != null)
                $scope.InnerCods.OnwerRegistrationCertificateId = $scope.classifier.RegistrationCertificate.OwnerRegistrationCertificate.Id;

        }, function () {
        });
    };

    // клик по кнопке "Поиск по справочнику"
    $scope.searchClassifier = function () {

        var modalClassifieInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/ClassifierEditor/_ClassifierEditorFilterView.html',
            controller: 'ClassifierEditorFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                classifierFilter: function () {
                    return $scope.classifierFilter;
                }
            }
        });

        modalClassifieInstance.result.then(function (classifierFilter) {
            $scope.classifierFilter = classifierFilter;
            loadClassifier();
        }, function (classifierFilter) {
        });
    };

    //Провереяем классификатор на изменения
    $scope.CheckClassifier = function () {

        //console.log($scope.classifier);

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/CheckClassifier/',
                data: json
            }).then(function (response) {
                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;
                    $scope.InnerCods.DrugId = data.DrugId;
                    $scope.InnerCods.OwnerTradeMarkId = data.OwnerTradeMarkId;
                    $scope.InnerCods.PackerId = data.PackerId;
                    $scope.InnerCods.OnwerRegistrationCertificateId = data.OwnerRegistrationCertificateId;
                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    //Запуск процесса изменения в классификаторе
    changeNfc = function () {
        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/ChangeNfc/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (!responseData.Success)
                    messageBoxService.showError(responseData.ErrorMessage);

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    //Провереяем классификатор на изменения
    $scope.AddClassifierAsk = function () {
        messageBoxService.showConfirm('Вы уверены, что хотите добавить новый препарат?', 'Добавление')
            .then(tryAddClassifier, function () { });
    };

    $scope.MergeClassifierAsk = function (drugDesription) {
        messageBoxService.showConfirm('Объединить с существующим DrugId?\n' + drugDesription, 'Изменение')
            .then(MergeClassifier, function () { });
    };

    function showMessage(text) {
        messageBoxService.showInfo(text, 'Изменение');
    }

    //Запуск процесса изменения в классификаторе
    $scope.EditClassifier = function () {
        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/GetChanges/',
                data: json
            }).then(function (response) {
                var responseData = response.data;

                if (responseData.Success) {

                    var data = responseData.Data;

                    if (data.Items.length > 0 ||
                        data.RealPackingCount.length > 0 ||
                        data.ClassifierPacking.length > 0 ||
                        data.RegistrationCertificate.length > 0 ||
                        data.RegistrationCertificateIsBlocked.length > 0 ||
                        data.ChangeUse.length > 0)
                        ShowChanges(data, true);
                    else
                        showMessage('Изменений не обнаружено');

                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }).catch(function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    //Показываем что изменилось и спрашиваем продолжить или нет
    function ShowChanges(data, isTry) {
        var modalInstance =
            $uibModal.open({
                animation: true,
                templateUrl: 'Views/Classifier/ClassifierEditor/_ChangeInfoView.html',
                controller: 'ChangeInfoController',
                size: 'lg',
                //windowClass: 'center-modal',
                backdrop: 'static',
                resolve: {
                    classifierInfo: data,
                    canCancel: isTry
                }
            });

        modalInstance.result.then(function (changeInfo) {
            //Если это была пробная попытка, и пользователь выбрал ок, то вызываем добавление
            if (isTry)
                //если поменяли только блокировку сертификата
                if (changeInfo.ClassifierPacking.length === 0 && changeInfo.Items.length === 0 && changeInfo.RealPackingCount.length === 0 && changeInfo.RegistrationCertificate.length === 0 && changeInfo.RegistrationCertificateIsBlocked.length > 0) {
                    editClassifierAsk(1);
                } else if (changeInfo.ClassifierPacking.length === 0 && changeInfo.Items.length === 0 && changeInfo.RealPackingCount.length === 0 && changeInfo.RegistrationCertificate.length === 0 && changeInfo.ChangeUse.length > 0) {
                    editClassifierAsk(3);
                } else {
                    editClassifierAsk(2);
                }
            else {
                //Если это финальное добавление, то загружаем изменения
                loadSelectedClassifier(data);
                (JSON.stringify($scope.classifierFilter));
            }
        }, function () {
            //Отменили выполнение
        });
    }

    function editClassifierAsk(mode) {

        switch (mode) {
            case 1:
                messageBoxService.showConfirm('Изменяем только блокировку сертификата регистрации?', 'Изменение')
                    .then($scope.changeCert, function () {
                    });
                break;
            case 2:
                //Спрашиваем как изменять с новым DrugId или старым
                messageBoxService.showConfirm('Изменить с новым DrugId?', 'Изменение')
                    .then(checkRecreate, function (result) {
                        if (result === 'no')
                            changeClassifier();
                    });
                break;
            case 3:
                messageBoxService.showConfirm('Изменяем только блокировку?', 'Изменение')
                    .then($scope.changeUse, function () {
                    });
                break;
        }
    }

    function checkSaveClassifier() {
        messageBoxService.showConfirm('Сохранить ClassifierId?', 'Изменение')
            .then(checkRecreate,
                function (result) {
                    if (result === 'no')
                        changeClassifier();
                });

    }

    //Проверяем можем ли мы пересоздать препарат, и не потребуется ли объединение с другим препаратом
    function checkRecreate() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/CheckRecreate/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {

                    var data = responseData.Data;

                    if (!data.CanRecreate) {
                        showMessage('Нельзя объединять в пределах одного DrugId');
                        return;
                    }

                    if (data.NeedMerge) {
                        $scope.MergeClassifierAsk(data.DrugDescription);
                    } else {
                        ReCreateClassifier(false);
                    }
                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    //Объединяем с другим препаратом
    function MergeClassifier() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/MergeClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;
                    loadSelectedClassifier(data);
                    loadClassifier();
                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    //Пересоздаем препарат с новым ClassifierId
    function ReCreateClassifier(saveClassifierId) {

        var json = JSON.stringify({ model: $scope.classifier, saveClassifierId: saveClassifierId });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/ReCreateClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;

                    loadSelectedClassifier(data);
                    loadClassifier();

                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    //Пробное добавление классификатора, чтобы узнать что изменилось
    function tryAddClassifier() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/TryAddClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;
                    ShowResult(data, true, null);

                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    //Запуск процесса изменения классификатора
    function changeClassifier() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/ChangeClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {

                    loadSelectedClassifier(responseData.Data);
                    loadClassifier();

                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    // Добавить классификатор
    function AddClassifier() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/AddClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;
                    ShowResult(data.data, false, data.itemView);
                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    // Удалить ЛП
    function DeleteDrug(data) {

        var json = JSON.stringify({ model: data });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/DeleteDrug/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (!responseData.Success) {
                    messageBoxService.showError(responseData.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }


    // Показать результат добавления
    function ShowResult(data, isTry, newItem) {

        var modalInstance =
            $uibModal.open({
                animation: true,
                templateUrl: 'Views/Classifier/ClassifierEditor/_AddClassifierInfoView.html',
                controller: 'AddClassifierInfoController',
                size: 'lg',
                //windowClass: 'center-modal',
                backdrop: 'static',
                resolve: {
                    classifierInfo: data,
                    canCancel: isTry
                }
            });

        modalInstance.result.then(function () {
            // Если это была пробная попытка и пользователь выбрал ОК, то вызываем добавление
            if (isTry)
                AddClassifier();
            else {
                // Если это финальное добавление, то загружаем изменения.
                // Если после загрузки добавленного элемента нет, добавим его вручную.
                loadClassifier(newItem);
            }
        }, function () { // Отменили выполнение: удалим Drug
            DeleteDrug(data);
        });
    }

    //Функция, которая изменяет описания
    function changeDescription() {
        $scope.classifier.InnGroupDosageDescription = getInnGroupDescription();
        $scope.classifier.DosageGroupDescription = getDosageGroupDescritpion();
        $scope.classifier.ShortDosageGroupDescription = getShortDosageGroupDescription();
    }

    //Получаем короткое описание Дозировки
    function getShortDosageGroupDescription() {
        var shortDescription = '';

        if ($scope.classifier.TotalVolumeCount != null && $scope.classifier.TotalVolumeCount != '~' && $scope.classifier.TotalVolumeCount.length > 0) {
            shortDescription = shortDescription + ' ' + $scope.classifier.TotalVolumeCount;
        }

        if ($scope.classifier.TotalVolume != null && $scope.classifier.TotalVolume.Value != null && $scope.classifier.TotalVolume.Value != '~' &&
            $scope.classifier.TotalVolume.Value.length > 0) {
            shortDescription = shortDescription + ' ' + $scope.classifier.TotalVolume.Value;
        }

        return shortDescription.trim();

    }

    //Получаем описание Дозировки
    function getDosageGroupDescritpion() {
        var description = '';

        for (i = 0; i < $scope.classifier.InnGroupDosage.length; i++) {
            var item = $scope.classifier.InnGroupDosage[i];

            var addDescription = '';

            if (item.DosageCount != null && item.DosageCount != '~' && item.DosageCount.length > 0) {
                addDescription = addDescription + item.DosageCount;
            }

            if (item.Dosage != null && item.Dosage.Value != null && item.Dosage.Value != '~' && item.Dosage.Value.length > 0) {
                addDescription = addDescription + ' ' + item.Dosage.Value;
            }

            if (description.length > 0 && addDescription.length > 0)
                addDescription = '+' + addDescription;

            if (addDescription != '+' && addDescription.length > 0)
                description = description + addDescription;
        }

        if (($scope.classifier.DosageValueCount != null && $scope.classifier.DosageValueCount != '~' && $scope.classifier.DosageValueCount.length > 0) ||
            ($scope.classifier.DosageValue != null && $scope.classifier.DosageValue.Value != null && $scope.classifier.DosageValue.Value != '~' &&
                $scope.classifier.DosageValue.Value.length > 0)) {

            description = '(' + description + ')';
        }


        if ($scope.classifier.DosageValueCount != null && $scope.classifier.DosageValueCount != '~' && $scope.classifier.DosageValueCount.length > 0) {
            description = description + '/' + $scope.classifier.DosageValueCount;
        }

        if ($scope.classifier.DosageValue != null && $scope.classifier.DosageValue.Value != null && $scope.classifier.DosageValue.Value != '~' &&
            $scope.classifier.DosageValue.Value.length > 0) {
            description = description + ' ' + $scope.classifier.DosageValue.Value;
        }

        if ($scope.classifier.TotalVolumeCount != null && $scope.classifier.TotalVolumeCount != '~' && $scope.classifier.TotalVolumeCount.length > 0) {
            description = description + ' ' + $scope.classifier.TotalVolumeCount;
        }

        if ($scope.classifier.TotalVolume != null && $scope.classifier.TotalVolume.Value != null && $scope.classifier.TotalVolume.Value != '~' &&
            $scope.classifier.TotalVolume.Value.length > 0) {
            description = description + ' ' + $scope.classifier.TotalVolume.Value;
        }

        return description.trim();
    }

    function groupBy(list, keyGetter) {
        const map = new Map();
        list.forEach((item) => {
            const key = keyGetter(item);
            const collection = map.get(key);
            if (!collection) {
                map.set(key, [item]);
            } else {
                collection.push(item);
            }
        });
        return map;
    }


    //Получаем описание МНН
    function getInnGroupDescription() {

        var description = '';


        //const groupById = groupBy('Id');
        //var ids = $scope.classifier.InnGroupDosage.filter((item) => { return item.INN != null; }).map((item) => { return item.INN; });
        //var group = groupById(ids);

        //if (group.length == 1 && group[0])
        var inns = $scope.classifier.InnGroupDosage.filter((item) => { return item.INN != null; });
        var grouped = groupBy(inns, item => {
            if (item.INN.Id > 0) {
                return item.INN.Id
            } else {
                return item.INN.Value
            }
        });

        if (grouped.size == 1) {

            var len = 0;
            var value = '';

            for (let elem of grouped.entries()) {
                len = elem[1].length;
                if (len > 1) {
                    value = elem[1][0].INN.Value;
                    return value.trim();
                }
            }
        }


        for (var i = 0; i < $scope.classifier.InnGroupDosage.length; i++) {
            var item = $scope.classifier.InnGroupDosage[i];



            if (item.INN != null && item.INN.Value != null && item.INN.Value.length > 0) {

                if (description.length > 0)
                    description = description + '+';

                description = description + item.INN.Value;
            }
        }

        if (description == '')
            description = '~';

        return description.trim();
    }

    $scope.clearKey = function (fieldKey) {
        $scope.InnerCods[fieldKey] = null;
    };


    //Отслеживаем изменения для формирования описания МНН и Дозировки.
    $scope.$watch(function ($scope) {
        var objects = $scope.classifier.InnGroupDosage.map(function (obj) {
            return { "INN": obj.INN.Value, "DosageCount": obj.DosageCount, "Dosage": obj.Dosage.Value }
        });

        objects.push({ "DosageCount": $scope.classifier.DosageValueCount, "Dosage": $scope.classifier.DosageValue.Value });

        objects.push({ "DosageCount": $scope.classifier.TotalVolumeCount, "Dosage": $scope.classifier.TotalVolume.Value });

        return objects;

    }, function (val) {
        changeDescription();
    }, true);

    $scope.drugTypeList = [];
    loadDrugType();


    function loadDrugType() {
        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/Dictionary/GetDrugTypes/'
            }).then(function (response) {
                $scope.drugTypeList = response.data;
            }, function () {
                $scope.message = 'Unexpected Error';
                messageBoxService.showError('Произошла ошибка при загрузке списка типов ЛС');
            });
    }

    loadProductionStage();

    function loadProductionStage() {
        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/Dictionary/LoadProductionStageList/'
            }).then(function (response) {
                $scope.ProductionStageList = response.data;
            }, function () {
                $scope.message = 'Unexpected Error';
                messageBoxService.showError('Произошла ошибка при загрузке списка типов ЛС');
            });
    }

    $scope.lastNfc = null;
    $scope.lastATCWho = null;
    $scope.lastATCEphmra = null;
    $scope.lastATCBaa = null;
    $scope.lastFTG = null;

    $scope.loadNfcList = function (selecedItem, event) {

        //при выборе значения из выпадающего списка сначала срабатывает ng-blur, потом typeahead-on-select.
        //при blur значение лекарственной формы ещё неизвестно, поэтому nfc сбрасывается.
        //приходится запоминать.
        if (event != null && event.type === 'blur') {
            $scope.lastNfc = $scope.classifier.Nfc;
        } else {
            if ($scope.lastNfc !== null) {
                $scope.classifier.Nfc = $scope.lastNfc;
            }
        }

        $scope.classifierLoading =
            $http({
                method: 'POST',
                data: {
                    formProductId: selecedItem ? selecedItem.Id : $scope.classifier.FormProduct ? $scope.classifier.FormProduct.Id : null,
                    currentNfcId: $scope.classifier.Nfc ? $scope.classifier.Nfc.Id : null,
                    formProductValue: $scope.classifier.FormProduct ? $scope.classifier.FormProduct.Value : null
                },
                url: '/ClassifierEditor/LoadNfcList/'
            }).then(function (response) {
                $scope.nfcList = response.data.NfcList;

                if (response.data.ShouldClearNfc) {
                    $scope.classifier.Nfc = null;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.loadGeneric = function (selectedItem, event) {

        $scope.classifierLoading =
            $http({
                method: 'POST',
                data: {
                    model: $scope.classifier
                },
                url: '/ClassifierEditor/LoadGenericList/'
            }).then(function (response) {
                $scope.GenericList = response.data.GenericList;
                $scope.GenericListToolTip = response.data.toolTip;

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.loadATCWhoList = function (selecedItem, event) {

        //при выборе значения из выпадающего списка сначала срабатывает ng-blur, потом typeahead-on-select.
        //при blur значение лекарственной формы ещё неизвестно, поэтому ATCWho сбрасывается.
        //приходится запоминать.
        //$scope.classifier.InnGroupDosage
        if (event != null && event.type === 'blur') {
            $scope.lastATCWho = $scope.classifier.ATCWho;
        } else {
            if ($scope.lastATCWho !== null) {
                $scope.classifier.ATCWho = $scope.lastATCWho;
            }
        }

        $scope.classifierLoading =
            $http({
                method: 'POST',
                data: {
                    InnGroupDosage: $scope.classifier.InnGroupDosage,
                    currentATCWhoId: $scope.classifier.ATCWho ? $scope.classifier.ATCWho.Id : null,
                    formProductValue: $scope.classifier.FormProduct ? $scope.classifier.FormProduct.Value : null
                },
                url: '/ClassifierEditor/LoadATCWhoList/'
            }).then(function (response) {
                $scope.ATCWhoList = response.data.ATCWhoList;


                if (response.data.ShouldClearATCWho) {
                    $scope.classifier.ATCWho = null;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.loadATCEphmraList = function (selecedItem, event) {

        //при выборе значения из выпадающего списка сначала срабатывает ng-blur, потом typeahead-on-select.
        //при blur значение лекарственной формы ещё неизвестно, поэтому ATCEphmra сбрасывается.
        //приходится запоминать.
        if (event != null && event.type === 'blur') {
            $scope.lastATCEphmra = $scope.classifier.ATCEphmra;
        } else {
            if ($scope.lastATCEphmra !== null) {
                $scope.classifier.ATCEphmra = $scope.lastATCEphmra;
            }
        }

        $scope.classifierLoading =
            $http({
                method: 'POST',
                data: {
                    InnGroupDosage: $scope.classifier.InnGroupDosage,
                    currentATCEphmraId: $scope.classifier.ATCEphmra ? $scope.classifier.ATCEphmra.Id : null,
                    formProductValue: $scope.classifier.FormProduct ? $scope.classifier.FormProduct.Value : null
                },
                url: '/ClassifierEditor/LoadATCEphmraList/'
            }).then(function (response) {
                $scope.ATCEphmraList = response.data.ATCEphmraList;

                if (response.data.ShouldClearATCEphmra) {
                    $scope.classifier.ATCEphmra = null;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.loadATCBaaList = function (selecedItem, event) {

        //при выборе значения из выпадающего списка сначала срабатывает ng-blur, потом typeahead-on-select.
        //при blur значение лекарственной формы ещё неизвестно, поэтому ATCBaa сбрасывается.
        //приходится запоминать.
        if (event != null && event.type === 'blur') {
            $scope.lastATCBaa = $scope.classifier.ATCBaa;
        } else {
            if ($scope.lastATCBaa !== null) {
                $scope.classifier.ATCBaa = $scope.lastATCBaa;
            }
        }

        $scope.classifierLoading =
            $http({
                method: 'POST',
                data: {
                    InnGroupDosage: $scope.classifier.InnGroupDosage,
                    currentATCBaaId: $scope.classifier.ATCBaa ? $scope.classifier.ATCBaa.Id : null,
                    formProductValue: $scope.classifier.FormProduct ? $scope.classifier.FormProduct.Value : null
                },
                url: '/ClassifierEditor/LoadATCBaaList/'
            }).then(function (response) {
                $scope.ATCBaaList = response.data.ATCBaaList;

                if (response.data.ShouldClearATCBaa) {
                    $scope.classifier.ATCBaa = null;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.loadFTGList = function (selecedItem, event) {

        //при выборе значения из выпадающего списка сначала срабатывает ng-blur, потом typeahead-on-select.
        //при blur значение лекарственной формы ещё неизвестно, поэтому FTG сбрасывается.
        //приходится запоминать.
        if (event != null && event.type === 'blur') {
            $scope.lastFTG = $scope.classifier.FTG;
        } else {
            if ($scope.lastFTG !== null) {
                $scope.classifier.FTG = $scope.lastFTG;
            }
        }

        $scope.classifierLoading =
            $http({
                method: 'POST',
                data: {
                    InnGroupDosage: $scope.classifier.InnGroupDosage,
                    currentFTGId: $scope.classifier.FTG ? $scope.classifier.FTG.Id : null,
                    formProductValue: $scope.classifier.FormProduct ? $scope.classifier.FormProduct.Value : null
                },
                url: '/ClassifierEditor/LoadFTGList/'
            }).then(function (response) {
                $scope.FTGList = response.data.FTGList;

                if (response.data.ShouldClearFTG) {
                    $scope.classifier.FTG = null;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.nfcList = [];
    $scope.ATCWhoList = [];
    $scope.GenericList = [];
    $scope.ATCEphmraList = [];
    $scope.ATCBaaList = [];
    $scope.FTG = [];

    $scope.loadNfcList();
    $scope.changeListATS();

    $scope.checkNfc = function () {
        if ($scope.classifier.Nfc.Id === 0) {
            $scope.classifier.Nfc = null;
        }
    };
    $scope.checkGeneric = function () {
        if ($scope.classifier.Generic.Id === 0) {
            $scope.classifier.Generic = null;
        }
    };
    $scope.checkATCWho = function () {
        if ($scope.classifier.ATCWho.Id === 0) {
            $scope.classifier.ATCWho = null;
        }
    };
    $scope.checkATCEphmra = function () {
        if ($scope.classifier.ATCEphmra.Id === 0) {
            $scope.classifier.ATCEphmra = null;
        }
    };
    $scope.checkATCBaa = function () {
        if ($scope.classifier.ATCBaa.Id === 0) {
            $scope.classifier.ATCBaa = null;
        }
    };
    $scope.checkFTG = function () {
        if ($scope.classifier.FTG.Id === 0) {
            $scope.classifier.FTG = null;
        }
    };
    $scope.checkProductionLocalization = function () {
        if ($scope.classifier.ProductionLocalization.Id === 0) {
            $scope.classifier.ProductionLocalization = null;
        }
    };

    $scope.NewManufacturer = function () {
        //$route.id = 0
        //$http.id = 0;
        //$state.id = 0;
        //var url = $scope.$state.href('Manufacturer', { id: 0 });
        window.open('/#/Classifier/Manufacturer/Edit?id=0', '_blank');
    };
    $scope.changeTradeName = function () {

        if ($scope.classifier.Brand !== null) {
            $scope.classifier.Brand.Value = "";
            $scope.classifier.Brand.Id = 0;
        }
    };
    $scope.LoadInit = function () {
        var F_ClassifierId = $route.current.params["ClassifierId"];
        if (F_ClassifierId > 0) {
            $scope.classifierFilter.used = null;
            $scope.classifierFilter.classifierId = F_ClassifierId;
            loadClassifier();
            return;
        }
        var F_Id = $route.current.params["Id"];
        if (F_Id > 0) {
            $scope.classifierFilter.used = null;
            $scope.classifierFilter.classifierId = F_Id;
            loadClassifier();
            return;
        }
    };

    $scope.LocalizationList = [
        { Id: 1, Value: "Отечественный" },
        { Id: 2, Value: "Импортный" },
        { Id: 3, Value: "Unknown" }
    ];
    $scope.LocalizationListFilter = $scope.LocalizationList;

    $scope.LoadFilterLocalization = function () {

        //console.log($scope.classifier.ProductionStage)

        if (($scope.classifier.ProductionStage === undefined) || ($scope.classifier.ProductionStage === null))
            return;

        //console.log($scope.classifier.PackerLocalization)
        switch ($scope.classifier.ProductionStage.Id) {
            case 1: // 1 = Unknown
            case 2: // 2 = Все стадии производства
                if (($scope.classifier.PackerLocalization !== undefined) && ($scope.classifier.PackerLocalization !== null))
                    $scope.LocalizationListFilter = $scope.LocalizationList.filter(loc => loc.Id === $scope.classifier.PackerLocalization.Id)
                else
                    $scope.LocalizationListFilter = [];
                break
            default: // 3: // Вторичная упаковка 4: // Без РУ
                $scope.LocalizationListFilter = $scope.LocalizationList;
        }

        if ($scope.LocalizationListFilter.length === 1) {
            //console.log($scope.classifier.ProductionLocalization);
            //console.log($scope.LocalizationListFilter[0]);

            $scope.classifier.ProductionLocalization = $scope.LocalizationListFilter[0];
        }

    }

    $scope.selectProductionStage = function () {
        $scope.LoadFilterLocalization();
    }

    $scope.LoadInit();
}