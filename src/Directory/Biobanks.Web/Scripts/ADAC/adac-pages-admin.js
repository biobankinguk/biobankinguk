$(function () {
    $(".delete-confirm").click(function (e) {
        e.preventDefault();

        bootbox.confirm("Are you sure you want to delete?",
            function (confirmation) {
                if (confirmation) {

                }
            }
        );
    });
})