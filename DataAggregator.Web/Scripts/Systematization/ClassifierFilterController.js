angular
    .module('DataAggregatorModule')
    .controller('ClassifierFilterController', ['$scope', '$http', '$uibModalInstance', 'hotkeys', 'classifierFilter', ClassifierFilterController]);

function ClassifierFilterController($scope, $http, $modalInstance, hotkeys, classifierFilter) {

    var pressedEnter = 0;

    $scope.noResults = false;

    //Поиск по enter
    //Срабатывает, если нету результатов поиска в справочнике noResults = true
    //Или если результаты были, и мы их выбрали то счетчик увеличиться на 1 в setId и на 1 в pressEnter и станет равным 2 и при следующем enter сработает pressedEnter > 1
    //Если результат был выбран, но изменяется текст то счетчик сброситься в 0 в $scope.searchDictionary
    $scope.pressTypeaHeadEnter = function () {
        if ($scope.noResults || pressedEnter > 1) {
            if ($scope.CanSearch())
                $scope.ok();

            //Обнуляем счетчик
            pressedEnter = 0;
        }
        //Счетчик станет 2 - и для следуюшего enter сработает pressedEnter > 1
        pressedEnter++;
    };

    $scope.pressEnter = function () {
        if ($scope.CanSearch())
            $scope.ok();
    };

    $scope.classifierFilter = classifierFilter;

    $scope.CanSearch = function () {
        for (var i in $scope.classifierFilter) {
            if ($scope.classifierFilter[i]) {
                return true;
            }
        }
        return false;
    };

    $scope.searchDictionary = function (value, dictionary) {
        //Обнуляем счетчик так меняется выпадающий список
        pressedEnter = 0;
        $scope.classifierFilter[dictionary + 'Id'] = null;
        return $http({
            method: 'POST',
            url: '/Dictionary/GetDictionary/',
            data: JSON.stringify({ Value: value, Dictionary: dictionary })
        }).then(function (response) {
            return response.data;
        });
    };


    $scope.changeCheck = function (dictionary) {
        if ($scope.classifierFilter[dictionary] != null && $scope.classifierFilter[dictionary].length === 0) {
            $scope.classifierFilter[dictionary] = null;
            $scope.classifierFilter[dictionary + 'Id'] = null;
        }
    };

    $scope.setId = function (item, field) {
        $scope.classifierFilter[field] = item.Id;

        //Счетчик станет 1
        pressedEnter++;
    };

    $scope.ok = function () {
        $modalInstance.close($scope.classifierFilter);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss($scope.classifierFilter);
    };

    $scope.clear = function () {
        $scope.classifierFilter = new Object();
    };
}