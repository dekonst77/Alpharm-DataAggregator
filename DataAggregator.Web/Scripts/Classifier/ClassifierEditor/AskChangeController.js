angular
    .module('DataAggregatorModule')
    .controller('AskChangeController', ['$scope', '$uibModalInstance', 'messageBoxService', 'errorHandlerService', mode, classifier, AskChangeController]);

function AskChangeController($scope, $modalInstance, messageBoxService, errorHandlerService, mode, classifier) {

    $scope.model = {
        OnlyCertBlock: false,
        ChangeDrugIdCheck: false,
        NeedMerge: false,
        CanMerge: false,
        SaveClassifierIdCheck: false,
        DrugDescription: null,
        ErrorMessage: null,
    }

    $scope.Result = {
        Change: false,
        Recreate: false,
        SaveClassifierId: false,
        Merge: false,
        ChangeCert:false,
        //Обнуляем результат
        Clear: function () {
            this.Change = false;
            this.Recreate = false;
            this.SaveClassifierId = false;
            this.Merge = false;
            this.ChangeCert = false;
        },
    }

    switch (mode) {
        //Изменяем только блокировку сертификата регистрации
        case 1:
            $scope.model.OnlyCertBlock = true;
            $scope.Result.ChangeCert = true;
            break;
        case 2:
            $scope.model.OnlyCertBlock = false;
            $scope.Result.Change = true;
            break;
    }

    //Проверяем если отмечено сохранить с новым ClassifierId
    $scope.ChangeDrugId = function () {

        //Если галочка снята
        if (!$scope.model.ChangeDrugId) {
            $scope.Result.Clear();
            $scope.Result.Change = true;
            return;
        }

        //Если галочку поставили, то осуществим проверку
        var json = JSON.stringify({ model: classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/ClassifierEditor/CheckRecreate/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {

                    var data = responseData.Data;

                    if (!data.CanRecreate) {
                        //'Нельзя объединять в пределах одного DrugId'
                        $scope.model.NeedMerge = true;
                        $scope.model.CanMerge = false;
                        $scope.Result.Clear();
                        return;
                    }

                    if (data.NeedMerge) {
                        $scope.model.NeedMerge = true;
                        $scope.model.DrugDescription = data.DrugDescription;
                        $scope.Result.Clear();
                    };

                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }, function (response) {
                    errorHandlerService.showResponseError(response);
            });
    }

    //Выбрали объединение
    $scope.ChangeMerge = function () {
        $scope.Result.Clear();
        $scope.Result.Merge = true;
    }

    $scope.ChangeSaveClassifierId = function () {
        if (!$scope.Result.Change) {
            $scope.model.ErrorMessage = 'Ошибка, не выбрано сохранить с новым DrugId';
        }

        $scope.Result.Clear();
        $scope.Result.SaveClassifierId = $scope.model.SaveClassifierId;
    }

    $scope.CanSave = function() {
        return false;
    }

    $scope.Save = function () {
        $modalInstance.close($scope.info);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}