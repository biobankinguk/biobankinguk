function RadioBinding(value, label, sortOrder) {
  this.value = value;
  this.label = label;
  this.sortOrder = sortOrder;
}

function DropDownBinding(id, description) {
  this.value = id;
  this.label = description;
}

function preservationValidation() {
  var selectedOption = $("input[name='radPreservation']:checked").val();
  $("input[name='radPreservationType']").attr("disabled", true);
  $("input[name='radPreservationType']").attr("checked", false);
  //Resetting the stored selected value
  sampleSetVM.modal.materialPreservationDetail().preservationType(null);

  //AJAX Update
  $.ajax({
    url:
      "/api/" +
      "StorageTemperature" +
      "/" +
      selectedOption +
      "/preservationtype",
    type: "GET",
    success: function (data) {
      data.forEach(function (value, i) {
        var temp = $(
          "input[name='radPreservationType'][value='" + value + "']"
        );
        temp.attr("disabled", false);
      });
    },
  });
}

function extractionValidation() {
  var selectedOption = $("input[name='radMaterial']:checked").val();

  //AJAX Update
  $.ajax({
    url:
      "/api/" + "MaterialType" + "/" + selectedOption + "/extractionprocedure",
    type: "GET",
    success: function (data) {
      lookup.materialExtractionProcedures(
        ko.utils.arrayMap(data, function (x) {
          return new DropDownBinding(x.id, x.value);
        })
      );
    },
  });
}

function Lookup() {
  var initValue = "Initial value to get it to build the vm model correctly.";
  this.ageRanges = ko.observableArray([new RadioBinding(0, initValue, 0)]);
  this.donorCounts = ko.observableArray([new RadioBinding(0, initValue, 0)]);
  this.preservationTypes = ko.observableArray([
    new RadioBinding(0, initValue, 0),
  ]);
  this.storageTemperatures = ko.observableArray([
    new RadioBinding(0, initValue, 0),
  ]);
  this.materialTypes = ko.observableArray([new RadioBinding(0, initValue, 0)]);
  this.macroscopicAssessments = ko.observableArray([
    new RadioBinding(0, initValue, 0),
  ]);
  this.percentages = ko.observableArray([new RadioBinding(0, initValue, 0)]);
  this.extractionProcedures = ko.observableArray([
    new DropDownBinding("", initValue),
  ]);
  this.materialExtractionProcedures = ko.observableArray([
    new DropDownBinding("", initValue),
  ]);
}

var lookup = new Lookup();

function MaterialPreservationDetail(
  id,
  materialType,
  preservationType,
  storageTemperature,
  percentage,
  macroscopicAssessment,
  extractionProcedure
) {
  var _this = this;

  this.getRadioBindingLabel = function (lookupArray, matchingId) {
    var matches = lookupArray.filter(function (radioBinding) {
      return radioBinding.value == matchingId;
    });

    return matches.length > 0 ? matches[0].label : "";
  };

  this.id = id;

  this.materialType = ko.observable(materialType).extend({
    min: { params: 1, message: "Please select a material type." },
  });
  this.extractionProcedure = ko.observable(extractionProcedure);
  this.preservationType = ko.observable(preservationType);
  this.storageTemperature = ko.observable(storageTemperature).extend({
    min: { params: 1, message: "Please select a storage temperature." },
  });
  this.percentage = ko.observable(percentage);
  this.macroscopicAssessment = ko.observable(macroscopicAssessment).extend({
    min: {
      params: 1,
      message:
        "Please select whether these samples are affected by macroscopic assessment.",
    },
  });
  this.isDuplicate = ko.observable(false).extend({
    mustEqual: {
      params: false,
      message: "The specified details already exist for this sample set.",
    },
  });
  this.showErrors = ko.observable(false);

  this.materialTypeDescription = ko.computed(function () {
    return _this.getRadioBindingLabel(
      lookup.materialTypes(),
      _this.materialType()
    );
  });

  this.extractionProcedureDescription = ko.computed(function () {
    return _this.getRadioBindingLabel(
      lookup.extractionProcedures(),
      _this.extractionProcedure()
    );
  });

  this.preservationTypeDescription = ko.computed(function () {
    return _this.getRadioBindingLabel(
      lookup.preservationTypes(),
      _this.preservationType()
    );
  });

  this.storageTemperatureDescription = ko.computed(function () {
    return _this.getRadioBindingLabel(
      lookup.storageTemperatures(),
      _this.storageTemperature()
    );
  });

  this.macroscopicAssessmentDescription = ko.computed(function () {
    return _this.getRadioBindingLabel(
      lookup.macroscopicAssessments(),
      _this.macroscopicAssessment()
    );
  });

  this.percentageDescription = ko.computed(function () {
    return _this.getRadioBindingLabel(lookup.percentages(), _this.percentage());
  });

  this.errors = ko.validation.group(this);
}

