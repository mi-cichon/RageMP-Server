let colShapeType = "";
let colShape = null;
let player = mp.players.local;
let iskeyPressed = false;
let isSecondKeyPressed = false;
let houseowner = false;
mp.events.add('render', () => {
    if(colShapeType!= "" && !player.getVariable("controlsblocked")){
        if(mp.keys.isDown(69) && !iskeyPressed){
            iskeyPressed = true;
        }
        if(mp.keys.isUp(69) && iskeyPressed){
            switch(colShapeType)
            {
                case "cartrader":
                    mp.events.callRemote("openCarTraderBrowser");
                    break;
                case "nickname":
                    mp.events.callRemote("openNicknameBrowser");
                    break;
                case "mech":
                    mp.events.callRemote("openMechHUD", colShape);
                    break;
                case "storage":
                    mp.events.call("openStorageHUD", colShape.getVariable("storageid"));
                    break;
                case "changingroom":
                    mp.events.callRemote("openClothes", colShape.getVariable("playerhead"));
                    break;  
                case "paintshop":
                    mp.events.call("openPaintShopBrowser", player.vehicle);
                    break;
                case "teleport":
                    player.position = colShape.getVariable("position");
                    player.setHeading(colShape.getVariable("heading"));
                    break;
                case "lspd-teleport":
                    player.position = colShape.getVariable("position");
                    player.setHeading(colShape.getVariable("heading"));
                    break;
                case "debriscleaner":
                    if(player.getVariable("job") === ""){
                        mp.events.callRemote("startDebrisCleaner");
                    }
                    else{
                        mp.events.callRemote("endJob");
                    }
                    break;
                case "diver":
                    mp.events.callRemote("diver_startJob");
                    break;
                case "warehouse":
                    if(player.getVariable("job") === ""){
                        mp.events.callRemote("startWarehouse");
                    }
                    else{
                        mp.events.callRemote("endJob");
                    }
                    break;
                case "lawnmowing":
                    if(player.getVariable("job") === ""){
                        mp.events.callRemote("startLawnmowing");
                    }
                    else{
                        mp.events.callRemote("endJob");
                    }
                    break;
                case "towtruck":
                    mp.events.callRemote("startTowTrucks");
                    break;
                case "refinery":
                    mp.events.callRemote("refinery_startJob");
                    break;
                case "junkyard":
                    mp.events.callRemote("startJunkyard");
                    break;
                case "hunter":
                    mp.events.callRemote("startHunter");
                    break;
                case "hunter-sell":
                    mp.events.callRemote("sellPelts");
                    break;
                case "gardener":
                    mp.events.callRemote("gardener_startJob");
                    break;
                case "gardener_sellout":
                    mp.events.call("gardener_sellPlants");
                    break;
                case "supplier":
                    mp.events.callRemote("startSupplier");
                    break;
                    
                case "house":
                    if(houseowner)
                    {
                        mp.events.callRemote("enterHouse", colShape);
                    }
                    else
                    {
                        mp.events.call("openHousePanelBrowser", colShape.getVariable("id"), colShape.getVariable("price").toString());
                    }
                    break;
                case "houseout":
                    mp.events.callRemote("leaveHouse", colShape);
                    break;
                case "market1":
                    if(player.vehicle && player.vehicle.getPedInSeat(-1) == player.handle)
                    {
                        mp.events.call("openCarMarketBrowser", player.vehicle.getVariable("name"));
                    }
                    break;
                case "karting":
                    if(!player.vehicle)
                    {
                        mp.events.callRemote("signPlayerForARace", "karting");
                    }
                    break;
                case "carwash":
                    if(player.vehicle)
                    {
                        mp.events.callRemote("startCarWash", player.vehicle, colShape);
                    }
                    break;
                case "fisherman":
                    mp.events.callRemote("buyFishingRod");
                    break;
                case "fishingspot":
                    if(player.getVariable("job") == "")
                        mp.events.callRemote("startFishing");
                    if(player.getVariable("job") == "fisherman")
                        if(doesPlayerFaceWater()){
                            mp.events.call("startFishing", colShape.getVariable("spottype") == "lake" ? 0 : 1);
                        }else{
                            mp.events.call("showNotification", "Nie stoisz przy wodzie!");
                        }
                    break;
                case "fishseller":
                    mp.events.callRemote("sellFishes");
                    break;
                case "fisherpaser":
                    mp.events.callRemote("sellJunk");
                    break;
                case "licenceb":
                    if(player.hasVariable("licenceBt") && !player.getVariable("licenceBt")){
                        mp.events.call("openLicenceBrowser");
                    }
                    else{
                        mp.events.call("showNotification", "Zdałeś już ten egzamin!");
                    }
                    break;
                case "licencebp":
                    if(player.hasVariable("licenceBt") && player.getVariable("licenceBt")){
                        if(player.hasVariable("licenceBp") && player.getVariable("licenceBp")){
                            mp.events.call("showNotification", "Ukończyłeś już ten egzamin!");
                        }
                        else{
                            mp.events.callRemote("startTestLicence");
                        }
                    }
                    else{
                        mp.events.call("showNotification", "Najpierw musisz zdać egzamin teoretyczny!");
                    }
                    break;
                case "cardealer":
                    mp.events.callRemote("openCarDealer");
                    break;
                case "org":
                    mp.events.callRemote("openOrgBrowser");
                    break;
                case "visutune":
                    mp.events.callRemote("openVisuTune");
                    break;
                case "wheelstune":
                    mp.events.callRemote("openWheelsTune");
                    break;
                case "business-tune":
                    mp.events.callRemote("buyTuneBusinessHUD", colShape.getVariable("bus-id"));
                    break;
                case "business-wheels-station":
                    if(!player.getVariable("gui"))
                    {
                        mp.events.callRemote("openBusinessWheelsStation", player.getVariable("business-id"));
                    }
                    break;
                case "business-mech-station":
                    if(!player.getVariable("gui"))
                    {
                        mp.events.callRemote("openBusinessMechStation", player.getVariable("business-id"));
                    }
                    break;
                case "housestorage":
                    if(player.hasVariable("houseid") && colShape.getVariable("houseid") == player.getVariable("houseid")){
                        mp.events.callRemote("openHouseStorage", colShape.getVariable("houseid"));
                    }
                    break;
                case "forklifts":
                    mp.events.callRemote("startForklifts");
                    break;
                case "itemShop":
                    mp.events.call("openItemShopBrowser");
                    break;
                case "atm":
                    mp.events.call("openBankingBrowser", player.getVariable("bank"));
                    break;
                case "lspd_duty":
                    mp.events.callRemote("lspd_StartDuty");
                    break;
                case "lspd_storage":
                    mp.events.callRemote("lspd_openStorage");
                    break;
                case "gardener_plant":
                    mp.events.callRemote("gardener_pickupPlant", colShape.getVariable("plantId"), colShape.getVariable("groundId"));
                    break;
                case "sellveh":
                    mp.events.callRemote("sellVeh_open");
                    break;
            }
            if(colShapeType != "fishingspot" && colShapeType != "business-wheels-station" && colShapeType != "business-mech-station"){
                colShapeType = "";
                colShape = null;
            }
            iskeyPressed = false;
        }
        if(mp.keys.isDown(82) && !isSecondKeyPressed){
            isSecondKeyPressed = true;
        }
        if(mp.keys.isUp(82) && isSecondKeyPressed){
            switch(colShapeType)
            {
                case "house":
                    if(houseowner)
                    {
                        mp.events.call("openOwnHousePanelBrowser", colShape.getVariable("id"), colShape.getVariable("time"), colShape.getVariable("price"));
                    }
                    break;
                case "droppeditem":
                    mp.events.callRemote("pickItemUp", colShape.getVariable("id"));
                    break;
            }
            colShapeType = "";
            colShape = null;
            isSecondKeyPressed = false;
        }
    }
});

