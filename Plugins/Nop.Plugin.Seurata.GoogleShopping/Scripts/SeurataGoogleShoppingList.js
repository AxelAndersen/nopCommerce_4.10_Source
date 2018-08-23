//List Page js

var selectedIds = [];
var interval_id_nxt = '';
var interval_id_divHide = '';
var impc = false;

$(document).ready(function () {
    CheckSyncServiceOnList();
    StartIntervalOnList(5);

    $('#search-products').click(function () {
        BindRecord();
    });

    $("#Name").keydown(function (event) {
        if (event.keyCode === 13) {
            $("#search-products").click();
            return false;
        }
    });

    $('#mastercheckbox').click(function () {
        $('.checkboxGroups').attr('checked', $(this).is(':checked')).change();
    });

    $('#mastercheckboxPending').click(function () {
        $('.checkboxGroupsPending').attr('checked', $(this).is(':checked')).change();
    });

    //wire up checkboxes.
    $('#shopping-products-grid').on('change', 'input[type=checkbox][id!=mastercheckbox][class=checkboxGroups]', function (e) {
        var $check = $(this);
        if ($check.is(":checked") == true) {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked == -1) {
                //add id to selectedIds.
                selectedIds.push($check.val());
            }
        }
        else {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked > -1) {
                //remove id from selectedIds.
                selectedIds = $.grep(selectedIds, function (item, index) {
                    return item != $check.val();
                });
            }
        }
        updateMasterCheckbox();
    });
    $('#error-shopping-products-grid').on('change', 'input[type=checkbox][id!=mastercheckboxerror][class=checkboxGroupsError]', function (e) {
        var $check = $(this);
        if ($check.is(":checked") == true) {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked == -1) {
                //add id to selectedIds.
                selectedIds.push($check.val());
            }
        }
        else {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked > -1) {
                //remove id from selectedIds.
                selectedIds = $.grep(selectedIds, function (item, index) {
                    return item != $check.val();
                });
            }
        }
        updateMasterCheckbox();
    });
    $('#published-shopping-products-grid').on('change', 'input[type=checkbox][id!=mastercheckboxpublished][class=checkboxGroupsPublished]', function (e) {
        var $check = $(this);
        if ($check.is(":checked") == true) {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked == -1) {
                //add id to selectedIds.
                selectedIds.push($check.val());
            }
        }
        else {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked > -1) {
                //remove id from selectedIds.
                selectedIds = $.grep(selectedIds, function (item, index) {
                    return item != $check.val();
                });
            }
        }
        updateMasterCheckbox();
    });
    $('#draft-shopping-products-grid').on('change', 'input[type=checkbox][id!=mastercheckboxdraft][class=checkboxGroupsDraft]', function (e) {
        var $check = $(this);
        if ($check.is(":checked") == true) {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked == -1) {
                //add id to selectedIds.
                selectedIds.push($check.val());
            }
        }
        else {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked > -1) {
                //remove id from selectedIds.
                selectedIds = $.grep(selectedIds, function (item, index) {
                    return item != $check.val();
                });
            }
        }
        updateMasterCheckbox();
    });
    $('#unpublished-shopping-products-grid').on('change', 'input[type=checkbox][id!=mastercheckboxunpublished][class=checkboxGroupsUnpublished]', function (e) {
        var $check = $(this);
        if ($check.is(":checked") == true) {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked == -1) {
                //add id to selectedIds.
                selectedIds.push($check.val());
            }
        }
        else {
            var checked = jQuery.inArray($check.val(), selectedIds);
            if (checked > -1) {
                //remove id from selectedIds.
                selectedIds = $.grep(selectedIds, function (item, index) {
                    return item != $check.val();
                });
            }
        }
        updateMasterCheckbox();
    });

    $("#publish-merchant-center").on('click', function () {
        if (selectedIds.length <= 0) {
            $('#myModal').modal('hide');
            $("#syncing-div").show();
            $("#spinner-sync").hide();
            $("#sync-message").empty().html("No Product(s) selected to publish.");
            $("#spinner-sync-success").hide();
            $("#spinner-sync-error").show();
            StartIntervalOnListToHideNotifDiv(15);
            return false;
        }

        $("#syncing-div").show();
        $("#spinner-sync").show();
        $("#sync-message").empty().html("Getting Ready to sync feed(s) to merchant center");
        $("#spinner-sync-success").hide();
        $("#spinner-sync-error").hide();
        $("#stop-running-task-list").show();
        PublishFeedToMerchantCenter(this);
    });

    $("#stop-running-task-list").on('click', function () {
        $.ajax({
            cache: false,
            type: "POST",
            url: $(this).data('request-url'),
            success: function (data) {

            },
            error: function (xhr, ajaxOptions, thrownError) {
            },
            traditional: true
        });
    });

    $('#shopping-exportexcel-selected').click(function (e) {
        e.preventDefault();
        var ids = selectedIds.join(",");
        $('#shopping-export-excel-selected-form #selectedIds').val(ids);
        $('#shopping-export-excel-selected-form').submit();
        return false;
    });

    $("#import-button-clicked").on('click', function () {
        impc = true;
    });
});

