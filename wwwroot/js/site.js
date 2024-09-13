// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Initialize DataTable with export to Excel button
    $('#leadTable').DataTable({
        paging: true,
        searching: true,
        ordering: true,
        info: true,
        lengthChange: false,  // Hide length menu (optional)
        pageLength: 10,       // Show 10 entries by default
        dom: 'Bfrtip',        // Show buttons for exporting data
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Download Excel',
                className: 'btn btn-success'
            }
        ]
    });

    // Initialize another DataTable (leadsTable) with responsive features
    var table = $('#leadsTable').DataTable({
        paging: true,
        pageLength: 10,
        ordering: true,
        info: true,
        lengthChange: false,  // Hide length menu (optional)
        lengthMenu: [10, 25, 50, 75, 100],
        responsive: true
    });

    // Download Excel button with FromDate and ToDate filtering
    $('#downloadExcel').on('click', function () {
        // Get the selected dates
        var fromDate = $('#fromDate').val();
        var toDate = $('#toDate').val();

        // Construct the download URL with query parameters
        var url = '@Url.Action("DownloadExcel", "Lead")' + '?fromDate=' + fromDate + '&toDate=' + toDate;

        // Redirect to the download URL
        window.location.href = url;
    });
});
