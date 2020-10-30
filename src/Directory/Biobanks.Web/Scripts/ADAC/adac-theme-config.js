function defaultImage(img) {
	$(img).attr("src", $(img).data("src"));
}

function previewImage(input) {
	if (input.files && input.files[0]) {	
		var file = input.files[0];
		var reader = new FileReader();
		var img = $("#" + $(input).data("preview"));

		reader.onload = function (e) {
			$(img).attr("src", e.target.result);
		};
		reader.readAsDataURL(file);
	}
}

function previewFile(input) {
	var text = $(input).siblings(".css-filename")

	if (input.files && input.files[0]) {
		text.text(input.files[0].name);
	}
	else {
		text.text("No File Chosen");
	}
}

$(function () {

	// Replace Broken Images With Provided Default
	$("img[data-src]").one("error", function () {
		defaultImage(this);
	});

	// Load Image Preview When File Selected
	$("input.img-upload").change(function () {
		previewImage(this)
	});
	$("input.css-upload").change(function () {
		previewFile(this);
	});

	// Trigger File Input
	$("button.select-img").click(function () {
		$(this).siblings("input.img-upload").trigger("click");
	});
	$("button.select-css").click(function () {
		$(this).siblings("input.css-upload").trigger("click");
	});

	// Remove File Input
	$("button.remove-img").click(function () {
		var $removeLogo = $(this).siblings("input.img-remove");
		var $logoInput = $(this).siblings("input.img-upload");
		var $logoImage = $("#" + $logoInput.data("preview"));

		console.log($removeLogo);
		console.log($logoInput);
		console.log($logoImage);

		// Revert to Default Image
		defaultImage($logoImage);

		// Remove Input and Flag For Deletion on Server
		$logoInput.val(null);
		$removeLogo.val(true);
	});
});
