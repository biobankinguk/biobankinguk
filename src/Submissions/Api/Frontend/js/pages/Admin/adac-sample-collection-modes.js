// Modals
var adacCollectionModeVM;

function CollectionMode(id, description, sortOrder) {
  this.id = id;
  this.description = ko.observable(description);
  this.sortOrder = sortOrder;
}

function CollectionModeModal(id, description, sortOrder) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.collectionMode = ko.observable(
    new CollectionMode(id, description, sortOrder)
  );
}

function AdacCollectionModeViewModel() {
  var _this = this;

  this.modalId = "#collection-modes-modal";
  this.modal = new CollectionModeModal(0, "", 0);
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
    _this.modal.collectionMode(new CollectionMode(0, "", 0));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var collectionMode = $(event.currentTarget).data("collection-mode");

    _this.modal.collectionMode(
      new CollectionMode(
        collectionMode.id,
        collectionMode.description,
        collectionMode.sortOrder
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
  $("#modal-collection-modes-form").submit(function (e) {
    adacCollectionModeVM.modalSubmit(e);
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

  adacCollectionModeVM = new AdacCollectionModeViewModel();
  ko.applyBindings(adacCollectionModeVM);
});

// DataTables
$(function () {
  var table = $("#collection-modes")["DataTable"]({
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
        $(triggerRow.node).data("collection-mode-id") +
        "/move",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: JSON.stringify({
        id: $(triggerRow.node).data("collection-mode-id"),
        description: $(triggerRow.node).data("collection-mode-desc"),
        sortOrder: triggerRow.newPosition + 1, //1-indexable
      }),
    });
  });
});
