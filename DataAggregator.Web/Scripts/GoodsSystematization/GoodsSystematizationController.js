angular
    .module('DataAggregatorModule')
    .controller('GoodsSystematizationController', [
        '$scope', '$http', '$uibModal', '$location', 'commonService', 'hotkeys', 'uiGridCustomService', '$timeout', '$translate', 'messageBoxService', GoodsSystematizationController]);

function GoodsSystematizationController($scope, $http, $uibModal, $location, commonService, hotkeys, uiGridCustomService, $timeout, $translate, messageBoxService) {
    // Выносим список свойств в начало чтобы понимать что именно мы используем в коде.

    $scope.classifierFilter = {};
    $scope.filterDescription = [];
    $scope.selectedRowText = '';
    $scope.gridApi = undefined;
    $scope.goodsLoading = undefined;
    $scope.classifierLoading = undefined;
    $scope.message = undefined;

    $scope.goodsCount = 0;
    $scope.goodsCompleteCount = 0;

    var statusTypes =
    [
        { Id: 1, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.ADDING') },//v1
            { Id: 4, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.BOUND') },//v1
            { Id: 5, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.UNBOUND') }//v1
    ];

    // Фильтр
    $scope.filter = {
        status: {
            selectedItems: [],
            displayValue: '',
            availableItems: statusTypes
        }
    };

    var doubleClickSearchCellTemplate = '<div ng-dblclick="grid.appScope.dblClickSearch(col.colDef.field, row.entity)" class="ui-grid-cell-contents">{{COL_FIELD CUSTOM_FILTERS}}</div>';

    $scope.dblClickSearch = function (field, rowEntity) {
        if (rowEntity[field] !== null && rowEntity[field] !== undefined) {
            $scope.classifierFilter = {};
            $scope.classifierFilter[field] = rowEntity[field];

            loadClassifier($scope.classifierFilter);
        }
    };

    $scope.goodsGridOptions = uiGridCustomService.createOptions('GoodsSystematization_GoodsGrid');

    var goodsGridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 20,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableSelectionBatchEvent: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: true,
        noUnselect: false,
        showGridFooter: false,
        columnDefs: [
                        { name: ' ', cellTemplate: '_icon.html', enableFiltering: false, enableSorting: false, width: 25 },
                        { name: 'Id', field: 'DrugClearId', width: 50, type: 'number', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Категория', field: 'GoodsCategoryName', width: 100, filter: { condition: uiGridCustomService.condition } },
                        { name: 'Исходные данные', field: 'Text', width: 750, filter: { condition: uiGridCustomService.condition } },
                        { name: 'Код препарата', field: 'GoodsId', type: 'number', filter: { condition: uiGridCustomService.condition }, cellTemplate: doubleClickSearchCellTemplate },
                        { name: 'Торговое наименование', field: 'GoodsTradeName', filter: { condition: uiGridCustomService.condition } },
                        { name: 'ФВ + Ф + Д', field: 'GoodsDescription', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Производитель', field: 'Manufacturer', filter: { condition: uiGridCustomService.condition } },
            { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', filter: { condition: uiGridCustomService.condition }, cellTemplate: doubleClickSearchCellTemplate },
                        { name: 'Правообладатель', field: 'OwnerTradeMark', width: 300, filter: { condition: uiGridCustomService.condition } },
            { name: 'PackerId', field: 'PackerId', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Упаковщик', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Пользователь', field: 'LastChangedUserName', filter: { condition: uiGridCustomService.condition } }
        ],
        rowTemplate: '_rowGoodsTemplate.html'
    };

    angular.extend($scope.goodsGridOptions, goodsGridOptions);

    $scope.goodsClassifierGridOptions = uiGridCustomService.createOptions('GoodsSystematization_GoodsClassifierGrid');

    var goodsClassifierGridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 20,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableSelectionBatchEvent: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        noUnselect: false,
        multiSelect: true,
        showGridFooter: false,
        columnDefs: [
            { name: 'GoodsId', field: 'GoodsId', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Торговое наименование', field: 'GoodsTradeName', filter: { condition: uiGridCustomService.condition } },
                        { name: 'ФВ + Ф + Д', field: 'GoodsDescription', filter: { condition: uiGridCustomService.condition } },
            { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Правообладатель', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
            { name: 'PackerId', field: 'PackerId', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Упаковщик', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Категория', field: 'GoodsCategoryName', width: 100, filter: { condition: uiGridCustomService.condition } }
        ],
        rowTemplate: '_rowGoodsClassifierTemplate.html'
    };

    angular.extend($scope.goodsClassifierGridOptions, goodsClassifierGridOptions);

    ////////////////////// Filters and selection

    var processedRows = [];
    var isFiltered = false;

    $scope.$watch(function () { return $scope.filter.status.selectedItems; },
        function (newValue, oldValue) {
            if (newValue === oldValue && newValue === undefined)
                return;

            if ($scope.gridApi)
                onFilterChanged();
        }, true);

    $scope.goodsGridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            onSelectionChanged(row);
        });

        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            onSelectionChanged(rows[0]);
        });

        gridApi.core.on.filterChanged($scope, function () {
            $timeout(function () {
                onFilterChanged();
            }, 100);
        });

        gridApi.grid.registerRowsProcessor(gridRowProcessor, 200);
    };

    function processedEventHandler(rows) {

        processedRows = rows;

        selectNextRow(rows);
        $scope.gridApi.grid.refresh();
        scrollToSelectedRow();

        updateCount();
    }

    function onSelectionChanged(row) {
        $timeout(function () {
            if (getSelectedAndFilteredRows().length === 0)
                row.isSelected = true;
        }, 100).then(function (success) { updateSelectedText(row); });
    }

    function onFilterChanged() {
        var isFilteredNow = isFilterSet();

        selectProperRow(isFilteredNow);
        $scope.gridApi.grid.refresh();
        scrollToSelectedRow();
        updateCount();

        isFiltered = isFilteredNow;
    }

    function selectProperRow(isFilteredNow) {
        var visibleRows;
        // Устанавливается фильтр, возможны два варианта
        if (isFiltered === false && isFilteredNow === true) {
            // Есть обработанные строчки
            if (processedRows.length > 0) {
                if (selectNextRow(processedRows))
                    return;
            }
            // Нет обработанных строчек
            visibleRows = $scope.gridApi.core.getVisibleRows();
            if (visibleRows.length > 0)
                selectGridRow(visibleRows[0]);
            return;
        }
        // Полное снятие фильтра
        if (isFilteredNow === false && isFiltered === true) {
            // Есть обработанные строчки
            if (processedRows.length > 0) {
                selectNextRow(processedRows);
                return;
            }
        }
        // Обычное изменение фильтра
        if (isFilteredNow === true && isFiltered === true) {
            if (selectNextRow(processedRows))
                return;
            if (getSelectedAndFilteredRows().length > 0)
                return;
            visibleRows = $scope.gridApi.core.getVisibleRows();
            if (visibleRows.length > 0)
                selectGridRow(visibleRows[0]);
        }
    }

    function scrollToSelectedRow() {
        var selectedRows = $scope.gridApi.selection.getSelectedRows();
        if (selectedRows.length === 0)
            return;

        $timeout(function () {
            $scope.gridApi.core.scrollTo(selectedRows[0]);
        }, 100);
    }

    function updateSelectedText(row) {
        if (row.isSelected) 
            $scope.selectedRowText = row.entity.Text + '\n' + row.entity.Manufacturer;
        else 
            $scope.selectedRowText = '';
    }

    function gridRowProcessor(renderableRows) {
        var selectedItems = $scope.filter.status.selectedItems;

        if (selectedItems.length === 0) {
            return renderableRows;
        }

        var isAdding = false;
        var isBound = false;
        var isUnbound = false;

        for (var i = 0; i < selectedItems.length; i++) {
            isAdding = isAdding || selectedItems[i].Id === 1;
            isBound = isBound || selectedItems[i].Id === 4;
            isUnbound = isUnbound || selectedItems[i].Id === 5;
        }

        function visibleByStatus(item) {
            if (isAdding && item.ForAdding) return true;

            var isMarked = item.ForAdding || item.GoodsId;

            if (isBound && isMarked) return true;
            if (isUnbound && !isMarked) return true;

            return false;
        }

        renderableRows.forEach(function (row) {
            if (!visibleByStatus(row.entity))
                row.visible = false;
        });

        return renderableRows;
    }

    function getSelectedAndFilteredRows() {
        var selectedRows = $scope.gridApi.selection.getSelectedGridRows();

        var selectedAndFilteredRows = selectedRows.filter(function (item) { return item.visible; });

        return selectedAndFilteredRows.map(function (item) { return item.entity; });
    }

    function getIds(items) {

        var ids = items.map(function (value) {
            return value.GoodsClearId;
        });

        return ids;
    }

    function selectNextRow(rows) {
        var selectedRows = rows;
        if (selectedRows.length === 0)
            return false;

        var visibleRows = $scope.gridApi.core.getVisibleRows();

        var lastRowIndex = getLastRowIndex(visibleRows, selectedRows);

        if (lastRowIndex === -1)
            return false;

        var nextIndex = Math.min(lastRowIndex + 1, visibleRows.length - 1);

        var newSelectedRow = visibleRows[nextIndex].entity;

        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridApi.selection.selectRow(newSelectedRow);
        return true;
    }

    function selectGridRow(row) {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridApi.selection.selectRow(row.entity);
    }

    function getLastRowIndex(visibleRows, selectedRows) {
        var lastRowIndex = -1;
        for (var i = 0; i < selectedRows.length; i++) {
            var selectedRow = selectedRows[i];
            var gridRow = $scope.gridApi.grid.getRow(selectedRow);
            var currentRowIndex = visibleRows.indexOf(gridRow);
            if (currentRowIndex > lastRowIndex)
                lastRowIndex = currentRowIndex;
        }
        return lastRowIndex;
    }

    function isFilterSet() {
        if ($scope.filter.status.selectedItems.length > 0)
            return true;

        var columns = $scope.gridApi.grid.columns;

        for (var i = 0; i < columns.length; i++) {
            if (columns[i].filter.term)
                return true;
        }

        return false;
    }

    function updateCount() {
        $timeout(function () {

            var visibleRows = $scope.gridApi.core.getVisibleRows().map(function (item) { return item.entity; });

            $scope.goodsCount = visibleRows.length;
            $scope.goodsCompleteCount = visibleRows.filter(function (item) { return item.GoodsTradeName != null; }).length;
        }, 200);
    }

    ///////////////////////////////////


    $scope.getSelectedCount = function() {
        return getSelectedAndFilteredRows().length;
    };

    $scope.canGetGoods = function () {
        return $scope.goodsGridOptions.data != null && $scope.goodsGridOptions.data.length || $scope.goodsLoading.$$state.status === -1;
    };

    $scope.canSetGoods = function () {
        return $scope.goodsGridOptions.data == null || !$scope.goodsGridOptions.data.length || $scope.goodsLoading.$$state.status === -1;
    };

    (function() {
        $scope.goodsLoading = $http.post('/GoodsSystematization/LoadGoods/')
        .then(function (response) {
            $scope.goodsGridOptions.data = response.data;

            function refreshGridWhileLoadingGoods() {
                $timeout(function () {
                    if (!$scope.gridApi) {
                        refreshGridWhileLoadingGoods();
                        return;
                    }
                    var visibleRows = $scope.gridApi.core.getVisibleRows();
                    if (visibleRows.length > 0) {
                        $scope.gridApi.selection.selectRow(visibleRows[0].entity);
                    }
                }, 200);
            }

            refreshGridWhileLoadingGoods();

            updateCount();
        }, function () {
            $scope.message = 'Unexpected Error';
        });
    })();

    function getGoods(filter) {
        if (filter.additional.DrugClearId !== null && filter.additional.DrugClearId !== undefined) {
            filter.additional.DrugClearId = filter.additional.DrugClearId.split(',');
        }

        $scope.goodsLoading = $http.post('/GoodsSystematization/GetGoods/', JSON.stringify(filter))
        .then(function (response) {
            $scope.goodsGridOptions.data = response.data;

            $timeout(function () {
                var visibleRows = $scope.gridApi.core.getVisibleRows();
                if (visibleRows.length > 0) {
                    $scope.gridApi.selection.selectRow(visibleRows[0].entity);
                }
            }, 200);

            updateCount();
        }, function () {
            $scope.message = 'Unexpected Error';
        });
    }

    // клик по кнопке "Забрать данные"
    $scope.getGoods = function () {
        var modalGoodsInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GoodsSystematization/_GoodsFilterView.html',
            controller: 'GoodsFilterController',
            size: 'lg',
            backdrop: 'static'
        });

        modalGoodsInstance.result.then(function (filter) {
            $location.hash(null);
            getGoods(filter);
        }, function () {
            $location.hash(null);
        });
    };

    // клик по кнопке "Вернуть данные"
    $scope.setGoods = function () {
        $scope.goodsLoading = $http.post('/GoodsSystematization/SetGoods/')
        .then(function () {
            $scope.goodsGridOptions.data = [];

            updateCount();
        }, function () {
            $scope.message = 'Unexpected Error';
        });
    };

    // клик по кнопке "Поиск по справочнику"
    $scope.searchGoodsClassifier = function () {
        var modalClassifierInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GoodsSystematization/_GoodsClassifierFilterView.html',
            controller: 'GoodsClassifierFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                classifierFilter: function () {
                    return $scope.classifierFilter;
                }
            }
        });

        modalClassifierInstance.result.then(function (classifierFilter) {
            $scope.classifierFilter = classifierFilter;
            updateFilterDescription();
            loadClassifier($scope.classifierFilter);
        }, function () {
        });
    };

    // результат фильтра - получить список данных классификатора
    function loadClassifier(filter) {
        var selectedRows = getSelectedAndFilteredRows();
        var goodsCategoryIdList = [];
        selectedRows.forEach(function (item) {
            var id = item.GoodsCategoryId;
            if (id !== undefined && id !== null && goodsCategoryIdList.indexOf(id) === -1)
                goodsCategoryIdList.push(id);
        }); 

        $scope.classifierLoading =
            $http.post('/GoodsClassifier/GetClassifier/', JSON.stringify({ filter: filter, goodsCategoryIdList: goodsCategoryIdList, rettype: 0 }))
            .then(function (response) {
                $scope.goodsClassifierGridOptions.data = response.data;
            }, function () {
                $scope.message = 'Unexpected Error';
            });
    }

    // собираем информацию по фильтру и выводим её:
    function updateFilterDescription() {
        $scope.filterDescription = [];

        tryAddFilterDescription($scope.classifierFilter.goodsTradeName, 'Торговое наименование');
        tryAddFilterDescription($scope.classifierFilter.ownerTradeMark, 'Производитель');
        tryAddFilterDescription($scope.classifierFilter.packer, 'Упаковщик');

        tryAddFilterDescription($scope.classifierFilter.goodsId, 'goodsId');
        tryAddFilterDescription($scope.classifierFilter.ownerTradeMarkId, 'ownerTradeMarkId');
        tryAddFilterDescription($scope.classifierFilter.packerId, 'PackerId');
        tryAddFilterDescription($scope.classifierFilter.used, 'Used');
    }

    function tryAddFilterDescription(value, name) {
        if (!value)
            return;

        $scope.filterDescription.push({ Name: name, Value: value });
    }

    // Привязать данные
    $scope.setClassifierToGoods = function (element) {

        checkCount(setClassifierToGoods);

        function setClassifierToGoods() {
            var selectedAndFilteredRows = getSelectedAndFilteredRows();
            var selectedGoodsIds = getIds(selectedAndFilteredRows);
            
            var parameters = {
                "GoodsInWorkIdList": selectedGoodsIds, //здесь GoodsClearId
                "GoodsId": element.GoodsId,
                "OwnerTradeMarkId": element.OwnerTradeMarkId,
                "PackerId": element.PackerId
            };

            $scope.classifierLoading =
                $http.post('/GoodsClassifier/SetClassifierToGoods/', JSON.stringify({ parameters: parameters }))
                .then(function () {
                    selectedAndFilteredRows.forEach(function (item) {
                        updateItem(item, element);
                        clearAllFlags(item);
                        item.HasChanges = true;
                    });

                    processedEventHandler(selectedAndFilteredRows);
                }, function () {
                    $scope.message = 'Unexpected Error';
                });
        }
    };

    // добавить/снять метку "на добавление"
    $scope.forAdding = function (value) {
        checkCount(forAdding);

        function forAdding() {
            var selectedAndFilteredRows = getSelectedAndFilteredRows();
            var selectedGoodsIds = getIds(selectedAndFilteredRows);

            var data = JSON.stringify({ GoodsInWorkIdList: selectedGoodsIds, value: value });

            $scope.goodsLoading =
                $http.post('/GoodsSystematization/ForAdding/', data)
                .then(function () {
                    selectedAndFilteredRows.forEach(function (item) {
                        item.ForAdding = value;
                        if (value) clearItem(item, true);
                        item.HasChanges = true;
                    });

                    processedEventHandler(selectedAndFilteredRows);
                }, function () {
                    $scope.message = 'Unexpected Error';
                });
        }
    };

    $scope.clearGoodsId = function () {

        checkCount(clearGoodsId);

        function clearGoodsId() {
            var selectedAndFilteredRows = getSelectedAndFilteredRows();
            var selectedGoodsIds = getIds(selectedAndFilteredRows);

            $scope.classifierLoading =
                $http.post('/GoodsClassifier/ClearClassifierToGoods/', JSON.stringify({ GoodsInWorkIdList: selectedGoodsIds }))
                .then(function () {
                    selectedAndFilteredRows.forEach(function (item) {
                        clearItem(item, false);
                        item.HasChanges = true;
                    });

                    processedEventHandler(selectedAndFilteredRows);
                }, function () {
                    $scope.message = 'Unexpected Error';
                });
        }
    };

    function selectAndChangeCategory() {
        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        if (selectedAndFilteredRows.length === 0)
            return;

        var modalGoodsInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GoodsSystematization/_GoodsCategorySelectorView.html',
            controller: 'GoodsCategorySelectorController',
            size: 'lg',
            backdrop: 'static'
        });

        modalGoodsInstance.result.then(function (category) {
            changeCategory(selectedAndFilteredRows,category);
            },
            function () {
            });
    }

    function changeCategory(selectedAndFilteredRows,category) {
        var selectedGoodsIds = getIds(selectedAndFilteredRows);

        $scope.goodsLoading =
            $http.post('/GoodsClassifier/ChangeGoodsCategory/', JSON.stringify({ GoodsInWorkIdList: selectedGoodsIds, GoodsCategoryId: category.Id }))
            .then(function () {
                selectedAndFilteredRows.forEach(function (item) {
                    item.GoodsCategoryId = category.Id;
                    item.GoodsCategoryName = category.Name;
                    item.HasChanges = true;
                });

                processedEventHandler(selectedAndFilteredRows);

            }, function () {
                $scope.message = 'Unexpected Error';
            });
    }

    function checkCount(callbackFunc) {
        var selectedAndFilteredRows = getSelectedAndFilteredRows();

        //Проверяем сколько элементов выделено
        if (selectedAndFilteredRows.length < 50) {
            callbackFunc();
            return;
        }

        messageBoxService.showConfirm('Выбрано более 50 записей. Продолжить?', 'Внимание')
            .then(function () {
                    callbackFunc();
                },
                function () {
                }
            );
    }

    function clearItem(item, isForAddingMode) {
        updateItem(item, {}, isForAddingMode);
    }

    function updateItem(item, sourceItem, isForAddingMode) {
        item.GoodsId = sourceItem.GoodsId;
        item.OwnerTradeMarkId = sourceItem.OwnerTradeMarkId;
        item.PackerId = sourceItem.PackerId;
        item.GoodsTradeNameId = sourceItem.GoodsTradeNameId;
        item.GoodsTradeName = sourceItem.GoodsTradeName;
        item.GoodsDescription = sourceItem.GoodsDescription;
        item.OwnerTradeMark = sourceItem.OwnerTradeMark;
        item.Packer = sourceItem.Packer;
        if (!isForAddingMode) {
            item.GoodsCategoryName = sourceItem.GoodsCategoryName;
            item.GoodsCategoryId = sourceItem.GoodsCategoryId;
        }
    }

    function clearAllFlags(item) {
        item.ForAdding = false;
    }

    $scope.searchGoods = function () {
        var value = commonService.getSelectionText();

        if (!value)
            value = $scope.selectedText;

        if (!value)
            return;

        var selectedRows = getSelectedAndFilteredRows();
        var goodsCategoryIdList = [];
        selectedRows.forEach(function (item) {
            var id = item.GoodsCategoryId;
            if (id !== undefined && id !== null && goodsCategoryIdList.indexOf(id) === -1)
                goodsCategoryIdList.push(id);
        }); 

        $scope.classifierLoading =
            $http.post('/GoodsClassifier/GetClassifierFromHotKey/', JSON.stringify({ value: value, goodsCategoryIdList: goodsCategoryIdList,rettype:0 }))
            .then(function (response) {
                $scope.goodsClassifierGridOptions.data = response.data;
            }, function () {
                $scope.message = 'Unexpected Error';
            });
    };

    hotkeys.bindTo($scope).add({
        combo: 'shift+z',
        description: 'На заведение',
        callback: function (event) {
            $scope.forAdding(true);
            event.preventDefault();
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+v',
        description: 'Отвязать',
        callback: function (event) {
            $scope.clearGoodsId();
            event.preventDefault();
        }
    });

    $scope.shiftF = function (event) {
        $scope.searchGoods();
        setIsShift(false);
        if (event) {
            event.preventDefault();
        }
    };

    hotkeys.bindTo($scope).add({
        combo: 'shift+f',
        description: 'Поиск по справочнику',
        callback: function (event) {
            $scope.shiftF(event);
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+a',
        callback: function (event) {
            selectAndChangeCategory();
            event.preventDefault();
        }
    });

    $(window).keydown(function (event) {
        if (event.keyCode === 16) {
            setIsShift(true);
        }
    });

    $(window).keyup(function (event) {
        if (event.keyCode === 16) {
            setIsShift(false);
        }
    });

    function setIsShift(value) {
        if (value && !$('#goods-grid-block').hasClass('disableSelection')) {
            $scope.selectedText = commonService.getSelectionText();
        }
        
        if (value) {
            $('#goods-grid-block').addClass('disableSelection');
            $('#classifier-grid-block').addClass('disableSelection');
        } else {
            $('#goods-grid-block').removeClass('disableSelection');
            $('#classifier-grid-block').removeClass('disableSelection');
        }
    }
}
