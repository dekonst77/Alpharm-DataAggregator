angular
    .module('DataAggregatorModule')
    .controller('ClientKeywordsEditController', ['messageBoxService', '$scope', '$http', '$uibModalInstance', 'editParameters', ClientKeywordsEditController]);

function ClientKeywordsEditController(messageBoxService, $scope, $http, $modalInstance, editParameters) {

    $scope.header = editParameters.header;
    $scope.fieldName = editParameters.fieldName;
    $scope.fieldValue = editParameters.fieldValue;

    $scope.loadKeywordsList = function () {
        if ($scope.header === 'Добавление ключевого слова' || $scope.header === 'Редактирование ключевого слова') {
            $scope.loadingDictionary = $http({
                method: 'POST',
                url: '/ClientKeywords/LoadKeywords/'
            }).then(function(response) {
                $scope.keyWordsList = response.data;
            }, function() {
                messageBoxService.showError('Не удалось загрузить список ключевых слов!');
            });
        } else {
            $scope.keyWordsList = null;
        }
    };

    $scope.loadKeywordsList();

    $scope.save = function () {
        switch ($scope.header) {
            case 'Добавление клиента':
                if (editParameters.fieldValue !== '') {
                    $http({
                        method: 'POST',
                        url: '/ClientKeywords/ClientAdd/',
                        data: { name: $scope.fieldValue }
                    }).then(function(response) {
                        if (response.data.Status === 'ok') {
                            $modalInstance.close({
                                newClientName: $scope.fieldValue
                            });
                        } else if (response.data.Status === 'error') {
                            messageBoxService.showError(response.data.Data);
                        } else {
                            messageBoxService.showError('Неизвестный код ответа сервера!');
                        }
                    }, function() {
                        messageBoxService.showError('Ошибка при сохранении!');
                    });
                } else {
                    messageBoxService.showError('Наименование не может быть пустым!');
                }
                break;
            case 'Редактирование клиента':
                if (editParameters.fieldValue !== '') {
                    $http({
                        method: 'POST',
                        url: '/ClientKeywords/ClientEdit/',
                        data: { name: $scope.fieldValue, clientId: editParameters.clientId }
                    }).then(function (response) {
                        if (response.data.Status === 'ok') {
                            $modalInstance.close({
                                newClientName: $scope.fieldValue
                            });
                        } else if (response.data.Status === 'error') {
                            messageBoxService.showError(response.data.Data);
                        } else {
                            messageBoxService.showError('Неизвестный код ответа сервера!');
                        }
                    }, function () {
                        messageBoxService.showError('Ошибка при сохранении!');
                    });
                } else {
                    messageBoxService.showError('Наименование не может быть пустым!');
                }
                break;
            case 'Добавление ключевого слова':
                if (editParameters.fieldValue !== '') {
                    $http({
                        method: 'POST',
                        url: '/ClientKeywords/KeywordAdd/',
                        data: { keywordText: $scope.fieldValue, clientId: editParameters.clientId }
                    }).then(function (response) {
                        if (response.data.Status === 'ok') {
                            $modalInstance.close({
                                keyWord: response.data.Data
                            });
                        } else if (response.data.Status === 'error') {
                            messageBoxService.showError(response.data.Data);
                        } else {
                            messageBoxService.showError('Неизвестный код ответа сервера!');
                        }
                    }, function () {
                        messageBoxService.showError('Ошибка при сохранении!');
                    });
                } else {
                    messageBoxService.showError('Наименование не может быть пустым!');
                }
                break;
        }
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };
}