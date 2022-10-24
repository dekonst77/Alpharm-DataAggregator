angular.module('DataAggregatorModule').config(['dynamicNumberStrategyProvider', function (dynamicNumberStrategyProvider) {
    dynamicNumberStrategyProvider.addStrategy('sum', {
        numInt: 50,
        numFract: 2,
        numSep: ',',
        numPos: true,
        numNeg: true,
        numRound: 'round',
        numThousand: true,
        numThousandSep: ' '
    });
}]);

angular.module('DataAggregatorModule').config(['dynamicNumberStrategyProvider', function (dynamicNumberStrategyProvider) {
    dynamicNumberStrategyProvider.addStrategy('int', {
        numInt: 50,
        numFract: 0,
        numPos: true,
        numNeg: false,
        numThousand: false
    });
}]);