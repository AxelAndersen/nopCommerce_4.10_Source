//Edit Page js

var titleCat =
    "<p>Alcoholic beverages must be submitted with one of these categories: " +
    "<ul><li><b>Food, Beverages & Tobacco > Beverages > Alcoholic Beverages (499676)</b> , or any of its subcategories </li>" +
    "<li><b> Arts & Entertainment > Hobbies & Creative Arts > Homebrewing & Winemaking Supplies(3577)</b >, or any of its subcategories.</li></ul></p>" +
    "<p>Mobile devices sold with contract must be submitted as: <ul><li><b>Electronics > Communications > Telephony > Mobile Phones (267)</b> for phones. </li> " + "<li><b>Electronics > Computers > Tablet Computers (4745)</b> for tablets.</li></ul></p>" +
    "<p>Gift Cards must be submitted as <b>Arts & Entertainment > Party & Celebration > Gift Giving > Gift Cards & Certificates (53)</b></p>";

var titleColor =
    "<p><b>Color </b>required for all apparel items in feeds that target <b>Brazil, France, Germany, Japan, the UK, and the US</b> as well as all products available in different colors.</p>" +
    "<p><ul><li>Don’t use a number such as 0 2 4 6 8</li>" +
    "<li>Don’t use characters that aren’t alphanumeric such as #fff000</li>" +
    "<li>Don’t use only 1 letter such as R (For Chinese, Japanese, or Korean languages, you can include a single character such as 红)</li>" +
    "<li>Don’t reference the product or image such as “see image”</li>" +
    "<li>Don't combine several color names into 1 word instead of separating them with a / such as RedPinkBlue</li>" +
    "<li>Don’t use a value that isn’t a color such as multicolor, various, variety, men's, women's, or N/A</li>" +
    "</ul></p>";

var titleSize =
    "<p><b>Size </b>required for all <b>apparel items</b> in the <b>Apparel & Accessories > Clothing and Apparel & Accessories > Shoes</b> product categories targeting <b>Brazil, France, Germany, Japan, the UK, and the US</b> as well as all products available in different sizes.</p>" +
    "<p><ul><li>If sizes contain multiple dimensions, condense them into 1 value. For example, '16 / 34 Tall' for neck size 16 inches, sleeve length 34 inches, and “Tall” fit</li>" +
    "<li>If your item is one size fits all or one size fits most, you can use one size, OS, one size fits all, OSFA, one size fits most, or OSFM</li>" +
    "</ul></p>";

var titlePattern =
    "<p><b>Pattern </b>required if relevant for distinguishing different products in a set of variants.</p>";

var publishAll = false;
var feedId = 0;

