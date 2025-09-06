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
            url: '/Patients/GetAll', 
            type: 'GET',
            datatype: "json"
        },
        columns: [
            { data: 'firstName', width: '15%' },
            { data: 'lastName', width: '15%' },
            { data: 'dateOfBirth', width: '12%' },
            { data: 'gender', width: '15%' },
            { data: 'insurance', width: '20%' },
            {
                data: 'id',
                width: '23%',
                orderable: false,
                searchable: false,
                render: function (data, type, row, meta) {
                    return `
                            <div class="btn-group" role="group" aria-label="Actions">
                                <a class="btn btn-sm btn-outline-primary" href="/Patients/Edit/${data}">Edit</a>
                                <a class="btn btn-sm btn-outline-secondary ms-1" href="/Patients/Details/${data}">Details</a>
                                <a class="btn btn-sm btn-outline-danger ms-1" href="/Patients/Delete/${data}">Delete</a>
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