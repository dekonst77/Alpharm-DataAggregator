angular.module('DataAggregatorModule').directive('valueIds',
    function() {
        return {
            require: 'ngModel',
            link: function (scope, element, attributes, ctrl) {

                function checkValidation(value) {
                    if(value === undefined || value === null)
                        return true;

                    value = value.trim();

                    if (value === '')
                        return true;

                    var arr = value.split(',');

                    for(var i=0;i<arr.length;i++)
                        if (arr[i] === '' || !Number.isInteger(+arr[i]))
                            return false;

                    return true;
                }

                function validate(value) {
                    ctrl.$setValidity('valueIds', checkValidation(value));
                }

                scope.$watch(attributes.ngModel, 
                    function (newValue, oldValue) {
                        if (newValue === oldValue)
                            return;

                        validate(newValue);
                    });
            }
        };
    });