angular
    .module('DataAggregatorModule')
    .controller('SystematizationSetPromoController', ['$scope', '$http', '$uibModalInstance', 'promoValue', SystematizationSetPromoController]);

function SystematizationSetPromoController($scope, $http, $modalInstance, promoValue) {
    var promoRegexp = /^\d+\+\d+$/;
    var digitOrPlusRegex = /\d|\+/;
    var digitRegex = /\d/;

    $scope.promoValue = promoValue;
    var oldValue = promoValue;

    $scope.checkValue = function (event) {
        if (event.key !== 'Backspace' &&
            event.key !== 'Delete' &&
            event.key !== 'ArrowLeft' &&
            event.key !== 'ArrowRight' &&
            event.key !== 'Shift' &&
            event.key !== 'Home' &&
            event.key !== 'End' &&
            event.key !== 'Insert' &&
            !(event.ctrlKey && (event.key === 'z' || event.key === 'Z' || event.key === 'я' || event.key === 'Я')) &&
            !(event.ctrlKey && (event.key === 'x' || event.key === 'X' || event.key === 'ч' || event.key === 'Ч')) &&
            !(event.ctrlKey && (event.key === 'c' || event.key === 'C' || event.key === 'с' || event.key === 'С')) &&
            !(event.ctrlKey && (event.key === 'v' || event.key === 'V' || event.key === 'м' || event.key === 'М'))) {

            if (!$scope.promoValue || $scope.promoValue.indexOf('+') < 0) {
                if (!digitOrPlusRegex.test(event.key) && event.key !== 'Enter') {
                    event.preventDefault();
                }
            } else {
                if (!digitRegex.test(event.key) && event.key !== 'Enter') {
                    event.preventDefault();
                }
            }
        }
    }

    $scope.checkPromoValueIsValid = function () {
        if ($scope.promoValue) {

            //запрещены значения с нулями вида 000+000
            var indexOfPlus = $scope.promoValue.indexOf('+');
            if (indexOfPlus > 0) {
                return !($scope.promoValue.substring(0, indexOfPlus) == 0) && //ноль до знака равно
                       !($scope.promoValue.substring(indexOfPlus + 1) == 0) && //ноль после знака равно
                       promoRegexp.test($scope.promoValue); //соответствует виду N+N
            }
        }

        return $scope.promoValue == '' || promoRegexp.test($scope.promoValue);
    }

    $scope.save = function () {
        if (oldValue === $scope.promoValue) {
            $scope.close();
        } else {
            $modalInstance.close({
                promoValue: $scope.promoValue
            });
        }
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };
}
