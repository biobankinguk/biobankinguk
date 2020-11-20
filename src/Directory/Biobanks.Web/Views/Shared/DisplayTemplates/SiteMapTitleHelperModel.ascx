<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[MvcSiteMapProvider.Web.Html.Models.SiteMapTitleHelperModel,MvcSiteMapProvider]]" %>

<% if (Model.CurrentNode != null) { %>
<%=Model.CurrentNode.Title%>
<% } %>