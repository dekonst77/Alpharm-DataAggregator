(function() {
    var contextMenuModule = angular.module('contextMenuModule', ['ui.bootstrap']);

    contextMenuModule.directive('showRowContextMenu', showRowContextMenu);

    function showRowContextMenu() {
        return {
            link: function(scope, element, attrs) {

                element.closest('tr').bind('mouseenter',
                    function() {
                        element.show();
                    });
                element.closest('tr').bind('mouseleave',
                    function() {
                        element.hide();

                        var contextmenu = element.find('#contextmenu');
                        contextmenu.click();

                        element.parent().removeClass('open');

                    });
            }
        };
    };
})();