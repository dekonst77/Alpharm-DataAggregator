angular
    .module('DataAggregatorModule')
    .controller('CountRuleFullVolumeEditorController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', '$translate', 'messageBoxService', CountRuleFullVolumeEditorController]);

function CountRuleFullVolumeEditorController($scope, $http, $uibModal, uiGridCustomService, formatConstants, $translate, messageBoxService) {

    var countRuleFullVolumeEditorGridApi = undefined;

    $scope.editMode = false;

    $scope.rule = {};

    $scope.format = 'MM.yyyy';

    $scope.actualityTypeList =
    [
        { Id: 0, Name: $translate.instant('RETAIL.COUNT_RULE_EDITOR.ACTUALITY_TYPES.ALL') },
        { Id: 1, Name: $translate.instant('RETAIL.COUNT_RULE_EDITOR.ACTUALITY_TYPES.ACTUAL') },
        { Id: 2, Name: $translate.instant('RETAIL.COUNT_RULE_EDITOR.ACTUALITY_TYPES.NOT_ACTUAL') }
    ];

    // Фильтр
    $scope.filter = {
        actualityType: $scope.actualityTypeList[1]
    };

    $scope.countRuleFullVolumeEditorGrid = {
        options: uiGridCustomService.createOptions('CountRuleFullVolumeEditor_Grid')
    };

    var gridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        multiSelect: false
    };

    $scope.countRuleFullVolumeEditorGrid.options.columnDefs = [
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.RULE', field: 'Id', width: 100, type: 'number' },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.USER', field: 'ChangeSurname', width: 150 },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.DATE', field: 'ChangeDate', width: 100, type: 'date', cellFilter: formatConstants.FILTER_DATE, filterCellFiltered: true },
        { displayName: 'PERIOD_START', field: 'StartDate', width: 100, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filterCellFiltered: true },
        { displayName: 'PERIOD_END', field: 'EndDate', width: 100, type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filterCellFiltered: true },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.SOURCE', field: 'FullDrugDescription' },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.DESTINATION', field: 'DistributionFullDrugDescription' },
        { displayName: 'RETAIL.COUNT_RULE_EDITOR.TOP_COUNT', field: 'TopCountTo', width: 100, type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT }
    ];

    angular.extend($scope.countRuleFullVolumeEditorGrid.options, gridOptions);


    $scope.countRuleFullVolumeEditorGrid.options.onRegisterApi = function (gridApi) {

        countRuleFullVolumeEditorGridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.rule = angular.copy(row.entity);
        });

    };


    $scope.clearFields = function () {
        var rule =
        {
            Id : $scope.rule.Id
        };

        $scope.rule = rule;
    };

    $scope.createItem = function() {
        $scope.editMode = true;
        $scope.rule = {};
    };

    $scope.editItem = function () {
        $scope.editMode = true;
    };

    $scope.cancelEditing = function() {
        $scope.editMode = false;

        var rules = getSelectedRows();
        if (rules.length === 1)
            $scope.rule = angular.copy(rules[0]);
        else
            $scope.rule = {};
    };


    // Доступность кнопок
    $scope.canCreate = function() { return !$scope.editMode; };
    $scope.canEdit = function () { return !$scope.editMode && getSelectedRows().length === 1; };
    $scope.canSave = function () { return $scope.editMode && fieldsAreValid(); };
    $scope.canCancel = function () { return $scope.editMode; };
    $scope.canDelete = function () { return !$scope.editMode && getSelectedRows().length === 1; };
    $scope.canClearFields = function() {
        return $scope.editMode &&
            (
                $scope.rule.FullDrugDescription ||
                $scope.rule.DistributionFullDrugDescription ||
                $scope.rule.StartDate ||
                $scope.rule.EndDate ||
                $scope.rule.TopCountTo
            );
    };





    function getSelectedRows() {
        return countRuleFullVolumeEditorGridApi ? countRuleFullVolumeEditorGridApi.selection.getSelectedRows() : [];
    };

    function fieldsAreValid() {
        var rule = $scope.rule;

        if (!rule.FullDrugDescription)
            return false;

        if (!rule.DistributionFullDrugDescription && !$scope.rule.TopCountTo)
            return false;

        if (!rule.StartDate && !rule.EndDate)
            return false;

        if (rule.StartDate && rule.EndDate && rule.StartDate > rule.EndDate)
            return false;

        return true;
    }

    function getPeriod(year, month) {
        if (!year)
            return undefined;

        return new Date(year, month - 1, 1);
    };


    function setPeriodDatesInRow(row) {
        row.StartDate = getPeriod(row.YearStart, row.MonthStart);
        row.EndDate = getPeriod(row.YearEnd, row.MonthEnd);
    }

    function loadRules() {
        var data = {
            actualityType: $scope.filter.actualityType.Id
        };

        $scope.loading = $http.get('/CountRuleFullVolumeEditor/LoadRules', { params: data })
            .then(function(response) {
                $scope.countRuleFullVolumeEditorGrid.options.data = response.data;

                    angular.forEach($scope.countRuleFullVolumeEditorGrid.options.data, function (row) {
                        setPeriodDatesInRow(row);
                    });
                },
                function() {
                    alert('Ошибка');
                });
    }

    $scope.$watch(function () { return $scope.filter; },
        function () {
            loadRules();
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

    // Форма поиска препарата
    $scope.searchDrug = function(isDistribution) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/CountRuleFullVolumeEditor/_SearchDrugView.html',
            controller: 'SearchDrugFullVolumeController',
            size: 'full',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function(value) {
            var rule = $scope.rule;

            if (isDistribution) {
                rule.DistributionDrugId = value.DrugId;
                rule.DistributionOwnerTradeMarkId = value.OwnerTradeMarkId;
                rule.DistributionPackerId = value.PackerId;
                rule.DistributionFullDrugDescription = value.DrugDescription;
                rule.DistributionClassifierId = value.ClassifierId;
            } else {
                rule.DrugId = value.DrugId;
                rule.OwnerTradeMarkId = value.OwnerTradeMarkId;
                rule.PackerId = value.PackerId;
                rule.FullDrugDescription = value.DrugDescription;
                rule.ClassifierId = value.ClassifierId;
            }

        });
    };

    // Удалить правило
    $scope.deleteItem = function () {
        var id = $scope.rule.Id;

        var result = messageBoxService.showConfirm('Удалить правило?');

        result.then(function () {

            $scope.loading = $http.post('/CountRuleFullVolumeEditor/DeleteRow', JSON.stringify({ id: id }))
                .then(function () {
                    var items = $scope.countRuleFullVolumeEditorGrid.options.data;

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


        $scope.loading = $http.post('/CountRuleFullVolumeEditor/SaveRow', JSON.stringify({ model: $scope.rule }))
            .then(function (response) {
                var data = response.data;

                if (data.isError) {
                    alert(data.errorMessage);
                } else {
                    var items = $scope.countRuleFullVolumeEditorGrid.options.data;

                    setPeriodDatesInRow(data);

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
            function() {
                alert('Ошибка');
            });
    };

    $scope.$watch(
        function () { return $scope.rule.StartDate; },
        function (newValue, oldValue) {
            if (newValue === oldValue)
                return;

            var yearMonth = dateToYearMonth(newValue);

            var rule = $scope.rule;

            rule.YearStart = yearMonth.Year;
            rule.MonthStart = yearMonth.Month;
        });

    $scope.$watch(
        function () { return $scope.rule.EndDate; },
        function (newValue, oldValue) {
            if (newValue === oldValue)
                return;

            var yearMonth = dateToYearMonth(newValue);

            var rule = $scope.rule;

            rule.YearEnd = yearMonth.Year;
            rule.MonthEnd = yearMonth.Month;
        });

    function dateToYearMonth(date) {
        if (!date)
            return {};

        return {
            Year: date.getFullYear(),
            Month: date.getMonth() + 1
        };
    }

    $scope.$watch(
        function () { return $scope.rule.DistributionDrugId; },
        function (newValue, oldValue) {
            if (newValue === oldValue)
                return;

            if (!newValue)
                return;

            if ($scope.rule.TopCountTo)
                $scope.rule.TopCountTo = undefined;
        });

    $scope.$watch(
        function () { return $scope.rule.TopCountTo; },
        function (newValue, oldValue) {
            if (newValue === oldValue)
                return;

            if (!newValue)
                return;

            if ($scope.rule.DistributionDrugId) {
                var rule = $scope.rule;

                rule.DistributionDrugId = undefined;
                rule.DistributionOwnerTradeMarkId = undefined;
                rule.DistributionPackerId = undefined;
                rule.DistributionFullDrugDescription = undefined;
            }
        });
}