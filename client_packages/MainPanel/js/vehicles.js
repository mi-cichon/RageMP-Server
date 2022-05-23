mp.events.call("mainPanel_requestVehiclesData");
function insertVehicles(data){
    data = JSON.parse(data);
    data.forEach(vehicle => {
        let id = vehicle[0];
        let name = vehicle[1];
        let model = vehicle[2];
        $('.panel_vehicles_vehicles').append(`
            <div class="panel_vehicles_vehicle" id="veh_${id}" style="background-image: url('http://51.38.128.119/img_new/${model}.png')">
                <div class="panel_vehicle_id">${id}</div>
                <div class="panel_vehicle_name">${name}</div>
            </div>
        `);

        $(`#veh_${id}`).on("click", function(){
            $('.panel_vehicles_vehicles *').removeClass("panel_vehicle_selected");
            $(this).addClass('panel_vehicle_selected');
            setLoading();
            mp.trigger('mainPanel_requestVehicleData', id);
        });
    })
}


function setLoading(){
    $('.panel_vehicle_info').css('align-items', 'center');
    $('.panel_vehicle_info').html(`
    <div class="lds-ring"><div></div><div></div><div></div><div></div></div>
    `);
}

function stopLoading(){
    $('.panel_vehicle_info').css('align-items', 'flex-start'); 
    $('.panel_vehicle_info').html(``);
}

function setVehiclesData(vehInfo, mechInfo, visuInfo){
    stopLoading();
    vehInfo = JSON.parse(vehInfo);
    
    let name = vehInfo[0];
    let id = vehInfo[1];
    let state = vehInfo[2];
    let comb = vehInfo[3];
    let tank = vehInfo[4];
    let trunk = vehInfo[5];
    let trip = vehInfo[6];
    let mechtune = mechInfo == "" ? "brak" : JSON.parse(mechInfo).join(', ');
    let visutune = visuInfo == "" ? "brak" : JSON.parse(visuInfo).join(', ');

    $('.panel_vehicle_info').html(`
        <p><b>Nazwa: </b>${name}</p>
        <p><b>ID: </b>${id}</p>
        <p><b>Status pojazdu: </b>${state}</p>
        <p><b>Przebieg: </b>${parseFloat(trip).toFixed(1)} km</p>
        <p><b>Współczynnik spalania: </b>${comb}</p>
        <p><b>Pojemność baku: </b>${tank}</p>
        <p><b>Bagażnik: </b>${trunk}</p>
        <p><b>Tuning mechaniczny: </b>${mechtune}</p>
        <p><b>Tuning wizualny: </b>${visutune}</p>
    `);
}

