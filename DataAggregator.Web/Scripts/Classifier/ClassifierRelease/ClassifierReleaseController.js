angular
    .module('DataAggregatorModule')
    .controller('ClassifierReleaseController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', 'errorHandlerService', ClassifierReleaseController]);

function ClassifierReleaseController($scope, $http, $uibModal, uiGridCustomService, formatConstants, errorHandlerService) {


    //$scope.CreateStableJobInfoStatus = "...";
    //$scope.LoadGoodsJobInfoStatus = "...";
    //$scope.CreateGoodsStableJobInfoStatus = "...";
    
    //var Init = function () {
    //    $scope.loading = $http({
    //        method: "POST",
    //        url: "/ClassifierRelease/Initialize"
    //    }).then(function (response) {

    //        $scope.CreateStableJobInfoStatus = response.data.CreateStableJobInfoStatus;
    //        $scope.LoadGoodsJobInfoStatus = response.data.LoadGoodsJobInfoStatus;
    //        $scope.CreateGoodsStableJobInfoStatus = response.data.CreateGoodsStableJobInfoStatus;

    //    }, function (response) {
    //        errorHandlerService.showResponseError(response);
    //    });
    //}

    //Init();

    //$scope.CreateGoodsStableJobInfo = function () {
    //    OpenInfo('CreateGoodsStableJob');
    //}

    //$scope.CreateStableJobInfo = function () {
    //    OpenInfo('CreateStableJob');
    //}

    //$scope.LoadGoodsJobInfo = function () {
    //    OpenInfo('LoadGoodsJob');      
    //}  


    //function OpenInfo(jobName)
    //{
    //    var modalInstance = $uibModal.open({
    //        animation: true,
    //        templateUrl: 'Views/Classifier/ClassifierRelease/_JobInfo.html',
    //        controller: 'ClassifierJobInfoController',
    //        size: 'full',
    //        windowClass: 'center-modal',
    //        backdrop: 'static',
    //        resolve: {
    //            model: function () {
    //                return {
    //                    JobName: jobName
    //                };
    //            }
    //        }
    //    });

    //    modalInstance.result.then(function (value) {
    //        Init();

    //    },
    //    function (response) {
    //        errorHandlerService.showResponseError(response);
    //        Init();
    //    });
    //}

}
















