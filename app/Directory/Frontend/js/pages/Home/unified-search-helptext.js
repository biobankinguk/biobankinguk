/**
 * Updates helptext in the unified search box based on what the search target is
 * (Collections or Capabilities)
 */

$(function () {
  $("input:radio[name='searchRadio']").change(function () {
    var helpText = $("#search-help");

    if (this.checked) {
      if (this.value === "Collections") {
        helpText.html(
          "i.e.find resources that may have suitable samples that have already been collected"
        );
        $(this).prop("checked", true);
      }
      if (this.value === "Capabilities") {
        helpText.html(
          "i.e. find resources that can collect samples specifically for you"
        );
        $(this).prop("checked", true);
      }
    }
  });
});
