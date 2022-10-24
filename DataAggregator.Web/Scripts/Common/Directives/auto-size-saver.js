angular
    .module('DataAggregatorModule').directive('autoSizeSaver',
        function () {
            return {
                restrict: 'AE',
                link: function (scope, element, attrs) {
                    var size = scope.$eval(attrs.autoSizeSaver);

                    if (size === undefined) {
                        size = {};
                        scope[attrs.autoSizeSaver] = size;
                    }

                    element.bind('resize', function () {
                        scope.$apply();
                    });

                    var getElementDimensions = function () {
                        return {
                            height: element.height(),
                            width: element.width()
                        };
                    };

                    scope.$watch(getElementDimensions, function (newValue) {
                        size.height = newValue.height + 'px';
                        size.width = newValue.width + 'px';
                    }, true);
                }
            }
        });