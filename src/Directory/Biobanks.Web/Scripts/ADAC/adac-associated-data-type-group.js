﻿function AssociatedDataTypeGroup(id, name) {
    this.id = id;
    this.name = ko.observable(name);
}

function AssociatedDataTypeGroupModal(id, name) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);
    this.associatedDataTypeGroup = ko.observable(
        new AssociatedDataTypeGroup(id, name)
    );
}

function AdacAssociatedDataTypeGroupViewModel() {
    var _this = this;

    this.modalId = "#associated-groups-modal";
    this.modal = new AssociatedDataTypeGroupModal(0, "");
    this.dialogErrors = ko.observableArray([]);

    this.showModal = function () {
        _this.dialogErrors.removeAll(); //clear errors on a new show
        $(_this.modalId).modal("show");
    };

    this.hideModal = function () {
        $(_this.modalId).modal("hide");
    };

    this.openModalForAdd = function () {
        _this.modal.mode(_this.modal.modalModeAdd);
        _this.modal.associatedDataTypeGroup(new AssociatedDataTypeGroup(0, ""));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var associatedDataTypeGroup = $(event.currentTarget).data("associated-groups");
        _this.modal.associatedDataTypeGroup(
            new AssociatedDataTypeGroup(
                associatedDataTypeGroup.AssociatedDataTypeGroupId,
                associatedDataTypeGroup.Name

            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();
        var form = $(e.target); //get the submit button's form

        var action = _this.modal.mode().toLowerCase() + "-action";
        var successRedirect =
            _this.modal.mode().toLowerCase() + "-success-redirect";

        $.ajax({
            type: "POST",
            url: form.data(action),
            data: form.serialize(),
            success: function (data) {
                //clear form errors (as these are in the page's ko model)
                _this.dialogErrors.removeAll();

                if (data.success) {
                    _this.hideModal();
                    //now we can redirect (force a page reload, following the successful AJAX submit
                    //(why not just do a regular POST submit? for nice AJAX modal form valdation)
                    window.location.href =
                        form.data(successRedirect) + "?Name=" + data.name;
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
}

var adacAssociatedDataTypeGroupVM;

$(function () {
    //jquery plugin to serialise checkboxes as bools
    (function ($) {
        $.fn.serialize = function () {
            return $.param(this.serializeArray());
        };

        $.fn.serializeArray = function () {
            var o = $.extend(
                {
                    checkboxesAsBools: true,
                },
                {}
            );

            var rselectTextarea = /select|textarea/i;
            var rinput = /text|hidden|password|search/i;

            return this.map(function () {
                return this.elements ? $.makeArray(this.elements) : this;
            })
                .filter(function () {
                    return (
                        this.name &&
                        !this.disabled &&
                        (this.checked ||
                            (o.checkboxesAsBools && this.type === "checkbox") ||
                            rselectTextarea.test(this.nodeName) ||
                            rinput.test(this.type))
                    );
                })
                .map(function (i, elem) {
                    const val = $(this).val();
                    return val == null
                        ? null
                        : $.isArray(val)
                            ? $.map(val, (innerVal) => ({ name: elem.name, value: innerVal }))
                            : {
                                name: elem.name,
                                value:
                                    o.checkboxesAsBools && this.type === "checkbox" //moar ternaries!
                                        ? this.checked
                                            ? "true"
                                            : "false"
                                        : val,
                            };
                })
                .get();
        };
    })(jQuery);

    $("#associated-groups")["DataTable"]({
        columnDefs: [
            { orderable: false, targets: 2 },
            { width: "250px", targets: 2 },
        ],
        paging: false,
        info: false,
        autoWidth: false,
        language: {
            search: "Filter: ",
        },
    });

    $("#modal-associated-groups-form").submit(function (e) {
        adacAssociatedDataTypeGroupVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you want to delete " + $link.data("associated-groups") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacAssociatedDataTypeGroupVM = new AdacAssociatedDataTypeGroupViewModel();

    ko.applyBindings(adacAssociatedDataTypeGroupVM);
});
