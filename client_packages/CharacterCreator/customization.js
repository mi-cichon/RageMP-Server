

const player = mp.players.local;
let customizationCEF = null, 
    customizatorOpened = false,
    bodyCam = null,
    bodyCamStart = null;

const genders = [
    "mp_m_freemode_01",
    "mp_f_freemode_01"
  ];

getCameraOffset = (pos, angle, dist) => {
    angle = angle * 0.0174533;
    pos.y = pos.y + dist * Math.sin(angle);
    pos.x = pos.x + dist * Math.cos(angle);
    return pos;
}

mp.events.add({

    'client:creator.show': () => {
        player.freezePosition(true);
        if (customizatorOpened == true) return;
        customizationCEF = mp.browsers.new('package://CharacterCreator/creator-interface/creator.html');
        setTimeout(() => { mp.gui.cursor.show(true, true); }, 500);
        mp.game.ui.displayRadar(false);
        mp.events.call('client:creator.cam', true);
        customizatorOpened = true;
        setTimeout(function(){
            mp.players.local.model = mp.game.joaat(genders[1]);
            mp.players.local.model = mp.game.joaat(genders[0]);
        }, 1000);
    },

    'client:creator.cam': (toggle) => {
        if (toggle) {
            bodyCamStart = player.position;
            let camValues = { Angle: player.getRotation(2).z + 90, Dist: 2.6, Height: 0.2 };
            let pos = getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), camValues.Angle, camValues.Dist);
            bodyCam = mp.cameras.new('default', pos, new mp.Vector3(0, 0, 0), 50);
            bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
            bodyCam.setActive(true);
            mp.game.cam.renderScriptCams(true, false, 500, true, false);
        }
        else {
            if (bodyCam == null) return;
            bodyCam.setActive(false);
            bodyCam.destroy();
            mp.game.cam.renderScriptCams(false, false, 3000, true, true);
            
            bodyCam = null;
        }
	    player.taskPlayAnim("amb@world_human_guard_patrol@male@base", "base", 8.0, 1, -1, 1, 0.0, false, false, false);
    },

    'client:creator.cam.set': (flag) => {
        let camValues = { Angle: 0, Dist: 1, Height: 0.2 };
        switch(flag)
        {
            case 0: // Torso
            {
                camValues = { Angle: 0, Dist: 2.6, Height: 0.2 };
                break;
            }
            case 1: // Head
            {
                camValues = { Angle: 0, Dist: 1, Height: 0.5 };
                break;
            }
            case 2: // Hair / Bear / Eyebrows
            {
                camValues = { Angle: 0, Dist: 0.5, Height: 0.7 };
                break;
            }
            case 3: // chesthair
            {
                camValues = { Angle: 0, Dist: 1, Height: 0.2 };
                break;
            }
        }
	    const camPos = getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), player.getRotation(2).z + 90 + camValues.Angle, camValues.Dist);
        bodyCam.setCoord(camPos.x, camPos.y, camPos.z);
	    bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
    },

    'client:creator.reload': () => { 
        let shirt = player.getNumberOfDrawableVariations(11), bottom = player.getNumberOfDrawableVariations(4), shoes = player.getNumberOfDrawableVariations(6);
        customizationCEF.execute(`customization.max([${shirt}, ${bottom}, ${shoes}]);`);
    },

    'client:creator.finish': (character) => { 
        mp.events.call('client:creator.cam', false);
        if (mp.browsers.exists(customizationCEF)) { customizationCEF.destroy() }
        customizatorOpened = false;
        mp.events.callRemote('createCharacter', JSON.stringify(character));
        mp.game.ui.displayRadar(true);
        setTimeout(() => { mp.gui.cursor.show(false, false); }, 500);
        player.freezePosition(false);
        mp.events.call('client:hud.show', true);
    },

    'client:creator.preview': (x, data, character) => { 
        data = JSON.parse(data);
        character = JSON.parse(character);
        switch (x) { 
            case 'hair': {
                mp.events.call('client:creator.cam.set', 2);
                if (data[0] == 23 || data[0] == 24) return false;
                player.setComponentVariation(2, parseInt(data[0]), 0, 0);
                player.setHairColor(parseInt(data[1]), parseInt(data[2]));
                break;
            }
            case 'faceFeatures': { 
                mp.events.call('client:creator.cam.set', 2);
                player.setFaceFeature(parseInt(data[0]), parseFloat(data[1]));
                break;
            }
            case 'gender': { 
                mp.players.local.model = mp.game.joaat(genders[data]);
                break;
            }
            case 'beard': {
                mp.events.call('client:creator.cam.set', 2);
                player.setHeadOverlay(1, parseInt(data[0]), 1, parseInt(data[1]), 0);
                break;
            }
            case 'blendData': { 
                player.setHeadBlendData(parseInt(data[0]), parseInt(data[1]), 0, parseInt(data[2]), parseInt(data[3]), 0, parseFloat(data[4]), parseFloat(data[5]), 0, false);
                mp.events.call('client:creator.cam.set', 1);
                break;
            }
            case 'headOverlays': { 
                mp.events.call('client:creator.cam.set', 2);
                player.setHeadOverlay(0, parseInt(data[0]), 1.0, 0, 0);
                player.setHeadOverlay(2, parseInt(data[1]), 1.0, 0, 0);
                player.setHeadOverlay(3, parseInt(data[2]), 1.0, 0, 0);
                player.setHeadOverlay(6, parseInt(data[5]), 1.0, 0, 0);
                player.setHeadOverlay(7, parseInt(data[6]), 1.0, 0, 0);
                player.setHeadOverlay(9, parseInt(data[8]), 1.0, 0, 0);
                if (player.model == mp.game.joaat(genders[1])) { 
                    player.setHeadOverlay(5, parseInt(data[4]), 1.0, 0, 0);
                    player.setHeadOverlay(8, parseInt(data[7]), 1.0, 0, 0);
                    player.setHeadOverlay(4, parseInt(data[3]), 1.0, 0, 0);
                }
                break;
            }
            case 'headOverlaysColors': { 
                mp.events.call('client:creator.cam.set', 2);
                player.setHeadOverlay(0, parseInt(character.headOverlays[0]), 1.0, parseInt(data[0]), 0);
                player.setHeadOverlay(2, parseInt(character.headOverlays[1]), 1.0, parseInt(data[1]), 0);
                player.setHeadOverlay(3, parseInt(character.headOverlays[2]), 1.0, parseInt(data[2]), 0);
                player.setHeadOverlay(6, parseInt(character.headOverlays[5]), 1.0, parseInt(data[5]), 0);
                player.setHeadOverlay(7, parseInt(character.headOverlays[6]), 1.0, parseInt(data[6]), 0);
                player.setHeadOverlay(9, parseInt(character.headOverlays[8]), 1.0, parseInt(data[8]), 0);
                if (player.model == mp.game.joaat(genders[1])) { 
                    player.setHeadOverlay(5, parseInt(character.headOverlays[4]), 1.0, parseInt(data[4]), 0);
                    player.setHeadOverlay(8, parseInt(character.headOverlays[7]), 1.0, parseInt(data[7]), 0);
                    player.setHeadOverlay(4, parseInt(character.headOverlays[3]), 1.0, parseInt(data[3]), 0);
                }
                break;
            }
        }
    },

    'render': () => { 
        if (customizatorOpened) { 
            
        }
    },
    
    'createCharacter': () => {
        if(player.hasVariable("character")){
            let character = JSON.parse(player.getVariable("character"));
            
            if(mp.players.local.model != mp.game.joaat(genders[parseInt(character.gender)]))
                mp.players.local.model = mp.game.joaat(genders[parseInt(character.gender)]);

            player.setHeadBlendData(parseInt(character.blendData[0]), parseInt(character.blendData[1]), 0, parseInt(character.blendData[2]), parseInt(character.blendData[3]), 0, parseFloat(character.blendData[4]), parseFloat(character.blendData[5]), 0, false);
            player.setHeadOverlay(0, parseInt(character.headOverlays[0]), 1.0, parseInt(character.headOverlaysColors[0]), 0);
            player.setHeadOverlay(2, parseInt(character.headOverlays[1]), 1.0, parseInt(character.headOverlaysColors[1]), 0);
            player.setHeadOverlay(3, parseInt(character.headOverlays[2]), 1.0, parseInt(character.headOverlaysColors[2]), 0);
            player.setHeadOverlay(6, parseInt(character.headOverlays[5]), 1.0, parseInt(character.headOverlaysColors[5]), 0);
            player.setHeadOverlay(7, parseInt(character.headOverlays[6]), 1.0, parseInt(character.headOverlaysColors[6]), 0);
            player.setHeadOverlay(9, parseInt(character.headOverlays[8]), 1.0, parseInt(character.headOverlaysColors[8]), 0);
            if(character.beard[0] != 0){
                player.setHeadOverlay(1, parseInt(character.beard[0]), 1, parseInt(character.beard[1]), 0);
            }
            else{
                player.setHeadOverlay(1, 0, 0, 0, 0);
            }
            
            if (player.model == mp.game.joaat(genders[1])) { 
                player.setHeadOverlay(5, parseInt(character.headOverlays[4]), 1.0, parseInt(character.headOverlaysColors[4]), 0);
                player.setHeadOverlay(8, parseInt(character.headOverlays[7]), 1.0, parseInt(character.headOverlaysColors[7]), 0);
                player.setHeadOverlay(4, parseInt(character.headOverlays[3]), 1.0, parseInt(character.headOverlaysColors[3]), 0);
                player.setHeadOverlay(1, 0, 0, 0, 0);
            }
            player.setComponentVariation(2, parseInt(character.hair[0]), 0, 0);
            player.setHairColor(parseInt(character.hair[1]), parseInt(character.hair[2]));
            for (var i = 0; i < character.faceFeatures.length; i++){
                player.setFaceFeature(i, parseFloat(character.faceFeatures[i]));
            }
        }
    }
});