function additionalData() {
    var data = {
        Name: $('#Name').val(),
        SearchCategoryId: $('#SearchCategoryId').val(),
        SearchStoreId: $('#SearchStoreId').val(),
        AvailabilityStatusIds: $('#AvailabilityStatusIds').val()
    };
    addAntiForgeryToken(data);
    return data;
}

function BindRecord() {
    var grid = $('#shopping-products-grid').data('kendoGrid');
    grid.dataSource.page(1);

    grid = $('#error-shopping-products-grid').data('kendoGrid');
    grid.dataSource.page(1);

    grid = $('#published-shopping-products-grid').data('kendoGrid');
    grid.dataSource.page(1);

    grid = $('#draft-shopping-products-grid').data('kendoGrid');
    grid.dataSource.page(1);

    grid = $('#unpublished-shopping-products-grid').data('kendoGrid');
    grid.dataSource.page(1);

    $('.checkboxGroups').attr('checked', false).change();
    $('.checkboxGroupsPublished').attr('checked', false).change();
    $('.checkboxGroupsError').attr('checked', false).change();
    $('.checkboxGroupsDraft').attr('checked', false).change();
    $('.checkboxGroupsUnpublished').attr('checked', false).change();
    selectedIds = [];
    return false;
}

function onDataBound(e) {
    var tabID = e.sender.element[0].id;
    var attrVal = "";
    switch (tabID) {
        case "published-shopping-products-grid":
            attrVal = "tab-published";
            break;
        case "unpublished-shopping-products-grid":
            attrVal = "tab-unpublished";
            break;
        case "error-shopping-products-grid":
            attrVal = "tab-error";
            break;
        case "shopping-products-grid":
            attrVal = "tab-all";
            break;
        case "draft-shopping-products-grid":
            attrVal = "tab-recently-imported";
            break;
        default:
    }
    var grid = $("#" + tabID).data("kendoGrid");
    var dataSource = grid.dataSource;
    var totalRecords = dataSource.total();
    var tab = $("[data-tab-name=" + attrVal + "]");
    var val = tab.html().split(' ');
    val = val[0] + " (" + totalRecords + ")";
    tab.html(val);

    $('#shopping-products-grid input[type=checkbox][id!=mastercheckbox][class=checkboxGroups]').each(function () {
        var currentId = $(this).val();
        var checked = jQuery.inArray(currentId, selectedIds);
        $(this).attr('checked', checked > -1);
    });
    $('#error-shopping-products-grid input[type=checkbox][id!=mastercheckboxerror][class=checkboxGroupsError]').each(function () {
        var currentId = $(this).val();
        var checked = jQuery.inArray(currentId, selectedIds);
        $(this).attr('checked', checked > -1);
    });
    $('#published-shopping-products-grid input[type=checkbox][id!=mastercheckboxpublished][class=checkboxGroupsPublished]').each(function () {
        var currentId = $(this).val();
        var checked = jQuery.inArray(currentId, selectedIds);
        $(this).attr('checked', checked > -1);
    });
    $('#draft-shopping-products-grid input[type=checkbox][id!=mastercheckboxdraft][class=checkboxGroupsDraft]').each(function () {
        var currentId = $(this).val();
        var checked = jQuery.inArray(currentId, selectedIds);
        $(this).attr('checked', checked > -1);
    });
    $('#unpublished-shopping-products-grid input[type=checkbox][id!=mastercheckboxunpublished][class=checkboxGroupsUnpublished]').each(function () {
        var currentId = $(this).val();
        var checked = jQuery.inArray(currentId, selectedIds);
        $(this).attr('checked', checked > -1);
    });
    updateMasterCheckbox();
}

