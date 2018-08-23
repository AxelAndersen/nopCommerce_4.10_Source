//Settings Page js

var interval_id = '';
var interval_gconfig = '';
var interval_id_dHide = '';
var authwinref = '';
$(document).ready(function () {
    $('[data-tooltip="tooltip"]').popover();
    HideShowSaleDate($("#DefaultOnSalePreference").val());
    CheckSyncService();
    StartInterval(5);

    $("#sync_locally").on('click', function () {
        $("#syncing-div").show();
        $("#spinner-sync").show();
        $("#sync-message").empty().html("Getting Ready to sync feed(s)");
        $("#spinner-sync-success").hide();
        $("#spinner-sync-error").hide();
        $("#stop-running-task-settings").show();
        StartLocalSync();
    });

    $("#gconfig-btn").on('click', function () {
        authwinref = window.open($(this).data('auth-url'));
        if (interval_gconfig === '') {
            interval_gconfig = setInterval(CheckGoogleAccountIntegrated, (5 * 1000));
        }
    });

    $("#TargetCountryId").change(function (event) {
        PopulateCurrencyField($(this).val(), true);
        PopulateLanguageDropdown($(this).val(), true);
    });

    $("#DefaultOnSalePreference").change(function (event) {
        HideShowSaleDate($(this).val());
    });
});

function HideShowSaleDate(optionVal) {
    if (optionVal === '0') {
        $("#salestrtDt").hide();
    }
    else {
        $("#salestrtDt").show();
    }
}

function StartInterval(time) {
    ClearIntervaltToHideNotifDiv();
    interval_id = setInterval(CheckSyncService, (time * 1000));
}

function ClearInterval() {
    if (interval_id !== '') {
        clearInterval(interval_id);
        interval_id = '';
    }
}
function StartIntervalOnListToHideNotif(time) {
    if (interval_id === '') {
        interval_id_dHide = setInterval(HideNotification, (time * 1000));
    }
}

function ClearIntervaltToHideNotifDiv() {
    if (interval_id_dHide !== '') {
        clearInterval(interval_id_dHide);
        interval_id_dHide = '';
    }
}

function HideNotification() {
    $("#sync-message").empty();
    $("#syncing-div").hide();
    ClearIntervaltToHideNotifDiv();
}

function HideNotificationOnGAConfig() {
    $("#sync-message").empty();
    $("#syncing-div").hide();
    if (interval_gconfig !== '') {
        clearInterval(interval_gconfig);
        interval_gconfig = '';
    }
}