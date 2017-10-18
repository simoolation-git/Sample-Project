$(document).ready(function () {

    var saveLikeDislikeUrl = window.saveLikeDislikeUrl;
    var refreshPostedItemPartialUrl = window.refreshPostedItemPartialUrl;

    var antiForgeryToken = $('[name=__RequestVerificationToken]').val();
    amplify.subscribe("refreshToken", function (token) {
        antiForgeryToken = token;
    });

    amplify.subscribe("DisplayVideo", function (id, url) {
        var divContainer = $('#posted-item-card-' + id);

        if (divContainer.length > 0) {
            var image = divContainer.find('.posted-item-card-image');
            if (image.length > 0) {
                var video = image.find('.posted-item-video');
                if (video.length === 0) {
                    if (url !== undefined && url !== null && url.length > 0) {

                        var videoElement = document.createElement('video');
                        videoElement.src = url;
                        //videoElement.setAttribute('style', 'width: 500px; height:500px; position:absolute; top:0; left:0');
                        videoElement.setAttribute("class", "posted-item-video");

                        image.append(videoElement);
                        videoElement.controls = true;
                        videoElement.play();
                    }
                }
            }
        }


    });


    amplify.subscribe("LikeButtonIsClicked", function (currentLike, id) {
        var isLiked = currentLike === true ? null : true;

        if (!currentLike) {
            $("#like-button-" + id).removeClass('mdi-heart-outline');
            $("#like-button-" + id).addClass('mdi-heart');
        }
        else {
            $("#like-button-" + id).removeClass('mdi-heart');
            $("#like-button-" + id).addClass('mdi-heart-outline');
        }
        saveAndRefresh(id, isLiked);
    });

    var saveAndRefresh = function (id, isLiked) {
        //$("#ajax-loading-spinner-" + id).addClass('ajax-loading-spinner');

        saveLikeDislike(id, isLiked, function (success) {
            if (success) {
                reFreshPostedItemCard(id, function () {
                    //$("#ajax-loading-spinner-" + id).removeClass('ajax-loading-spinner');
                });
            }
        });
    }

    var saveLikeDislike = function (id, newIsLiked, callback) {
        var dataToSave = { postedItemId: id, liked: newIsLiked };

        $.ajax({
            url: saveLikeDislikeUrl,
            type: 'POST',
            data: dataToSave,
            headers: {
                '__RequestVerificationToken': antiForgeryToken
            },
            dataType: 'json',
            success: function (data) {
                callback(true);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                callback(false);
            }
        });
    }

    var reFreshPostedItemCard = function (id, callback) {
        var divContainer = $('#posted-item-card-' + id);

        var refreshUrl = refreshPostedItemPartialUrl;

        if (refreshPostedItemPartialUrl.indexOf(id) === -1) {
            refreshUrl = refreshPostedItemPartialUrl + "/" + id.toString() + "/false";
        }
        else {
            refreshUrl += "/true";
        }

        $.ajax({
            url: refreshUrl,
            dataType: 'html',
            success: function (html) {
                divContainer.replaceWith(html);
                callback();
            }
        });
    }

});
