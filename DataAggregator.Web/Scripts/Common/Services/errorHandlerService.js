angular.module('DataAggregatorModule').service('errorHandlerService', ['messageBoxService', function (messageBoxService) {

    this.showResponseError = function(response) {
        var header = response.statusText;

        var message;
        if (response.data.message)
            message = response.data.message;
        else {
            //var msg = response.data.match(/[<]title[>]=(.*?)[<][/]title/g);
            message = response.data;
        }

        return messageBoxService.showError(message, header);
    };

}]);