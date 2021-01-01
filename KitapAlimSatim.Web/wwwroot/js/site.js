// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var ajaxState = false;
var saveButtons;
var ajaxSuccess = false;
var jsonData;
var globalColumns = ["CreatedAt", "UpdatedAt"];
var _localizer = [];
//


$(document).ready(function () {
    // profil sayfası
    var allWellsExceptFirst = $('.admin-content:not(:first)');
    saveButtons = $('.form-save button');
    allWellsExceptFirst.hide();
    listenAgain();
    // login sayfası
    $('.login-info-box').fadeOut();
    $('.login-show').addClass('show-log-panel');
    if (typeof form === undefined) var form = '';
    if (form == 'Register') {
        $('#label-login').click();
    }
    // ürün sayfası
    $('#addCart').click(function () {
        var cname = 'ProductCart',
            cookie = getCookie(cname);

        if (cookie == "") {
            cookie = [];
        } else {
            cookie = JSON.parse(decodeURIComponent(cookie));
        }
        if (!cookie.includes(productId)) cookie.push(productId);
        setCookie(cname, encodeURIComponent(JSON.stringify(cookie)), 7);

        // sepette gösterme
        $('#productCount').html(` (${cookie.length})`);
    });
    // sepete ekleme
    $('.remove-product').click(function () {
        var self = $(this),
            pid = self.data('pid'),
            cname = 'ProductCart',
            cookie = getCookie(cname);

        if (cookie == "") {
            cookie = [];
        } else {
            cookie = JSON.parse(decodeURIComponent(cookie));
        }
        var newArray = [];
        for (var ix in cookie) {
            var val = cookie[ix];
            if (val != pid) newArray.push(val);
        }
        setCookie(cname, encodeURIComponent(JSON.stringify(newArray)), 7);

        // sepette gösterme
        $('#productCount').html(` (${newArray.length})`);
        $('#productCountTitle').html(newArray.length);
        // kaldırma
        self.parents('.row:eq(0)').remove();
        // toplam tutarı değiştirme
        subtotal -= parseFloat(self.data('price'));
        $('#subTotal').html(`${Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(subtotal).replace('₺', '')} TL`);
    });
    // js içinde localizer erişimi
    for (var ix in localizer) {
        var item = localizer[ix];
        _localizer[item.Name] = item.Value;
    }
});


// cookie ekleyen fonksiyon
function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}
// cookie getiren fonksiyon
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

// profil sayfası için ajax çağrılarını gerçekleştiren fonksiyon
function ajax(req, data = {}, item = null) {
    if (ajaxState != false) ajaxState.abort;
    data.Request = req;
    ajaxState = $.post('/Account/Get', data, function (json) {
        ajaxSuccess = json.success;
        if (json.success) {
            // şifre değiştirme
            if (data.Action == 'ChangePassword') {
                item.parents('form').find('input').val('');
            } else if (typeof data.Action === "undefined") {
                // tablolar
                var table = $('<table/>'), thead, tbody = $('<tbody/>'), cnt = $('#' + req);
                if (typeof qrXc === "function") {
                    table = qrXc(req, json);
                } else {
                    switch (data.Request) {
                        case "siparislerim":
                            // headerlar
                            var columns = ["#", "Products", "SubTotal"];
                            columns = columns.concat(globalColumns);
                            thead = getTableHead(columns);
                            // satırlar
                            for (var ix in json.data) {
                                var model = json.data[ix];
                                var products = "";

                                for (ix in model.OrderItemList) {
                                    var orderModel = model.OrderItemList[ix],
                                        product;

                                    product = `<a href="/Product/${orderModel.Id}">${orderModel.Book.Name} (${orderModel.Price} TL)</a>`;
                                    products += product + "<br />";
                                }

                                var values = [
                                    model.Id,
                                    products,
                                    model.SubTotal,
                                    model.CreatedAt,
                                    model.UpdatedAt
                                ];
                                tbody.append(getTableRow(values));
                            }
                            break;
                        case "satislarim":
                            // headerlar
                            var columns = ["#", "Name", "Price", "Stock", "IsActive"];
                            columns = columns.concat(globalColumns);
                            columns.push("Action");
                            thead = getTableHead(columns);
                            // satırlar
                            for (var ix in json.data) {
                                var model = json.data[ix];

                                var bookName = `<a href="/Product/${model.product.Id}">${model.book.Name}</a>`;
                                action = `<div class='btn btn-xs btn-primary ajax-action' data-action='active'>${_localizer["Active"]}</div><div class='btn btn-xs btn-danger ajax-action' data-action='delete'>${_localizer["Delete"]}</div>`;

                                var values = [
                                    model.product.Id,
                                    bookName,
                                    model.product.Price + " TL",
                                    model.product.Stock,
                                    model.product.IsActive,
                                    model.product.CreatedAt,
                                    model.product.UpdatedAt,
                                    action
                                ];
                                tbody.append(getTableRow(values));
                            }
                            break;
                        default:
                    }
                    table.append(thead).append(tbody);
                }
                if (!table.is('table')) table.find('table').attr('class', 'table table-striped');
                else table.attr('class', 'table table-striped');
                cnt.html(table);
                if (json.data.length == 0) cnt.html(_localizer["NoRecordsFound"]);
            }
            listenAgain();
        }
    }, 'json').always(function (response) {
        // ajax sorgusu bittiğinde çalışan fonksiyon
        saveButtons.attr('disabled', false);
        setTimeout(function () {
            if (item != null) {
                if (ajaxSuccess) {
                    item = item.find('.text-success');
                }
                else item = item.find('.text-danger');
                item.show().delay(2000).fadeOut().find('.error').html(': ' + response.error);
            }
        }, 100);
        console.log(response);
    });
    ajaxSuccess = false;
}

