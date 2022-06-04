mp.events.call("mainPanel_requestInterfaceSettingsData");

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

function insertInterfaceSettings(set){
    settings = JSON.parse(set);

    $("#hudSize").val(settings.hudSize);

    $("#chatSize").val(settings.chatSize);

    $("#speedometerSize").val(settings.speedometerSize);

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