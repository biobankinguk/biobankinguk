<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[MvcSiteMapProvider.Web.Html.Models.CanonicalHelperModel,MvcSiteMapProvider]]" %>

<% if (Model.CurrentNode != null && !string.IsNullOrEmpty(Model.CurrentNode.CanonicalUrl)) { %>
    <link rel="canonical" href="<%=Model.CurrentNode.CanonicalUrl%>" />
<% } %>