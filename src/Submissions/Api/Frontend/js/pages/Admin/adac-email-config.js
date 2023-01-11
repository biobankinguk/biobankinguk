var adacRegistrationDomainRuleVM;

function RegistrationDomainRule(id, ruleType, value, source, dateModified) {
    this.id = ko.observable(id);
    this.ruleType = ko.observable(ruleType);
    this.value = ko.observable(value);
    this.source = ko.observable(source);
    this.dateModified = ko.observable(dateModified);
}

function RegistrationDomainRuleModal(id, ruleType, value, source, dateModified) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);
    this.registrationDomainRule = ko.observable(
        new RegistrationDomainRule(id, ruleType, value, source, dateModified)
    );
}

function AdacRegistrationDomainRuleViewModel() {
    var _this = this;

    this.modalId = "#registration-domain-rules-modal";
    this.modal = new RegistrationDomainRuleModal(0, "", "","","");
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
        _this.modal.registrationDomainRule(new RegistrationDomainRule(0, "", "", "", ""));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var registrationDomainRule = $(event.currentTarget).data("registration-domain-rule");
        _this.modal.registrationDomainRule(
            new RegistrationDomainRule(
                registrationDomainRule.Id,
                registrationDomainRule.RuleType,
                registrationDomainRule.Value,
                registrationDomainRule.Source,
                registrationDomainRule.DateModified
            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();
        var form = $(e.target); // get form as a jquery object

        // Get Action Type
        var action = _this.modal.mode();
        if (action == 'Add') {
            addRefData(_this, form.data("resource-url"), form.serialize(),
                form.data("success-redirect"), form.data("refdata-type")); // cf. adac-refdata-utility.js
        } else if (action == 'Update') {
            editRefData(_this, form.data("resource-url") + '/' + $(e.target.Id).val(), form.serialize(),
                form.data("success-redirect"), form.data("refdata-type"));
        }
    };
}

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
                    var val = $(this).val();
                    return val == null
                        ? null
                        : $.isArray(val)
                            ? $.map(val, function(innerVal) {return ({ name: elem.name, value: innerVal })})
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

    $("#modal-registration-domain-rules-form").submit(function (e) {
        adacRegistrationDomainRuleVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var linkData = $link.data("refdata-model")
        var url = $link.data("resource-url") + "/" + linkData.Id;

        bootbox.confirm("Are you sure you want to delete " + linkData.Value + "?",
            function (confirmation) {
                if (confirmation) {
                    deleteRefData(url, $link.data("success-redirect"), $link.data("refdata-type"));
                }
            }
        );
    });

    adacRegistrationDomainRuleVM = new AdacRegistrationDomainRuleViewModel();
    ko.applyBindings(adacRegistrationDomainRuleVM);
});

// DataTables
$(function () {
    var table = $("#registration-domain-rules")["DataTable"]({
        paging: false,
        info: false,
        autoWidth: false,
        rowReorder: true,
        columnDefs: [
            { orderable: true, targets: '_all' }
        ],
        language: {
            search: "Filter: ",
        },
    });
   
});
