$(function () {
  $(".clickable-row").click(function () {
    window.location.href = $(this).data("href");
  });

  $("#biobank-publications").DataTable({
    paging: false,
    info: false,
    autoWidth: false,
    aoColumns: [
      { sWidth: "50%" },
      { sWidth: "20%" },
      { sWidth: "10%" },
      { sWidth: "10%" },
      { sWidth: "10%" },
    ],
    language: {
      search: "Filter: ",
    },
  });
});
