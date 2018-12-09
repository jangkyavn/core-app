'use strict';

var productCategoryController = function() {
    this.initialize = function() {
        validateForm();
        loadParents();
        initialTree();
        registerEvents();
    };

    var registerEvents = function() {
        $('#btnAddNew').on('click', function() {
            $('#modalTitle').text('Thêm mới danh mục sản phẩm');
            resetForm();
            $('#addEditModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('#txtName').on('keyup', function(e) {
            $('#txtSeoAlias').val(core.configs.domain + core.makeSeoAlias($(this).val()));
        });

        $('#fImage').on('change', function() {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();

            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }

            core.callAjaxFile('/Admin/Upload/UploadImage?type=products', 'POST', data, function (res) {
                if (res) {
                    $('#wrapImage').show();
                } else {
                    $('#wrapImage').hide();
                }

                $('#iImage').attr('src', `/uploaded/images/products/${res}`);
                $('#lblImage').text(res);
            });
        });

        $('#btnRemoveImage').on('click', function(e) {
            e.preventDefault();

            $('#lblImage').text('Chọn ảnh');
            $('#fImage').val(null);
            $('#iImage').attr('src', '');
            $('#wrapImage').hide();
        });

        $('#btnSave').on('click', function() {
            if ($('#productCategoryForm').valid()) {
                var data = {
                    Id: $('#txtId').val(),
                    Name: $('#txtName').val(),
                    Description: $('#txtDescription').val(),
                    ParentId: $('#ddlParent').val(),
                    Image: $('#lblImage').text() === 'Chọn ảnh' ? null : $('#lblImage').text(),
                    Status: $('#chkStatus').prop('checked') ? 1 : 0,
                    SeoPageTitle: $('#txtSeoPageTitle').val(),
                    SeoAlias: $('#txtSeoAlias').val().substring($('#txtSeoAlias').val().lastIndexOf('/') + 1, $('#txtSeoAlias').val().length),
                    SeoKeywords: $('#txtSeoKeywords').val(),
                    SeoDescription: $('#txtSeoDescription').val(),
                    DateCreated: $('#txtDateCreated').val(),
                    SortOrder: $('#txtSortOrder').val()
                };

                core.callAjax('/Admin/ProductCategory/SaveEntity', 'POST', data, 'json', function (res) {
                    if (res) {
                        core.notify(message.add_success, 'success');
                    } else {
                        core.notify(message.edit_success, 'success');
                    }

                    $('#addEditModal').modal('hide');
                    refreshTree();
                });
            }
        });

        $('#txtKeyword').on('keypress', function(e) {
            if (e.which === 13) {
                $('#btnSearch').click();
            }
        });

        $('#btnSearch').on('click', function() {
            var keyword = $('#txtKeyword').val();
            $('#treeContainer').jstree(true).search(keyword);
        });

        $('#btnRefresh').on('click', function() {
            $('#txtKeyword').val('');
            $('#treeContainer').jstree(true).search('');
        });

        $('#btnExpandTree').on('click', function () {
            $("#treeContainer").jstree().open_all(null, 200);
        });

        $('#btnCollapseTree').on('click', function () {
            $("#treeContainer").jstree().close_all(null, 200);
        });

        $(document).on('dnd_stop.vakata', function() {
            setTimeout(function() {
                var treeArr = $('#treeContainer').jstree(true).get_json();
                var jsonModel = JSON.stringify(treeArr);
                core.callAjax('/Admin/ProductCategory/UpdateTreeNodePosition', 'POST', { jsonModel }, 'json', function(res) {
                    if (res) {
                        core.notify(message.change_success, 'success');
                    }
                });
            }, 200);
        });
    };

    var getById = function(id) {
        core.callAjax('/Admin/ProductCategory/GetById', 'GET', { id }, 'json', function(res) {
            $('#txtId').val(res.Id);
            $('#txtDateCreated').val(res.DateCreated);
            $('#txtSortOrder').val(res.SortOrder);

            $('#txtName').val(res.Name);
            $('#txtDescription').val(res.Description);

            $('#ddlParent').val(res.ParentId);
            $('#fImage').val(null);
            if (res.Image !== null && res.Image !== '') {
                $('#wrapImage').show();
                $('#lblImage').text(res.Image);
                $('#iImage').attr('src', `/uploaded/images/products/${res.Image}`);
            }
            $('#chkStatus').prop('checked', res.Status);

            $('#txtSeoPageTitle').val(res.SeoPageTitle);
            $('#txtSeoAlias').val(core.configs.domain + res.SeoAlias);
            $('#txtSeoKeywords').val(res.SeoKeywords);
            $('#txtSeoDescription').val(res.SeoDescription);
        });
    };

    var validateForm = function() {
        $('#productCategoryForm').validate({
            errorClass: 'text-danger',
            rules: {
                name: {
                    required: true,
                    maxlength: 200
                },
                description: {
                    maxlength: 500
                },
                image: {
                    maxlength: 200
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
                description: {
                    maxlength: 'Mô tả không vượt quá 500 ký tự'
                },
                image: {
                    maxlength: 'Hình ảnh không vượt quá 200 ký tự'
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
            highlight: function(element) {
                $(element).addClass('is-invalid').removeClass('is-valid');
            },
            unhighlight: function(element) {
                $(element).addClass('is-valid').removeClass('is-invalid');
            }
        });
    };

    var resetForm = function() {
        $('#txtId').val(0);
        $('#txtDateCreated').val(null);
        $('#txtSortOrder').val(0);

        $('#txtName').val('');
        $('#txtDescription').val('');

        $('#ddlParent').val('');
        $('#lblImage').text('Chọn ảnh');
        $('#fImage').val(null);
        $('#iImage').attr('src', null);
        $('#wrapImage').hide();
        $('#chkStatus').prop('checked', false);

        $('#txtSeoPageTitle').val('');
        $('#txtSeoAlias').val(core.configs.domain);
        $('#txtSeoKeywords').val('');
        $('#txtSeoDescription').val('');

        $("#productCategoryForm").validate().resetForm();
        $('input, textarea, select, file, number').removeClass('is-valid');
        $('input, textarea, select, file, number').removeClass('is-invalid');
    };

    var loadParents = function() {
        core.callAjax('/Admin/ProductCategory/GetAllParent', 'GET', null, 'json', function(res) {
            var render = '<option value="">Chọn danh mục cha</option>';

            $.each(res, function (index, item) {
                if (item.ParentId === null) {
                    render += `<option value="${item.Id}">${item.Name}</option>`;
                } else {
                    render += `<option value="${item.Id}">&nbsp;&nbsp;&nbsp;${item.Name}</option>`;
                }
            });

            $('#ddlParent').html(render);
        });
    };

    var confirmDelete = function(id) {
        core.confirm(message.confirm_delete, function() {
            core.callAjax('/Admin/ProductCategory/Delete', 'POST', { id }, 'json', function() {
                core.notify(message.delete_success, 'success');
                refreshTree();
            });
        });
    };

    var refreshTree = function() {
        loadData(function(res) {
            $('#treeContainer').jstree(true).settings.core.data = res;
            $('#treeContainer').jstree(true).refresh();
        });
    };

    var initialTree = function() {
        loadData(function(res) {
            $('#treeContainer').jstree({
                'core': {
                    'multiple': false,
                    'data': res,
                    "check_callback": true
                },
                'plugins': ['contextmenu', 'dnd', 'search', 'types'],
                'types': {
                    '#': {
                        'max_depth': 3
                    }
                },
                'contextmenu': {
                    'select_node': false,
                    'items': function ($node) {
                       return loadContextMenu();
                    }
                }
            });
        });
    };

    var loadContextMenu = function () {
        var canUpdate = $('#hidCanUpdate').val();
        var canDelete = $('#hidCanDelete').val();

        if (canUpdate === 'value' && canDelete === 'value') {
            return {
                'Sửa': {
                    'separator_after': true,
                    'label': 'Sửa',
                    'action': function (obj) {
                        var inst = $.jstree.reference(obj.reference);
                        var id = inst.get_node(obj.reference).id;

                        resetForm();
                        getById(id);
                        $('#modalTitle').text('Sửa danh mục sản phẩm');
                        $('#addEditModal').modal({
                            show: true,
                            backdrop: 'static'
                        });
                    }
                },
                'Xóa': {
                    'label': 'Xóa',
                    'action': function (obj) {
                        var inst = $.jstree.reference(obj.reference);
                        var id = inst.get_node(obj.reference).id;

                        confirmDelete(id);
                    }
                }
            };
        }

        if (canUpdate === 'value' && canDelete === '') {
            return {
                'Sửa': {
                    'separator_after': true,
                    'label': 'Sửa',
                    'action': function (obj) {
                        var inst = $.jstree.reference(obj.reference);
                        var id = inst.get_node(obj.reference).id;

                        resetForm();
                        getById(id);
                        $('#modalTitle').text('Sửa danh mục sản phẩm');
                        $('#addEditModal').modal({
                            show: true,
                            backdrop: 'static'
                        });
                    }
                }
            };
        }

        if (canUpdate === '' && canDelete === 'value') {
            return {
                'Xóa': {
                    'label': 'Xóa',
                    'action': function (obj) {
                        var inst = $.jstree.reference(obj.reference);
                        var id = inst.get_node(obj.reference).id;

                        confirmDelete(id);
                    }
                }
            };
        }

        return {};
    };

    var loadData = function(callBack) {
        core.callAjax('/Admin/ProductCategory/GetAllHierarchy', 'GET', null, 'json', function(res) {
            var data = [];

            $.each(res, function(index, item) {
                data.push({
                    id: item.Id,
                    text: item.Name,
                    parentId: item.ParentId
                });
            });

            var dataUnflattern = core.unflattern(data);
            callBack(dataUnflattern);
        });
    };
};
