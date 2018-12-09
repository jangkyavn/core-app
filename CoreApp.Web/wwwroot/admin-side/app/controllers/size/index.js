'use strict';

var sizeController = function () {
    this.initialize = function () {
        validateForm();
        loadData();
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnAddNew').on('click', function () {
            resetForm();
            $('#modalTitle').text('Thêm mới kích cỡ');
            $('#addEditModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('#btnSave').on('click', function () {
            if ($('#sizeForm').valid()) {
                var data = {
                    Id: $('#hidId').val(),
                    Name: $('#txtName').val()
                };

                core.callAjax('/Admin/Size/SaveEntity', 'POST', data, 'json', function (res) {
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
            $('#modalTitle').text('Sửa kích cỡ');
            var id = $(this).data('id');
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
                core.callAjax('/Admin/Size/Delete', 'POST', { id }, 'json', function () {
                    core.notify(message.delete_success, 'success');
                    loadData(true);
                });
            });
        });

        $('#chkAll').change(function () {
            var checkboxes = $(this).closest('table').find(':checkbox');
            checkboxes.prop('checked', $(this).is(':checked'));
        });

        $('table').on('change', '.chk-item', function () {
            if ($('.chk-item:checked').length === $('.chk-item').length) {
                $('#chkAll').prop('indeterminate', false);
                $('#chkAll').prop('checked', true);
            } else if ($('.chk-item:checked').length !== 0) {
                $('#chkAll').prop('indeterminate', true);
                $('#chkAll').prop('checked', false);
            } else {
                $('#chkAll').prop('indeterminate', false);
                $('#chkAll').prop('checked', false);
            }
        });

        $('#ddlShowPage').on('change', function () {
            core.configs.pageIndex = 1;
            core.configs.pageSize = parseInt($(this).val());

            loadData(true);
        });
    };

    var validateForm = function () {
        $('#sizeForm').validate({
            errorClass: 'text-danger',
            rules: {
                name: {
                    required: true,
                    maxlength: 50
                }
            },
            messages: {
                name: {
                    required: 'Hãy nhập tên kích thước',
                    maxlength: 'Tên kích thước không vượt quá 50 ký tự'
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
        core.callAjax('/Admin/Size/GetById', 'GET', { id }, 'json', function (res) {
            $('#hidId').val(res.Id);
            $('#txtName').val(res.Name);

            callBack();
        });
    };

    var resetForm = function () {
        $('#hidId').val(0);
        $('#txtName').val('');

        $("#sizeForm").validate().resetForm();
        $('input, textarea, select, file, number').removeClass('is-valid');
        $('input, textarea, select, file, number').removeClass('is-invalid');
    };

    var loadData = function (isPageChanged) {
        core.callAjax('/Admin/Size/GetAllPaging', 'GET', {
            keyword: $('#txtKeyword').val(),
            page: core.configs.pageIndex,
            pageSize: core.configs.pageSize
        }, 'json', function (res) {
            var render = '';
            var template = $('#tbodyTemplate').html();
            var numberOrder = res.FirstRowOnPage;

            $.each(res.Results, function (index, item) {
                render += Mustache.render(template, {
                    NumberOrder: numberOrder++,
                    Id: item.Id,
                    Name: item.Name,
                    Code: item.Code
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
