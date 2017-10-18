var DisplaySearchResultImage = React.createClass({
    shouldComponentUpdate: function (nextProps, nextState) {
        if (nextState.displayVideo === true) {
            return true;
        }

        return false;
    },
    getInitialState: function () {
        return { displayVideo: false };
    },
    handleClick: function (event) {
        this.setState({ displayVideo: true });
    },
    componentDidMount: function () {

        this.setState({ displayVideo: false });
    },
    getUrl: function () {
        return "PostedItems/" + this.props.searchResult.Id + "/" + this.props.searchResult.Slug;
    },
    getVideoUrl: function () {
        return this.props.searchResult.VideoSourceUrl;
    },
    componentDidUpdate(_prevProps, _prevState) {

        this.refs.video.controls = true;
        this.refs.video.play();

        //React.findDOMNode(this.refs.video).load();
    },
    render: function () {


        if (this.state.displayVideo === true) {
            return (
                <div className="posted-item-card-image">
                    <a>
                        <span className="card-img-custom" title={this.props.searchResult.Title} style={{ backgroundImage: 'url(' + this.props.searchResult.PhotoUrl + ')' }}>
                        </span>
                        <video src={this.getVideoUrl()} className="posted-item-video" controls="" ref="video"></video>
                    </a>
                </div>
                );
        }

        return (
            <div className="posted-item-card-image">
                <a onClick={this.handleClick}>
                 <span className="card-img-custom" title={this.props.searchResult.Title} style={{ backgroundImage: 'url(' + this.props.searchResult.PhotoUrl + ')' }}>
                 </span>
                </a>
            </div>
        );
    }
});

var DisplaySearchResultContent = React.createClass({
    shouldComponentUpdate: function (nextProps, nextState) {
        //return this.props.searchResult.Id !== this.props.searchResult.Id;
        //{Math.random()}
        return false;
    },
    handleClick: function (event) {
        //amplify.publish("displayPostMainPageModal");
    },
    getUrl: function () {
        return "PostedItems/" + this.props.searchResult.Id + "/" + this.props.searchResult.Slug;
    },
    render: function () {
        return (
            <div className="card-text-bottom">
                <div className="card-title">
                    <a href={this.getUrl()}>
                        <span id="title">
                            {this.props.searchResult.Title}
                        </span>
                    </a>
                </div>
            </div>
        );
    }
});

