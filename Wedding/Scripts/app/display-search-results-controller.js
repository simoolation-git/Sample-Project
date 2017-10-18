angular.element(document).ready(function () {


    angular.module('display-search-results-module', []).controller('display-search-results-controller', ['$scope', '$window', '$http', function ($scope, $window, $http) {

        $scope.posts = null;
        $scope.loaded = false;

        $scope.searchResults = [];

        amplify.subscribe("searchResults", function (searchResults) {
            $scope.searchResults = searchResults;
            $scope.$apply();
        });
    }]);

    var docElement = document.getElementById("display-search-results-display");

    angular.bootstrap(docElement, ['display-search-results-module']);


});