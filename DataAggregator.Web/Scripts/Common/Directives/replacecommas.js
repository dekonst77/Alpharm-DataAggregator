angular.module('DataAggregatorModule').directive('replacecommas', ['$parse',function ($parse) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, iElement, iAttrs, controller) {
            scope.$watch(iAttrs.ngModel, function (value) {
                if (!value) {
                    return;
                }
                $parse(iAttrs.ngModel).assign(scope, value.replace(",", "."));
            });
        }
    }
}]);
