function ExtractionProcedure(ontologyTermId, description, otherTerms) {
    this.ontologyTermId = ko.observable(ontologyTermId);
    this.description = ko.observable(description);
    this.otherTerms = ko.observableArray(otherTerms);
}

function ExtractionProcedureModal(ontologyTermId, description, otherTerms) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

    this.mode = ko.observable(this.modalModeAdd);

    this.extractionProcedure = ko.observable(
        new ExtractionProcedure(ontologyTermId, description, otherTerms)
    );
}

function AdacExtractionProcedureViewModel() {

    var _this = this;

    this.modalId = "#extraction-procedure-modal";
    this.modal = new ExtractionProcedureModal("", "", []);
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
        _this.modal.extractionProcedure(new ExtractionProcedure("", "",[]));
        _this.setPartialEdit(false);
        _this.showModal();
    };

  this.openModalForEdit = function (_, event) {
    _this.modal.mode(_this.modal.modalModeEdit);

      var extractionProcedure = $(event.currentTarget).data("extraction-procedure");
      let otherTerms = (extractionProcedure.OtherTerms
          ? extractionProcedure.OtherTerms.split(",").map(item => item.trim())
          : extractionProcedure.OtherTerms)

    _this.modal.extractionProcedure(
      new ExtractionProcedure(
          extractionProcedure.OntologyTermId,
          extractionProcedure.Description,
          otherTerms

      )
    );
    $("#OntologyTermId").prop('readonly', true);
    _this.setPartialEdit($(event.currentTarget).data("partial-edit"));
    _this.showModal();
  };

    this.modalSubmit = function (e) {
        e.preventDefault();
        var form = $(e.target); // get form as a jquery object

        //Concatenate other terms (exclude null/empty/whitespace strings)
        $("#OtherTerms").val(_this.modal.extractionProcedure().otherTerms().filter(x => x && x.trim()).join(','));

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

    //Turns on/off partial editing of some input fields
    this.setPartialEdit = function (flag) {
        if (flag == true)
            $("#Description").prop('readonly', true);
        else 
            $("#Description").prop('readonly', false);
    }

    this.addOtherTerms = function () {
        _this.modal.extractionProcedure().otherTerms.push("");
    }
    this.removeOtherTerms = function (idx) {
        _this.modal.extractionProcedure().otherTerms.splice(idx,1)
    }
}

var adacExtractionProcedureVM;

$(function () {
    $("#extraction-procedures")["DataTable"]({
        columnDefs: [
            { orderable: false, targets: 4 },
            { width: "180px", targets: 4 },
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
      var linkData = $link.data("refdata-model")
      var url = $link.data("resource-url") + "/" + linkData.OntologyTermId;

      bootbox.confirm("Are you sure you want to delete " + linkData.Description + "?",
          function (confirmation) {
              if (confirmation) {
                  deleteRefData(url, $link.data("success-redirect"), $link.data("refdata-type"));
              }
          }
      );
  });

    adacExtractionProcedureVM = new AdacExtractionProcedureViewModel();

    ko.applyBindings(adacExtractionProcedureVM);
});