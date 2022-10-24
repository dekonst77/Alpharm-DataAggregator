angular
    .module('DataAggregatorModule')
    .controller('AveragePriceFilterController', ['$scope', '$http', '$uibModalInstance', 'dialogParams', AveragePriceFilterController]);

function AveragePriceFilterController($scope, $http, $modalInstance, dialogParams) {

    $scope.regions = {
        federalDistricts: [],
        federationSubjects: [],
        districts: [],
        cities: []
    };

    $scope.selectedRegion = dialogParams.selectedRegion;

    $scope.selectedDrug = dialogParams.selectedDrug;

    $scope.filter = {
        date: dialogParams.selectedPeriodDate
    };

    $scope.loadFederalDistricts = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/AveragePrice/GetRegionNames',
            data: { parentId: null }
        }).then(function (response) {
            $scope.regions.federalDistricts = response.data;
        }, function () {
            alert(':(');
        });
    };

    $scope.loadFederationSubjects = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/AveragePrice/GetRegionNames',
            data: { parentId: $scope.selectedRegion.FederalDistrictId }
        }).then(function (response) {
            $scope.regions.federationSubjects = response.data;
        }, function () {
            alert(':(');
        });
    };

    $scope.loadDistricts = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/AveragePrice/GetRegionNames',
            data: { parentId: $scope.selectedRegion.FederationSubjectId }
        }).then(function (response) {
            $scope.regions.districts = response.data;
        }, function () {
            alert(':(');
        });
    };

    $scope.loadCities = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/AveragePrice/GetRegionNames',
            data: { parentId: $scope.selectedRegion.DistrictId }
        }).then(function (response) {
            $scope.regions.cities = response.data;
        }, function () {
            alert(':(');
        });
    };

    $scope.searchDictionary = function(value, dictionary) {
        return $http({
            method: 'POST',
            url: '/Dictionary/GetDictionary/',
            data: JSON.stringify({ Value: value, Dictionary: dictionary })
        }).then(function(response) {
            return response.data;
        });
    };

    $scope.federalDistrictSelected = function (region) {
        $scope.selectedRegion.FederalDistrictId = region.Id;
        $scope.selectedRegion.SelectedRegionId = region.Id;

        $scope.selectedRegion.FederationSubject = null;
        $scope.selectedRegion.FederationSubjectId = null;
        $scope.selectedRegion.District = null;
        $scope.selectedRegion.DistrictId = null;
        $scope.selectedRegion.City = null;
        $scope.selectedRegion.CityId = null;

        $scope.loadFederationSubjects();
    };

    $scope.federationSubjectSelected = function (region) {
        $scope.selectedRegion.FederationSubjectId = region.Id;
        $scope.selectedRegion.SelectedRegionId = region.Id;

        $scope.selectedRegion.District = null;
        $scope.selectedRegion.DistrictId = null;
        $scope.selectedRegion.City = null;
        $scope.selectedRegion.CityId = null;

        $scope.loadDistricts();
    };

    $scope.districtSelected = function (region) {
        $scope.selectedRegion.DistrictId = region.Id;
        $scope.selectedRegion.SelectedRegionId = region.Id;

        $scope.selectedRegion.City = null;
        $scope.selectedRegion.CityId = null;

        $scope.loadCities();
    };

    $scope.citySelected = function (region) {
        $scope.selectedRegion.CityId = region.Id;
        $scope.selectedRegion.SelectedRegionId = region.Id;
    };

    $scope.drugSelected = function (drug) {
        $scope.selectedDrug.DrugId = drug.Id;
    };

    $scope.packerSelected = function (packer) {
        $scope.selectedDrug.PackerId = packer.Id;
    };

    $scope.ownerTradeMarkSelected = function (ownerTradeMark) {
        $scope.selectedDrug.OwnerTradeMarkId = ownerTradeMark.Id;
    };

    $scope.federalDistrictChanged = function () {
        $scope.selectedRegion.SelectedRegionId = null;
        $scope.selectedRegion.FederalDistrictId = null;
        $scope.selectedRegion.FederationSubject = null;
        $scope.selectedRegion.FederationSubjectId = null;
        $scope.selectedRegion.District = null;
        $scope.selectedRegion.DistrictId = null;
        $scope.selectedRegion.City = null;
        $scope.selectedRegion.CityId = null;
    };

    $scope.federationSubjectChanged = function () {
        $scope.selectedRegion.SelectedRegionId = null;
        $scope.selectedRegion.FederationSubjectId = null;
        $scope.selectedRegion.District = null;
        $scope.selectedRegion.DistrictId = null;
        $scope.selectedRegion.City = null;
        $scope.selectedRegion.CityId = null;
    };

    $scope.districtChanged = function () {
        $scope.selectedRegion.SelectedRegionId = null;
        $scope.selectedRegion.DistrictId = null;
        $scope.selectedRegion.City = null;
        $scope.selectedRegion.CityId = null;
    };

    $scope.cityChanged = function () {
        $scope.selectedRegion.SelectedRegionId = null;
        $scope.selectedRegion.CityId = null;
    };

    $scope.drugChanged = function () {
        $scope.selectedDrug.DrugId = null;
    };

    $scope.packerChanged = function () {
        $scope.selectedDrug.PackerId = null;
    };

    $scope.ownerTradeMarkChanged = function () {
        $scope.selectedDrug.OwnerTradeMarkId = null;
    };

    $scope.ok = function () {
        $modalInstance.close({
            selectedRegion: $scope.selectedRegion,
            selectedDrug: $scope.selectedDrug,
            selectedPeriodDate: $scope.filter.date
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };

    $scope.loadFederalDistricts();
}