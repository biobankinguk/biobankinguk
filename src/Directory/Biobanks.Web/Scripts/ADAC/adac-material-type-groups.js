﻿var adacMaterialTypeVM;

function MaterialTypeGroup(id, description) {
    this.id = ko.observable(id);
    this.description = ko.observable(description);
}

function MaterialTypeModal(id, description) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);
    this.materialType = ko.observable(
        new MaterialTypeGroup(id, description)
  );
}

function AdacMaterialTypeViewModel() {
  var _this = this;

  this.modalId = "#material-type-modal";
  this.modal = new MaterialTypeModal(0, "");
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
      _this.modal.materialType(new MaterialTypeGroup(0, ""));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var materialType = $(event.currentTarget).data("material-type");
    _this.modal.materialType(
      new MaterialTypeGroup(
          materialType.Id,
          materialType.Description,
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

// DataTables
$(function () {
    var table = $("#material-types")["DataTable"]({
        paging: false,
        info: false,
        autoWidth: false,
        language: {
            search: "Filter: ",
        },
    });
});

// Events
$(function () {
    $("#modal-material-type-form").submit(function (e) {
        adacMaterialTypeVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var linkData = $link.data("refdata-model")
        var url = $link.data("resource-url") + "/" + linkData.Id;

        bootbox.confirm("Are you sure you want to delete " + linkData.Description + "?",
            function (confirmation) {
                if (confirmation) {
                    deleteRefData(url, $link.data("success-redirect"), $link.data("refdata-type"));
                }
            }
        );
    });

    adacMaterialTypeVM = new AdacMaterialTypeViewModel();
    ko.applyBindings(adacMaterialTypeVM);
});