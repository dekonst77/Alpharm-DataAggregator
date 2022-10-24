angular
    .module('DataAggregatorModule')
    .controller('ClientKeywordsController', ['messageBoxService', '$scope', '$http', '$uibModal', 'uiGridCustomService', ClientKeywordsController]);

function ClientKeywordsController(messageBoxService, $scope, $http, $uibModal, uiGridCustomService) {
    $scope.onlyActive = true;
    $scope.deleteBtnDisabled = true;

    //Методы
    $scope.getClientsList = function () {
        $scope.loadingDictionary = $http({
            method: 'POST',
            url: '/ClientKeywords/GetClientsList/'
        }).then(function (response) {
            $scope.clientsList = response.data;
            $scope.clientsList.selected = $scope.clientsList.find(c => c.Name === $scope.newClientName);
        }, function () {
            messageBoxService.showError('Не удалось загрузить список клиентов!');
        });
    }

    $scope.getClientsList();

    $scope.getClientKeywords = function () {
        $scope.loadingDictionary = $http({
            method: 'POST',
            data: { clientId: $scope.clientsList.selected.Id, onlyActive: $scope.onlyActive },
            url: '/ClientKeywords/GetClientKeywords/'
        }).then(function (response) {
            $scope.listGrid.Options.data = response.data;
        }, function () {
            messageBoxService.showError('Не удалось загрузить список клиентов!');
        });
    }

    $scope.clientAdd = function () {
        var editParameters = {
            "header": 'Добавление клиента',
            "fieldName": 'Наименование',
            "fieldValue": null
        }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/ClientKeywords/_EditView.html',
            controller: 'ClientKeywordsEditController',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                editParameters: function () {
                    return editParameters;
                }
            }
        });

        modalInstance.result.then(function (data) {
            $scope.newClientName = data.newClientName;
            $scope.getClientsList();
        }, function () {
        });
    }

    $scope.clientEdit = function () {
        var editParameters = {
            "header": 'Редактирование клиента',
            "fieldName": 'Наименование',
            "fieldValue": $scope.clientsList.selected.Name,
            "clientId": $scope.clientsList.selected.Id
        }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/ClientKeywords/_EditView.html',
            controller: 'ClientKeywordsEditController',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                editParameters: function () {
                    return editParameters;
                }
            }
        });

        modalInstance.result.then(function (data) {
            $scope.newClientName = data.newClientName;
            $scope.getClientsList();
        }, function () {
        });
    }

    $scope.clientDelete = function () {
        if ($scope.clientsList.selected == undefined) {
            return;
        } else {
            messageBoxService.showConfirm('Удалить клиента?')
                .then(
                    function() { //yes
                        $http({
                            method: 'POST',
                            url: '/ClientKeywords/ClientDelete/',
                            data: JSON.stringify({ clientId: $scope.clientsList.selected.Id })
                        }).then(function(response) {
                            if (response.data.Status === 'ok') {
                                $scope.newClientName = null;
                                $scope.getClientsList();
                            } else if (response.data.Status === 'error') {
                                messageBoxService.showError(response.data.Data);
                            } else {
                                messageBoxService.showError('Неизвестный код ответа сервера!');
                            }
                        }, function() {
                            messageBoxService.showError('Ошибка при удалении!');
                        });
                    },
                    function() { //no
                    })
                .finally(function() {
                });
        }
    }
    
    $scope.keywordAdd = function () {
        var editParameters = {
            "header": 'Добавление ключевого слова',
            "fieldName": 'Текст',
            "fieldValue": null,
            "clientId": $scope.clientsList.selected.Id
        }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/ClientKeywords/_EditView.html',
            controller: 'ClientKeywordsEditController',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                editParameters: function () {
                    return editParameters;
                }
            }
        });

        modalInstance.result.then(function (data) {
            $scope.listGrid.Options.data.push(data.keyWord);
        }, function () {
        });
    }

    $scope.keywordDelete = function () {
        if ($scope.selectedKeyWord == undefined || $scope.selectedKeyWord.length === 0) {
            return;
        }

        var swToServer = [];
        angular.forEach($scope.selectedKeyWord, function(kw) {
            if (kw.DateEnd === null) {
                swToServer.push(kw.Id);
            }
        });

        if (swToServer.length > 0) {
            messageBoxService.showConfirm('Аннулировать выделенные строки?')
                .then(
                    function() { //yes
                        $http({
                            method: 'POST',
                            url: '/ClientKeywords/KeywordsDelete/',
                            data: JSON.stringify({ keyWordsIdList: swToServer })
                        }).then(function(response) {
                            if (response.data.Status === 'ok') {
                                var index;
                                angular.forEach(response.data.Data, function (kw) {
                                    index = $scope.listGrid.Options.data.findIndex(l => l.Id === kw.Id);
                                    if ($scope.onlyActive) {
                                        $scope.listGrid.Options.data.splice(index, 1);
                                    } else {
                                        $scope.listGrid.Options.data[index].DateEnd = kw.DateEnd;
                                        $scope.listGrid.Options.data[index].EndUser = kw.EndUser;
                                    }
                                });
                            } else if (response.data.Status === 'error') {
                                messageBoxService.showError(response.data.Data);
                            } else {
                                messageBoxService.showError('Неизвестный код ответа сервера!');
                            }
                        }, function() {
                            messageBoxService.showError('Ошибка при удалении!');
                        });
                    },
                    function() { //no
                    })
                .finally(function() {
                });
        }
    }

    function setDeleteBtnDisabled() {
        if ($scope.selectedKeyWord == undefined || $scope.selectedKeyWord.length === 0) {
            $scope.deleteBtnDisabled = true;
            return;
        }

        for(var i = 0; i < $scope.selectedKeyWord.length; i++) {
            if ($scope.selectedKeyWord[i].DateEnd === null) {
                $scope.deleteBtnDisabled = false;
                return;
            }
        }

        $scope.deleteBtnDisabled = true;
    }
    
    $scope.listGrid = uiGridCustomService.createGridClass($scope, 'ClientKeywords_Grid');
    $scope.listGrid.Options.showGridFooter = true;
    $scope.listGrid.Options.enableSorting = true,
    $scope.listGrid.Options.columnDefs =
    [
        { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { name: 'Текст', field: 'KeywordText', filter: { condition: uiGridCustomService.condition } },
        { name: 'Создал', field: 'StartUser', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата создания', field: 'DateStart', type: 'date' },
        { name: 'Закрыл', field: 'EndUser', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата закрытия', field: 'DateEnd', type: 'date' }
    ];
    $scope.listGrid.Options.onRegisterApi = function(gridApi) {
        //set gridApi on scope
        $scope.listGridApi = gridApi;

        //Что-то выделили
        $scope.listGridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedKeyWord = $scope.listGridApi.selection.getSelectedRows().map(function (value) {
                return value;
            });
            setDeleteBtnDisabled();
        });

        //Что-то выделили
        $scope.listGridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.selectedKeyWord = $scope.listGridApi.selection.getSelectedRows().map(function (value) {
                return value;
            });
            setDeleteBtnDisabled();
        });
    }

    $(window).keydown(function (event) {
        if (event.keyCode === 16) {
            SetIsShift(true);
        }
    });

    $(window).keyup(function (event) {
        if (event.keyCode === 16) {
            SetIsShift(false);
        }
    });

    function SetIsShift(value) {
        if (value)
            $('#list-grid-block').addClass('disableSelection');
        else {
            $('#list-grid-block').removeClass('disableSelection');
        }
    };
}