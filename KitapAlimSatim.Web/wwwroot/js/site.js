// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var ajaxState = false;
var saveButtons;
var ajaxSuccess = false;
var jsonData;

$(document).ready(function () {
    // profil sayfası
    var navItems = $('.admin-menu li > a');
    var navListItems = $('.admin-menu li');
    var allWells = $('.admin-content');
    var allWellsExceptFirst = $('.admin-content:not(:first)');
    saveButtons = $('.form-save button');

    allWellsExceptFirst.hide();
    navItems.click(function (e) {
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
    saveButtons.click(function () {
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

// 
function buildHtmlTable(myList) {
    var table = $('<table/>'),
        thead = $('<thead/>'),
        tbody = $('<tbody/>');
    var columns = addAllColumnHeaders(myList, thead);

    table.attr('class', 'table table-striped');

    for (var i = 0; i < myList.length; i++) {
        var row$ = $('<tr/>');
        for (var colIndex = 0; colIndex < columns.length; colIndex++) {
            var cellValue = myList[i][columns[colIndex]];
            if (cellValue == null) cellValue = "";
            row$.append($('<td/>').html(cellValue));
        }
        tbody.append(row$);
    }
    table.append(thead).append(tbody);
    return table;
}

// Adds a header row to the table and returns the set of columns.
// Need to do union of keys from all records as some records may not contain
// all records.
function addAllColumnHeaders(myList, selector) {
    var columnSet = [];
    var headerTr$ = $('<tr/>');

    for (var i = 0; i < myList.length; i++) {
        var rowHash = myList[i];
        for (var key in rowHash) {
            if ($.inArray(key, columnSet) == -1) {
                columnSet.push(key);
                headerTr$.append($('<th/>').html(key));
            }
        }
    }
   selector.append(headerTr$);

    return columnSet;
}

function ajax(req, data = {}, item = null) {
    if (ajaxState != false) ajaxState.abort;
    data.Request = req;
    ajaxState = $.post('/Account/Get', data, function (json) {
        ajaxSuccess = json.success;
        if (json.success) {
            if (data.Action == 'ChangePassword') {
                item.parents('form').find('input').val('');
            }
            $('#' + req).html(buildHtmlTable(json.data));
        }
    }, 'json').always(function (response) {
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