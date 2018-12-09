'use strict';

var ProductDetail = function () {
    this.initialize = function () {
        registerEvents();
    };

    var registerEvents = function () {
        $('#btnAddToCart').on('click', function (e) {
            e.preventDefault();
            var id = parseInt($(this).data('id'));
            var colorId = parseInt($('#ddlColorId').val());
            var sizeId = parseInt($('#ddlSizeId').val());

            core.callAjax('/Cart/AddToCart', 'POST', {
                productId: id,
                quantity: parseInt($('#txtQuantity').val()),
                color: colorId,
                size: sizeId
            }, 'json', function () {
                core.notify(resources.add_to_cart_success, 'success');
                BaseController.loadHeaderCart();
            });
        });
    };
};