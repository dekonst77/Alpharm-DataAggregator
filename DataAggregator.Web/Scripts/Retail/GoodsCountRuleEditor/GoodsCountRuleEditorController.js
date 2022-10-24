angular
    .module('DataAggregatorModule')
    .controller('GoodsCountRuleEditorController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', '$filter', '$translate', 'messageBoxService', GoodsCountRuleEditorController]);

function GoodsCountRuleEditorController($scope, $http, $uibModal, uiGridCustomService, formatConstants, $filter, $translate, messageBoxService) {

    var countRuleEditorGridApi = undefined;

    $scope.rule = {};
    $scope.regionDisplayValue = undefined;

    var translatedRegionNames = {
        russia: $translate.instant('COUNTRIES.RUSSIA'),
        moscow: $translate.instant('CITIES.MOSCOW'),
        saintPetersburg: $translate.instant('CITIES.SAINT_PETERSBURG')
    };

    $scope.actualityTypeList =
    [
        { Id: 0, Name: $translate.instant('RETAIL.COUNT_RULE_EDITOR.ACTUALITY_TYPES.ALL') },
        { Id: 1, Name: $translate.instant('RETAIL.COUNT_RULE_EDITOR.ACTUALITY_TYPES.ACTUAL') },
        { Id: 2, Name: $translate.instant('RETAIL.COUNT_RULE_EDITOR.ACTUALITY_TYPES.NOT_ACTUAL') }
    ];

    $scope.editMode = false;

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    // Фильтр
    $scope.filter = {
        date: previousMonthDate,
        actualityType: $scope.actualityTypeList[1]
    };

    $scope.countRuleEditorGrid = {
        options: uiGridCustomService.createOptions('GoodsCountRuleEditor_Grid')
    };

    var gridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        multiSelect: false,
        rowHeight: 50
    };


    $scope.countRuleEditorGrid.options.columnDefs = [
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.RULE', field: 'Id', width: 100, type: 'number' },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.CHANGED', field: 'getChangedBy()', width: 150,
            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Surname}}<br/>{{row.entity.getDateAsString(row.entity.Date)}}</div>'
        },
        { displayName: 'REGION', field: 'getRegion()', width: 200 },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.SOURCE_DESTINATION', field: 'getDistributionSourceDestination()',
            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.FullGoodsDescription}}<br/>{{row.entity.DistributionFullGoodsDescription}}</div>'
        },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.TOP_COUNT', field: 'getTopCountFromTo()', width: 70,
            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.TopCountFrom}}<br/>{{row.entity.TopCountTo}}</div>'
        },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.SELLING_PERCENT', field: 'SellingSumPart', width: 90, type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT }
    ];

    angular.extend($scope.countRuleEditorGrid.options, gridOptions);


    $scope.countRuleEditorGrid.options.onRegisterApi = function (gridApi) {

        countRuleEditorGridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.rule = angular.copy(row.entity);
        });

    };

    function getRegion() {
        if (this.RegionRus)
            return translatedRegionNames.russia;

        if (this.RegionMsk)
            return translatedRegionNames.moscow;

        if (this.RegionSpb)
            return translatedRegionNames.saintPetersburg;

        return this.Region;
    }

    function getDateAsString(date) {
        return $filter('date')(new Date(date), 'dd.MM.yyyy HH:mm');
    }

    function getChangedBy() {
        return this.Surname + ' ' + this.getDateAsString(this.Date);
    }

    function getDistributionSourceDestination() {
        return this.FullGoodsDescription + ' ' + this.DistributionFullGoodsDescription;
    }

    function getTopCountFromTo() {
        return this.TopCountFrom + ' ' + this.TopCountTo;
    }

    function updateRowFunctions(row) {
        row.getRegion = getRegion;
        row.getDateAsString = getDateAsString;
        row.getChangedBy = getChangedBy;
        row.getDistributionSourceDestination = getDistributionSourceDestination;
        row.getTopCountFromTo = getTopCountFromTo;
    }

    function loadRules() {
        var data =
        {
            year: $scope.filter.date.getFullYear(),
            month: $scope.filter.date.getMonth() + 1,
            actualityType: $scope.filter.actualityType.Id
        };

        $scope.loading = $http.post('/GoodsCountRuleEditor/LoadRules', JSON.stringify(data))
            .then(function (response) {
                $scope.countRuleEditorGrid.options.data = response.data;


                angular.forEach($scope.countRuleEditorGrid.options.data, updateRowFunctions);
            },
            function () {
                alert('Ошибка');
            });
    }

    $scope.$watch(function () { return $scope.filter; },
        function () {
            loadRules();
        },
        true);

    // Доступность кнопок
    $scope.canCreate = function () { return !$scope.editMode; };
    $scope.canEdit = function () { return !$scope.editMode && getSelectedRows().length === 1; };
    $scope.canSave = function () { return $scope.editMode && fieldsAreValid(); };
    $scope.canCancel = function () { return $scope.editMode; };
    $scope.canTransfer = function () { return !$scope.editMode && getSelectedRows().length === 1; };
    $scope.canDelete = function () { return !$scope.editMode && getSelectedRows().length === 1; };
    $scope.canClearFields = function () {
        return $scope.editMode &&
        (
            $scope.rule.RegionRus ||
            $scope.rule.RegionMsk ||
            $scope.rule.RegionSpb ||
            $scope.rule.RegionCode ||

            $scope.rule.TopCountFrom ||
            $scope.rule.TopCountTo ||

            $scope.rule.SellingSumPart ||

            $scope.rule.FullGoodsDescription ||
            $scope.rule.DistributionFullGoodsDescription
        );
    };

    function getSelectedRows() {
        return countRuleEditorGridApi ? countRuleEditorGridApi.selection.getSelectedRows() : [];
    }

    function fieldsAreValid() {
        var rule = $scope.rule;

        if (!(
            rule.RegionRus ||
            rule.RegionMsk ||
            rule.RegionSpb ||
            rule.RegionCode))
            return false;

        if (!((rule.TopCountFrom && rule.TopCountFrom > 0) || rule.GoodsId))
            return false;

        if (!((rule.TopCountTo && rule.TopCountTo > 0) || rule.DistributionGoodsId))
            return false;

        if (rule.SellingSumPart === undefined || !(rule.SellingSumPart >= 0 && rule.SellingSumPart <= 100))
            return false;

        return true;
    }

    // Кнопки
    $scope.createItem = function () {
        $scope.editMode = true;
        $scope.rule = {};
    };

    $scope.editItem = function () {
        $scope.editMode = true;
    };

    $scope.cancelEditing = function () {
        $scope.editMode = false;

        var rules = getSelectedRows();
        if (rules.length === 1)
            $scope.rule = angular.copy(rules[0]);
        else
            $scope.rule = {};
    };

    $scope.clearFields = function () {
        var rule =
            {
                Id: $scope.rule.Id
            };

        $scope.rule = rule;
    };

    $scope.$watch(function () { return $scope.rule.Region; },
        function (newValue) {
            $scope.regionDisplayValue = newValue;
        },
        true);

    // Поиск региона
    $scope.searchRegion = function(value) {
        $scope.loading = $http.post('/Region/SearchRegion/', JSON.stringify({ Value: value }))
            .then(function(response) {
            return response.data;
        });

        return $scope.loading;
    };

    // Перемигивание выбора региона
    $scope.regionRusChanged = function() {
        var rule = $scope.rule;

        if (rule.RegionRus) {
            rule.RegionMsk = false;
            rule.RegionSpb = false;
            rule.Region = '';
            rule.RegionCode = undefined;
        }
    };

    $scope.regionMskChanged = function () {
        var rule = $scope.rule;

        if (rule.RegionMsk) {
            rule.RegionRus = false;
            rule.RegionSpb = false;
            rule.Region = '';
            rule.RegionCode = undefined;
        }
    };

    $scope.regionSpbChanged = function () {
        var rule = $scope.rule;

        if (rule.RegionSpb) {
            rule.RegionRus = false;
            rule.RegionMsk = false;
            rule.Region = '';
            rule.RegionCode = undefined;
        }
    };

    $scope.regionChanged = function (item) {
        var rule = $scope.rule;

        rule.Region = item !== undefined ? item.Region : undefined;
        rule.RegionCode = item !== undefined ? item.RegionCode : undefined;

        rule.RegionRus = false;
        rule.RegionMsk = false;
        rule.RegionSpb = false;
    };

    // Форма поиска препарата
    $scope.searchGoods = function (isDistribution) {
        var rule = $scope.rule;

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/GoodsCountRuleEditor/_SearchGoodsView.html',
            controller: 'SearchGoodsController',
            size: 'full',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                model: function () {
                    return {
                        Year: $scope.filter.date.getFullYear(),
                        Month: $scope.filter.date.getMonth() + 1,
                        RegionRus: rule.RegionRus,
                        RegionMsk: rule.RegionMsk,
                        RegionSpb: rule.RegionSpb,
                        RegionCode: rule.RegionCode,
                        Region: rule.Region
                    };
                }
            }
        });

        modalInstance.result.then(function (value) {
            if (isDistribution) {
                rule.DistributionGoodsId = value.GoodsId;
                rule.DistributionOwnerTradeMarkId = value.OwnerTradeMarkId;
                rule.DistributionPackerId = value.PackerId;
                rule.DistributionFullGoodsDescription = value.GoodsDescription;
                rule.DistributionGoodsTradeName = value.GoodsTradeName;
                rule.DistributionClassifierId = value.ClassifierId;
            } else {
                rule.GoodsId = value.GoodsId;
                rule.OwnerTradeMarkId = value.OwnerTradeMarkId;
                rule.PackerId = value.PackerId;
                rule.FullGoodsDescription = value.GoodsDescription;
                rule.GoodsTradeName = value.GoodsTradeName;
                rule.ClassifierId = value.ClassifierId;
            }
        }, function() {});
    };

    // Перемигивание топ / источник / назначение
    $scope.$watch(function () { return $scope.rule.TopCountFrom; },
        function (newValue) {
            if (!newValue && newValue !== 0)
                return;

            var rule = $scope.rule;

            rule.GoodsId = undefined;
            rule.OwnerTradeMarkId = undefined;
            rule.PackerId = undefined;
            rule.FullGoodsDescription = undefined;
            rule.GoodsTradeName = undefined;
        },
        true);

    $scope.$watch(function () { return $scope.rule.TopCountTo; },
        function (newValue) {
            if (!newValue && newValue !== 0)
                return;

            var rule = $scope.rule;

            rule.DistributionGoodsId = undefined;
            rule.DistributionOwnerTradeMarkId = undefined;
            rule.DistributionPackerId = undefined;
            rule.DistributionFullGoodsDescription = undefined;
            rule.DistributionGoodsTradeName = undefined;
        },
        true);

    $scope.$watch(function () { return $scope.rule.GoodsId; },
        function (newValue) {
            if (!newValue)
                return;

            $scope.rule.TopCountFrom = undefined;
        },
        true);

    $scope.$watch(function () { return $scope.rule.DistributionGoodsId; },
        function (newValue) {
            if (!newValue)
                return;

            $scope.rule.TopCountTo = undefined;
        },
        true);


    function findIndexById(items, id) {
        var index = -1;
        for (var i = 0; i < items.length; i++)
            if (items[i].Id === id) {
                index = i;
                break;
            }

        return index;
    }

    // Удалить правило
    $scope.deleteItem = function () {
        var id = $scope.rule.Id;

        var result = messageBoxService.showConfirm('Удалить правило?');

        result.then(function () {

            $scope.loading = $http.post('/GoodsCountRuleEditor/DeleteRow', JSON.stringify({ id: id }))
                .then(function () {
                    var items = $scope.countRuleEditorGrid.options.data;

                    var index = findIndexById(items, id);

                    if (index !== -1) {
                        items.splice(index, 1);
                        // clear all fields
                        $scope.rule = {};
                    }
                }, function () {
                    alert('Ошибка');
                });

        }, function () {
        });

    };

    $scope.saveItem = function () {
        if ($scope.rule.ClassifierId && $scope.rule.ClassifierId === $scope.rule.DistributionClassifierId) {
            messageBoxService.showError('Источник и назначение не должны совпадать');
            return;
        }

        $scope.rule.Year = $scope.filter.date.getFullYear();
        $scope.rule.Month = $scope.filter.date.getMonth() + 1;

        $scope.loading = $http.post('/GoodsCountRuleEditor/SaveRow', JSON.stringify({ model: $scope.rule }))
            .then(function (response) {
                    var data = response.data;

                    if (data.isError) {
                        alert(data.errorMessage);
                    } else {
                        var items = $scope.countRuleEditorGrid.options.data;

                        updateRowFunctions(data);

                        if ($scope.rule.Id === undefined) {
                            items.unshift(data);
                        } else {
                            var id = data.Id;

                            var index = findIndexById(items, id);

                            items[index] = data;
                        }

                        $scope.rule = angular.copy(data);

                        $scope.editMode = false;
                    }
                },
                function () {
                    alert('Ошибка');
                });
    };

    // копировать правило на другие периоды
    $scope.transferItem = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/GoodsCountRuleEditor/_TransferRuleView.html',
            controller: 'GoodsTransferRuleController',
            size: 'full',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                model: function () {
                    return $scope.rule;
                }
            }
        });

        modalInstance.result.then(function () {
            loadRules();
        }, function () {});
    };
}