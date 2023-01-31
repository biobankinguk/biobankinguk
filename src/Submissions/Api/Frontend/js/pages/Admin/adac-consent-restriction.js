function ConsentRestriction(id, description, sortOrder) {
  this.id = ko.observable(id);
  this.description = ko.observable(description);
  this.sortOrder = ko.observable(sortOrder);
}
function ConsentRestrictionModal(id, description, sortOrder) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";
  this.mode = ko.observable(this.modalModeAdd);
  this.consentRestriction = ko.observable(
    new ConsentRestriction(id, description, sortOrder)
  );
}
function AdacConsentRestrictionViewModel() {
  var _this = this;

  this.modalId = "#consent-restriction-modal";
  this.modal = new ConsentRestrictionModal(0, "", 0);
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
    _this.modal.consentRestriction(new ConsentRestriction(0, "", 0));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var consentRestriction = $(event.currentTarget).data("consent-restriction");
    _this.modal.consentRestriction(
      new ConsentRestriction(
        consentRestriction.id,
        consentRestriction.description,
        consentRestriction.sortOrder
      )
    );
    _this.showModal();
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

var adacConsentRestrictionVM;
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
            ? $.map(val, function (innerVal) {
                return { name: elem.name, value: innerVal };
              })
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

  $(function () {
    var table = $("#consent-restriction")["DataTable"]({
      paging: false,
      info: false,
      autoWidth: false,
      rowReorder: true,
      columnDefs: [
        { orderable: true, visible: false, className: "reorder", targets: 0 }, // Column must be orderable for rowReorder
        { orderable: false, targets: "_all" },
      ],
      language: {
        search: "Filter: ",
      },
    });

    // Re-Order Event
    table.on("row-reorder", function (e, diff, edit) {
      // Find the row that was moved
      var triggerRow = diff.filter(function (row) {
        return row.node == edit.triggerRow.node();
      })[0];

      //AJAX Update
      $.ajax({
        url:
          $(triggerRow.node).data("resource-url") +
          "/" +
          $(triggerRow.node).data("restriction-id") +
          "/move",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
          id: $(triggerRow.node).data("restriction-id"),
          description: $(triggerRow.node).data("restriction-desc"),
          sortOrder: triggerRow.newPosition + 1, //1-indexable
        }),
      });
    });
  });

  $("#modal-consent-restriction-form").submit(function (e) {
    adacConsentRestrictionVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
    e.preventDefault();

    var $link = $(this);
    var linkData = $link.data("refdata-model");
    var url = $link.data("resource-url") + "/" + linkData.id;

    bootbox.confirm(
      "Are you sure you want to delete " + linkData.description + "?",
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

  adacConsentRestrictionVM = new AdacConsentRestrictionViewModel();

  ko.applyBindings(adacConsentRestrictionVM);
});
