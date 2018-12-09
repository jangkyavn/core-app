'use strict';

var feedbackController = function () {
    this.initialize = function () {
        validateForm();
        loadData();
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnAddNew').on('click', function () {
            resetForm();
            $('#modalTitle').text('Thêm mới phản hồi');
            $('#addEditModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('#btnSave').on('click', function () {
            if ($('#feedbackForm').valid()) {
                var data = {
                    Id: $('#hidId').val(),
                    DateCreated: $('#hidDateCreated').val(),
                    Name: $('#txtName').val(),
                    Email: $('#txtEmail').val(),
                    PhoneNumber: $('#txtPhoneNumber').val(),
                    Message: $('#txtMessage').val(),
                    Status: $('#chkStatus').prop('checked') ? 1 : 0
                };

                core.callAjax('/Admin/Feedback/SaveEntity', 'POST', data, 'json', function (res) {
                    if (res) {
                        core.notify(message.add_success, 'success');
                    } else {
                        core.notify(message.edit_success, 'success');
                    }

                    $('#addEditModal').modal('hide');
                    loadData(true);
                });
            }
        });

        $('#btnDeleteMultiple').on('click', function () {
            var listId = [];

            $('.chk-item:checked').each(function () {
                listId.push($(this).data('id'));
            });

            core.confirm(message.confirm_delete_multiple, function () {
                core.callAjax('/Admin/Feedback/DeleteMultiple', 'POST', {
                    jsonId: JSON.stringify(listId)
                }, 'json', function () {
                    core.notify(message.delete_success, 'success');
                    loadData(true);
                });
            });
        });

        $('#btnSearch').on('click', function () {
            loadData(true);
        });

        $('#btnRefresh').on('click', function () {
            $('#txtKeyword').val('');

            loadData(true);
        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            resetForm();
            $('#modalTitle').text('Sửa phản hồi');
            var id = $(this).data('id');
            $('#wrapPassword').hide();
            getById(id, function () {
                $('#addEditModal').modal({
                    show: true,
                    backdrop: 'static'
                });
            });
        });

        $('table').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');

            core.confirm(message.confirm_delete, function () {
                core.callAjax('/Admin/Feedback/Delete', 'POST', { id }, 'json', function () {
                    core.notify(message.delete_success, 'success');
                    loadData(true);
                });
            });
        });

        $('#chkAll').change(function () {
            var checkboxes = $(this).closest('table').find(':checkbox');
            checkboxes.prop('checked', $(this).is(':checked'));
            $('#btnDeleteMultiple').prop('disabled', !$(this).is(':checked'));
        });

        $('table').on('change', '.chk-item', function () {
            if ($('.chk-item:checked').length === $('.chk-item').length) {
                $('#chkAll').prop('indeterminate', false);
                $('#chkAll').prop('checked', true);
                $('#btnDeleteMultiple').prop('disabled', false);
            } else if ($('.chk-item:checked').length !== 0) {
                $('#chkAll').prop('indeterminate', true);
                $('#chkAll').prop('checked', false);
                $('#btnDeleteMultiple').prop('disabled', false);
            } else {
                $('#chkAll').prop('indeterminate', false);
                $('#chkAll').prop('checked', false);
                $('#btnDeleteMultiple').prop('disabled', true);
            }
        });

        $('#ddlShowPage').on('change', function () {
            core.configs.pageIndex = 1;
            core.configs.pageSize = parseInt($(this).val());

            loadData(true);
        });
    };

    var validateForm = function () {
        $('#feedbackForm').validate({
            errorClass: 'text-danger',
            rules: {
                name: {
                    required: true,
                    maxlength: 50
                },
                email: {
                    required: true,
                    email: true,
                    maxlength: 50
                },
                phoneNumber: {
                    maxlength: 20
                },
                message: {
                    maxlength: 500
                }
            },
            messages: {
                name: {
                    required: 'Hãy nhập tên khách hàng',
                    maxlength: 'Tên khách hàng không vượt quá 50 ký tự'
                },
                email: {
                    required: 'Hãy nhập email',
                    email: 'Email không hợp lệ',
                    maxlength: 'Email không vượt quá 50 ký tự'
                },
                phoneNumber: {
                    maxlength: 'Số điện thoại không vượt quá 20 ký tự'
                },
                message: {
                    maxlength: 'Tin nhắn không vượt quá 500 ký tự'
                }
            },
            highlight: function (element) {
                $(element).addClass('is-invalid').removeClass('is-valid');
            },
            unhighlight: function (element) {
                $(element).addClass('is-valid').removeClass('is-invalid');
            }
        });
    };

    var getById = function (id, callBack) {
        core.callAjax('/Admin/Feedback/GetById', 'GET', { id }, 'json', function (res) {
            $('#hidId').val(res.Id);
            $('#hidDateCreated').val(res.DateCreated);
            $('#txtName').val(res.Name);
            $('#txtEmail').val(res.Email);
            $('#txtPhoneNumber').val(res.PhoneNumber);
            $('#txtMessage').val(res.Message);
            $('#chkStatus').prop('checked', res.Status);

            callBack();
        });
    };

    var resetForm = function () {
        $('#hidId').val(0);
        $('#hidDateCreated').val(null);
        $('#txtName').val('');
        $('#txtEmail').val('');
        $('#txtPhoneNumber').val('');
        $('#txtMessage').val('');
        $('#chkStatus').prop('checked', false);

        $("#feedbackForm").validate().resetForm();
        $('input, textarea, select, file, number').removeClass('is-valid');
        $('input, textarea, select, file, number').removeClass('is-invalid');
    };

    var loadData = function (isPageChanged) {
        core.callAjax('/Admin/Feedback/GetAllPaging', 'GET', {
            keyword: $('#txtKeyword').val(),
            page: core.configs.pageIndex,
            pageSize: core.configs.pageSize
        }, 'json', function (res) {
            console.log(res);
            var render = '';
            var template = $('#tbodyTemplate').html();
            var numberOrder = res.FirstRowOnPage;

            $.each(res.Results, function (index, item) {
                render += Mustache.render(template, {
                    NumberOrder: numberOrder++,
                    Id: item.Id,
                    Name: item.Name,
                    Email: item.Email,
                    PhoneNumber: item.PhoneNumber,
                    Message: item.Message,
                    Status: core.getStatus(item.Status)
                });
            });

            $('#tbodyContent').html(render);

            $('#lblTotalRecords').text(res.RowCount);
            $('#lblFirstRow').text(res.FirstRowOnPage);
            $('#lblLastRow').text(res.LastRowOnPage);

            if (core.configs.pageSize < res.RowCount) {
                $('#paginationUL').show();

                wrapPaging(res.RowCount, function () {
                    loadData();
                }, isPageChanged);
            } else {
                $('#paginationUL').hide();
            }

            $('#chkAll').prop('checked', false);
            $('#chkAll').prop('indeterminate', false);
            $('#btnDeleteMultiple').prop('disabled', true); 
        });
    };

    var wrapPaging = function (recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / (core.configs.pageSize === -1 ? recordCount : core.configs.pageSize));

        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }

        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: 'Đầu',
            prev: 'Trước',
            next: 'Sau',
            last: 'Cuối',
            onPageClick: function (event, p) {
                core.configs.pageIndex = p;
                setTimeout(callBack(), 200);
            }
        });
    };
};