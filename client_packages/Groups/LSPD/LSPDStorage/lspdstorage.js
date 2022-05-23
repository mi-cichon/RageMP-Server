function insertCar(id, model, name){
    let vehicle = `
    <div class="vehicle" id="${id}">
        <div class="name">${name}</div>
        <div class="img" style="background-image: url('http://51.38.128.119/img_new/${model}.png')"></div>
        <div class="id">${id}</div>
        <div class="info">Wybierz</div>
    </div>`;
    $(".private").append(vehicle);

    $(`#${id}`).on("click", function(){
        if($(this).hasClass("selected")){
            mp.trigger("createLspdVehicle", id);
            mp.trigger('closeLspdStorageBrowser');
        }
        else{
            $('*').removeClass("selected");
            $(this).addClass("selected");
        }
    });
}

function setStorageId(sId){
    storageId = sId;
}