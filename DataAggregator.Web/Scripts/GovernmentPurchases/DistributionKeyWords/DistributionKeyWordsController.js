angular
    .module('DataAggregatorModule')
    .controller('DistributionKeyWordsController', ['messageBoxService', '$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', DistributionKeyWordsController]);

function DistributionKeyWordsController(messageBoxService, $scope, $http, $uibModal, uiGridCustomService, formatConstants) {
    $scope.onlyActive = true;
    $scope.recheck = false;
    $scope.deleteBtnDisabled = true;

    //Методы
    $scope.getKeyWordsList = function () {
        $scope.loadingDictionary = $http({
            method: 'POST',
            data: { onlyActive: $scope.onlyActive },
            url: '/DistributionKeyWords/GetKeyWordsList/'
        }).then(function (response) {
            $scope.listGrid.Options.data = response.data;

            if (response.data.count === 50000) {
                messageBoxService.showInfo('Показано 50 000 записей! Возможно, это не все данные.');
            }
        }, function () {
            messageBoxService.showError('Не удалось загрузить отчёт!');
        });
    }

    $scope.getKeyWordsList();

    $scope.add = function () {
        var editParameters = {
            "header": 'Добавление ключевого слова',
            "selectedKeyWord": {},
            "recheck": $scope.recheck
        }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/DistributionKeyWords/_EditView.html',
            controller: 'EditKeyWordController',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                editParameters: function () {
                    return editParameters;
                }
            }
        });

        modalInstance.result.then(function (data) {
            refreshGrid(data.keyWord);
        }, function () {
        });
    }

    $scope.edit = function () {
        if ($scope.selectedKeyWord == undefined || $scope.selectedKeyWord.length !== 1) {
            return;
        }

        var editParameters = {
            "header": 'Редактирование ключевого слова',
            "selectedKeyWord": JSON.parse(JSON.stringify($scope.selectedKeyWord[0])),
            "recheck": $scope.recheck
        }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/DistributionKeyWords/_EditView.html',
            controller: 'EditKeyWordController',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                editParameters: function () {
                    return editParameters;
                }
            }
        });

        modalInstance.result.then(function (data) {
            refreshGrid(data.keyWord);
        }, function () {
        });
    }

    $scope.delete = function () {
        if ($scope.selectedKeyWord == undefined || $scope.selectedKeyWord.length === 0) {
            return;
        }

        var swToServer = [];
        angular.forEach($scope.selectedKeyWord, function(kw) {
            if (kw.DateEnd === null) {
                swToServer.push(kw);
            }
        });

        if (swToServer.length > 0) {
            messageBoxService.showConfirm('Аннулировать выделенные строки?')
                .then(
                    function() { //yes
                        $http({
                            method: 'POST',
                            url: '/DistributionKeyWords/DeleteKeyWords/',
                            data: JSON.stringify({ keyWordsList: swToServer })
                        }).then(function(response) {
                            if (response.data.Status === 'ok') {
                                refreshGrid(response.data.Data);
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
    
    //с сервера приходят только либо новые записи, либо аннулированные
    function refreshGrid(keyValuesFromServer) {
        var index;
        
        angular.forEach(keyValuesFromServer, function (kw) {
            index = $scope.listGrid.Options.data.findIndex(l => l.Id === kw.Id);
            if (index > -1) {
                if ($scope.onlyActive) {
                    $scope.listGrid.Options.data.splice(index, 1);
                } else {
                    $scope.listGrid.Options.data[index].DateEnd = kw.DateEnd;
                    $scope.listGrid.Options.data[index].EndUser = kw.EndUser;
                }
            } else {
                $scope.listGrid.Options.data.push(kw);
            }
        });

        setDeleteBtnDisabled();
    }

    $scope.listGrid = uiGridCustomService.createGridClass($scope, 'DistributionKeyWords_Grid');
    $scope.listGrid.Options.showGridFooter = true;
    $scope.listGrid.Options.columnDefs =
    [
        { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition }, type: 'number' },
        { name: 'Текст', field: 'Value', filter: { condition: uiGridCustomService.condition } },
        { name: 'Тип списка', field: 'ListTypeValue', filter: { condition: uiGridCustomService.condition } },
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
    }
}