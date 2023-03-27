angular
    .module('DataAggregatorModule')
    .controller('AddingDOPMonitoringDatabaseController', ['$scope', '$http', '$q', '$uibModal', '$cacheFactory', '$timeout', 'userService', 'uiGridCustomService', 'errorHandlerService', 'messageBoxService', 'uiGridConstants', 'formatConstants', AddingDOPMonitoringDatabaseController]);

function AddingDOPMonitoringDatabaseController($scope, $http, $q, $uibModal, $cacheFactory, $timeout, userService, uiGridCustomService, errorHandlerService, messageBoxService, uiGridConstants, formatConstants) {
    $scope.Title = "Модуль для добавления ДОП ассортимента в БД мониторинг";
    $scope.user = userService.getUser();

    console.log('Модуль для добавления ДОП ассортимента в БД мониторинг');

    // ================================== Категории ==================================
    $scope.goodsCategory = null;
    $scope.selectedParameterGroup = null;
    $scope.selectedParameterLevel1 = null;
    $scope.ClassifierId = null;

    var getGoodsCategoryList = function () {
        $scope.goodsCategory = null;
        $scope.loading = $http.post("/DOPMonitoringDatabase/GetGoodsCategoryList/")
            .then(function (response) {
                $scope.goodsCategoryList = response.data.Data;
            }, function () {
                messageBoxService.showError("Не удалось загрузить список категорий!");
            });
    };
    getGoodsCategoryList();
    // ================================== Категории ==================================

    // ======================== Свойства (ParameterGroup) =======================
    $scope.getParameterGroups = function (selectedItemId) {
        $scope.gridParameterLevel1.Options.data = [];
        $scope.selectedParameterLevel1 = null;

        $scope.loading = $http.post('/GoodsParametersEditor/GetParameterGroups/', { goodsCategoryId: $scope.goodsCategory.Id })
            .then(function (response) {
                $scope.gridParameterGroup.Options.data = response.data.Data;

                var selectedItemIndex = selectedItemId == null ? 0 : response.data.Data.map(function (d) { return d.Id; }).indexOf(selectedItemId);

                $scope.gridParameterGroupApi.grid.modifyRows($scope.gridParameterGroup.Options.data);
                $scope.gridParameterGroupApi.selection.selectRow($scope.gridParameterGroup.Options.data[selectedItemIndex]);

                $timeout(function () {
                    $scope.gridParameterGroupApi.core.scrollTo($scope.selectedParameterGroup);
                }, 100);
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }
    $scope.gridParameterGroup = uiGridCustomService.createGridClass($scope, 'ObjectsToObjectsReady_LogsGrid');
    $scope.gridParameterGroup.Options.multiSelect = false;
    $scope.gridParameterGroup.Options.noUnselect = true;
    $scope.gridParameterGroup.Options.columnDefs = [
        { name: 'Свойство', field: 'Name', filter: { condition: uiGridCustomService.condition }, enableCellEdit: true }
    ];

    $scope.gridParameterGroup.Options.onRegisterApi = function (gridApi) {
        $scope.gridParameterGroupApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, selectParameterGroup);
        gridApi.edit.on.afterCellEdit($scope, renameParameterGroup);
    };

    function selectParameterGroup(row) {
        if (row.isSelected) {
            $scope.selectedParameterGroup = row.entity;
        } else {
            $scope.selectedParameterGroup = null;
        }
        $scope.selectedParameterLevel1 = null;
        $scope.getParametersLevel1();
    }

    function renameParameterGroup(rowEntity, colDef, newValue, oldValue) {
        var data = JSON.stringify({ id: rowEntity.Id, newValue: newValue });
        $scope.sectionsLoading = $http.post('/GoodsParametersEditor/RenameParameterGroup/', data)
            .then(function (response) {
                var data = response.data;
                if (data.Success) {
                    return true;
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                    rowEntity[colDef.field] = oldValue;
                    $scope.$apply();
                    return false;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
                rowEntity[colDef.field] = oldValue;
                $scope.$apply();
                return false;
            });
    }

    $scope.addParameterGroup = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/GoodsParametersEditor/_AddParameterView.html',
            controller: 'AddParameterViewController',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function (newValue) {
            $scope.loading = $http.post("/GoodsParametersEditor/AddParameterGroup/", { newValue: newValue, goodsCategoryId: $scope.goodsCategory.Id })
                .then(function (response) {
                    var selectedItemId = response.data.Data.Id;
                    $scope.getParameterGroups(selectedItemId);
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }, function () {
        });
    }
    // ======================== Свойства (ParameterGroup) =======================

    // ======================== Подуровень 1 =======================
    $scope.getParametersLevel1 = function (selectedItemId) {
        //$scope.gridParameterLevel2.Options.data = [];

        $scope.loading = $http.post('/GoodsParametersEditor/GetParameters/', { parentId: $scope.selectedParameterGroup.Id, level: 1 })
            .then(function (response) {
                $scope.gridParameterLevel1.Options.data = response.data.Data;

                var selectedItemIndex = selectedItemId == null ? 0 : response.data.Data.map(function (d) { return d.Id; }).indexOf(selectedItemId);;

                $scope.gridParameterLevel1Api.grid.modifyRows($scope.gridParameterLevel1.Options.data);
                $scope.gridParameterLevel1Api.selection.selectRow($scope.gridParameterLevel1.Options.data[selectedItemIndex]);

                $timeout(function () {
                    $scope.gridParameterLevel1Api.core.scrollTo($scope.selectedParameterLevel1);
                }, 100);
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.gridParameterLevel1 = uiGridCustomService.createGridClass($scope, 'GoodsParametersEditor_ParameterLevel1Grid');
    $scope.gridParameterLevel1.Options.multiSelect = false;
    $scope.gridParameterLevel1.Options.noUnselect = true;
    $scope.gridParameterLevel1.Options.columnDefs = [
        { name: 'Подуровень 1', field: 'Value', filter: { condition: uiGridCustomService.condition }, enableCellEdit: true }
    ];

    $scope.gridParameterLevel1.Options.onRegisterApi = function (gridApi) {
        $scope.gridParameterLevel1Api = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, selectParameterLevel1);
        gridApi.edit.on.afterCellEdit($scope, renameParameter);
    };

    function selectParameterLevel1(row) {
        if (row.isSelected) {
            $scope.selectedParameterLevel1 = row.entity;
        } else {
            $scope.selectedParameterLevel1 = null;
        }
        //$scope.selectedParameterLevel2 = null;
        //$scope.getParametersLevel2();
    }

    function renameParameter(rowEntity, colDef, newValue, oldValue) {
        var data = JSON.stringify({ id: rowEntity.Id, newValue: newValue });
        $scope.sectionsLoading = $http.post('/GoodsParametersEditor/RenameParameter/', data)
            .then(function (response) {
                var data = response.data;
                if (data.Success) {
                    return true;
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                    rowEntity[colDef.field] = oldValue;
                    $scope.$apply();
                    return false;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
                rowEntity[colDef.field] = oldValue;
                $scope.$apply();
                return false;
            });
    }

    $scope.addParameter = function (level) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/GoodsParametersEditor/_AddParameterView.html',
            controller: 'AddParameterViewController',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function (newValue) {
            $scope.loading = $http.post(
                "/GoodsParametersEditor/AddParameter/",
                {
                    newValue: newValue,
                    parameterGroupId: $scope.selectedParameterGroup.Id,
                    parentId: level === 1 ? null : $scope.selectedParameterLevel1.Id
                })
                .then(function (response) {
                    var selectedItemId = response.data.Data.Id;
                    if (level === 1) {
                        $scope.getParametersLevel1(selectedItemId);
                    } else {
                        $scope.getParametersLevel2(selectedItemId);
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }, function () {
        });
    }

    $scope.removeParameterAsk = function (level) {

        if ((level === 1 && $scope.selectedParameterLevel1 == null) || (level === 2 && $scope.selectedParameterLevel2 == null)) {
            return;
        }

        messageBoxService.showConfirm('Вы уверены, что хотите удалить запись "' + (level === 1 ? $scope.selectedParameterLevel1.Value : $scope.selectedParameterLevel2.Value) + '"?', 'Удаление')
            .then(
                function () {
                    removeParameter(level);
                },
                function () {
                });
    };

    function removeParameter(level) {
        $scope.loading = $http.post('/GoodsParametersEditor/RemoveParameter/', { parameterId: level === 1 ? $scope.selectedParameterLevel1.Id : $scope.selectedParameterLevel2.Id })
            .then(function () {
                var selectedItemIndex;
                if (level === 1) {
                    selectedItemIndex = $scope.gridParameterLevel1.Options.data.indexOf($scope.selectedParameterLevel1);
                    $scope.gridParameterLevel1.Options.data.splice(selectedItemIndex, 1);
                    $scope.selectedParameterLevel1 = null;
                    $scope.selectedParameterLevel2 = null;
                    $scope.gridParameterLevel2.Options.data = [];
                } else {
                    selectedItemIndex = $scope.gridParameterLevel2.Options.data.indexOf($scope.selectedParameterLevel2);
                    $scope.gridParameterLevel2.Options.data.splice(selectedItemIndex, 1);
                    $scope.selectedParameterLevel2 = null;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    // ================= Инициализация таблиц =================->
    $scope.selectedRows = [];

    $scope.AddingDOPMonitoringDatabase_Init = function () {

        $scope.message = 'Пожалуйста, ожидайте... Загрузка';

        // Таблица СКЮ, блокировок ->
        $scope.GridDOPMonitoringDatabase = uiGridCustomService.createGridClassMod($scope, 'GridDOPMonitoringDatabase');
        $scope.GridDOPMonitoringDatabase.Options.showGridFooter = true;
        $scope.GridDOPMonitoringDatabase.Options.multiSelect = true;
        $scope.GridDOPMonitoringDatabase.Options.noUnselect = true;
        $scope.GridDOPMonitoringDatabase.Options.enableFiltering = true;
        $scope.GridDOPMonitoringDatabase.Options.enableSelectAll = true;
        $scope.GridDOPMonitoringDatabase.Options.modifierKeysToMultiSelect = true;
        $scope.GridDOPMonitoringDatabase.Options.flatEntityAccess = true;
        $scope.GridDOPMonitoringDatabase.Options.enableGridMenu = true;

        $scope.GridDOPMonitoringDatabase.Options.columnDefs = [
            { headerTooltip: true, name: 'GoodsId', displayName: 'Код препарата', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'GoodsTradeNameId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsTradeNameId', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'GoodsTradeName', displayName: 'Торговое наименование', enableCellEdit: false, width: 300, cellTooltip: true, field: 'GoodsTradeName', visible: true },
            { headerTooltip: true, name: 'GoodsDescription', displayName: 'ФВ + Ф + Д', enableCellEdit: false, width: 300, cellTooltip: true, field: 'GoodsDescription', visible: true },

            { headerTooltip: true, name: 'OwnerTradeMarkId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'OwnerTradeMarkId', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'OwnerTradeMarkKey', enableCellEdit: false, width: 100, cellTooltip: true, field: 'OwnerTradeMarkKey', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'OwnerTradeMark', displayName: 'Правообладатель', enableCellEdit: false, width: 200, cellTooltip: true, field: 'OwnerTradeMark', visible: true },

            { headerTooltip: true, name: 'PackerId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'PackerId', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'PackerKey', enableCellEdit: false, width: 100, cellTooltip: true, field: 'PackerKey', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'Packer', displayName: 'Упаковщик', enableCellEdit: false, width: 200, cellTooltip: true, field: 'Packer', visible: true },

            { headerTooltip: true, name: 'GoodsCategoryId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsCategoryId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'GoodsCategoryName', displayName: 'Категория', enableCellEdit: false, width: 200, cellTooltip: true, field: 'GoodsCategoryName', visible: true },
            { headerTooltip: true, name: 'BrandId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BrandId', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'Brand', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Brand', visible: true },
            { headerTooltip: true, name: 'ClassifierId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'ClassifierId', type: 'number', visible: true, nullable: false, filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { headerTooltip: true, name: 'Status', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Status', type: 'boolean', visible: true, nullable: false },
            { headerTooltip: true, name: 'StatusDesc', displayName: 'Выливать в БД мониторинг', enableCellEdit: false, width: 300, cellTooltip: true, field: 'StatusDesc', visible: false },
            { headerTooltip: true, name: 'StartDate', displayName: 'Дата начала', enableCellEdit: false, width: 150, cellTooltip: true, field: 'StartDate', type: 'date', cellFilter: formatConstants.FILTER_DATE, visible: true, nullable: true },
            { headerTooltip: true, name: 'BlockTypeId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BlockTypeId', type: 'number', visible: true, nullable: true },
            { headerTooltip: true, name: 'BlockTypeName', displayName: 'Тип блокировки', enableCellEdit: false, width: 300, cellTooltip: true, field: 'BlockTypeName', visible: true },
            { headerTooltip: true, name: 'BlockTypeDescription', enableCellEdit: false, width: 300, cellTooltip: true, field: 'BlockTypeDescription', visible: false }
        ];

        $scope.GridDOPMonitoringDatabase.SetDefaults();

        $scope.GridDOPMonitoringDatabase.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;

            // Что-то выделили
            gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                $scope.selectedRows = $scope.gridApi.selection.getSelectedRows();
            });

            // Что-то выделили
            gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
                $scope.selectedRows = $scope.gridApi.selection.getSelectedRows();
            });
        };
        // Таблица СКЮ, блокировок <-

        // Таблица блокировок ->
        $scope.GridDBlocking = uiGridCustomService.createGridClassMod($scope, 'GridDBlocking');
        $scope.GridDBlocking.Options.showGridFooter = true;
        $scope.GridDBlocking.Options.multiSelect = true;
        $scope.GridDBlocking.Options.enableFiltering = true;
        $scope.GridDBlocking.Options.enableSelectAll = true;
        $scope.GridDBlocking.Options.modifierKeysToMultiSelect = true;
        $scope.GridDBlocking.Options.flatEntityAccess = true;
        $scope.GridDBlocking.Options.enableGridMenu = true;

        $scope.GridDBlocking.Options.columnDefs = [
            { headerTooltip: true, name: 'Id', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BlockingForMonitoringId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'GoodsCategoryId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsCategoryId', type: 'number', visible: true, nullable: false },
            { headerTooltip: true, name: 'GoodsCategoryName', displayName: 'Категория', enableCellEdit: false, width: 200, cellTooltip: true, field: 'GoodsCategoryName', visible: true },
            { headerTooltip: true, name: 'ParameterId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'ParameterId', type: 'number', visible: false, nullable: true },
            { headerTooltip: true, name: 'ParameterValue', displayName: 'Доп. свойство', enableCellEdit: false, width: 200, cellTooltip: true, field: 'ParameterValue', visible: true },
            { headerTooltip: true, name: 'ClassifierId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'ClassifierId', type: 'number', visible: true, nullable: false, filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { headerTooltip: true, name: 'Status', displayName: 'Статус', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Status', type: 'boolean', visible: true, nullable: false },
            { headerTooltip: true, name: 'StatusDesc', displayName: 'Выливать в БД мониторинг', enableCellEdit: false, width: 300, cellTooltip: true, field: 'StatusDesc', visible: true },
            { headerTooltip: true, name: 'BlockTypeId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BlockTypeId', type: 'number', visible: false, nullable: false },
            { headerTooltip: true, name: 'BlockTypeName', displayName: 'Тип блокировки', enableCellEdit: false, width: 300, cellTooltip: true, field: 'BlockTypeName', visible: true },
            { headerTooltip: true, name: 'StartDate', displayName: 'Дата начала', enableCellEdit: false, width: 150, cellTooltip: true, field: 'StartDate', type: 'date', cellFilter: formatConstants.FILTER_DATE, visible: true, nullable: false }
        ];

        $scope.GridDBlocking.SetDefaults();

        //$scope.GridDBlocking.Options.onRegisterApi = function (gridApi) {
        //    $scope.gridBlockApi = gridApi;
        //};

        // Таблица блокировок <-
    }
    // ================= Инициализация таблиц =================<-

    // ============>
    // Блок панелей
    $scope.PanelCategoryListIsShow = true;
    $scope.PanelAdditionalPropertyIsShow = true;

    $scope.PanelCategoryListToogle = function () {
        $scope.PanelCategoryListIsShow = !$scope.PanelCategoryListIsShow;
        setTimeout(function () { if (!$scope.$$phase) $scope.$apply(); });
    }

    $scope.PanelAdditionalPropertyToogle = function () {
        $scope.PanelAdditionalPropertyIsShow = !$scope.PanelAdditionalPropertyIsShow;
        setTimeout(function () { if (!$scope.$$phase) $scope.$apply(); });
    }

    $scope.hideGridDOPMonitoringDatabase = true;
    $scope.hideGridDBlocking = true;

    // показать таблицу блокировок и классификаторов
    $scope.GridDOPMonitoringDatabase_Show = function () {
        if ($scope.GridDOPMonitoringDatabase.Options.data.length == 0)
            $scope.DOPMonitoringDatabase_Refresh();

        $scope.hideGridDBlocking = true;
        $scope.hideGridDOPMonitoringDatabase = false;
    }

    // показать таблицу блокировок
    $scope.GridBlocking_Show = function () {
        if ($scope.GridDBlocking.Options.data.length == 0)
            $scope.GridBlocking_Refresh();

        $scope.hideGridDOPMonitoringDatabase = true;
        $scope.hideGridDBlocking = false;
    }
    // Блок панелей
    // ============<

    // обновить все таблицы
    $scope.RefreshTables = function () {
        if (!$scope.hideGridDOPMonitoringDatabase)
            $scope.DOPMonitoringDatabase_Refresh();

        if (!$scope.hideGridDBlocking) {
            $scope.GridBlocking_Refresh();
        }
    }

    $scope.DOPMonitoringDatabase_Refresh = function () {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DOPMonitoringDatabase/Init/'
        }).then(function (response) {
            var data = response.data;

            if (data.Success) {
                $scope.GridDOPMonitoringDatabase.SetData(data.Data.DOPBlocking);
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }

        }, function (response) {
            console.error(response);

            messageBoxService.showError(response.data.message);
        }).catch(error => alert(error.message));
    }

    $scope.GridBlocking_Refresh = function () {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DOPMonitoringDatabase/InitBlocking/'
        }).then(function (response) {
            var data = response.data;

            if (data.Success) {
                $scope.GridDBlocking.Options.data = data.Data.Blocking;
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }

        }, function (response) {
            console.error(response);

            messageBoxService.showError(response.data.message);
        }).catch(error => alert(error.message));
    }

    // установить заглушку на категорию
    $scope.SetPlugOnByCategory = function () {

        console.log($scope.goodsCategory);

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DOPMonitoringDatabase/SetPlugOnByCategory/',
                data: JSON.stringify({ GoodsCategoryId: $scope.goodsCategory.Id })
            }).then(function (response) {
                var data = response.data;
                if (data.Data.Success) {
                    $scope.RefreshTables();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    // установить заглушку на категорию и доп свойство
    $scope.SetPlugOnByCategoryAndProperty = function () {

        console.log($scope.goodsCategory);
        console.log($scope.selectedParameterLevel1);

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DOPMonitoringDatabase/SetPlugOnByCategoryAndProperty/',
                data: JSON.stringify({ GoodsCategoryId: $scope.goodsCategory.Id, ParameterID: $scope.selectedParameterLevel1.Id })
            }).then(function (response) {
                var data = response.data;
                if (data.Data.Success) {
                    $scope.RefreshTables();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    // Поставить заглушку на СКЮ
    $scope.SetPlugOnByClassifier = function () {

        if ($scope.selectedRows.length == 0)
            return;

        console.log($scope.selectedRows);

        var ClassifierArray = [];

        Array.prototype.push.apply(ClassifierArray, $scope.selectedRows.map(function (obj) {
            return obj.ClassifierId;
        }));

        console.log(ClassifierArray);

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DOPMonitoringDatabase/SetPlugOnByClassifierList/',
                data: JSON.stringify({ ClassifierIdList: ClassifierArray})
            }).then(function (response) {
                var data = response.data;
                if (data.Data.Success) {
                    $scope.RefreshTables();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });

    }

    $scope.PouringStartDate = null; // дата начала выливки

    // =============================
    // снять заглушку с категории ->
    $scope.DialogSetPlugOffByCategory = function () {

        var modalDialogInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/AddingDOPMonitoringDatabase/DialogSetPlugOffByCategory.html',
            controller: 'DialogSetPlugOffByCategoryController',
            size: 'md',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                PlugInfo: function () {
                    return { GoodsCategory: $scope.goodsCategory, Parameter: null, ClassifierId: null };
                }
            }
        });

        modalDialogInstance.result.then(function (DialogData) {
            console.log(Date.parse(DialogData));
            $scope.PouringStartDate = new Date(DialogData).toLocaleDateString('en', { year: 'numeric', month: 'numeric', day: 'numeric' });
            console.log($scope.PouringStartDate);
            SetPlugOffByCategory($scope.goodsCategory.Id, $scope.PouringStartDate);
        }, function (DialogData) {

        });
    }

    SetPlugOffByCategory = function (GoodsCategoryId, PouringStartDate) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/DOPMonitoringDatabase/SetPlugOffByCategory/',
                data: JSON.stringify({ GoodsCategoryId: GoodsCategoryId, PouringStartDate: PouringStartDate })
            }).then(function (response) {
                var data = response.data;
                if (data.Data.Success) {
                    $scope.RefreshTables();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }
    // снять заглушку с категории <-
    // =============================

    // ============================================
    // снять заглушку с категории и доп свойства ->
    $scope.DialogSetPlugOffByCategoryAndProperty = function () {
        var GoodsCategoryId = $scope.goodsCategory.Id;
        var ParameterID = $scope.selectedParameterLevel1.Id;

        if (GoodsCategoryId == null)
            throw "Не выбрана категория"

        if (ParameterID == null)
            throw "Не выбран параметр"

        var modalDialogInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/AddingDOPMonitoringDatabase/DialogSetPlugOffByCategory.html',
            controller: 'DialogSetPlugOffByCategoryController',
            size: 'md',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                PlugInfo: function () {
                    return { GoodsCategory: $scope.goodsCategory, Parameter: $scope.selectedParameterLevel1, ClassifierId: null };
                }
            }
        });

        modalDialogInstance.result.then(function (DialogData) {
            $scope.PouringStartDate = new Date(DialogData).toLocaleDateString('en', { year: 'numeric', month: 'numeric', day: 'numeric' });

            console.log($scope.PouringStartDate);

            SetPlugOffByCategoryAndProperty(GoodsCategoryId, ParameterID, $scope.PouringStartDate);
        }, function (DialogData) {

        });

        SetPlugOffByCategoryAndProperty = function (GoodsCategoryId, ParameterID, PouringStartDate) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/DOPMonitoringDatabase/SetPlugOffByCategoryAndProperty/',
                    data: JSON.stringify({ GoodsCategoryId: GoodsCategoryId, ParameterID: ParameterID, PouringStartDate: PouringStartDate })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Data.Success) {
                        $scope.RefreshTables();
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    }

    // снять заглушку с категории и доп свойства <-
    // ============================================
}