/**
 * Enhances Bootstrap Collapse behaviour on `facet-group` class containers
 * with nice icons :)
 */

$(".facet-group").on("hidden.bs.collapse", function () {
  $("#" + $(this).data("icon-id")).removeClass("fa-plus-square");
  $("#" + $(this).data("icon-id")).removeClass("fa-minus-square");

  $("#" + $(this).data("icon-id")).addClass("fa-plus-square");

  $.cookie($(this).attr("id") + "Collapsed", true);
});

$(".facet-group").on("shown.bs.collapse", function () {
  $("#" + $(this).data("icon-id")).removeClass("fa-plus-square");
  $("#" + $(this).data("icon-id")).removeClass("fa-minus-square");

  $("#" + $(this).data("icon-id")).addClass("fa-minus-square");

  $.cookie($(this).attr("id") + "Collapsed", false);
});
