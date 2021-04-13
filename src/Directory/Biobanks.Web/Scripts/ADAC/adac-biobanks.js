﻿function BiobankAdminsViewModel() {
    var _this = this;
    var base = "BiobankAdmin";
    this.elements = {
        id: "#BiobankId",
        modal: "#modalInvite" + base,
        form: "#modalInvite" + base + "Form",
        modal2: "#modalReset" + base
    };

    this.dialogErrors = ko.observableArray([]);
    this.biobankId = $(this.elements.id).data("biobank-id");

    this.openInviteDialog = function () {
        $.ajax({
            url: "/Adac/InviteAdminAjax/",
            data: { biobankId: _this.biobankId },
            contentType: "application/html",
            success: function (content) {
                //clear form errors (as these are in the page's ko model)
                _this.dialogErrors.removeAll();

                _this.cleanNodeJquerySafe(_this.elements.modal);

                //populate the modal with the form
                $(_this.elements.modal).html(content);

                //apply ko bindings to the ajax'd elements
                ko.applyBindings(biobankAdminsVm, $(_this.elements.modal)[0]);

                //wire up the form submission
                $(_this.elements.form).submit(function (e) {
                    _this.submitInviteDialog(e);
                });
            },
        });
    };

    this.submitInviteDialog = function (e) {
        e.preventDefault();
        var form = $(_this.elements.form);

        $.ajax({
            type: "POST",
            url: form.data("action"),
            data: form.serialize(),
            success: function (data) {
                //clear form errors (as these are in the page's ko model)
                _this.dialogErrors.removeAll();

                if (data.success) {
                    $(_this.elements.modal).modal("hide");
                    //now we can redirect (force a page reload, following the successful AJAX submit
                    //(why not just do a regular POST submit? for nice AJAX modal form valdation)
                    window.location.href =
                        form.data("success-redirect") + "?Name=" + data.name;
                } else {
                    if (Array.isArray(data.errors)) {
                        for (var error of data.errors) {
                            _this.dialogErrors.push(error);
                        }
                    }
                }
            },
        });
    };

    this.cleanNodeJquerySafe = function (nodeSelector) {
        //clear knockout bindings,
        //but leave jQuery/bootstrap bindings intact!
        var original = ko.utils.domNodeDisposal["cleanExternalData"];
        ko.utils.domNodeDisposal["cleanExternalData"] = function () { };
        ko.cleanNode($(nodeSelector)[0]); //designed to work with ID selectors, so only does the first match
        ko.utils.domNodeDisposal["cleanExternalData"] = original;
    };

    this.openResetBox = function () {
        $.ajax({
            url: "/Adac/GenerateResetLinkAjax/",
            data: { biobankUserId: _this.biobankUserId },
            contentType: "application/html",
            success: function (content) {                
                //clear form errors (as these are in the page's ko model)
                _this.dialogErrors.removeAll();

                _this.cleanNodeJquerySafe(_this.elements.modal2);

                //populate the modal with the form
                $(_this.elements.modal2).html(content);

                //apply ko bindings to the ajax'd elements
                ko.applyBindings(biobankAdminsVm, $(_this.elements.modal2)[0]);                
            },
        });
    }
}

var biobankAdminsVm;

$(function () {
    biobankAdminsVm = new BiobankAdminsViewModel();
    ko.applyBindings(biobankAdminsVm);

    $("#adac-biobanks").DataTable({
        "columnDefs": [{ "orderable": false, "targets": [2, 3, 4, 5] }], // remove the sort icon for columns with just buttons/links in
        "paging": false,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });

    $(".resend-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            $link.data("admin-name") +
            " (" + $link.data("admin-email") + ") " +
            "has not yet confirmed their account.<br/>Do you want to resend their confirmation link?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            });
    });

    $(".suspend-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you want to suspend " + $link.data("biobank-name") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            });
    });

    $(".unsuspend-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you want to unsuspend " + $link.data("biobank-name") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            });
    });

    $(".confirm-admin-delete").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you wish to remove " +
            $link.data("admin-name") +
            " from the biobank " +
            $link.data("biobank-name") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

})

function copyToClipboard() {
    var copyText = document.getElementById("link");

    copyText.select();
    copyText.setSelectionRange(0, 99999);

    document.execCommand("copy");

    alert("Copied");
}