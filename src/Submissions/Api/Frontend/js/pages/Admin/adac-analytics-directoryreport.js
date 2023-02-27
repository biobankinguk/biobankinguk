$(function () {
  // Turn on tooltips
  $('[data-toggle="tooltip"]').tooltip();

  //****** Overall Stats *******

  //Total Number of Sessions
  var sessionData = [
    {
      x: plotlyAnalytics.sessionStats.sessionNumberLabels,
      y: plotlyAnalytics.sessionStats.sessionNumberCount,
      name: "Total Number of Sessions",
    },
  ];
  sessionDiv = document.getElementById("plotly-SessionNumber");
  perQuarterPlot(sessionDiv, sessionData);

  //Average Bouncerate
  var bounceRateData = [
    {
      x: plotlyAnalytics.sessionStats.avgBounceRateLabels,
      y: plotlyAnalytics.sessionStats.avgBounceRateCount,
      name: "Average Bouncerate",
    },
  ];
  bounceRateDiv = document.getElementById("plotly-AvgBounceRate");
  perQuarterPlot(bounceRateDiv, bounceRateData, "Percent %");

  //Average Ratio of New Sessions
  var newSessionData = [
    {
      x: plotlyAnalytics.sessionStats.avgNewSessionLabels,
      y: plotlyAnalytics.sessionStats.avgNewSessionCount,
      name: "Average Ratio of New Sessions",
    },
  ];
  newSessionDiv = document.getElementById("plotly-AvgNewSession");
  perQuarterPlot(newSessionDiv, newSessionData, "Percent %");

  //Average Session Duration
  var sessionDurationData = [
    {
      x: plotlyAnalytics.sessionStats.avgSessionDurationLabels,
      y: plotlyAnalytics.sessionStats.avgSessionDurationCount,
      name: "Average Session Duration",
    },
  ];
  sessionDurationDiv = document.getElementById("plotly-AvgSessionDuration");
  durationPlot(sessionDurationDiv, sessionDurationData);

  //****** Tissue Directory Stats Related to Searches *******

  //Total Number of Sessions (Search)
  var sessionSearchData = [
    {
      x: plotlyAnalytics.sessionSearchStats.sessionNumberLabels,
      y: plotlyAnalytics.sessionSearchStats.sessionNumberCount,
      name: "Total Number of Sessions (Search)",
    },
  ];
  sessionSearchDiv = document.getElementById("plotly-SessionNumberSearch");
  perQuarterPlot(sessionSearchDiv, sessionSearchData);

  //Average Bouncerate (Search)
  var bounceRateSearchData = [
    {
      x: plotlyAnalytics.sessionSearchStats.avgBounceRateLabels,
      y: plotlyAnalytics.sessionSearchStats.avgBounceRateCount,
      name: "Average Bouncerate (Search)",
    },
  ];
  bounceRateSearchDiv = document.getElementById("plotly-AvgBounceRateSearch");
  perQuarterPlot(bounceRateSearchDiv, bounceRateSearchData, "Percent %");

  //Average Ratio of New Sessions (Search)
  var newSessionSearchData = [
    {
      x: plotlyAnalytics.sessionSearchStats.avgNewSessionLabels,
      y: plotlyAnalytics.sessionSearchStats.avgNewSessionCount,
      name: "Average Ratio of New Sessions (Search)",
    },
  ];
  newSessionSearchDiv = document.getElementById("plotly-AvgNewSessionSearch");
  perQuarterPlot(newSessionSearchDiv, newSessionSearchData, "Percent %");

  //Average Session Duration (Search)
  var sessionDurationSearchData = [
    {
      x: plotlyAnalytics.sessionSearchStats.avgSessionDurationLabels,
      y: plotlyAnalytics.sessionSearchStats.avgSessionDurationCount,
    },
  ];
  sessionDurationSearchDiv = document.getElementById(
    "plotly-AvgSessionDurationSearch"
  );
  durationPlot(sessionDurationSearchDiv, sessionDurationSearchData);

  //****** Tissue Directory Search Characteristics *******

  //Search Type Bar plot
  var typeData = [
    {
      x: plotlyAnalytics.searchCharacteristics.searchTypeLabels,
      y: plotlyAnalytics.searchCharacteristics.searchTypeCount,
      type: "bar",
    },
  ];
  typeDiv = document.getElementById("plotly-SearchTypes");
  barPlot(typeDiv, typeData);

  //Search Term Pie plot
  var routeData = [
    {
      labels: plotlyAnalytics.searchCharacteristics.searchTermLabels,
      values: plotlyAnalytics.searchCharacteristics.searchTermCount,
      type: "pie",
      textinfo: "label+percent",
      insidetextorientation: "radial",
    },
  ];
  routeDiv = document.getElementById("plotly-SearchTerms");
  piePlot(routeDiv, routeData, 10);

  //Search Filters Bar plot
  var filterData = [
    {
      x: plotlyAnalytics.searchCharacteristics.searchFilterLabels,
      y: plotlyAnalytics.searchCharacteristics.searchFilterCount,
      type: "bar",
    },
  ];
  filterDiv = document.getElementById("plotly-SearchFilters");
  barPlot(filterDiv, filterData, 10);

  //****** Tissue Directory Stats Related to Events *******
  //Number of Sample Resource Contact Detail Requests (All Sessions)
  var contactData = [
    {
      x: plotlyAnalytics.eventStats.contactNumberLabels,
      y: plotlyAnalytics.eventStats.contactNumberCount,
    },
  ];
  contactDiv = document.getElementById("plotly-ContactCount");
  perQuarterPlot(contactDiv, contactData);

  //Number of Sample Resource Contact Detail Requests (Filtered by date and location)
  var filteredContactData = [
    {
      x: plotlyAnalytics.eventStats.filteredContactLabels,
      y: plotlyAnalytics.eventStats.filteredContactCount,
    },
  ];
  filteredContactDiv = document.getElementById("plotly-FilteredContactCount");
  perQuarterPlot(filteredContactDiv, filteredContactData);

  //Number of Mailto User Actions (Filtered by date and location)
  var filteredMailToData = [
    {
      x: plotlyAnalytics.eventStats.filteredMailToLabels,
      y: plotlyAnalytics.eventStats.filteredMailToCount,
    },
  ];
  filteredMailToDiv = document.getElementById("plotly-FilteredMailToCount");
  perQuarterPlot(filteredMailToDiv, filteredMailToData);

  //****** Tissue Directory Biobank Profile Page Statistics *******
  //Sample Resource Profile Pages
  var profileRouteData = [
    {
      x: plotlyAnalytics.profilePageStats.pageRouteLabels,
      y: plotlyAnalytics.profilePageStats.routeCount,
      type: "bar",
    },
  ];
  profileRouteDiv = document.getElementById("plotly-ProfileRoute");
  barPlot(profileRouteDiv, profileRouteData, 10);
});

