(function() {
    angular.module('cgBusy').value('cgBusyDefaults', {
        message: 'Загрузка',
        backdrop: true,
        templateUrl: 'Views/Static/Busy.html',
        delay: 1000,
        minDuration: 0
    });
})();