// verilen diziyi tablo headerı olarak döndüren fonksiyon
function getTableHead(columns) {
    // dil bilgisi gelmediyse null döndür
    if (typeof localizer === "undefined") return null;

    var thead = $('<thead/>'), tr = $('<tr/>'), th, title;
    for (var ix in columns) {
        th = $('<th/>');
        title = _localizer[columns[ix]];
        if (title === undefined) title = columns[ix];
        th.html(title)
        tr.append(th);
    }
    return thead.append(tr);
}

// verilen diziyi tablo satırı olarak döndüren fonksiyon
function getTableRow(values) {
    var tr = $('<tr/>'), td, val;
    for (var ix in values) {
        td = $('<td/>');
        // true veya false ise 
        val = values[ix];
        if (val === true) val = _localizer["Yes"];
        else if (val === false) val = _localizer["No"];
        //
        td.html(val);
        tr.append(td);
    }
    return tr;
}

// elementler güncellediğinde eventlerin çalışması için tekrar çağırılan fonksiyon
function listenAgain() {
    var navItems = $('.admin-menu li > a, .admin-menu-title');
    var navListItems = $('.admin-menu li');
    var allWells = $('.admin-content');
    var allWellsExceptFirst = $('.admin-content:not(:first)');
    var ajaxActions = $('.ajax-action');

    navItems.unbind().click(function (e) {
        e.preventDefault();
        navListItems.removeClass('active');
        $(this).closest('li').addClass('active');

        allWells.hide();
        var targetId = $(this).attr('data-target-id');
        target = $('#' + targetId).show();
        if (target.data('loading') != false) {
            target.html(`${loadingDisplay}..`);
            // ajax
            ajax(targetId);
        }
    });
    saveButtons.unbind().click(function () {
        var self = $(this), data = { Action: self.data('action') },
            form = self.parents('form'),
            formSave = form.find('.form-save'),
            inputs = form.find('textarea,input');
        saveButtons.attr('disabled', true);
        inputs.each(function () {
            var self = $(this);
            data[self.attr('name')] = self.val();
        });
        ajax('ayarlar', data, formSave);
    });
    // actions
    ajaxActions.unbind().click(function () {
        var self = $(this), req = self.data('req'),
            cnt = self.parents('.admin-content'), table = cnt.attr('id'),
            actionId = self.parents('tr').find('td:eq(0)').text();

        var data = {
            Request: req,
            Id: actionId,
            Table: table
        };
        self.attr('disabled', true);
        $.post('/Account/Get', data).always(function () {
            ajax(table);
        });
    });
}

// oturum açma sayfasındaki animasyonu sağlayan fonksiyon
$('.login-reg-panel input[type="radio"]').on('change', function () {
    if ($('#log-login-show').is(':checked')) {
        $('.register-info-box').fadeOut();
        $('.login-info-box').fadeIn();

        $('.white-panel').addClass('right-log');
        $('.register-show').addClass('show-log-panel');
        $('.login-show').removeClass('show-log-panel');

    }
    else if ($('#log-reg-show').is(':checked')) {
        $('.register-info-box').fadeIn();
        $('.login-info-box').fadeOut();

        $('.white-panel').removeClass('right-log');

        $('.login-show').addClass('show-log-panel');
        $('.register-show').removeClass('show-log-panel');
    }
});