$(document).ready(function () {
    $('[data-tooltip="tooltip"]').popover();
    HideAllRButtons();
    OnLoadVariantsInit();
    $('#search-category-clear').click(function () {
        $('#GoogleCategory').val('');
        $('#search-category-friendly-name').text('');
        $('#search-category-clear').hide();
        return false;
    });

    $('#IsOnSale').change(function () {
        if ($(this).is(':checked')) {
            $("#salepricediv").show();
            $("#salepricestartdiv").show();
            $("#salepriceenddiv").show();
        }
        else {
            $("#salepricediv").hide();
            $("#salepricestartdiv").hide();
            $("#salepriceenddiv").hide();
        }
    });

    $('input[type=radio][name=nameselectiontype]').change(function () {
        var selectedrbVal = $(this).val();
        $("#NameSelectionType").val(selectedrbVal);
        ShowHideNameRButtons(selectedrbVal);
    });
    $('input[type=radio][name=descriptionselectiontype]').change(function () {
        var selectedrbVal = $(this).val();
        $("#DescriptionSelectionType").val(selectedrbVal);
        ShowHideNameRButtons(selectedrbVal);
    });
    var nameradiobtnVal = $('input[name=nameselectiontype]:checked').val();
    ShowHideNameRButtons(nameradiobtnVal);
    var descradiobtnVal = $('input[name=descriptionselectiontype]:checked').val();
    ShowHideNameRButtons(descradiobtnVal);

    $('#GoogleCategory').hide();
    $('#search-category-name').autocomplete({
        delay: 200,
        minLength: 3,
        scroll: true,
        source: $('#loadVariantsData').data('request-urlasearch'),
        select: function (event, ui) {
            $('#GoogleCategory').val(ui.item.labelValue);
            $('#search-category-friendly-name').text(ui.item.label);
            $('#search-category-clear').show();
            $('#search-category-name').val('');
            return false;
        }
    });

    $('#StockAvailabilityStatusId').change(function () {
        var selectedText = $('#StockAvailabilityStatusId :selected').text();
        $("#stockAvl").html(selectedText);
        if (selectedText === 'In Stock') {
            $('#stockAvl').css('background', '#d6e4e4');
            $('#stockAvl').css('color', 'black');
        }
        else {

            $('#stockAvl').css('background', '#f1f1f1');
            $('#stockAvl').css('color', '#999999');
        }
    });

    $("#change-status-clear").on('click', function () {
        $("#change-status-sel").show();
        $("#change-status-clear").hide();
        $("#changeStockStatus").hide();
    });

    $("#gcat-more").hover(
        function () {
            SetHoverPopup(titleCat, 'gcat-more');
        }, function () {
            $(document).find("div.box").remove();
        }
    );

    $("#spec-color-instr").hover(
        function () {
            SetHoverPopup(titleColor, 'spec-color-instr');
        }, function () {
            $(document).find("div.box").remove();
        }
    );

    $("#spec-size-instr").hover(
        function () {
            SetHoverPopup(titleSize, 'spec-size-instr');
        }, function () {
            $(document).find("div.box").remove();
        }
    );

    $("#spec-pttrn-instr").hover(
        function () {
            SetHoverPopup(titlePattern, 'spec-pttrn-instr');
        }, function () {
            $(document).find("div.box").remove();
        }
    );

    $('#CustomLabel0, #CustomLabel1, #CustomLabel2, #CustomLabel3, #CustomLabel4, #ShippingLabel, #ManufacturerPartNumber, #Gtin, #Color, #Size, #Pattern, #CustomDescription, #CustomName').keyup(function () {
        SetMaxLength(this);
        CheckTextboxCharCount(this);
    });

    $('input[type=radio][name=feedselectiontype]').click(function () {
        var selectedrbVal = $(this).val();
        if (selectedrbVal === 'FeedSelectionCommon') {
            $("#FeedSelectionTypeId").val('1');
            OpenModalForFeedSelection(selectedrbVal);
        }
        else if (selectedrbVal === 'FeedSelectionVariants') {
            $("#FeedSelectionTypeId").val('5');
            OpenModalForFeedSelection(selectedrbVal);
        }
        return false;
    });

    $('#resetbtn').on('click', function () {
        $('#reConfirm').modal('show');
        $('.modal-title').html('Confirmation');
        $('#commonmodalbtn').empty().html('<i class="fa fa-undo"></i>Reset');
        $('.modal-body').html('Are you sure you want to <b>reset</b> this product to its original state ? All changes made will be lost.');
        $('#commonmodalbtn').hide();
        $('#modalpublishbtn').hide();
        $('#modalunpublishbtn').hide();
        $('#modalresetbtn').show();
    });

    $('#publishMain').on('click', function () {
        var feedSelVal = $("#FeedSelectionTypeId").val();
        feedId = $("#Id").val();
        if (feedSelVal !== '0') {
            publishAll = true;
        }
        else {
            publishAll = false;
        }
        $('#reConfirm').modal('show');
        $('.modal-title').html('Confirmation');
        if (feedSelVal !== '0') {
            $('#modalpublishbtn').empty().html('<i class="fa fa-upload"></i>Publish All Variants');
            $('.modal-body').html('Are you sure you want to <b>publish all</b> variants to merchant center.');
        }
        else {
            $('#modalpublishbtn').empty().html('<i class="fa fa-upload"></i>Publish');
            $('.modal-body').html('Are you sure you want to <b>publish</b> this feed to merchant center.');
        }
        $('#commonmodalbtn').hide();
        $('#modalresetbtn').hide();
        $('#modalunpublishbtn').hide();
        $('#modalpublishbtn').show();
    });

    $('#unpublishMain').on('click', function () {
        var feedSelVal = $("#FeedSelectionTypeId").val();
        feedId = $("#Id").val();
        if (feedSelVal !== '0') {
            publishAll = true;
        }
        else {
            publishAll = false;
        }
        $('#reConfirm').modal('show');
        $('.modal-title').html('Confirmation');
        if (feedSelVal !== '0') {
            $('#modalunpublishbtn').empty().html('<i class="fa fa-eye-slash"></i>Unpublish All Variants');
            $('.modal-body').html('Are you sure you want to <b>unpublish all</b> variants from merchant center.');
        }
        else {
            $('#modalunpublishbtn').empty().html('<i class="fa fa-eye-slash"></i>Unpublish');
            $('.modal-body').html('Are you sure you want to <b>unpublish</b> this feed from merchant center.');
        }
        $('#commonmodalbtn').hide();
        $('#modalresetbtn').hide();
        $('#modalpublishbtn').hide();
        $('#modalunpublishbtn').show();
    });
});

