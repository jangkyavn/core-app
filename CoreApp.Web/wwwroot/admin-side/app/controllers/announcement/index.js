'use strict';

var AnnouncementController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnSearch').on('click', function () {
            loadData(true);
        });

        $('#btnRefresh').on('click', function () {
            $('#txtKeyword').val('');

            loadData(true);
        });

        $('#btnDeleteMultiple').on('click', function () {
            var listId = [];

            $('.chk-item:checked').each(function () {
                listId.push($(this).data('id'));
            });

            //core.confirm(message.confirm_delete_multiple, function () {
            //    core.callAjax('/Admin/Announcement/DeleteMultiple', 'POST', {
            //        jsonId: JSON.stringify(listId)
            //    }, 'json', function () {
            //        core.notify(message.delete_success, 'success');
            //        loadData(true);
            //    });
            //});
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

    var loadData = function (isPageChanged) {
        core.callAjax('/Admin/Announcement/GetAllPaging', 'GET', {
            keyword: $('#txtKeyword').val(),
            page: core.configs.pageIndex,
            pageSize: core.configs.pageSize
        }, 'json', function (res) {
            var data = res.Data;
            var render = "";
            var template = $('#tbodyTemplate').html();
            var numberOrder = data.FirstRowOnPage;

            if (data.RowCount > 0) {
                $.each(data.Results, function (i, item) {
                    render += Mustache.render(template, {
                        NumberOrder: numberOrder++,
                        Id: item.Id,
                        Title: item.Title,
                        Content: item.Content,
                        FullName: item.FullName,
                        DateCreated: moment(item.DateCreated).format('DD/MM/YYYY HH:mm:ss'),
                        HasRead: item.HasRead ? '<span class="badge badge-success">Đã đọc</span>' : '<span class="badge badge-danger">Chưa đọc</span>'
                    });
                });

                $('#tbodyContent').html(render);

                $('#lblTotalRecords').text(data.RowCount);
                $('#lblFirstRow').text(data.FirstRowOnPage);
                $('#lblLastRow').text(data.LastRowOnPage);

                if (core.configs.pageSize < data.RowCount) {
                    $('#paginationUL').show();

                    wrapPaging(data.RowCount, function () {
                        loadData();
                    }, isPageChanged);
                } else {
                    $('#paginationUL').hide();
                }
            } else {
                $('#tbodyContent').html('<div style="text-align: center; color: red;">Không có thông báo</div>');
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