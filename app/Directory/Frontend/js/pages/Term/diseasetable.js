$("#disease-terms")["DataTable"]({
    columnDefs: [
        { orderable: false, targets: 2 },
    
    ],
    paging: false,
    info: false,
    autoWidth: true,
    language: {
        search: "Filter: ",
    },
});