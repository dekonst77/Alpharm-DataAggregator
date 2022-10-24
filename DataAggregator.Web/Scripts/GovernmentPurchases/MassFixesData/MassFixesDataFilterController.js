angular.module('DataAggregatorModule')
    .controller('MassFixesDataFilterController', ['messageBoxService', '$scope', '$http', '$uibModalInstance', 'dialogParams', MassFixesDataFilterController]);

function MassFixesDataFilterController(messageBoxService, $scope, $http, $modalInstance, dialogParams) {
    $scope.format = 'yyyy-MM-dd';

    $scope.federationSubjects = [];
    $scope.categoryNames = [];
    $scope.natureNames = [];
    $scope.fundingNames = [];

    $scope.filter = dialogParams.filter;

    $scope.loadFederationSubjects = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/MassFixesData/GetFederationSubjects'
        }).then(function (response) {
            $scope.federationSubjects = response.data;
        }, function () {
            messageBoxService.showError('Не удалось загрузить список "Регионов получателя"!');
        });
    };
    $scope.loadFederationSubjects();

    $scope.loadCategoryNames = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/MassFixesData/GetCategoryNames'
        }).then(function (response) {
            $scope.categoryNames = response.data;
        }, function () {
            messageBoxService.showError('Не удалось загрузить список "Категорий"!');
        });
    };
    $scope.loadCategoryNames();
    
    $scope.loadNatureNames = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/MassFixesData/GetNatureNames'
        }).then(function (response) {
            $scope.natureNames = response.data;
        }, function () {
            messageBoxService.showError('Не удалось загрузить список "Характеров"!');
        });
    };
    $scope.loadNatureNames();

    $scope.loadFundingNames = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/MassFixesData/GetFundingNames'
        }).then(function (response) {
            $scope.fundingNames = response.data;
        }, function () {
            messageBoxService.showError('Не удалось загрузить список "Источников финансирования"!');
        });
    };
    $scope.loadFundingNames();

    $scope.setDateBeginEnd = function () {
        if (typeof $scope.filter.DateBeginStart.Value !== 'undefined' &&
            $scope.filter.DateBeginStart.Value !== null &&
            !$scope.filter.DateBeginStart.Value.$invalid &&
            $scope.filter.DateBeginEnd.Value === null) {
            $scope.filter.DateBeginEnd.setTodayWithoutTime();
        }
    }

    $scope.setDateEndEnd = function () {
        if (typeof $scope.filter.DateEndStart.Value !== 'undefined' &&
            $scope.filter.DateEndStart.Value !== null &&
            !$scope.filter.DateEndStart.Value.$invalid &&
            $scope.filter.DateEndEnd.Value === null) {
            $scope.filter.DateEndEnd.setTodayWithoutTime();
        }
    }

    $scope.setDateValueNullIfEmpty = function (event) {
        if (event.target.value === '') {
            $scope.filter[event.target.name].Value = null;
        }
    };

    $scope.ok = function () {
        $modalInstance.close({
            filter: $scope.filter
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
}