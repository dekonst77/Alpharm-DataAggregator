(function () {
    angular.module('ui.grid').config([
        '$provide', function($provide) {
            $provide.decorator('i18nService',
                [
                    '$delegate', function($delegate) {

                        var extendedTranslations =
                        {
                            gridMenu:
                            {
                                exporterAllAsExcel: 'Экспортировать всё в Excel',
                                exporterVisibleAsExcel: 'Экспортировать видимые данные в Excel',
                                exporterSelectedAsExcel: 'Экспортировать выделенные данные в Excel',
                                clearAllFilters: 'Очистить все фильтры',
                                resetState: 'Сбросить состояние'
                            }
                        };

                        var originalTranslation = $delegate.get('ru');

                        angular.extend(originalTranslation.gridMenu, extendedTranslations.gridMenu);

                        return $delegate;
                    }
                ]);
        }
    ]);

    angular.module('ui.grid').config(['$provide', function ($provide) {
        $provide.decorator('GridOptions', ['$delegate', 'i18nService', function ($delegate, i18nService) {
            var gridOptions;
            gridOptions = angular.copy($delegate);
            gridOptions.initialize = function (options) {
                var initOptions;
                initOptions = $delegate.initialize(options);
                return initOptions;
            };

            i18nService.setCurrentLang('ru');
            return gridOptions;
        }]);
    }]);
})();
