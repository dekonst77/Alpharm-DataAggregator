angular
    .module('DataAggregatorModule')
    .controller('EditKeyWordController', ['messageBoxService', '$scope', '$http', '$uibModalInstance', 'editParameters', EditKeyWordController]);

function EditKeyWordController(messageBoxService, $scope, $http, $modalInstance, editParameters) {

    $scope.header = editParameters.header;
    $scope.selectedKeyWord = editParameters.selectedKeyWord;
    $scope.recheck = editParameters.recheck;

    $scope.getlistTypesList = function () {
        $scope.loadingDictionary = $http({
            method: 'POST',
            url: '/DistributionKeyWords/GetlistTypesList/'
        }).then(function (response) {
            $scope.listTypesList = response.data;
            $scope.selectedKeyWord.ListTypeValue = $scope.selectedKeyWord.ListTypeValue != undefined ? $scope.selectedKeyWord.ListTypeValue : $scope.listTypesList[0];
        }, function () {
            messageBoxService.showError('Не удалось загрузить список типов!');
        });
    }

    $scope.getlistTypesList();

    $scope.save = function () {
        //add
        if ($scope.selectedKeyWord.Id == undefined) {
            $http({
                method: 'POST',
                url: '/DistributionKeyWords/AddKeyWord/',
                data: { keyWord: $scope.selectedKeyWord, recheck: $scope.recheck }
            }).then(function (response) {
                if (response.data.Status === 'ok') {
                    $scope.selectedKeyWord = response.data.Data;
                    $modalInstance.close({
                        keyWord: $scope.selectedKeyWord
                    });
                } else if (response.data.Status === 'error') {
                    messageBoxService.showError(response.data.Data);
                } else {
                    messageBoxService.showError('Неизвестный код ответа сервера!');
                }
            }, function () {
                messageBoxService.showError('Ошибка при сохранении!');
            });
        }
        // edit
        else
        {
            $http({
                method: 'POST',
                url: '/DistributionKeyWords/EditKeyWord/',
                //data: JSON.stringify({ keyWord: $scope.selectedKeyWord })
                data: { keyWord: $scope.selectedKeyWord, recheck: $scope.recheck }
            }).then(function (response) {
                if (response.data.Status === 'ok') {
                    $scope.selectedKeyWord = response.data.Data;
                    $modalInstance.close({
                        keyWord: $scope.selectedKeyWord
                    });
                } else if (response.data.Status === 'error') {
                    messageBoxService.showError(response.data.Data);
                } else {
                    messageBoxService.showError('Неизвестный код ответа сервера!');
                }
            }, function () {
                messageBoxService.showError('Ошибка при сохранении!');
            });
        }
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };
}