function PublishVariant(handle) {
    feedId = $(handle).data('var-id');
    $('#reConfirm').modal('show');
    $('.modal-title').html('Confirmation');
    $('#modalpublishbtn').empty().html('<i class="fa fa-upload"></i>Publish Variant');
    $('.modal-body').html('Are you sure you want to publish this variant to merchant center.');
    $('#commonmodalbtn').hide();
    $('#modalresetbtn').hide();
    $('#modalpublishbtn').show();
    $('#modalunpublishbtn').hide();
}

function UnpublishVariant(handle) {
    feedId = $(handle).data('var-id');
    $('#reConfirm').modal('show');
    $('.modal-title').html('Confirmation');
    $('#modalunpublishbtn').empty().html('<i class="fa fa-eye-slash"></i>Unpublish Variant');
    $('.modal-body').html('Are you sure you want to unpublish this variant from merchant center.');
    $('#commonmodalbtn').hide();
    $('#modalresetbtn').hide();
    $('#modalpublishbtn').hide();
    $('#modalunpublishbtn').show();
}

function OpenModalForFeedSelection(selectionType) {
    $('#commonmodalbtn').show();
    $('#modalresetbtn').hide();
    $('#reConfirm').modal('show');
    $('.modal-title').html('Confirmation');
    $('#commonmodalbtn').text('Change');
    $('#commonmodalbtn').val(selectionType);
    if (selectionType === 'FeedSelectionCommon') {
        $('.modal-body').html('Are you sure you want to shift to <b>Common Parent data feed</b> option. Any changes made will be lost.');
    }
    else if (selectionType === 'FeedSelectionVariants') {
        $('.modal-body').html('Are you sure you want to shift to <b>Variant wise data feed</b> option ?');
    }
}

function SetHoverPopup(title, updateHandle) {
    $('<div/>', {
        class: 'box'
    }).html(title).appendTo("#" + updateHandle);
}

