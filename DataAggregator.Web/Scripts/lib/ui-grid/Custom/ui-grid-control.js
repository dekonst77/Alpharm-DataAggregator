angular.module('ui.grid').directive('uiGridControl',
    [
        function() {
            return {
                scope: {
                    options: '='
                },

                templateUrl: 'Views/Static/ui-grid/ui-grid-control.html'
            };
        }
    ]);