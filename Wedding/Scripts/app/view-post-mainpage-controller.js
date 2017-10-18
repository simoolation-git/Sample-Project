angular.element(document).ready(function () {

    var app = angular.module('view-post-mainpage-module', []);

    app.controller('view-post-mainpage-controller', ['$scope', '$window', '$http', function ($scope, $window, $http) {

        $scope.searchResults = [];
        $scope.currentSearchResult = null;
        var searchResultIndex = 0;
        //var posted_item_modal = $('#postModal');
        //var posted_item_modal_for_facebook = document.getElementById('postModal');
        //var facebookButton = posted_item_modal.find('#social-icons-facebook');

        $scope.isUserGenerated = function () {
            if ($scope.currentSearchResult === null)
                return false;

            var isUserGenerated = (
                $scope.currentSearchResult.Source === null &&
                $scope.currentSearchResult.ApplicationUserId != null &&
                $scope.currentSearchResult.ApplicationUserId.length > 0);

            return isUserGenerated;
        }

        $scope.next = function () {
            if (searchResultIndex < $scope.searchResults.length - 1)
                searchResultIndex = searchResultIndex + 1;

            $scope.currentSearchResult = $scope.searchResults[searchResultIndex];
            setFacebookUrl();
        }

        $scope.previous = function () {
            if (searchResultIndex > 0)
                searchResultIndex = searchResultIndex - 1;

            $scope.currentSearchResult = $scope.searchResults[searchResultIndex];
            setFacebookUrl();
        }

        //amplify.subscribe("prepareDataForModal", function (searchResults, index) {
        //    searchResultIndex = index;

        //    $scope.$apply(function () {
        //        $scope.searchResults = searchResults;
        //        $scope.currentSearchResult = $scope.searchResults[index];
        //        setFacebookUrl();
        //    });
        //});

        function setFacebookUrl() {
            if ($scope.currentSearchResult != null) {
                if ($scope.currentSearchResult.facebookUrl === undefined || $scope.currentSearchResult.facebookUrl === null || $scope.currentSearchResult.facebookUrl.length === 0)
                    $scope.currentSearchResult.facebookUrl = "posteditems/" + $scope.currentSearchResult.Id + '/' + $scope.currentSearchResult.Slug;

                //facebookButton.attr('href', $scope.currentSearchResult.facebookUrl);
            }
            //FB.XFBML.parse(posted_item_modal_for_facebook);
        }

        $scope.shareFeedToFacebook = function () {
            FB.ui({
                method: 'feed',
                name: $scope.currentSearchResult.Title,
                link: posted_item_modal_for_facebook.baseURI + '/' + $scope.currentSearchResult.facebookUrl,
                picture: $scope.currentSearchResult.PhotoUrl,
                caption: '',
                description: '',
                message: ''
            });
        }


        //amplify.subscribe("displayPostMainPageModal", function () {
        //    posted_item_modal.modal('show');
        //});


        $('.social-icons a').tooltip();
    }]);

    app.directive('ngRight', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 39) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngRight);
                    });

                    event.preventDefault();
                }
            });
        };
    });

    app.directive('ngLeft', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 37) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngLeft);
                    });

                    event.preventDefault();
                }
            });
        };
    });

    var docElement = document.getElementById("postModal");

    angular.bootstrap(docElement, ['view-post-mainpage-module']);
});

