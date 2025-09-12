var dataTable;

$(document).ready(function () {
    loadDataTable();
});


$(document).ready(function () {
    $('#tableData').DataTable({
        ajax: {
            url: '/Doctors/GetAll',
            type: 'GET',
            datatype: "json"
        },
        columns: [
            { data: 'firstName', width: '15%' },
            { data: 'lastName', width: '15%' },
            { data: 'specialty', width: '15%' },
            { data: 'visits', width: '20%' },
            {
                data: 'id',
                width: '23%',
                orderable: false,
                searchable: false,
                render: function (data, type, row, meta) {
                    return `
                            <div class="btn-group text-end" role="group" aria-label="Actions">
                                <a class="btn btn-sm btn-outline-primary" href="/Doctors/Edit/${data}">Edit</a>
                                <a class="btn btn-sm btn-outline-secondary ms-1" href="/Doctors/Details/${data}">Details</a>
                                <a class="btn btn-sm btn-outline-danger ms-1" href="/Doctors/Delete/${data}">Delete</a>
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