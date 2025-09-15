var dataTable;

$(document).ready(function () {
    loadDataTable();
});

//function loadDataTable() {
//    dataTable = $('#tableData').DataTable({
//        ajax: {
//            url: "/patients/GetAll"
//        },
//        columns: [
//            { data: "firstName", "width": "60%" },
//            { data: "lastName", "width": "60%" },
//            { data: "dateOfBirth", "width": "60%" },
//            {
//                data: "id",
//                render: function (data) {
//                    return `
//                        <div>
//                                <a class="btn btn-outline-primary" asp-action="Edit" asp-route-id="${data}">Edit</a> |
//                                <a class="btn btn-outline-primary" asp-action="Details" asp-route-id="${data}">Details</a> |
//                                <a class="btn btn-outline-danger" asp-action="Delete" asp-route-id="${data}">Delete</a>
//                        </div>
//                    `
//                }, width: "50%"
//            }
//        ]
//    });

//}

$(document).ready(function () {
    $('#tableData').DataTable({
        ajax: {
            url: '/Visits/GetAll', 
            type: 'GET',
            datatype: "json"
        },
        columns: [
            { data: 'patientName', width: '15%' },
            { data: 'doctorName', width: '15%' },
            { data: 'visitDate', width: '12%' },
            {
                data: 'id',
                width: '23%',
                orderable: false,
                searchable: false,
                render: function (data, type, row, meta) {
                    return `
                            <div class="btn-group" role="group" aria-label="Actions">
                                <a class="btn btn-sm btn-outline-primary" href="/Visits/Edit/${data}">Edit</a>
                                <a class="btn btn-sm btn-outline-secondary ms-1" href="/Visits/Details/${data}">Details</a>
                                <a onclick=Delete("/Visits/Delete/${data}") class="btn btn-sm btn-outline-danger ms-1">Delete</a>
                            </div>
                        `;
                }
            }
        ],
        responsive: true,
        lengthMenu: [5, 10, 25, 50],
        pageLength: 10,
        language: {
            emptyTable: "No patients available"
        }
    });
});

function Delete(url) {
    Swal.fire({
        title: "Are you sure you want to delete the patient?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'POST',   
                success: function (data) {
                    if (data.success) {
                        $('#tableData').DataTable().ajax.reload();

                        Swal.fire(
                            "Deleted!",
                            data.data,   
                            "success"
                        );
                    } else {
                        Swal.fire(
                            "Error!",
                            data.message,
                            "error"
                        );
                    }
                },
                error: function () {
                    Swal.fire(
                        "Error!",
                        "Something went wrong while deleting.",
                        "error"
                    );
                }
            });
        }
    });
}

