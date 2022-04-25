function insertVehicles(data){
    data = JSON.parse(data);
    data.forEach(vehicle => {
        let id = vehicle[0];
        let name = vehicle[1];
        let model = vehicle[2];
        $('.panel_vehicles_vehicles').append(`
            <div class="panel_vehicles_vehicle" id="veh_${id}" style="background-image: url('http://51.38.128.119/img/${model}.png')">
                <div class="panel_vehicle_id">${id}</div>
                <div class="panel_vehicle_name">${name}</div>
            </div>
        `);

        $(`#veh_${id}`).on("click", function(){
            $('.panel_vehicles_vehicles *').removeClass("panel_vehicle_selected");
            $(this).addClass('panel_vehicle_selected');
            setLoading();
            mp.trigger('mainPanel_requestVehicleData', id);
        });
    })
}


function setLoading(){
    $('.panel_vehicle_info').css('align-items', 'center');
    $('.panel_vehicle_info').html(`
    <div class="lds-ring"><div></div><div></div><div></div><div></div></div>
    `);
}

function stopLoading(){
    $('.panel_vehicle_info').css('align-items', 'flex-start'); 
    $('.panel_vehicle_info').html(``);
}

function setVehiclesData(vehInfo, mechInfo, visuInfo){
    stopLoading();
    vehInfo = JSON.parse(vehInfo);
    
    let name = vehInfo[0];
    let id = vehInfo[1];
    let state = vehInfo[2];
    let comb = vehInfo[3];
    let tank = vehInfo[4];
    let trunk = vehInfo[5];
    let trip = vehInfo[6];
    let mechtune = mechInfo == "" ? "brak" : JSON.parse(mechInfo).join(', ');
    let visutune = visuInfo == "" ? "brak" : JSON.parse(visuInfo).join(', ');

    $('.panel_vehicle_info').html(`
        <p><b>Nazwa: </b>${name}</p>
        <p><b>ID: </b>${id}</p>
        <p><b>Status pojazdu: </b>${state}</p>
        <p><b>Przebieg: </b>${parseFloat(trip).toFixed(1)} km</p>
        <p><b>Współczynnik spalania: </b>${comb}</p>
        <p><b>Pojemność baku: </b>${tank}</p>
        <p><b>Bagażnik: </b>${trunk}</p>
        <p><b>Tuning mechaniczny: </b>${mechtune}</p>
        <p><b>Tuning wizualny: </b>${visutune}</p>
    `);
}

const maxLevels = [4,15,10,20,1];
const costs = [5,2,1,1,18];
let currentLevels = [];
let skillPoints = 0;
let wairForResponse = true;
let authcode = "";
function insertPlayerData(playersData, skillsData){
    playersData = JSON.parse(playersData);
    skillsData = JSON.parse(skillsData);

    $('.panel_user_general').append(`
        <p><b>Nick: </b>${playersData[0]}</p>
        <p><b>SocialClubID: </b>${playersData[1]}</p>
        <p><b>ServerID: </b>${playersData[2]}</p>
        <p><b>Data rejestracji: </b>${playersData[3]}</p>
        <p><b>Łączny czas gry: </b>${(Math.round(parseInt(playersData[4]) /60 * 10) / 10).toString()}h</p>
        <p><b>Postęp poziomu: </b>${playersData[5]}</p>
        <p><b>Ilość pojazdów: </b>${playersData[6]}</p>
        <p class="panel_skillPoints"><b>Dostępne punkty umiejętności: </b>${playersData[7]}</p>
        <p><b>Postęp znajdziek: </b>${playersData[8]}</p>
        <p><b>Do bonusu dziennego: </b> ${playersData[14]}m</p>
    `);

    $('.panel_user_jobs').append(`
    <p><b>Logistyki (PL): </b>${playersData[9]}</p>
    <p><b>Natury (PN): </b>${playersData[10]}</p>
    <p><b>Wody (PW): </b>${playersData[11]}</p>
    <p><b>Społeczne (PS): </b>${playersData[12]}</p>
    `);
    authcode = playersData[13];
    skillPoints = parseInt(playersData[7]);
    skillsData.forEach((level, index) => {
        currentLevels[index] = level;
        let className = getClassNameByIndex(index);
        $("." + className + "Lvl").text(level.toString() + "/" + maxLevels[index].toString());
    });
    waitForResponse = false;
    checkPoints();
}

function checkPoints(){
    currentLevels.forEach((level, index) => {
        let className = getClassNameByIndex(index);
        if(skillPoints >= costs[index] && level < maxLevels[index]){
            $("." + className + "Add .addPoint").removeClass("hide");
            console.log("XD");
        }
        else{
            $("." + className + "Add .addPoint").addClass("hide");
        }
    });
}

function getClassNameByIndex(index){
    let className = "";
    switch(index){
        case 0:
            className = "eq";
            break;
        case 1:
            className = "zarobki";
            break;
        case 2:
            className = "ed";
            break;
        case 3:
            className = "sloty";
            break;
        case 4:
            className = "sz";
            break;
    }
    return className;
}

function getIndexByClassName(className){
    let index = 0;
    switch(className){
        case "eqAdd":
            index = 0;
            break;
        case "zarobkiAdd":
            index = 1;
            break;
        case "edAdd":
            index = 2;
            break;
        case "slotyAdd":
            index = 3;
            break;
        case "szAdd":
            index = 4;
            break;
    }
    return index;
}

$(".addPoint").on("click", function(event){
    if(!waitForResponse){
        let c = $(this).parent().attr("class");
        let index = getIndexByClassName(c);
        mp.events.call("mainPanel_addPoint", index);
    }
    
});

function setSkillPoints(sPoints, levels){
    $(`.panel_skillPoints`).html(`<b>Dostępne punkty umiejętności: </b>${sPoints}`);
    skillPoints = sPoints;
    levels = JSON.parse(levels);
    levels.forEach((level, index) => {
        currentLevels[index] = level;
        let className = getClassNameByIndex(index);
        $("." + className + "Lvl").text(level.toString() + "/" + maxLevels[index].toString());
    });
    waitForResponse = false;
    checkPoints();
}



const keyCodes = {
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

let settings = {
    hudSize: 50,
    chatSize: 50,
    speedometerSize: 50,
    displayNick: true,
    displayGlobal: true,
    voiceChat: false,
    voiceKey: 88,
    useEmojis: true
}

function insertSettings(set){
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

let settingsTime = null;
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