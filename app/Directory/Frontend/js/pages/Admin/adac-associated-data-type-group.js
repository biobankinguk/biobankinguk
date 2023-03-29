function AssociatedDataTypeGroup(id, name) {
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

    var associatedDataTypeGroup = $(event.currentTarget).data(
      "associated-groups"
    );
    _this.modal.associatedDataTypeGroup(
      new AssociatedDataTypeGroup(
        associatedDataTypeGroup.associatedDataTypeGroupId,
        associatedDataTypeGroup.name
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
        form.data("resource-url") +
          "/" +
          $(e.target.AssociatedDataTypeGroupId).val(),
        data,
        form.data("success-redirect"),
        form.data("refdata-type")
      );
    }
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
    var linkData = $link.data("refdata-model");
    var url =
      $link.data("resource-url") + "/" + linkData.associatedDataTypeGroupId;

    bootbox.confirm(
      "Are you sure you want to delete " + linkData.name + "?",
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

  adacAssociatedDataTypeGroupVM = new AdacAssociatedDataTypeGroupViewModel();

  ko.applyBindings(adacAssociatedDataTypeGroupVM);
});
