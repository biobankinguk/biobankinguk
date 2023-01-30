// Modals
var adacStorageTemperatureVM;

function StorageTemperature(id, value, sortOrder) {
  this.id = id;
  this.value = ko.observable(value);
  this.sortOrder = sortOrder;
}

function StorageTemperatureModal(id, value, sortOrder) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.storageTemperature = ko.observable(
    new StorageTemperature(id, value, sortOrder)
  );
}

function AdacStorageTemperatureViewModel() {
  var _this = this;

  this.modalId = "#storage-temperatures-modal";
  this.modal = new StorageTemperatureModal(0, "", 0);
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
    _this.modal.storageTemperature(new StorageTemperature(0, "", 0));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var storageTemperature = $(event.currentTarget).data("storage-temperature");

    _this.modal.storageTemperature(
      new StorageTemperature(
        storageTemperature.id,
        storageTemperature.value,
        storageTemperature.sortOrder
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
  $("#modal-storage-temperatures-form").submit(function (e) {
    adacStorageTemperatureVM.modalSubmit(e);
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

  adacStorageTemperatureVM = new AdacStorageTemperatureViewModel();
  ko.applyBindings(adacStorageTemperatureVM);
});

$(function () {
  $("#change-storage-temperature-name-form").submit(function (e) {
    e.preventDefault();
    $.ajax({
      type: "POST",
      url: "/Admin/SiteConfig/UpdateReferenceTermName",
      data: {
        newReferenceTermKey: "site.display.storagetemperature.name",
        newReferenceTermName: document.getElementById("StorageTemperatureName")
          .value,
      },
      dataType: "json",
      success: function () {
        location.reload();
      },
    });
  });

  $("#storageTemperatureTitle").click(function () {
    document.getElementById("titleName").setAttribute("hidden", true);
    document
      .getElementById("change-storage-temperature-name-form")
      .removeAttribute("hidden");
  });

  $("#storageTemperatureTitleCancel").click(function () {
    document
      .getElementById("change-storage-temperature-name-form")
      .setAttribute("hidden", true);
    document.getElementById("titleName").removeAttribute("hidden");
  });
});

// DataTables
$(function () {
  var table = $("#storage-temperatures")["DataTable"]({
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
    var $row = $(triggerRow.node);

    //AJAX Update
    $.ajax({
      url: $row.data("resource-url") + "/" + $row.data("id") + "/move",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: JSON.stringify({
        id: $row.data("id"),
        value: $row.data("value"),
        sortOrder: triggerRow.newPosition + 1, //1-indexable
      }),
    });
  });
});
