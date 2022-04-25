let equipmentBrowser = null;
let secondEquipmentBrowser = null;
let player = mp.players.local;

let equipment = false;
mp.events.add("openEquipmentBrowser", (items, additionalLines) => {
    // if(secondEquipmentBrowser){
    //     secondEquipmentBrowser.destroy();
    //     secondEquipmentBrowser = null;
    // }
    if(secondEquipmentBrowser){
        secondEquipmentBrowser.destroy();
        secondEquipmentBrowser = null;
    }
    if(equipmentBrowser){
        equipmentBrowser.destroy();
        equipmentBrowser = null;
    }
    equipmentBrowser = mp.browsers.new('package://Equipment//index.html');
    equipmentBrowser.execute(`equipmentGrid.changeSize(${additionalLines})`);
    setTimeout(function(){
        equipmentBrowser.execute(`instantiateItems('${items}')`);
    },1000);
    
});

mp.events.add("updateEquipment", updatedString => {
    mp.events.callRemote("updateEquipment", updatedString);
});

mp.events.add("useItem", (typeId, itemId) => {
    mp.events.callRemote("useItem", typeId, itemId);
});

mp.events.add("dropItem", (typeId, name) => {
    mp.events.callRemote("dropItem", typeId, name);
});

mp.events.add("changeEquipmentSize", size => {
    if(equipmentBrowser){
        equipmentBrowser.execute(`equipmentGrid.changeSize(${size})`);
        setTimeout(()=>{
            equipmentBrowser.execute(`refreshEquipment('${player.getVariable("equipment")}')`);
        }, 1000)
    }
});

mp.events.add("closeEquipmentBrowser", () => {
    if(equipmentBrowser){
        equipmentBrowser.destroy();
        equipmentBrowser = null;
    }
});

mp.events.add("switchEquipmentBrowser", (updatedString) => {
    if(secondEquipmentBrowser){
        secondEquipmentBrowser.destroy();
        secondEquipmentBrowser = null;
        mp.gui.cursor.show(false,false);
        mp.events.callRemote("setGui", false);
    }
    if(equipmentBrowser){
        if(!equipment && !player.getVariable("gui")){
            equipment = !equipment;
            equipmentBrowser.execute(`switchEquipment(${equipment})`);
            mp.events.callRemote("setGui", true);
        }
        else if(equipment){
            equipment = !equipment;
            equipmentBrowser.execute(`switchEquipment(${equipment})`);
            mp.events.callRemote("setGui", false);
        }
    }
});

mp.events.add("saveEquipment", () => {
    if(equipmentBrowser){
        equipmentBrowser.execute('updateEquipment()');
    }
});

mp.events.add("refreshEquipment", (eqString) => {
    if(equipmentBrowser){
        equipmentBrowser.execute(`refreshEquipment('${eqString}')`);
    }
});
mp.events.add("addItemToEquipment", itemId => {
    if(equipmentBrowser){
        equipmentBrowser.execute(`addItem(${itemId})`);
    }
});

mp.events.add("fitItemInEquipment", (itemType) => {
    if(equipmentBrowser){
        equipmentBrowser.execute(`fitItem(${itemType}, equipmentGrid)`);
    }
});

mp.events.add("checkIfItemFits", (itemId, type) => {
    if(equipmentBrowser){
        equipmentBrowser.execute(`doesItemFit(${itemId}, '${type}')`);
    }
});

mp.events.add("itemFit", (state, itemType) => {
    mp.events.callRemote("doesItemFit", state, itemType);
});

mp.events.add("itemWillFit", (state, itemType, type) => {
    mp.events.callRemote("willItemFit", state, itemType, type);
});

mp.events.add("removeEqItem", index => {
    if(equipmentBrowser){
        equipmentBrowser.execute(`removeItem(${index}, equipmentGrid)`);
    }
    if(secondEquipmentBrowser){
        setTimeout(function(){
            secondEquipmentBrowser.execute(`refreshEquipment('${mp.players.local.getVariable("equipment")}', false)`);
        }, 300);
    }
});
mp.events.add("logInfo", text => {
    mp.console.logInfo(text.toString());
});

mp.events.add("openSecondEquipmentBrowser", (items, items2, eqId, trunksize) => {
    if(equipment){
        equipment = !equipment;
        equipmentBrowser.execute(`switchEquipment(${equipment})`);
        mp.events.callRemote("setGui", false);
    }
    if(mp.browsers.exists(secondEquipmentBrowser)){
        secondEquipmentBrowser.destroy();
        secondEquipmentBrowser = null;
        mp.gui.cursor.show(false,false);
        mp.events.callRemote("setGui", false);
    }
    else if(!player.getVariable("gui")){
        mp.gui.cursor.show(true,true);
        secondEquipmentBrowser = mp.browsers.new('package://Equipment//index2.html');
        secondEquipmentBrowser.execute(`createGrids(${player.getVariable("skill-0").toString()}, '${trunksize}')`);
        secondEquipmentBrowser.execute(`instantiateItems('${items}', '${items2}', '${eqId}')`);
        mp.events.callRemote("setGui", true);
    }
    
});

mp.events.add("updateEquipments", (updatedString, updatedString2, eqId) => {
    if((eqId == "gardener" || eqId.includes("v")) && !player.vehicle){
        secondEquipmentBrowser.destroy();
        secondEquipmentBrowser = null;
        mp.gui.cursor.show(false,false);
        mp.events.callRemote("setGui", false);
        mp.events.call("openTrollBrowser", "rick");
    }
    else{
        mp.events.callRemote("updateEquipments", updatedString, updatedString2, eqId, player.vehicle ? player.vehicle : null);
        if(equipmentBrowser)
            equipmentBrowser.execute(`refreshEquipment('${updatedString}')`);
    }
});

mp.events.add("closeSecondEquipmentBrowser", () => {
    if(secondEquipmentBrowser){
        secondEquipmentBrowser.destroy();
        secondEquipmentBrowser = null;
        mp.gui.cursor.show(false,false);
        mp.events.callRemote("setGui", false);
    }
});

mp.events.add("toggleMouse", state => {
    mp.gui.cursor.show(state, state);
});

mp.events.add("setGui", state => {
    mp.events.callRemote("setGui", state);
});

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(secondEquipmentBrowser)
    {
        secondEquipmentBrowser.destroy();
        secondEquipmentBrowser = null;
        mp.gui.cursor.show(false,false);
        mp.events.callRemote("setGui", false);
        
    }
});