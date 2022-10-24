angular
    .module('DataAggregatorModule')
    .controller('PurchaseLinkController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', '$uibModal', PurchaseLinkController]);

function PurchaseLinkController(messageBoxService, $scope, $http, uiGridCustomService, $uibModal) {
    $scope.objectType = 'Purchases';
    
    $scope.getLinks = function () {
        getLinks();
    }

    // Загрузка данных
    getLinks();

    function getLinks() {
        $scope.loadingDictionary = $http({
            method: 'POST',
            url: '/PurchaseLink/GetLinks/',
            data: {objectType: $scope.objectType}
        }).then(function (response) {
            if (response.data.Status === 'ok') {
                $scope.purchaseLinksGrid.Options.data = response.data.Data;
                if (response.data.count === 50000) {
                    messageBoxService.showInfo('Показано 50 000 записей! Возможно, это не все данные.');
                }
            } else if (response.data.Status === 'error') {
                messageBoxService.showError(response.data.Data);
            } else {
                messageBoxService.showError('Неизвестный код ответа сервера!');
            }
        }, function () {
            $scope.message = 'Unexpected Error';
            messageBoxService.showError('Не удалось загрузить таблицу со ссылками!');
        });
    }

    function getLawTypes() {
        if ($scope.lawTypesList == null) {
            $scope.loadingDictionary = $http({
                method: 'POST',
                url: '/PurchaseLink/GetLawTypes/',
                data: { objectType: $scope.objectType }
            }).then(function(response) {
                if (response.data.Status === 'ok') {
                    $scope.lawTypesList = response.data.Data;
                } else if (response.data.Status === 'error') {
                    messageBoxService.showError(response.data.Data);
                } else {
                    messageBoxService.showError('Неизвестный код ответа сервера!');
                }
            }, function() {
                $scope.message = 'Unexpected Error';
                messageBoxService.showError('Не удалось загрузить список типов ФЗ!');
            });
        }
    }
    getLawTypes();

    $scope.objectTypeChange = function () {
        $scope.purchaseLinksGrid.Options.columnDefs[3].visible = $scope.objectType === 'Contracts';
        $scope.purchaseLinksGrid.Options.columnDefs[4].visible = $scope.objectType !== 'Contracts';
        $scope.gridApi.grid.refresh();
        getLinks();
    }

    $scope.add = function () {
        var editParameters = {
            "objectType": $scope.objectType,
            "lawTypesList": $scope.lawTypesList
        };

        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/PurchaseLink/_AddView.html',
            controller: 'PurchaseLinkAddController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: {
                    editParameters: editParameters
                }
            }
        });

        dialog.result.then(
            // ok
            function (item) {
                    addLink(item);
            },
            // cancel
            function (reason) {
            }
        );

    };

    function addLink(item) {
        var url;
        var data;
        if ($scope.objectType === 'Contracts') {
            url = '/PurchaseLink/AddContractLink/';
            data = { "purchaseNumber": item.purchaseNumber, "lawTypeId": item.lawTypeId, "reestrNumber": item.reestrNumber }
        } else {
            url = '/PurchaseLink/AddPurchaseLink/';
            data = { "purchaseNumber": item.purchaseNumber, "lawTypeId": item.lawTypeId, "purchaseUrl": item.purchaseUrl }
        }
        $scope.loadingDictionary = $http({
            "method": 'POST',
            "url": url,
            "data": data
        }).then(function(response) {
            if (response.data.Status === 'ok') {
                $scope.purchaseLinksGrid.Options.data.push(response.data.Data);
            } else if (response.data.Status === 'error') {
                messageBoxService.showError(response.data.Data);
            } else {
                messageBoxService.showError('Неизвестный код ответа сервера!');
            }
        }, function() {
            messageBoxService.showError('Не удалось загрузить список типов ФЗ!');
        });
    }
    
    $scope.UploadFromExcel = function (files) {
        var stop = 1;

        if (files && files.length) {
            var formData = new FormData();
            formData.append('objectType', $scope.objectType);

            files.forEach(function (item, i, arr) {
                formData.append('file', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/PurchaseLink/UploadFromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                if (response.data.Status === 'ok') {
                    $scope.getLinks();
                    messageBoxService.showInfo('Файл обработан успешно!', 'Результаты загрузки');
                } else if (response.data.Status === 'error') {
                    messageBoxService.showError(response.data.Data);
                } else {
                    messageBoxService.showError('Неизвестный код ответа сервера!');
                }
            }, function () {
                messageBoxService.showError('Ошибка при загрузке файла!');
            });
        }
    };

    $scope.purchaseLinksGrid = uiGridCustomService.createGridClass($scope, 'PurchaseLink_Grid');
    $scope.purchaseLinksGrid.Options.showGridFooter = true;
    $scope.purchaseLinksGrid.Options.columnDefs =
    [
        { name: 'Id', field: 'Id', type: 'number', filter: { condition: uiGridCustomService.condition } },
        { name: 'Номер закупки', field: 'PurchaseNumber', filter: { condition: uiGridCustomService.conditionSpace } },
        { name: 'Тип ФЗ', field: 'LawTypeName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Номер контракта', field: 'ReestrNumber', filter: { condition: uiGridCustomService.conditionSpace }, visible: false },
        { name: 'Ссылка', field: 'PurchaseUrl', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата добавления', field: 'AddDate', type: 'date', filter: { condition: uiGridCustomService.condition } },
        { name: 'Пользователь', field: 'UserName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Статус', field: 'StatusEnd', filter: { condition: uiGridCustomService.condition } }
     ];     

    $scope.purchaseLinksGrid.Options.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    };
}