function MaterialPreservationDetailModal(
  id,
  materialType,
  preservationType,
  storageTemperature,
  percentage,
  macroscopicAssessment,
  extractionProcedure
) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";
  this.modalModeCopy = "Copy";

  this.mode = ko.observable(this.modalModeAdd);

  this.materialPreservationDetail = ko.observable(
    new MaterialPreservationDetail(
      id,
      materialType,
      preservationType,
      storageTemperature,
      percentage,
      macroscopicAssessment,
      extractionProcedure
    )
  );
}

function AppViewModel() {
  var _this = this;
  this.donorCount = ko.observable(0);
  this.modal = new MaterialPreservationDetailModal(0, 0, 0, 0, 1, 0, "");
  this.materialPreservationDetails = ko.observableArray([]);
  this.currentlyEdited = ko.observable(null);

  this.lookup = lookup;

  this.donorCountDescription = ko.computed(function () {
    return lookup.donorCounts
      ? lookup.donorCounts()[_this.donorCount()].label
      : "";
  });

  this.donorCountId = ko.computed(function () {
    return lookup.donorCounts
      ? lookup.donorCounts()[_this.donorCount()].value
      : 0;
  });

  this.anyMaterialPreservationDetails = ko.computed(function () {
    return _this.materialPreservationDetails().length > 0;
  });

  this.showModal = function () {
    $("#materialPreservationDetail").modal("show");
  };

  this.hideModal = function () {
    $("#materialPreservationDetail").modal("hide");
  };

  this.openModalForAdd = function () {
    _this.modal.mode(_this.modal.modalModeAdd);
    _this.showModal();
  };

  this.openModalForCopy = function () {
    _this.modal.mode(_this.modal.modalModeCopy);
    _this.showModal();
    extractionValidation();
  };

  this.openModalForEdit = function () {
    _this.modal.mode(_this.modal.modalModeEdit);
    _this.showModal();

    extractionValidation();
    preservationValidation();
  };

  this.validateExtractionOnSubmit = function () {
    var eplabel = _this.modal
      .materialPreservationDetail()
      .getRadioBindingLabel(
        lookup.materialExtractionProcedures(),
        _this.modal.materialPreservationDetail().extractionProcedure()
      );
    return eplabel != "" ? true : false;
  };

  this.modalSubmit = function () {
    //check to ensure details are unique
    _this.validateDetailUnique(_this.modal.materialPreservationDetail());

    //validate extraction procedure
    var extractionProcedure = _this.validateExtractionOnSubmit()
      ? _this.modal.materialPreservationDetail().extractionProcedure()
      : null;

    //Copy is the same as Add for the purposes of submission
    if (
      _this.modal.mode() == _this.modal.modalModeAdd ||
      _this.modal.mode() == _this.modal.modalModeCopy
    ) {
      if (_this.modal.materialPreservationDetail().errors().length === 0) {
        if (_this.modal.materialPreservationDetail().preservationType() === 0) {
          var newMaterialPreservationDetail = new MaterialPreservationDetail(
            _this.modal.materialPreservationDetail().id,
            _this.modal.materialPreservationDetail().materialType(),
            null,
            _this.modal.materialPreservationDetail().storageTemperature(),
            _this.modal.materialPreservationDetail().percentage(),
            _this.modal.materialPreservationDetail().macroscopicAssessment(),
            extractionProcedure
          );
        } else {
          var newMaterialPreservationDetail = new MaterialPreservationDetail(
            _this.modal.materialPreservationDetail().id,
            _this.modal.materialPreservationDetail().materialType(),
            _this.modal.materialPreservationDetail().preservationType(),
            _this.modal.materialPreservationDetail().storageTemperature(),
            _this.modal.materialPreservationDetail().percentage(),
            _this.modal.materialPreservationDetail().macroscopicAssessment(),
            extractionProcedure
          );
        }

        _this.materialPreservationDetails.push(newMaterialPreservationDetail);

        _this.resetModalValues();

        _this.hideModal();
      } else {
        _this.modal.materialPreservationDetail().errors.showAllMessages();
        _this.modal.materialPreservationDetail().showErrors(true);
      }
    } else {
      if (_this.modal.materialPreservationDetail().errors().length === 0) {
        _this
          .currentlyEdited()
          .materialType(
            _this.modal.materialPreservationDetail().materialType()
          );
        _this
          .currentlyEdited()
          .preservationType(
            _this.modal.materialPreservationDetail().preservationType()
          );
        _this
          .currentlyEdited()
          .storageTemperature(
            _this.modal.materialPreservationDetail().storageTemperature()
          );
        _this
          .currentlyEdited()
          .percentage(_this.modal.materialPreservationDetail().percentage());
        _this
          .currentlyEdited()
          .macroscopicAssessment(
            _this.modal.materialPreservationDetail().macroscopicAssessment()
          );

        _this.currentlyEdited().extractionProcedure(extractionProcedure);

        _this.resetModalValues();
        _this.currentlyEdited(null);

        _this.hideModal();
      } else {
        _this.modal.materialPreservationDetail().errors.showAllMessages();
        _this.modal.materialPreservationDetail().showErrors(true);
      }
    }
  };

  this.modalSubmitNoPercentage = function () {
    //check to ensure details are unique
    _this.validateDetailUnique(_this.modal.materialPreservationDetail());

    //validate extraction procedure
    var extractionProcedure = _this.validateExtractionOnSubmit()
      ? _this.modal.materialPreservationDetail().extractionProcedure()
      : null;

    //Copy is the same as Add for the purposes of submission
    if (
      _this.modal.mode() == _this.modal.modalModeAdd ||
      _this.modal.mode() == _this.modal.modalModeCopy
    ) {
      if (_this.modal.materialPreservationDetail().errors().length === 0) {
        if (_this.modal.materialPreservationDetail().preservationType() === 0) {
          var newMaterialPreservationDetail = new MaterialPreservationDetail(
            _this.modal.materialPreservationDetail().id,
            _this.modal.materialPreservationDetail().materialType(),
            null,
            _this.modal.materialPreservationDetail().storageTemperature(),
            null,
            _this.modal.materialPreservationDetail().macroscopicAssessment(),
            extractionProcedure
          );
        } else {
          var newMaterialPreservationDetail = new MaterialPreservationDetail(
            _this.modal.materialPreservationDetail().id,
            _this.modal.materialPreservationDetail().materialType(),
            _this.modal.materialPreservationDetail().preservationType(),
            _this.modal.materialPreservationDetail().storageTemperature(),
            null,
            _this.modal.materialPreservationDetail().macroscopicAssessment(),
            extractionProcedure
          );
        }

        _this.materialPreservationDetails.push(newMaterialPreservationDetail);

        _this.resetModalValues();

        _this.hideModal();
      } else {
        _this.modal.materialPreservationDetail().errors.showAllMessages();
        _this.modal.materialPreservationDetail().showErrors(true);
      }
    } else {
      if (_this.modal.materialPreservationDetail().errors().length === 0) {
        _this
          .currentlyEdited()
          .materialType(
            _this.modal.materialPreservationDetail().materialType()
          );
        _this
          .currentlyEdited()
          .preservationType(
            _this.modal.materialPreservationDetail().preservationType()
          );
        _this
          .currentlyEdited()
          .storageTemperature(
            _this.modal.materialPreservationDetail().storageTemperature()
          );
        _this
          .currentlyEdited()
          .percentage(_this.modal.materialPreservationDetail().percentage());
        _this
          .currentlyEdited()
          .macroscopicAssessment(
            _this.modal.materialPreservationDetail().macroscopicAssessment()
          );

        _this.currentlyEdited().extractionProcedure(extractionProcedure);

        _this.resetModalValues();
        _this.currentlyEdited(null);

        _this.hideModal();
      } else {
        _this.modal.materialPreservationDetail().errors.showAllMessages();
        _this.modal.materialPreservationDetail().showErrors(true);
      }
    }
  };

  this.resetModalValues = function () {
    _this.modal.materialPreservationDetail().materialType(0);
    _this.modal.materialPreservationDetail().preservationType(0);
    _this.modal.materialPreservationDetail().storageTemperature(0);
    _this.modal.materialPreservationDetail().percentage(1);
    _this.modal.materialPreservationDetail().macroscopicAssessment(3); // Default: 'Not-Applicable'
    _this.modal.materialPreservationDetail().extractionProcedure(null);

    _this.modal.materialPreservationDetail().materialType.isModified(false);
    _this.modal
      .materialPreservationDetail()
      .extractionProcedure.isModified(false);
    _this.modal.materialPreservationDetail().preservationType.isModified(false);
    _this.modal
      .materialPreservationDetail()
      .storageTemperature.isModified(false);
    _this.modal.materialPreservationDetail().percentage.isModified(false);
    _this.modal
      .materialPreservationDetail()
      .macroscopicAssessment.isModified(false);

    _this.modal.materialPreservationDetail().isDuplicate(false);
    _this.modal.materialPreservationDetail().isDuplicate.isModified(false);

    _this.modal.materialPreservationDetail().showErrors(false);
  };

  this.deleteDetails = function (details) {
    _this.materialPreservationDetails.remove(details);
  };

  this.editDetails = function (details) {
    _this.currentlyEdited(details);

    _this.modal
      .materialPreservationDetail()
      .materialType(details.materialType());
    _this.modal
      .materialPreservationDetail()
      .preservationType(details.preservationType());
    _this.modal
      .materialPreservationDetail()
      .storageTemperature(details.storageTemperature());
    _this.modal.materialPreservationDetail().percentage(details.percentage());
    _this.modal
      .materialPreservationDetail()
      .macroscopicAssessment(details.macroscopicAssessment());
    _this.modal
      .materialPreservationDetail()
      .extractionProcedure(details.extractionProcedure());
    _this.openModalForEdit();
  };

  this.copyDetails = function (details) {
    _this.modal
      .materialPreservationDetail()
      .materialType(details.materialType());
    _this.modal
      .materialPreservationDetail()
      .preservationType(details.preservationType());
    _this.modal
      .materialPreservationDetail()
      .storageTemperature(details.storageTemperature());
    _this.modal.materialPreservationDetail().percentage(details.percentage());
    _this.modal
      .materialPreservationDetail()
      .macroscopicAssessment(details.macroscopicAssessment());
    _this.modal
      .materialPreservationDetail()
      .extractionProcedure(details.extractionProcedure());
    _this.openModalForCopy();
  };

  this.validateDetailUnique = function (details) {
    var match = ko.utils.arrayFirst(
      _this.materialPreservationDetails(),
      function (item) {
        return (
          item !== _this.currentlyEdited() && //doesn't matter if the same item matches itself when editing
          item.materialType() === details.materialType() &&
          item.preservationType() === details.preservationType() &&
          item.storageTemperature() === details.storageTemperature() &&
          item.macroscopicAssessment() === details.macroscopicAssessment() &&
          item.extractionProcedure() === details.extractionProcedure()
        );
      }
    );
    details.isDuplicate(match != null);
  };
}

var sampleSetVM;

$(function () {
  ko.validation.rules["mustEqual"] = {
    validator: function (val, otherVal) {
      return val === otherVal;
    },
    message: "The field must equal {0}",
  };
  ko.validation.registerExtenders();

  //initialise VM after the validation extender is registered
  sampleSetVM = new AppViewModel();
  sampleSetVM.resetModalValues(); // Used to init values
  ko.applyBindings(sampleSetVM);
});
