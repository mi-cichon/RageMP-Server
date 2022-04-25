let repairParts = [];
let partsToRepair = [];
let time = 0;
let price = 0;
function setInfo(id, model, damage, modelPrice){

    setRepairParts(damage, modelPrice);
    insertPartsToTable();

    $(".idText").text(id);
    $(".modelText").text(model);
    $(".priceText").text("0$");
    $(".timeText").text("0 s.");
    copyToClipboard(`setInfo('${id}', '${model}', '${damage}', ${modelPrice});`)
}


function confirmRepair(){
    if(price > 0){
        mp.trigger('mech_confirmRepair', price, time, JSON.stringify(getPartsNames()));
        mp.trigger("closeMechBrowser");
    }
    else{
        mp.trigger("showNotification", "Nie wybrałeś żadnej części!");
    }
}

function closeBrowser(){
    mp.trigger('closeMechBrowser');
}


function visualizeDamage(damage, value){
    switch(damage)
    {
        case "engine":
            if(value < 1000 && value > 200){
                $('.car_engine').css('background-image', `url('img/engine/engine_medium.png')`);
            }
            else if(value <= 200){
                $('.car_engine').css('background-image', `url('img/engine/engine_bad.png')`);
            }
            break;
        case "body":
            if(value < 1000 && value > 100){
                $('.car_body_top').css('background-image', `url('img/body/top/body_medium.png')`);
                $('.car_body_right').css('background-image', `url('img/body/right/body_right_medium.png')`);
                $('.car_body_left').css('background-image', `url('img/body/left/body_left_medium.png')`);
            }
            else if(value <= 100){
                $('.car_body_top').css('background-image', `url('img/body/top/body_bad.png')`);
                $('.car_body_right').css('background-image', `url('img/body/right/body_right_bad.png')`);
                $('.car_body_left').css('background-image', `url('img/body/left/body_left_bad.png')`);
            }
            break;
        case "fldoor":
            if(value == 0){
                $('.car_door_left_front').css('background-image', `url('img/doors/left/front_bad.png')`);
            }
            break;
        case "frdoor":
            if(value == 0){
                $('.car_door_right_front').css('background-image', `url('img/doors/right/front_bad.png')`);
            }
            break;
        case "bldoor":
            if(value == 0){
                $('.car_door_left_back').css('background-image', `url('img/doors/left/back_bad.png')`);
            }
            break;
        case "brdoor":
            if(value == 0){
                $('.car_door_right_back').css('background-image', `url('img/doors/right/back_bad.png')`);
            }
            break;
        case "hood":
            if(value == 0){
                $('.car_door_front').css('background-image', `url('img/doors/front/front_bad.png')`);
            }
            break;
        case "trunk":
            if(value == 0){
                $('.car_door_back').css('background-image', `url('img/doors/back/back_bad.png')`);
            }
            break;
        case "flwindow":
            if(value == 0){
                $('.car_window_left_front').css('background-image', `url('img/windows/left/front_bad.png')`);
            }
            break;                        
        case "frwindow":
            if(value == 0){
                $('.car_window_right_front').css('background-image', `url('img/windows/right/front_bad.png')`);
            }
            break;
        case "blwindow":
            if(value == 0){
                $('.car_window_left_back').css('background-image', `url('img/windows/left/back_bad.png')`);
            }
            break;
        case "brwindow":
            if(value == 0){
                $('.car_window_right_back').css('background-image', `url('img/windows/right/back_bad.png')`);
            }
            break;
        case "fwindow":
            if(value == 0){
                $('.car_window_front').css('background-image', `url('img/windows/front/front_bad.png')`);
            }
            break;
        case "bwindow":
            if(value == 0){
                $('.car_window_back').css('background-image', `url('img/windows/back/back_bad.png')`);
            }
            break;
        case "flwheel":
            if(value == 0){
                $('.car_wheel_left_front').css('background-image', `url('img/wheels/wheel_bad.png')`);
            }
            break;
        case "frwheel":
            if(value == 0){
                $('.car_wheel_right_front').css('background-image', `url('img/wheels/wheel_bad.png')`);
            }
            break;
        case "blwheel":
            if(value == 0){
                $('.car_wheel_left_back').css('background-image', `url('img/wheels/wheel_bad.png')`);
            }
            break;
        case "brwheel":
            if(value == 0){
                $('.car_wheel_right_back').css('background-image', `url('img/wheels/wheel_bad.png')`);
            }
            break;
    }
}


