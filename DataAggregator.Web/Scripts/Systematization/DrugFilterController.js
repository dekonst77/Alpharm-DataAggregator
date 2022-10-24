angular
    .module('DataAggregatorModule')
    .controller('DrugFilterController', ['$scope', '$http', '$uibModalInstance', 'hotkeys', DrugFilterController]);

function DrugFilterController($scope, $http, $modalInstance, hotkeys) {

    hotkeys.bindTo($scope).add({
        combo: 'enter',
        description: 'Поиск',
        callback: function () {
            $scope.CheckOk();
        }
    });

    //Переменная выбрать всё
    $scope.checkedAll = false;

    function setBool(item) {
        item.IsChecked = $scope.checkedAll;
        
    }

    //Функция проставляет/снимает всем галки  в chekbox
    $scope.CheckAll = function() {
        if ($scope.checkedAll) {
            $scope.checkedAll = false;
            $scope.filter.DrugClearWorkStat.IsChecked = false;
            $scope.filter.UserWorkStat.forEach(setBool);
            $scope.filter.DataTypeStat.forEach(setBool);
        } else {
            $scope.checkedAll = true;
            $scope.filter.DrugClearWorkStat.IsChecked = true;
            $scope.filter.UserWorkStat.forEach(setBool);
            $scope.filter.DataTypeStat.forEach(setBool);
        }
    };

    $scope.CheckOk = function() {
        if ($scope.CanSearch())
            $scope.ok();
    };

    $scope.update = function () {
        //$scope.updateStatLockCount = 1; //чтобы сразу заблокировать кнопку
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/Systematization/UpdateStatistic/'
        }).then(function (response) {
            $scope.filter = response.data;
        }, function () {
            $scope.message = 'Unexpected Error';
        });
    };

    $scope.getUpdateStatLockCount = function() {
        //$scope.loading = $http({
        //    method: "POST",
        //    url: "/Systematization/getUpdateStatLockCount"
        //}).then(function (response) {
        //    $scope.updateStatLockCount = response.data;
        //}, function () {
        //    $scope.updateStatLockCount = 0;
        //});
    };

    $scope.isCollapsed_AF = true;

    $scope.collapse_AF = function() {
        $scope.isCollapsed_AF = !$scope.isCollapsed_AF;
    };

    $scope.isCollapsed_Category = true;

    $scope.collapse_Category = function () {
        $scope.isCollapsed_Category = !$scope.isCollapsed_Category;
    };

    $scope.isCollapsed_PrioritetStat = true;

    $scope.collapse_PrioritetStat = function () {
        $scope.isCollapsed_PrioritetStat = !$scope.isCollapsed_PrioritetStat;
    };

    $scope.isCollapsed_user = true;

    $scope.collapse_user = function () {
        $scope.isCollapsed_user = !$scope.isCollapsed_user;
    };

    $scope.ok = function() {
        $modalInstance.close($scope.filter);
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

    function filterIsChecked(value) {
        return value.IsChecked;
    }

    $scope.CanSearch = function() {
        if (!$scope.filter)
            return false;

        var block1 = $scope.filter.DrugClearWorkStat.IsChecked || $scope.filter.UserWorkStat.filter(filterIsChecked).length > 0 || $scope.filter.RobotStat.filter(filterIsChecked).length > 0;
        var block2 = $scope.filter.DataTypeStat.filter(filterIsChecked).length > 0;
        var block3 = !!$scope.filter.Additional && !!$scope.filter.Additional.DrugClearId && $scope.filter.Additional.DrugClearId.length > 0;
        var block4 = $scope.filter.CategoryStat.filter(filterIsChecked).length > 0;
        var block7 = $scope.filter.PrioritetStat.filter(filterIsChecked).length > 0;

        var block5 = !!$scope.filter.Additional && !!$scope.filter.Additional.GZ_code && $scope.filter.Additional.GZ_code.length > 0;
        var block6 = !!$scope.filter.Additional && !!$scope.filter.Additional.Text && $scope.filter.Additional.Text.length > 0;
        //Не особо нужный блок
        //var block4 = !!$scope.filter.DateStat && !!$scope.filter.Additional.DrugClearId && $scope.filter.Additional.DrugClearId.length > 0;

        return block1 && block2 || block3 || block4 || block5 || block6 || block7;
    };

    // загрузить обрабатываемые drugs
    function loadDrugFilterStatistic() {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/Systematization/GetDrugFilterStatistic/'
        }).then(function (response) {
            $scope.filter = response.data;
        }, function () {
            $scope.message = 'Unexpected Error';
        });
    }

    loadDrugFilterStatistic();

    $scope.getUpdateStatLockCount();
}