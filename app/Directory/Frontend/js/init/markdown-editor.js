/**
 * Initialises a Markdown Editor using MarkdownDeep
 * on all suitable elements (TextArea with the `mdd_editor` class)
 */

$(function () {
  $("textarea.mdd_editor").click(function () {
    // MarkdownDeep editor instance
    var editor = $(this).data("mdd");

    // If this isn't the open editor
    if (editor == null) {
      // Remove existing editor(s)
      $(".mdd_toolbar_wrap").remove();
      $(".mdd_editor_wrap").each(function (i, el) {
        var $wrap = $(el);
        var $text = $wrap.find("textarea.mdd_editor");

        // Replace .mdd_editor_wrap with child textarea
        $wrap.replaceWith($text.removeData());
      });

      // Make this editor active
      $(this).MarkdownDeep({
        help_location: "/dist/mdd_help.htm",
        disableTabHandling: true,
        resizebar: false,
      });
    }
  });
});
