function RadioBinding(value, label, sortOrder) {
    this.value = value;
    this.label = label;
    this.sortOrder = sortOrder;
}

function Lookup() {
    var initValue = "Initial value to get it to build the vm model correctly.";
    this.ageRanges = ko.observableArray([new RadioBinding(0, initValue, 0)]);
    this.donorCounts = ko.observableArray([new RadioBinding(0, initValue, 0)]);
    this.preservationTypes = ko.observableArray([
        new RadioBinding(0, initValue, 0),
    ]);
    this.materialTypes = ko.observableArray([new RadioBinding(0, initValue, 0)]);
    this.macroscopicAssessments = ko.observableArray([
        new RadioBinding(0, initValue, 0),
    ]);
    this.percentages = ko.observableArray([new RadioBinding(0, initValue, 0)]);
}

var lookup = new Lookup();

function MaterialPreservationDetail(
  materialType,
  preservationType,
  percentage,
  macroscopicAssessment
) {
  var _this = this;

  this.getRadioBindingLabel = function (lookupArray, matchingId) {
	var matches = lookupArray.filter(function (radioBinding) {
	  return radioBinding.value == matchingId;
	});

	return matches.length > 0 ? matches[0].label : "";
  };

  this.materialType = ko.observable(materialType).extend({
	min: { params: 1, message: "Please select a material type." },
  });
  this.preservationType = ko.observable(preservationType).extend({
	min: { params: 1, message: "Please select a preservation type." },
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

  this.preservationTypeDescription = ko.computed(function () {
	return _this.getRadioBindingLabel(
	  lookup.preservationTypes(),
	  _this.preservationType()
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
  materialType,
  preservationType,
  percentage,
  macroscopicAssessment
) {
  this.modalModeAdd = "Add";
  this.modalModeEdit = "Update";
  this.modalModeCopy = "Copy";

  this.mode = ko.observable(this.modalModeAdd);

  this.materialPreservationDetail = ko.observable(
	new MaterialPreservationDetail(
	  materialType,
	  preservationType,
	  percentage,
	  macroscopicAssessment
	)
  );
}

function AppViewModel() {
  var _this = this;
  this.donorCount = ko.observable(0);
  this.modal = new MaterialPreservationDetailModal(0, 0, 1, 0);
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
	  console.log(_this.modal.materialPreservationDetail().percentage());
  };

  this.openModalForCopy = function () {
	_this.modal.mode(_this.modal.modalModeCopy);
	_this.showModal();
  };

  this.openModalForEdit = function () {
	_this.modal.mode(_this.modal.modalModeEdit);
	_this.showModal();
  };

  this.modalSubmit = function () {

	console.log(_this.modal.materialPreservationDetail().percentage());

	//check to ensure details are unique
	_this.validateDetailUnique(_this.modal.materialPreservationDetail());

	//Copy is the same as Add for the purposes of submission
	if (
	  _this.modal.mode() == _this.modal.modalModeAdd ||
	  _this.modal.mode() == _this.modal.modalModeCopy
	) {
	  if (_this.modal.materialPreservationDetail().errors().length === 0) {
		var newMaterialPreservationDetail = new MaterialPreservationDetail(
		  _this.modal.materialPreservationDetail().materialType(),
		  _this.modal.materialPreservationDetail().preservationType(),
		  _this.modal.materialPreservationDetail().percentage(),
		  _this.modal.materialPreservationDetail().macroscopicAssessment()
		);

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
		  .percentage(_this.modal.materialPreservationDetail().percentage());
		_this
		  .currentlyEdited()
		  .macroscopicAssessment(
			_this.modal.materialPreservationDetail().macroscopicAssessment()
		  );

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

		console.log(_this.modal.materialPreservationDetail().percentage());

		//check to ensure details are unique
		_this.validateDetailUnique(_this.modal.materialPreservationDetail());

		//Copy is the same as Add for the purposes of submission
		if (
			_this.modal.mode() == _this.modal.modalModeAdd ||
			_this.modal.mode() == _this.modal.modalModeCopy
		) {
			if (_this.modal.materialPreservationDetail().errors().length === 0) {
				var newMaterialPreservationDetail = new MaterialPreservationDetail(
					_this.modal.materialPreservationDetail().materialType(),
					_this.modal.materialPreservationDetail().preservationType(),
					null,
					_this.modal.materialPreservationDetail().macroscopicAssessment()
				);

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
					.percentage(_this.modal.materialPreservationDetail().percentage());
				_this
					.currentlyEdited()
					.macroscopicAssessment(
						_this.modal.materialPreservationDetail().macroscopicAssessment()
					);

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
	_this.modal.materialPreservationDetail().percentage(1);
	_this.modal.materialPreservationDetail().macroscopicAssessment(3); // Default: 'Not-Applicable'

	_this.modal.materialPreservationDetail().materialType.isModified(false);
	_this.modal.materialPreservationDetail().preservationType.isModified(false);
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
	_this.modal.materialPreservationDetail().percentage(details.percentage());
	_this.modal
	  .materialPreservationDetail()
	  .macroscopicAssessment(details.macroscopicAssessment());

	_this.openModalForEdit();
  };

  this.copyDetails = function (details) {
	_this.modal
	  .materialPreservationDetail()
	  .materialType(details.materialType());
	_this.modal
	  .materialPreservationDetail()
	  .preservationType(details.preservationType());
	_this.modal.materialPreservationDetail().percentage(details.percentage());
	_this.modal
	  .materialPreservationDetail()
	  .macroscopicAssessment(details.macroscopicAssessment());

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
		  item.macroscopicAssessment() === details.macroscopicAssessment()
		);
	  }
	);
	details.isDuplicate(match !== null);
  };
}

var sampleSetVM;

$(function () {
  ko.validation.rules["mustEqual"] = {
	validator(val, otherVal) {
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
