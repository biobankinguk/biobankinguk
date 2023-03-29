function Country(id, name) {
  this.id = id;
  this.name = ko.observable(name);
}

function CountryModal(id, name) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);
  this.country = ko.observable(new Country(id, name));
}

function AdacCountryViewModel() {
  var _this = this;

  this.modalId = "#country-modal";
  this.modal = new CountryModal(0, "");
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
    _this.modal.country(new Country(0, ""));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var country = $(event.currentTarget).data("country");
    _this.modal.country(new Country(country.id, country.name));

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

var adacCountryVM;

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
                return {
                  name: elem.name,
                  value: innerVal,
                };
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

  $("#country")["DataTable"]({
    columnDefs: [
      { orderable: false, targets: 3 },
      { width: "180px", targets: 3 },
    ],
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
  });

  $("#modal-country-form").submit(function (e) {
    adacCountryVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
    e.preventDefault();

    var $link = $(this);
    var linkData = $link.data("refdata-model");
    var url = $link.data("resource-url") + "/" + linkData.id;

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

  adacCountryVM = new AdacCountryViewModel();

  ko.applyBindings(adacCountryVM);
});
