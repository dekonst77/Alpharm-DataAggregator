angular.module('ui.grid').directive('uiGridControlTree',
    [
        function () {
            return {
                scope: {
                    options: '='
                },

                templateUrl: 'Views/Static/ui-grid/ui-grid-control-tree.html'
            };
        }
    ]);