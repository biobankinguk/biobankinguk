// jquery plugin to serialise checkboxes as bools
(function ($) {
    $.fn.serialize = function () {
        return $.param(this.serializeArray());
    };

    $.fn.serializeArray = function () {
        var o = $.extend(
            {
                checkboxesAsBools: true,
            },
            {}
        );

        var rselectTextarea = /select|textarea/i;
        var rinput = /text|hidden|password|search/i;

        return this.map(function () {
            return this.elements ? $.makeArray(this.elements) : this;
        })
            .filter(function () {
                return (
                    this.name &&
                    !this.disabled &&
                    (this.checked ||
                        (o.checkboxesAsBools && this.type === "checkbox") ||
                        rselectTextarea.test(this.nodeName) ||
                        rinput.test(this.type))
                );
            })
            .map(function (i, elem) {
                var val = $(this).val();
                return val == null
                    ? null
                    : $.isArray(val)
                        ? $.map(val, function(innerVal) {return ({ name: elem.name, value: innerVal })})
                        : {
                            name: elem.name,
                            value:
                                o.checkboxesAsBools && this.type === "checkbox" //moar ternaries!
                                    ? this.checked
                                        ? "true"
                                        : "false"
                                    : val,
                        };
            })
            .get();
    };
}
)(jQuery);
