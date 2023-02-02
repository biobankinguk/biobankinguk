// Modals
var adacRegistrationDomainRuleVM;

function RegistrationDomainRule(id, value, ruleType) {
  this.id = id;
  this.value = ko.observable(value);
  this.ruleType = ko.observable(ruleType);
}

function RegistrationDomainRuleModal(id, value, ruleType) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.registrationDomainRule = ko.observable(
    new RegistrationDomainRule(id, value, ruleType)
  );
}

function AdacRegistrationDomainRuleViewModel() {
  var _this = this;

  this.modalId = "#domain-rule-modal";
  this.modal = new RegistrationDomainRuleModal(0, "", "Block");
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
    _this.modal.registrationDomainRule(
      new RegistrationDomainRule(0, "", "Block")
    );

    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var registrationDomainRule = $(event.currentTarget).data("domain-rule");

    _this.modal.registrationDomainRule(
      new RegistrationDomainRule(
        registrationDomainRule.id,
        registrationDomainRule.value,
        registrationDomainRule.ruleType
      )
    );

    _this.showModal();

    // turn off editing ruletype
    $("#ruleTypes").find("input:radio:not(:checked)").attr("disabled", true);
  };

  this.modalSubmit = function (e) {
    e.preventDefault();
    var form = $(e.target); // get form as a jquery object

    // Get Form Data
    var data = serializeFormData(form);

    // Get Action Type
    var action = _this.modal.mode();
    if (action == "Add") {
      addRefData(
        _this,
        form.data("resource-url"),
        data,
        form.data("success-redirect"),
        form.data("refdata-type")
      ); // cf. adac-refdata-utility.js
    } else if (action == "Update") {
      editRefData(
        _this,
        form.data("resource-url") + "/" + $(e.target.Id).val(),
        data,
        form.data("success-redirect"),
        form.data("refdata-type")
      );
    }
  };
}

$(function () {
  $("#modal-domain-rule-form").submit(function (e) {
    adacRegistrationDomainRuleVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
    e.preventDefault();

    var $link = $(this);
    var linkData = $link.data("domain-rule");
    var url = $link.data("resource-url") + "/" + linkData.id;

    bootbox.confirm(
      "Are you sure you want to delete " + linkData.value + "?",
      function (confirmation) {
        if (confirmation) {
          deleteRefData(
            url,
            $link.data("success-redirect"),
            $link.data("refdata-type")
          );
        }
      }
    );
  });

  adacRegistrationDomainRuleVM = new AdacRegistrationDomainRuleViewModel();
  ko.applyBindings(adacRegistrationDomainRuleVM);

  // re-enable radio buttons on modal close
  $("#domain-rule-modal").on("hidden.bs.modal", function (e) {
    $("#ruleTypes").find("input:radio").attr("disabled", false);
  });
});

// DataTables
$(function () {
  $("#domain-rule-block")["DataTable"]({
    paging: false,
    info: false,
    autoWidth: false,
    rowReorder: false,
    columnDefs: [
      { orderable: false, targets: 3 },
      { width: "250px", targets: 3 },
    ],
    language: {
      search: "Filter: ",
    },
  });
  $("#domain-rule-allow")["DataTable"]({
    paging: false,
    info: false,
    autoWidth: false,
    rowReorder: false,
    columnDefs: [
      { orderable: false, targets: 3 },
      { width: "250px", targets: 3 },
    ],
    language: {
      search: "Filter: ",
    },
  });
});