function updateMasterCheckbox() {
    var numChkBoxes = $('#shopping-products-grid input[type=checkbox][id!=mastercheckbox][class=checkboxGroups]').length;
    var numChkBoxesChecked = $('#shopping-products-grid input[type=checkbox][id!=mastercheckbox][class=checkboxGroups]:checked').length;
    $('#mastercheckbox').attr('checked', numChkBoxes == numChkBoxesChecked && numChkBoxes > 0);

    var numChkBoxesPublished = $('#published-shopping-products-grid input[type=checkbox][id!=mastercheckboxpublished][class=checkboxGroupspublished]').length;
    var numChkBoxesCheckedPublished = $('#published-shopping-products-grid input[type=checkbox][id!=mastercheckboxpublished][class=checkboxGroupspublished]:checked').length;
    $('#mastercheckboxpublished').attr('checked', numChkBoxesPublished == numChkBoxesCheckedPublished && numChkBoxesPublished > 0);

    var numChkBoxesError = $('#error-shopping-products-grid input[type=checkbox][id!=mastercheckboxError][class=checkboxGroupsError]').length;
    var numChkBoxesCheckedError = $('#error-shopping-products-grid input[type=checkbox][id!=mastercheckboxError][class=checkboxGroupsError]:checked').length;
    $('#mastercheckboxerror').attr('checked', numChkBoxesError == numChkBoxesCheckedError && numChkBoxesError > 0);

    var numChkBoxesDraft = $('#draft-shopping-products-grid input[type=checkbox][id!=mastercheckboxDraft][class=checkboxGroupsDraft]').length;
    var numChkBoxesCheckedDraft = $('#draft-shopping-products-grid input[type=checkbox][id!=mastercheckboxDraft][class=checkboxGroupsDraft]:checked').length;
    $('#mastercheckboxdraft').attr('checked', numChkBoxesDraft == numChkBoxesCheckedDraft && numChkBoxesDraft > 0);

    var numChkBoxesUnpublished = $('#unpublished-shopping-products-grid input[type=checkbox][id!=mastercheckboxunpublished][class=checkboxGroupsUnpublished]').length;
    var numChkBoxesCheckedUnpublished = $('#unpublished-shopping-products-grid input[type=checkbox][id!=mastercheckboxunpublished][class=checkboxGroupsUnpublished]:checked').length;
    $('#mastercheckboxunpublished').attr('checked', numChkBoxesUnpublished == numChkBoxesCheckedUnpublished && numChkBoxesUnpublished > 0);
}

