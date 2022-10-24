angular.module('ui.grid')
    .directive('uiGridCustomExportFormatAdjuster', ['$filter', function ($filter) {
        return {
            replace: true,
            require: '^uiGrid',
            controller: function () { },
            compile: function () {
                return {
                    pre: function () { },
                    post: function (scope, $elm, attrs) {

                        var options = scope.$eval(attrs.uiGrid);

                        var formatters = {};

                        options.exporterFieldCallback = function (grid, row, gridCol, cellValue) {
                            var context = { col: gridCol };
                            if (gridCol.cellFilter) { // check if any filter is applied on the column

                                var filters = parseFilter(gridCol.cellFilter);

                                angular.forEach(filters,
                                    function (filter) {
                                        var filterParams = filter.params.slice();

                                        if (filter.name !== 'number') {
                                            if (filter.name !== 'date' && filter.name !== 'number') {
                                                filterParams.unshift(context);
                                            }
                                            filterParams.unshift(cellValue); // insert the input element to the filter parameters list
                                            var filterFn = $filter(filter.name); // filter function

                                            // call the filter, with multiple filter parameters. 
                                            //'Apply' will call the function and pass the array elements as individual parameters to that function.
                                             try {
                                            cellValue = filterFn.apply(this, filterParams);
                                            }
                                            catch (e) {
                                                cellValue = "";
                                            }
                                        }
                                    });
                            }

                            return cellValue;
                        };

                        options.exporterExcelCustomFormatters = function (grid, workbook, docDefinition) {
                            var stylesheet = workbook.getStyleSheet();

                            formatters = {};

                            for (var i = 0; i < grid.columns.length; i++) {
                                var column = grid.columns[i];

                                if (!column.cellFilter || formatters.hasOwnProperty(column.cellFilter))
                                    continue;

                                var filters = parseFilter(column.cellFilter);

                                var filter = filters.find(function (item) { return item.name === 'number'; });

                                if (!filter)
                                    continue;

                                var format = '#,##0';

                                if (filter.params.length === 1) {
                                    format += '.' + Array(parseInt(filter.params[0]) + 1).join('0');
                                }

                                var numberDefinition = {
                                    format: format
                                };

                                var numberFormatter = stylesheet.createFormat(numberDefinition);
                                formatters[column.cellFilter] = numberFormatter;
                            }

                            Object.assign(docDefinition.styles, formatters);

                            return docDefinition;
                        };

                        options.exporterFieldFormatCallback = function (grid, row, gridCol) {
                            // set metadata on export data to set format id. See exportExcelHeader config above for example of creating
                            // a formatter and obtaining the id
                            var formatterId = null;
                            if (gridCol.cellFilter && formatters.hasOwnProperty(gridCol.cellFilter)) {
                                formatterId = formatters[gridCol.cellFilter].id;
                            }

                            if (formatterId) {
                                return { metadata: { style: formatterId } };
                            } else {
                                return null;
                            }
                        };


                        function unquote(inputValue) {
                            if (!angular.isString(inputValue))
                                return inputValue;

                            inputValue = inputValue.trim();

                            if (inputValue.charAt(0) === '\'' && inputValue.charAt(inputValue.length - 1) === '\'')
                                return inputValue.substr(1, inputValue.length - 2);

                            return inputValue;
                        }

                        function parseFilter(filter) {
                            var filters = filter.split('|');

                            var mappedFilters = filters.map(function (item) {
                                var filterName = item.split(':')[0]; // fetch filter name
                                var filterParams = item.split(':').splice(1); //fetch all the filter parameters

                                filterParams = filterParams.map(function (param) { return unquote(param); });

                                return {
                                    name: filterName,
                                    params: filterParams
                                };
                            });

                            return mappedFilters;
                        }
                    }
                };
            }
        };
    }]);