function perQuarterPlot(container, data, ylabel) {
  ylabel = ylabel || "";
  var config = { responsive: true };
  var layout = {
    margin: { l: 45, r: 10, b: 80, t: 15 },
    showlegend: false,
    legend: { orientation: "h" },
    yaxis: {
      title: ylabel,
      showgrid: false,
      zeroline: false,
    },
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

function piePlot(container, data, cutoff) {
  cutoff = cutoff || -1;
  if (cutoff > 0) {
    data[0].values = data[0].values.slice(0, cutoff);
    data[0].labels = data[0].labels.slice(0, cutoff);
  }

  data.type = "pie";

  var config = { responsive: true };
  var layout = {
    //margin: { l: 30, r: 20, b: 180, t: 5 },
    showlegend: false,
  };
  Plotly.newPlot(container, data, layout, config);
}

function durationPlot(container, data) {
  var config = { responsive: true };
  var layout = {
    margin: { l: 35, r: 10, b: 80, t: 5 },
    showlegend: false,
    legend: { orientation: "h" },
    yaxis: {
      title: "Seconds",
      showgrid: false,
      zeroline: false,
    },
  };
  //format duration in minutes and seconds and add as annotation
  count = data[0].y;
  duration = [];
  for (var i = 0, length = count.length; i < length; i++) {
    duration[i] =
      Math.floor(count[i] / 60) + "m " + Math.round(count[i] % 60) + "s";
  }
  data[0].text = duration;
  Plotly.newPlot(container, data, layout, config);
}
