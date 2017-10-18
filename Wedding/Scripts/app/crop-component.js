angular.element(document).ready(function () {

    angular.module('crop-module-component', []).directive('croptool', function () {
        return {
            restrict: 'E',
            scope: {},
            templateUrl: '/Scripts/partials/crop.html',
            controller: function ($scope, $http, $attrs, $window) {

                var addphotoUrl = $window.addphotoUrl;
                var deletephotoUrl = $window.deletephotoUrl;

                var antiForgeryToken = $('[name=__RequestVerificationToken]').val();
                amplify.subscribe("refreshToken", function (token) {
                    antiForgeryToken = token;
                });
               
                $scope.dataToSave = {
                    photodata: null,
                    title: "",
                    Tags: [],
                    PostedId: null
                };

                $scope.configuration = {
                    sourceId: "inputSourceId",
                    targetDisplayId: "target",
                    defaultUrl: "Images/plus-300-300.png"
                };

                $scope.hideImage = true;

                var imageElement = null;

                $scope.selectFile = function () {
                    amplify.publish('errors', null);

                    var targetElement = document.getElementById($scope.configuration.sourceId);
                    angular.element(targetElement).click();
                }

                function showImage(src, target) {
                    var fr = new FileReader();

                    // when image is loaded, set the src of the image where you want to display it
                    fr.onload = function (e) {
                        target.src = this.result;
                    };

                    fr.onloadend = function () {

                        if (fr.result != null && fr.result.length > 0)
                            $scope.dataToSave.photodata = fr.result.split("base64,")[1];

                        $(target).imageScale({
                            didScale: function (firstTime, options) {
                                imageElement.show();
                            }
                        });

                        $scope.hideImage = false;
                    };

                    fr.readAsDataURL(src.files[0]);
                }

                $scope.fileNameChanged = function () {
                    var src = document.getElementById($scope.configuration.sourceId);
                    var target = document.getElementById($scope.configuration.targetDisplayId);
                    imageElement = angular.element(target);
                    imageElement.hide();
                    showImage(src, target);
                }

                var modal = $('#NewPostModal');
                var deleteButton = $("#NewPostModal #deletePostedItem");

                var saveButton = $("#NewPostModal #savePostedItem");
                var saveButtonLabel = $("#NewPostModal #savePostedItem #saveLabel");
                var savingButtonLabel = $("#NewPostModal #savePostedItem #savingLabel");

                function saving() {
                    saveButton.prop('disabled', true);
                    saveButtonLabel.hide();
                    savingButtonLabel.show();
                }

                function saved() {
                    saveButton.prop('disabled', false);
                    saveButtonLabel.show();
                    savingButtonLabel.hide();
                }

                amplify.subscribe("saveImageToCloud", function () {

                    saving();

                    var errors = [];

                    if ($scope.dataToSave.PostedId == 0 && ($scope.dataToSave.photodata == null || $scope.dataToSave.photodata.length == 0))
                        errors.push("Please add an image");

                    if ($scope.dataToSave.title == null || $scope.dataToSave.title.length == 0)
                        errors.push("Please add a title");

                    if (errors.length > 0) {
                        amplify.publish('errors', errors);
                    } else {

                        var req = {
                            method: 'POST',
                            url: addphotoUrl,
                            headers: {
                                '__RequestVerificationToken': antiForgeryToken
                            },
                            data: JSON.stringify($scope.dataToSave)
                        }

                        var saveResponsePromise = $http(req);

                        saveResponsePromise.success(function (response, status, headers, config) {
                            saved();
                            if (response.status != null && response.status == "error") {
                                toastr.error(response.message, 'Oops!');

                                if (response.errors != undefined && response.errors.length > 0) {
                                    angular.forEach(response.errors, function (value, index) {
                                        errors.push(value.Value);
                                    });
                                    if (errors.length > 0) {
                                        amplify.publish('errors', errors);
                                    }
                                }
                            }
                            else {
                                toastr.success('New Post Added', '');

                                if (modal != null)
                                    modal.modal('hide');

                                amplify.publish('newPostedItemAdded', response, $scope.dataToSave.PostedId == null);
                            };
                        });


                        saveResponsePromise.error(function (response, status, headers, config) {
                            saved();
                            toastr.error("Couldn't save the post.", '');
                        });

                    }
                });

                amplify.subscribe("openingNewPostModal", function (tags) {
                    //Lets reset everything
                    $scope.dataToSave = {
                        photodata: null,
                        title: "",
                        Tags: [],
                        PostedId: null
                    };

                    //clear all the tags
                    amplify.publish('clearTags');

                    //reset title
                    var title = $('textarea#Title');
                    if (title.length > 0)
                        title.val("");

                    //reset photo picker     
                    var target = $("#" + $scope.configuration.targetDisplayId);
                    $(target).attr('src', 'Images/plus-300-300.png');

                    target.imageScale({
                        didScale: function (firstTime, options) {
                            target.show();
                        }
                    });

                    //hide delete button
                    if (deleteButton != null && deleteButton.length > 0) {
                        deleteButton.hide();
                    }

                });

                amplify.subscribe("tags", function (tags) {
                    $scope.dataToSave.Tags = tags;
                });

                amplify.subscribe("titleChanged", function (title) {
                    amplify.publish('errors', null);
                    $scope.dataToSave.title = title;
                });

                amplify.subscribe("displayExistingPostedItem", function (post) {

                    if (post == null)
                        return;

                    if (deleteButton != null && deleteButton.length > 0) {
                        deleteButton.show();
                    }

                    $scope.dataToSave.photodata = null;
                    $scope.dataToSave.title = post.Title;
                    $scope.dataToSave.PostedId = post.Id;

                    $scope.dataToSave.Tags = [];
                    var tags = '';
                    angular.forEach(post.Tags, function (value, index) {
                        $scope.dataToSave.Tags.push(value.Name);
                        tags = tags + ' ' + value.Name;
                    });

                    //set current tags
                    amplify.publish("setTags", $scope.dataToSave.Tags, tags);

                    //reset title
                    var title = $('textarea#Title');
                    if (title.length > 0)
                        title.val($scope.dataToSave.title);

                    //reset photo picker     
                    var target = $("#" + $scope.configuration.targetDisplayId);
                    target.attr('src', post.PhotoUrl);
                    target.hide();

                    target.imageScale({
                        didScale: function (firstTime, options) {
                            target.show();
                        }
                    });

                    if (modal != null)
                        modal.modal('show');
                });

                amplify.subscribe("deletePostedItem", function () {
                    var req = {
                        method: 'POST',
                        url: deletephotoUrl,
                        headers: {
                            '__RequestVerificationToken': antiForgeryToken
                        },
                        data: JSON.stringify({ 'id': $scope.dataToSave.PostedId })
                    }

                    var deleteResponsePromise = $http(req);

                    deleteResponsePromise.success(function (response, status, headers, config) {
                        if (response.status != null && response.status == "error") {
                            toastr.error(response.message, 'Oops!');
                        }
                        else {
                            amplify.publish("removePostedItem", $scope.dataToSave.PostedId);
                            if (modal != null)
                                modal.modal('hide');
                        }
                    });


                    deleteResponsePromise.error(function (response, status, headers, config) {
                        toastr.error("Couldn't delete the post.", '');
                    });

                });
            }
        }
    });


    var docElement = document.getElementById("crop-module");

    angular.bootstrap(docElement, ['crop-module-component']);


});