mp.events.addDataHandler("character", (entity, value, oldvalue) => {
    if(entity.exists && entity.type === "player" && isJson(entity.getVariable("character").replace(/\\/g, ""))){
        let character = JSON.parse(entity.getVariable("character").replace(/\\/g, ""));
        entity.setHeadBlendData(parseInt(character.blendData[0]), parseInt(character.blendData[1]), 0, parseInt(character.blendData[2]), parseInt(character.blendData[3]), 0, parseFloat(character.blendData[4]), parseFloat(character.blendData[5]), 0, false);
        entity.setHeadOverlay(0, parseInt(character.headOverlays[0]), 1.0, parseInt(character.headOverlaysColors[0]), 0);
        entity.setHeadOverlay(2, parseInt(character.headOverlays[1]), 1.0, parseInt(character.headOverlaysColors[1]), 0);
        entity.setHeadOverlay(3, parseInt(character.headOverlays[2]), 1.0, parseInt(character.headOverlaysColors[2]), 0);
        entity.setHeadOverlay(6, parseInt(character.headOverlays[5]), 1.0, parseInt(character.headOverlaysColors[5]), 0);
        entity.setHeadOverlay(7, parseInt(character.headOverlays[6]), 1.0, parseInt(character.headOverlaysColors[6]), 0);
        entity.setHeadOverlay(9, parseInt(character.headOverlays[8]), 1.0, parseInt(character.headOverlaysColors[8]), 0);
        if (entity.model == mp.game.joaat(genders[1])) { 
            entity.setHeadOverlay(5, parseInt(character.headOverlays[4]), 1.0, parseInt(character.headOverlaysColors[4]), 0);
            entity.setHeadOverlay(8, parseInt(character.headOverlays[7]), 1.0, parseInt(character.headOverlaysColors[7]), 0);
            entity.setHeadOverlay(4, parseInt(character.headOverlays[3]), 1.0, parseInt(character.headOverlaysColors[3]), 0);
        }
        if(character.beard[0] != 0){
            entity.setHeadOverlay(1, parseInt(character.beard[0]), 1, parseInt(character.beard[1]), 0);
        }
        else{
            entity.setHeadOverlay(1, 0, 0, 0, 0);
        }
        entity.setComponentVariation(2, parseInt(character.hair[0]), 0, 0);
        entity.setHairColor(parseInt(character.hair[1]), parseInt(character.hair[2]));
        for (var i = 0; i < character.faceFeatures.length; i++){
            entity.setFaceFeature(i, parseFloat(character.faceFeatures[i]));
        }   
        setTimeout(function(){
            if(character.gender == 1){
                if(entity.model != mp.game.joaat(genders[1])){
                    entity.model = mp.game.joaat(genders[parseInt(1)]);
                    mp.events.callRemote("loadPlayersClothes", entity);
                }     
                entity.setHeadOverlay(1, 0, 0, 0, 0);    
            }
        },500);     
    }
});

