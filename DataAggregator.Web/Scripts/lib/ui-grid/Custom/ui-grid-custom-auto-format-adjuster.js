angular.module('ui.grid')
    .directive('uiGridCustomAutoFormatAdjuster', [function () {
        return {
            replace: true,
            require: '^uiGrid',
            controller: function () { },
            compile: function () {
                return {
                    pre: function () { },
                    post: function (scope, $elm, attrs) {

                        var options = scope.$eval(attrs.uiGrid);

                        prepareColumnDefs(options.columnDefs);

                        function prepareColumnDefs(columnDefs) {

                            for (var i = 0; i < columnDefs.length; i++) {
                                var columnDef = columnDefs[i];

                                columnDef.headerCellFilter = 'translate';
                                if (columnDef.type === 'date') {
                                    if (columnDef.cellClass === undefined)
                                        columnDef.cellClass = 'text-center';
                                }

                                if (columnDef.type === 'number') {
                                    if (columnDef.cellClass === undefined)
                                        columnDef.cellClass = 'text-right';
                                }
                            }
                        }
                    }
                };
            }
        };
    }]);