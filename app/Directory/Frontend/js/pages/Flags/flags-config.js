

$(function () {
    // Data Table
    $("#flags-config")["DataTable"]({
        paging: false,
        info: false,
        autoWidth: false,
        language: {
            search: "Filter: ",
        },
    });

    // Checkboxes emit booleans
    $('input[type=checkbox]').each(function () {
        $(this).prop("checked", $(this).val() == "true");
    });
    $('input[type=checkbox]').change(function () {
        $(this).val($(this).prop("checked"));
    });

    
    // Form Submission
    $("#flags-config-form").submit(function (e) {
        e.preventDefault();

        // Get config data
        var data = $(this).serializeArray({ checkboxesAsBools: true }).map(function (item) {
            return {
                key: item.name,
                value: item.value
            }
        });

        // Support for unchecked checkboxes
        data = data.concat(
            $('#flags-config-form input[type=checkbox]:not(:checked)').map(function () {
                return {
                    key: this.name,
                    value: "false"
                }
            }).get()
        );

        $.ajax({
            type: "POST",
            url: "UpdateFlagsConfig",
            data: { values: data },
            dataType: "json",
            success: function (data) {
                window.location.replace(data.redirect);
            }
        });
    });

    

});
