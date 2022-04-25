let lplayer = mp.players.local;
mp.nametags.enabled = false;
let x,y;

mp.gui.chat.show(false);

let chatEnabled = false;
let vehicleControlDown = false;
let height = 0;
let dots = 1;


//VEHICLE CONTROL - ARROWS
mp.keys.bind(0x26, true, function (){
    if(vehicleControlDown){
        mp.events.call("vehc_move", false);
    }
});

mp.keys.bind(0x28, true, function (){
    if(vehicleControlDown){
        mp.events.call("vehc_move", true);
    }
});

//SKILL PANEL
// mp.keys.bind(0x55, true, function (){
//     mp.events.call("openSkillPanelBrowser");
// })

//REPORT PANEL
mp.keys.bind(0x74, true, function (){
    if(lplayer.getVariable("power") >= 3)
        mp.events.call("openReportBrowser");
})

//ONLINE PANEL
mp.keys.bind(0x73, true, function (){
    mp.events.call("openOnlinePlayersBrowser");
})

//INFO PANEL
// mp.keys.bind(0x72, true, function (){
//     mp.events.call("openInfoPanel");
// })

//HELP PANEL
// mp.keys.bind(0x71, true, function (){
//     mp.events.call("openHelpPanelBrowser");
// })

//MAIN PANEL
mp.keys.bind(0x71, true, function (){
    mp.events.call("mainPanel_openBrowser");
})

//JOB HUD
mp.keys.bind(0x42, true, function (){
    if(lplayer.getVariable("job") != "" && !lplayer.getVariable("controlsblocked"))
        mp.events.call('toggleJobHUD');
})

//TOGGLE CURSOR
mp.keys.bind(0xC0, true, function (){
    mp.gui.cursor.show(!mp.gui.cursor.visible, !mp.gui.cursor.visible);
})

//CHAT INPUT
mp.keys.bind(0x54, true, function (){
    if(!chatEnabled && !lplayer.getVariable("gui")){
        chatEnabled = true;
        mp.events.callRemote("setPlayerTexting", true);
        mp.events.call("showChatInput");
    }
})

//HIDE CHAT INPUT
mp.keys.bind(0x1B, true, function (){
    if(chatEnabled){
        chatEnabled = false;
        setTimeout(function(){
            mp.events.call("hideChatInput");
            mp.events.callRemote("setPlayerTexting", false);
        },200)
    }
})

//SWITCH EQUIPMENT
mp.keys.bind(0x49, true, function (){
    if(!lplayer.getVariable("controlsblocked")){
        if(lplayer.vehicle && lplayer.vehicle.hasVariable("owner") && lplayer.vehicle.getVariable("owner") == lplayer.getVariable("socialclub")){
            let trunksize = lplayer.vehicle.getVariable("trunksize");
            mp.events.call("openSecondEquipmentBrowser", lplayer.getVariable("equipment"), lplayer.vehicle.getVariable("trunk"), "v" + lplayer.vehicle.getVariable("id").toString(), trunksize);
        }
        else if(lplayer.vehicle && lplayer.getVariable("job") == "gardener" && lplayer.vehicle.remoteId == lplayer.getVariable("jobveh") && lplayer.getVariable("vehSeat") == 0){
            mp.events.call("openSecondEquipmentBrowser", lplayer.getVariable("equipment"), lplayer.vehicle.getVariable("trunk"), "gardener", "[8,8]");
        }
        else{
            mp.events.call("switchEquipmentBrowser", lplayer.getVariable("equipment"));
        }
    }
})




