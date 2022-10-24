angular.module('DataAggregatorModule').service('commonService',
    function() {
        this.getSelectionText = function() {
            var text = '';
            if (window.getSelection) {
                text = window.getSelection().toString();
            } else if (document.selection && document.selection.type !== 'Control') {
                text = document.selection.createRange().text;
            }
            return text;
        };


    });