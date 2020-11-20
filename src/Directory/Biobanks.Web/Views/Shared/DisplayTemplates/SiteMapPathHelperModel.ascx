<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[MvcSiteMapProvider.Web.Html.Models.SiteMapPathHelperModel,MvcSiteMapProvider]]" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<% foreach (var node in Model) { %>
    <%=Html.DisplayFor(m => node)%>
    <% if (node != Model.Last()) { %>
        &gt;
    <% } %>
<% } %>