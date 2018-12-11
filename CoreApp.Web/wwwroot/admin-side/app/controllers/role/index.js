'use strict';

var roleController = function () {
    this.initialize = function () {
        validateForm();
        loadData();
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnAddNew').on('click', function () {
            resetForm();
            $('#modalTitle').text('Thêm mới vai trò');
            $('#addEditModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('#btnSave').on('click', function () {
            if ($('#roleForm').valid()) {
                var data = {
                    Id: $('#txtId').val(),
                    Name: $('#txtName').val(),
                    Description: $('#txtDescription').val()
                };

                core.callAjax('/Admin/Role/SaveEntity', 'POST', data, 'json', function (res) {
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

        $('body').on('click', '.btn-grant', function () {
            $('#hidRoleId').val($(this).data('id'));
          
            loadFunctionList(function (res) {
                fillPermission($('#hidRoleId').val(), res);
            });

            $('#grantPermissionModal').modal('show');
        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            resetForm();
            $('#modalTitle').text('Sửa vai trò');
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
                core.callAjax('/Admin/Role/Delete', 'POST', { id }, 'json', function () {
                    core.notify(message.delete_success, 'success');
                    loadData(true);
                });
            });
        });

        $("#btnSavePermission").on('click', function () {
            var listPermmission = [];
            $.each($('#tblFunction tbody tr'), function (i, item) {
                listPermmission.push({
                    RoleId: $('#hidRoleId').val(),
                    FunctionId: $(item).data('id'),
                    CanRead: $(item).find('.chk-read').first().prop('checked'),
                    CanCreate: $(item).find('.chk-create').first().prop('checked'),
                    CanUpdate: $(item).find('.chk-edit').first().prop('checked'),
                    CanDelete: $(item).find('.chk-delete').first().prop('checked')
                });
            });

            core.callAjax('/Admin/Role/SavePermission', 'POST', {
                listPermmission: listPermmission,
                roleId: $('#hidRoleId').val()
            }, 'json', function (res) {
                $('#grantPermissionModal').modal('hide');
                core.notify('Cấp quyền thành công', 'success');
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

    var loadFunctionList = function (callback) {
        core.callAjax('/Admin/Function/GetAllHierarchy', 'GET', null, 'json', function (res) {
            var template = $('#permissionDataTemplate').html();
            var render = '';

            $.each(res, function (i, item) {
                render += Mustache.render(template, {
                    Id: item.Id,
                    Name: item.Name,
                    treegridparent: item.ParentId !== null ? `treegrid-parent-${item.ParentId}` : `root-active`,
                    AllowCreate: item.AllowCreate ? "checked" : "",
                    AllowEdit: item.AllowEdit ? "checked" : "",
                    AllowView: item.AllowView ? "checked" : "",
                    AllowDelete: item.AllowDelete ? "checked" : "",
                    isCheckReadAllParent: item.ParentId !== null ? `chk-read-item-${item.ParentId}` : `chk-read-parent-${item.Id}`,
                    isCheckCreateAllParent: item.ParentId !== null ? `chk-create-item-${item.ParentId}` : `chk-create-parent-${item.Id}`,
                    isCheckEditAllParent: item.ParentId !== null ? `chk-edit-item-${item.ParentId}` : `chk-edit-parent-${item.Id}`,
                    isCheckDeleteAllParent: item.ParentId !== null ? `chk-delete-item-${item.ParentId}` : `chk-delete-parent-${item.Id}`
                });
            });

            $('#permissionDataContent').html(render);
            $('.tree').treegrid();

            $('#chkAllRead').on('click', function () {
                $('.chk-read').prop('checked', $(this).prop('checked'));
            });

            $('#chkAllCreate').on('click', function () {
                $('.chk-create').prop('checked', $(this).prop('checked'));
            });

            $('#chkAllEdit').on('click', function () {
                $('.chk-edit').prop('checked', $(this).prop('checked'));
            });

            $('#chkAllDelete').on('click', function () {
                $('.chk-delete').prop('checked', $(this).prop('checked'));
            });

            $('.chk-read').on('click', function () {
                var currentFunctionId = $(this).val();
                var parent = res.find(x => x.Id === currentFunctionId).ParentId;

                if (parent === null) {
                    $(`.chk-read-item-${currentFunctionId}`).prop('checked', $(`.chk-read-parent-${currentFunctionId}`).prop('checked'));
                } else {
                    if ($(`.chk-read-item-${parent}:checked`).length === $(`.chk-read-item-${parent}`).length) {
                        $(`.chk-read-parent-${parent}`).prop('indeterminate', false);
                        $(`.chk-read-parent-${parent}`).prop('checked', true);
                    } else if ($(`.chk-read-item-${parent}:checked`).length !== 0) {
                        $(`.chk-read-parent-${parent}`).prop('indeterminate', true);
                        $(`.chk-read-parent-${parent}`).prop('checked', false);
                    } else {
                        $(`.chk-read-parent-${parent}`).prop('indeterminate', false);
                        $(`.chk-read-parent-${parent}`).prop('checked', false);
                    }
                }

                if ($('.chk-read:checked').length === res.length) {
                    $('#chkAllRead').prop('indeterminate', false);
                    $('#chkAllRead').prop('checked', true);
                } else if ($('.chk-read:checked').length !== 0) {
                    $('#chkAllRead').prop('indeterminate', true);
                    $('#chkAllRead').prop('checked', false);
                } else {
                    $('#chkAllRead').prop('indeterminate', false);
                    $('#chkAllRead').prop('checked', false);
                }
            });

            $('.chk-create').on('click', function () {
                var currentFunctionId = $(this).val();
                var parent = res.find(x => x.Id === currentFunctionId).ParentId;

                if (parent === null) {
                    $(`.chk-create-item-${currentFunctionId}`).prop('checked', $(`.chk-create-parent-${currentFunctionId}`).prop('checked'));
                } else {
                    if ($(`.chk-create-item-${parent}:checked`).length === $(`.chk-create-item-${parent}`).length) {
                        $(`.chk-create-parent-${parent}`).prop('indeterminate', false);
                        $(`.chk-create-parent-${parent}`).prop('checked', true);
                    } else if ($(`.chk-create-item-${parent}:checked`).length !== 0) {
                        $(`.chk-create-parent-${parent}`).prop('indeterminate', true);
                        $(`.chk-create-parent-${parent}`).prop('checked', false);
                    } else {
                        $(`.chk-create-parent-${parent}`).prop('indeterminate', false);
                        $(`.chk-create-parent-${parent}`).prop('checked', false);
                    }
                }

                if ($('.chk-create:checked').length === res.length) {
                    $('#chkAllCreate').prop('indeterminate', false);
                    $('#chkAllCreate').prop('checked', true);
                } else if ($('.chk-create:checked').length !== 0) {
                    $('#chkAllCreate').prop('indeterminate', true);
                    $('#chkAllCreate').prop('checked', false);
                } else {
                    $('#chkAllCreate').prop('indeterminate', false);
                    $('#chkAllCreate').prop('checked', false);
                }
            });

            $('.chk-edit').on('click', function () {
                var currentFunctionId = $(this).val();
                var parent = res.find(x => x.Id === currentFunctionId).ParentId;

                if (parent === null) {
                    $(`.chk-edit-item-${currentFunctionId}`).prop('checked', $(`.chk-edit-parent-${currentFunctionId}`).prop('checked'));
                } else {
                    if ($(`.chk-edit-item-${parent}:checked`).length === $(`.chk-edit-item-${parent}`).length) {
                        $(`.chk-edit-parent-${parent}`).prop('indeterminate', false);
                        $(`.chk-edit-parent-${parent}`).prop('checked', true);
                    } else if ($(`.chk-edit-item-${parent}:checked`).length !== 0) {
                        $(`.chk-edit-parent-${parent}`).prop('indeterminate', true);
                        $(`.chk-edit-parent-${parent}`).prop('checked', false);
                    } else {
                        $(`.chk-edit-parent-${parent}`).prop('indeterminate', false);
                        $(`.chk-edit-parent-${parent}`).prop('checked', false);
                    }
                }

                if ($('.chk-edit:checked').length === res.length) {
                    $('#chkAllEdit').prop('indeterminate', false);
                    $('#chkAllEdit').prop('checked', true);
                } else if ($('.chk-edit:checked').length !== 0) {
                    $('#chkAllEdit').prop('indeterminate', true);
                    $('#chkAllEdit').prop('checked', false);
                } else {
                    $('#chkAllEdit').prop('indeterminate', false);
                    $('#chkAllEdit').prop('checked', false);
                }
            });

            $('.chk-delete').on('click', function () {
                var currentFunctionId = $(this).val();
                var parent = res.find(x => x.Id === currentFunctionId).ParentId;

                if (parent === null) {
                    $(`.chk-delete-item-${currentFunctionId}`).prop('checked', $(`.chk-delete-parent-${currentFunctionId}`).prop('checked'));
                } else {
                    if ($(`.chk-delete-item-${parent}:checked`).length === $(`.chk-delete-item-${parent}`).length) {
                        $(`.chk-delete-parent-${parent}`).prop('indeterminate', false);
                        $(`.chk-delete-parent-${parent}`).prop('checked', true);
                    } else if ($(`.chk-delete-item-${parent}:checked`).length !== 0) {
                        $(`.chk-delete-parent-${parent}`).prop('indeterminate', true);
                        $(`.chk-delete-parent-${parent}`).prop('checked', false);
                    } else {
                        $(`.chk-delete-parent-${parent}`).prop('indeterminate', false);
                        $(`.chk-delete-parent-${parent}`).prop('checked', false);
                    }
                }

                if ($('.chk-delete:checked').length === res.length) {
                    $('#chkAllDelete').prop('indeterminate', false);
                    $('#chkAllDelete').prop('checked', true);
                } else if ($('.chk-delete:checked').length !== 0) {
                    $('#chkAllDelete').prop('indeterminate', true);
                    $('#chkAllDelete').prop('checked', false);
                } else {
                    $('#chkAllDelete').prop('indeterminate', false);
                    $('#chkAllDelete').prop('checked', false);
                }
            });

            callback(res);
        });
    };

    var fillPermission = function (roleId, functions) {
        core.callAjax('/Admin/Role/GetListFunctionWithRole', 'GET', { roleId }, 'json', function (res) {
            $.each($('#tblFunction tbody tr'), function (i, item) {
                $.each(res, function (j, jitem) {
                    if (jitem.FunctionId === $(item).data('id')) {
                        $(item).find('.chk-read').first().prop('checked', jitem.CanRead);
                        $(item).find('.chk-create').first().prop('checked', jitem.CanCreate);
                        $(item).find('.chk-edit').first().prop('checked', jitem.CanUpdate);
                        $(item).find('.chk-delete').first().prop('checked', jitem.CanDelete);
                    }
                });
            });

            if ($('.chk-read:checked').length === $('#tblFunction tbody tr .chk-read').length) {
                $('#chkAllRead').prop('indeterminate', false);
                $('#chkAllRead').prop('checked', true);
            } else if ($('.chk-read:checked').length !== 0) {
                $('#chkAllRead').prop('indeterminate', true);
                $('#chkAllRead').prop('checked', false);
            } else {
                $('#chkAllRead').prop('indeterminate', false);
                $('#chkAllRead').prop('checked', false);
            }

            if ($('.chk-create:checked').length === $('#tblFunction tbody tr .chk-create').length) {
                $('#chkAllCreate').prop('indeterminate', false);
                $('#chkAllCreate').prop('checked', true);
            } else if ($('.chk-create:checked').length !== 0) {
                $('#chkAllCreate').prop('indeterminate', true);
                $('#chkAllCreate').prop('checked', false);
            } else {
                $('#chkAllCreate').prop('indeterminate', false);
                $('#chkAllCreate').prop('checked', false);
            }

            if ($('.chk-edit:checked').length === $('#tblFunction tbody tr .chk-edit').length) {
                $('#chkAllEdit').prop('indeterminate', false);
                $('#chkAllEdit').prop('checked', true);
            } else if ($('.chk-edit:checked').length !== 0) {
                $('#chkAllEdit').prop('indeterminate', true);
                $('#chkAllEdit').prop('checked', false);
            } else {
                $('#chkAllEdit').prop('indeterminate', false);
                $('#chkAllEdit').prop('checked', false);
            }

            if ($('.chk-delete:checked').length === $('#tblFunction tbody tr .chk-delete').length) {
                $('#chkAllDelete').prop('indeterminate', false);
                $('#chkAllDelete').prop('checked', true);
            } else if ($('.chk-delete:checked').length !== 0) {
                $('#chkAllDelete').prop('indeterminate', true);
                $('#chkAllDelete').prop('checked', false);
            } else {
                $('#chkAllDelete').prop('indeterminate', false);
                $('#chkAllDelete').prop('checked', false);
            }

            $.each(functions, function (i, item) {
                if (item.ParentId === null) {
                    if ($(`.chk-read-item-${item.Id}:checked`).length === $(`.chk-read-item-${item.Id}`).length) {
                        $(`.chk-read-parent-${item.Id}`).prop('indeterminate', false);
                        $(`.chk-read-parent-${item.Id}`).prop('checked', true);
                    } else if ($(`.chk-read-item-${item.Id}:checked`).length !== 0) {
                        $(`.chk-read-parent-${item.Id}`).prop('indeterminate', true);
                        $(`.chk-read-parent-${item.Id}`).prop('checked', false);
                    } else {
                        $(`.chk-read-parent-${item.Id}`).prop('indeterminate', false);
                        $(`.chk-read-parent-${item.Id}`).prop('checked', false);
                    }

                    if ($(`.chk-create-item-${item.Id}:checked`).length === $(`.chk-create-item-${item.Id}`).length) {
                        $(`.chk-create-parent-${item.Id}`).prop('indeterminate', false);
                        $(`.chk-create-parent-${item.Id}`).prop('checked', true);
                    } else if ($(`.chk-create-item-${item.Id}:checked`).length !== 0) {
                        $(`.chk-create-parent-${item.Id}`).prop('indeterminate', true);
                        $(`.chk-create-parent-${item.Id}`).prop('checked', false);
                    } else {
                        $(`.chk-create-parent-${item.Id}`).prop('indeterminate', false);
                        $(`.chk-create-parent-${item.Id}`).prop('checked', false);
                    }

                    if ($(`.chk-edit-item-${item.Id}:checked`).length === $(`.chk-edit-item-${item.Id}`).length) {
                        $(`.chk-edit-parent-${item.Id}`).prop('indeterminate', false);
                        $(`.chk-edit-parent-${item.Id}`).prop('checked', true);
                    } else if ($(`.chk-edit-item-${item.Id}:checked`).length !== 0) {
                        $(`.chk-edit-parent-${item.Id}`).prop('indeterminate', true);
                        $(`.chk-edit-parent-${item.Id}`).prop('checked', false);
                    } else {
                        $(`.chk-edit-parent-${item.Id}`).prop('indeterminate', false);
                        $(`.chk-edit-parent-${item.Id}`).prop('checked', false);
                    }

                    if ($(`.chk-delete-item-${item.Id}:checked`).length === $(`.chk-delete-item-${item.Id}`).length) {
                        $(`.chk-delete-parent-${item.Id}`).prop('indeterminate', false);
                        $(`.chk-delete-parent-${item.Id}`).prop('checked', true);
                    } else if ($(`.chk-delete-item-${item.Id}:checked`).length !== 0) {
                        $(`.chk-delete-parent-${item.Id}`).prop('indeterminate', true);
                        $(`.chk-delete-parent-${item.Id}`).prop('checked', false);
                    } else {
                        $(`.chk-delete-parent-${item.Id}`).prop('indeterminate', false);
                        $(`.chk-delete-parent-${item.Id}`).prop('checked', false);
                    }
                }
            });
        });
    };

    var validateForm = function () {
        $('#roleForm').validate({
            errorClass: 'text-danger',
            rules: {
                name: {
                    required: true,
                    maxlength: 50
                },
                description: {
                    required: true,
                    maxlength: 250
                }
            },
            messages: {
                name: {
                    required: 'Hãy nhập tên vai trò',
                    maxlength: 'Tên vai trò không vượt quá 50 ký tự'
                },
                description: {
                    required: 'Hãy nhập mô tả',
                    maxlength: 'Mô tả không vượt quá 250 ký tự'
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
        core.callAjax('/Admin/Role/GetById', 'GET', { id }, 'json', function (res) {
            $('#txtId').val(res.Id);
            $('#txtName').val(res.Name);
            $('#txtDescription').val(res.Description);

            callBack();
        });
    };

    var resetForm = function () {
        $('#txtId').val(null);
        $('#txtName').val('');
        $('#txtDescription').val('');

        $("#roleForm").validate().resetForm();
        $('input, textarea, select, file, number').removeClass('is-valid');
        $('input, textarea, select, file, number').removeClass('is-invalid');
    };

    var loadData = function (isPageChanged) {
        core.callAjax('/Admin/Role/GetAllPaging', 'GET', {
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
                    Description: item.Description,
                    isAdmin: item.Name === 'Admin' ? 'none' : 'block'
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
