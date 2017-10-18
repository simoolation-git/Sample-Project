angular.element(document).ready(function () {

    var app = angular.module('search-module-component', []).directive('searchbar', function () {
        return {
            restrict: 'E',
            scope: {},
            templateUrl: '/Scripts/partials/search.html',
            controller: function ($scope, $http, $attrs, $window) {

                var serverGeneratedPostedItemsWrapper = $('div#serverGeneratedPostedItemsWrapper.card-deck-wrapper');
                var zenzoyCarousel = $('div#zenzoyCarousel');

                var userAuthorized = $window.userAuthorized;
                amplify.subscribe("isAuthenticated", function (isAuthenticated) {
                    userAuthorized = isAuthenticated;
                });

                $scope.editMode = true;
                $scope.textEntered = "";
                $scope.tags = [];
                var baseAddress = $window.baseAddress;

                $scope.startSearch = function (termTyped) {

                    if (termTyped.length > 0) {
                        serverGeneratedPostedItemsWrapper.hide();
                        zenzoyCarousel.hide();
                    }
                    else {
                        serverGeneratedPostedItemsWrapper.show();
                        zenzoyCarousel.show();
                    }

                    amplify.publish("DisableInfiniteScrolling", termTyped);

                    $http.post("/Search/FindPost/", { term: termTyped }).success(function (data, status) {
                        amplify.publish("searchResults", data, userAuthorized, baseAddress);

                        amplify.publish("EnableInfiniteScrolling");
                    });

                }


                $scope.DisplayTextBox = function () {
                    $scope.editMode = true;
                    var textBox = $('.search-wrapper .search-bar');
                    textBox.focus();
                    setTimeout(function () {
                        textBox.focus();
                    }, 1);
                }

                $scope.ParseContent = function () {
                    ConvertToTags();
                }

                function ConvertToTags() {
                    if ($scope.textEntered !== null && $scope.textEntered) {
                        $scope.tags = $scope.textEntered.trim().split(" ");
                        $scope.editMode = false;
                        $scope.startSearch($scope.textEntered);
                    }
                    else
                        $scope.startSearch($scope.textEntered);
                }


                $scope.Clear = function () {
                    $scope.textEntered = "";
                    $scope.startSearch("");
                }

                $scope.startSearch("");
            }
        }
    });

    app.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
    }]);

    app.directive('ngEnter', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                    event.stopPropagation();
                    return false;
                }
            });
        };
    });

    var docElement = document.getElementById("search-module");

    if (docElement)
        angular.bootstrap(docElement, ['search-module-component']);


});