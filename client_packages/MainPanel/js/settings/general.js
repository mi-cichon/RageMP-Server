mp.events.call("mainPanel_requestGeneralSettingsData");

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
    
    function insertGeneralSettings(set){
        settings = JSON.parse(set);
    
        settings.displayNick ? $('input[type=checkbox][name=nick]').attr("checked", "checked") : $('input[type=checkbox][name=nick]').removeAttr("checked");
    
        settings.displayGlobal ? $('input[type=checkbox][name=global]').attr("checked", "checked") : $('input[type=checkbox][name=global]').removeAttr("checked");
    
        settings.voiceChat ? $('input[type=checkbox][name=voice]').attr("checked", "checked") : $('input[type=checkbox][name=voice]').removeAttr("checked");
    
        settings.useEmojis ? $('input[type=checkbox][name=useEmoji]').attr("checked", "checked") : $('input[type=checkbox][name=useEmoji]').removeAttr("checked");
        
        $(".voiceKey").text(keyCodes[settings.voiceKey].toUpperCase());
    }
    
    
    $('input').on("change", function() {
        console.log($(this).val());
        switch($(this).attr('name')){
            case "nick":
                settings.displayNick = $(this).attr("checked") !== "checked";
                break;
            case "global":
                settings.displayGlobal = $(this).attr("checked") !== "checked";
                break;
            case "voice":
                settings.voiceChat = $(this).attr("checked") !== "checked";
                break;
            case "useEmoji":
                settings.useEmojis = $(this).attr("checked") !== "checked";
                break;
        }

        if($(this).attr("checked") === "checked"){
            $(this).removeAttr("checked");
        }
        else{
            $(this).attr("checked", "checked");
        }
        saveSettings();
    });
    
    $(".voiceKey").on("click", function(){
        listenToKey = true;
        $(".clickText").css("display", "flex");
    });
    
    $(window).on("keyup", function(e){
        if(listenToKey && (e.which in keyCodes)){
            listenToKey = false;
            settings.voiceKey = e.which;
            $(".voiceKey").text(keyCodes[e.which].toUpperCase());
            $(".clickText").css("display", "none");
            saveSettings();
        }
    });
    
    var settingsTime = null;
    function saveSettings(){
        if(settingsTime == null){
            settingsTime = setTimeout(function(){
                mp.trigger("mainPanel_saveSettings", JSON.stringify(settings));
                settingsTime = null;
            }, 1000);
        }
        else{
            clearTimeout(settingsTime);
            settingsTime = null;
            saveSettings();
        }
    }