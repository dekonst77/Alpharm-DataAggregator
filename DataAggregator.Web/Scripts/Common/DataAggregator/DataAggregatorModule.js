﻿angular.module('DataAggregatorModule',
    [
        'ngRoute',
        'pascalprecht.translate',
        'cgBusy',
        'ngAnimate',
        'ui.bootstrap',
        'ui.grid.module',
        'ui.tree',
        'cfp.hotkeys',
        'ngContextMenu',
        'ngRightClick',
        'ngFileUpload',
        'datetime',
        'ngSanitize',
        'dynamicNumber',
        'long2know',
        'ui.select',
        'ui.select.pagination.groups',
        'angularjs-dropdown-multiselect',
        'angular-loading-bar',
        'toaster'
    ]).config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
        cfpLoadingBarProvider.includeSpinner = false;
    }])