function UnPublishFeedFromMerchantCenter(handle) {
    $('#myModal').modal('hide');
    $("#ajaxBusy").hide();
    var postData = {
        selectedIds: selectedIds
    };

    if (selectedIds.length <= 0) {
        $('#myModal').modal('hide');
        $("#syncing-div").show();
        $("#spinner-sync").hide();
        $("#sync-message").empty().html("No Product(s) selected to unpublish.");
        $("#spinner-sync-success").hide();
        $("#spinner-sync-error").show();
        StartIntervalOnListToHideNotifDiv(15);
        return false;
    }

    addAntiForgeryToken(postData);
    $("#syncing-div").show();
    $("#spinner-sync").show();
    $("#sync-message").empty().html("Getting Ready to delete feed(s) to merchant center");
    $("#spinner-sync-success").hide();
    $("#spinner-sync-error").hide();
    $("#stop-running-task-list").show();
    $.ajax({
        cache: false,
        type: "POST",
        url: $(handle).data('request-url'),
        data: postData,
        complete: function (data) {
            window.location.reload();
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
    StartIntervalOnList(5);
    $("#ajaxBusy").hide();
    return false;
}

function SyncSingleProduct(id, handle) {
    var postData = {
        selectedId: id
    };
    $.ajax({
        cache: false,
        type: "POST",
        url: $(handle).data('request-url'),
        data: postData,
        success: function (data) {
            if (data.Message !== '') {
                $('#import-syncing-div').show();
                $('#sync-message-import').html(data.Message);

                setTimeout(function () { $('#import-syncing-div').hide(); $('#sync-message-import').html(''); }, 8000);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });

}
function OpenModalPopup(data) {
    var arr = data.getAttribute("data-errordata").split(',');
    var divData = "<ul>";
    for (i = 0; i < arr.length; i++) {
        //var c = arr[i].split(':');
        //if (c[0] === "N/A") {
        //    divData += "<li>" + c[1] + "</li>";
        //}
        //else {
            divData += "<li>" + arr[i] + "</li>";
        //}
    }
    divData += "</ul>";
    $("#modalbodyData").html(divData);
    $('#errorModal').modal('show');
}

function CheckSyncServiceOnList() {
    $("#ajaxBusy").hide();
    $.ajax({
        cache: false,
        type: "GET",
        url: $('#syncServiceCheck').data('request-url'),
        success: function (data) {
            if (data.Success) {
                $("#syncing-div").show();
                $("#spinner-sync").show();
                $("#sync-message").empty().html(data.Message);
                $("#spinner-sync-success").hide();
                $("#spinner-sync-error").hide();
                $("#stop-running-task-list").show();
            } else if (data.Message !== '') {
                ClearIntervalOnList();
                $("#syncing-div").show();
                $("#spinner-sync").hide();
                $("#sync-message").empty().html(data.Message);
                if (data.Complete) {
                    $("#spinner-sync-error").hide();
                    $("#spinner-sync-success").show();
                } else {
                    $("#spinner-sync-success").hide();
                    $("#spinner-sync-error").show();
                }
                $("#stop-running-task-list").hide();
                StartIntervalOnListToHideNotifDiv(15)
            } else {
                ClearIntervalOnList();
                $("#syncing-div").hide();
                $("#spinner-sync").hide();
                $("#sync-message").empty();
                $("#spinner-sync-success").hide();
                $("#spinner-sync-error").hide();
                $("#stop-running-task-list").hide();
                StartIntervalOnListToHideNotifDiv(15)
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
    $("#ajaxBusy").hide();
}

function StartIntervalOnList(time) {
    ClearIntervalOnListToHideDiv();
    if (interval_id_nxt === '') {
        interval_id_nxt = setInterval(CheckSyncServiceOnList, (time * 1000));
    }
}

function ClearIntervalOnList() {
    if (interval_id_nxt !== '') {
        clearInterval(interval_id_nxt);
        interval_id_nxt = '';
    }
}

function StartIntervalOnListToHideNotifDiv(time) {
    if (interval_id_nxt === '') {
        interval_id_divHide = setInterval(HideNotificationDiv, (time * 1000));
    }
}

function ClearIntervalOnListToHideDiv() {
    if (interval_id_divHide !== '') {
        clearInterval(interval_id_divHide);
        interval_id_divHide = '';
    }
}

function HideNotificationDiv() {
    $("#sync-message").empty();
    $("#syncing-div").hide();
    ClearIntervalOnListToHideDiv();
}

function PublishFeedToMerchantCenter(handle) {
    $("#ajaxBusy").hide();
    var postData = {
        selectedIds: selectedIds
    };
    addAntiForgeryToken(postData);
    $.ajax({
        cache: false,
        type: "POST",
        url: $(handle).data('request-url'),
        data: postData,
        complete: function (data) {
            window.location.reload();
        },
        error: function (xhr, ajaxOptions, thrownError) {
        },
        traditional: true
    });
    StartIntervalOnList(5);
    $("#ajaxBusy").hide();
}