function OnLoadVariantsInit() {
    var feedSelVal = $("#FeedSelectionTypeId").val();
    $.ajax({
        type: "POST",
        url: $('#loadVariantsData').data('request-url'),
        data: JSON.stringify({ productId: $("#ProductId").val(), storeId: $("#StoreId").val() }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.length > 0) {
                if (feedSelVal !== '0') {
                    $('#attributesID').show();
                }
                //$('.header-status-text').hide();

                if (feedSelVal === '5') {
                    //$('.header-save-btns').show();
                    $('.com-parent').hide();
                    $('#fsel-div').show();
                }
                else {
                    $('.com-parent').show();
                }
                //$('.header-status-text-var').hide();
                var str = ``;
                data.forEach(item => {
                    str = str +
                        `<div class="row var-list-row">
                        <div class="col-sm-2 img-res">
                            <img src="${item.ImageUrl}" height="125" width="125">
                        </div>
                        <div class="col-sm-10">
                            <div class="col-sm-12">
                                <div class="col-xs-6 col-sm-2 var-list-head"><span class="var-list-head-span">Sku</span><br />${item.Sku}</div>
                                <div class="col-xs-6 col-sm-2 var-list-head"><span class="var-list-head-span">Price</span><br />${item.Price}</div>

                                <div class="col-xs-6 col-sm-2 var-list-head"><span class="var-list-head-span">Size</span><br />`
                    //if (item.Size === '' && feedSelVal !== '5') {
                    //    str = str + `<input type ="text" name="size" class='var-lst-inp form-control text-box single-line'/> `
                    //}
                    //else {
                    str = str + `${item.Size}`
                    //}

                    str = str + `</div><div class="col-xs-6 col-sm-2 var-list-head"><span class="var-list-head-span">Color</span><br />`
                    //if (item.Color === '' && feedSelVal !== '5') {
                    //    str = str + `<input type ="text" name="color" class='var-lst-inp form-control text-box single-line' /> `
                    //}
                    //else {
                    str = str + `${item.Color}`
                    //}

                    str = str + `</div><div class="col-xs-6 col-sm-2 var-list-head"><span class="var-list-head-span">Pattern</span><br />`
                    //if (item.Pattern === '' && feedSelVal !== '5') {
                    //    str = str + `<input type="text" name="pattern" class='var-lst-inp form-control text-box single-line'/>`
                    //}
                    //else {
                    str = str + `${item.Pattern}`
                    //}

                    str = str + `</div><div class="col-xs-6 col-sm-2 var-list-head">
                                    <span class="var-list-head-span">Stock/Feed</span><br />`
                    if (item.StockStatusId == 1)//FeedAvailability.InStock 
                    {
                        str = str + `<span style="display: inline-block;background:#d6e4e4;color:black;font-size:12px;margin-right:5px;" class="grid-report-item">${item.StockStatus}</span>`
                    }
                    if (item.StockStatusId == 5 || item.StockStatusId == 10)//FeedAvailability.OutOfStock,FeedAvailability.Preorder
                    {
                        str = str + `<span style="display: inline-block; background:#f1f1f1;color:#999999;font-size:12px;margin-right:5px;" class="grid-report-item">${item.StockStatus}</span>`
                    }
                    //str = str + `</div>`
                    //<div class="col-sm-2 var-list-head">
                    //    <span class="var-list-head-span">Feed Status</span><br />`
                    if (item.StatusId == 15)//FeedStatus.Published 
                    {
                        str = str + `<span style="display: inline-block;font-size:12px;" class="grid-report-item green">${item.Status}</span>`
                    }
                    if (item.StatusId == 20 || item.StatusId == 25)//FeedStatus.Unpublished,FeedStatus.Disapproved
                    {
                        str = str + `<span style="display: inline-block;font-size:12px;" class="grid-report-item red">${item.Status}</span>`
                    }
                    if (item.StatusId == 10) //FeedStatus.Processing
                    {
                        str = str + `<span style="display: inline-block;font-size:12px;" class="grid-report-item blue">Processing</span>`
                    }
                    if (item.StatusId == 1 || item.StatusId == 30 || item.StatusId == 5) //FeedStatus.New,FeedStatus.Imported,FeedStatus.Pending
                    {
                        str = str + `<span style="display: inline-block;background:#cabcdc;font-size:12px;" class="grid-report-item">New</span>`
                    }
                    str = str + `</div>
                            </div>
                            <div class="col-sm-10 col-sm-offset-3" style="padding-top: 47px;">
                                <div class="col-sm-3 col-sm-offset-1 res-btn">`
                    if (item.PublishedOnce) {
                        str = str + `<a href='https://merchants.google.com/mc/items/details?offerId=${item.Sku}&country=${item.Country}&language=${item.Language}&channel=${item.Channel}&a=${item.MerchantId}' class="btn btn-default" target='_blank'><i class="fa fa-eye"></i>View in Merchant Center</a>`
                    }
                    else {
                        str = str + `<a href="javascript:void(0)" onclick="" disabled='disabled' class="btn btn-default" target='_blank' title='This variant is not published to merchant center.'><i class="fa fa-eye"></i>View in Merchant Center</a>`
                    }
                    str = str + `</div>
                                <div class="col-sm-2 res-btn">`
                    if (feedSelVal === '5') {
                        str = str + `<a href="javascript:void(0)" onclick="OpenVariantEditModalPopup(${item.FeedId})" class="btn btn-default" target="_blank"><i class="fa fa-pencil"></i>Edit Variant</a>`
                    }
                    else {
                        //str = str + `<a href="javascript:void(0)" onclick="" class="btn bg-blue" target="_blank" title="Variant will be saved."><i class="fa fa-floppy-o"></i>Save Variant</a>`
                        str = str + `<a href="javascript:void(0)" onclick="" disabled="disabled" class="btn btn-default" title="Enbale by shifting to Variant wise data feed mode" target="_blank"><i class="fa fa-pencil"></i>Edit Variant</a>`
                    }
                    str = str + `</div>
                                <div class="col-sm-2 res-btn">
                                    <a href="javascript:void(0)" class="btn bg-blue" data-var-id="${item.FeedId}" title="Variant will be published to merchant center." id="varPublish" onclick="PublishVariant(this)"><i class="fa fa-upload"></i>Publish</a>
                                </div>
                                <div class="col-sm-2 res-btn">
                                    <a href="javascript:void(0)" class="btn bg-red" data-var-id="${item.FeedId}" title="Variant will be unpublished from merchant center." id="varUnpublish" onclick="UnpublishVariant(this)"><i class="fa fa-eye-slash"></i>Unpublish</a>
                                </div>`
                    if (item.HasErrorList) {
                        str = str + `<div class="col-sm-2 res-btn error-row-var">
                                                <a href="javascript:void(0)" title="Click to view error(s)" onclick="OpenErrorListModalPopup('${item.GoogleProductId}')">View Issue(s)</a>
                                            </div>`
                    }
                    str = str + `</div>
                         </div>
                    </div>`
                });
                document.getElementById('variantList').innerHTML = str;
            }
            else {
                $("#mainProdVMCenter").show();
                $('#attributesID').hide();

                if (feedSelVal !== '0') {

                }
                $('.com-parent').show();
                $('#fsel-div').show();
                //$('.header-status-text').show();
                //if (feedSelVal !== '5') {
                //    $('.header-save-btns').show();
                //}
                //$('.header-status-text-var').hide();
            }
        }
    });
}

