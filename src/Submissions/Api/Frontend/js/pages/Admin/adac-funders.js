function Funder(funderId, name) {
  this.funderId = funderId;
  this.name = ko.observable(name);
}

function FunderModal(funderId, name) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);
  this.funder = ko.observable(new Funder(funderId, name));
}

function AdacFunderViewModel() {
  var _this = this;

  this.modalId = "#funder-modal";
  this.modal = new FunderModal(0, "");
  this.dialogErrors = ko.observableArray([]);

  this.showModal = function () {
    this.dialogErrors.removeAll(); //clear errors on a new show
    $(_this.modalId).modal("show");
  };

  this.hideModal = function () {
    $(_this.modalId).modal("hide");
  };

  this.openModalForAdd = function () {
    _this.modal.mode(_this.modal.modalModeAdd);
    _this.modal.funder(new Funder(0, ""));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var funder = $(event.currentTarget).data("funder");
    _this.modal.funder(new Funder(funder.funderId, funder.name));
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
            data.errors.forEach(function (error, index) {
              _this.dialogErrors.push(error);
            });
          }
        }
      },
    });
  };
}

var adacFunderVM;

$(function () {
  $("#funders")["DataTable"]({
    columnDefs: { orderable: false, targets: 2 },
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
  });

  $("#modal-funder-form").submit(function (e) {
    adacFunderVM.modalSubmit(e);
  });

  adacFunderVM = new AdacFunderViewModel();

  ko.applyBindings(adacFunderVM);
});
