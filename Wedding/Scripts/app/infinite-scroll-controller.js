angular.element(document).ready(function () {


    var app = angular.module('infinite-scroll-module', ['infinite-scroll']).controller('infinite-scroll-controller', ['$scope', '$window', '$http', function ($scope, $window, $http) {

        $scope.loading = false;
        $scope.infinite_scrolling_disabled = true;
        var baseAddress = $window.baseAddress;
        var userAuthorized = $window.userAuthorized;

        amplify.subscribe("isAuthenticated", function (isAuthenticated) {
            userAuthorized = isAuthenticated;
        });

        var totalItems = 0;

        var antiForgeryToken = $('[name=__RequestVerificationToken]').val();
        amplify.subscribe("refreshToken", function (token) {
            antiForgeryToken = token;
        });

        var saveLikeDislike = $window.saveLikeDislike;


        amplify.subscribe("likeDislikeButtonPushed", function (searchResult) {

            var data = { postedItemId: searchResult.Id, liked: searchResult.IsLiked };

            var req = {
                method: 'POST',
                url: saveLikeDislike,
                headers: {
                    '__RequestVerificationToken': antiForgeryToken
                },
                data: JSON.stringify(data),
            };

            var saveResponsePromise = $http(req);

            saveResponsePromise.success(function (data, status, headers, config) {
            });

            saveResponsePromise.error(function (data, status, headers, config) {
            });
        });


        amplify.subscribe("searchResults", function (results, userAuthorized) {
            if (Array.isArray(results))
                totalItems = results.length;
        });

        amplify.subscribe("EnableInfiniteScrolling", function () {
            $scope.infinite_scrolling_disabled = false;
            $scope.$apply();
        });

        amplify.subscribe("DisableInfiniteScrolling", function () {
            $scope.infinite_scrolling_disabled = true;
            $scope.$apply();
        });

        $scope.nextPage = function () {

            //don't load if its already loading, wait until loading is completed
            if ($scope.loading === false) {
                $scope.loading = true;

                $http.get("/Search/NextPage/").success(function (results, status) {
                    $scope.loading = false;

                    var oldCount = totalItems;

                    amplify.publish("searchResults", results, userAuthorized, baseAddress);

                    //we should enable or disable the infinit scroll first and then publish the results
                    if (results.length === oldCount) {
                        $scope.infinite_scrolling_disabled = true;

                    }
                    else {
                        $scope.infinite_scrolling_disabled = false;
                        totalItems = results.length;
                    }

                });
            }
        }

    }]);

    app.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
    }]);

    var docElement = document.getElementById("infinite-scroll-display");

    angular.bootstrap(docElement, ['infinite-scroll-module']);


});