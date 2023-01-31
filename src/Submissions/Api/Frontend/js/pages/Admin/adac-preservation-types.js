// Modals
var adacPreservationTypeVM;

function PreservationType(id, name, storageTemperatureId, sortOrder) {
  this.id = id;
  this.sortOrder = ko.observable(sortOrder);
  this.name = ko.observable(name);
  this.storageTemperatureId = ko.observable(storageTemperatureId);
}

function PreservationTypeModal(
  id,
  name,
  storageTemperatureId,
  storageTemperatures,
  sortOrder
) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.preservationType = ko.observable(
    new PreservationType(id, name, storageTemperatureId, sortOrder)
  );

  this.storageTemperatures = ko.observableArray(storageTemperatures);
}

function AdacPreservationTypeViewModel() {
  var _this = this;
  this.modalId = "#preservation-types-modal";

  this.storageTemperatures = $(this.modalId).data("storageTemperatures");

  this.modal = new PreservationTypeModal(0, "", 0, this.storageTemperatures, 0);
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
    _this.modal.preservationType(new PreservationType(0, "", 0, 0));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var preservationType = $(event.currentTarget).data("preservation-type");
    var sortOrder = $(event.currentTarget).parent().parent().index() + 1;
    _this.modal.preservationType(
      new PreservationType(
        preservationType.id,
        preservationType.value,
        preservationType.storageTemperatureId,
        sortOrder
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

$(function () {
  $("#modal-preservation-types-form").submit(function (e) {
    adacPreservationTypeVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
    e.preventDefault();

    var $link = $(this);
    var linkData = $link.data("refdata-model");
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

  adacPreservationTypeVM = new AdacPreservationTypeViewModel();
  ko.applyBindings(adacPreservationTypeVM);
});

// DataTables
$(function () {
  var table = $("#preservation-types")["DataTable"]({
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
        $(triggerRow.node).data("preservation-type-id") +
        "/move",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: JSON.stringify({
        id: $(triggerRow.node).data("preservation-type-id"),
        value: $(triggerRow.node).data("preservation-type-value"),
        sortOrder: triggerRow.newPosition + 1,
        storageTemperatureId: $(triggerRow.node).data(
          "preservation-type-storagetemperatureid"
        ),
      }),
    });
  });
});
