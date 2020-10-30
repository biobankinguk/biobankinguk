
function ClearButton_Click(e) {
    ClearContactList("All resources removed from contact list.");
}

function ContactButton_Click(e) {
    // Stop the linking being processed.
    e.preventDefault();

    var button = $(e.target);

    //if the icon span was clicked, get the parent button, then continue
    if (button.is("span")) button = button.parent();

    // Get the biobank ID.
    var biobankId = button.data("biobank-id");

    //Get contact Ids
    var contactIds = JSON.parse(localStorage.getItem("contactIds"));

    // Otherwise, remove it.
    var index = contactIds.indexOf(biobankId);

    if (index !== -1) {
        contactIds.splice(index, 1);
    }

    UpdateContactList(JSON.stringify(contactIds)).done(response => {
        if (!jQuery.isEmptyObject(response)) {
            SaveToLocalStorage(contactIds);
            UpdateContactCounter(contactIds);
            window.feedbackMessage("Resource removed from contact list.", "success");
        }
    });

};

function SaveToLocalStorage(contactIds) {
    localStorage.setItem("contactIds", JSON.stringify(contactIds))
};

function UpdateContactCounter(contactIds) {
    var count = contactIds.length;
    var contactButtonBase = "<span class=\"fa fa-envelope-o labelled-icon\"></span>View Contact List ({count})";

    // Update counter dropdown
    $("#ContactCounter").html(contactButtonBase.replace("{count}", String(count)));

    // Update counter
    if (count > 9) {
        $(".contact-counter").html("+");
        $(".contact-counter").addClass("active");
    }
    else if (count > 0) {
        $(".contact-counter").html(count);
        $(".contact-counter").addClass("active");
    }
    else {
        $(".contact-counter").empty();
        $(".contact-counter").removeClass("active");
    }
}

function ClearContactList(msg) {
    contactIds = [];
    UpdateContactList(JSON.stringify(contactIds)).done(response => {
        if (!jQuery.isEmptyObject(response)) {
            SaveToLocalStorage(contactIds);
            UpdateContactCounter(contactIds);
            if (msg != "") {
                window.feedbackMessage(msg, "success");
            }
        }
    });
}

function PrepareBiobankSlugForUrl (slug) {
    var output = slug.replace(" ", "+");
    return encodeURIComponent(output);
};

function UpdateContactList(contactIds) {
    return $.ajax({
        url: "/Home/BiobankContactDetailsAjax/" + contactIds,//localStorage.getItem("contactIds"),
        type: "GET",
        success: response => {

            //Find table
            var contactListTable = $("#home-contactlist");
            var newRow;
            var biobankEmails = [];

            if (jQuery.isEmptyObject(response)) {
                window.feedbackMessage("Oops! Something went wrong. Please try again.", "warning");
            } else {
                contactListTable.empty(); //clear table
                for (let i = 0; i < response.Contacts.length; i++) {
                    newRow = `<a href="/Profile/Biobank/${response.Contacts[i].BiobankExternalId}" class="detailed-search-link">`
                        + `<div class="well well-hover">
                                <div class="row no-link-style">
                                    <div class="col-sm-8">
                                        <h4>${response.Contacts[i].BiobankName}</h4>
                                    </div>
                                    <div class="col-sm-4 text-right">
                                        <button class="btn btn-default contact-button pull-right" data-biobank-id="${response.Contacts[i].BiobankExternalId}">
                                        <span class="fa fa-envelope-o labelled-icon"></span>Remove</button>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-10 sample-set-summaries">
                                        <p class="no-link-style">${response.Contacts[i].Description}</p>
                                        <p>Click for more details...</p>
                                    </div>
                                </div>
                            </div>`
                        + `</a>`;

                    biobankEmails.push(response.Contacts[i].ContactEmail);
                    contactListTable.append(newRow);
                }

                $(".contact-button").click(e => {
                    ContactButton_Click(e);
                });

                // Render Networks HTML
                var networkHtml = "";
                $("#third-parties").empty(); //clear table
                for (var i = 0; i < response.Networks.length; i++) {
                    var network = response.Networks[i];
                    networkHtml += "<div class=\"clear-fix";
                    if (network.LogoName != null && network.LogoName.length > 0) {
                        var logoUrl = logoBaseUrl + network.LogoName;
                        networkHtml += "\"><img src=\"" + logoUrl + "\" class=\"contact-modal-profile-image pull-left public-profile-logo\" alt=\"" + network
                            .NetworkName + "\"/>";
                    }
                    else {
                        networkHtml += " contact-modal-no-logo-container\">";
                    }
                    var attributeReadyAnonymousIdentifiers = network.NonMemberBiobankAnonymousIds.toString();
                    var url = network.HandoverBaseUrl;
                    if (network.MultipleHandoverOrdIdsParams) {
                        // Add in all those who are already members of the network
                        for (var bb = 0; bb < network.BiobankExternalIdsInNetwork.length; bb++) {
                            var slug = network.BiobankExternalIdsInNetwork[bb];
                            if (slug != null) {
                                url += "" + network.HandoverOrgIdsUrlParamName + PrepareBiobankSlugForUrl(slug);
                            }
                        }
                        // Add in all those who aren't yet part of the network
                        if (network.HandoverNonMembers) {
                            for (var bb = 0; bb < network.NonMemberBiobankAnonymousIds.length; bb++) {
                                var id = network.NonMemberBiobankAnonymousIds[bb];
                                if (id != null) {
                                    url += "" + network.HandoverNonMembersUrlParamName + PrepareBiobankSlugForUrl(id);
                                }
                            }
                        }
                    }
                    else {
                        // Add in all those who are already members of the network
                        url += "" + network.HandoverOrgIdsUrlParamName;
                        for (var bb = 0; bb < network.BiobankExternalIdsInNetwork.length; bb++) {
                            var slug = network.BiobankExternalIdsInNetwork[bb];
                            if (slug != null) {
                                url += PrepareBiobankSlugForUrl(slug) + ",";
                            }
                        }
                        // Add in all those who aren't yet part of the network
                        if (network.HandoverNonMembers) {
                            for (var bb = 0; bb < network.NonMemberBiobankAnonymousIds.length; bb++) {
                                var id = network.NonMemberBiobankAnonymousIds[bb];
                                if (id != null) {
                                    url += PrepareBiobankSlugForUrl(id) + ",";
                                }
                            }
                        }
                    }
                    networkHtml += "<p class=\"contact-modal-network-blurb\"><strong>" + network.NetworkName + "</strong><br/>";
                    if (network.BiobankExternalIdsInNetwork.length > 0) {
                        networkHtml += network.BiobankExternalIdsInNetwork.length + " of the " + response.Contacts.length + " resources are available to submit a request via <a href=\"" + url + "\" class=\"network-handover-link\" data-networkid=\"" + network.NetworkId + "\" data-nonmemberidentifiers=\"" + attributeReadyAnonymousIdentifiers + "\">" + network.NetworkName + "</a>";
                    }
                    else {
                        networkHtml += "No resources are available but ask them to sign-up and create you request in <a href=\"" + url + "\" class=\"network-handover-link\" data-networkid=\"" + network.NetworkId + "\" data-nonmemberidentifiers=\"" + attributeReadyAnonymousIdentifiers + "\">" + network.NetworkName + "</a>";
                    }
                    networkHtml += "</div>";
                }

                // Show if Networks Exist
                if (response.Networks.length > 0) {
                    var $contact = $("#contact-third-party");
                    var $parties = $("#third-parties");

                    $parties.append(networkHtml);
                    $contact.removeClass("hidden");
                }
            }
        },
        error: response => {
            console.log("Ajax request failed");
            window.feedbackMessage("An error occurred. Please try again later.", "warning");
        }
    });
}

