// report
const myRepCircle = document.getElementById('repcir');
const myRepIcon = document.getElementById('repnotify');
const myDropr = document.getElementById('navbarDropdownr');
myRepIcon.addEventListener('click', function () {
    myRepCircle.style.display = 'none';
});
myDropr.addEventListener('click', function () {
    myRepCircle.style.display = 'none';
});





var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationhub")
    .build();
connection.on("SendNewReport", function (message) {
    // Display the notification to the user
    myRepCircle.style.display = 'block';
});
connection.start();


function getReports(id) {
    $.ajax({
        url: '/api/GetDoctorReports/?id=' + id,
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#reportList').empty();
            $.each(data, function (index, item) {
                var link = document.createElement('a');
                link.classList.add('dropdown-item');
                link.setAttribute('href', '/Report/GetReport/?ReportId=' + item.id);
                link.textContent = "Dr/ " + item.firstName + " " + item.lastName + " Send Report To You In " + item.reportDate;
                $('#reportList').append(link);
                console.log(item.id + " " + item.firstName + " " + item.lastName);
            });
        },
        error: function () {
            alert('Something went wrong');
        }
    });
}