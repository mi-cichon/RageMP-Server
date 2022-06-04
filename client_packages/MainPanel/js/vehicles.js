mp.events.call("mainPanel_requestVehiclesData");
function insertVehiclesData(data){
    let empty = true;
    let inStorage = [];
    let outStorage = [];
    data = JSON.parse(data);
    data.forEach(vehicle => {
        if(vehicle[2] == "False"){
            inStorage.push(vehicle);
        }
        else{
            outStorage.push(vehicle);
        }
    });

    if(inStorage.length > 0){
        $(".panel_carview_tabs").empty();
        empty = false;
        $(".panel_carview_tabs").append(`
            <div class="panel_carview_tab_general">
                Pojazdy w przechowalni
            </div>
        `);

        inStorage.forEach(vehicle => {
            let id = vehicle[0];
            let name = vehicle[1];
            let type = vehicle[3];
            $(".panel_carview_tabs").append(`
                <div class="panel_carview_tab" onclick="loadVehicle(${id}, this)">
                    ${type == "motorcycle" ? '<i class="fa-solid fa-motorcycle"></i>' : '<i class="fa-solid fa-car"></i>'}
                    <div class="panel_carview_id">${id}</div>
                    <div class="panel_carview_name">${name}</div>
                </div>
            `);
        });
    }

    if(outStorage.length > 0){
        if(empty){
            $(".panel_carview_tabs").empty();
        }

        $(".panel_carview_tabs").append(`
            <div class="panel_carview_tab_general">
                Pojazdy poza przechowalnią
            </div>
        `);

        outStorage.forEach(vehicle => {
            let id = vehicle[0];
            let name = vehicle[1];
            let type = vehicle[3];
            $(".panel_carview_tabs").append(`
                <div class="panel_carview_tab" onclick="loadVehicle(${id}, this)">
                    ${type == "motorcycle" ? '<i class="fa-solid fa-motorcycle"></i>' : '<i class="fa-solid fa-car"></i>'}
                    <div class="panel_carview_id">${id}</div>
                    <div class="panel_carview_name">${name}</div>
                </div>
            `);
        });
    }
}

function loadVehicle(id, tab){
    $(".panel_carview_tab.selected").removeClass("selected");
    $(tab).addClass("selected");
    $(".panel_carview_view").empty();
    mp.trigger("mainPanel_requestVehicleData", id);
}

function setVehicleData(vehInfo, mechInfo, visuInfo){
    vehInfo = JSON.parse(vehInfo);
    
    let name = vehInfo[0];
    let id = vehInfo[1];
    let model = vehInfo[2];
    let comb = vehInfo[3];
    let tank = vehInfo[4];
    let trunk = vehInfo[5];
    let trip = vehInfo[6];
    let mechtune = mechInfo == "" ? null : JSON.parse(mechInfo);
    let visutune = visuInfo == "" ? null : JSON.parse(visuInfo);

    $('.panel_carview_view').html(`
        <div class="carview_viewInfo">
            <div class="carview_name">${name}<div class="carview_id">${id}</div></div>
            <p><b>Współczynnik spalania: </b>${comb}</p>
            <p><b>Pojemność baku: </b>${tank}</p>
            <p><b>Bagażnik: </b>${trunk}</p>
            <p><b>Przebieg: </b>${parseFloat(trip).toFixed(1)} km</p>
        </div>
        <div class="carview_viewImage" style="background-image: url(http://51.38.128.119/img_new/${model}.png)"></div>
        <div class="carview_viewAddInfo">
            ${mechtune != null ? '<div><b>Tuning mechaniczny:' + createTuneParts(mechtune) +'</b></div>' : ""}
            ${visutune != null ? '<div><b>Tuning wizualny:' + createTuneParts(visutune) +'</b></div>' : ""}
        </div>
    `);
}

function createTuneParts(tune){
    var tuneStr = "";
    tune.forEach(part => {
        tuneStr += `<div class="panel_carview_tune">${part}</div>`
    });
    return tuneStr;
}
