// Modals
var adacAgeRangeVM;

function AgeRange(
  id,
  description,
  sortOrder,
  lowerBound,
  upperBound,
  lowerDuration,
  upperDuration
) {
  this.id = id;
  this.description = ko.observable(description);
  this.sortOrder = sortOrder;
  this.lowerBound = ko.observable(lowerBound);
  this.upperBound = ko.observable(upperBound);
  this.lowerDuration = ko.observable(lowerDuration);
  this.upperDuration = ko.observable(upperDuration);
}

function AgeRangeModal(
  id,
  description,
  sortOrder,
  lowerBound,
  upperBound,
  lowerDuration,
  upperDuration
) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.ageRange = ko.observable(
    new AgeRange(
      id,
      description,
      sortOrder,
      lowerBound,
      upperBound,
      lowerDuration,
      upperDuration
    )
  );
}

function AdacAgeRangeViewModel() {
  var _this = this;

  this.modalId = "#age-ranges-modal";
  this.modal = new AgeRangeModal(0, "", 0, 0, 0);
  this.dialogErrors = ko.observableArray([]);

  this.showModal = function () {
    _this.dialogErrors.removeAll(); //clear errors on a new show
    $(_this.modalId).modal("show");
  };

  this.hideModal = function () {
    $(_this.modalId).modal("hide");
  };

  this.openModalForAdd = function () {
    // Return to default dropdown options
    document.getElementById("LowerBound").readOnly = false;
    document.getElementById("UpperBound").readOnly = false;
    var lowerDuration = document.getElementById("lowerDuration");
    lowerDuration.disabled = false;
    lowerDuration.value = "M";
    var upperDuration = document.getElementById("upperDuration");
    upperDuration.disabled = false;
    upperDuration.value = "M";

    _this.modal.mode(_this.modal.modalModeAdd);
    _this.modal.ageRange(new AgeRange(0, "", 0, 0, 0));
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var ageRange = $(event.currentTarget).data("age-range");

    // Selected value in dropdown
    var lowerDuration = ageRange.lowerBound.replace(/[^a-zA-Z]+/g, "");
    if (lowerDuration == "") {
      document.getElementById("lowerDuration").value = "N/A";
    } else {
      document.getElementById("lowerDuration").value = lowerDuration.charAt(0);
    }

    var upperDuration = ageRange.upperBound.replace(/[^a-zA-Z]+/g, "");
    if (upperDuration == "") {
      document.getElementById("upperDuration").value = "N/A";
    } else {
      document.getElementById("upperDuration").value = upperDuration.charAt(0);
    }

    // Disables all input options for data not recorded entry (both bounds being null)
    if (ageRange.lowerBound == "" && ageRange.upperBound == "") {
      document.getElementById("LowerBound").readOnly = true;
      document.getElementById("UpperBound").readOnly = true;
      document.getElementById("lowerDuration").disabled = true;
      document.getElementById("upperDuration").disabled = true;
    } else {
      document.getElementById("LowerBound").readOnly = false;
      document.getElementById("UpperBound").readOnly = false;
      document.getElementById("lowerDuration").disabled = false;
      document.getElementById("upperDuration").disabled = false;
    }

    _this.modal.ageRange(
      new AgeRange(
        ageRange.id,
        ageRange.description,
        ageRange.sortOrder,
        ageRange.lowerBound.replace(/[a-z]/gi, "").replace(/\s/g, ""),
        ageRange.upperBound.replace(/[a-z]/gi, "").replace(/\s/g, ""),
        ageRange.lowerDuration,
        ageRange.upperDuration
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
  $("#modal-age-ranges-form").submit(function (e) {
    adacAgeRangeVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
    e.preventDefault();

    var $link = $(this);
    var linkData = $link.data("age-range");
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

  adacAgeRangeVM = new AdacAgeRangeViewModel();
  ko.applyBindings(adacAgeRangeVM);
});

function checkBound(obj, id) {
  var upperInput = document.getElementById(id);
  upperInput.disabled = obj.value == "N/A";
}

// DataTables
$(function () {
  var table = $("#age-ranges")["DataTable"]({
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

    // Encode to API format.
    var lowerDuration = $(triggerRow.node)
      .data("age-range-lowerbound")
      .split(" ")[1][0];
    var upperDuration = $(triggerRow.node)
      .data("age-range-upperbound")
      .split(" ")[1][0];
    var lowerBound = $(triggerRow.node)
      .data("age-range-lowerbound")
      .replace(/[a-z]/gi, "")
      .replace(/\s/g, "");
    var upperBound = $(triggerRow.node)
      .data("age-range-upperbound")
      .replace(/[a-z]/gi, "")
      .replace(/\s/g, "");

    //AJAX Update
    $.ajax({
      url:
        $(triggerRow.node).data("resource-url") +
        "/" +
        $(triggerRow.node).data("age-range-id") +
        "/move",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: JSON.stringify({
        id: $(triggerRow.node).data("age-range-id"),
        description: $(triggerRow.node).data("age-range-desc"),
        sortOrder: triggerRow.newPosition + 1, //1-indexable,
        lowerBound: lowerBound,
        upperBound: upperBound,
        lowerDuration: lowerDuration,
        upperDuration: upperDuration,
      }),
    });
  });
});
