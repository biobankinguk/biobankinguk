$(function () {
    $("#adac-biobanks").DataTable({
        "columnDefs": [{ "orderable": false, "targets": [2, 3, 4, 5] }], // remove the sort icon for columns with just buttons/links in
        "paging": false,
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });

    $(".resend-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            $link.data("admin-name") +
            " (" + $link.data("admin-email") + ") " +
            "has not yet confirmed their account.<br/>Do you want to resend their confirmation link?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            });
    });

    $(".suspend-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you want to suspend " + $link.data("biobank-name") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            });
    });

    $(".unsuspend-confirm").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you want to unsuspend " + $link.data("biobank-name") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            });
    });

    $(".confirm-admin-delete").click(function (e) {
        e.preventDefault();
        var $link = $(this);
        bootbox.confirm(
            "Are you sure you wish to remove " +
            $link.data("admin-name") +
            " from the biobank " +
            $link.data("biobank-name") + "?",
            function (confirmation) {
                confirmation && window.location.assign($link.attr("href"));
            }
        );
    });
});