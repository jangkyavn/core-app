'use strict';

var BaseController = {
    initialize: function () {
        var href = window.location.href;

        $('.nav-item a').each(function (i, item) {
            if (href.indexOf($(this).attr('href')) >= 0) {
                $(this).parent('li').removeClass('nav-item');
                $(this).parent('li').addClass('active');
            }
        });

        $('.menu-content a.menu-item').each(function (e, i) {
            if (href.indexOf($(this).attr('href')) >= 0) {
                $(this).parent('li').addClass('active');
                $(this).closest('li.nav-item').addClass('open');
            }
        });

        BaseController.loadData();

        setInterval(function () {
            BaseController.loadData();
        }, 180000);

        BaseController.registerEvents();
    },
    registerEvents: function () {
        $('#announcementContent').scroll(function () {
            if (Math.round($(this).scrollTop() + $(this).innerHeight(), 10) >= Math.round($(this)[0].scrollHeight, 10)) {
                core.configs.pageIndex += 1;
                BaseController.loadData();
            }
        });

        $('body').on('click', '.btn-detail-announcement', function (e) {
            var id = $(this).data('id');

            core.callAjax('/Admin/Announcement/MaskAsRead', 'POST', { id }, 'json', function (res) {
                if (res) {
                    window.location.href = '/Admin/Announcement/Index';
                }
            });
        });
    },
    loadData: function () {
        core.callAjax('/Admin/Announcement/GetAllPaging', 'GET', {
            page: core.configs.pageIndex,
            pageSize: core.configs.pageSize
        }, 'json', function (res) {
            var render = "";
            var template = $('#announcementTemplate').html();

            if (res.RowCount > 0) {
                $.each(res.Results, function (i, item) {
                    render += Mustache.render(template, {
                        Id: item.Id,
                        Title: item.Title,
                        Content: item.Content,
                        FullName: item.FullName,
                        Avatar: item.Avatar,
                        Period: core.getPeriod(item.DateCreated),
                        HasRead: item.HasRead === true ? 'text-muted' : 'text-primary',
                        HasReadName: item.HasRead === true ? '' : 'text-success'
                    });
                });

                var arrUnread = res.Results.map(function (i, item) {
                    return item.HasRead === false;
                });

                $('#lblAnnouncementTotal').show();
                $('#lblAnnouncementTotal').text(arrUnread.length);
                $('#announcementContent').append(render);
                $('#announcementFooter').show();
            } else {
                $('#lblAnnouncementTotal').hide();
                $('#announcementContent').html('<div style="text-align: center; margin: 1rem 0 1rem 0;">Không có thông báo</div>');
                $('#announcementFooter').hide();
            }
        });
    }
};

BaseController.initialize();
