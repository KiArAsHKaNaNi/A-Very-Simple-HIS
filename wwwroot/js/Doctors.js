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
                                <a onclick=Delete("/Doctors/Delete/${data}") class="btn btn-sm btn-outline-danger ms-1">Delete</a>
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
        title: "Are you sure you want to delete the doctor?",
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