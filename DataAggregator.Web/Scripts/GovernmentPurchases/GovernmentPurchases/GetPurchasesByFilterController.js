angular
    .module('DataAggregatorModule')
    .controller('GetPurchasesByFilterController', ['$scope', '$http', '$uibModalInstance', GetPurchasesByFilterController]);


function GetPurchasesByFilterController($scope, $http, $modalInstance) {

    function loadUsers() {
        $scope.purchasesFilterLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetUsers/'
        }).then(function (response) {
            var data = response.data;
            $scope.filter.lastChangedPurchaseUser.DictionaryData = data;
            $scope.filter.lastChangedLotUser.DictionaryData = data;
            $scope.filter.lastChangedObjectReadyUser.DictionaryData = data;
        },function () {
            $scope.message = 'Unexpected Error';
        });
    }
    loadUsers();

    function loadRegionFilter() {
        $scope.purchasesFilterLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetRegionFilter/'
        }).then(function (response) {
            var data = response.data;
            $scope.filter.city.DictionaryData = data.City;
            $scope.filter.district.DictionaryData = data.District;
            $scope.filter.federalDistrict.DictionaryData = data.FederalDistrict;
            $scope.filter.federationSubject.DictionaryData = data.FederationSubject;
        },function () {
            $scope.message = 'Unexpected Error';
        });
    }
    loadRegionFilter();

    function loadNatureList() {
        $scope.purchasesFilterLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetNatureList/'
        }).then(function (response) {
            var data = response.data;
            $scope.filter.nature.DictionaryData = data;
        },function () {
            $scope.message = 'Unexpected Error';
        });
    }
    loadNatureList();

    function loadFundingList() {
        $scope.purchasesFilterLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetFundingList/'
        }).then(function (response) {
            var data = response.data;
            $scope.filter.funding.DictionaryData = data;
        },function () {
            $scope.message = 'Unexpected Error';
        });
    }
    loadFundingList();

    function loadPurchaseClassList() {
        $scope.purchasesFilterLoading = $http({
            method: 'POST',
            url: '/GovernmentPurchases/GetPurchaseClassList/'
        }).then(function (response) {
            var data = response.data;
            $scope.filter.purchaseClass.DictionaryData = data;
            $scope.filter.purchaseClass.DictionaryData.unshift({ Id: null, Name: '' });
            $scope.filter.purchaseClass.selected = $scope.filter.purchaseClass.DictionaryData[0];
        },function () {
            $scope.message = 'Unexpected Error';
        });
    }

    loadPurchaseClassList();

    $scope.save = function () {
        $modalInstance.close($scope.filter.getJson());
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };


    //Фильтр

    var filterClass = function () {
        this.id = '';
        this.number = '';
        this.reestrnumber = '';
        this.lastChangedPurchaseUser = new dictTypeHead();
        this.lastChangedLotUser = new dictTypeHead();
        this.lastChangedObjectReadyUser = new dictTypeHead();
        this.city = new dictTypeHead();
        this.district = new dictTypeHead();
        this.federalDistrict = new dictTypeHead();
        this.federationSubject = new dictTypeHead();
        this.dateStart = new dateClass();
        this.dateEnd = new dateClass();
        this.name = '';
        this.nature = new dictTypeHead();
        this.funding = new dictTypeHead();
        this.purchaseClass = new dictTypeHead();

        this.clear = function () {
            this.id = '';
            this.number = '';
            this.reestrnumber = '';
            this.lastChangedPurchaseUser.clear();
            this.lastChangedLotUser.clear();
            this.lastChangedObjectReadyUser.clear();
            this.city.clear();
            this.district.clear();
            this.federalDistrict.clear();
            this.federationSubject.clear();
            this.dateStart.setNull();
            this.dateEnd.setNull();
            this.name = '';
            this.nature.clear();
            this.funding.clear();
            this.purchaseClass.clear();
        }

        this.validate = function () {
            return true;
        }

        this.getJson = function () {
            var filter = {
                Id: this.id,
                Number: this.number,
                ReestrNumber: this.reestrnumber,
                LastChangedPurchaseUser: this.lastChangedPurchaseUser.selected != null ? this.lastChangedPurchaseUser.selected.Id : null,
                LastChangedLotUser: this.lastChangedLotUser.selected != null ? this.lastChangedLotUser.selected.Id : null,
                LastChangedObjectReadyUser: this.lastChangedObjectReadyUser.selected != null ? this.lastChangedObjectReadyUser.selected.Id : null,
                City: this.city.selected != null ? this.city.selected : null,
                District: this.district.selected != null ? this.district.selected : null,
                FederalDistrict: this.federalDistrict.selected != null ? this.federalDistrict.selected : null,
                FederationSubject: this.federationSubject.selected != null ? this.federationSubject.selected : null,
                DateStart: this.dateStart.Value,
                DateEnd: this.dateEnd.Value,
                Name: this.name,
                NatureId: this.nature.selected != null ? this.nature.selected.Id : null,
                FundingId: this.funding.selected != null ? this.funding.selected.Id : null,
                PurchaseClassId: this.purchaseClass.selected != null ? this.purchaseClass.selected.Id : null
            }
            
            return JSON.stringify({ filter: filter });
        }
    }

    //Объект фильтр

    $scope.filter = new filterClass();

}