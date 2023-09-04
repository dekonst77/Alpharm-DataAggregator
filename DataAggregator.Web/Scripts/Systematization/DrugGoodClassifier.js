angular
    .module('DataAggregatorModule')
    .controller('DrugGoodClassifierController', [
        '$scope', '$http', '$uibModal', '$location', 'commonService', 'formatConstants', 'hotkeys', 'uiGridCustomService', '$timeout', '$translate', 'messageBoxService', 'errorHandlerService', DrugGoodClassifierController]);

function DrugGoodClassifierController($scope, $http, $uibModal, $location, commonService, formatConstants, hotkeys, uiGridCustomService, $timeout, $translate, messageBoxService, errorHandlerService) {

    $scope.gridApi = undefined;

    $scope.drugsCount = 0;
    $scope.drugsCompleteCount = 0;

    var statusTypes =
        [
            { Id: 0, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.CHECKING') },
            { Id: 1, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.ADDING') },
            { Id: 2, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.OTHER') },
            { Id: 3, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.ERROR') },
            { Id: 4, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.BOUND') },
            { Id: 5, displayValue: $translate.instant('SYSTEMATIZATION.STATUS.UNBOUND') },
            { Id: 6, displayValue: $translate.instant('Непривязана ДОП') },
            { Id: 7, displayValue: $translate.instant('Без категории') }
        ];

    // Фильтр
    $scope.filter = {
        status: {
            selectedItems: [],
            displayValue: '',
            availableItems: statusTypes
        }
    };

    function clearSelected() {
        $scope.gridApi.selection.clearSelectedRows();
    }

    $scope.canGetDrugs = function () {
        return $scope.gridOptions.data !== null && $scope.gridOptions.data !== undefined && $scope.gridOptions.data.length || $scope.drugLoading.$$state.status === -1;
    };

    $scope.canSetDrugs = function () {
        return $scope.gridOptions.data === null || $scope.gridOptions.data === undefined || !$scope.gridOptions.data.length || $scope.drugLoading.$$state.status === -1;
    };

    var doubleClickSearchDOPCellTemplate = '<div ng-dblclick="grid.appScope.dblClickSearchDOP(col.colDef.field, row.entity)" class="ui-grid-cell-contents">{{COL_FIELD CUSTOM_FILTERS}}</div>';
    var doubleClickSearchCellTemplate = '<div ng-dblclick="grid.appScope.dblClickSearch(col.colDef.field, row.entity)" class="ui-grid-cell-contents">{{COL_FIELD CUSTOM_FILTERS}}</div>';


    $scope.dblClickSearchDOP = function (field, rowEntity) {
        if (rowEntity[field] !== null && rowEntity[field] !== undefined) {
            $scope.classifierFilter = {};
            $scope.classifierFilter[field] = rowEntity[field];

            loadClassifierDOP($scope.classifierFilter);
        }
    };

    $scope.dblClickSearch = function (field, rowEntity) {
        if (rowEntity[field] !== null && rowEntity[field] !== undefined) {
            $scope.classifierFilter = {};
            $scope.classifierFilter[field] = rowEntity[field];

            loadClassifier($scope.classifierFilter);
        }
    };


    $scope.gridOptions = uiGridCustomService.createOptions('Systematization_DrugsGrid');

    var gridOptions = {
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
            { name: ' ', cellTemplate: '_icon.html', enableFiltering: false, enableSorting: false, width: 40 },
            { name: 'DrugClearId', field: 'DrugClearId', width: 50, type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'DrugClearText', field: 'DrugClearText', width: 750, filter: { condition: uiGridCustomService.condition } },
            { name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: doubleClickSearchCellTemplate },
            { name: 'GoodsId', field: 'GoodsId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: doubleClickSearchDOPCellTemplate },
            { name: 'TradeName', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
            { name: 'DrugDescription', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
            { name: 'Manufacturer', field: 'Manufacturer', filter: { condition: uiGridCustomService.condition } },
            { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: doubleClickSearchCellTemplate },
            { name: 'OwnerTradeMark', field: 'OwnerTradeMark', width: 300, filter: { condition: uiGridCustomService.condition } },
            { name: 'PackerId', field: 'PackerId', filter: { condition: uiGridCustomService.condition } },
            { name: 'Packer', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
            { name: 'Пользователь', field: 'UserName', filter: { condition: uiGridCustomService.condition } },
            { name: 'Promo', field: 'Promo', filter: { condition: uiGridCustomService.condition } },
            { name: 'Flags', field: 'Flags', filter: { condition: uiGridCustomService.condition } },
            { name: 'GoodsCategoryName', field: 'GoodsCategoryName', filter: { condition: uiGridCustomService.condition } },
            { name: 'RegNumber', field: 'RegNumber', filter: { condition: uiGridCustomService.condition }, cellTemplate: doubleClickSearchCellTemplate },
            { name: 'Comment', field: 'Comment', filter: { condition: uiGridCustomService.condition } },
            { name: 'PrioritetWords', field: 'PrioritetWords', filter: { condition: uiGridCustomService.condition } },
            { name: 'OperatorComment', field: 'OperatorComment', filter: { condition: uiGridCustomService.condition }, enableCellEdit: true },
            { name: 'HasEmptyClassfierId', field: 'HasEmptyClassfierId', filter: { condition: uiGridCustomService.condition } },
        ],
        rowTemplate: '_rowTemplate.html'
    };

    angular.extend($scope.gridOptions, gridOptions);

    //Поиск следующей пустой строки
    $scope.searchEmpty = function () {
        //Выберем весь массив отсортированных отфильтрованных данных
        var rows = $scope.gridApi.grid.renderContainers.body.visibleRowCache

        //Выеберем все выделенные строки, которые видны.
        var selectedRows = $scope.gridApi.selection.getSelectedGridRows();
        var selectedAndFilteredRows = selectedRows.filter(function (item) { return item.visible; });

        //Найдем последний максимально выделенный индекс
        var lastindex = -1;

        for (var i = 0; i < selectedAndFilteredRows.length; i++) {

            var index = rows.findIndex((item) => {
                return selectedAndFilteredRows[i].entity.Id == item.entity.Id;
            });

            if (index > lastindex)
                lastindex = index;
        }

        //Найдем следующий за lastindex index где данные не привязаны
        var emptydata = rows.filter((item) => { return item.entity.OwnerTradeMarkId == null; });
        var emptydataindex = [];

        //Соберем массив индексов
        for (var i = 0; i < emptydata.length; i++) {
            var index = rows.indexOf(emptydata[i]);
            emptydataindex.push(index);
        }

        //Выберем следующий за последней выделенной строчкой не привязанный
        var nexts = emptydataindex.filter((value) => { return value > lastindex; });
        if (nexts.length == 0)
            nexts = emptydataindex;

        var nextIndex = nexts.sort(function (a, b) { return a - b; })[0];

        //Выделим и проскролим до найденного
        var row = rows[nextIndex];
        //Найдем эту строчку среди исходных данных
        var data = $scope.gridOptions.data.filter((data) => { return data.Id == row.entity.Id; })[0];
        //Выделим её и проскролим до неё
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridApi.selection.selectRow(data);

        //Если скролим назад то постараемся выделить предыдущий элемент, если скролить до выделенного, то он не полностью виден.
        if (nextIndex < lastindex && nextIndex > 0) {
            row = rows[nextIndex - 1];
            data = $scope.gridOptions.data.filter((data) => { return data.Id == row.entity.Id; })[0];
        }

        $scope.gridApi.core.scrollTo(data);
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

    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            onSelectionChanged(row);
        });

        gridApi.edit.on.afterCellEdit($scope, editRowDataSource);

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

    function editRowDataSource(rowEntity, colDef, newValue, oldValue) {
        str = JSON.stringify(rowEntity, null, '\t');
        console.log(str);

        let json = JSON.stringify(rowEntity);

        $scope.DataSourceLoading = $http({
            method: "POST",
            url: "/Systematization/EditComment/",
            data: json
        }).then(function () {
            return true;
        }, function (response) {
            rowEntity[colDef.field] = oldValue;
            errorHandlerService.showResponseError(response);
            return false;
        });

        return;

        /*
                Systematization.DrugClassifier
        
                [Systematization].[GetDrugsV2]
                [Systematization].[SetDrugs]
        */
    }


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
            $scope.selectedRowText = row.entity.DrugClearText + '\n' + row.entity.Manufacturer;
        else
            $scope.selectedRowText = '';
    }

    function gridRowProcessor(renderableRows) {
        var selectedItems = $scope.filter.status.selectedItems;

        if (selectedItems.length === 0) {
            return renderableRows;
        }

        var isChecking = false;
        var isAdding = false;
        var isOther = false;
        var isUnboundOther = false;
        var isError = false;
        var isBound = false;
        var isUnbound = false;
        var isnonCategory = false;

        for (var i = 0; i < selectedItems.length; i++) {
            isChecking = isChecking || selectedItems[i].Id === 0;
            isAdding = isAdding || selectedItems[i].Id === 1;
            isOther = isOther || selectedItems[i].Id === 2;
            isError = isError || selectedItems[i].Id === 3;
            isBound = isBound || selectedItems[i].Id === 4;
            isUnbound = isUnbound || selectedItems[i].Id === 5;
            isUnboundOther = isUnboundOther || selectedItems[i].Id === 6;
            isnonCategory = isnonCategory || selectedItems[i].Id === 7;
        }

        function visibleByStatus(item) {
            if (isChecking && item.ForChecking) return true;
            if (isAdding && item.ForAdding) return true;
            if (isOther && item.IsOther) return true;
            if (isError && item.IsError) return true;

            var isMarked = item.ForChecking || item.ForAdding || item.IsOther || item.DrugId;

            if (isUnboundOther) {
                if (item.IsOther === true && item.GoodsId === null)
                    return true;
                else
                    return false;
            }
            if (isnonCategory) {
                if (item.IsOther === true && item.GoodsCategoryId === null)
                    return true;
                else
                    return false;
            }

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
            return value.Id;
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

            $scope.drugsCount = visibleRows.length;
            $scope.drugsCompleteCount = visibleRows.filter(function (item) { return (item.DrugId !== null || item.GoodsId !== null) && item.OwnerTradeMarkId !== null; }).length;
        }, 200);
    }

    ///////////////////////////

    $scope.selectedRowText = '';

    $scope.shiftF = function (event) {
        loadClassifierFromHotKey();
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
        combo: 'shift+down',
        description: 'Следующая пустая строка',
        callback: function (event) {
            $scope.searchEmpty();
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+d',
        description: 'Поиск по исходным данным',
        callback: function (event) {
            var column = $scope.gridApi.grid.columns.filter(function (item) { return item.field === 'DrugClearText'; });
            if (column.length === 1) {
                column[0].filters[0].term = $scope.selectedText;
                clearSelected();
                setIsShift(false);
                event.preventDefault();
            }
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+d',
        description: 'Поиск по исходным данным',
        callback: function (event) {
            var column = $scope.gridApi.grid.columns.filter(function (item) { return item.field === 'DrugClearText'; });
            if (column.length === 1) {
                column[0].filters[0].term = $scope.selectedText;
                clearSelected();
                setIsShift(false);
                event.preventDefault();
            }
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+c',
        description: 'На проверку',
        callback: function (event) {
            $scope.forChecking(true);
            setIsShift(false);
            event.preventDefault();
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+z',
        description: 'На заведение',
        callback: function (event) {
            $scope.forAdding(true);
            setIsShift(false);
            event.preventDefault();
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+x',
        description: 'Добавить в ДОП',
        callback: function (event) {
            //value, GoodsCategoryId, GoodsCategoryName, checked
            $scope.forIsOther(true, null, null, 0);//v2
            setIsShift(false);
            event.preventDefault();
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+v',
        description: 'Отвязать',
        callback: function (event) {
            $scope.clearDrugId();
            setIsShift(false);
            event.preventDefault();
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+w',
        description: 'Акция',
        callback: function (event) {
            $scope.openSetPromoDialog();
            setIsShift(false);
            event.preventDefault();
        }
    });
    $scope.clearTags = function (value, s1, s2) {
        var i1 = value.indexOf(s1);
        var i2 = value.indexOf(s2, i1);
        if (i1 < 0 || i2 < 0)
            return value;

        var repl = value.substring(i1, i2 - i1 + 1);
        value = value.replace(repl, "");
        return $scope.clearTags(value, s1, s2);
    };
    $scope.setFlag = function (type, value) {

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        // var selectedDrugIds = getIds(selectedAndFilteredRows);
        selectedAndFilteredRows.forEach(function (item) {
            var new_val = item.Flags;
            if (new_val === null || new_val === undefined)
                new_val = "";

            new_val = $scope.clearTags(new_val, "[", "]");

            new_val += value;

            $scope.drugLoading = $http.post('/Systematization/SetFlags/',
                JSON.stringify({ drugClassifierInWorkId: item.Id, FlagsValue: new_val }))
                .then(function () {
                    item.Flags = new_val;
                    item.HasChanges = true;
                },
                    function (response) {
                        messageBoxService.showError(response.data.message);
                    });

        });

    };
    hotkeys.bindTo($scope).add({
        combo: 'shift+0',
        description: 'Убрать флаги ТН',
        callback: function (event) {
            $scope.setFlag("[", "");
            event.preventDefault();
        }
    });
    hotkeys.bindTo($scope).add({
        combo: 'shift+1',
        description: 'Флаг ТН',
        callback: function (event) {
            $scope.setFlag("[", "[ТН]");
            event.preventDefault();
        }
    });
    hotkeys.bindTo($scope).add({
        combo: 'shift+5',
        description: 'Флаг Бренд',
        callback: function (event) {
            $scope.setFlag("[", "[Бренд]");
            event.preventDefault();
        }
    });
    hotkeys.bindTo($scope).add({
        combo: 'shift+2',
        description: 'флаг ТН+ФВ',
        callback: function (event) {
            $scope.setFlag("[", "[ТН+ФВ]");
            event.preventDefault();
        }
    });
    hotkeys.bindTo($scope).add({
        combo: 'shift+3',
        description: 'Флаг ТН+ФВ+Доз',
        callback: function (event) {
            $scope.setFlag("[", "[ТН+ФВ+Доз]");
            event.preventDefault();
        }
    });


    $scope.openSetPromoDialog = function () {
        var selectedAndFilteredRows = getSelectedAndFilteredRows();

        if (selectedAndFilteredRows.length !== 1) {
            return;
        }

        var drugClassifierInWorkId = getIds(selectedAndFilteredRows)[0];

        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Systematization/_SystematizationSetPromoView.html',
            controller: 'SystematizationSetPromoController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                promoValue: function () {
                    return selectedAndFilteredRows[0].Promo;
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                $scope.drugLoading = $http.post('/Systematization/SetPromo/',
                    JSON.stringify({ drugClassifierInWorkId: drugClassifierInWorkId, promoValue: data.promoValue }))
                    .then(function () {
                        selectedAndFilteredRows[0].Promo = data.promoValue;
                    },
                        function () {
                            errorHandlerService.showResponseError(response);
                        });
            },
            // cancel
            function () {
            }
        );
    };

    $scope.sort = {
        column: '',
        descending: false
    };

    $(window).keydown(function (event) {
        if (event.keyCode === 16)
            setIsShift(true);
    });

    $(window).keyup(function (event) {
        if (event.keyCode === 16)
            setIsShift(false);
    });


    function setIsShift(value) {
        if (value && !$('#drug-grid-block').hasClass('disableSelection')) {
            $scope.selectedText = commonService.getSelectionText();
        }

        if (value)
            $('#drug-grid-block').addClass('disableSelection');
        else {
            $('#drug-grid-block').removeClass('disableSelection');
        }
    }

    $scope.openHelp = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Systematization/_HelpView.html',
            controller: 'HelpController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function () {
        }, function () {
        });
    };

    //=========== Drugs ==================

    $scope.drugLoading = null;
    $scope.classifierLoading = null;
    $scope.GoodsCategory = null;
    $scope.GoodsCategorySection = [];
    // загрузить обрабатываемые drugs
    (function () {
        getPeriodName();
        $scope.drugLoading = $http.post('/Systematization/LoadDrugs/')
            .then(function (response) {
                $scope.gridOptions.data = response.data;

                $timeout(function () {
                    var visibleRows = $scope.gridApi.core.getVisibleRows();
                    if (visibleRows.length > 0) {
                        $scope.gridApi.selection.selectRow(visibleRows[0].entity);
                    }
                }, 200);

                updateCount();
                $scope.drugLoading = $http.post('/Systematization/DrugGoodInit/')
                    .then(function (response) {
                        $scope.GoodsCategory = response.data;
                        $scope.GoodsCategory.forEach(function (item) {
                            if ($scope.GoodsCategorySection.indexOf(item.Section) === -1) {
                                $scope.GoodsCategorySection.push(item.Section);
                            }
                        });
                    },
                        function () {
                            $scope.message = 'Unexpected Error';
                        });
            },
                function () {
                    $scope.message = 'Unexpected Error';
                });
    }
    )();

    // загрузить обрабатываемые drugs
    function LoadDrugs() {
        getPeriodName();
        $scope.drugLoading = $http.post('/Systematization/LoadDrugs/')
            .then(function (response) {
                $scope.gridOptions.data = response.data;

                $timeout(function () {
                    var visibleRows = $scope.gridApi.core.getVisibleRows();
                    if (visibleRows.length > 0) {
                        $scope.gridApi.selection.selectRow(visibleRows[0].entity);
                    }
                }, 200);
            },
                function () {
                    $scope.message = 'Unexpected Error';
                });
    };

    function getPeriodName() {
        $scope.drugLoading = $http.post('/Systematization/GetPeriodName/')
            .then(function (response) {
                $scope.periodName = response.data;
            }, function () {
                $scope.message = 'Unexpected Error';
                $scope.periodName = '-';
            });
    }

    var currentClassifiedDrug;

    var checkIsSynAdd;
    $scope.setSelectedDrugsRightClick = function (row) {

        if (!row.isSelected) {
            clearSelected();
            row.setSelected(true);
        }

        var selectedAndFilteredRows = getSelectedAndFilteredRows();

        $scope.selectedCount = selectedAndFilteredRows.length;

        //Проверяем выделенный текст
        $scope.IsSynAdd = checkIsSynAdd();

        //Проверяем можем ли добавить в синонимы
        currentClassifiedDrug = getCurrentClassifiedDrug();

        $scope.canToSyn = currentClassifiedDrug !== null;
    };

    // Результат фильтра - получить новые drugs для обработки
    function getDrugs(filter) {
        $scope.drugLoading =
            $http.post('/Systematization/GetDrugs/', JSON.stringify({ drugFilterParameters: filter, rettype: 1 }))
                .then(function (response) {
                    $scope.gridOptions.data = response.data;

                    $timeout(function () {
                        var visibleRows = $scope.gridApi.core.getVisibleRows();
                        if (visibleRows.length > 0) {
                            $scope.gridApi.selection.selectRow(visibleRows[0].entity);
                        }
                    }, 200);

                    updateCount();
                }, function (err) {
                    messageBoxService.showError(err.data.message);
                    $scope.message = 'Unexpected Error';
                });
    }

    // клик по кнопке "Забрать данные"
    $scope.getDrugs = function () {
        var modalDrugsInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Systematization/_DrugFilterView.html',
            controller: 'DrugFilterController',
            size: 'lg',
            backdrop: 'static'
        });

        modalDrugsInstance.result.then(function (filter) {
            $location.hash(null);
            getDrugs(filter);
        }, function (err) {
            $location.hash(null);
        });
    };

    // клик по кнопке "Вернуть данные"
    $scope.setDrugs = function () {

        $scope.drugLoading = $http.post('/Systematization/SetDrugs/')
            .then(function () {
                $scope.gridOptions.data = [];
                updateCount();
            }, function (err) {
                $scope.message = 'Unexpected Error';
            });
    };

    function ClearDrugId(item) {
        item.DrugId = null;
        item.GoodsId = null;
        item.OwnerTradeMarkId = null;
        item.PackerId = null;
        item.TradeName = null;
        item.OwnerTradeMark = null;
        item.Packer = null;
        item.DrugDescription = null;
        item.RealPackingCount = null;
    }

    $scope.clearDrugId = function (param, checked) {

        if (!checked) {
            checkCount(null, $scope.clearDrugId);
            return;
        }

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        var selectedDrugIds = getIds(selectedAndFilteredRows);

        $scope.classifierLoading =
            $http.post('/Classifier/ClearClassifierToDrugs/', JSON.stringify({ DrugInWorkIdList: selectedDrugIds }))
                .then(function () {
                    selectedAndFilteredRows.forEach(function (item) {
                        ClearDrugId(item);
                        item.HasChanges = true;
                    });

                    processedEventHandler(selectedAndFilteredRows);

                }, function () {
                    $scope.message = 'Unexpected Error';
                });
    };
    $scope.PrioritetDrugClassifier = function (value) {
        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        var selectedDrugIds = getIds(selectedAndFilteredRows);

        var json = JSON.stringify({ drugClassifierInWork: selectedDrugIds, value: value });

        $scope.drugLoading = $http.post('/Systematization/PrioritetWords_isControl/', json)
            .then(function () {
                selectedAndFilteredRows.forEach(function (item) {
                    item.PrioritetWords_isControl = value;
                });
            }, function () {
                $scope.message = 'Unexpected Error';
            });


    };
    $scope.forIsOther = function (value, GoodsCategoryId, GoodsCategoryName, checked) {

        /*  if (!checked) {
              checkCount2param(value, type, $scope.forIsOther);
              return;
          }*/

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        var selectedDrugIds = getIds(selectedAndFilteredRows);

        var json = JSON.stringify({ drugClassifierInWork: selectedDrugIds, value: value, GoodsCategoryId: GoodsCategoryId });

        $scope.drugLoading = $http.post('/Systematization/ForIsOther/', json)
            .then(function (response) {
                var drugs = response.data;

                selectedAndFilteredRows.forEach(function (item) {
                    item.IsOther = value;

                    if (value) {
                        item.GoodsCategoryId = GoodsCategoryId;
                        item.GoodsCategoryName = GoodsCategoryName;
                        item.ForChecking = false;
                        item.ForAdding = false;
                    }
                    else {
                        item.GoodsCategoryId = null;
                        item.GoodsCategoryName = null;
                    }

                    let drug = drugs.find(element => element.Id == item.Id);
                    if (drug) {
                        item.UserName = drug.UserName;
                    }

                    ClearDrugId(item);
                    item.HasChanges = true;
                });

                processedEventHandler(selectedAndFilteredRows);

            }, function () {
                $scope.message = 'Unexpected Error';
            });
    };

    $scope.forIsError = function (value, checked) {

        if (!checked) {
            checkCount(value, $scope.forIsError);
            return;
        }

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        var selectedDrugIds = getIds(selectedAndFilteredRows);

        var json = JSON.stringify({ drugClassifierInWork: selectedDrugIds, value: value });

        $scope.drugLoading = $http.post('/Systematization/ForIsError/', json)
            .then(function () {
                selectedAndFilteredRows.forEach(function (item) {
                    item.IsError = value;
                    item.HasChanges = true;
                });

                processedEventHandler(selectedAndFilteredRows);

            }, function () {
                $scope.message = 'Unexpected Error';
            });
    };

    // добавить/снять метку "на проверку"
    $scope.forChecking = function (value, checked) {

        if (!checked) {
            checkCount(value, $scope.forChecking);
            return;
        }

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        var selectedDrugIds = getIds(selectedAndFilteredRows);

        var json = JSON.stringify({ drugClassifierInWork: selectedDrugIds, value: value });

        $scope.drugLoading = $http.post('/Systematization/ForChecking/', json)
            .then(function () {
                selectedAndFilteredRows.forEach(function (item) {
                    item.ForChecking = value;
                    if (value) {
                        item.ForAdding = false;
                        ClearDrugId(item);
                    }
                    item.HasChanges = true;
                });

                processedEventHandler(selectedAndFilteredRows);

            }, function () {
                $scope.message = 'Unexpected Error';
            });
    };

    $scope.SetSuperCheck = function (value) {

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        var selectedDrugIds = getIds(selectedAndFilteredRows);
        var json = JSON.stringify({ drugClassifierInWork: selectedDrugIds, value: value });

        $scope.drugLoading = $http.post('/Systematization/SetSuperCheck/', json)
            .then(function () {
                selectedAndFilteredRows.forEach(function (item) {
                    item.SuperCheck = value;
                    item.HasChanges = true;
                });

                processedEventHandler(selectedAndFilteredRows);

            }, function () {
                $scope.message = 'Unexpected Error';
            });
    }


    // добавить/снять метку "на добавление"
    $scope.forAdding = function (value, checked) {

        if (!checked) {
            checkCount(value, $scope.forAdding);
            return;
        }

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        var selectedDrugIds = getIds(selectedAndFilteredRows);

        var json = JSON.stringify({ drugClassifierInWork: selectedDrugIds, value: value });

        $scope.drugLoading = $http.post('/Systematization/ForAdding/', json)
            .then(function () {
                selectedAndFilteredRows.forEach(function (item) {
                    item.ForAdding = value;
                    if (value) {
                        item.ForChecking = false;
                        ClearDrugId(item);
                    }
                    item.HasChanges = true;
                });

                processedEventHandler(selectedAndFilteredRows);

            }, function () {
                $scope.message = 'Unexpected Error';
            });
    };



    //=========== Classifier ==================


    $scope.classifierFilter = new Object;
    $scope.classifierFilterDOP = {};

    $scope.gridClassifierOptions = uiGridCustomService.createOptions('Systematization_ClassifierGrid');
    let booleanCellTemplate = '<div class="ui-grid-cell-contents"><span class="glyphicon" ng-class="COL_FIELD===true ? \'ui-grid-true glyphicon-check\' : COL_FIELD===false ? \'ui-grid-false glyphicon-unchecked\' : \'ui-grid-null glyphicon-question-sign\'"></span></div>';

    var gridClassifierOptions = {
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
            { name: 'CertificateNumber', field: 'RegistrationCertificateNumber', filter: { condition: uiGridCustomService.condition } },
            { name: 'ClassifierId', field: 'ClassifierId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'GoodsId', field: 'GoodsId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'TradeName', field: 'TradeName', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { name: 'INNGroup', field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
            { name: 'DrugDescription', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint },
            { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'OwnerTradeMark', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
            { name: 'PackerId', field: 'PackerId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'Packer', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
            { name: 'IsOther', field: 'IsOther', type: 'boolean', filter: { condition: uiGridCustomService.booleanConditionX }, cellTemplate: booleanCellTemplate },
            { name: 'ToRetail', field: 'ToRetail', displayName: 'Обязательно к простановке', type: 'boolean', filter: { condition: uiGridCustomService.booleanConditionX }, cellTemplate: booleanCellTemplate },
            { name: 'Price', field: 'Price', filter: { condition: uiGridCustomService.condition } },
            { name: 'Comment', field: 'Comment', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateHint }
        ],
        rowTemplate: '_rowClassifierTemplate.html'
    };

    angular.extend($scope.gridClassifierOptions, gridClassifierOptions);

    $scope.gridClassifierOptions.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.gridClassifierApi = gridApi;
        $scope.gridClassifierApi.grid.registerRowsProcessor(gridClassifierApiProcessor, 200);
    };
    $scope.Classifier_filter_HidePacking = false;
    $scope.Classifier_filter_change = function () {
        $scope.gridClassifierApi.grid.refresh();
    };
    function gridClassifierApiProcessor(renderableRows) {
        var isShow = true;

        function visibleByStatus(item) {
            if ($scope.Classifier_filter_HidePacking === 1) {
                if (item.RealPackingCount !== item.ConsumerPackingCount) {
                    return false;
                }
            }
            return true;
        }

        renderableRows.forEach(function (row) {
            if (!visibleByStatus(row.entity))
                row.visible = false;
        });

        return renderableRows;
    }

    // результат фильтра - получить список данных классификатора
    function loadClassifier(filter) {
        $scope.classifierLoading =
            $http.post('/Classifier/GetClassifier/', JSON.stringify({ filter: filter, rettype: 1 }))
                .then(function (response) {
                    $scope.gridClassifierOptions.data = response.data;
                }, function () {
                    $scope.message = 'Unexpected Error';
                });
    }

    function clearAllFlags(item) {
        //item.IsOther = false;
        item.type = 0;
        item.ForChecking = false;
        item.ForAdding = false;
    }

    $scope.setClassifierToDrugs = function (element, checked) {

        if (!checked) {
            checkCount(element, $scope.setClassifierToDrugs);
            return;
        }

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        if (selectedAndFilteredRows.length === 0)
            return;

        var selectedDrugIds = getIds(selectedAndFilteredRows);

        var parameters = {
            "DrugInWorkIdList": selectedDrugIds,
            "DrugId": element.DrugId,
            "GoodsId": element.GoodsId,
            "IsOther": element.IsOther,
            "OwnerTradeMarkId": element.OwnerTradeMarkId,
            "PackerId": element.PackerId,
            "RealPackingCount": element.RealPackingCount
        };

        $scope.classifierLoading = $http.post('/Classifier/SetClassifierToDrugs/', JSON.stringify({ parameters: parameters }))
            .then(function (ReLoadDrugs) {
                selectedAndFilteredRows.forEach(function (item) {
                    item.DrugId = element.DrugId;
                    item.GoodsId = element.GoodsId;
                    item.IsOther = element.IsOther;

                    if (item.GoodsId > 0) {
                        item.GoodsCategoryId = element.GoodsCategoryId;
                        item.GoodsCategoryName = element.INNGroup;
                    }
                    else {
                        item.GoodsCategoryId = null;
                        item.GoodsCategoryName = null;
                    }


                    item.OwnerTradeMarkId = element.OwnerTradeMarkId;
                    item.PackerId = element.PackerId;
                    item.TradeName = element.TradeName;
                    item.OwnerTradeMark = element.OwnerTradeMark;
                    item.Packer = element.Packer;
                    item.FormProductId = element.FormProductId;
                    item.TradeNameId = element.TradeNameId;
                    item.DosageGroupId = element.DosageGroupId;
                    item.DrugDescription = element.DrugDescription;
                    item.RealPackingCount = element.RealPackingCount;
                    clearAllFlags(item);
                    item.HasChanges = true;
                });

                processedEventHandler(selectedAndFilteredRows);

                if (ReLoadDrugs.data.ReLoadDrugs) {
                    LoadDrugs(); // перезагрузка доп. ассортименнта
                    loadClassifierDOP($scope.classifierFilterDOP); // перезагрузка фильтра по доп. ассортименту
                }


            }, function () {
                $scope.message = 'Unexpected Error';
            });
    };

    //Выделение нижнего элемента
    $scope.idSelectedClassifier = -1;

    $scope.setSelectedClassifier = function (id) {
        $scope.idSelectedClassifier = id;
    };


    // собираем информацию по фильтру и выводим её:
    var getFilterDescription = function () {
        $scope.FilterDescription = [];
        tryAddFilterDescription('ЛП', 'Тип');

        tryAddFilterDescription($scope.classifierFilter.TradeName, 'TradeName');
        tryAddFilterDescription($scope.classifierFilter.RuNumber, 'RuNumber');
        tryAddFilterDescription($scope.classifierFilter.InnGroup, 'InnGroup');
        tryAddFilterDescription($scope.classifierFilter.FormProduct, 'FormProduct');
        tryAddFilterDescription($scope.classifierFilter.DosageGroup, 'DosageGroup');
        tryAddFilterDescription($scope.classifierFilter.ConsumerPackingCount, 'ConsumerPackingCount');
        tryAddFilterDescription($scope.classifierFilter.OwnerTradeMark, 'OwnerTradeMark');
        tryAddFilterDescription($scope.classifierFilter.Packer, 'Packer');
        tryAddFilterDescription($scope.classifierFilter.DrugId, 'DrugId');
        tryAddFilterDescription($scope.classifierFilter.GoodsId, 'GoodsId');
        tryAddFilterDescription($scope.classifierFilter.OwnerTradeMarkId, 'OwnerTradeMarkId');
    };

    // клик по кнопке "Поиск по справочнику"
    $scope.searchClassifier = function () {
        var modalClassifieInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Systematization/_ClassifierFilterView.html',
            controller: 'ClassifierFilterController',
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
            getFilterDescription();
            loadClassifier($scope.classifierFilter);
        }, function () {
        });
    };
    // клик по кнопке "Поиск по справочнику"
    $scope.searchClassifierGoods = function () {
        var modalClassifierInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GoodsSystematization/_GoodsClassifierFilterView.html',
            controller: 'GoodsClassifierFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                classifierFilter: function () {
                    return $scope.classifierFilterDOP;
                }
            }
        });

        modalClassifierInstance.result.then(function (classifierFilter) {
            $scope.classifierFilterDOP = classifierFilter;
            updateFilterDescriptionDOP();
            loadClassifierDOP($scope.classifierFilterDOP);
        }, function () {
        });
    };

    // результат фильтра - получить список данных классификатора
    function loadClassifierDOP(filter) {
        var selectedRows = getSelectedAndFilteredRows();
        var goodsCategoryIdList = [];
        selectedRows.forEach(function (item) {
            var id = item.GoodsCategoryId;
            if (id !== undefined && id !== null && goodsCategoryIdList.indexOf(id) === -1) {
                goodsCategoryIdList.push(id);
                tryAddFilterDescription(item.GoodsCategoryName, 'категория');
            }
        });

        $scope.classifierLoading =
            $http.post('/GoodsClassifier/GetClassifier/', JSON.stringify({ filter: filter, goodsCategoryIdList: goodsCategoryIdList, rettype: 1 }))
                .then(function (response) {
                    $scope.gridClassifierOptions.data = response.data;
                }, function () {
                    $scope.message = 'Unexpected Error';
                });
    }
    // собираем информацию по фильтру и выводим её:
    function updateFilterDescriptionDOP() {
        $scope.FilterDescription = [];
        tryAddFilterDescription('ДОП', 'Тип');
        tryAddFilterDescription($scope.classifierFilterDOP.GoodsTradeName, 'GoodsTradeName');
        tryAddFilterDescription($scope.classifierFilterDOP.OwnerTradeMark, 'OwnerTradeMark');
        tryAddFilterDescription($scope.classifierFilterDOP.GoodsTradeNameId, 'GoodsTradeNameId');
        tryAddFilterDescription($scope.classifierFilterDOP.Packer, 'Packer');

        tryAddFilterDescription($scope.classifierFilterDOP.GoodsId, 'GoodsId');
        tryAddFilterDescription($scope.classifierFilterDOP.OwnerTradeMarkId, 'OwnerTradeMarkId');
        tryAddFilterDescription($scope.classifierFilterDOP.PackerId, 'PackerId');
        tryAddFilterDescription($scope.classifierFilterDOP.Used, 'Used');
        tryAddFilterDescription($scope.classifierFilterDOP.ToRetail, 'ToRetail');
    }

    function tryAddFilterDescription(value, name) {
        if (!value)
            return;
        $scope.FilterDescription.push({ Name: name, Value: value });
    }

    function loadClassifierFromHotKey() {

        var value = commonService.getSelectionText();

        if (!value)
            value = $scope.selectedText;

        if (!value)
            return;
        $scope.FilterDescription = [];

        //Проверка может ли это быть ДОП
        var selectedRows = getSelectedAndFilteredRows();
        var goodsCategoryIdList = [];
        selectedRows.forEach(function (item) {
            var id = item.GoodsCategoryId;
            if (id !== undefined && id !== null && id > 0 && goodsCategoryIdList.indexOf(id) === -1) {
                goodsCategoryIdList.push(id);
                tryAddFilterDescription(item.GoodsCategoryName, 'категория');
            }
        });
        if (goodsCategoryIdList.length > 0 || selectedRows[0].IsOther === true) {
            tryAddFilterDescription('ДОП', 'Тип');
            tryAddFilterDescription(value, 'Текст');
            $scope.classifierLoading =
                $http.post('/GoodsClassifier/GetClassifierFromHotKey/', JSON.stringify({ value: value, goodsCategoryIdList: goodsCategoryIdList, rettype: 1 }))
                    .then(function (response) {
                        $scope.gridClassifierOptions.data = response.data;
                    }, function () {
                        $scope.message = 'Unexpected Error';
                    });
        }
        else {
            tryAddFilterDescription('ЛП', 'Тип');
            tryAddFilterDescription(value, 'Текст');
            $scope.classifierLoading = $http.post('/Classifier/GetClassifierFromHotKey/', JSON.stringify({ value: value, rettype: 1 }))
                .then(function (response) {
                    $scope.gridClassifierOptions.data = response.data;
                }, function () {
                    $scope.message = 'Unexpected Error';
                });
        }
    }

    //========= Synoym ===========
    $scope.IsSynAdd = false;

    //Выделен фрагмент текста
    checkIsSynAdd = function () {
        $scope.selectedText = commonService.getSelectionText();
        return $scope.selectedText !== null && $scope.selectedText !== undefined && $scope.selectedText.length > 0;
    };

    currentClassifiedDrug = null;

    function getCurrentClassifiedDrug() {

        $scope.canToSyn = false;

        var selectedAndFilteredRows = getSelectedAndFilteredRows();
        var selectedDrugIds = getIds(selectedAndFilteredRows);

        if (selectedDrugIds.length !== 1) {
            return null;
        }

        var drugClearId = selectedDrugIds[0];

        var result = getById($scope.gridOptions.data, drugClearId);

        return result !== null && result.DrugId !== null ? result : null;
    }

    function getById(arr, id) {
        for (var d = 0, len = arr.length; d < len; d += 1) {
            if (arr[d].Id === id) {
                return arr[d];
            }
        }

        return null;
    }


    function toSyn(synTableName) {
        if (currentClassifiedDrug === null || currentClassifiedDrug === undefined)
            return;

        var newSyn = {
            SynTableName: synTableName,
            DrugClearId: currentClassifiedDrug.DrugClearId,
            Value: $scope.selectedText
        };

        if (synTableName === 'SynINNGroup') {
            newSyn.OriginalId = currentClassifiedDrug.INNGroupId;
        } else
            if (synTableName === 'SynFormProduct') {
                newSyn.OriginalId = currentClassifiedDrug.FormProductId;
            } else
                if (synTableName === 'SynTradeName') {
                    newSyn.OriginalId = currentClassifiedDrug.TradeNameId;
                } else
                    if (synTableName === 'SynDosageGroup') {
                        newSyn.OriginalId = currentClassifiedDrug.DosageGroupId;
                    }

        $scope.classifierLoading = $http.post('/SearchTerms/SetSynonym/', JSON.stringify({ synonymJson: newSyn }))
            .then(function () {

            }, function () {
                $scope.message = 'Unexpected Error';
            });
    }

    $scope.toSyn = toSyn;

    //========== Synoym ============



    //Проверка количества изменяемых строчек

    function checkCount(param, funcRun) {
        var selectedAndFilteredRows = getSelectedAndFilteredRows();

        //Проверяем сколько элементов выделено
        if (selectedAndFilteredRows.length < 50) {
            funcRun(param, true);
            return;
        }

        messageBoxService.showConfirm('Выбрано более 50 записей. Продолжить?', 'Внимание')
            .then(function () {
                funcRun(param, true);
            },
                function () {
                }
            );
    }

    function checkCount2param(param1, param12, funcRun) {
        var selectedAndFilteredRows = getSelectedAndFilteredRows();

        //Проверяем сколько элементов выделено
        if (selectedAndFilteredRows.length < 50) {
            funcRun(param1, param12, true);
            return;
        }

        messageBoxService.showConfirm('Выбрано более 50 записей. Продолжить?', 'Внимание')
            .then(function () {
                funcRun(param1, param12, true);
            },
                function () {
                }
            );
    }

}
