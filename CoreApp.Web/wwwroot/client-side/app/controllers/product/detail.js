'use strict';

var ProductDetail = function () {
    this.initialize = function () {
        loadReviewList();
        registerEvents();
        validateReviewForm();
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

        $('#btnReview').on('click', function () {
            if ($('#reviewForm').valid()) {
                var form = $('#reviewForm');
                var token = $('input[name="__RequestVerificationToken"]', form).val();

                var data = {
                    Id: parseInt($('#hidReviewId').val()),
                    ProductId: parseInt($('#hidProductId').val()),
                    Rating: parseInt($('input[name=rating]:checked').val()),
                    Content: $('textarea[name=content]').val(),
                    DateCreated: $('#hidDateCreated').val()
                };

                core.callAjax('/Review/SaveChange', 'POST', {
                    __RequestVerificationToken: token,
                    reviewViewModel: data
                }, 'json', function (res) {
                    if (res) {
                        core.notify('Đánh giá thành công.', 'success');
                        loadReviewList();
                    } else {
                        core.notify('Cập nhật đánh giá thành công.', 'success');
                        loadReviewList();
                    }
                });
            }
        });

        $('body').on('click', '.btn-edit-review', function (e) {
            e.preventDefault();

            $('input[name=rating]').prop('disabled', false);
            $('#txtContent').prop('readonly', false);
            $('#btnReview').show();
        });

        $('body').on('click', '.btn-delete-review', function (e) {
            e.preventDefault();
            var id = $('#hidReviewId').val();

            var result = confirm('Bạn có chắc chắn muốn xóa review không?');
            if (result) {
                core.callAjax('/Review/Delete', 'POST', { id }, 'json', function (res) {
                    if (res) {
                        core.notify('Xóa đánh giá thành công.', 'success');
                        loadReviewList();

                        $('#hidReviewId').val(0);
                        $('#hidDateCreated').val('');
                        $('input[name=rating]').prop('disabled', false);
                        $('input[name=rating]').prop('checked', false);
                        $('#txtContent').prop('readonly', false);
                        $('#txtContent').val('');
                        $('#btnReview').show();
                    } else {
                        core.notify(resources.has_error, 'error');
                    }
                });
            }
        });
    };

    var validateReviewForm = function () {
        $('#reviewForm').validate({
            errorClass: 'text-danger',
            rules: {
                rating: {
                    required: true
                },
                content: {
                    required: true,
                    maxlength: 500
                }
            },
            messages: {
                rating: {
                    required: 'Vui lòng bình chọn sản phẩm.'
                },
                content: {
                    required: 'Vui lòng nhập nội dung nhận xét.',
                    maxlength: 'Vui lòng nhập nội dung không quá 500 ký tự.'
                }
            },
            errorPlacement: function (error, element) {
                if (element.attr("name") === 'rating') {
                    error.appendTo('#errorRating');
                    return;
                } else {
                    error.insertAfter(element);
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

    var checkUniqueReview = function (productId) {
        core.callAjax('/Review/CheckExistReview', 'GET', { productId }, 'json', function (res) {
            if (res.Status) {
                var data = res.Data;
                $('#hidReviewId').val(data.Id);
                $('#hidDateCreated').val(data.DateCreated);

                $('input[name=rating]').prop('disabled', true);
                $('input[name=rating]').each(function () {
                    if (parseInt($(this).val()) === data.Rating) {
                        $(this).prop('checked', true);
                        return;
                    }
                });
                $('#txtContent').val(data.Content);
                $('#txtContent').prop('readonly', true);
                $('#btnReview').hide();
                $(`.review-setting-${data.Id}`).show();
                $(`.lbl-fullname-${data.Id}`).html('<strong>Tôi</strong>');
            } else {
                $('#hidReviewId').val(0);
                $('#hidDateCreated').val('');

                $('input[name=rating]').prop('disabled', false);
                $('input[name=rating]').prop('checked', false);
                $('#txtContent').val('');
                $('#txtContent').prop('readonly', false);
                $('#btnReview').show();
            }
        });
    };

    var loadReviewList = function () {
        var productId = $('#hidProductId').val();

        core.callAjax('/Review/GetAllByProductId', 'GET', { productId }, 'json', function (res) {
            var render = '';
            var template = $('#templateReviewListItem').html();

            if (res.length > 0) {
                $.each(res, function (i, item) {
                    var renderRating = '';

                    for (i = 1; i <= 5; i++) {
                        if (i <= item.Rating) {
                            renderRating += '<i class="fa fa-star"></i>';
                        } else {
                            renderRating += '<i class="fa fa-star-o"></i>';
                        }
                    }

                    render += Mustache.render(template,
                        {
                            Id: item.Id,
                            FullName: item.AppUser.FullName,
                            Rating: renderRating,
                            Content: item.Content,
                            DateCreated: core.dateFormatJson(item.DateCreated)
                        });
                });

                checkUniqueReview(productId);

                $('#reviewListContent').html(render);
            } else {
                $('#reviewListContent').html('<div class="alert alert-warning">Chưa có đánh giá nào.</div>');
            }
        });
    };
};