'use strict';

var productController = function () {
    var productQuantity = new ProductQuantity();
    var productImage = new ProductImage();
    var wholePrice = new WholePrice();

    this.initialize = function () {
        validateForm();
        loadCategoriesDropDown();
        loadData();
        registerEvents();
        registerControls();
        productQuantity.initialize();
        productImage.initialize();
        wholePrice.initialize();
    };

    var registerEvents = function () {
        $('#btnAddNew').on('click', function () {
            resetForm();
            $('#modalTitle').text('Thêm mới sản phẩm');
            $('#addEditModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('#txtName').on('keyup', function (e) {
            $('#txtSeoAlias').val(window.location.host + '/' + core.makeSeoAlias($(this).val()));
        });

        $('#fImage').on('change', function () {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();

            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }

            core.callAjaxFile('/Admin/Upload/UploadImage?type=products', 'POST', data, function (res) {
                if (res) {
                    $('#wrapImage').show();
                    $('#iImage').attr('src', `/uploaded/images/products/${res}`);
                    $('#lblImage').text(res);
                } else {
                    $('#wrapImage').hide();
                    $('#lblImage').text('Chọn ảnh');
                    $('#fImage').val(null);
                    $('#iImage').attr('src', '');
                }
            });
        });

        $('#btnRemoveImage').on('click', function (e) {
            e.preventDefault();

            $('#lblImage').text('Chọn ảnh');
            $('#fImage').val(null);
            $('#iImage').attr('src', '');
            $('#wrapImage').hide();
        });

        $('#btnSave').on('click', function () {
            if ($('#productForm').valid()) {
                var data = {
                    Id: $('#txtId').val(),
                    DateCreated: $('#txtDateCreated').val(),
                    ViewCount: $('#txtViewCount').val(),
                    Name: $('#txtName').val(),
                    Description: $('#txtDescription').val(),
                    Content: CKEDITOR.instances.txtContent.getData(),
                    Unit: $('#txtUnit').val(),
                    CategoryId: $('#ddlCategory').val(),
                    Price: $('#txtPrice').val().replace(',', ''),
                    OriginalPrice: $('#txtOriginalPrice').val().replace(',', ''),
                    PromotionPrice: $('#txtPromotionPrice').val().replace(',', ''),
                    Image: $('#lblImage').text() === 'Chọn ảnh' ? null : $('#lblImage').text(),
                    Tags: $('#txtTags').val(),
                    Status: $('#chkStatus').prop('checked') ? 1 : 0,
                    HotFlag: $('#chkHotFlag').prop('checked'),
                    SeoPageTitle: $('#txtSeoPageTitle').val(),
                    SeoAlias: $('#txtSeoAlias').val().substring($('#txtSeoAlias').val().lastIndexOf('/') + 1, $('#txtSeoAlias').val().length),
                    SeoKeywords: $('#txtSeoKeywords').val(),
                    SeoDescription: $('#txtSeoDescription').val()
                };

                core.callAjax('/Admin/Product/SaveEntity', 'POST', data, 'json', function (res) {
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
            $('#ddlSearchCategories').val('');

            loadData(true);
        });

        $('#btnImportExcel').on('click', function () {
            $('#importExcelModal').modal('show');
        });

        $('#btnExportExcel').on('click', function () {
            core.callAjax('/Admin/Product/ExportExcel', 'POST', null, 'text', function (res) {
                window.location.href = res;
            });
        });

        $('#btnDeleteMultiple').on('click', function () {
            var listId = [];

            $('.chk-item:checked').each(function () {
                listId.push($(this).data('id'));
            });

            core.confirm(message.confirm_delete_multiple, function () {
                core.callAjax('/Admin/Product/DeleteMultiple', 'POST', {
                    jsonId: JSON.stringify(listId)
                }, 'json', function () {
                    core.notify(message.delete_success, 'success');
                    loadData(true);
                });
            });
        });

        $('#btnSaveImportExcel').on('click', function () {
            var fileUpload = $("#fileImportExcel").get(0);
            var files = fileUpload.files;

            // Create FormData object  
            var fileData = new FormData();
            // Looping over all files and add it to FormData object  
            for (var i = 0; i < files.length; i++) {
                fileData.append("files", files[i]);
            }
            // Adding one more key to FormData object  
            fileData.append('categoryId', $('#ddlCategoryImportExcel').val());

            core.callAjaxFile('/Admin/Product/ImportExcel', 'POST', fileData, function (res) {
                $('#importExcelModal').modal('hide');
                core.notify(message.import_file_success, 'success');
                loadData();
            });

            return false;
        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            resetForm();
            $('#modalTitle').text('Sửa sản phẩm');
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
                core.callAjax('/Admin/Product/Delete', 'POST', { id }, 'json', function () {
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

    var registerControls = function () {
        $('#txtTags').tagsinput({
            tagClass: 'badge badge-primary'
        });

        $('#txtPrice').mask('000,000,000,000', { reverse: true });
        $('#txtOriginalPrice').mask('000,000,000,000', { reverse: true });
        $('#txtPromotionPrice').mask('000,000,000,000', { reverse: true });

        CKEDITOR.replace('txtContent', {
            extraPlugins: 'colorbutton',
            filebrowserImageUploadUrl: '/Admin/Upload/UploadImageForCKEditor',
            filebrowserUploadMethod: 'form',
            height: '30rem'
        });

        //Fix: cannot click on element ck in modal
        $.fn.modal.Constructor.prototype._enforceFocus = function () {
            //var $modalElement = this.$element;
            //$(document).on('focusin.modal', function (e) {
            //    if ($modalElement[0] !== e.target
            //        && !$modalElement.has(e.target).length
            //        && $(e.target).parentsUntil('*[role="dialog"]').length === 0) {
            //        $modalElement.focus();
            //    }
            //});
        };
    };

    var loadCategoriesDropDown = function () {
        core.callAjax('/Admin/ProductCategory/GetAllHierarchy', 'GET', null, 'json', function (res) {
            var renderS = '<option value="">Chọn danh mục cần tìm</option>';
            var render = '<option value="" disabled>Chọn danh mục</option>';

            $.each(res, function (index, item) {
                if (item.ParentId === null) {
                    renderS += `<option value=${item.Id}>${item.Name}</option>`;
                    render += `<optgroup label="${item.Name}">`;
                } else {
                    var lstChild = $.map(res, function (item1) {
                        if (item1.ParentId !== null) 
                            return item1;
                    });
                    var lstParent = $.map(res, function (item2) {
                        if (item2.ParentId === null) {
                            return item2;
                        }
                    });

                    if (!lstChild.some(x => x.ParentId === item.Id) && lstParent.some(x => x.Id === item.ParentId)) {
                        renderS += `<option value=${item.Id}>&nbsp;&nbsp;&nbsp;${item.Name}</option>`;
                        render += `<option value=${item.Id}>&nbsp;&nbsp;&nbsp;${item.Name}</option>`;
                    } else {
                        if (!lstChild.some(x => x.Id === item.ParentId)) {
                            renderS += `<option value=${item.Id}>&nbsp;&nbsp;&nbsp;${item.Name}</option>`;
                            render += `<option value=${item.Id} disabled style='font-weight: bold;'>&nbsp;&nbsp;&nbsp;${item.Name}</option>`;
                        } else {
                            renderS += `<option value=${item.Id}>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;${item.Name}</option>`;
                            render += `<option value=${item.Id}>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;${item.Name}</option>`;
                        }
                    }
                }
            });

            $('#ddlSearchCategories').html(renderS);
            $('#ddlCategory').html(render);
            $('#ddlCategoryImportExcel').html(render);
        });
    };

    var getById = function (id, callBack) {
        core.callAjax('/Admin/Product/GetById', 'GET', { id }, 'json', function (res) {
            $('#txtId').val(res.Id);
            $('#txtDateCreated').val(res.DateCreated);
            $('#txtViewCount').val(res.ViewCount);
            $('#txtName').val(res.Name);
            $('#txtDescription').val(res.Description);
            CKEDITOR.instances.txtContent.setData(res.Content);
            $('#txtUnit').val(res.Unit);
            $('#ddlCategory').val(res.CategoryId);
            $('#txtPrice').val(core.formatNumber(res.Price));
            $('#txtOriginalPrice').val(core.formatNumber(res.OriginalPrice));
            $('#txtPromotionPrice').val(core.formatNumber(res.PromotionPrice));
            if (res.Image !== null && res.Image !== '') {
                $('#wrapImage').show();
                $('#lblImage').text(res.Image);
                $('#iImage').attr('src', `/uploaded/images/products/${res.Image}`);
            }
            $('#txtTags').tagsinput('add', res.Tags);
            $('#chkStatus').prop('checked', res.Status);
            $('#chkHotFlag').prop('checked', res.HotFlag);
            $('#txtSeoPageTitle').val(res.SeoPageTitle);
            $('#txtSeoAlias').val(window.location.host + '/' + res.SeoAlias);
            $('#txtSeoKeywords').val(res.SeoKeywords);
            $('#txtSeoDescription').val(res.SeoDescription);

            callBack();
        });
    };

    var validateForm = function () {
        $('#productForm').validate({
            errorClass: 'text-danger',
            rules: {
                name: {
                    required: true,
                    maxlength: 200
                },
                categoryId: {
                    required: true
                },
                image: {
                    maxlength: 200
                },
                price: {
                    required: true
                },
                originalPrice: {
                    required: true
                },
                description: {
                    maxlength: 500
                },
                unit: {
                    required: true,
                    maxlength: 50
                },
                seoPageTitle: {
                    maxlength: 200
                },
                seoKeywords: {
                    maxlength: 200
                },
                seoDescription: {
                    maxlength: 200
                }
            },
            messages: {
                name: {
                    required: 'Hãy nhập tên danh mục',
                    maxlength: 'Tên danh mục không vượt quá 200 ký tự'
                },
                categoryId: {
                    required: 'Hãy chọn danh mục sản phẩm'
                },
                image: {
                    maxlength: 'Hình ảnh không vượt quá 200 ký tự'
                },
                price: {
                    required: 'Hãy nhập giá bán'
                },
                originalPrice: {
                    required: 'Hãy nhập giá gốc'
                },
                description: {
                    maxlength: 'Mô tả không vượt quá 500 ký tự'
                },
                unit: {
                    required: 'Hãy nhập đơn vị',
                    maxlength: 'Đơn vị không vượt quá 50 ký tự'
                },
                seoPageTitle: {
                    maxlength: 'Tiêu đề SEO không vượt quá 200 ký tự'
                },
                seoKeywords: {
                    maxlength: 'Từ khóa SEO không vượt quá 200 ký tự'
                },
                seoDescription: {
                    maxlength: 'Mô tả SEO không vượt quá 200 ký tự'
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

    var resetForm = function () {
        $('#txtId').val(0);
        $('#txtDateCreated').val(null);
        $('#txtViewCount').val(0);

        $('#txtName').val('');
        $('#txtDescription').val('');
        CKEDITOR.instances.txtContent.setData('');
        $('#txtUnit').val('');

        $('#ddlCategory').val('');
        $('#txtPrice').val('');
        $('#txtOriginalPrice').val('');
        $('#txtPromotionPrice').val('');
        $('#lblImage').text('Chọn ảnh');
        $('#fImage').val(null);
        $('#iImage').attr('src', null);
        $('#wrapImage').hide();
        $("#txtTags").tagsinput('removeAll');
        $('#chkStatus').prop('checked', false);
        $('#chkHotFlag').prop('checked', false);

        $('#txtSeoPageTitle').val('');
        $('#txtSeoAlias').val(window.location.host);
        $('#txtSeoKeywords').val('');
        $('#txtSeoDescription').val('');

        $("#productForm").validate().resetForm();
        $('input, textarea, select, file, number').removeClass('is-valid');
        $('input, textarea, select, file, number').removeClass('is-invalid');
    };

    var loadData = function (isPageChanged) {
        core.callAjax('/Admin/Product/GetAllPaging', 'GET', {
            keyword: $('#txtKeyword').val(),
            categoryId: $('#ddlSearchCategories').val(),
            page: core.configs.pageIndex,
            pageSize: core.configs.pageSize
        }, 'json', function (res) {
            var render = '';
            var template = $('#tbodyTemplate').html();
            var numberOrder = res.FirstRowOnPage;

            console.log(res.Results);

            $.each(res.Results, function (index, item) {
                render += Mustache.render(template, {
                    NumberOrder: numberOrder++,
                    Id: item.Id,
                    ProductName: item.Name,
                    CategoryName: item.ProductCategory.Name,
                    Price: core.formatNumber(item.Price, 0),
                    Image: core.getImage('products', item.Image),
                    ImageName: item.Image === null ? 'no-image.png' : 'products/' + item.Image,
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

            $('.popup-link').magnificPopup({
                type: 'image'
            });

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