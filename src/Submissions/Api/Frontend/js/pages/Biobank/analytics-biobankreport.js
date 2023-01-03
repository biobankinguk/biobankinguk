
$(function () {

	//Profile PageViews Top 10
	var viewData = [{
			x: plotlyAnalytics.ProfilePageViews.QuarterLabels,
			y: plotlyAnalytics.ProfilePageViews.ViewsPerQuarter,
			name: plotlyAnalytics.Name
		},
		{
			x: plotlyAnalytics.ProfilePageViews.QuarterLabels,
			y: plotlyAnalytics.ProfilePageViews.ViewsAverages,
			name: 'Directory Average'
		}
	];
	viewDiv = document.getElementById('plotly-ViewsPerQuarter');
	perQuarterPlot(viewDiv, viewData);


	//Profile Page Routes Bar plot
	var routeData = [{
		x: plotlyAnalytics.ProfilePageViews.PageRouteLabels,
		y: plotlyAnalytics.ProfilePageViews.RouteCount,
		name: plotlyAnalytics.Name,
		type: 'bar'
	}];
	routeDiv = document.getElementById('plotly-ViewRoutes');
	barPlot(routeDiv, routeData);

	//Searches Top 10
	var searchData = [{
		x: plotlyAnalytics.SearchActivity.QuarterLabels,
		y: plotlyAnalytics.SearchActivity.SearchPerQuarter,
		name: plotlyAnalytics.Name
	},
	{
		x: plotlyAnalytics.SearchActivity.QuarterLabels,
		y: plotlyAnalytics.SearchActivity.SearchAverages,
		name: 'Directory Average'
	}
	];
	searchDiv = document.getElementById('plotly-SearchPerQuarter');
	perQuarterPlot(searchDiv, searchData);

	//Search Type Bar plot
	var typeData = [{
		x: plotlyAnalytics.SearchActivity.SearchTypeLabels,
		y: plotlyAnalytics.SearchActivity.SearchTypeCount,
		name: plotlyAnalytics.Name,
		type: 'bar'
	}];
	typeDiv = document.getElementById('plotly-SearchTypes');
	barPlot(typeDiv, typeData);

	//Search Term Bar plot
	var routeData = [{
		x: plotlyAnalytics.SearchActivity.SearchTermLabels,
		y: plotlyAnalytics.SearchActivity.SearchTermCount,
		name: plotlyAnalytics.Name,
		type: 'bar'
	}];
	routeDiv = document.getElementById('plotly-SearchTerms');
	barPlot(routeDiv, routeData, 10);

	//Search Filters Bar plot
	var filterData = [{
		x: plotlyAnalytics.SearchActivity.SearchFilterLabels,
		y: plotlyAnalytics.SearchActivity.SearchFilterCount,
		name: plotlyAnalytics.Name,
		type: 'bar'
	}];
	filterDiv = document.getElementById('plotly-SearchFilters');
	barPlot(filterDiv, filterData, 10);

	//Contact Requests Top 10
	var contactData = [{
		x: plotlyAnalytics.ContactRequests.QuarterLabels,
		y: plotlyAnalytics.ContactRequests.ContactsPerQuarter,
		name: plotlyAnalytics.Name
	},
	{
		x: plotlyAnalytics.ContactRequests.QuarterLabels,
		y: plotlyAnalytics.ContactRequests.ContactAverages,
		name: 'Directory Average'
	}
	];
	contactDiv = document.getElementById('plotly-ContactsPerQuarter');
	perQuarterPlot(contactDiv, contactData);
});

function perQuarterPlot(container, data) {
	var config = { responsive: true };
	var layout = {
		margin: { l: 30, r: 20, b: 80, t: 5 },
		showlegend: true,
		legend: { "orientation": "h" }
	};

	Plotly.newPlot(container, data, layout, config);
}

function barPlot(container, data, cutoff = -1) {
	if (cutoff > 0) {
		data[0].x = data[0].x.slice(0, cutoff);
		data[0].y = data[0].y.slice(0, cutoff);
	}
		
	var config = { responsive: true };
	var layout = {
		margin: { l: 30, r: 20, b: 180, t: 5 },
		showlegend: false,
		xaxis: {
			tickangle: -45
		}
	};
		Plotly.newPlot(container, data, layout, config);
}