'use strict';

var billController = function () {
    var cachedObj = {
        products: [],
        colors: [],
        sizes: [],
        paymentMethods: [],
        billStatuses: []
    };

    this.initialize = function () {
        initDateRangePicker();
        validateForm();

        $.when(loadBillStatus(),
            loadPaymentMethod(),
            loadColors(),
            loadSizes(),
            loadProducts())
            .done(function () {
                loadData();
            });

        registerEvents();
    };

    var registerEvents = function () {
        $("#btnSearch").on('click', function () {
            loadData(true);
        });

        $('#btnRefresh').on('click', function () {
            $('#txtKeyword').val('');
            $('#txtDateRange').val(`${core.dateFormatJson(new Date())} - ${core.dateFormatJson(new Date())}`);

            loadData(true);
        });

        $("#btnAddNew").on('click', function () {
            resetForm();
            $('#modalTitle').text('Thêm mới hóa đơn');
            $('#detailModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('body').on('click', '.btn-view', function (e) {
            e.preventDefault();
            resetForm();
            $('#modalTitle').text('Xem hóa đơn');
           
            var id = $(this).data('id');

            core.callAjax('/Admin/Bill/GetById', 'GET', { id }, 'json', function (res) {
                $('#hidId').val(res.Id);
                $('#hidDateCreated').val(res.DateCreated);
                $('#txtCustomerName').val(res.CustomerName);
                $('#txtCustomerAddress').val(res.CustomerAddress);
                $('#txtCustomerMobile').val(res.CustomerMobile);
                $('#txtCustomerMessage').val(res.CustomerMessage);
                $('#ddlPaymentMethod').val(res.PaymentMethod);
                $('#ddlCustomerId').val(res.CustomerId);
                $('#ddlBillStatus').val(res.BillStatus);

                var billDetails = res.BillDetails;
                if (billDetails !== null && billDetails.length > 0) {
                    var render = '';
                    var templateDetails = $('#billDetailsTemplate').html();

                    $.each(billDetails, function (i, item) {
                        var products = getProductOptions(item.ProductId);
                        var colors = getColorOptions(item.ColorId);
                        var sizes = getSizeOptions(item.SizeId);

                        render += Mustache.render(templateDetails,
                            {
                                Id: item.Id,
                                Products: products,
                                Colors: colors,
                                Sizes: sizes,
                                Quantity: item.Quantity
                            });
                    });

                    $('#billDetailsContent').html(render);
                }

                $('#detailModal').modal({
                    show: true,
                    backdrop: 'static'
                });
            });
        });

        $('#btnAddDetail').on('click', function () {
            var template = $('#billDetailsTemplate').html();
            var products = getProductOptions(null);
            var colors = getColorOptions(null);
            var sizes = getSizeOptions(null);
            var render = Mustache.render(template,
                {
                    Id: 0,
                    Products: products,
                    Colors: colors,
                    Sizes: sizes,
                    Quantity: 0,
                    Total: 0
                });
            $('#billDetailsContent').append(render);
        });

        $('body').on('click', '.btn-delete-detail', function () {
            $(this).parent().parent().remove();
        });

        $('#btnSave').on('click', function (e) {
            if ($('#billForm').valid()) {
                var billDetails = [];
                $.each($('#billDetailsContent tr'), function (i, item) {
                    billDetails.push({
                        Id: $(item).data('id'),
                        ProductId: $(item).find('select.ddlProductId').first().val(),
                        Quantity: $(item).find('input.txtQuantity').first().val(),
                        ColorId: $(item).find('select.ddlColorId').first().val(),
                        SizeId: $(item).find('select.ddlSizeId').first().val(),
                        BillId: $('#hidId').val()
                    });
                });

                var data = {
                    Id: $('#hidId').val(),
                    DateCreated: $('#hidDateCreated').val(),
                    CustomerName: $('#txtCustomerName').val(),
                    CustomerAddress: $('#txtCustomerAddress').val(),
                    CustomerId: $('#ddlCustomerId').val(),
                    CustomerMobile: $('#txtCustomerMobile').val(),
                    CustomerMessage: $('#txtCustomerMessage').val(),
                    PaymentMethod: $('#ddlPaymentMethod').val(),
                    BillStatus: $('#ddlBillStatus').val(),
                    BillDetails: billDetails
                };

                console.log(data);
                core.callAjax('/Admin/Bill/SaveEntity', 'POST', data, 'json', function (res) {
                    core.notify(message.add_success, 'success');
                    $('#detailModal').modal('hide');
                    loadData(true);
                });
            }
        });

        $("#btnExport").on('click', function () {
            var billId = $('#hidId').val();

            core.callAjax('/Admin/Bill/ExportExcel', 'POST', { billId }, 'text', function (res) {
                window.location.href = res;
            });
        });

        $('table').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');

            core.confirm(message.confirm_delete, function () {
                core.callAjax('/Admin/Bill/Delete', 'POST', { id }, 'json', function () {
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
        $('#billForm').validate({
            errorClass: 'text-danger',
            rules: {
                customerName: {
                    required: true,
                    maxlength: 50
                },
                customerMobile: {
                    required: true,
                    maxlength: 20
                },
                customerAddress: {
                    required: true,
                    maxlength: 500
                },
                customerMessage: {
                    maxlength: 500
                },
                paymentMethod: {
                    required: true
                },
                billStatus: {
                    required: true
                }
            },
            messages: {
                customerName: {
                    required: 'Hãy nhập tên khách hàng',
                    maxlength: 'Tên khách hàng không vượt quá 50 ký tự'
                },
                customerMobile: {
                    required: 'Hãy nhập số điện thoại',
                    maxlength: 'Số điện thoại không vượt quá 20 ký tự'
                },
                customerAddress: {
                    required: 'Hãy nhập địa chỉ',
                    maxlength: 'Địa chỉ không vượt quá 500 ký tự'
                },
                customerMessage: {
                    maxlength: 'Tin nhắn không vượt quá 500 ký tự'
                },
                paymentMethod: {
                    required: 'Hãy chọn phương thức thanh toán'
                },
                billStatus: {
                    required: 'Hãy chọn trình trạng đơn hàng'
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

    var initDateRangePicker = function () {
        $('input[name="daterange"]').daterangepicker({
            "startDate": new Date(),
            "endDate": new Date(),
            "locale": {
                "format": "DD/MM/YYYY",
                "applyLabel": "Áp dụng",
                "cancelLabel": "Hủy",
                "fromLabel": "Từ",
                "toLabel": "Tới",
                "daysOfWeek": [
                    "CN",
                    "T2",
                    "T3",
                    "T4",
                    "T5",
                    "T6",
                    "T7"
                ],
                "monthNames": [
                    "Tháng 1",
                    "Tháng 2",
                    "Tháng 3",
                    "Tháng 4",
                    "Tháng 5",
                    "Tháng 6",
                    "Tháng 7",
                    "Tháng 8",
                    "Tháng 9",
                    "Tháng 10",
                    "Tháng 11",
                    "Tháng 12"
                ]
            }
        });
    };

    var loadBillStatus = function () {
        return core.callAjax('/Admin/Bill/GetBillStatus', 'GET', null, 'json', function (res) {
            cachedObj.billStatuses = res;
            var render = "";
            $.each(res, function (i, item) {
                render += "<option value='" + item.Value + "'>" + item.Name + "</option>";
            });
            $('#ddlBillStatus').html(render);
        });
    };

    var loadPaymentMethod = function () {
        return core.callAjax('/Admin/Bill/GetPaymentMethod', 'GET', null, 'json', function (res) {
            cachedObj.paymentMethods = res;
            var render = "";
            $.each(res, function (i, item) {
                render += "<option value='" + item.Value + "'>" + item.Name + "</option>";
            });
            $('#ddlPaymentMethod').html(render);
        });
    };

    var loadProducts = function () {
        return core.callAjax('/Admin/Product/GetAll', 'GET', null, 'json', function (res) {
            cachedObj.products = res;
        });
    };

    var loadColors = function () {
        return core.callAjax('/Admin/Color/GetAll', 'GET', null, 'json', function (res) {
            cachedObj.colors = res;
        });
    };

    var loadSizes = function () {
        return core.callAjax('/Admin/Size/GetAll', 'GET', null, 'json', function (res) {
            cachedObj.sizes = res;
        });
    };

    var getProductOptions = function (selectedId) {
        var products = "<select class='form-control ddlProductId'>";
        $.each(cachedObj.products, function (i, product) {
            if (selectedId === product.Id)
                products += '<option value="' + product.Id + '" selected="select">' + product.Name + '</option>';
            else
                products += '<option value="' + product.Id + '">' + product.Name + '</option>';
        });
        products += "</select>";
        return products;
    };

    var getColorOptions = function (selectedId) {
        var colors = "<select class='form-control ddlColorId'>";
        $.each(cachedObj.colors, function (i, color) {
            if (selectedId === color.Id)
                colors += '<option value="' + color.Id + '" selected="select">' + color.Name + '</option>';
            else
                colors += '<option value="' + color.Id + '">' + color.Name + '</option>';
        });
        colors += "</select>";
        return colors;
    };

    var getSizeOptions = function (selectedId) {
        var sizes = "<select class='form-control ddlSizeId'>";
        $.each(cachedObj.sizes, function (i, size) {
            if (selectedId === size.Id)
                sizes += '<option value="' + size.Id + '" selected="select">' + size.Name + '</option>';
            else
                sizes += '<option value="' + size.Id + '">' + size.Name + '</option>';
        });
        sizes += "</select>";
        return sizes;
    };

    var getPaymentMethodName = function (paymentMethod) {
        var method = $.grep(cachedObj.paymentMethods, function (element, index) {
            return element.Value === paymentMethod;
        });
        if (method.length > 0)
            return method[0].Name;
        else return '';
    };

    var getBillStatusName = function (billStatus) {
        var status = $.grep(cachedObj.billStatuses, function (element, index) {
            return element.Value === billStatus;
        });
        if (status.length > 0)
            return status[0].Name;
        else return '';
    };

    var resetForm = function () {
        $('#hidId').val(0);
        $('#hidDateCreated').val(null);
        $('#txtCustomerName').val('');
        $('#txtCustomerAddress').val('');
        $('#txtCustomerMobile').val('');
        $('#txtCustomerMessage').val('');
        $('#ddlPaymentMethod').val('');
        $('#ddlCustomerId').val('');
        $('#ddlBillStatus').val('');
        $('#billDetailsContent').html('');
    };

    var loadData = function (isPageChanged) {
        var dateArr = $('#txtDateRange').val().split(' - ');

        return core.callAjax('/Admin/Bill/GetAllPaging', 'GET', {
            startDate: dateArr[0],
            endDate: dateArr[1],
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
                    CustomerName: item.CustomerName,
                    PaymentMethod: getPaymentMethodName(item.PaymentMethod),
                    DateCreated: core.dateFormatJson(item.DateCreated),
                    BillStatus: getBillStatusName(item.BillStatus)
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