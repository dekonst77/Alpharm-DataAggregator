angular.module('ui.grid.module').config(['$provide', function ($provide) {

    $provide.decorator('uiGridCellnavDirective', [
        '$delegate',
        function ($delegate) {

            var directive = $delegate[0];

            var originalCompile = directive.compile();

            function customEnableCellSelection(scope, attributes) {
                var options = scope.$eval(attributes.uiGrid);

                return !(options.customEnableCellSelection === false);
            }

            function preCompile(scope, elm, attributes) {

                if (!customEnableCellSelection(scope, attributes))
                    return;

                originalCompile.pre.apply(this, arguments);
            }

            function postCompile(scope, elm, attributes) {

                if (!customEnableCellSelection(scope, attributes))
                    return;

                originalCompile.post.apply(this, arguments);
            }

            directive.compile = function () {
                return {
                    pre: preCompile,
                    post: postCompile
                }
            };

            return $delegate;
        }
    ]);
}]);