function HideAllRButtons() {
    $('#NameMain').hide();
    $('#SEName').hide();
    $('#CustomName').hide();
    $('#Description').hide();
    $('#SEDescription').hide();
    $('#CustomDescription').hide();
}

function OpenVariantEditModalPopup(feedId) {
    $.ajax({
        cache: false,
        type: "POST",
        url: $('#loadVariantsData').data('request-urlpopup'),
        data: { productFeedId: feedId, productId: $("#ProductId").val() },
        success: function (data) {
            if (data !== '') {
                $("#modalbodyData").html(data.VariantPartialView);
                $('#errorModal').modal('show');
                if (feedId !== '0') {
                    $('.modal-title').text('Edit Variant - ' + $('#Name').val());
                }
                else {
                    $('.modal-title').text('Add Variant - ' + $('#Name').val());
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
}

function OpenErrorListModalPopup(googleProductId) {
    if (googleProductId == '')
        return false;

    $.ajax({
        cache: false,
        type: "POST",
        url: $('#loadVariantsData').data('request-errorpopup'),
        data: { googleProductId: googleProductId },
        success: function (data) {
            if (data !== '') {
                $("#modalbodyData").html(data.ErrorListPartialView);
                $('#errorModal').modal('show');
                $('.modal-title').text('Item Issue(s)');
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
}

function ChangeFeedSelectionType(confirmedData) {
    var feedSelVal = $("#commonmodalbtn").val();
    var tosendValue = 0;
    if (feedSelVal === 'FeedSelectionCommon') {
        $("#FeedSelectionTypeId").val('1');
        tosendValue = 1;
    }
    else if (feedSelVal === 'FeedSelectionVariants') {
        $("#FeedSelectionTypeId").val('5');
        tosendValue = 5;
    }

    $.ajax({
        cache: false,
        type: "POST",
        url: $('#commonmodalbtn').data('request-fselurl'),
        data: { productFeedId: $("#Id").val(), selectionType: tosendValue },
        success: function (data) {
            $('#reConfirm').modal('hide');
            ShowNotifications(data.Message);
            if (data.Success) {
                if (data.NewFeedUrl !== '') {
                    location.replace(data.NewFeedUrl);
                    return true;
                }
                location.reload(true);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
}

function ShowNotifications(message) {
    $('#edit-notifications').show();
    $('#notific-message').html(message);
    setTimeout(function () {
        $('#edit-notifications').hide();
        $('#notific-message').html('');
    }, 8000);
    $("html, body").animate({ scrollTop: 0 }, "slow");
}

function ResetFeed() {
    $('#reConfirm').modal('hide');
    $.ajax({
        cache: false,
        type: "POST",
        url: $('#modalresetbtn').data('request-url'),
        data: { productFeedId: $("#Id").val() },
        success: function (data) {
            ShowNotifications(data.Message);
            if (data.Success) {
                setTimeout(function () {
                    if (data.NewFeedUrl !== '') {
                        location.replace(data.NewFeedUrl);
                        return true;
                    }
                    location.reload(true);
                }, 3000);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
}

function PublishFeed() {
    $('#reConfirm').modal('hide');
    $.ajax({
        cache: false,
        type: "POST",
        url: $('#modalpublishbtn').data('request-url'),
        data: { productFeedId: feedId, allVariants: publishAll },
        success: function (data) {
            ShowNotifications(data.Message + " Please wait..Reloading.");
            setTimeout(function () {
                location.reload(true);
            }, 5000);  
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
}

function UnpublishFeed() {
    $('#reConfirm').modal('hide');
    $.ajax({
        cache: false,
        type: "POST",
        url: $('#modalunpublishbtn').data('request-url'),
        data: { productFeedId: feedId, allVariants: publishAll },
        success: function (data) {
            ShowNotifications(data.Message + " Please wait..Reloading.");
            setTimeout(function () {
                location.reload(true);
            }, 5000);  
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
}

function ChangeStockStatus() {
    $("#changeStockStatus").show();
    $("#change-status-sel").hide();
    $("#change-status-clear").show();
}

//function ShowTaxAddForm() {
//    $("#overrideTaxData").show();
//    $("#add-tax-product").hide();
//    $("#remove-tax-product").show();
//    $("#accountTaxTable").hide();
//}

//function HideTaxAddForm() {
//    $("#overrideTaxData").hide();
//    $("#add-tax-product").show();
//    $("#remove-tax-product").hide();
//}

function CheckTextboxCharCount(handle) {
    var currentLength = $(handle).val().length;
    var maxLength = $(handle).data('val-length-max');
    var keywords_remaining = maxLength - currentLength;
    var toUpdateDiv = $(handle).attr('name') + "_valtn";
    if (keywords_remaining < 0) {
        $('#' + updateHandle).html('<font color="red">' + keywords_remaining + ' characters remaining </font>');
    }
    else {
        $('#' + toUpdateDiv).html(keywords_remaining + ' characters remaining');
    }
}

function SetMaxLength(handle) {
    var maxLength = $(handle).data('val-length-max');
    $(handle).attr('maxlength', maxLength);
}