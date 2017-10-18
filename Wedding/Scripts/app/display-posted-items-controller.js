angular.element(document).ready(function () {


    angular.module('posted-items-display-module', []).controller('posted-items-display-controller', ['$scope', '$window', '$http', function ($scope, $window, $http) {

        $scope.posts = null;
        $scope.loaded = false;

        var getPostedItemsUrl = $window.getPostedItemsUrl;

        $http.get(getPostedItemsUrl).then(
            function (response) {
                // success callback
                if (response.data != null && response.data.status == "error") {
                    toastr.error(response.data.message, 'Oops!');
                    $scope.posts = null;
                }
                else {
                    $scope.loaded = true;
                    $scope.posts = response.data;
                }
            },
            function (response) {
                // failure callback

            }
        );


        $scope.openModal = function (post) {
            amplify.publish("displayExistingPostedItem", post);
        }

        amplify.subscribe("newPostedItemAdded", function (photoItem, newItem) {
            if (newItem) {
                $scope.posts.splice(0, 0, photoItem);
                $scope.$apply();
            } else {
                findPostedItemIndex(photoItem.Id, function (index) {
                    $scope.posts[index] = photoItem;
                    $scope.$apply();
                });
            }
        });

        var findPostedItemIndex = function (id, callback) {
            var index = 0;
            while (index <= $scope.posts.length) {
                if (id == $scope.posts[index].Id) {
                    callback(index);
                    break;
                }
                index++;
            }
        }

        amplify.subscribe("removePostedItem", function (id) {
            //Lets remove the item from list
            findPostedItemIndex(id, function (index) {
                $scope.posts.splice(index, 1);
                $scope.$apply();
            });
        });
    }]);

    var docElement = document.getElementById("posted-items-display");

    angular.bootstrap(docElement, ['posted-items-display-module']);


});

