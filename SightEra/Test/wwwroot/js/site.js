// on click on notify icon .. red circle not display

//  request
const myCircle = document.getElementById('cir');
const myIcon = document.getElementById('notify');
const myDrop = document.getElementById('navbarDropdown');
myIcon.addEventListener('click', function () {
    myCircle.style.display = 'none';
});
myDrop.addEventListener('click', function () {
    myCircle.style.display = 'none';
});





var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationhub")
    .build();
connection.on("DeleteRequest", function (message) {
    // Display the notification to the user
    alert(message);
});
connection.on("SendNewRequest", function (message) {
    // Display the notification to the user
    myCircle.style.display = 'block';
});
connection.start();

function getRequests(id) {
    //console.log("hello world");
    $.ajax({
        url: '/api/GetRadiologistRequests/?id=' + id,
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#notifyList').empty();
            $.each(data, function (index, item) {
                var link = document.createElement('a');
                link.classList.add('dropdown-item');
                //link.setAttribute('asp-controller', 'Request');
                //link.setAttribute('asp-action', 'GetRequest');
                //link.setAttribute('asp-route-RequestId', item.id);
                if (item.show) {
                    link.style.backgroundColor = '#C6E2FF'; // Change to your desired background color
                }
                link.setAttribute('href', '/Request/GetRequest/?RequestId='+item.id);
                link.textContent = "Dr/ " + item.firstName + " " + item.lastName + " Send Request To You In " + item.requestDate;
                $('#notifyList').append(link);
                console.log(item.id+" "+ item.firstName + " " + item.lastName);
            });
        },
        error: function () {
            alert('Something went wrong');
        }
    });
}

//function getReports(id) {
//    $.ajax({
//        url: '/api/GetDoctorReports/?id=' + id,
//        method: 'GET',
//        dataType: 'json',
//        success: function (data) {
//            $('#reportList').empty();
//            $.each(data, function (index, item) {
//                var link = document.createElement('a');
//                link.classList.add('dropdown-item');
//                link.setAttribute('href', '/Report/GetReport/?ReportId=' + item.id);
//                link.textContent = "Dr/ " + item.firstName + " " + item.lastName + " Send Report To You In " + item.reportDate;
//                $('#reportList').append(link);
//                console.log(item.id + " " + item.firstName + " " + item.lastName);
//            });
//        },
//        error: function () {
//            alert('Something went wrong');
//        }
//    });
//}