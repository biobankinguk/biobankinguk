function ExtractionProcedure(
  ontologyTermId,
  description,
  materialTypeIds,
  displayOnDirectory
) {
  this.ontologyTermId = ko.observable(ontologyTermId);
  this.description = ko.observable(description);
  this.materialTypeIds = ko.observableArray(materialTypeIds);
  this.displayOnDirectory = ko.observable(displayOnDirectory);
}

function ExtractionProcedureModal(
  ontologyTermId,
  description,
  materialTypeIds,
  materialTypes,
  displayOnDirectory
) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";

  this.mode = ko.observable(this.modalModeAdd);

  this.extractionProcedure = ko.observable(
    new ExtractionProcedure(
      ontologyTermId,
      description,
      materialTypeIds,
      displayOnDirectory
    )
  );
  this.materialTypes = materialTypes;
}

function AdacExtractionProcedureViewModel() {
  var _this = this;

  this.modalId = "#extraction-procedure-modal";
  this.materialTypes = $(this.modalId).data("material-types");

  this.modal = new ExtractionProcedureModal(
    "",
    "",
    0,
    this.materialTypes,
    false
  );
  this.dialogErrors = ko.observableArray([]);

  this.showModal = function () {
    _this.dialogErrors.removeAll(); //clear errors on a new show
    $(_this.modalId).modal("show");
  };

  this.hideModal = function () {
    $(_this.modalId).modal("hide");
  };

  this.openModalForAdd = function () {
    $("#OntologyTermId").prop("readonly", false);

    _this.modal.mode(_this.modal.modalModeAdd);
    _this.modal.extractionProcedure(new ExtractionProcedure("", "", 0, false));
    _this.setPartialEdit(false);
    _this.showModal();
  };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

    var extractionProcedure = $(event.currentTarget).data(
      "extraction-procedure"
    );

    _this.modal.extractionProcedure(
      new ExtractionProcedure(
        extractionProcedure.ontologyTermId,
        extractionProcedure.description,
        extractionProcedure.materialTypeIds,
        extractionProcedure.displayOnDirectory
      )
    );
    $("#OntologyTermId").prop("readonly", true);
    _this.setPartialEdit($(event.currentTarget).data("partial-edit"));
    _this.showModal();
  };

  this.modalSubmit = function (e) {
    e.preventDefault();
    var form = $(e.target); // get form as a jquery object

    // Get Form Data
    var data = serializeFormData(form);
    data.MaterialTypeIds = _this.modal.extractionProcedure().materialTypeIds();
    data.DisplayOnDirectory = data.DisplayOnDirectory === "true";

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

  //Turns on/off partial editing of some input fields
  this.setPartialEdit = function (flag) {
    if (flag == true) $("#Description").prop("readonly", true);
    else $("#Description").prop("readonly", false);
  };

  this.addMaterialType = function () {
    _this.modal.extractionProcedure().materialTypeIds.push(0);
  };
  this.removeMaterialType = function (idx) {
    _this.modal.extractionProcedure().materialTypeIds.splice(idx, 1);
  };
}

var adacExtractionProcedureVM;

$(function () {
  $("#extraction-procedures")["DataTable"]({
    columnDefs: [
      { orderable: false, targets: 5 },
      { width: "180px", targets: 5 },
    ],
    paging: false,
    info: false,
    autoWidth: false,
    language: {
      search: "Filter: ",
    },
  });

  $("#modal-extraction-procedure-form").submit(function (e) {
    adacExtractionProcedureVM.modalSubmit(e);
  });

  $(".delete-confirm").click(function (e) {
    e.preventDefault();

    var $link = $(this);
    var linkData = $link.data("refdata-model");
    var url = $link.data("resource-url") + "/" + linkData.ontologyTermId;

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

  adacExtractionProcedureVM = new AdacExtractionProcedureViewModel();

  ko.applyBindings(adacExtractionProcedureVM);
});
