angular.module('DataAggregatorModule')
    .directive('dictionaryrequiredid', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attributes, ngModel) {                              
           

            function validate(value) {
               
                var Id = attributes.dictionaryrequiredid;                
                var valid = Id != null && Id > 0;             
                ngModel.$setValidity('dictionaryrequiredid', valid);
                return valid ? value : undefined;
            }           

            scope.$watch(attributes.ngModel,
                function (newValue) {
                    validate(newValue);
                });

        }
    };
});