// Modals
var adacCountyVM;

function County(id, name, countryId) {
    this.id = id;
    this.name = ko.observable(name);
    this.countryId = ko.observable(countryId);
}

function CountyModal(id, name, countryId, countries) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Edit";

    this.mode = ko.observable(this.modalModeAdd);

    this.county = ko.observable(
        new County(id, name, countryId)
    );

    this.countries = ko.observableArray(countries);
}

function AdacCountyViewModel() {
    var _this = this;
    this.modalId = "#county-modal";

    this.countries = $(this.modalId).data("countries");

    this.modal = new CountyModal(0, "", 0, this.countries);
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
        _this.modal.county(new County(0, "", 0, this.countries));
        _this.showModal();
    };

    this.openModalForEdit = function (_, event) {
        _this.modal.mode(_this.modal.modalModeEdit);

        var county = $(event.currentTarget).data("county");

        _this.modal.county(
            new County(
                county.Id,
                county.Name,
                county.CountryId,
                this.countries
            )
        );

        _this.showModal();
    };

    this.modalSubmit = function (e) {
        e.preventDefault();

        // Get Action Type
        var action = _this.modal.mode().toLowerCase();
        var url = `/api/County/${action}CountyAjax`;

        console.log($(e.target).serialize());

        // Make AJAX Call
        $.post(url, $(e.target).serialize(), function (data) {

            // Clear any previous errors
            _this.dialogErrors.removeAll();

            if (data.success) {
                _this.hideModal();
                window.location.replace(data.redirect);
            }
            else {
                if (Array.isArray(data.errors)) {
                    for (var error of data.errors) {
                        _this.dialogErrors.push(error);
                    }
                }
            }
        });
    };
}

$(function () {
    $("#modal-county-form").submit(function (e) {
        adacCountyVM.modalSubmit(e);
    });

    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);

        bootbox.confirm("Are you sure you want to delete " + $link .data("county") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });

    adacCountyVM = new AdacCountyViewModel();
    ko.applyBindings(adacCountyVM);
});

// DataTables
$(function () {
    var table = $("#county")["DataTable"]({
        paging: false,
        info: false,
        autoWidth: false,
        language: {
            search: "Filter: ",
        },
    });
});
