'use strict';

var FunctionController = function () {
    this.initialize = function () {
        validateForm();
        loadParents();
        loadIcons();
        initialTree();
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnAddNew').on('click', function () {
            $('#modalTitle').text('Thêm mới chức năng');
            $('#hidIsAddNew').val(true);
            resetForm();
            $('#addEditModal').modal({
                show: true,
                backdrop: 'static'
            });
        });

        $('#btnSave').on('click', function () {
            if ($('#functionForm').valid()) {
                var data = {
                    Id: $('#txtId').val(),
                    Name: $('#txtName').val(),
                    ParentId: $('#ddlParent').val(),
                    URL: $('#txtURL').val(),
                    IconCss: $('#ddlIcon').val(),
                    SortOrder: $('#hidSortOrder').val(),
                    Status: $('#chkStatus').prop('checked') ? 1 : 0,
                    IsAddNew: $('#hidIsAddNew').val()
                };

                core.callAjax('/Admin/Function/SaveEntity', 'POST', data, 'json', function (res) {
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

        $('#txtKeyword').on('keypress', function (e) {
            if (e.which === 13) {
                $('#btnSearch').click();
            }
        });

        $('#btnSearch').on('click', function () {
            var keyword = $('#txtKeyword').val();
            $('#treeContainer').jstree(true).search(keyword);
        });

        $('#btnRefresh').on('click', function () {
            $('#txtKeyword').val('');
            $('#treeContainer').jstree(true).search('');
        });

        $('#btnExpandTree').on('click', function () {
            $("#treeContainer").jstree().open_all(null, 200);
        });

        $('#btnCollapseTree').on('click', function () {
            $("#treeContainer").jstree().close_all(null, 200);
        });

        $(document).on('dnd_stop.vakata', function () {
            setTimeout(function () {
                var treeArr = $('#treeContainer').jstree(true).get_json();
                var jsonModel = JSON.stringify(treeArr);
                core.callAjax('/Admin/Function/UpdateTreeNodePosition', 'POST', { jsonModel }, 'json', function (res) {
                    if (res) {
                        core.notify(message.change_success, 'success');
                    }
                });
            }, 200);
        });
    };

    var getById = function (id) {
        core.callAjax('/Admin/Function/GetById', 'GET', { id }, 'json', function (res) {
            $('#hidSortOrder').val(res.SortOrder);

            $('#txtId').val(res.Id);
            $('#txtName').val(res.Name);
            $('#ddlParent').val(res.ParentId);
            $('#txtURL').val(res.URL);
            $('#ddlIcon').val(res.IconCss);
            $('#chkStatus').prop('checked', res.Status);

            console.log(res);
        });
    };

    var validateForm = function () {
        $('#functionForm').validate({
            errorClass: 'text-danger',
            rules: {
                id: {
                    required: true,
                    maxlength: 50
                },
                name: {
                    required: true,
                    maxlength: 200
                },
                url: {
                    maxlength: 200
                }
            },
            messages: {
                id: {
                    required: 'Vui lòng nhập mã chức năng',
                    maxlength: 'Mã chức năng không vượt quá 50 ký tự'
                },
                name: {
                    required: 'Vui lòng nhập tên chức năng',
                    maxlength: 'Tên chức năng không vượt quá 200 ký tự'
                },
                url: {
                    maxlength: 'Đường dẫn không vượt quá 200 ký tự'
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
        $('#hidSortOrder').val(0);

        $('#txtId').val('');
        $('#txtName').val('');
        $('#ddlParent').val('');
        $('#txtURL').val('');
        $('#ddlIcon').val('');
        $('#chkStatus').prop('checked', false);

        $("#functionForm").validate().resetForm();
        $('input, textarea, select, file, number').removeClass('is-valid');
        $('input, textarea, select, file, number').removeClass('is-invalid');
    };

    var loadParents = function () {
        core.callAjax('/Admin/Function/GetAllParent', 'GET', null, 'json', function (res) {
            var render = '<option value="">Chọn chức năng cha</option>';

            $.each(res, function (index, item) {
                render += `<option value="${item.Id}">${item.Name}</option>`;
            });

            $('#ddlParent').html(render);
        });
    };

    var loadIcons = function () {
        core.callAjax('/Admin/Function/GetIcons', 'GET', null, 'json', function (res) {
            var data = [];
            $.each(res, function (i, item) {
                data.push({
                    id: item.Value,
                    text: item.Name
                });
            });

            function formatState(state) {
                if (!state.id) {
                    return state.text;
                }
                var $state = $(
                    '<span><i class="' + state.text + '"></i>' + state.text + '</span>'
                );
                return $state;
            }

            $("#ddlIcon").select2({
                data: data,
                width: '100%',
                dropdownParent: $('#addEditModal'),
                templateResult: formatState,
                templateSelection: formatState
            });

        });
    };
    
    var confirmDelete = function (id) {
        core.confirm(message.confirm_delete, function () {
            core.callAjax('/Admin/Function/Delete', 'POST', { id }, 'json', function (res) {
                core.notify(message.delete_success, 'success');
                refreshTree();
            });
        });
    };

    var refreshTree = function () {
        loadData(function (res) {
            $('#treeContainer').jstree(true).settings.core.data = res;
            $('#treeContainer').jstree(true).refresh();
        });
    };

    var initialTree = function () {
        loadData(function (res) {
            $('#treeContainer').jstree({
                'core': {
                    'multiple': false,
                    'data': res,
                    "check_callback": true
                },
                'plugins': ['checkbox', 'contextmenu', 'dnd', 'search', 'types'],
                'types': {
                    '#': {
                        'max_depth': 2
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
                        $('#hidIsAddNew').val(false);
                        $('#modalTitle').text('Sửa thông tin chức năng');
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
                        $('#hidIsAddNew').val(false);
                        $('#modalTitle').text('Sửa thông tin chức năng');
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

    var loadData = function (callBack) {
        core.callAjax('/Admin/Function/GetAll', 'GET', null, 'json', function (res) {
            var data = [];

            $.each(res, function (index, item) {
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
