function addItemsToNav($nav, items) {
    $nav.prepend(items);
}

$(function () {
    var storageKey = "__TDCC__WordPressNavItems";
    var $nav = $("#wordpressNav");

    // Retrieve cached items
    var navItems = sessionStorage.getItem(storageKey);

    // Call AJAX if not cached
    if (navItems === null) {
        $.get({
            url: $nav.data("wordpress-ajax-url"),
            success: function (items) {
                sessionStorage.setItem(storageKey, items);
                addItemsToNav($nav, items);
            }
        }).fail(function () {
            console.error("Failed to get Wordpress Nav Menu Items. Check server logs for details.")
        });
    }
    else {
        addItemsToNav($nav, navItems);
    }

    // Link Wordpress Dropdown To Action Dropdown
    $("#myNavbar").find("a.dropdown-toggle").data("target", "#myNavbar")
});