var DisplaySearchResultLikeDislike = React.createClass({
    getInitialState: function () {
        return { searchResult: this.props.searchResult };
    },
    reCalculateTotal: function (searchResult, newIsLiked) {

        if (newIsLiked === true && searchResult.IsLiked === false) {
            searchResult.TotalLike++;
            searchResult.TotalDislike--;
        }
        else if (newIsLiked === true && searchResult.IsLiked === null) {
            searchResult.TotalLike++;
        }
        else if (newIsLiked === false && searchResult.IsLiked === null) {
            searchResult.TotalDislike++;
        }
        else if (newIsLiked === false && searchResult.IsLiked === true) {
            searchResult.TotalDislike++;
            searchResult.TotalLike--;
        }
        else if (newIsLiked === null && searchResult.IsLiked === true && searchResult.TotalLike > 0) {
            searchResult.TotalLike--;
        }
        else if (newIsLiked === null && searchResult.IsLiked === false && searchResult.TotalDislike > 0) {
            searchResult.TotalDislike--;
        }

        searchResult.IsLiked = newIsLiked;

        this.setState({ searchResult: searchResult });

        amplify.publish("likeDislikeButtonPushed", searchResult);

    },
    handleNoClick: function (event) {
        //Dislike

        var searchResult = this.state.searchResult;
        var newIsLiked = searchResult.IsLiked === false ? null : false;
        this.reCalculateTotal(searchResult, newIsLiked);
    },
    handleYesClick: function (event) {
        //Like
        var searchResult = this.state.searchResult;
        var newIsLiked = searchResult.IsLiked === true ? null : true;
        this.reCalculateTotal(searchResult, newIsLiked);
    },
    shareOnFB: function (event) {
        var postedItemUrl = "PostedItems/" + this.state.searchResult.Id + "/" + this.state.searchResult.Slug;
        var hrefLink = this.props.baseAddress + "/" + postedItemUrl;
        var tags = "#" + this.state.searchResult.Tags.join(" #");
        amplify.publish('ShareFacebookClicked', {title: this.state.searchResult.Title, photoUrl: this.state.searchResult.PhotoUrl, link: hrefLink, tags: tags, videoUrl: this.state.searchResult.VideoSourceUrl})
    },
    handleClickOpenLoginModal: function (event) {
        amplify.publish("displayLoginRegisterModal");
    },
    isActiveLike: function (isLiked) {
        return 'mdi mdi-36px float-right cursor-pointer ' + ((isLiked != null && isLiked === true) ? 'mdi-heart' : 'mdi-heart-outline');
    },
    render: function () {
        var that = this;
        var likeButton = null;
        var shareButton = null;
        var readMore = null;

        if (this.props.userAuthorized === true) {
            likeButton = (<a onClick={that.handleYesClick }><i className={this.isActiveLike(this.state.searchResult.IsLiked) }></i></a>);
            shareButton = (<a onClick={that.shareOnFB }><i className="mdi mdi-facebook mdi-36px cursor-pointer"></i></a>);
            var postedItemUrl = "PostedItems/" + this.state.searchResult.Id + "/" + this.state.searchResult.Slug;
            var hrefLink = this.props.baseAddress + "/" + postedItemUrl;
            readMore = (<a href={hrefLink} title={this.state.searchResult.Slug} className="more-text"> READ </a>);

            return (
    <div className="card-text card-likeOrDislike-div">
        <div className="col-xs-4">
            {shareButton}
        </div>
        <div className="col-xs-4 more-div">
            {readMore}
        </div>
        <div className="col-xs-4">
            {likeButton}
        </div>
    </div>
        );
        }

        likeButton = (<a onClick={that.handleClickOpenLoginModal }><i className="mdi mdi-heart-outline mdi-36px float-right cursor-pointer"></i></a>);
        shareButton = (<a onClick={that.shareOnFB }><i className="mdi mdi-facebook mdi-36px cursor-pointer"></i></a>);
        var postedItemUrl = "PostedItems/" + this.state.searchResult.Id + "/" + this.state.searchResult.Slug;
        var hrefLink = this.props.baseAddress + "/" + postedItemUrl;
        readMore = (<a href={hrefLink} title={this.state.searchResult.Slug} className="more-text"> READ </a>);

        return (
        <div className="card-text card-likeOrDislike-div">
        <div className="col-xs-4">
            {shareButton}
        </div>
        <div className="col-xs-4 more-div">
            {readMore}
        </div>
        <div className="col-xs-4">
            {likeButton}
        </div>
        </div>
        );
    }
});

var DisplaySearchResult = React.createClass({
    render: function () {
        return (
            <div>
                 <DisplaySearchResultImage searchResult={this.props.searchResult} userAuthorized={this.props.userAuthorized} baseAddress={this.props.baseAddress} />
                 <DisplaySearchResultLikeDislike searchResult={this.props.searchResult} userAuthorized={this.props.userAuthorized} baseAddress={this.props.baseAddress} />
                 <DisplaySearchResultContent searchResult={this.props.searchResult} userAuthorized={this.props.userAuthorized} baseAddress={this.props.baseAddress} />
            </div>
        );
    }
});

var DisplaySearchResults = React.createClass({
    render: function () {
        var that = this;
        var createItem = function (item, i) {
            return (
                <div className="col-custom">
                     <div className="card-custom" key={item.Id}>
                        <DisplaySearchResult searchResult={item} userAuthorized={that.props.userAuthorized} baseAddress={that.props.baseAddress} />
                     </div>
                </div>
        );
        }

        if (!Array.isArray(this.props.searchResults))
            return null;

        return (
  <span>

      {this.props.searchResults.map(createItem)}

  </span>
        );
    }
});


var DisplaySearchResultsWrapper = React.createClass({
    getInitialState: function () {
        //return { searchResults: [{ ApplicationUserId: '', Id: '', Tags: [], Title: '', PhotoUrl: '' }] };
        return null;
    },
    onChange: function (e) {

    },
    componentDidMount: function () {
        var that = this;

        amplify.subscribe("searchResults", function (newsearchResults, userAuthorized, baseAddress) {
            if (newsearchResults === null)
                return;

            that.setState({ searchResults: newsearchResults, userAuthorized: userAuthorized, baseAddress: baseAddress });

        });
    },
    render: function () {

        if (this.state === null || this.state.searchResults === null)
            return null;

        return (
        <div className="card-deck">
            <DisplaySearchResults searchResults={this.state.searchResults} userAuthorized={this.state.userAuthorized} baseAddress={this.state.baseAddress} />
        </div>
        );
    }
});

ReactDOM.render(<DisplaySearchResultsWrapper />, document.getElementById('display-search-results-display'));