/*
 *  Email Contact List
 */
function ResetEmailFields() {
    $("#Email").val("");
    var chkbox = $("#ContactMe");
    if (chkbox.is(':checked')) {
        chkbox.click();
        //chkbox.prop("checked", false);
    }   
}

function submitEmail(e) {

    // Toggles button state and animation
    function toggleButton($button, state) {
        var $text = $button.children("span");
        var $spinner = $button.children(".fa-spin");

        if (state) {
            $button.attr("disabled", true);
            $spinner.removeClass("hidden");
            $text.text($text.data("active-text"));
        }
        else {
            $button.attr("disabled", false);
            $spinner.addClass("hidden");
            $text.text($text.data("inactive-text"));
        }
    }

    e.preventDefault();

    var contactIds = JSON.parse(localStorage.getItem("contactIds"));
    var emailAddress = $("#Email").val();
    var contactMe = $("#contactMe").is(':checked');

    if (contactIds == null || contactIds.length == 0) {
        window.feedbackMessage("You need to select some organisations to contact first!", "danger");
    }
    else if (emailAddress == null || emailAddress.length == 0) {
        window.feedbackMessage("Please supply a valid email address!", "danger");
    }
    else {

        // Start Spinner
        var $button = $(e.target).find(":submit");
        toggleButton($button, true);

        // Send POST Request
        var url = "/Home/EmailContactListAjax";
        var data = {
            ids: contactIds,
            email: emailAddress,
            contactMe: contactMe
        };

        $.post(url, data)
            .done(function () {
                ClearContactList("Email sent successfully!");
                ResetEmailFields();
            })
            .fail(function () {
                window.feedbackMessage("Something went wrong while sending email!", "danger");
            })
            .always(function () {
                // Stop Spinner
                toggleButton($button, false);
         });
    }
};

$(function() {
    setTimeout(function () { UpdateContactList(localStorage.getItem("contactIds")) }, 500);

    $(".delete-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you want to clear your list?",
            function (confirmation) {
                confirmation && ClearButton_Click(e);
            }
        );
    });

    $("#email-contactlist-form").submit(function (e) {
        submitEmail(e);
    });
});


/*
 * Modal For Linking To External Site
 */
var externalSiteModal;

function ExternalSiteModalView(modalId) {

    var $link;
    var _this = this;
    
    // Hook into submit event
    $(modalId).submit(function (e) {
        _this.modalSubmit(e);
    });

    this.hideModal = function () {
        $(modalId).modal("hide");
    };

    this.showModal = function () {
        $(modalId).modal("show");
    };

    this.openModel = function (link) {
        this.$link = $(link);
        this.showModal();
    }

    this.modalSubmit = function (e) {
        e.preventDefault();

        var ajaxData = {
            "NetworkId": _this.$link.data("networkid"),
            "BiobankAnonymousIdentifiers": _this.$link.data("nonmemberidentifiers").split(",")
        };

        $.post("/Home/NotifyNetworkNonMembersOfHandoverAjax", ajaxData)
            .done(function () {
                window.location.href = _this.$link.attr("href");
            });
    };
}

$(function () {
    // Open Modal on Link Click
    $("body").delegate(".network-handover-link", "click", function (e) {
        e.preventDefault();
        externalSiteModal.openModel(this);
    });

    externalSiteModal = new ExternalSiteModalView("#external-site-modal");
    ko.applyBindings(externalSiteModal);
})
