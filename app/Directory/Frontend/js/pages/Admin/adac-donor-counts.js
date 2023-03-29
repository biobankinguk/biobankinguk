// Modals
var adacDonorCountVM;

function DonorCount(id, description, sortOrder, lowerBound, upperBound) {
  this.id = id;
  this.description = ko.observable(description);
  this.sortOrder = sortOrder;
  this.lowerBound = ko.observable(lowerBound);
  this.upperBound = ko.observable(upperBound);
}

function DonorCountModal(id, description, sortOrder, lowerBound, upperBound) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.donorCount = ko.observable(
    new DonorCount(id, description, sortOrder, lowerBound, upperBound)
  );
}

function AdacDonorCountViewModel() {
  var _this = this;

  this.modalId = "#donor-counts-modal";
  this.modal = new DonorCountModal(0, "", 0, 0, 0);
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
    _this.modal.donorCount(new DonorCount(0, "", 0, 0, 0));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var donorCount = $(event.currentTarget).data("donor-count");

    _this.modal.donorCount(
      new DonorCount(
        donorCount.id,
        donorCount.description,
        donorCount.sortOrder,
        donorCount.lowerBound,
        donorCount.upperBound
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
  $("#modal-donor-counts-form").submit(function (e) {
    adacDonorCountVM.modalSubmit(e);
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

  adacDonorCountVM = new AdacDonorCountViewModel();
  ko.applyBindings(adacDonorCountVM);
});

$(function () {
  $("#change-donor-count-name-form").submit(function (e) {
    e.preventDefault();
    $.ajax({
      type: "POST",
      url: "/Admin/SiteConfig/UpdateReferenceTermName",
      data: {
        newReferenceTermKey: "site.display.donorcount.name",
        newReferenceTermName: document.getElementById("DonorCountName").value,
      },
      dataType: "json",
      success: function () {
        location.reload();
      },
    });
  });

  $("#donorCountTitle").click(function () {
    document.getElementById("titleName").setAttribute("hidden", true);
    document
      .getElementById("change-donor-count-name-form")
      .removeAttribute("hidden");
  });

  $("#donorCountTitleCancel").click(function () {
    document
      .getElementById("change-donor-count-name-form")
      .setAttribute("hidden", true);
    document.getElementById("titleName").removeAttribute("hidden");
  });
});

// DataTables
$(function () {
  var table = $("#donor-counts")["DataTable"]({
    //"ajax": "data/arrays.txt",
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
        $(triggerRow.node).data("donor-count-id") +
        "/move",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: JSON.stringify({
        id: $(triggerRow.node).data("donor-count-id"),
        description: $(triggerRow.node).data("donor-count-desc"),
        sortOrder: triggerRow.newPosition + 1, //1-indexable
        lowerBound: $(triggerRow.node).data("donor-count-lower-bound"),
        upperBound: $(triggerRow.node).data("donor-count-upper-bound"),
      }),
    });
  });
});
