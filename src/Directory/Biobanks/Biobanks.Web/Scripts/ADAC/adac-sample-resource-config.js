$(function () {
    // Data Table
    $("#sample-resource-config")["DataTable"]({
        paging: false,
        order: [1, 'desc'],
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
    $("#sample-resource-config-form").submit(function (e) {
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
            $('#sample-resource-config-form input[type=checkbox]:not(:checked)').map(function () {
                return {
                    key: this.name,
                    value: "false"
                }
            }).get()
        );

        $.ajax({
            type: "POST",
            url: "UpdateSiteConfig",
            data: { values: data },
            dataType: "json",
            success: function (data) {
                window.location.replace("SampleResourceConfigSuccess");
            }
        });
    });
});
