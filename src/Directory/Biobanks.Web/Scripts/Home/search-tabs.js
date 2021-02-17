    $('#searchtabs a').click(function (e) {
        e.preventDefault()
        var tabID = $(this).attr("href").substr(1);
        $(".tab-pane").each(function () {
            console.log("clearing " + $(this).attr("id") + " tab");
            $(this).empty();
        });

        $.ajax({
            url: '@ViewContext.RouteData.Values["controller"]/' + tabID,
            cache: false,
            type: "get",
            dataType: "html",
            success: function (result) {
                $("#" + tabID).html(result);
            }

        })
        $(this).tab('show')
    });