angular
    .module('DataAggregatorModule')
    .controller('GoodsTransferRuleController', ['$scope', '$http', '$uibModalInstance', 'model', '$translate', GoodsTransferRuleController]);

function GoodsTransferRuleController($scope, $http, $modalInstance, model, $translate) {

    var isChanged = false;
    $scope.regionInfo = undefined;

    var translatedRegionNames = {
        russia: $translate.instant('COUNTRIES.RUSSIA'),
        moscow: $translate.instant('CITIES.MOSCOW'),
        saintPetersburg: $translate.instant('CITIES.SAINT_PETERSBURG')
    };

    // Первоначальная загрузка формы при редактировании
    (function() {
        if (model.RegionRus) {
            $scope.regionInfo = translatedRegionNames.russia;
            return;
        }

        if (model.RegionMsk) {
            $scope.regionInfo = translatedRegionNames.moscow;
            return;
        }

        if (model.RegionSpb) {
            $scope.regionInfo = translatedRegionNames.saintPetersburg;
            return;
        }

        if (model.RegionCode) {
            $scope.regionInfo = model.Region;
            return;
        }

        $scope.regionInfo = '-';
    })();

    // Модель формы
    $scope.model = {
        rule: model,
        CurrentRegion: null, // bind input
        RegionForCopy: null, // data for save rule
        NewRules: [] // rules, created on this form
    };

    // Поиск региона
    $scope.searchRegion = function(value) {
        return $http.post('/Region/SearchRegion/', JSON.stringify({ Value: value }))
            .then(function(response) {
                return response.data;
            });
    };

    $scope.setRegion = function(value) {
        $scope.model.RegionForCopy = value;
    };

    $scope.canCopyRule = function() {
        return $scope.model.RegionForCopy !== null;
    };

    $scope.copyRule = function() {
        var ruleForSave = JSON.parse(JSON.stringify($scope.model.rule));
        ruleForSave.Id = null;
        ruleForSave.RegionSpb = null;
        ruleForSave.RegionMsk = null;
        ruleForSave.RegionRus = null;
        ruleForSave.RegionCode = $scope.model.RegionForCopy.RegionCode;
        ruleForSave.Region = $scope.model.RegionForCopy.Region;

        $scope.loading = $http.post('/GoodsCountRuleEditor/SaveRow', JSON.stringify({ model: ruleForSave }))
            .then(function(response) {
                    if (response.data.isError)
                        alert(response.data.errorMessage);
                    else
                        $scope.model.NewRules.push(response.data);

                    isChanged = true;
                },
                function() {
                    alert('Ошибка');
                });
    };

    // Закрытие формы
    $scope.Close = function () {
        if (isChanged)
            $modalInstance.close();
        else
            $modalInstance.dismiss();
    };

}