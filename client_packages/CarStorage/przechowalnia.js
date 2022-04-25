let storageId = null;
function insertCar(id, name, model, org){
    let vehicle = `
    <div class="vehicle" id="${id}">
        <div class="name">${name}</div>
        <div class="img" style="background-image: url('http://51.38.128.119/img/${model}.png')"></div>
        <div class="id">${id}</div>
        <div class="info">Wybierz</div>
    </div>`;
    switch(org)
    {
        case "0":
            $(".private").append(vehicle);
            break;
        case "1":
            $(".org").append(vehicle);
            break;
    }

    $(`#${id}`).on("click", function(){
        if($(this).hasClass("selected")){
            mp.trigger("createVehicle", id, storageId);
            mp.trigger('closeStorageBrowser');
        }
        else{
            $('*').removeClass("selected");
            $(this).addClass("selected");
        }
    });
}


$(".privateButton").on("click", function(){
    $(".private").css("display", "flex");
    $(".org").css("display", "none");
});
$(".orgButton").on("click", function(){
    $(".org").css("display", "flex");
    $(".private").css("display", "none");
});

function setStorageId(sId){
    storageId = sId;
}