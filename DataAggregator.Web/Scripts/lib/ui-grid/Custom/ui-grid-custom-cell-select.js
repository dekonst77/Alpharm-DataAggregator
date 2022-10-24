angular.module('ui.grid.module')
    .directive('uiGridCustomCellSelect', ['$timeout', function ($timeout) {
        return {
            replace: true,
            require: '^uiGrid',
            controller: function () { },
            compile: function () {
                return {
                    pre: function () { },
                    post: function (scope, $elm, attrs, uiGridCtrl) {

                        var options = scope.$eval(attrs.uiGrid);

                        if (options.customEnableCellSelection === false)
                            return;

                        if (!options.customCellSelect)
                            options.customCellSelect = {};

                        var ugCustomSelect = options.customCellSelect;

                        var grid = uiGridCtrl.grid;

                        // Data setup
                        var newUgCustomSelect = {
                            hiddenInput: angular.element('<input class="ui-grid-custom-selection-input" type="text" />').appendTo('body'),
                            isDragging: false,
                            selectedCells: [],
                            selectedRows: [],
                            cellMap: {},
                            copyData: '',
                            dragData: {
                                startCell: {
                                    row: null,
                                    col: null
                                },
                                endCell: {
                                    row: null,
                                    col: null
                                }
                            },
                            clearSelection: clearDragData
                        };

                        angular.extend(ugCustomSelect, newUgCustomSelect);

                        // Bind events
                        $timeout(function () {
                            grid.element.on('mousedown', '.ui-grid-cell-contents', dragStart);
                            grid.element.on('mouseenter', '.ui-grid-cell-contents', mouseEnterCell);
                            angular.element('body').on('mouseup', bodyMouseUp);
                            angular.element(document).on('keydown', documentKeyUp);
                            angular.element(document).on('copy', documentCopyCells);
                            grid.api.core.on.scrollBegin(scope, gridScrollBegin);
                            grid.api.core.on.scrollEnd(scope, gridScrollEnd);

                            grid.api.core.on.filterChanged(scope, clearDragData);
                            grid.api.core.on.columnVisibilityChanged(scope, clearDragData);
                            grid.api.core.on.rowsVisibleChanged(scope, clearDragData);
                            grid.api.core.on.sortChanged(scope, clearDragData);
                        });

                        // Events
                        function dragStart(evt) {
                            if (angular.element(evt.target).hasClass('ui-grid-cell-contents')) {
                                var cellData = $(this).data().$scope;
                                clearDragData(true);
                                ugCustomSelect.isDragging = true;
                                setStartCell(cellData.row, cellData.col);
                                setSelectedStates();
                            }
                        }

                        function mouseEnterCell() {
                            if (ugCustomSelect.isDragging) {
                                var cellData = $(this).data().$scope;
                                setEndCell(cellData.row, cellData.col);
                                setSelectedStates();
                            }
                        }

                        function bodyMouseUp() {
                            if (ugCustomSelect.isDragging) {
                                ugCustomSelect.isDragging = false;
                            }
                        }

                        function documentKeyUp(evt) {
                            var cKey = 67;
                            if (evt.keyCode === cKey && evt.ctrlKey && window.getSelection() + '' === '') {
                                ugCustomSelect.hiddenInput.val(' ').select();
                                document.execCommand('copy');
                                evt.preventDefault();
                            }
                        }

                        function documentCopyCells(evt) {
                            var cbData,
                                cbType;

                            if (evt.originalEvent.clipboardData) {
                                cbData = evt.originalEvent.clipboardData;
                                cbType = 'text/plain';
                            } else {
                                cbData = window.clipboardData;
                                cbType = 'Text';
                            }

                            if (cbData && (window.getSelection() + '' === '' || window.getSelection() + '' === ' ') && ugCustomSelect.copyData !== '') {
                                cbData.setData(cbType, ugCustomSelect.copyData);
                                evt.preventDefault();
                            }
                        }

                        function gridScrollBegin() {
                            grid.element.addClass('ui-grid-custom-selected-scrolling');
                        }

                        function gridScrollEnd() {
                            angular.element('.ui-grid-custom-selected').removeClass('ui-grid-custom-selected');
                            var visibleCols = grid.renderContainers.body.renderedColumns;
                            var visibleRows = grid.renderContainers.body.renderedRows;

                            for (var ri = 0; ri < visibleRows.length; ri++) {
                                var currentRow = visibleRows[ri];
                                for (var ci = 0; ci < visibleCols.length; ci++) {
                                    var currentCol = visibleCols[ci];
                                    var rowCol = uiGridCtrl.cellNav.makeRowCol({ row: currentRow, col: currentCol });

                                    if (cellIsSelected(rowCol)) {
                                        getCellElem(currentCol, ri).find('.ui-grid-cell-contents').addClass('ui-grid-custom-selected');
                                    }
                                }
                            }

                            grid.element.removeClass('ui-grid-custom-selected-scrolling');
                        }

                        // Functions
                        function setStartCell(row, col) {
                            ugCustomSelect.dragData.startCell.row = row;
                            ugCustomSelect.dragData.startCell.col = col;
                        }

                        function setEndCell(row, col) {
                            ugCustomSelect.dragData.endCell.row = row;
                            ugCustomSelect.dragData.endCell.col = col;
                        }

                        function clearDragData(preventEvent) {
                            clearEndCell();
                            clearStartCell();
                            clearSelectedStates();
                            ugCustomSelect.copyData = '';
                            if (!preventEvent)
                                selectedChanged();
                        }

                        function clearStartCell() {
                            ugCustomSelect.dragData.startCell.row = null;
                            ugCustomSelect.dragData.startCell.col = null;
                        }

                        function clearEndCell() {
                            ugCustomSelect.dragData.endCell.row = null;
                            ugCustomSelect.dragData.endCell.col = null;
                        }

                        // Sets selected styling based on start cell and end cell, including cells in between that range
                        function setSelectedStates() {
                            clearSelectedStates();

                            var indexMap = createIndexMap(ugCustomSelect.dragData.startCell, ugCustomSelect.dragData.endCell);
                            ugCustomSelect.selectedCells = getCellsWithIndexMap(indexMap);

                            ugCustomSelect.selectedRows = getRowsWithIndexMap(indexMap);

                            ugCustomSelect.cellMap = ugCustomSelect.selectedCells.reduce(function (map, obj) {
                                if (map[obj.rowCol.row.uid]) {
                                    map[obj.rowCol.row.uid].push(obj.rowCol.col.uid);
                                } else {
                                    map[obj.rowCol.row.uid] = [obj.rowCol.col.uid];
                                }
                                return map;
                            }, {});

                            for (var i = 0; i < ugCustomSelect.selectedCells.length; i++) {
                                var currentCell = ugCustomSelect.selectedCells[i];
                                currentCell.elem.find('.ui-grid-cell-contents').addClass('ui-grid-custom-selected');
                            }

                            ugCustomSelect.copyData = createCopyData(ugCustomSelect.selectedCells, (indexMap.col.end - indexMap.col.start) + 1);

                            selectedChanged();
                        }

                        // Clears selected state from any selected cells
                        function clearSelectedStates() {
                            angular.element('.ui-grid-custom-selected').removeClass('ui-grid-custom-selected');
                            ugCustomSelect.selectedCells = [];
                            ugCustomSelect.selectedRows = [];
                            ugCustomSelect.cellMap = {};
                        }

                        function createIndexMap() {
                            var rowStart = grid.renderContainers.body.renderedRows.indexOf(ugCustomSelect.dragData.startCell.row);
                            var rowEnd = grid.renderContainers.body.renderedRows.indexOf(ugCustomSelect.dragData.endCell.row);
                            var colStart = grid.renderContainers.body.renderedColumns.indexOf(ugCustomSelect.dragData.startCell.col);
                            var colEnd = grid.renderContainers.body.renderedColumns.indexOf(ugCustomSelect.dragData.endCell.col);

                            if (rowEnd === -1)
                                rowEnd = rowStart;

                            if (colEnd === -1)
                                colEnd = colStart;

                            return {
                                row: {
                                    start: (rowStart < rowEnd) ? rowStart : rowEnd,
                                    end: (rowEnd > rowStart) ? rowEnd : rowStart
                                },
                                col: {
                                    start: (colStart < colEnd) ? colStart : colEnd,
                                    end: (colEnd > colStart) ? colEnd : colStart
                                }
                            };
                        }

                        function getCellsWithIndexMap(indexMap) {
                            var visibleCols = grid.renderContainers.body.renderedColumns;
                            var visibleRows = grid.renderContainers.body.renderedRows;
                            var cellsArray = [];

                            for (var ri = indexMap.row.start; ri <= indexMap.row.end; ri++) {
                                var currentRow = visibleRows[ri];
                                for (var ci = indexMap.col.start; ci <= indexMap.col.end; ci++) {
                                    var currentCol = visibleCols[ci];
                                    var rowCol = uiGridCtrl.cellNav.makeRowCol({ row: currentRow, col: currentCol });
                                    var cellElem = getCellElem(rowCol.col, ri);

                                    if (cellElem) {
                                        cellsArray.push({
                                            rowCol: rowCol,
                                            elem: cellElem
                                        });
                                    }
                                }
                            }

                            return cellsArray;
                        }

                        function getRowsWithIndexMap(indexMap) {
                            var visibleRows = grid.renderContainers.body.renderedRows;
                            var rowsArray = [];

                            for (var ri = indexMap.row.start; ri <= indexMap.row.end; ri++) {
                                rowsArray.push(visibleRows[ri]);
                            }

                            return rowsArray;
                        }

                        function cellIsSelected(rowCol) {
                            if (rowCol && rowCol.row && rowCol.col) {
                                return ugCustomSelect.cellMap[rowCol.row.uid] !== undefined && ugCustomSelect.cellMap[rowCol.row.uid].indexOf(rowCol.col.uid) > -1;
                            }
                            return false;
                        }

                        function getCellElem(col, rowIndex) {
                            return (col && col.uid && typeof rowIndex == 'number') ? angular.element('#' + grid.id + '-' + rowIndex + '-' + col.uid + '-cell') : null;
                        }

                        function createCopyData(cells, numCols) {
                            var copyData = '';

                            for (var i = 0; i < cells.length; i++) {
                                var currentCell = cells[i];
                                var cellValue = grid.getCellDisplayValue(currentCell.rowCol.row, currentCell.rowCol.col);

                                copyData += cellValue;

                                if ((i + 1) % numCols === 0 && i !== cells.length - 1) {
                                    copyData += '\r\n';
                                } else if (i !== cells.length - 1) {
                                    copyData += '\t';
                                }
                            }

                            return copyData;
                        }

                        function selectedChanged() {
                            $timeout(function() {
                                if (ugCustomSelect.selectedChanged)
                                    ugCustomSelect.selectedChanged();
                            });
                        }
                    }
                };
            }
        };
    }]);