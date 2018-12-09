'use strict';

var CartController = function () {
    var cachedObj = {
        colors: [],
        sizes: []
    };

    this.initialize = function () {
        loadColors(function () {
            loadSizes(function () {
                loadData();
            });
        });
        
        registerEvents();
    };

    var registerEvents = function () {
        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var productId = $(this).data('id');

            core.callAjax('/Cart/RemoveFromCart', 'POST', { productId }, 'json', function () {
                core.notify(resources.delete_to_item_cart_success, 'success');
                BaseController.loadHeaderCart();
                loadData();
            });
        });

        $('body').on('keyup', '.txtQuantity', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var q = $(this).val();
            if (q > 0) {
                core.callAjax('/Cart/UpdateCart', 'POST', {
                    productId: id,
                    quantity: q
                }, 'json', function () {
                    core.notify(resources.update_to_cart_success, 'success');
                    BaseController.loadHeaderCart();
                    loadData();
                });
            } else {
                core.notify('Không hợp lệ', 'error');
            }
        });

        $('body').on('change', '.ddlColorId', function (e) {
            e.preventDefault();
            var id = parseInt($(this).closest('tr').data('id'));
            var colorId = $(this).val();
            var q = $(this).closest('tr').find('.txtQuantity').first().val();
            var sizeId = $(this).closest('tr').find('.ddlSizeId').first().val();

            if (q > 0) {
                core.callAjax('/Cart/UpdateCart', 'POST', {
                    productId: id,
                    quantity: q,
                    color: colorId,
                    size: sizeId
                }, 'json', function () {
                    core.notify(resources.update_to_cart_success, 'success');
                    BaseController.loadHeaderCart();
                    loadData();
                });
            } else {
                core.notify('Your quantity is invalid', 'error');
            }
        });

        $('body').on('change', '.ddlSizeId', function (e) {
            e.preventDefault();
            var id = parseInt($(this).closest('tr').data('id'));
            var sizeId = $(this).val();
            var q = parseInt($(this).closest('tr').find('.txtQuantity').first().val());
            var colorId = parseInt($(this).closest('tr').find('.ddlColorId').first().val());

            if (q > 0) {
                core.callAjax('/Cart/UpdateCart', 'POST', {
                    productId: id,
                    quantity: q,
                    color: colorId,
                    size: sizeId
                }, 'json', function () {
                    core.notify(resources.update_to_cart_success, 'success');
                    BaseController.loadHeaderCart();
                    loadData();
                });
            } else {
                core.notify('Your quantity is invalid', 'error');
            }
        });

        $('#btnClearAll').on('click', function (e) {
            e.preventDefault();

            var result = confirm('Bạn có chắc chắn muốn xóa tất cả sản phẩm trong giỏ hàng không?');
            if (result) {
                core.callAjax('/Cart/ClearCart', 'POST', null, 'json', function () {
                    core.notify('Xóa tất cả sản phẩm trong giỏ hàng thành công!', 'success');
                    BaseController.loadHeaderCart();
                    loadData();
                });
            }
        });
    };

    var loadColors = function (callback) {
        core.callAjax('/Cart/GetColors', 'GET', null, 'json', function (res) {
            cachedObj.colors = res;

            callback();
        });
    };

    var loadSizes = function (callback) {
        return core.callAjax('/Cart/GetSizes', 'GET', null, 'json', function (res) {
            cachedObj.sizes = res;

            callback();
        });
    };

    var getColorOptions = function (selectedId) {
        var colors = "<select class='form-control' id='ddlColorId'><option value='0'></option>";
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
        var sizes = "<select class='form-control' id='ddlSizeId'> <option value='0'></option>";
        $.each(cachedObj.sizes, function (i, size) {
            if (selectedId === size.Id)
                sizes += '<option value="' + size.Id + '" selected="select">' + size.Name + '</option>';
            else
                sizes += '<option value="' + size.Id + '">' + size.Name + '</option>';
        });
        sizes += "</select>";
        return sizes;
    };

    var loadData = function () {
        return core.callAjax('/Cart/GetCart', 'GET', null, 'json', function (res) {
            var template = $('#cartItemTemplate').html();
            var render = "";
            var totalAmount = 0;
            $.each(res, function (i, item) {
                console.log(cachedObj.colors);
                console.log(cachedObj.sizes);
                render += Mustache.render(template,
                    {
                        ProductId: item.Product.Id,
                        ProductName: item.Product.Name,
                        UrlImage: `/uploaded/images/products/${item.Product.Image}`,
                        Price: core.formatNumber(item.Price, 0),
                        Quantity: item.Quantity,
                        Colors: getColorOptions(item.Color === null ? 0 : item.Color.Id),
                        Sizes: getSizeOptions(item.Size === null ? "" : item.Size.Id),
                        Amount: core.formatNumber(item.Price * item.Quantity, 0),
                        Url: '/' + item.Product.SeoAlias + "-p." + item.Product.Id + ".html"
                    });
                totalAmount += item.Price * item.Quantity;
            });

            $('#lblTotalAmount').text(core.formatNumber(totalAmount, 0));
            if (render !== "") {
                $('#lblEmptyCart').hide();
                $('.table-responsive').show();
                $('#cartsContent').html(render);
                $('.checkout-btn').show();
                $('#btnClearAll').show();
            } else {
                $('#lblEmptyCart').show();
                $('.table-responsive').hide();
                $('.checkout-btn').hide();
                $('#btnClearAll').hide();
            }
        });
    };
};