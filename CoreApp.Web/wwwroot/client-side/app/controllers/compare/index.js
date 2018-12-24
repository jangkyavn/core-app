'use strict';

var CompareController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    };

    var registerEvents = function () {
        $('body').on('click', '.btn-delete-compare', function () {
            var productId = $(this).data('id');

            core.callAjax('/Compare/RemoveFromCompare', 'POST', { productId }, 'json', function (res) {
                if (res) {
                    core.notify(resources.delete_success, 'success');
                    loadData();
                }
            });
        });
    };

    var loadData = function () {
        var renderProductImage = '<td class="compare-label">Hình ảnh</td>';
        var renderProductName = '<td class="compare-label">Tên sản phẩm</td>';
        var renderReviews = '<td class="compare-label">Đánh giá</td>';
        var renderPrices = '<td class="compare-label">Giá bán</td>';
        var renderDescriptions = '<td class="compare-label">Mô tả</td>';
        var renderStatus = '<td class="compare-label">Tình trạng</td>';
        var renderActions = '<td class="compare-label">Hành động</td>';

        core.callAjax('/Compare/GetAll', 'GET', null, 'json', function (res) {
            $.each(res, function (i, item) {
                var renderRating = '<td><div class="rating">';
                for (i = 1; i <= 5; i++) {
                    if (i <= item.RatingAverage) {
                        renderRating += '<i class="fa fa-star"></i>';
                    } else {
                        renderRating += '<i class="fa fa-star-o"></i>';
                    }

                    if (i === 5) {
                        renderRating += `&nbsp; <span>(${item.RatingTotal} đánh giá)</span>`;
                    }
                }
                renderRating += '</div></td>';

                var url = `/${item.Product.SeoAlias}-p.${item.Product.Id}.html`;
                renderProductImage += `
                    <td class="compare-pro">
                        <a href="${url}">
                            <img src="/uploaded/images/products/${item.Product.Image}" alt="${item.Product.Name}" width="260">
                        </a>
                    </td>
                `;

                renderProductName += `
                    <td><a href="${url}">${item.Product.Name}</a></td>
                `;

                renderReviews += renderRating;

                renderPrices += `
                    <td class="price">${item.Product.PromotionPrice === null ? core.formatNumber(item.Product.Price, 0) : core.formatNumber(item.Product.PromotionPrice, 0)}đ</td>
                `;

                renderDescriptions += `<td>${item.Product.Description}</td>`;

                renderStatus += `<td class="${item.Product.Status ? 'instock' : 'outofstock'}">${item.Product.Status ? 'Còn hàng' : 'Hết hàng'}</td>`;

                renderActions += `
                    <td class="action">
                        <button class="button button-sm btn-delete-compare" data-id="${item.Product.Id}"><i class="fa fa-close"></i></button>
                    </td>
                `;
            });

            $('#trProductImages').html(renderProductImage);
            $('#trProductNames').html(renderProductName);
            $('#trReviews').html(renderReviews);
            $('#trPrices').html(renderPrices);
            $('#trDescriptions').html(renderDescriptions);
            $('#trStatus').html(renderStatus);
            $('#trActions').html(renderActions);
        });
    };
};