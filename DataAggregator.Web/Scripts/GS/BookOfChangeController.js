angular
    .module('DataAggregatorModule')
    .controller('BookOfChangeController', ['$scope', '$http', '$uibModal', '$interval', '$timeout', '$q', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', 'Upload', 'cfpLoadingBar', 'messageBoxService', BookOfChangeController]);

function BookOfChangeController($scope, $http, $uibModal, $interval, $timeout, $q, uiGridCustomService, errorHandlerService, formatConstants, Upload, cfpLoadingBar, messageBoxService) {

    $scope.setTabIndex = function (index) {
        $scope.currentTabIndex = index;
    };

    $scope.FormingTransactionDataGridOptions = uiGridCustomService.createOptions('FormingTransactionGrid');
    $scope.RebrandingDataGridOptions = uiGridCustomService.createOptions('RebrandingGrid');
    $scope.Network2DataGridOptions = uiGridCustomService.createOptions('Network2Grid');
    $scope.AsnaDataGridOptions = uiGridCustomService.createOptions('AsnaGrid');

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
    angular.extend($scope.Network2DataGridOptions, options);
    angular.extend($scope.AsnaDataGridOptions, options);

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

    $scope.Network2DataGridOptions.columnDefs = [
        {
            displayName: 'XlsId',
            field: 'XlsId',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'PharmacyId',
            field: 'PharmacyId',
            width: 300,
            type: 'string'
        },
        {
            displayName: 'EntityINN',
            field: 'EntityINN',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'EntityName',
            field: 'EntityName',
            width: 300,
            type: 'string'
        },
        {
            displayName: 'NetworkName',
            field: 'NetworkName',
            width: 300,
            type: 'string'
        },
        {
            displayName: 'Комментарий (сеть2)',
            field: 'Comment',
            width: 400,
            type: 'string'
        }
    ];

    $scope.AsnaDataGridOptions.columnDefs = [
        {
            displayName: 'XlsId',
            field: 'XlsId',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'PharmacyId',
            field: 'PharmacyId',
            width: 300,
            type: 'string'
        },
        {
            displayName: 'АСНА',
            field: 'ASNA',
            width: 120,
            type: 'string'
        },
        {
            displayName: 'NetworkName',
            field: 'NetworkName',
            width: 300,
            type: 'string'
        }
    ];

    $scope.periods = [];
    $scope.currentperiod = { myModel: null};

    //Инициализация
    function init() {

        var bookOfChange = $http({
            method: "POST",
            url: "/GS/BookOfChange_Init"
        }).then(function (response) {
            $scope.FormingTransactionDataGridOptions.data = response.data.Data
            $scope.RebrandingDataGridOptions.data = response.data.Data2
        });

        var networkAsna = $http({
            method: "POST",
            url: "/GS/Network2Asna_Init"
        }).then(function (response) {
            $scope.Network2DataGridOptions.data = response.data.Data
            $scope.AsnaDataGridOptions.data = response.data.Data2
            $scope.periods = response.data.Data3
            $scope.currentperiod.myModel = $scope.periods[0];
        });

        $scope.dataLoading = $q.all([bookOfChange, networkAsna])
    }

    init();

    $scope.BookOfChange_from_Excel = function (file) {

        if (file == null)
            return;

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
            if (progressPercentage >= 100) {
                $scope.message = "Обработка файла"
            }
        })
     
        $scope.dataLoading = $q.all([upload])
            .then(function () {
                init()
            });
    };

    $scope.BookOfChange_reloadQlik = function (file) {

        messageBoxService.showConfirm('Вы уверены, что хотите выгрузить книгу перемен в Qlik sense?', 'Экспорт в Qlik')
            .then(function () {
                cfpLoadingBar.start();
                $scope.loading = $http({
                    method: "POST",
                    url: "/GS/BookOfChange_reloadQlik"
                }).then(function (response) {
                    messageBoxService.showInfo('Успешно', 'Экспорт в Qlik');
                    cfpLoadingBar.set(1);
                    cfpLoadingBar.complete();
                }, function (error) {
                    cfpLoadingBar.complete();
                    messageBoxService.showError(error.data.message);
                });
            }, function () { });
    };

    $scope.Network2Asna_from_Excel = function (file) {

        if (file == null)
            return;

        cfpLoadingBar.start();

        var upload = Upload.upload({
            url: '/GS/Network2Asna_from_Excel/',
            data: {
                uploads: file,
                periodKey: $scope.currentperiod.myModel
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
            if (progressPercentage >= 100) {
                $scope.message = "Обработка файла"
            }
        })

        $scope.dataLoading = $q.all([upload])
            .then(function () {
                init()
            });
    };

    $scope.Network2Asna_reloadQlik = function (file) {

        messageBoxService.showConfirm('Вы уверены, что хотите выгрузить Сеть2/Асна в Qlik sense?', 'Экспорт в Qlik')
            .then(function () {
                cfpLoadingBar.start();
                $scope.loading = $http({
                    method: "POST",
                    url: "/GS/Network2Asna_reloadQlik"
                }).then(function (response) {
                    messageBoxService.showInfo('Успешно', 'Экспорт в Qlik');
                    cfpLoadingBar.set(1);
                    cfpLoadingBar.complete();
                }, function (error) {
                    cfpLoadingBar.complete();
                    messageBoxService.showError(error.data.message);
                });
            }, function () { });
    };
  
}
















