angular
    .module('DataAggregatorModule')
    .controller('EtalonPriceController', ['$scope', '$http', 'uiGridCustomService', 'messageBoxService', 'errorHandlerService', 'formatConstants', 'uiGridPinningService', '$uibModal', EtalonPriceController]);

function EtalonPriceController($scope, $http, uiGridCustomService, messageBoxService, errorHandlerService, formatConstants, uiGridPinningService, $uibModal) {

    /*фильтр*/
    var today = new Date();
    var previousMonthDate = new Date(today.getFullYear(), today.getMonth() - 1, 1);
    $scope.devPercents = [0, 10, 20, 30, 40, 50, 60, 70, 80, 90];
    $scope.priceDiffDirections = [{ name: 'Все', value: 0 }, { name: 'Увеличение', value: 1 }, { name: 'Уменьшение', value: -1 }];
    $scope.filter = {
        date: previousMonthDate,
        devPercent: 10,
        searchText: null,
        priceDiffDirection: 0
    };
    $scope.commentStatuses = [];
    $scope.selectedPrices = [];
    $scope.isSourceInfoLoading = false;

    /*Grid*/
    $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'EtalonPrice_Grid');
    $scope.Grid.Options.showGridFooter = true;
    $scope.Grid.Options.multiSelect = true;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;
    $scope.Grid.Options.enableSelectAll = true;
    $scope.Grid.Options.enableFiltering = true;

    let skuTemplateHint = '<div class="ui-grid-cell-contents" ng-dblclick="javascript:void(0);" ng-click="grid.appScope.toggleClassifierClick($event, row, col)"><a href="javascript:void(0);">{{COL_FIELD}}</a></div>';
    let cellTemplateHint = '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD}}</div>';
    let sumCellTemplateHint = '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD | number:0}}</div>';
    let avgCellTemplateHint = '<div class="ui-grid-cell-contents avg_price" ng-click="grid.appScope.togglePriceSelection($event, row, col)">{{COL_FIELD | number:2}}</div>';
    let priceCellTemplateHint = '<div class="ui-grid-cell-contents number" ng-click="grid.appScope.togglePriceSelection($event, row, col)" title="{{COL_FIELD}}">{{COL_FIELD | number:2}}</div>';
    let diffCellTemplateHint = '<div class="ui-grid-cell-contents" ng-class="{\'red\' : row.entity.PriceDiff < 0, \'green\' : row.entity.PriceDiff >= 0}">{{COL_FIELD}}</div>';

    $scope.Grid.Options.columnDefs = [
        {
            cellTooltip: true, enableCellEdit: false, width: 80, visible: true, nullable: false, name: 'ClassifierId',
            field: 'ClassifierId', type: 'number', filter: { condition: uiGridCustomService.condition }, cellTemplate: skuTemplateHint, pinnedLeft: true
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 300, visible: true, nullable: true, name: 'Торговое наименование',
            field: 'TradeName', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint, pinnedLeft: true
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'МНН',
            field: 'INNGroup', filter: { condition: uiGridCustomService.condition }, pinnedLeft: true
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 300, visible: true, nullable: true, name: 'Описание препарата',
            field: 'DrugDescription', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint, pinnedLeft: true
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 300, visible: true, nullable: true, name: 'Правообладатель',
            field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint, pinnedLeft: true
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 50, visible: true, nullable: true, name: 'Тип',
            field: 'Type', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 80, visible: true, nullable: true, name: 'Заблокированные',
            field: 'Used', type: 'boolean', filter: { condition: uiGridCustomService.condition }
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Комм. по заблокированным',
            field: 'Comment', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'РУ',
            field: 'RegistrationCertificateNumber', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint
        },
        // Эталонная цена old
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'ЭЦ - 1',
            field: 'PricePrev', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint
        },
        // расчетная цена SellOut (или установленная через контектное меню цена из другого столбца)
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Расч. цена SellOut',
            field: 'PriceCalc', type: 'number', headerCellClass: 'avg_sellout_pricedata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint
        },
        // отклонение эталонной цены прошлого месяца
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: '[% откл.]',
            field: 'DeviationPercent', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint,
        },
        // разница расчётной цены и эталонной цены прошлого месяца
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Разница',
            field: 'PriceDiff', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: diffCellTemplateHint
        },
        //ЖНВЛП
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'ЖНВЛП',
            field: 'PriceVED', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        // Цена SellIn
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Цена SellIn',
            field: 'SellIn_PriceAVG', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint
        },
        // средняя цена - Контракты
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Контракты',
            field: 'Contract_PriceAVG', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint
        },
        // Комментарий
        {
            cellTooltip: true, enableCellEdit: true, width: 100, visible: true, nullable: true, name: 'Комментарий',
            field: 'CommentStatus', filter: { condition: uiGridCustomService.condition }
        },
        // Комментарий - 1
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Комментарий - 1',
            field: 'PrevCommentStatus', filter: { condition: uiGridCustomService.condition }
        },
        //суммы по СКЮ: Исходники и ОФД
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Исходники, руб.',
            field: 'Initial_Sum', type: 'number', headerCellClass: 'sellin', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: sumCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'ОФД, руб',
            field: 'OFD_Sum', type: 'number', headerCellClass: 'sellin', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: sumCellTemplateHint
        },

        // Исходные данные
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'AVG цена исх.', field: 'Initial_PriceAVG', type: 'number', headerCellClass: 'avg_initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint
        },
        // по источникам
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Ригла', field: 'Rigla_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Group 36,6', field: 'Group366_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Максавит', field: 'Maksavit_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Нео-Фарм', field: 'Neopharm_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Алоэ БСС', field: 'Aloe_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Вита', field: 'Vita_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Фармаимпекс', field: 'Farmaimpeks_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Прочие АС', field: 'Others_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        // Исходные данные <<<<<
        // Парсинг
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'AVG цена по сайтам', field: 'Downloaded_PriceAVG', type: 'number', headerCellClass: 'avg_downloadeddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint
        },
        // по сайтам
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Aptekaru', field: 'Aptekaru_PriceAVG',
            type: 'number', headerCellClass: 'downloadeddata', headerCellTemplate: '<div class="ui-grid-header-cell-label"><a href="https://apteka.ru/" target="blank">apteka.ru</a></div>',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Zdravcity', field: 'Zdravcity_PriceAVG',
            type: 'number', headerCellClass: 'downloadeddata', headerCellTemplate: '<div class="ui-grid-header-cell-label"><a href="https://zdravcity.ru/" target="blank">zdravcity.ru</a></div>',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Uteka', field: 'Uteka_PriceAVG',
            type: 'number', headerCellClass: 'downloadeddata', headerCellTemplate: '<div class="ui-grid-header-cell-label"><a href="https://uteka.ru/" target="blank">uteka.ru</a></div>',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Eapteka', field: 'Eapteka_PriceAVG',
            type: 'number', headerCellClass: 'downloadeddata', headerCellTemplate: '<div class="ui-grid-header-cell-label"><a href="https://www.eapteka.ru/" target="blank">eapteka.ru</a></div>',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        // Парсинг <<<<<
        // ОФД
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'AVG цена ОФД', field: 'OFD_PriceAVG', type: 'number', headerCellClass: 'avg_ofddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint
        },
        // по платформам
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Первый ОФД', field: 'OFD1_PriceAVG', type: 'number', headerCellClass: 'ofddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Платформа ОФД', field: 'Platformaofd_PriceAVG', type: 'number', headerCellClass: 'ofddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Ярус', field: 'OFDYa_PriceAVG', type: 'number', headerCellClass: 'ofddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Такском', field: 'Taxcom_PriceAVG', type: 'number', headerCellClass: 'ofddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'СКБ Контур', field: 'Kontur_PriceAVG', type: 'number', headerCellClass: 'ofddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'ИнитПро', field: 'Initpro_PriceAVG', type: 'number', headerCellClass: 'ofddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        // ОФД <<<<<
        {
            cellTooltip: true, enableCellEdit: false, width: 150, visible: true, nullable: true, name: 'Дата изменения', field: 'DateModified',
            filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 150, visible: true, nullable: true, name: 'Исполнитель',
            field: 'UserName', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint
        },
    ];

    const priceSelectedClass = 'price_selected';

    $scope.togglePriceSelection = function ($event, row, col) {
        var item = { ClassifierId: row.entity.ClassifierId, PriceField: col.field, TransferPrice: row.entity[col.field] };
        var elm = angular.element($event.target);

        if (parseFloat(item.TransferPrice) > 0) {
            const index = $scope.selectedPrices.findIndex(i => i.ClassifierId === item.ClassifierId);
            if (index === -1)
                $scope.selectedPrices.push(item);
            else {
                if ($scope.selectedPrices[index].PriceField === item.PriceField)
                    $scope.selectedPrices.splice(index, 1);
                else {
                    $scope.selectedPrices[index].PriceField = item.PriceField;
                    $scope.selectedPrices[index].TransferPrice = item.TransferPrice;

                    let elements = elm.parents('div[role="row"]:nth(0)').find('.' + priceSelectedClass);
                    elements.each(function (index, element) {
                        element.classList.remove(priceSelectedClass);
                    });
                }
            }
            if (elm.hasClass(priceSelectedClass))
                elm.removeClass(priceSelectedClass);
            else
                elm.addClass(priceSelectedClass);
        }
    }

    $scope.toggleClassifierClick = function ($event, row, col) {
        $event.stopPropagation();
        $event.preventDefault();

        var id = row.entity.Id;
        var classifierId = row.entity.ClassifierId;
        $scope.showClassifierInfo(id, classifierId);
    }

    $scope.showClassifierInfo = function (id, classifierId) {
        if (!$scope.isSourceInfoLoading) {
            $scope.isSourceInfoLoading = true;
            $scope.loading = $http({
                method: 'POST',
                url: '/EtalonPrice/GetSourceInfo',
                data: JSON.stringify({
                    year: $scope.filter.date.getFullYear(), month: $scope.filter.date.getMonth() + 1,
                    classifierId: classifierId
                }),
                async: false
            }).then(function (response) {
                $scope.isSourceInfoLoading = false;
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/Views/OFD/EtalonPrice/ClassifierInfo.html',
                    controller: 'ClassifierInfoController',
                    windowClass: 'center-modal',
                    backdrop: 'static',
                    size: 'giant',
                    resolve: {
                        data: function () {
                            return response.data
                        }
                    }
                });

                modalInstance.result.then(function () {
                    $scope.loading = $http({
                        method: 'POST',
                        url: '/EtalonPrice/SetForChecking/',
                        data: JSON.stringify({
                            id: id
                        })
                    }).then(function () {
                        messageBoxService.showInfo("Сохранено");
                    }, function (response) {
                        errorHandlerService.showResponseError(response);
                    });
                });
            });
        }
    }

    $scope.Grid.SetDefaults();

    $scope.Grid.afterCellEdit = function (rowEntity, colDef, newValue, oldValue) {
        if (newValue === oldValue || newValue === undefined)
            return;

        if (colDef.field == "CommentStatus" && rowEntity.CommentStatusId != null)
            return;
        else {
            rowEntity.CommentStatusId = null;
            rowEntity.CommentStatus = newValue;
            rowEntity.CommentStatusManual = newValue;
        }
    };

    $scope.Init = function () {
        $http({
            method: 'GET',
            url: '/EtalonPrice/GetCommentStatuses/'
        }).then(function (response) {
            $scope.commentStatuses = response.data;
        });
    }

    $scope.Init();

    $scope.getList = function () {
        $scope.clearSelectedPrices();
        $scope.loading = $http({
            method: 'POST',
            url: '/EtalonPrice/GetList',
            data: JSON.stringify({
                year: $scope.filter.date.getFullYear(), month: $scope.filter.date.getMonth() + 1,
                devPercent: $scope.filter.devPercent, searchText: $scope.filter.searchText,
                priceDiffDirection: $scope.filter.priceDiffDirection
            })
        }).then(function (response) {
            $scope.Grid.Options.data = response.data;
        }, function () {
            $scope.Grid.Options.data = [];
        });
    }

    $scope.reCalc = function () {
        messageBoxService.showConfirm('Вы уверены, что хотите пересчитать данные? Операция может занять длительное время.', 'Пересчитать')
            .then(
                function () {
                    $scope.loading = $http({
                        method: 'POST',
                        url: '/EtalonPrice/ReloadAllData',
                        data: JSON.stringify({
                            year: $scope.filter.date.getFullYear(), month: $scope.filter.date.getMonth() + 1
                        })
                    }).then(function () {
                        $scope.getList();
                    }, function (response) {
                        errorHandlerService.showResponseError(response);
                    });
                })

    };

    $scope.transfer = function () {
        messageBoxService.showError("Ещё не рализовано");
    }

    $scope.saveStatuses = function () {
        var array_upd = $scope.Grid.GetArrayModify();
        if (array_upd.length > 0) {
            var model = array_upd.map(function (obj) {
                return { "Id": obj.Id, "CommentStatusId": obj.CommentStatusId, "CommentStatusManual": obj.CommentStatusManual }
            });

            $scope.loading =
                $http({
                    method: 'POST',
                    url: '/EtalonPrice/SaveCommentStatuses/',
                    data: JSON.stringify({ array: model })
                }).then(function () {
                    $scope.Grid.ClearModify();
                    $scope.getList();
                    messageBoxService.showInfo("Сохранено");
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                    $scope.Grid.ClearModify();
                });
        }
    }

    $scope.uploadFromExcel = function (files) {
        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item) {
                formData.append('file', item);
            });
            formData.append('month', $scope.filter.date.getMonth() + 1);
            formData.append('year', $scope.filter.date.getFullYear());

            $scope.loading = $http({
                method: 'POST',
                url: '/EtalonPrice/UploadFromExcel/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function () {
                messageBoxService.showInfo("Сохранено");
                $scope.getList();
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        }
    };

    /*-------------------------------------------------------------------------actions-------------------------------------------------------------------------*/

    $scope.setComment = function (value, caption) {
        if (value && $scope.Grid.selectedRows())
            $scope.Grid.selectedRows().forEach(function (item) {
                item.CommentStatusId = value;
                item.CommentStatus = caption;
                $scope.Grid.GridCellsMod(item, "CommentStatus", caption);
            });
    }

    $scope.clearComment = function () {
        if ($scope.Grid.selectedRows())
            $scope.Grid.selectedRows().forEach(function (item) {
                $scope.Grid.GridCellsMod(item, "CommentStatusId", null);
                $scope.Grid.GridCellsMod(item, "CommentStatus", null);
            });
    }

    $scope.transferSelectedPrices = function () {
        if ($scope.selectedPrices.length > 0) {
            messageBoxService.showConfirm('Вставить выделенные (' + $scope.selectedPrices.length + ' шт.) цены?', 'Перенос цен')
                .then(function () {
                    $scope.loading =
                        $http({
                            method: 'POST',
                            url: '/EtalonPrice/TransferPrice/',
                            data: JSON.stringify({
                                array: $scope.selectedPrices,
                                year: $scope.filter.date.getFullYear(),
                                month: $scope.filter.date.getMonth() + 1
                            })
                        }).then(function (response) {
                            if (response.data.Data) {
                                $scope.mergeGrid(response.data.Data);
                                $scope.clearSelectedPrices();
                            }
                            messageBoxService.showInfo("Сохранено");
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });
                });
        }
    }

    $scope.mergeGrid = function (data) {
        $scope.Grid.Options.data.forEach(item => {
            let index = data.findIndex(i => i.ClassifierId === item.ClassifierId);
            if (index >= 0) {
                let target = data[index];
                item.PriceCalc = target.TransferPrice;
                item.PriceDiff = target.PriceDiff;
                item.DeviationPercent = target.DeviationPercent;
                item.DateModified = target.DateModified;
                item.UserName = target.UserName;
            }
        })
    }

    $scope.clearSelectedPrices = function () {
        $scope.selectedPrices = [];
        let elements = document.querySelectorAll('.' + priceSelectedClass);
        elements.forEach((element) => {
            element.classList.remove(priceSelectedClass);
        });
    }
}

