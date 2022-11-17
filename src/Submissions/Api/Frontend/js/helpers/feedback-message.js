/* exported feedbackMessage */

/**
 * Provides a global function for triggering a "Feedback Message" Alert
 * by JavaScript
 * @param {string} message The message content
 * @param {string} type The type of alert (Bootstrap 3.x alert types)
 * @param {boolean} html Whether the message content should be rendered as html -
 * defaults to false (plain text)
 */
function feedbackMessage(message, type, html) {
  var feedback = $("#feedback-message");

  $.get({
    url: feedback.data("ajax-source"),
    data: { message: message, type: type, html: html },
    success: function (content) {
      //use animation to make it clear the message has changed if there was already one there!
      feedback.fadeOut(200, "swing", function () {
        feedback.html(content);

        feedback.fadeIn(100);
      });
    },
  });
}