mp.events.add('playerEnterColshape', (shape) => {
    if(player.getVariable("job") === "" && shape.hasVariable("type") && player.vehicle == null){
        let type = shape.getVariable("type");
        switch(type)
        {
            case "lawnmowing":
                if(player.getVariable("job") == "")
                {
                    colShapeType = type;
                    colShape = shape;   
                }
                else{
                    colShapeType = type;
                    colShape = shape;   
                }
                break;
            case "debriscleaner":
                if(player.getVariable("job") == "")
                {
                    colShapeType = type;
                    colShape = shape;   
                    mp.events.call("showNotification", "Naciśnij E aby rozpocząć pracę zbierania odpadów!");
                }
                break;
            case "warehouse":
                if(player.getVariable("job") == "")
                {
                    colShapeType = type;
                    colShape = shape;   
                }
                else{
                    colShapeType = type;
                    colShape = shape;   
                }
                break;
            case "towtruck":
                colShapeType = type;
                colShape = shape;
                break;
            case "junkyard":
                colShapeType = type;
                colShape = shape;
                break;
            case "refinery":
                colShapeType = type;
                colShape = shape;
                break;
            case "supplier":
                colShapeType = type;
                colShape = shape;
                break;
            case "forklifts":
                colShapeType = type;
                colShape = shape;
                break;
            case "gardener":
                colShapeType = type;
                colShape = shape;
                break;
        } 
    }
    if(shape.hasVariable("type") && player.vehicle == null){
        let type = shape.getVariable("type");
        switch(type){
            case "stationveh":
                if(player.vehicle)
                    mp.events.call("showNotification", "Zatrzymaj tu pojazd i użyj pobliskiego dystrybutora aby zatankować!");
                break;
            case "cartrader":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby otworzyć okno sprzedaży pojazdu!");
                colShape = shape;
                break;
            case "nickname":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby otworzyć panel zmiany nicku!");
                colShape = shape;
                break;
            case "mech":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby porozmawiać z mechanikiem!");
                colShape = shape;
                break;
            case "storage":
                colShapeType = type;
                colShape = shape;
                break;
            case "changingroom":
                colShapeType = type;
                colShape = shape;
                break;
            case "teleport":
                colShapeType = type;
                colShape = shape;
                break;
            case "lspd-teleport":
                if(player.hasVariable("lspd_power") && player.getVariable("lspd_power") > 0){
                    colShapeType = type;
                    colShape = shape;
                }
                break;
            case "house":
                if(shape.hasVariable("owner") && player.hasVariable("socialclub") && shape.getVariable("owner") === player.getVariable("socialclub").toString())                 
                {
                    mp.events.call("showNotification", "Naciśnij E aby wejść do domu lub R aby zobaczyć panel!")
                    houseowner = true;
                    colShapeType = type;
                    colShape = shape;
                }
                else if(shape.getVariable("owner") === "")
                {
                    mp.events.call("showNotification", "Naciśnij E aby zobaczyć panel domu!")
                    colShapeType = type;
                    colShape = shape;
                    houseowner = false;
                }              
                break;
            case "houseout":
                colShapeType = type;
                colShape = shape;
                break;
            case "housestorage":
                colShapeType = type;
                colShape = shape;
                break;
            case "karting":
                colShapeType = type;
                colShape = shape;
                break;
            case "hunter":
                colShapeType = type;
                colShape = shape;
                break;
            case "diver":
                colShapeType = type;
                colShape = shape;
                break;
            case "hunter-sell":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby sprzedać skóry myśliwemu!")
                colShape = shape;
                break;
            case "fisherman":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby zakupić wędkę ($3000)!")
                colShape = shape;
                break;
            case "fishingspot":
                colShapeType = type;
                mp.events.call("showNotification", "Wszedłeś na teren łowiecki! Nacisnij E aby zacząć łowić!");
                colShape = shape;
                break;
            case "fishseller":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby sprzedać ryby!");
                colShape = shape;
                break;
            case "fisherpaser":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby sprzedać złowione przedmioty!");
                colShape = shape;
                break;
            case "droppeditem":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij R aby podnieść przedmiot!");
                colShape = shape;
                break;
            case "licenceb":
                colShapeType = type;
                colShape = shape;
                break;
            case "licencebp":
                colShapeType = type;
                colShape = shape;
                break;
            case "cardealer":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby otworzyć panel zakupu pojazdu!");
                colShape = shape;
                break;
            case "org":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby otworzyć panel organizacji!");
                colShape = shape;
                break;
            case "business-tune":
                colShapeType = type;
                colShape = shape;
                break;
            case "business-wheels-station":
                if(player.hasVariable("business-id") && shape.getVariable("business-id") == player.getVariable("business-id")){
                    colShapeType = type;
                    colShape = shape;
                    mp.events.call("showNotification", "Naciśnij E aby otworzyć panel montażu felg!");
                }
                break;
            case "business-mech-station":
                if(player.hasVariable("business-id") && shape.getVariable("business-id") == player.getVariable("business-id")){
                    colShapeType = type;
                    colShape = shape;
                    mp.events.call("showNotification", "Naciśnij E aby otworzyć panel tuningu mechanicznego!");
                }
                break;
            case "itemShop":
                colShapeType = type;
                colShape = shape;
                mp.events.call("showNotification", "Naciśnij E aby otworzyć sklep z przedmiotami!");
                break;
            case "atm":
                colShapeType = type;
                colShape = shape;
                mp.events.call("showNotification", "Naciśnij E aby skorzystać z bankomatu!");
                break;
            case "lspd_duty":
                if(player.hasVariable("lspd_power") && player.getVariable("lspd_power") > 0){
                    colShapeType = type;
                    colShape = shape;
                    mp.events.call("showNotification", "Naciśnij E aby rozpocząć/zakończyć służbę!");
                }
                break;
            case "lspd_storage":
                if(player.hasVariable("lspd_duty") && player.getVariable("lspd_duty")){
                    colShapeType = type;
                    colShape = shape;
                    mp.events.call("showNotification", "Naciśnij E aby otworzyć przechowalnię pojazdów frakcyjnych");
                }
                break;
            case "gardener_plant":
                if(player.getVariable("job") == "gardener"){
                    colShapeType = type;
                    colShape = shape;
                    mp.events.call("showNotification", "Naciśnij E aby zebrać roślinę!");
                }
                break;
            case "gardener_sellout":
                if(player.getVariable("job") == "gardener"){
                    colShapeType = type;
                    colShape = shape;
                    mp.events.call("showNotification", "Naciśnij E aby oddać rośliny w ramach zlecenia!");
                }
                break;
        }
    }
    else if(player.vehicle != null && player.getVariable("vehSeat") == 0)
    {
        let type = shape.getVariable("type");
        switch(type)
        {
            case "mechstation":
                if(player.vehicle.hasVariable("damage")){
                    mp.events.call("showNotification", "Zaparkuj pojazd na stanowisku i porozmawiaj z mechanikiem!");
                }
                break;
            case "paintshop":
                if(player.vehicle != null && player.vehicle.hasVariable("owner") && player.vehicle.getVariable("owner").toString() === player.getVariable("socialclub")){
                    colShapeType = type;
                    mp.events.call("showNotification", "Naciśnij E aby otworzyć lakiernię!");
                    colShape = shape;
                }
                break;
            case "market1":
                if(player.vehicle && player.vehicle.hasVariable("owner") && player.vehicle.getVariable("owner").toString() === player.getVariable("socialclub").toString()){
                    colShapeType = type;
                    mp.events.call("showNotification", "Naciśnij E aby wystawić pojazd na giełdę!");
                    colShape = shape;
                }
                break;
            case "carwash":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby umyć pojazd (100$)!");
                colShape = shape;
                break;
            case "visutune":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby otworzyć panel tuningu wizualnego pojazdu!");
                colShape = shape;
                break;
            case "sellveh":
                colShapeType = type;
                mp.events.call("showNotification", "Naciśnij E aby otworzyć panel sprzedaży pojazdu!");
                colShape = shape;
                break;
        }
    }
});

mp.events.add('playerExitColshape', (shape) => {
    colShapeType = "";
    colShape = null;
    if(shape.hasVariable("type") && shape.getVariable("type") == "fishingspot" && player.getVariable("job") == "fisherman"){
        mp.events.callRemote("stopFisherman");
    }
});

function doesPlayerFaceWater(){
    if(!player.isInWater()){
        let position = player.position;
        let direction = player.getForwardVector();
        let distance = 15;
        let farAway = new mp.Vector3((direction.x * distance) + (position.x), (direction.y * distance) + (position.y), (direction.z * distance) + (position.z + 2) - 25);
        let res = mp.game.water.testProbeAgainstWater(player.position.x, player.position.y, player.position.z+2, farAway.x, farAway.y, farAway.z, true);
        if(res){
            return true;
        }
        else{
            return false;
        }
    }
    else{
        return false;
    }
}