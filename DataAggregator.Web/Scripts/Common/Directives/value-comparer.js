angular.module('DataAggregatorModule').directive('valueComparer',
    function() {
        return {
            require: 'ngModel',
            link: function (scope, element, attributes, ctrl) {
                if (!attributes.valueComparerType)
                    throw new Error('Comparer type is not configured');

                function validate(value1, value2) {
                    if (value1 === undefined || value2 === undefined)
                        return;

                    var isValid;
                    switch (attributes.valueComparerType) {
                        case '<':
                            isValid = value1 < value2;
                            break;
                        case '<=':
                            isValid = value1 <= value2;
                            break;
                        case '>':
                            isValid = value1 > value2;
                            break;
                        case '>=':
                            isValid = value1 >= value2;
                            break;
                        case '=':
                        case '==':
                            isValid = value1 === value2;
                            break;
                        default:
                            throw new Error('Comparer type \'' + attributes.valueComparerType + '\' is not supported');
                    }

                    ctrl.$setValidity('valueComparer', isValid);
                }

                scope.$watch(attributes.valueComparer,
                    function (newValue) {
                        validate(scope.$eval(attributes.ngModel), newValue);
                    });

                scope.$watch(attributes.ngModel,
                    function (newValue) {
                        validate(newValue, scope.$eval(attributes.valueComparer));
                    });
            }
        };
    });