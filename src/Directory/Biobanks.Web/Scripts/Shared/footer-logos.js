function logo(src) {
	return $("<a>").append(
		$("<img>").attr("src", src)
	);
}

$(function () {
	var $footerLogos = $("#footerLogos");

	$.getJSON("/Home/FetchFooterLogosAjax", function (json) {
		$(json).each(function (index, src) {
			$footerLogos.append(logo(src));
		});
	});
});
