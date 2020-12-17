// Modals
var adacCountyVM;

function County(id, name, countryId) {
    this.id = id;
    this.name = ko.observable(name);
    this.countryId = ko.observable(countryId);
}

function CountyModal(id, name, countryId, countries) {
    this.modalModeAdd = "Add";
    this.modalModeEdit = "Update";

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
        var form = $(e.target); // get form as a jquery object

        // Get Action Type
        var resourceUrl = form.data("resource-url")
        var action = _this.modal.mode();
        if (action == 'Add') {
            var ajaxType = 'POST'
            var url = resourceUrl;
        } else if (action == 'Update') {
            var ajaxType = 'PUT';
            var url = resourceUrl + '/' + $(e.target.Id).val();
        }
        var successRedirect = action.toLowerCase() + "-success-redirect";

        // Make AJAX Call
        $.ajax({
            url: url,
            type: ajaxType,
            dataType: 'json',
            data: form.serialize(),
            success: function (data, textStatus, xhr) {
                _this.dialogErrors.removeAll();
                _this.hideModal();
                window.location.href =
                    form.data(successRedirect) + "?Name=" + data.name;
            },
            error: function (data, xhr, textStatus, errorThrown) {
                _this.dialogErrors.removeAll();
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
