$(function () {
    $.fn.dataTable.moment("DD/MM/YYYY HH:mm:ss");

    $("#adac-request-history").DataTable({
        "order": [[ 4, "desc" ]],
        "columnDefs": [{ "orderable": false, "targets": 6 }],
        "paging": false, //we might actually want this on this one! Depends if we keep the history indefinitely
        "info": false,
        "autoWidth": false,
        "language": {
            "search": "Filter: "
        }
    });

    $(".resend-confirm").click(function(e) {
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

    $(".manual-confirm").click(function (e) {
        e.preventDefault();

        var $link = $(this);
        var url = $link.attr("href");
        var user = $link.data("admin-name");
        var email = $link.data("admin-email");

        $.getJSON(url, { userEmail : email }, function (data) {
            bootbox.alert(
                "<p>" + user + " (" + email + ") has not yet confirmed their account. </p>" +
                "<p> Send them the link below to confirm their account.</p>" +
                "<p><b>Ensure this link is sent to the correct user</b></p> " +
                "<input type='text' class='form-control' style='width:100%' value='" + data.link + "'/>"
            );     
        });
    });
});