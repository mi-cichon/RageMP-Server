let currentIndex = 0;
let maxIndex = 5;
const controls = {
    
    engine: `<li class="engine box" id="1"><div style="background-image:url('img/enginehover.png')"></div><p>Silnik</p></li>`,
    handbrake: `<li class="handbrake box" id="2"><div style="background-image:url('img/handbrakehover.png')"></div><p>Ręczny</p></li>`,
    seatbelt: `<li class="seatbelt box" id="3"><div style="background-image:url('img/seatbelthover.png')"></div><p>Pasy</p></li>`,
    doorlock: `<li class="doorlock box" id="4"><div style="background-image:url('img/doorlockhover.png')"></div><p>Zamki</p></li>`,
    throw: `<li class="throw box" id="5"><div style="background-image:url('img/throwhover.png')"></div><p>Wyrzuć</p></li>`,
    lights: `<li class="lights box" id="0"><div style="background-image:url('img/lightshover.png')"></div><p>Światła</p></li>`
}

function setType(type, seatbelt, driver){
    let main = $(".main");
    if(!driver){
        switch(type)
        {
            case "personal":
            case "furka":
            case "public":
            case "lspd":
            case "jobveh":
                currentIndex = -1;
                maxIndex = 0;
                main.append(controls.seatbelt);
                // $(".main .seatbelt").addClass("selected");
                break;
        }
    }
    else{
        switch(type)
        {
            case "personal":
            case "furka":
                currentIndex = -1;
                maxIndex = 4;
                main.append(controls.engine);
                // $(".main .engine").addClass("selected");
                main.append(controls.handbrake);
                if(seatbelt){
                    main.append(controls.seatbelt);
                    maxIndex++;
                }
                main.append(controls.doorlock);
                main.append(controls.throw);
                main.append(controls.lights);
                break;
            case "public":
            case "jobveh":
                currentIndex = -1;
                maxIndex = 1;
                main.append(controls.engine);
                // $(".main .engine").addClass("selected");
                if(seatbelt){
                    main.append(controls.seatbelt);
                    maxIndex++;
                }
                main.append(controls.throw);
                break;
            case "lspd":
                currentIndex = -1;
                maxIndex = 3;
                main.append(controls.engine);
                //$(".main .engine").addClass("selected");
                main.append(controls.handbrake);
                if(seatbelt){
                    main.append(controls.seatbelt);
                    maxIndex++;
                }
                main.append(controls.doorlock);
                main.append(controls.throw);
                break;
        }
    }
}
function move(state){
    if(!$(".main").hasClass("hidden")){
        if(!state){
            let main = $('.main');
            main.children().eq(currentIndex).removeClass("selected");
            if (currentIndex == -1){
                currentIndex=maxIndex;
            }
            else{currentIndex--}
            main.children().eq(currentIndex).addClass("selected");
        }
        else{
            let main =$('.main');
            main.children().eq(currentIndex).removeClass("selected");
            if (currentIndex == maxIndex){
                currentIndex=0;
            }
            else{currentIndex++}
            main.children().eq(currentIndex).addClass("selected");
        }
    }
}

function select(){
    if(!$(".main").hasClass("hidden") && currentIndex != -1){
        let main = $('.main');
        let current = parseInt($('.selected').attr("id"));
        if(current == 5){
            mp.trigger("vehc_kickPassengers");
        }
        else{
            let state;

            if ($(`.main #${current}`).hasClass("on"))
                state = false;    
            else
                state = true;

            state ? $(`.main #${current}`).addClass("on") : $(`.main #${current}`).removeClass("on");
            switch(current){
                case 0:
                    mp.trigger("vehc_switchLights", state);
                    break;
                case 1:
                    mp.trigger("vehc_switchEngine", state);
                    break;
                case 2:
                    mp.trigger("vehc_switchParkingbrake", state);
                    break;
                case 3:
                    mp.trigger("vehc_switchSeatbelt", state);
                    break;
                case 4:
                    mp.trigger("vehc_switchLocks", state);
                    break;
            }
        }
    }
}


function setValues(lights, engine, parkingbrake, seatbelt, locks){
    lights ? $(".lights").addClass("on") : $(".lights").removeClass("on");
    engine ? $(".engine").addClass("on") : $(".engine").removeClass("on");
    parkingbrake ? $(".handbrake").addClass("on") : $(".handbrake").removeClass("on");
    seatbelt ? $(".seatbelt").addClass("on") : $(".seatbelt").removeClass("on");
    locks ? $(".doorlock").addClass("on") : $(".doorlock").removeClass("on");
}

function setEngine(state){
    state ? $(".engine").addClass("on") : $(".engine").removeClass("on");
}

function setParkingBrake(state){
    state ? $(".handbrake").addClass("on") : $(".handbrake").removeClass("on");
}

function switchMenu(state)
{
    let main = $('.main');
    state ? main.removeClass("hidden") : main.addClass("hidden");
    if(!state){
        main.children().eq(currentIndex).removeClass("selected");
        currentIndex = -1;
    }
}