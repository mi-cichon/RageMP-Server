let currentWheels = [];
let selectedWheelToInstall = [];
function setOwnedWheels(data, vehClass){
    data = JSON.parse(data);
    for(let i = 0; i < data.length; i++){
        let partId = data[i][0]
        let name = data[i][1];
        let type = data[i][2];
        let id = data[i][3];
        let price = data[i][4];
        $(`.owned-wheels`).append(`
        <div class="owned-wheel" id="wheel-${type}-${id}"><p>${name}</p><img src="../ManageTune/img/${type}_${id}.png"><p>$${price}</p></div>
        `);
        $(`.owned-wheel#wheel-${type}-${id}`).on("click", function(){
            if((vehClass != 8 && type != "6") || (vehClass == 8 && type == "6")){
                $("*").removeClass("selected");
                $(this).addClass("selected");
                selectedWheelToInstall = ["install", name, type, id, price, partId];
                $(".offerInput").attr("max", (parseInt(price) * 1.1).toString());
                mp.trigger("previewWheelTune", type, id, 0);
            }
        });
    }
    $(".owned").css("display", "block");
}

function setCurrentWheels(data){
    data = JSON.parse(data);
    let name = data[0];
    let type = data[1];
    let id = data[2];
    let price = data[3];
    if(name != "")
    {
        currentWheels = [name, type, id, price];
        $(`.current-wheels`).append(`
            <div class="current-wheel" id="current-wheel-${type}-${id}">
                <p>${name}</p>
                <img src="../ManageTune/img/${type}_${id}.png">
                <p>$${price}</p>
                <input type="button" id="current-button" class="button_cancel" value="Zdemontuj">
            </div>
            `);
        $("#current-button").on("click", function () {
            $("*").removeClass("selected");
            $(".current-wheel").addClass("selected");
            selectedWheelToInstall = ["remove", name, type, id, price];
            $(".offerInput").attr("min", (parseInt(price)/2).toString());
            mp.trigger("previewWheelTune", 0, -1, 0);
        });
    }
}

$("#closeOwned").on("click", function(){
    mp.trigger("closeWheelsTuneBrowser");
});

$(".applyWheels").on("click", function(){
    if(currentWheels.length > 0){
        mp.trigger("applyWheelTune", currentWheels[0], currentWheels[1], currentWheels[2], currentWheels[3]);
    }
});

$("#sendOffer").on("click", function(){
    let offer = parseInt($(".offerInput").val());
    if(selectedWheelToInstall.length > 0){
        if(selectedWheelToInstall[0] == "install"){
            let price = parseInt(selectedWheelToInstall[4]);
            if(offer >= price && offer <= price * 1.1){
                if(currentWheels.length == 0){
                    $(".offerInput").css("border-color", "#0f0");
                    mp.trigger("sendWheelTuneOffer", selectedWheelToInstall[0], selectedWheelToInstall[1], selectedWheelToInstall[2], selectedWheelToInstall[3], offer, selectedWheelToInstall[5]);
                }
                else{
                    $("#sendOffer").html("Najpierw zdemontuj felgi!")
                    resetButton();
                }
            }
            else{
                $(".offerInput").css("border-color", "#f00");
                $("#sendOffer").html("Niepoprawna cena!")
                resetButton();
            }
        }
        else if(selectedWheelToInstall[0] == "remove"){
            let price = parseInt(selectedWheelToInstall[4]);
            if(offer <= price && offer >= price * 0.5){
                $(".offerInput").css("border-color", "#0f0");
                mp.trigger("sendWheelTuneOffer", selectedWheelToInstall[0], selectedWheelToInstall[1], selectedWheelToInstall[2], selectedWheelToInstall[3], offer);
            }
            else{
                $(".offerInput").css("border-color", "#f00");
                $("#sendOffer").html("Niepoprawna cena!")
                resetButton();
            }
        }
    }
    else{
        $(".offerInput").css("border-color", "#f00");
    }
});


function resetButton(){
    setTimeout(function () {
        $("#sendOffer").html("Zaoferuj")
    }, 3000)
}