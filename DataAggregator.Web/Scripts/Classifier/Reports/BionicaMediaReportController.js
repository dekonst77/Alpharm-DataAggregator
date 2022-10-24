angular
    .module('DataAggregatorModule')
    .controller('BionicaMediaReportController', ['$scope', '$http', 'messageBoxService', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', '$translate', BionicaMediaReportController]);

function BionicaMediaReportController($scope, $http, messageBoxService, uiGridCustomService, uiGridConstants, formatConstants, $translate) {

    $scope.filter = {
        periodDate: null,
        tradeName: {
            selectedItems: [],
            displayValue: '',
            search: searchTradeName
        },
        ownerTradeMark: {
            selectedItems: [],
            displayValue: '',
            search: searchOwnerTradeMark
        },
        packer: {
            selectedItems: [],
            displayValue: '',
            search: searchPacker
        },

        canBeCleared: function() {
            return $scope.filter.tradeName.selectedItems.length > 0 || $scope.filter.ownerTradeMark.selectedItems.length > 0 || $scope.filter.packer.selectedItems.length > 0;
        },

        clear: function() {
            $scope.filter.tradeName.selectedItems = [];
            $scope.filter.tradeName.displayValue = '';
            $scope.filter.ownerTradeMark.selectedItems = [];
            $scope.filter.ownerTradeMark.displayValue = '';
            $scope.filter.packer.selectedItems = [];
            $scope.filter.packer.displayValue = '';
        }
    };

    $scope.getReport = function () {
        $scope.loadingDictionary =
            $http.post("/BionicaMediaReport/GetReport/", {
                year: $scope.filter.periodDate.getFullYear(),
                month: $scope.filter.periodDate.getMonth() + 1,
                tradeNameIds: multiFieldToIdArray($scope.filter.tradeName),
                ownerTradeMarkIds: multiFieldToIdArray($scope.filter.ownerTradeMark),
                packerIds: multiFieldToIdArray($scope.filter.packer)
            })
            .then(function(response) {
                $scope.reportGrid.Options.data = response.data.reportData;
                if (response.data.count === 40000) {
                    messageBoxService.showInfo("Показано 40 000 записей! Возможно, это не все данные.");
                }
            }, function() {
                $scope.message = "Unexpected Error";
                messageBoxService.showError("Не удалось загрузить отчёт!");
            });
    }

    $scope.exportAllToCsv = function () {
        $scope.loadingDictionary = $http({
            method: "POST",
            data: { year: $scope.filter.periodDate.getFullYear(), month: $scope.filter.periodDate.getMonth() + 1 },
            url: "/BionicaMediaReport/ExportAllToCsv/",
            headers: {
                'Content-type': 'application/json'
            },
            responseType: 'arraybuffer'
        }).then(function (response) {
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

            var month = $scope.filter.periodDate.getMonth() + 1;
            month = month < 10 ? '0' + month : month;
            var year = $scope.filter.periodDate.getFullYear();
            var fileName = 'Отчёт Бионика Сервис ' + month + year + '.csv';

            saveAs(blob, fileName);
        }, function () {
            messageBoxService.showError("Не удалось экспортировать в CSV!");
        });
    }

    var booleanCellTemplate = '<div class="text-center">{{COL_FIELD ? "' + $translate.instant('YES') + '" : COL_FIELD != null ? "' + $translate.instant('NO') + '" : ""}}</div>';
    var booleanCondition = uiGridCustomService.booleanCondition;

    function createFilters(condition) {
        return [{ condition: condition, type: uiGridConstants.filter.INPUT }, { selectOptions: [], type: uiGridConstants.filter.SELECT }];
    }

    $scope.reportGrid = uiGridCustomService.createGridClass($scope, 'BionicaMediaReport_Grid');
    $scope.reportGrid.Options.showGridFooter = true;
    $scope.reportGrid.Options.enableSorting = true,
    $scope.reportGrid.Options.columnDefs =
    [
        { width: 100, name: 'DrugId', displayName: "DrugId", field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { width: 100, name: 'TradeNameId', displayName: "TradeNameId", field: 'TradeNameId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
        { width: 100, name: 'Торговое наименование', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'INNGroupId', displayName: "INNGroupId", field: 'INNGroupId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
        { width: 100, name: 'МНН', field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Составной', field: 'IsCompound', filters: createFilters(booleanCondition), cellTemplate: booleanCellTemplate, enableCellEdit: false },
        { width: 100, name: 'DosageGroupId', displayName: "DosageGroupId", field: 'DosageGroupId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
        { width: 100, name: 'Дозировка', field: 'DosageGroup', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'FormProductId', displayName: "FormProductId", field: 'FormProductId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
        { width: 100, name: 'Форма выпуска', field: 'FormProduct', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Фасовка', field: 'ConsumerPackingCount', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'OwnerTradeMarkId', displayName: "OwnerTradeMarkId", field: 'OwnerTradeMarkId', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'Правообладатель', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Страна правообладателя', field: 'OwnerTradeMarkCountry', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'DrugTypeId', displayName: "DrugTypeId", field: 'DrugTypeId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
        { width: 100, name: 'Тип', field: 'DrugType', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'FTGId', displayName: 'FTGId', field: 'FTGId', filters: createFilters(uiGridCustomService.condition), type: 'number', visible: false },
        { width: 100, name: 'ФТГ', headerTooltip: 'ФТГ', field: 'FTG', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'ATCWhoId', displayName: 'ATCWhoId', field: 'ATCWhoId', filters: createFilters(uiGridCustomService.condition), type: 'number', visible: false },
        { width: 100, name: 'ATCWho код', displayName: 'ATCWho код', field: 'ATCWhoCode', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'ATCWho описание', displayName: 'ATCWho описание', field: 'ATCWhoDescription', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'ATCEphmraId', displayName: 'ATCEphmraId', field: 'ATCEphmraId', filters: createFilters(uiGridCustomService.condition), type: 'number', visible: false },
        { width: 100, name: 'ATCEphmra код', displayName: 'ATCEphmra код', field: 'ATCEphmraCode', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'ATCEphmra описание', displayName: 'ATCEphmra описание', field: 'ATCEphmraDescription', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'ATCBaaId', displayName: 'ATCBaaId', field: 'ATCBaaId', filters: createFilters(uiGridCustomService.condition), type: 'number', visible: false },
        { width: 100, name: 'ATCBaa код', displayName: 'ATCBaa код', field: 'ATCBaaCode', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'ATCBaa описание', displayName: 'ATCBaa описание', field: 'ATCBaaDescription', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'NfcId', displayName: 'NfcId', field: 'NfcId', filters: createFilters(uiGridCustomService.condition), type: 'number', visible: false },
        { width: 100, name: 'NFC код', displayName: 'NFC код', field: 'NfcCode', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'NFC описание', displayName: 'NFC описание', field: 'NfcDescription', filters: createFilters(uiGridCustomService.condition) },
        { width: 100, name: 'OTC', displayName: 'OTC', field: 'IsOtc', filters: createFilters(booleanCondition), cellTemplate: booleanCellTemplate, enableCellEdit: false },
        { width: 100, name: 'RX', displayName: 'RX', field: 'IsRx', filters: createFilters(booleanCondition), cellTemplate: booleanCellTemplate, enableCellEdit: false },
        { width: 100, name: 'PackerId', displayName: 'PackerId', field: 'PackerId', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'Упаковщик', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Страна упаковщика', field: 'PackerCountry', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Описание', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'BrandId', displayName: 'BrandId', field: 'BrandId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
        { width: 100, name: 'Бренд', field: 'Brand', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'CorporationId', displayName: 'CorporationId', field: 'CorporationId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
        { width: 100, name: 'Корпорация', field: 'Corporation', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'OwnerRegistrationCertificateId', displayName: 'OwnerRegistrationCertificateId', field: 'OwnerRegistrationCertificateId', filter: { condition: uiGridCustomService.condition }, type: 'number', visible: false },
        { width: 100, name: 'Код владельца РУ', field: 'OwnerRegistrationCertificateId', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Владелец РУ', field: 'OwnerRegistrationCertificate', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Страна владельца РУ', field: 'OwnerRegistrationCertificateCountry', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'РУ', field: 'RegistrationCertificateNumber', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Дата регистрации РУ', field: 'RegistrationDate', filter: { condition: uiGridCustomService.condition }, cellFilter: formatConstants.FILTER_DATE, type: 'date' },
        { width: 100, name: 'Дата регистрации РУ', field: 'RegistrationDate', filter: { condition: uiGridCustomService.condition }, cellFilter: formatConstants.FILTER_DATE, type: 'date' },
        { width: 100, name: 'Дата перерег. РУ', field: 'ReissueDate', filter: { condition: uiGridCustomService.condition }, cellFilter: formatConstants.FILTER_DATE, type: 'date' },
        { width: 100, name: 'Дата окончания РУ', field: 'ExpDate', filter: { condition: uiGridCustomService.condition }, cellFilter: formatConstants.FILTER_DATE, type: 'date' },
        { width: 100, name: 'Срок введ. в ГО', field: 'CirculationPeriod', filter: { condition: uiGridCustomService.condition } },
        { width: 100, name: 'Не заблокировано', field: 'Used', filters: createFilters(booleanCondition), cellTemplate: booleanCellTemplate, enableCellEdit: false },
        { width: 100, name: '2015', field: 'VED2015', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: '2016', field: 'VED2016', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: '2017', field: 'VED2017', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: '2018', field: 'VED2018', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'ФЛ 2014', field: 'FB2014', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'ФЛ 2015', field: 'FB2015', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'ФЛ 2016', field: 'FB2016', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'ФЛ 2017', field: 'FB2017', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'ФЛ 2018', field: 'FB2018', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { width: 100, name: 'Мин. ассорт.', field: 'MinimumAssortment', filters: createFilters(booleanCondition), cellTemplate: booleanCellTemplate, enableCellEdit: false },
        { width: 100, name: 'Взаимозаменяемый', field: 'Exchangeable', filters: createFilters(booleanCondition), cellTemplate: booleanCellTemplate, enableCellEdit: false },
        { width: 100, name: 'Референтный', field: 'Reference', filters: createFilters(booleanCondition), cellTemplate: booleanCellTemplate, enableCellEdit: false },
        { width: 100, name: 'Ср. цена продажи', field: 'SellingPrice', filter: { condition: uiGridCustomService.numberCondition }, type: 'number', cellFilter: formatConstants.FILTER_PRICE }
    ];

    function searchTradeName(value) {
        var httpPromise = $http.post('/BionicaMediaReport/SearchTradeName/', JSON.stringify({ Value: value }))
            .then(function (response) {
                return prepareDictionary(response.data);
            });

        return httpPromise;
    };

    function searchOwnerTradeMark(value) {
        var httpPromise = $http.post('/BionicaMediaReport/SearchManufacturer/', JSON.stringify({ Value: value }))
            .then(function (response) {
                return prepareDictionary(response.data);
            });

        return httpPromise;
    };

    function searchPacker(value) {
        var httpPromise = $http.post('/BionicaMediaReport/SearchManufacturer/', JSON.stringify({ Value: value }))
            .then(function (response) {
                return prepareDictionary(response.data);
            });

        return httpPromise;
    };

    function prepareDictionary(dictionary) {
        angular.forEach(dictionary, function (item) {
            item.displayValue = item.Value;
        });

        return dictionary;
    }

    function multiFieldToIdArray(field) {
        return field.selectedItems.map(function (r) { return r.Id; });
    }
}