function setRepairParts(damage, modelPrice){
    if(modelPrice == 0)
    {
        return null;
    }
    else
    {
        let maxCarPrice = 2000000; //dwa miliony max
        let maxPartPrice = 150.0;
        let priceScale = modelPrice / maxCarPrice;
        priceScale *= 50;
        let partPrice = (10.0 * (-1 * Math.pow((1.0 / 2.0), priceScale / 6.0)) + 10.0) / 10.0 * maxPartPrice;
        partPrice = partPrice.toFixed(2);

        let doorMultiplier = 0.8;
        let windowMultiplier = 0.6;
        let engineMultiplier = 3;
        let bodyMultiplier = 2;


        damage = JSON.parse(damage);
        for(var key in damage){
            var part = key;
            var value = damage[key];
            visualizeDamage(part, Math.ceil(value));
            if(part == "engine" && value < 1000){
                let price = Math.ceil(partPrice * engineMultiplier * (Math.abs(value - 1000) / 1000));
                let time = 5.0;
                repairParts.push({type: part, price: price, time: time});
            }
            else if(part =="body" && value < 1000){
                let price = Math.ceil(partPrice * bodyMultiplier * (Math.abs(value - 1000) / 1000));
                let time = 5.0;
                repairParts.push({type: part, price: price, time: time});
            }   
            else if(value == 0){
                if (part.includes("window"))
                {
                    let price = Math.ceil(partPrice * windowMultiplier);
                    let time = 3.0;
                    repairParts.push({type: part, price: price, time: time});
                }
                if (part.includes("door") || part == "hood" || part == "trunk")
                {
                    let price = Math.ceil(partPrice * doorMultiplier);
                    let time = 3.0;
                    repairParts.push({type: part, price: price, time: time});
                }
                if (part.includes("wheel"))
                {
                    let price = 50;
                    let time = 5;
                    repairParts.push({type: part, price: price, time: time});
                }
            }
        }
    }
}

function insertPartsToTable(){
    repairParts.forEach((part, index) => {
        let name;
        switch(part.type)
        {
            case "engine":
                name = "Silnik";
                break;
            case "body":
                name = "Karoseria";
                break;
            case "fldoor":
                name = "Przednie lewe drzwi";
                break;
            case "frdoor":
                name = "Przednie prawe drzwi";
                break;
            case "bldoor":
                name = "Tylnie lewe drzwi";
                break;
            case "brdoor":
                name = "Tylnie prawe drzwi";
                break;
            case "hood":
                name = "Maska";
                break;
            case "trunk":
                name = "Bagażnik";
                break;
            case "flwindow":
                name = "Przednia lewa szyba";
                break;                        
            case "frwindow":
                name = "Przednia prawa szyba";
                break;
            case "blwindow":
                name = "Tylna lewa szyba";
                break;
            case "brwindow":
                name = "Tylna prawa szyba";
                break;
            case "fwindow":
                name = "Przednia szyba";
                break;
            case "bwindow":
                name = "Tylna szyba";
                break;
            case "flwheel":
                name = "Przednia lewa opona";
                break;
            case "frwheel":
                name = "Przednia prawa opona";
                break;
            case "blwheel":
                name = "Tylna lewa opona";
                break;
            case "brwheel":
                name = "Tylna prawa opona";
                break;
        }
        $('.scroll').append(`
            <div class="mech_parts_part" id="part_${part.type}"><p>${name}</p><div></div></div>
        `);

        $(`#part_${part.type}`).on("click", function(){
            if(partsToRepair.includes(part)){
                partsToRepair.splice(partsToRepair.indexOf(part), 1);
                $(`#part_${part.type} div`).removeClass('checked');
                setPriceAndTime();
            }
            else{
                partsToRepair.push(part);
                $(`#part_${part.type} div`).addClass('checked');
                setPriceAndTime();
            }
        });
        //<div class="mech_parts_part"><p></p><div></div></div>
    });
}



function setPriceAndTime(){
    time = 0;
    price = 0;
    partsToRepair.forEach(part => {
        time += part.time;
        price += part.price;
    });
    $(".priceText").text(price + "$");
    $(".timeText").text(time + " s.");
}

function getPartsNames(){
    let names = [];
    partsToRepair.forEach(part => {
        names.push(part.type);
    });
    return names;
}

const copyToClipboard = str => {
    const el = document.createElement('textarea');
    el.value = str;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
  };



  $(window).on("load", () =>{
    $(".mech_parts_all").on("click", function(){
        let everySelected = true;
        repairParts.forEach(part => {
            if(!partsToRepair.includes(part)){
                everySelected = false;
            }
        });
        if(everySelected){
            $(".mech_parts_part > div").removeClass("checked");
            partsToRepair = [];
            setPriceAndTime();
        }
        else{
            $(".mech_parts_part > div").removeClass("checked");
            $(".mech_parts_part > div").addClass("checked");
            partsToRepair = [];
            repairParts.forEach((part)=>{
                partsToRepair.push(part);
                setPriceAndTime();
            });
        }
    });
  })
