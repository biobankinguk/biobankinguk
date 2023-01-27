// Modals
var adacSopStatusVM;

function SopStatus(id, description, sortOrder) {
  this.id = id;
  this.description = ko.observable(description);
  this.sortOrder = sortOrder;
}

function SopStatusModal(id, description, sortOrder) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.sopStatus = ko.observable(new SopStatus(id, description, sortOrder));
}

function AdacSopStatusViewModel() {
  var _this = this;

  this.modalId = "#sop-status-modal";
  this.modal = new SopStatusModal(0, "", 0);
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
    _this.modal.sopStatus(new SopStatus(0, "", 0));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var sopStatus = $(event.currentTarget).data("sop-status");

    _this.modal.sopStatus(
      new SopStatus(sopStatus.id, sopStatus.description, sopStatus.sortOrder)
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
  $("#modal-sop-status-form").submit(function (e) {
    adacSopStatusVM.modalSubmit(e);
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

  adacSopStatusVM = new AdacSopStatusViewModel();
  ko.applyBindings(adacSopStatusVM);
});

// DataTables
$(function () {
  var table = $("#sop-status")["DataTable"]({
    paging: false,
    info: false,
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
        $(triggerRow.node).data("sop-status-id") +
        "/move",
      type: "POST",
      dataType: "json",
      data: {
        id: $(triggerRow.node).data("sop-status-id"),
        description: $(triggerRow.node).data("sop-status-desc"),
        sortOrder: triggerRow.newPosition + 1, //1-indexable
      },
    });
  });
});
