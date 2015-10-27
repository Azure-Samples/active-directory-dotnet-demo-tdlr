function AadPicker(maxResultsPerPage, input, searchUrl, tenant) {

        // Inputs
        var resultsPerPage = maxResultsPerPage / 2;
        var $input = $( input );

        // Outputs
        var selected = null;

        // Members
        var currentResults = [];
        var userSkipToken = null;
        var groupSkipToken = null;
        var lastDisplayed = null;
        var lastInput;
        var isPaging = false;
        var isResultsOpen = false;

        // Constants
        var graphLoc = "https://graph.windows.net";
        var apiVersion = "1.5";

        // UI Labels
        var userLabel = "";
        var groupLabel = "(group)";

        // Public Methods
        this.Selected = function () {
            return selected;
        };

        function ConstructUserQuery(inputValue) {

            var url = graphLoc + '/' + tenant + "/users?api-version="
                + apiVersion + "&$top=" + resultsPerPage;

            if (inputValue.length > 0) {
                url += "&$filter=" +
                "startswith(displayName,'" + inputValue +
                "') or startswith(givenName,'" + inputValue +
                "') or startswith(surname,'" + inputValue +
                "') or startswith(userPrincipalName,'" + inputValue +
                "') or startswith(mail,'" + inputValue +
                "') or startswith(mailNickname,'" + inputValue +
                "') or startswith(jobTitle,'" + inputValue +
                "') or startswith(department,'" + inputValue +
                "') or startswith(city,'" + inputValue + "')";
            }

            if (userSkipToken && lastDisplayed != null && inputValue == lastDisplayed)
                url += '&' + userSkipToken;

            return url;
        }

        function ConstructGroupQuery(inputValue) {

            var url = graphLoc + '/' + tenant + "/groups?api-version="
                + apiVersion + "&$top=" + resultsPerPage;

            if (inputValue.length > 0) {
                url += "&$filter=" +
                "startswith(displayName,'" + inputValue +
                "') or startswith(mail,'" + inputValue +
                "') or startswith(mailNickname,'" + inputValue + "')";
            }

            if (groupSkipToken && lastDisplayed != null && inputValue == lastDisplayed)
                url += '&' + groupSkipToken;

            return url;
        }

        function SendQuery(graphQuery) {

            return $.ajax({
                url: searchUrl,
                type: "POST",
                data: {
                    query: graphQuery
                },
                beforeSend: function (jqxhr, settings) {
                    jqxhr.overrideMimeType("application/json");
                }
            });
        }

        function Page() {
            var $resultsDiv = $input.catcomplete("widget");
            if (($resultsDiv.scrollTop() + $resultsDiv.innerHeight() >= $resultsDiv[0].scrollHeight) && !isPaging && (userSkipToken || groupSkipToken)) {
                isPaging = true;
                $input.catcomplete("search", lastDisplayed);
            }
        };

        function BindPagingListener() {

            isPaging = false;
            $input.catcomplete("widget").bind("scroll", Page);
        };

        function Search(inputValue, callback) {

            lastInput = inputValue;
            selected = null;

            var userQuery = ConstructUserQuery(inputValue);

            var userDeffered = new $.Deferred().resolve({ value: [] }, "success");
            var groupDeffered = new $.Deferred().resolve({ value: [] }, "success");

            if ((inputValue == lastDisplayed && userSkipToken) || inputValue != lastDisplayed)
                userDeffered = SendQuery(userQuery);

            var recordResults = function () {
                return function (userQ) {
                    if (userQ) {

                        var usersAndGroups = userQ[0].value;

                        if (userQ[0]["odata.nextLink"] != undefined) {
                            userSkipToken = userQ[0]["odata.nextLink"]
                                .substring(userQ[0]["odata.nextLink"].indexOf("$skiptoken"),
                                userQ[0]["odata.nextLink"].length);
                        }
                        else {
                            userSkipToken = null;
                        }

                        if (lastDisplayed == null || inputValue != lastDisplayed) {
                            currentResults = [];
                        }

                        for (var i = 0; i < usersAndGroups.length; i++) {

                            if (usersAndGroups[i].objectType == "User") {
                                currentResults.push({
                                    label: usersAndGroups[i].displayName,
                                    value: usersAndGroups[i].displayName,
                                    objectId: usersAndGroups[i].objectId,
                                    objectType: userLabel,
                                });
                            } else if (usersAndGroups[i].objectType == "Group") {
                                currentResults.push({
                                    label: usersAndGroups[i].displayName,
                                    value: usersAndGroups[i].displayName,
                                    objectId: usersAndGroups[i].objectId,
                                    objectType: groupLabel,
                                });
                            }
                        }
                    }
                    else {
                        currentResults = [];
                        callback([{ label: "Error During Search" }]);
                        selected = null;
                        return;
                    }

                    if (inputValue == lastInput) {
                        lastDisplayed = inputValue;
                        callback(currentResults);
                    }
                };
            };

            $.when(userDeffered, groupDeffered)
                .always(recordResults());
        };

        function Listen() {
            $input.catcomplete({
                source: function (request, response) {
                    Search(request.term, response);
                },
                minLength: 0,
                delay: 200,
                open: function (event, ui) {
                    isResultsOpen = true;
                    if (isPaging) {
                        event.target.scrollTop = 0;
                        isPaging = false;
                    }
                    Page();
                },
                select: function (event, ui) {
                    selected = {
                        objectId: ui.item.objectId, 
                        displayName: ui.item.label,
                        objectType: ui.item.objectType,
                    };
                    $input.trigger("picker-select");
                    $(this).val('');
                    return false;
                },
                close: function (event, ui) {
                    isResultsOpen = false;
                    lastDisplayed = null;
                    userSkipToken = null;
                    groupSkipToken = null;
                    currentResults = [];
                },
            });

            $input.focus(function (event) {
                if (!isResultsOpen)
                    $(this).catcomplete("search", this.value);
            });

            $input.catcomplete("widget").css("max-height", "200px")
                .css("overflow-y", "scroll")
                .css("overflow-x", "hidden");

            BindPagingListener();
        }

        // Activate
        Listen();
    }

    $.widget("custom.catcomplete", $.ui.autocomplete, {
        _create: function () {
            this._super();
            this.widget().menu("option", "items", "> :not(.ui-autocomplete-category)");
        },
        _renderMenu: function (ul, items) {
            var that = this;

            $.each(items, function (index, item) {
                that._renderItemData(ul, item);
            });
        },
        _renderItem: function (ul, item) {

            var label = $("<div>").addClass("aadpicker-result-label").css("display", "inline-block").append(item.label);
            var type = $("<div>").addClass("aadpicker-result-type")
                .css("text-align", "right")
                .css("display", "inline-block")
                .css("float", "right")
                .append(item.objectType);
            var toappend = [label, type];

            return $("<li>").addClass("aadpicker-result-elem").attr("data-selected", "false")
                .attr("data-objectId", item.objectId).append(toappend).appendTo(ul);
        },
    });