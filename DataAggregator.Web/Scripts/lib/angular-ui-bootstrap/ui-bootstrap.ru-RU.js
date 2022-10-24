angular.module('ui.bootstrap').config([
    'uibDatepickerPopupConfig',
    function (uibDatepickerPopupConfig) {
        uibDatepickerPopupConfig.closeText = 'Закрыть';
        uibDatepickerPopupConfig.currentText = 'Сегодня';
        uibDatepickerPopupConfig.clearText = 'Очистить';
    }
]);

angular.module('ui.bootstrap').config([
    'uibDatepickerConfig',
    function (uibDatepickerConfig) {
        uibDatepickerConfig.startingDay = 1;
}]);