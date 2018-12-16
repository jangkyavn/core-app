"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/coreHub").build();

connection.on("ReceiveMessage", function (res) {
    core.notify(`Có một thông báo từ <strong>${res.FullName}</strong>`, 'success');
    $('#announcementContent').html('');
    BaseController.loadData();
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