mp.events.add("entityStreamIn", entity => {
    if(entity.type === "player" && entity.hasVariable("character") && isJson(entity.getVariable("character").replace(/\\/g, ""))){
        let character = JSON.parse(entity.getVariable("character").replace(/\\/g, ""));
        entity.setHeadBlendData(parseInt(character.blendData[0]), parseInt(character.blendData[1]), 0, parseInt(character.blendData[2]), parseInt(character.blendData[3]), 0, parseFloat(character.blendData[4]), parseFloat(character.blendData[5]), 0, false);
        entity.setHeadOverlay(0, parseInt(character.headOverlays[0]), 1.0, parseInt(character.headOverlaysColors[0]), 0);
        entity.setHeadOverlay(2, parseInt(character.headOverlays[1]), 1.0, parseInt(character.headOverlaysColors[1]), 0);
        entity.setHeadOverlay(3, parseInt(character.headOverlays[2]), 1.0, parseInt(character.headOverlaysColors[2]), 0);
        entity.setHeadOverlay(6, parseInt(character.headOverlays[5]), 1.0, parseInt(character.headOverlaysColors[5]), 0);
        entity.setHeadOverlay(7, parseInt(character.headOverlays[6]), 1.0, parseInt(character.headOverlaysColors[6]), 0);
        entity.setHeadOverlay(9, parseInt(character.headOverlays[8]), 1.0, parseInt(character.headOverlaysColors[8]), 0);
        if (entity.model == mp.game.joaat(genders[1])) { 
            entity.setHeadOverlay(5, parseInt(character.headOverlays[4]), 1.0, parseInt(character.headOverlaysColors[4]), 0);
            entity.setHeadOverlay(8, parseInt(character.headOverlays[7]), 1.0, parseInt(character.headOverlaysColors[7]), 0);
            entity.setHeadOverlay(4, parseInt(character.headOverlays[3]), 1.0, parseInt(character.headOverlaysColors[3]), 0);
        }
        if(character.beard[0] != 0){
            entity.setHeadOverlay(1, parseInt(character.beard[0]), 1, parseInt(character.beard[1]), 0);
        }
        else{
            entity.setHeadOverlay(1, 0, 0, 0, 0);
        }
        entity.setComponentVariation(2, parseInt(character.hair[0]), 0, 0);
        entity.setHairColor(parseInt(character.hair[1]), parseInt(character.hair[2]));
        for (var i = 0; i < character.faceFeatures.length; i++){
            entity.setFaceFeature(i, parseFloat(character.faceFeatures[i]));
        }
        setTimeout(function(){
            if(character.gender == 1){
                if(entity.model != mp.game.joaat(genders[1])){
                    entity.model = mp.game.joaat(genders[parseInt(1)]);
                    mp.events.callRemote("loadPlayersClothes", entity);
                }         
                entity.setHeadOverlay(1, 0, 0, 0, 0);
            }
        },500);     
    } 
});

function isJson(str) {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}