angular
    .module('DataAggregatorModule')
    .controller('ClassifierInfoController', [
        '$scope', '$uibModalInstance', 'uiGridCustomService', ClassifierInfoController]);

function ClassifierInfoController($scope, $modalInstance, uiGridCustomService) {
    $scope.currentTabIndex = 0;
    $scope.classifierData = null;

    let numberCellTemplate = '<div class="ui-grid-cell-contents">{{COL_FIELD | number:2}}</div>';

    $scope.InitialGrid = uiGridCustomService.createGridClassMod($scope, "InitialGrid");

    $scope.InitialGrid.Options.columnDefs = [
        { name: 'Источник данных', width: 230, enableCellEdit: false, field: 'SourceName', filter: { condition: uiGridCustomService.condition } },
        { name: 'DrugClearId', width: 125, enableCellEdit: false, field: 'DrugClearId', filter: { condition: uiGridCustomService.condition } },
        { name: 'Исходные строки написание', enableCellEdit: false, field: 'OriginalDrugName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Цена', cellTooltip: true, width: 100, enableCellEdit: false, field: 'Price', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numberCellTemplate }
    ];

    $scope.DownloadedGrid = uiGridCustomService.createGridClassMod($scope, "DownloadedGrid");

    $scope.DownloadedGrid.Options.columnDefs = [
        { name: 'Источник данных', width: 230, enableCellEdit: false, field: 'SourceName', filter: { condition: uiGridCustomService.condition } },
        { name: 'DrugClearId', width: 125, enableCellEdit: false, field: 'DrugClearId', filter: { condition: uiGridCustomService.condition } },
        { name: 'Исходные строки написание', enableCellEdit: false, field: 'OriginalDrugName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Цена', cellTooltip: true, width: 100, enableCellEdit: false, field: 'Price', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numberCellTemplate }
    ];

    $scope.OfdGrid = uiGridCustomService.createGridClassMod($scope, "OfdGrid");

    $scope.OfdGrid.Options.columnDefs = [
        { name: 'Источник данных', width: 230, enableCellEdit: false, field: 'SourceName', filter: { condition: uiGridCustomService.condition } },
        { name: 'DrugClearId', width: 125, enableCellEdit: false, field: 'DrugClearId', filter: { condition: uiGridCustomService.condition } },
        { name: 'Исходные строки написание', enableCellEdit: false, field: 'OriginalDrugName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Цена', cellTooltip: true, width: 100, enableCellEdit: false, field: 'Price', type: 'number', filter: { condition: uiGridCustomService.numberCondition } }
    ];

    $scope.init = function () {
        if ($scope.$resolve.data != null && $scope.$resolve.data.length > 0) {
            let data = $scope.$resolve.data;

            $scope.classifierData = {
                PeriodShort: data[0].PeriodShort,
                ClassifierId: data[0].ClassifierId,
                TradeName: data[0].TradeName,
                DrugDescription: data[0].DrugDescription,
                OwnerTradeMark: data[0].OwnerTradeMark,
            };

            $scope.InitialGrid.Options.data = data.filter(x => x.SourceTypeId === 1);
            $scope.DownloadedGrid.Options.data = data.filter(x => x.SourceTypeId === 2);
            $scope.OfdGrid.Options.data = data.filter(x => x.SourceTypeId === 3);
        }
    }

    $scope.init();

    $scope.setTabIndex = function (index) {
        $scope.currentTabIndex = index;
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    $scope.save = function () {
        $modalInstance.close();
    };
}