angular.module('DataAggregatorModule').directive('ngShiftF',
    function() {
        return function(scope, element, attrs) {
            element.bind('keydown keypress',
                function(event) {
                    if (event.shiftKey && event.which === 70) {
                        scope.$apply(function() {
                            scope.$eval(attrs.ngEnter, { 'event': event });
                        });

                        event.preventDefault();
                    }
                });
        };
    });