mp.events.add('render', () => {
    mp.game.ui.hideHudComponentThisFrame(2);
    mp.game.ui.hideHudComponentThisFrame(3);
    mp.game.ui.hideHudComponentThisFrame(4);
    mp.game.ui.hideHudComponentThisFrame(7);
    mp.game.ui.hideHudComponentThisFrame(9);
    mp.game.ui.hideHudComponentThisFrame(6);
    mp.game.ui.hideHudComponentThisFrame(8);
    mp.game.ui.hideHudComponentThisFrame(14);

    //VEHICLE CONTROL - SCROLL

    if(vehicleControlDown && mp.game.controls.isControlJustPressed(32, 14)){
        mp.events.call("vehc_move", true);
    }
    if(vehicleControlDown && mp.game.controls.isControlJustPressed(32, 15)){
        mp.events.call("vehc_move", false);
    }

    //HOLDING

    //VEHICLE CONTROL - OPEN/CLOSE

    if(!vehicleControlDown && mp.keys.isDown(18) && lplayer.vehicle && !lplayer.getVariable("gui")){
        vehicleControlDown = true;
        mp.events.call("vehc_switchHUD", true);
    }
    else if(vehicleControlDown && mp.keys.isUp(18) && lplayer.vehicle){
        vehicleControlDown = false;
        mp.events.call("vehc_select");
        mp.events.call("vehc_switchHUD", false);
    }

    //DISABLE CONTROLS WHILE TYPING IN CHAT
    if(chatEnabled)
    {
        mp.game.controls.disableAllControlActions(32);
    }

    //PREVENT MOVING WHILE HAVING ANY GUI ON
    if(lplayer.getVariable("gui")){
        mp.game.controls.disableAllControlActions(0);
    }
 
    //THINGS THAT GO ON ABOVE PLAYERS HEADS
    height += 0.025;
    if(dots == 3 && height >= 1){
        height = 0;
        dots = 1;
    }else if(height >= 1){
        dots++;
        height = 0;
    }
    mp.players.forEachInRange(lplayer.position, 30, (player) =>
    {
        if(!(lplayer == player && lplayer.hasVariable("settings_DisplayNick") && !lplayer.getVariable("settings_DisplayNick"))){
            let offset = -0.015;
            
            if(player.doesExist() && !(player.hasVariable("spec") && player.getVariable("spec")))
            {
                let scale = (30 - getDistance(player.position, lplayer.position) / 30.0).clamp(0.2, 0.35);
                if(player.hasVariable("orgTag") && player.getVariable("orgTag") != ""){
                    point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
                    if(point && point.x < 1 && point.y < 1 && point.x > 0 && point.y > 0 && player.hasVariable("id")){
                        let pos = new Float64Array([point.x,point.y+offset - 0.005]);
                        // let scale = 0.7 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.3, 0.6);
                        mp.game.graphics.drawText(player.getVariable("orgTag"), pos, { 
                            font: 4, 
                            color: [141,89,194,255], 
                            scale: [scale*5/6, scale*5/6], 
                            outline: false
                        });
                    }
                    offset = -0.03;
                }
                let color = null;
                switch(player.getVariable("type"))
                {
                    case "owner":
                        color = [255, 0, 0, 255]
                        break;
                    case "admin":
                        color = [196, 49, 36, 255]
                        break;
                    case "jadmin":
                        color = [245, 102, 0, 255]
                        break;
                    case "smod":
                        color = [0, 99, 28, 255]
                        break;
                    case "mod":
                        color = [0, 153, 43, 255]
                        break;
                    case "jmod":
                        color = [0, 250, 70, 255]
                        break;
                    case "tester":
                        color = [0, 170, 255, 255]
                        break;
                    default:
                        color = [255, 255, 255, 255]
                        break;
                }
                point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
                if(point && point.x < 1 && point.y < 1 && point.x > 0 && point.y > 0 && player.hasVariable("id")){
                    let pos = new Float64Array([point.x,point.y]);
                    // let scale = 0.7 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.3, 0.6);
                    mp.game.graphics.drawText('[' + player.getVariable("id").toString() + ']' + player.getVariable("username"), pos, { 
                        font: 4, 
                        color: color, 
                        scale: [scale, scale], 
                        outline: true
                    });
                }
                if(player.hasVariable("voicechat") && player.getVariable("voicechat")){
                    if (!mp.game.graphics.hasStreamedTextureDictLoaded("mpleaderboard")) {
                        mp.game.graphics.requestStreamedTextureDict("mpleaderboard", true);
                    }
                    let point2 = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
                    let pos2 = new Float64Array([point2.x, point2.y + offset]);
                    // let scale2 = 0.05 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.3, 0.6);
                    // let scale3 = 0.06 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.3, 0.6);
                    mp.game.graphics.drawSprite("mpleaderboard", "leaderboard_audio_3", pos2[0], pos2[1], 0.7 * 0.085*scale, 0.085*scale, 0, 0, 145, 109, 255);
                    mp.game.graphics.drawSprite("mpleaderboard", "leaderboard_audio_3", pos2[0], pos2[1], 0.7 * 0.075*scale, 0.075*scale, 0, 0, 204, 153, 255);
                }   
                if(player.hasVariable("texting") && player.getVariable("texting")){
                    drawDots(player, offset);
                }
                if(player.hasVariable("lspd_duty") && player.getVariable("lspd_duty")){
                    if (!mp.game.graphics.hasStreamedTextureDictLoaded("mpleaderboard")) {
                        mp.game.graphics.requestStreamedTextureDict("mpleaderboard", true);
                    }
                    let lspdPoint = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
                    let lspdPos = new Float64Array([lspdPoint.x, lspdPoint.y + 0.035]);
                    mp.game.graphics.drawSprite("mpleaderboard", "leaderboard_star_icon", lspdPos[0], lspdPos[1], 0.6 * 0.085*scale, 1 * 0.085*scale, 0, 0, 0, 204, 255);
                }
            }        
        }
    });

    // mp.vehicles.forEachInRange(lplayer.position, 5, (vehicle) =>
    // {
    //     if(vehicle.hasVariable("mech") && vehicle.getVariable("mech"))
    //     {
    //         point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(vehicle.position.x, vehicle.position.y, vehicle.position.z + 1.6));
    //         if(point && point.x < 1 && point.y < 1 && point.x > 0 && point.y > 0){
    //             let pos = new Float64Array([point.x,point.y]);
    //             mp.game.graphics.drawText('Pojazd w trakcie naprawy', pos, { 
    //                 font: 4, 
    //                 color: [255, 94, 19, 200], 
    //                 scale: [0.7, 0.7], 
    //                 outline: true
    //             });
    //         }
    //     }
        
    // });
});

