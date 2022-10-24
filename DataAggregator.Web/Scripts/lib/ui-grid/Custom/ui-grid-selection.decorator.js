angular.module('ui.grid.module').config(['$provide', function ($provide) {

    $provide.decorator('uiGridSelectionDirective', [
        '$delegate',
        function ($delegate) {

            var directive = $delegate[0];

            var originalPreCompile = directive.compile().pre;

            function preCompile(scope, elm, attributes) {
                var options = scope.$eval(attributes.uiGrid);

                if (options.customEnableRowSelection === false)
                    return;

                originalPreCompile.apply(this, arguments);
            }

            directive.compile = function () {
                return {
                    pre: preCompile
                };
            };

            return $delegate;
        }
    ]);
}]);
