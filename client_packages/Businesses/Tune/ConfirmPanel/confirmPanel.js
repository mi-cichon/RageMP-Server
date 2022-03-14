let type;

function setVars(name, price, state, t){
    type = t;
    if(state){
        $(".name").text("Montaż " + name);
        $(".price").text("-" + price + "$");
        $(".price").addClass("bad");
    }
    else{
        {
            $(".name").text("Demontaż " + name);
            $(".price").text("+" + price + "$");
            $(".price").addClass("good");
        }
    }
}

$("#close").on("click", function(){
    if(type == "wheels")
        mp.trigger("declineWheelsTuneOffer");
    if(type == "mech")
        mp.trigger("declineMechTuneOffer");
    mp.trigger("closeConfirmWheelsTunePanel");
});

$("#confirm").on("click", function() {
    if(type == "wheels")
        mp.trigger("acceptWheelsTuneOffer");
    if(type == "mech")
        mp.trigger("acceptMechTuneOffer");
    mp.trigger("closeConfirmWheelsTunePanel")
});