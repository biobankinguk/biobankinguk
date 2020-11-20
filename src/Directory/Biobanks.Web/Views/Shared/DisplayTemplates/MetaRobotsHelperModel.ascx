<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[MvcSiteMapProvider.Web.Html.Models.MetaRobotsHelperModel,MvcSiteMapProvider]]" %>

<% if (Model.CurrentNode != null && !string.IsNullOrEmpty(Model.CurrentNode.MetaRobotsContent)) { %>
    <meta name="robots" content="<%=Model.CurrentNode.MetaRobotsContent%>" />
<% } %>