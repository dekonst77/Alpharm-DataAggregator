angular.module('ui.grid.module').config(['$provide', function ($provide) {

    $provide.decorator('uiGridPaginationDirective', [
        '$delegate',
        function ($delegate) {

            var directive = $delegate[0];

            var originalPreLink = directive.link.pre;

            function preLink(scope, elm, attributes) {
                var options = scope.$eval(attributes.uiGrid);

                if (options.hasOwnProperty('customEnablePagination') && !options.customEnablePagination)
                    return;

                originalPreLink.apply(this, arguments);
            }

            directive.compile = function () {
                return {
                    pre: preLink
                };
            };

            return $delegate;
        }
    ]);
}]);
