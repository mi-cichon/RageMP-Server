mp.events.call("mainPanel_requestSettingsData");
var authcode = "";
function copyToClipboard(str){
    const el = document.createElement('textarea');
    el.value = str;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
  };

var keyCodes = {
    8: "backspace",
    9: "tab",
    13: "enter",
    16: "shift",
    17: "ctrl",
    18: "alt",
    19: "pause",
    20: "capslock",
    33: "pageup",
    34: "pagedown",
    35: "end",
    36: "home",
    37: "left",
    38: "up",
    39: "right",
    40: "down",
    45: "insert",
    46: "delete",
    48: "0",
    49: "1",
    50: "2",
    51: "3",
    52: "4",
    53: "5",
    54: "6",
    55: "7",
    56: "8",
    57: "9",
    65: "a",
    66: "b",
    67: "c",
    68: "d",
    69: "e",
    70: "f",
    71: "g",
    72: "h",
    73: "i",
    74: "j",
    75: "k",
    76: "l",
    77: "m",
    78: "n",
    79: "o",
    80: "p",
    81: "q",
    82: "r",
    83: "s",
    84: "t",
    85: "u",
    86: "v",
    87: "w",
    88: "x",
    89: "y",
    90: "z",
    96: "num 0",
    97: " num 1",
    98: " num 2",
    99: " num 3",
    100: " num 4",
    101: " num 5",
    102: " num 6",
    103: " num 7",
    104: " num 8",
    105: " num 9",
    106: " num *",
    107: " num +",
    109: " num -",
    110: " num .",
    111: " num /",
    112: "f1",
    113: "f2",
    114: "f3",
    115: "f4",
    116: "f5",
    117: "f6",
    118: "f7",
    119: "f8",
    120: "f9",
    121: "f10",
    122: "f11",
    123: "f12",
    144: "numlock",
    145: "scrolllock",
    186: ";",
    187: "=",
    188: ",",
    189: "-",
    190: ".",
    191: "/",
    192: "~",
    219: "[",
    220: "\\",
    221: "]",
    222: "'"
  };

var listenToKey = false;

var settings = {
    hudSize: 50,
    chatSize: 50,
    speedometerSize: 50,
    displayNick: true,
    displayGlobal: true,
    voiceChat: false,
    voiceKey: 88,
    useEmojis: true
}

function insertSettings(set, ac){
    authcode = ac;
    settings = JSON.parse(set);

    $("#hudSize").val(settings.hudSize);

    $("#chatSize").val(settings.chatSize);

    $("#speedometerSize").val(settings.speedometerSize);

    $('input[type=radio][name=nick]').filter(function(){return this.value == settings.displayNick.toString()}).attr("checked", "checked");

    $('input[type=radio][name=global]').filter(function(){return this.value == settings.displayGlobal.toString()}).attr("checked", "checked");

    $('input[type=radio][name=voice]').filter(function(){return this.value == settings.voiceChat.toString()}).attr("checked", "checked");

    $('input[type=radio][name=useEmoji]').filter(function(){return this.value == settings.useEmojis.toString()}).attr("checked", "checked");
    
    $(".voiceKey").text(keyCodes[settings.voiceKey].toUpperCase());
}

$("#hudSize").on("change", function(){
    settings.hudSize = parseInt($(this).val());
    saveSettings();
});

$("#chatSize").on("change", function(){
    settings.chatSize = parseInt($(this).val());
    saveSettings();
});

$("#speedometerSize").on("change", function(){
    settings.speedometerSize = parseInt($(this).val());
    saveSettings();
});

$('input[type=radio][name=nick]').on("change", function() {
    settings.displayNick = $(this).val() === "true";
    saveSettings();
});

$('input[type=radio][name=global]').on("change", function() {
    settings.displayGlobal = $(this).val() === "true";
    saveSettings();
});

$('input[type=radio][name=voice]').on("change", function() {
    settings.voiceChat = $(this).val() === "true";
    saveSettings();
});

$('input[type=radio][name=useEmoji]').on("change", function() {
    settings.useEmojis = $(this).val() === "true";
    saveSettings();
});

$(".voiceKey").on("click", function(){
    listenToKey = true;
    $(".clickText").css("display", "flex");
    $('.clickText div').css("display", "block");
});

$(window).on("keyup", function(e){
    if(listenToKey && (e.which in keyCodes)){
        listenToKey = false;
        settings.voiceKey = e.which;
        $(".voiceKey").text(keyCodes[e.which].toUpperCase());
        $("body").removeClass("hidden");
        $(".clickText").css("display", "none");
        saveSettings();
    }
});

var settingsTime = null;
function saveSettings(){
    if(settingsTime == null){
        settingsTime = setTimeout(function(){
            mp.trigger("mainPanel_saveSettings", JSON.stringify(settings));
            mp.trigger("settings_setHUDScales", settings.hudSize, settings.chatSize);
            mp.trigger("settings_setSpeedometerScale", settings.speedometerSize);
            settingsTime = null;
        }, 1000);
    }
}

$('.panel_settings_discordButton').on('click', function(){
    copyToClipboard(authcode);
});