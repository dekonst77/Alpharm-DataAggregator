﻿angular
    .module('DataAggregatorModule')
    .controller('PriceCurrentController_v2', ['$scope', '$http', 'hotkeys', 'uiGridCustomService', 'formatConstants', PriceCurrentController_v2]);

function PriceCurrentController_v2($scope, $http, hotkeys, uiGridCustomService, formatConstants) {


    hotkeys.bindTo($scope).add({
        combo: 'shift+c',
        description: 'На проверку',
        callback: function (event) {
            $scope.forChecking();
            event.preventDefault();
        }
    });

    hotkeys.bindTo($scope).add({
        combo: 'shift+d',
        description: 'Копировать описание',
        callback: function (event) {
            $scope.copyIntoBuffer();
            event.preventDefault();
        }
    });
    
    $scope.forChecking = function (Checking) {
        var selectedRows = $scope.gridApi.selection.getSelectedRows();
        if (selectedRows.length === 0)
            return;

        selectedRows.forEach(function (row) {
            if (Checking != null)
                row.ForChecking = Checking;
            else
                row.ForChecking = !row.ForChecking;

            $scope.edit(row);
        });

    };

    $scope.copyIntoBuffer = function (field) {
        var selectedRows = $scope.gridApi.selection.getSelectedRows();
        if (selectedRows.length === 0)
            return;

        var text = (selectedRows[0].TradeName + ' ' + selectedRows[0].DrugDescription + ' ' + selectedRows[0].OwnerTradeMark).trim();


        var textArea = document.createElement("textarea");
        textArea.value = text;
        textArea.style.position = "fixed";  //avoid scrolling to bottom
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        document.execCommand('copy');
        document.body.removeChild(textArea);

    }

    $scope.form = {};
    $scope.form.IsRun = true;
    $scope.CalcStaus = true;

    //Выставляем даты
    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);
    $scope.form.date = previousMonthDate;
    $scope.priceGrid = uiGridCustomService.createOptions('priceEtalon_Grid');

    var gridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 30,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableSelectionBatchEvent: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: true,
        noUnselect: false,
        showGridFooter: true,
    };

    angular.extend($scope.priceGrid, gridOptions);

    //$scope.classifierGrid.Options.rowTemplate = '_rowClassifierTemplate.html';
    $scope.priceGrid.columnDefs = [
        {
            name: 'Class.Id', field: 'ClassifierId', type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT, enableCellEdit: false,
            cellTemplate: '<div><div ng-class="{checking:row.entity.ForChecking}" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
        },

        {
            name: 'Нет', field: 'WithoutPrice', type: 'boolean', width: 40, filter: { condition: uiGridCustomService.booleanConditionX },
            cellTemplate: '<input type="checkbox" ng-model="row.entity.WithoutPrice" ng-change="grid.appScope.edit(row.entity)">'
        },

        {
            name: 'Sum30k',
            field: 'Sum30k',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            enableCellEdit: false
        },    
       
        { name: 'ТН', field: 'TradeName', enableCellEdit: false },
        { name: 'Описание', field: 'DrugDescription', enableCellEdit: false },
        {// Описание
            name: 'Описание',
            field: 'DrugDescription',
            cellTemplate: '<div><div class= "ui-grid-cell-contents description" title = "{{row.entity.DrugDescription}}">{{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false,
            style: 'font-size: 10px;'
        },
        { name: 'МНН', field: 'INNGroup', enableCellEdit: false },
        { name: 'Тип', field: 'Type', enableCellEdit: false },
        { name: 'Правообладатель', field: 'OwnerTradeMark', enableCellEdit: false },
        { name: 'Упаковщик', field: 'Packer', enableCellEdit: false },
        { name: 'Владелец РУ', field: 'OwnerRegistrationCertificate', enableCellEdit: false },
        {
            name: 'SellOut',
            field: 'RetailDataSellingSum',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            enableCellEdit: false
        },
        {// Old
            name: 'Old',
            field: 'PriceOld',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-click="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents"> {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },       
        { name: 'New', field: 'PriceNew', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
      
        {// raw
            name: 'Апт.Mediana',
            field: 'RetailDataMedianaPrice',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },
        {//OFD
            name: 'ОФД Mediana',
            field: 'OFDMediana50Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },
        {// percentile OFD
            name: 'ОФД 65th',
            field: 'OFDMediana65Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false,
        },
        {
            name: 'OFDSum',
            field: 'OFDSum',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            enableCellEdit: false
        },       
      
        {
            name: 'ЖНВЛП',
            field: 'PriceVED',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },
        {
            name: 'РУ',
            field: 'RegistrationCertificateNumber',
            enableCellEdit: false
        },
      
        { name: 'Комм.', field: 'Comment', enableCellEdit: true, enableCellEdit: true },
        { name: 'Обновление', field: 'DateUpdate', enableCellEdit: false, cellFilter: formatConstants.FILTER_DATE },
        { name: 'User', field: 'User', enableCellEdit: false },    
        {
            name: 'AptekaMediana50Price',
            field: 'Web1Mediana50Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },

        {
            name: 'AptekaMediana65Price',
            field: 'Web1Mediana65Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },

        {
            name: 'EaptekaMediana50Price',
            field: 'Web2Mediana50Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },

        {
            name: 'EaptekaMediana65Price',
            field: 'Web2Mediana65Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },

        {
            name: 'Mos3Mediana50Price',
            field: 'Web3Mediana50Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },

        {
            name: 'MosMediana65Price',
            field: 'Web3Mediana65Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },

        {
            name: 'AprilMediana50Price',
            field: 'Web4Mediana50Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },

        {
            name: 'AprilMediana65Price',
            field: 'Web4Mediana65Price',
            type: 'number',
            filter: { condition: uiGridCustomService.numberCondition },
            cellFilter: formatConstants.FILTER_INT_COUNT,
            cellTemplate: '<div><div ng-dblclick="grid.appScope.cellClicked(row,col)" class= "ui-grid-cell-contents" > {{ COL_FIELD CUSTOM_FILTERS }}</div></div>',
            enableCellEdit: false
        },

        {
            name: '[%OFD]',
            field: 'ProcentOFD',
            type: 'number',
            enableCellEdit: false
        },

        {
            name: '[%AB]',
            field: 'ProcentAB',
            type: 'number',
            enableCellEdit: false
        },

        {
            name: '[%OFD/AB]',
            field: 'ProcentOFDAB',
            type: 'number',
            enableCellEdit: false
        },

      




    ];

    //Отслеживает изменения в ячейках и сохраняем их в базу
    $scope.$on('uiGridEventEndCellEdit', function (data) {
        var newValue = data.targetScope.row.entity;
        $scope.edit(newValue);
    });

    $scope.priceGrid.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    };

    $scope.cellClicked = function (row, col) {
        var newValue = row.entity;
        newValue.PriceNew = row.entity[col.field];
        $scope.edit(newValue);
    };

    $scope.change = function (model) {
        $scope.edit(model);
    }

    //Внесли изменения в строчку
    $scope.edit = function (model) {

        var request = {
            Price: model
        };
        $scope.loading = $http({
            method: 'POST',
            url: '/PriceCurrent_v2/Edit',
            data: JSON.stringify(request)
        }).then(function (response) {
            var editedRow = $.grep($scope.priceGrid.data, function (r) {
                return r.ClassifierId == model.ClassifierId;
            })[0];
            //Список полей на изменение
            editedRow.ForChecking = response.data.ForChecking;
            editedRow.DateUpdate = response.data.DateUpdate;
            editedRow.Comment = response.data.Comment;
            editedRow.PriceNew = response.data.PriceNew;
            editedRow.DateUpdate = response.data.DateUpdate;
            editedRow.UserIdUpdate = response.data.UserIdUpdate;
            editedRow.User = response.data.User;
        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });
    }

    $scope.update = function () {
        //Проверка на запущенный рассчет
        GetCalcLock();
        //Посмотреть историю запусков
        GetStatuses();
        //Загрузка
        load();
    }

    //Перенести текущие цены в эталонные
    $scope.currentPriceCopyToEtalonPrice = function () {
        var request = {
            Date: $scope.form.date
        };
        $scope.loading = $http({
            method: 'POST',
            url: '/PriceCurrent_v2/CurrentPriceCopyToEtalonPrice',
            data: JSON.stringify(request)
        }).then(function (response) {

        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });
    }

    //Запустить расчет
    $scope.runCalcCurrentPrice = function () {

        $scope.loading = $http({
            method: 'POST',
            url: '/PriceCurrent_v2/RunCalcCurrentPrice',
        }).then(function (response) {
            $scope.update();
        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });
    }

    //Загрузка
    function load() {

        $scope.loading = $http.get('/PriceCurrent_v2/Load')
            .then(function (response) {
                $scope.alldata = response.data.Data;
                $scope.ChangeShow();
            }, function (response) {
                alert('Ошибка:\n' + JSON.stringify(response));
            });
    }

    $scope.ChangeShow = function () {
        if ($scope.showAll)
            $scope.priceGrid.data = $scope.alldata;
        else
            $scope.priceGrid.data = $scope.alldata.filter((item) => { return item.Used = 1 });
    }

    //Проверяем, что расчет не запущен, если запущен то блокируем форму пока расчет не закончен,
    //если не запущен то можно работать
    function GetCalcLock() {
        $scope.loading = $http({
            method: 'POST',
            url: '/PriceCurrent_v2/GetCalcLock'
        }).then(function (response) {
            $scope.form.IsRun = response.data;
            // $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.ALL);
        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });
    }

    //Посмотреть историю запуска рассчета
    function GetStatuses() {
        $scope.loading = $http({
            method: 'POST',
            url: '/PriceCurrent_v2/GetStatuses'
        }).then(function (response) {
            $scope.form.Status = response.data;
        }, function (response) {
            alert('Ошибка:\n' + JSON.stringify(response));
        });
    }

    $scope.update();

}