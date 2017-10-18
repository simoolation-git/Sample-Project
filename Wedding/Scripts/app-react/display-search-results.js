"use strict";

var DisplaySearchResultImage = React.createClass({
    displayName: "DisplaySearchResultImage",

    shouldComponentUpdate: function shouldComponentUpdate(nextProps, nextState) {
        if (nextState.displayVideo === true) {
            return true;
        }

        return false;
    },
    getInitialState: function getInitialState() {
        return { displayVideo: false };
    },
    handleClick: function handleClick(event) {
        this.setState({ displayVideo: true });
    },
    componentDidMount: function componentDidMount() {

        this.setState({ displayVideo: false });
    },
    getUrl: function getUrl() {
        return "PostedItems/" + this.props.searchResult.Id + "/" + this.props.searchResult.Slug;
    },
    getVideoUrl: function getVideoUrl() {
        return this.props.searchResult.VideoSourceUrl;
    },
    componentDidUpdate: function componentDidUpdate(_prevProps, _prevState) {

        this.refs.video.controls = true;
        this.refs.video.play();

        //React.findDOMNode(this.refs.video).load();
    },
    render: function render() {

        if (this.state.displayVideo === true) {
            return React.createElement(
                "div",
                { className: "posted-item-card-image" },
                React.createElement(
                    "a",
                    null,
                    React.createElement("span", { className: "card-img-custom", title: this.props.searchResult.Title, style: { backgroundImage: 'url(' + this.props.searchResult.PhotoUrl + ')' } }),
                    React.createElement("video", { src: this.getVideoUrl(), className: "posted-item-video", controls: "", ref: "video" })
                )
            );
        }

        return React.createElement(
            "div",
            { className: "posted-item-card-image" },
            React.createElement(
                "a",
                { onClick: this.handleClick },
                React.createElement("span", { className: "card-img-custom", title: this.props.searchResult.Title, style: { backgroundImage: 'url(' + this.props.searchResult.PhotoUrl + ')' } })
            )
        );
    }
});

var DisplaySearchResultContent = React.createClass({
    displayName: "DisplaySearchResultContent",

    shouldComponentUpdate: function shouldComponentUpdate(nextProps, nextState) {
        //return this.props.searchResult.Id !== this.props.searchResult.Id;
        //{Math.random()}
        return false;
    },
    handleClick: function handleClick(event) {
        //amplify.publish("displayPostMainPageModal");
    },
    getUrl: function getUrl() {
        return "PostedItems/" + this.props.searchResult.Id + "/" + this.props.searchResult.Slug;
    },
    render: function render() {
        return React.createElement(
            "div",
            { className: "card-text-bottom" },
            React.createElement(
                "div",
                { className: "card-title" },
                React.createElement(
                    "a",
                    { href: this.getUrl() },
                    React.createElement(
                        "span",
                        { id: "title" },
                        this.props.searchResult.Title
                    )
                )
            )
        );
    }
});

