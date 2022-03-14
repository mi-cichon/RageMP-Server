let carTraderBrowser = null;
let player = mp.players.local;
let sellPlayer = null;
let selledCarId = null;
let carPrice = null;
let sold = false;
let offer = false;
mp.events.add("openCarTraderBrowser", (playerString, vehiclesString) => {
    if(carTraderBrowser == null && !player.getVariable("gui")){
        if(offer){
            mp.events.call("showNotification", "Ofertę można składać tylko raz na minutę!");
        }
        else{
            carTraderBrowser = mp.browsers.new("package://CarTrader/index.html");
            sendData(playerString, true);
            sendData(vehiclesString, false);
            mp.gui.cursor.show(true, true);
            mp.events.callRemote("setGui", true);
        }
    }
});

mp.events.add("closeCarTraderBrowser", () => {
    carTraderBrowser.destroy();
    carTraderBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
});

function sendData(dataString, player){

    if(dataString != "" && player){
        let players = JSON.parse(dataString);
        Object.entries(players).forEach(([key, value]) => {
            carTraderBrowser.execute(`addPlayer('${key.toString()}','${players[key].toString()}');`);
         });
    }
    else if(dataString != ""){
        let vehicles = JSON.parse(dataString);
        vehicles.forEach(vehicle => {
            carTraderBrowser.execute(`addCar('${vehicle[0]}','${vehicle[1]}');`);
         });
    }


    // if(dataString){
    //     if(dataString.includes(';')){
    //         dataString.split(';').forEach((element) => {
    //             let id = element.split(':')[0];
    //             let name = element.split(':')[1];
    //             if(player){
    //                 carTraderBrowser.execute(`addPlayer('${id}','${name}');`);
    //             }
    //             else{
    //                 carTraderBrowser.execute(`addCar('${name}','${id}');`);
    //             }
    //         });
    //     }
    //     else{
    //         let id = dataString.split(':')[0];
    //         let name = dataString.split(':')[1];
    //         if(player){
    //             carTraderBrowser.execute(`addPlayer('${id}','${name}');`);
    //         }
    //         else{
    //             carTraderBrowser.execute(`addCar('${name}','${id}');`);
    //         }
    //     }
    // }
    
}

mp.events.add("sendTrade", (playerId, carId, price) =>{
    if(price > 0) {
        mp.events.callRemote("sendTrade", playerId, carId, price);
        mp.events.call("closeCarTraderBrowser");
        offer = true;
        setTimeout(() => {
            offer = false;

        }, 60000);
    }
    else{
        carTraderBrowser.execute(`showError('Podałeś błędną cenę!')`);
    }
        
});

mp.events.add("sendCarTrade", (seller, carName, carId, price) => {
    mp.events.callRemote("sendInfoMessage", `Gracz ${seller.getVariable("username")} wysyła Ci ofertę sprzedaży ${carName} o ID: ${carId} za ${price.toString()}$. Aby ją zaakceptować wpisz /akceptuj ${carId} ${price.toString()}`);
    sellPlayer = seller;
    selledCarId = parseInt(carId);
    carPrice = price;
    setTimeout(() => {
        sellPlayer = null;
        selledCarId = null;
        carPrice = null;
        if(!sold)
        {
            mp.events.callRemote("sendInfoMessage", `Czas na akceptację oferty minął!`);
            mp.events.callRemote("setTradeOffer", false);
        }
        else{
            sold = true;
        }
           
    }, 30000)
});

mp.events.add("acceptCarTrade", (carId, price) => {
    if(sellPlayer == null){
        mp.events.call("showNotification", "Nie masz żadnych oczekujących ofert sprzedaży!");
    }
    else{
        if(carId == selledCarId.toString() && price == carPrice.toString()){
            mp.events.callRemote("confirmCarTrade", sellPlayer, selledCarId, carPrice);
            sellPlayer = null;
            selledCarId = null;
            carPrice = null;
            sold = true;
        }
        else{
            mp.events.call("showNotification", "Podałeś błędne dane!");
        }
    }
});

  function getHour(){
      let date = new Date();
      return (date.getHours() < 10 ? "0" + date.getHours() : date.getHours()).toString() + ":" + (date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes());
  }