Number.prototype.clamp = function(min, max) {
    return Math.min(Math.max(this, min), max);
  };

function getDistance(vec1, vec2){
     return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}


mp.events.add("setChatEnabled", (value) =>{
    chatEnabled = value;
    mp.events.callRemote("setPlayersControlsBlocked", value);
    mp.gui.cursor.show(value, value);
});

//DRAWING DOTS THAT INDICATE WHETHER PLAYER'S TYPING OR NOT
function drawDots(player, offset){
    if(dots == 1){
        if (!mp.game.graphics.hasStreamedTextureDictLoaded("commonmenu")) {
            mp.game.graphics.requestStreamedTextureDict("commonmenu", true);
        }
        
        let scale = 0.05 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.2, 0.4);
        let point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
        let scale2 = scale * 5 / 2 * 10;
        let pos = new Float64Array([point.x - (scale2 * 0.014),point.y + offset]);
        mp.game.graphics.drawSprite("commonmenu", "common_medal", pos[0], pos[1], height * 0.60 * scale, 0.8 *scale * height, 0, 0, 204, 153, 255);
    }
    if(dots == 2){
        if (!mp.game.graphics.hasStreamedTextureDictLoaded("commonmenu")) {
            mp.game.graphics.requestStreamedTextureDict("commonmenu", true);
        }
        let scale = 0.05 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.2, 0.4);
        let point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
        let scale2 = scale * 5 / 2 * 10;
        let pos = new Float64Array([point.x - (scale2 * 0.014),point.y + offset]);
        mp.game.graphics.drawSprite("commonmenu", "common_medal", pos[0], pos[1], 0.60 * scale, 0.8 *scale, 0, 0, 204, 153, 255);

        point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
        pos = new Float64Array([point.x ,point.y + offset]);
        scale = 0.05 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.2, 0.4);
        mp.game.graphics.drawSprite("commonmenu", "common_medal", pos[0], pos[1], height * 0.60 * scale, 0.8 *scale * height, 0, 0, 204, 153, 255);
    }
    if(dots == 3){
        if (!mp.game.graphics.hasStreamedTextureDictLoaded("commonmenu")) {
            mp.game.graphics.requestStreamedTextureDict("commonmenu", true);
        }
        let scale = 0.05 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.2, 0.4);
        let point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
        let scale2 = scale * 5 / 2 * 10;
        let pos = new Float64Array([point.x - (scale2 * 0.014),point.y + offset]);
        mp.game.graphics.drawSprite("commonmenu", "common_medal", pos[0], pos[1], 0.60 * scale, 0.8 *scale, 0, 0, 204, 153, 255);

        point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
        pos = new Float64Array([point.x ,point.y + offset]);
        scale = 0.05 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.2, 0.4);
        mp.game.graphics.drawSprite("commonmenu", "common_medal", pos[0], pos[1], 0.60 * scale, 0.8 *scale, 0, 0, 204, 153, 255);

        point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(player.position.x, player.position.y, player.position.z + 1.2));
        scale = 0.05 * (0.2 + (1 / ((getDistance(player.position, lplayer.position) / 50.0)) / 50.0)).clamp(0.2, 0.4);
        scale2 = scale * 5 / 2 * 10;
        pos = new Float64Array([point.x + (scale2 * 0.014),point.y + offset]);
        mp.game.graphics.drawSprite("commonmenu", "common_medal", pos[0], pos[1], height * 0.60 * scale, 0.8 *scale * height, 0, 0, 204, 153, 255);
    }
}