var DisplaySearchResultLikeDislike = React.createClass({
    displayName: "DisplaySearchResultLikeDislike",

    getInitialState: function getInitialState() {
        return { searchResult: this.props.searchResult };
    },
    reCalculateTotal: function reCalculateTotal(searchResult, newIsLiked) {

        if (newIsLiked === true && searchResult.IsLiked === false) {
            searchResult.TotalLike++;
            searchResult.TotalDislike--;
        } else if (newIsLiked === true && searchResult.IsLiked === null) {
            searchResult.TotalLike++;
        } else if (newIsLiked === false && searchResult.IsLiked === null) {
            searchResult.TotalDislike++;
        } else if (newIsLiked === false && searchResult.IsLiked === true) {
            searchResult.TotalDislike++;
            searchResult.TotalLike--;
        } else if (newIsLiked === null && searchResult.IsLiked === true && searchResult.TotalLike > 0) {
            searchResult.TotalLike--;
        } else if (newIsLiked === null && searchResult.IsLiked === false && searchResult.TotalDislike > 0) {
            searchResult.TotalDislike--;
        }

        searchResult.IsLiked = newIsLiked;

        this.setState({ searchResult: searchResult });

        amplify.publish("likeDislikeButtonPushed", searchResult);
    },
    handleNoClick: function handleNoClick(event) {
        //Dislike

        var searchResult = this.state.searchResult;
        var newIsLiked = searchResult.IsLiked === false ? null : false;
        this.reCalculateTotal(searchResult, newIsLiked);
    },
    handleYesClick: function handleYesClick(event) {
        //Like
        var searchResult = this.state.searchResult;
        var newIsLiked = searchResult.IsLiked === true ? null : true;
        this.reCalculateTotal(searchResult, newIsLiked);
    },
    shareOnFB: function shareOnFB(event) {
        var postedItemUrl = "PostedItems/" + this.state.searchResult.Id + "/" + this.state.searchResult.Slug;
        var hrefLink = this.props.baseAddress + "/" + postedItemUrl;
        var tags = "#" + this.state.searchResult.Tags.join(" #");
        amplify.publish('ShareFacebookClicked', { title: this.state.searchResult.Title, photoUrl: this.state.searchResult.PhotoUrl, link: hrefLink, tags: tags, videoUrl: this.state.searchResult.VideoSourceUrl });
    },
    handleClickOpenLoginModal: function handleClickOpenLoginModal(event) {
        amplify.publish("displayLoginRegisterModal");
    },
    isActiveLike: function isActiveLike(isLiked) {
        return 'mdi mdi-36px float-right cursor-pointer ' + (isLiked != null && isLiked === true ? 'mdi-heart' : 'mdi-heart-outline');
    },
    render: function render() {
        var that = this;
        var likeButton = null;
        var shareButton = null;
        var readMore = null;

        if (this.props.userAuthorized === true) {
            likeButton = React.createElement(
                "a",
                { onClick: that.handleYesClick },
                React.createElement("i", { className: this.isActiveLike(this.state.searchResult.IsLiked) })
            );
            shareButton = React.createElement(
                "a",
                { onClick: that.shareOnFB },
                React.createElement("i", { className: "mdi mdi-facebook mdi-36px cursor-pointer" })
            );
            var postedItemUrl = "PostedItems/" + this.state.searchResult.Id + "/" + this.state.searchResult.Slug;
            var hrefLink = this.props.baseAddress + "/" + postedItemUrl;
            readMore = React.createElement(
                "a",
                { href: hrefLink, title: this.state.searchResult.Slug, className: "more-text" },
                " READ "
            );

            return React.createElement(
                "div",
                { className: "card-text card-likeOrDislike-div" },
                React.createElement(
                    "div",
                    { className: "col-xs-4" },
                    shareButton
                ),
                React.createElement(
                    "div",
                    { className: "col-xs-4 more-div" },
                    readMore
                ),
                React.createElement(
                    "div",
                    { className: "col-xs-4" },
                    likeButton
                )
            );
        }

        likeButton = React.createElement(
            "a",
            { onClick: that.handleClickOpenLoginModal },
            React.createElement("i", { className: "mdi mdi-heart-outline mdi-36px float-right cursor-pointer" })
        );
        shareButton = React.createElement(
            "a",
            { onClick: that.shareOnFB },
            React.createElement("i", { className: "mdi mdi-facebook mdi-36px cursor-pointer" })
        );
        var postedItemUrl = "PostedItems/" + this.state.searchResult.Id + "/" + this.state.searchResult.Slug;
        var hrefLink = this.props.baseAddress + "/" + postedItemUrl;
        readMore = React.createElement(
            "a",
            { href: hrefLink, title: this.state.searchResult.Slug, className: "more-text" },
            " READ "
        );

        return React.createElement(
            "div",
            { className: "card-text card-likeOrDislike-div" },
            React.createElement(
                "div",
                { className: "col-xs-4" },
                shareButton
            ),
            React.createElement(
                "div",
                { className: "col-xs-4 more-div" },
                readMore
            ),
            React.createElement(
                "div",
                { className: "col-xs-4" },
                likeButton
            )
        );
    }
});

var DisplaySearchResult = React.createClass({
    displayName: "DisplaySearchResult",

    render: function render() {
        return React.createElement(
            "div",
            null,
            React.createElement(DisplaySearchResultImage, { searchResult: this.props.searchResult, userAuthorized: this.props.userAuthorized, baseAddress: this.props.baseAddress }),
            React.createElement(DisplaySearchResultLikeDislike, { searchResult: this.props.searchResult, userAuthorized: this.props.userAuthorized, baseAddress: this.props.baseAddress }),
            React.createElement(DisplaySearchResultContent, { searchResult: this.props.searchResult, userAuthorized: this.props.userAuthorized, baseAddress: this.props.baseAddress })
        );
    }
});

var DisplaySearchResults = React.createClass({
    displayName: "DisplaySearchResults",

    render: function render() {
        var that = this;
        var createItem = function createItem(item, i) {
            return React.createElement(
                "div",
                { className: "col-custom" },
                React.createElement(
                    "div",
                    { className: "card-custom", key: item.Id },
                    React.createElement(DisplaySearchResult, { searchResult: item, userAuthorized: that.props.userAuthorized, baseAddress: that.props.baseAddress })
                )
            );
        };

        if (!Array.isArray(this.props.searchResults)) return null;

        return React.createElement(
            "span",
            null,
            this.props.searchResults.map(createItem)
        );
    }
});

var DisplaySearchResultsWrapper = React.createClass({
    displayName: "DisplaySearchResultsWrapper",

    getInitialState: function getInitialState() {
        //return { searchResults: [{ ApplicationUserId: '', Id: '', Tags: [], Title: '', PhotoUrl: '' }] };
        return null;
    },
    onChange: function onChange(e) {},
    componentDidMount: function componentDidMount() {
        var that = this;

        amplify.subscribe("searchResults", function (newsearchResults, userAuthorized, baseAddress) {
            if (newsearchResults === null) return;

            that.setState({ searchResults: newsearchResults, userAuthorized: userAuthorized, baseAddress: baseAddress });
        });
    },
    render: function render() {

        if (this.state === null || this.state.searchResults === null) return null;

        return React.createElement(
            "div",
            { className: "card-deck" },
            React.createElement(DisplaySearchResults, { searchResults: this.state.searchResults, userAuthorized: this.state.userAuthorized, baseAddress: this.state.baseAddress })
        );
    }
});

ReactDOM.render(React.createElement(DisplaySearchResultsWrapper, null), document.getElementById('display-search-results-display'));

