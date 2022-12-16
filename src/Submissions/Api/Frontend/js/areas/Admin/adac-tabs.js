

window.addEventListener("DOMContentLoaded", function () {
    var dropDown = $("#refdropdown");
    var links = dropDown.find("li");
    links.sort(function (a, b) {

        if ($(a).text().trim() > $(b).text().trim()) {
            return 1;
        }
        else if ($(a).text().trim() < $(b).text().trim()) {
            return -1;
        }
        else if ($(a).text().trim() === $(b).text().trim()) {
            return 0;
        }

    }).appendTo(dropDown);
})

