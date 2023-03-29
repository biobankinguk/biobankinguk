$(function() {
    $('.annualstats-input').keydown(function(e) {

        // modern browsers
        if (e.key !== undefined)
            var isEOrDot = (e.key === "e" || e.key === "E" || e.key === ".");
        // death traps
        else if (e.keyCode !== undefined)
            var isEOrDot = (e.keyCode && e.keyCode == 69) || (e.keyCode && (e.keyCode == 110 || e.keyCode == 190));

        if (isEOrDot)
            return false;
    });

    $(".annualstats-input").change(function() {
        $.ajax({
            type: "POST",
            url: annualStatUpdateUrl,
            data: {
                AnnualStatisticId: $(this).data("statistic"),
                Year: $(this).data("year"),
                Value: $(this).val()
            },
            success: function(data) {
                var feedbackMessageBox = $(".annualstats-feedback");

                if (!data.success) {
                    if (Array.isArray(data.errors)) {
                        feedbackMessageBox.removeClass("alert-info");
                        feedbackMessageBox.html("&nbsp;");
                        feedbackMessageBox.addClass("alert-danger");
                        data.errors.forEach(
                            function(item, index) {
                                feedbackMessageBox.append(item + "<br>");
                            }
                        );
                    }
                }
            }
        });
    });
});