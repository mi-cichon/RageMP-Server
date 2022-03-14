let currentPrice = -1;
let currentType = -1;
let currentId = -1;
let currentAmount = -1;
//catalog
$("#catalog").on("click", function(){
    $(".controls").css("display", "none");
    mp.trigger("requestAvailableWheels");
});

function setAvailableWheels(data){
    data = JSON.parse(data);
    for(let i = 0; i < data.length; i++){
        let name = data[i][0];
        let type = data[i][1];
        let id = data[i][2];
        let price = data[i][3];
        $(`.category#${type}`).append(`
        <div class="wheel" id="wheel-${type}-${id}"><p>${name}</p><img src="img/${type}_${id}.png"><p>$${price}</p></div>
        `);
        $(`#wheel-${type}-${id}`).on("click", function(){
            $("*").removeClass("selected");
            $(this).addClass("selected");
            currentPrice = price;
            currentType = type;
            currentId = id;
            currentAmount = parseInt($(".amountInput").val());
            $(".price").text("Cena: " + (parseInt(currentPrice) * parseInt($(".amountInput").val())).toString() + "$");
        });
    }
    $(".catalog").css("display", "block");
}

$(".amountInput").on("change", function () {
    $(".price").text("Cena: " + (parseInt(currentPrice) * parseInt($(".amountInput").val())).toString() + "$");
    currentAmount = parseInt($(this).val());
});

$("#createOrder").on("click", function(){
    if(currentType != -1){
        mp.trigger('createWheelsOrder', currentType, currentId, currentAmount, currentPrice);
    }
});
$("#closeCatalog").on("click", function(){
    $(".category").children().remove();
    $(".catalog").css("display", "none");
    $(".controls").css("display", "flex");
});

//owned wheels
$("#owned").on("click", function(){
    $(".controls").css("display", "none");
    mp.trigger("requestOwnedWheels");
});

function setOwnedWheels(data){
    data = JSON.parse(data);
    for(let i = 0; i < data.length; i++){
        let name = data[i][0];
        let type = data[i][1];
        let id = data[i][2];
        let price = data[i][3];
        $(`.owned-wheels`).append(`
        <div class="owned-wheel" id="wheel-${type}-${id}"><p>${name}</p><img src="img/${type}_${id}.png"><p>$${price}</p></div>
        `);
    }
    $(".owned").css("display", "flex");
}

$("#closeOwned").on("click", function(){
    $(".owned-wheels").children().remove();
    $(".owned").css("display", "none");
    $(".controls").css("display", "flex");
});

//shipment wheels
$("#shipment").on("click", function(){
    $(".controls").css("display", "none");
    mp.trigger("requestShipmentWheels");
});

function setShipmentWheels(data){
    data = JSON.parse(data);
    for(let i = 0; i < data.length; i++){
        let name = data[i][0];
        let type = data[i][1];
        let id = data[i][2];
        let date = data[i][3];
        $(`.shipment-wheels`).append(`
        <div class="owned-wheel" id="wheel-${type}-${id}"><p>${name}</p><img src="img/${type}_${id}.png"><p>${date}</p></div>
        `);
    }
    $(".shipment").css("display", "flex");
}

$("#closeShipment").on("click", function(){
    $(".shipment-wheels").children().remove();
    $(".shipment").css("display", "none");
    $(".controls").css("display", "flex");
});
let closeBusiness = 0;
//manage
$("#manage").on("click", function(){
    $(".controls").css("display", "none");
    mp.trigger("requestManageData");
});

function setManageData(date, price){

    $(".manage-price").text("$" + price);
    $(".manage-date").text(date);

    $(".manage").css("display", "flex");
}

$("#closeManage").on("click", function(){
    $(".manage").css("display", "none");
    $(".controls").css("display", "flex");
});
$("#closeBusiness").on("click", function(){
    closeBusiness += 1;
    if(closeBusiness != 4){
        $(this).html("Jesteś pewny?");
    }
    else{
        mp.trigger("closeBusiness");
    }
    
});

$("#extendTime").on("click", function(){
    mp.trigger("extendTuneTime", $(".daysInput").val());
});

//controls
$("#close").on("click", function(){
    mp.trigger(`closeManageTuneBusinessBrowser`);
});

function setJobStatus(status){
    if(status){
        $("#jobstatus").html("Zakończ pracę");
    }
    else{
        $("#jobstatus").html("Rozpocznij pracę");
    }
}

$("#jobstatus").on("click", function(){
    mp.trigger("switchBusinessJob");
});