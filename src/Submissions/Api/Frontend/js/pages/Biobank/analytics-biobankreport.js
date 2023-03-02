$(function () {
  //Profile PageViews Top 10
  var viewData = [
    {
      x: plotlyAnalytics.profilePageViews.quarterLabels,
      y: plotlyAnalytics.profilePageViews.viewsPerQuarter,
      name: plotlyAnalytics.name,
    },
    {
      x: plotlyAnalytics.profilePageViews.quarterLabels,
      y: plotlyAnalytics.profilePageViews.viewsAverages,
      name: "Directory Average",
    },
  ];
  viewDiv = document.getElementById("plotly-ViewsPerQuarter");
  perQuarterPlot(viewDiv, viewData);

  //Profile Page Routes Bar plot
  var routeData = [
    {
      x: plotlyAnalytics.profilePageViews.pageRouteLabels,
      y: plotlyAnalytics.profilePageViews.routeCount,
      name: plotlyAnalytics.name,
      type: "bar",
    },
  ];
  routeDiv = document.getElementById("plotly-ViewRoutes");
  barPlot(routeDiv, routeData);

  //Searches Top 10
  var searchData = [
    {
      x: plotlyAnalytics.searchActivity.quarterLabels,
      y: plotlyAnalytics.searchActivity.searchPerQuarter,
      name: plotlyAnalytics.name,
    },
    {
      x: plotlyAnalytics.searchActivity.quarterLabels,
      y: plotlyAnalytics.searchActivity.searchAverages,
      name: "Directory Average",
    },
  ];
  searchDiv = document.getElementById("plotly-SearchPerQuarter");
  perQuarterPlot(searchDiv, searchData);

  //Search Type Bar plot
  var typeData = [
    {
      x: plotlyAnalytics.searchActivity.searchTypeLabels,
      y: plotlyAnalytics.searchActivity.searchTypeCount,
      name: plotlyAnalytics.name,
      type: "bar",
    },
  ];
  typeDiv = document.getElementById("plotly-SearchTypes");
  barPlot(typeDiv, typeData);

  //Search Term Bar plot
  var routeData = [
    {
      x: plotlyAnalytics.searchActivity.searchTermLabels,
      y: plotlyAnalytics.searchActivity.searchTermCount,
      name: plotlyAnalytics.same,
      type: "bar",
    },
  ];
  routeDiv = document.getElementById("plotly-SearchTerms");
  barPlot(routeDiv, routeData, 10);

  //Search Filters Bar plot
  var filterData = [
    {
      x: plotlyAnalytics.searchActivity.searchFilterLabels,
      y: plotlyAnalytics.searchActivity.searchFilterCount,
      name: plotlyAnalytics.name,
      type: "bar",
    },
  ];
  filterDiv = document.getElementById("plotly-SearchFilters");
  barPlot(filterDiv, filterData, 10);

  //Contact Requests Top 10
  var contactData = [
    {
      x: plotlyAnalytics.contactRequests.quarterLabels,
      y: plotlyAnalytics.contactRequests.contactsPerQuarter,
      name: plotlyAnalytics.name,
    },
    {
      x: plotlyAnalytics.contactRequests.quarterLabels,
      y: plotlyAnalytics.contactRequests.contactAverages,
      name: "Directory Average",
    },
  ];
  contactDiv = document.getElementById("plotly-ContactsPerQuarter");
  perQuarterPlot(contactDiv, contactData);
});

function perQuarterPlot(container, data) {
  var config = { responsive: true };
  var layout = {
    margin: { l: 30, r: 20, b: 80, t: 5 },
    showlegend: true,
    legend: { orientation: "h" },
  };

  Plotly.newPlot(container, data, layout, config);
}

function barPlot(container, data, cutoff) {
  cutoff = cutoff || -1;
  if (cutoff > 0) {
    data[0].x = data[0].x.slice(0, cutoff);
    data[0].y = data[0].y.slice(0, cutoff);
  }

  var config = { responsive: true };
  var layout = {
    margin: { l: 30, r: 20, b: 180, t: 5 },
    showlegend: false,
    xaxis: {
      tickangle: -45,
    },
  };
  Plotly.newPlot(container, data, layout, config);
}
