angular
    .module('DataAggregatorModule')
    .controller('CountRuleEditorController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', '$filter', '$translate', 'messageBoxService', CountRuleEditorController]);

function CountRuleEditorController($scope, $http, $uibModal, uiGridCustomService, formatConstants, $filter, $translate, messageBoxService) {

    var countRuleEditorGridApi = undefined;

    $scope.rule = {};
    $scope.regionDisplayValue = undefined;


    $scope.editMode = false;

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    // Фильтр
    $scope.filter = {
        date: previousMonthDate,
        dateEnd: previousMonthDate,
    };

    $scope.type = [
        { id: 1, value: 'Сумма' },
        { id: 2, value: 'Количество' },
        { id: 3, value: '%' }
    ];



    $scope.SellingChange = function () {
        //Сумма
        if ($scope.SellingType == 1) {
            $scope.rule.SellingSum = $scope.Selling;
        }
        //Количество
        else if ($scope.SellingType == 2) {
            $scope.rule.SellingCount = $scope.Selling;
        }
        else if ($scope.SellingType == 3) {
            $scope.rule.SellingSumPart = $scope.Selling;
        }
    };

    $scope.PurchaseChange = function () {

        if ($scope.Purchase != null && $scope.Purchase.length == 0)
            $scope.Purchase = null;

        //Сумма
        if ($scope.PurchaseType == 1) {
            $scope.rule.PurchaseSum = $scope.Purchase;
        }
        //Количество
        else if ($scope.PurchaseType == 2) {
            $scope.rule.PurchaseCount = $scope.Purchase;
        }
        else if ($scope.PurchaseType == 3) {
            $scope.rule.PurchaseSumPart = $scope.Purchase;
        }
    };

    $scope.PurchaseTypeChange = function() {
        $scope.rule.PurchaseSum = null;
        $scope.rule.PurchaseCount = null;
        $scope.rule.PurchaseSumPart = null;
        $scope.Purchase = null;
    }

    $scope.SellingTypeChange = function () {
        $scope.rule.SellingOtherSum = null;
        $scope.rule.SellingOtherCount = null;
        $scope.rule.SellingSumPart = null;
        $scope.Selling = null;
    }

    $scope.countRuleEditorGrid = {
        options: uiGridCustomService.createOptions('CountRuleEditor_Grid')
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
        { displayName: 'PERIOD', field: 'getPeriod()', width: 100 },

        { displayName: 'REGION', field: 'getRegion()', width: 200 },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.SOURCE_DESTINATION', field: 'getDistributionSourceDestination()',
            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.FullDrugDescription}}<br/>{{row.entity.DistributionFullDrugDescription}}</div>'
        },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.TOP_COUNT', field: 'getTopCountFromTo()', width: 70,
            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.TopCountFrom}}<br/>{{row.entity.TopCountTo}}</div>'
        },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.PURCHASE_PERCENT', field: 'PurchaseSumPart', width: 80, type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.SELLING_PERCENT', field: 'SellingSumPart', width: 90, type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.PURCHASE_SUM', field: 'PurchaseSum', width: 80, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.SELLING_SUM', field: 'SellingSum', width: 90, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.PURCHASE_COUNT', field: 'PurchaseCount', width: 80, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.SELLING_COUNT', field: 'SellingCount', width: 90, type: 'number', cellFilter: formatConstants.FILTER_FLOAT_COUNT },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.OUT_USED', field: 'OutUsed', width: 25,
            cellTemplate: "<div class='ui-grid-cell-contents'>{{row.entity.OutUsed ? 'Да' : 'Нет'}}</div>"
        },
        {
            displayName: 'RETAIL.COUNT_RULE_EDITOR.IN_USED', field: 'InUsed', width: 25,
            cellTemplate: "<div class='ui-grid-cell-contents'>{{row.entity.InUsed ? 'Да' : 'Нет'}}</div>"
        }
    ];

    $scope.countRuleEditorGrid.options.rowTemplate = '_rowClassifierTemplate.html';
    angular.extend($scope.countRuleEditorGrid.options, gridOptions);


    $scope.countRuleEditorGrid.options.onRegisterApi = function (gridApi) {

        countRuleEditorGridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.rule = angular.copy(row.entity);

            if ($scope.rule.SellingSum != null && $scope.rule.SellingSum >= 0) {
                $scope.SellingType = 1;
                $scope.Selling = $scope.rule.SellingSum;
            }
            else if ($scope.rule.SellingCount != null && $scope.rule.SellingCount >= 0) {
                $scope.SellingType = 2;
                $scope.Selling = $scope.rule.SellingCount;
            }
            else {
                $scope.SellingType = 3;
                $scope.Selling = $scope.rule.SellingSumPart;
            }

            if ($scope.rule.PurchaseSum != null && $scope.rule.PurchaseSum >= 0) {
                $scope.PurchaseType = 1;
                $scope.Purchase = $scope.rule.PurchaseSum;
            }
            else if ($scope.rule.PurchaseCount != null && $scope.rule.PurchaseCount >= 0) {
                $scope.PurchaseType = 2;
                $scope.Purchase = $scope.rule.PurchaseCount;
            }
            else {
                $scope.PurchaseType = 3;
                $scope.Purchase = $scope.rule.PurchaseSumPart;
            }

            if ($scope.rule.TopCountTo > 0) {
                $scope.DestinationText = "Топ " + $scope.rule.TopCountTo;
            } else {
                $scope.DestinationText = $scope.rule.DistributionFullDrugDescription;
            }

            if ($scope.rule.TopCountFrom > 0) {
                $scope.SourceText = "Топ " + $scope.rule.TopCountFrom;
            } else {
                $scope.SourceText = $scope.rule.FullDrugDescription;
            }
        });

    };

    function getPeriod() {
        var period = this.Year + '.' + this.Month + '-';

        if (this.YearEnd != null) {
            period = period + this.YearEnd + '.' + this.MonthEnd;
        }

        return period;
    }

    function getRegion() {

        return this.Region;
    }

    function getDateAsString(date) {
        return $filter('date')(new Date(date), 'dd.MM.yyyy HH:mm');
    }

    function getChangedBy() {
        return this.Surname + ' ' + this.getDateAsString(this.Date);
    }

    function getDistributionSourceDestination() {
        return this.FullDrugDescription + ' ' + this.DistributionFullDrugDescription;
    }

    function getTopCountFromTo() {
        return this.TopCountFrom + ' ' + this.TopCountTo;
    }

    function updateRowFunctions(row) {
        row.getRegion = getRegion;
        row.getPeriod = getPeriod;
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
        };

        $scope.loading = $http.post('/CountRuleEditor/LoadRules', JSON.stringify(data))
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
    $scope.canSearch = function () {
        return $scope.rule.RegionCode != null && $scope.editMode;
    }
    $scope.canCreate = function() {
        return !$scope.editMode;

    };
    $scope.canEdit = function () { return !$scope.editMode && getSelectedRows().length === 1; };
    $scope.canSave = function () { return $scope.editMode && fieldsAreValid(); };
    $scope.canCancel = function () { return $scope.editMode; };
    $scope.canTransfer = function () { return !$scope.editMode && getSelectedRows().length === 1; };
    $scope.canDelete = function () { return !$scope.editMode && getSelectedRows().length === 1; };
    $scope.canClearFields = function () {
        return $scope.editMode &&
            (
                $scope.rule.RegionCode ||

                $scope.rule.TopCountFrom ||
                $scope.rule.TopCountTo ||

                $scope.rule.PurchaseSumPart ||
                $scope.rule.SellingSumPart ||

                $scope.rule.FullDrugDescription ||
                $scope.rule.DistributionFullDrugDescription
            );
    };

    function getSelectedRows() {
        return countRuleEditorGridApi ? countRuleEditorGridApi.selection.getSelectedRows() : [];
    };

    function fieldsAreValid() {
        var rule = $scope.rule;

        if (!(rule.RegionCode))
            return false;

        if (!((rule.TopCountFrom && rule.TopCountFrom > 0) || rule.ClassifierId >= 0))
            return false;

        if (!((rule.TopCountTo && rule.TopCountTo > 0) || rule.DistributionClassifierId >= 0))
            return false;

        //Если не заполнен ни один показатель
        //if (    (rule.PurchaseSumPart == null || rule.PurchaseSumPart == '' || !(rule.PurchaseSumPart >= 0 && rule.PurchaseSumPart <= 100)) &&
        //        (rule.SellingSumPart == null || rule.SellingSumPart == ''  || !(rule.SellingSumPart >= 0  && rule.SellingSumPart <= 100)))
        //    return false;


        var year = null;
        var month = null;

        if ($scope.filter.date != null) {
            var year = $scope.filter.date.getFullYear();
            var month = $scope.filter.date.getMonth() + 1;
        };

        var yearEnd = null;
        var monthEnd = null;
        if ($scope.filter.dateEnd != null) {
            var yearEnd = $scope.filter.dateEnd.getFullYear();
            var monthEnd = $scope.filter.dateEnd.getMonth() + 1;
        };


        //Процент закупки не болше 100 и если 100, то задана дата конца
        if (rule.PurchaseSumPart != null) {
            if (rule.PurchaseSumPart > 100 || rule.PurchaseSumPart < 0)
                return false;
            if (rule.PurchaseSumPart > 0 && rule.PurchaseSumPart < 100 && yearEnd == null)
                return false;
        }

        //Процент закупки не болше 100 и если 100, то задана дата конца
        if (rule.SellingSumPart != null) {
            if (rule.SellingSumPart > 100 || rule.SellingSumPart < 0)
                return false;
            if (rule.SellingSumPart < 100 && yearEnd == null)
                return false;
        }

        //Если суммы или колличества меньше 0
        if (rule.SellingSum != null && rule.SellingSum <= 0 ||
            rule.SellingCount != null && rule.SellingCount <= 0 ||
            rule.PurchaseSum != null && rule.PurchaseSum <= 0 ||
            rule.PurchaseCount != null && rule.PurchaseCount <= 0) {
            return false;
        }

       

        //Если заданы сумма или колличества должна стоять дата конца
        if ((rule.SellingSum > 0 || rule.SellingCount > 0 || rule.PurchaseSum > 0 || rule.PurchaseCount > 0) && yearEnd == null) {
            return false;
        }

        // Дата окончания больше даты начала
        if (yearEnd != null && yearEnd * 100 + monthEnd < year * 100 + month)
            return false;

        //Прочее на прочее
        if (rule.ClassifierId == 0 && rule.DistributionClassifierId == 0)
            return false;

        //Топ на топ
        if (rule.TopCountTo > 0 && rule.TopCountFrom > 0)
            return false;

        return true;
    }

    // Кнопки
    $scope.createItem = function () {
        $scope.editMode = true;
        $scope.rule = {};
        $scope.clearFields();
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

        $scope.SellingType = null;
        $scope.Selling = null;
        $scope.PurchaseType = null;
        $scope.Purchase = null;
        $scope.SourceText = null;
        $scope.DestinationText = null;
    };

    $scope.$watch(function () { return $scope.rule.Region; },
        function (newValue) {
            $scope.regionDisplayValue = newValue;
        },
        true);
    
    // Поиск региона
    $scope.searchRegion = function (value) {
        $scope.loading = $http.post('/Region/SearchRegionPM01/', JSON.stringify({ Value: value }))
            .then(function (response) {
                return response.data;
            });

        return $scope.loading;
    };


    $scope.regionChanged = function (item) {
        var rule = $scope.rule;

        rule.Region = item !== undefined ? item.Region : undefined;
        rule.RegionCode = item !== undefined ? item.RegionCode : undefined;

        rule.RegionRus = false;
    };

    // Форма поиска препарата
    $scope.searchDrug = function (isDistribution) {
        var rule = $scope.rule;

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/CountRuleEditor/_SearchDrugView.html',
            controller: 'SearchDrugController',
            size: 'full',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                model: function () {
                    return {
                        Year: $scope.filter.date.getFullYear(),
                        Month: $scope.filter.date.getMonth() + 1,
                        RegionCode: rule.RegionCode,
                        Region: rule.Region
                    };
                }
            }
        });




        modalInstance.result.then(function (value) {
            if (isDistribution) {
                rule.DistributionClassifierId = value.ClassifierId;;
                rule.DistributionTradeName = value.TradeName;
                rule.TopCountTo = value.topCount;
                if (value.topCount > 0) {
                    $scope.DestinationText = "Топ " + value.topCount;
                } else {
                    $scope.DestinationText = value.DrugDescription;
                }
            } else {
                rule.TradeName = value.TradeName;
                rule.ClassifierId = value.ClassifierId;
                rule.TopCountFrom = value.topCount;
                if (value.topCount > 0) {
                    $scope.SourceText = "Топ " + value.topCount;
                } else {
                    $scope.SourceText = value.DrugDescription;
                }
            }
        }, function () { });
    };


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

            $scope.loading = $http.post('/CountRuleEditor/DeleteRow', JSON.stringify({ id: id }))
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
        if ($scope.filter.dateEnd != null) {
            $scope.rule.YearEnd = $scope.filter.dateEnd.getFullYear();
            $scope.rule.MonthEnd = $scope.filter.dateEnd.getMonth() + 1;
        }
        else {
            $scope.rule.YearEnd = null;
            $scope.rule.MonthEnd = null;
        }

        $scope.loading = $http.post('/CountRuleEditor/SaveRow', JSON.stringify({ model: $scope.rule }))
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
            templateUrl: 'Views/Retail/CountRuleEditor/_TransferRuleView.html',
            controller: 'TransferRuleController',
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
        }, function () { });
    };
}