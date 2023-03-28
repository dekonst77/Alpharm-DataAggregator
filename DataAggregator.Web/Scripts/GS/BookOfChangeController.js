angular
    .module('DataAggregatorModule')
    .controller('BookOfChangeController', ['$scope', '$http', '$uibModal', '$interval', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', 'Upload', 'cfpLoadingBar', BookOfChangeController]);

function BookOfChangeController($scope, $http, $uibModal, $interval, $timeout, uiGridCustomService, errorHandlerService, formatConstants, Upload, cfpLoadingBar) {

    $scope.setTabIndex = function (index) {
        $scope.currentTabIndex = index;
    };

    $scope.FormingTransactionDataGridOptions = uiGridCustomService.createOptions('FormingTransactionGrid');
    $scope.RebrandingDataGridOptions = uiGridCustomService.createOptions('RebrandingGrid');

    var options = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 30,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: false,
        noUnselect: false
    };

    angular.extend($scope.FormingTransactionDataGridOptions, options);
    angular.extend($scope.RebrandingDataGridOptions, options);

    $scope.FormingTransactionDataGridOptions.columnDefs = [
        {
            displayName: 'GS.BookOfChange.FormingTransaction.CodeAS1',
            field: 'CodeAS1',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.Who',
            field: 'Who',
            width: 300,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.CodeAS2',
            field: 'CodeAS2',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.Whom',
            field: 'Whom',
            width: 300,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.Region',
            field: 'Region',
            width: 200,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.WhenPeriod',
            field: 'WhenPeriod',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.CountAS',
            field: 'CountAS',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.Comment',
            field: 'Comment',
            width: 400,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.Show',
            field: 'Show',
            width: 140,
            type: 'string'
        },
    ];

    $scope.RebrandingDataGridOptions.columnDefs = [
        {
            displayName: 'GS.BookOfChange.Rebranding.CodeAS1',
            field: 'CodeAS1',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.Rebranding.PrevName',
            field: 'PrevName',
            width: 300,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.Rebranding.CodeAS2',
            field: 'CodeAS2',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.Rebranding.CurrentName',
            field: 'CurrentName',
            width: 300,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.Rebranding.WhenPeriod',
            field: 'WhenPeriod',
            width: 200,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.Rebranding.Comment',
            field: 'Comment',
            width: 400,
            type: 'string'
        },
        {
            displayName: 'GS.BookOfChange.FormingTransaction.Show',
            field: 'Show',
            width: 140,
            type: 'string'
        },
    ];

    //Инициализация
    function init() {

        $scope.loading = $http({
            method: "POST",
            url: "/GS/BookOfChange_Init"
        }).then(function (response) {
            $scope.FormingTransactionDataGridOptions.data = response.data.Data
            $scope.RebrandingDataGridOptions.data = response.data.Data2
        });
    }

    init();

    $scope.BookOfChange_from_Excel = function (file) {

        cfpLoadingBar.start();

        var upload = Upload.upload({
            url: '/GS/BookOfChange_from_Excel/',
            data: {
                uploads: file
            }
        }).then(function (resp) {
            console.log('Success ' + resp.config.data.uploads.name + 'uploaded. Response: ' + resp.data);
            cfpLoadingBar.complete();
            $scope.message = "Загрузка"
        }, function (resp) {
            errorHandlerService.showResponseError(resp);
            cfpLoadingBar.complete();
            $scope.message = "Загрузка"
        }, function (evt) {
            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            cfpLoadingBar.set(progressPercentage / 100);
            $scope.message = "Загрузка файла " + progressPercentage + "%"
            if (progressPercentage >= 100)
                $scope.message = "Обработка файла"
        })
        .then(function () {
            init()
        });

        $scope.dataLoading = $q.all([upload])
    };
  
}
















