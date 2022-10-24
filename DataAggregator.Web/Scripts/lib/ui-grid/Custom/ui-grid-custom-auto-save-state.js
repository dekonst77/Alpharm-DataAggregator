angular.module('ui.grid')
    .directive('uiGridCustomAutoSaveState', ['$timeout', 'i18nService', function ($timeout, i18nService) {
        return {
            replace: true,
            require: '^uiGrid',
            controller: function () { },
            compile: function () {
                return {
                    pre: function () { },
                    post: function (scope, $elm, attrs, uiGridCtrl) {
                        var options = scope.$eval(attrs.uiGrid);

                        var gridStateName = 'UiGridState_' + options.gridName;

                        var defaultState;

                        var grid = uiGridCtrl.grid;

                        // Bind events
                        $timeout(function () {
                            cacheDefaultState();
                            restoreSavedState();

                            changeMenu();

                            grid.api.colResizable.on.columnSizeChanged(scope, saveState);

                            if (grid.api.colMovable)
                                grid.api.colMovable.on.columnPositionChanged(scope, saveState);

                            grid.api.core.on.columnVisibilityChanged(scope, saveState);
                            grid.api.core.on.filterChanged(scope, saveState);
                            grid.api.core.on.sortChanged(scope, saveState);
                        });

                        function changeMenu() {
                            grid.api.core.addToGridMenu(grid,
                                [
                                    {
                                        title: i18nService.getSafeText('gridMenu.resetState'),
                                        action: restoreDefaultState,
                                        order: 100
                                    }]);
                        }

                        function cacheDefaultState() {
                            defaultState = grid.api.saveState.save();
                        }

                        function saveState() {
                            var state = grid.api.saveState.save();

                            if (options.gridVersion)
                                state.gridVersion = options.gridVersion;

                            localStorage.setItem(gridStateName, JSON.stringify(state));
                        }

                        function restoreSavedState() {
                            var jsonState = localStorage.getItem(gridStateName);

                            if (jsonState) {
                                var state = JSON.parse(jsonState);

                                if (state.gridVersion === options.gridVersion && isColumnsTheSame(state.columns, defaultState.columns)) {
                                    adoptStateToDefault(state);
                                    grid.api.saveState.restore(scope, state);
                                }
                            }
                        }

                        function restoreDefaultState() {
                            grid.api.saveState.restore(scope, defaultState);
                            saveState();
                        }

                        function isColumnsTheSame(firstColumns, secondColumns) {

                            if (firstColumns.length !== secondColumns.length)
                                return false;

                            if (firstColumns.length === 0)
                                return true;

                            var firstColumnNames = extractAndSortColumnNames(firstColumns);
                            var secondColumnNames = extractAndSortColumnNames(secondColumns);

                            for (var i = 0; i < firstColumnNames.length; i++) {
                                if (firstColumnNames[i] !== secondColumnNames[i])
                                    return false;
                            }

                            return true;
                        }

                        function extractAndSortColumnNames(columns) {
                            return columns.map(function(item) { return item.name; }).sort();
                        }

                        function adoptStateToDefault(state) {
                            adoptPaginationState(state);
                        }

                        function adoptPaginationState(restoredState) {
                            if (defaultState.pagination.paginationPageSize && !restoredState.pagination.paginationPageSize) {
                                restoredState.pagination = angular.copy(defaultState.pagination);
                                return;
                            }

                            if (!defaultState.pagination.paginationPageSize && restoredState.pagination.paginationPageSize) {
                                restoredState.pagination = {};
                                return;
                            }
                        }
                    }
                };
            }
        };
    }]);