var DetailsViewModel = (function () {
    function DetailsViewModel() {
        var _this = this;
        this.toggleSchemeString = function (current) {
            return (current === "http://") ? "https://" : "http://";
        };
        this.toggleNetworkUrlScheme = function () {
            _this.networkUrlScheme(_this.toggleSchemeString(_this.networkUrlScheme()));
        };
        this.cleanNodeJquerySafe = function (nodeSelector) {
            //clear knockout bindings,
            //but leave jQuery/bootstrap bindings intact!
            var original = ko.utils.domNodeDisposal["cleanExternalData"];
            ko.utils.domNodeDisposal["cleanExternalData"] = function () { };
            ko.cleanNode($(nodeSelector)[0]); //designed to work with ID selectors, so only does the first match
            ko.utils.domNodeDisposal["cleanExternalData"] = original;
        };
        this.networkAdminsModel = new NetworkAdminsViewModel();
        this.networkUrlScheme = ko.observable($("#NetworkModel_UrlScheme").val());
    }
    return DetailsViewModel;
})();
