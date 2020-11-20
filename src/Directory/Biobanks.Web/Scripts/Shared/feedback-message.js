function feedbackMessage(message, type, html) {
    var feedback = $("#feedback-message");

    $.get({
        url: feedback.data("ajax-source"),
        data: { "message": message, "type": type, "html": html },
        success: function(content) {
            //use animation to make it clear the message has changed if there was already one there!
            feedback.fadeOut(200, "swing", function() {
                feedback.html(content);

                feedback.fadeIn(100);
            });
        }
    });
}