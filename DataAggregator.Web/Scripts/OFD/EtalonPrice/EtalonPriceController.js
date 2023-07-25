angular
    .module('DataAggregatorModule')
    .controller('EtalonPriceController', ['$scope', '$http', 'uiGridCustomService', 'messageBoxService', 'errorHandlerService', EtalonPriceController]);

function EtalonPriceController($scope, $http, uiGridCustomService, messageBoxService, errorHandlerService) {

    /*фильтр*/
    var today = new Date();
    var previousMonthDate = new Date(today.getFullYear(), today.getMonth() - 1, 1);
    $scope.devPercents = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9];
    $scope.filter = {
        date: previousMonthDate,
        devPercent: 0.8,
        searchText: null
    };
    $scope.commentStatuses = [];

    /*Grid*/
    $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'EtalonPrice_Grid');
    $scope.Grid.Options.showGridFooter = true;
    $scope.Grid.Options.multiSelect = true;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;
    $scope.Grid.Options.enableSelectAll = true;
    $scope.Grid.Options.enableFiltering = true;

    let cellTemplateHint = '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'
    let numbercellTemplateHint = '<div class="ui-grid-cell-contents number" title="{{COL_FIELD}}">{{COL_FIELD | number:2}}</div>'
    let avgCellTemplateHint = '<div class="ui-grid-cell-contents">{{COL_FIELD | number:2}}</div>'
    let priceCellTemplateHint = '<div class="ui-grid-cell-contents number" ng-dblclick="grid.appScope.transferPrice(row,col)" title="{{COL_FIELD}}">{{COL_FIELD | number:2}}</div>'

    $scope.Grid.Options.columnDefs = [
        {
            cellTooltip: true, enableCellEdit: false, width: 80, visible: true, nullable: false, name: 'ClassifierId',
            field: 'ClassifierId', type: 'number', filter: { condition: uiGridCustomService.condition }
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 300, visible: true, nullable: true, name: 'Торговое наименование',
            field: 'TradeName', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'МНН',
            field: 'INNGroup', filter: { condition: uiGridCustomService.condition }
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 300, visible: true, nullable: true, name: 'Описание препарата',
            field: 'DrugDescription', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 300, visible: true, nullable: true, name: 'Правообладатель',
            field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 50, visible: true, nullable: true, name: 'Тип',
            field: 'Type', filter: { condition: uiGridCustomService.condition }, cellTemplate: cellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'ЖНВЛП',
            field: 'PriceVED', type: 'number', filter: { condition: uiGridCustomService.condition }, cellTemplate: priceCellTemplateHint
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
        // отклонение эталонной цены прошлого месяца
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: '% откл.',
            field: 'DeviationPercent', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.condition }, cellTemplate: avgCellTemplateHint
        },
        // расчетная цена SellOut (или установленная через контектное меню цена из другого столбца)
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Расч. цена SellOut',
            field: 'PriceCalc', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: avgCellTemplateHint
        },
        // Цена SellIn
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Цена SellIn',
            field: 'SellIn_PriceAVG', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        // средняя цена - Контракты
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Контракты',
            field: 'Contract_PriceAVG', type: 'number', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        // Комментарий
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Комментарий',
            field: 'CommentStatus', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.condition }
        },
        // Комментарий - 1
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Комментарий - 1',
            field: 'PrevCommentStatus', headerCellClass: 'contractdata', filter: { condition: uiGridCustomService.condition }
        },
        //суммы по СКЮ: Исходники и ОФД
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'Исходники, руб.',
            field: 'Initial_Sum', type: 'number', headerCellClass: 'sellin', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint
        },
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'ОФД, руб',
            field: 'OFD_Sum', type: 'number', headerCellClass: 'sellin', filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: numbercellTemplateHint
        },

        // Исходные данные
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'AVG цена исх.', field: 'Initial_PriceAVG', type: 'number', headerCellClass: 'initialdata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
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
        // Исходные данные <<<<<
        // Парсинг
        {
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'AVG цена по сайтам', field: 'Downloaded_PriceAVG', type: 'number', headerCellClass: 'downloadeddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
        },
        //Downloaded_PriceAVG
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
            cellTooltip: true, enableCellEdit: false, width: 100, visible: true, nullable: true, name: 'AVG цена ОФД', field: 'OFD_PriceAVG', type: 'number', headerCellClass: 'ofddata',
            filter: { condition: uiGridCustomService.numberCondition }, cellTemplate: priceCellTemplateHint
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
    ];

    $scope.Grid.SetDefaults();

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
        $scope.loading = $http({
            method: 'POST',
            url: '/EtalonPrice/GetList',
            data: JSON.stringify({
                year: $scope.filter.date.getFullYear(), month: $scope.filter.date.getMonth() + 1,
                devPercent: $scope.filter.devPercent, searchText: $scope.filter.searchText
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
                    $http({
                        method: 'POST',
                        url: '/EtalonPrice/ReloadAllData',
                        data: JSON.stringify({
                            year: $scope.filter.date.getFullYear(), month: $scope.filter.date.getMonth() + 1
                        })
                    }).then(function () {
                        $scope.getList(response);
                    }, function () {
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
                return { "Id": obj.Id, "CommentStatusId": obj.CommentStatusId }
            });

            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/EtalonPrice/SaveCommentStatuses/',
                    data: JSON.stringify({ array: model })
                }).then(function () {
                    $scope.getList();
                    messageBoxService.showInfo("Сохранено");
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                    $scope.Grid.ClearModify();
                });
        }
    }

    /*-------------------------------------------------------------------------actions-------------------------------------------------------------------------*/

    $scope.setComment = function (value, caption) {
        if (value && $scope.Grid.selectedRows())
            $scope.Grid.selectedRows().forEach(function (item) {
                $scope.Grid.GridCellsMod(item, "CommentStatusId", value);
                $scope.Grid.GridCellsMod(item, "CommentStatus", caption);
            });
    }

    $scope.clearComment = function () {
        if ($scope.Grid.selectedRows())
            $scope.Grid.selectedRows().forEach(function (item) {
                $scope.Grid.GridCellsMod(item, "CommentStatusId", null);
            });
    }

    $scope.transferPrice = function (row, col) {
        var entity = row.entity;
        var TransferPrice = row.entity[col.field];

        if (TransferPrice && parseFloat(TransferPrice) > 0) {
            messageBoxService.showConfirm('Подставить значение ' + TransferPrice + ' в Расчетная цена sell out?', 'Перенос цены')
                .then(function () {
                    entity.TransferPrice = TransferPrice;
                    var model = [];
                    model.push({ "Id": entity.Id, "TransferPrice": entity.TransferPrice });

                    $scope.dataLoading =
                        $http({
                            method: 'POST',
                            url: '/EtalonPrice/TransferPrice/',
                            data: JSON.stringify({ array: model })
                        }).then(function () {
                            entity.PriceCalc = entity.TransferPrice;
                            messageBoxService.showInfo("Сохранено");
                        }, function (response) {
                            errorHandlerService.showResponseError(response);
                        });
                });
        }
    };
}