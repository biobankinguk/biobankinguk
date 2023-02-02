// Modals
var adacCountyVM;

function County(id, name, countryId) {
  this.id = id;
  this.name = ko.observable(name);
  this.countryId = ko.observable(countryId);
}

function CountyModal(id, name, countryId, countries) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.county = ko.observable(new County(id, name, countryId));

  this.countries = ko.observableArray(countries);
}

function AdacCountyViewModel() {
  var _this = this;
  this.modalId = "#county-modal";

  this.countries = $(this.modalId).data("countries");

  this.modal = new CountyModal(0, "", 0, this.countries);
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
    _this.modal.county(new County(0, "", 0, this.countries));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var county = $(event.currentTarget).data("county");

    _this.modal.county(
      new County(county.id, county.name, county.countryId, this.countries)
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

$(function () {
  $("#modal-county-form").submit(function (e) {
    adacCountyVM.modalSubmit(e);
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

  adacCountyVM = new AdacCountyViewModel();
  ko.applyBindings(adacCountyVM);
});

// DataTables
$(function () {
  var table = $("#county")["DataTable"]({
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
  });
});
