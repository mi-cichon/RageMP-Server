let pumpState = 0;
let temp = 0.0;
let coolingDown = false;
let pumpBonus = 1;

var tempGraph = new ProgressBar.Line(".pressure_graph", {
    strokeWidth: 4,
    easing: 'easeInOut',
    duration: 1400,
    color: '#FFEA82',
    trailColor: '#888',
    trailWidth: 4,
    svgStyle: {width: '100%', height: '100%'},
    from: {color: '#0f0'},
    to: {color: '#f00'},
    step: (state, bar) => {
      bar.path.setAttribute('stroke', state.color);
    }
  });

let pressureInterval = setInterval(function(){
    if(!coolingDown){
        pumpState = parseInt($('.slider').val());
    }
    if(temp <= 1){
        if(pumpState >= 40){
            temp += 0.000015 * pumpState;
        }
        else if(pumpState >= 0 && temp > 0){
            temp -= 0.002;
        }
        
        tempGraph.set(temp);
    }
    if(temp > 1 && !coolingDown){
        coolingDown = true;
        pumpState = 0;
        mp.trigger("showNotification", "Dopuściłeś do przegrzania pompy!");
        $('.slider').val(0);
    }
    if(coolingDown && temp > 0){
        temp -= 0.0003;
    }
    else if(coolingDown && temp <= 0){
        coolingDown = false;
        $('.slider').val(0);
    }
}, 10);

$('.pressure_graph input').on("click", function(){
    if(!coolingDown){
        if(!pumpState){
            $(this).removeClass("button_select");
            $(this).addClass("button_cancel");
            $(this).attr('value', 'Wyłącz pompę');
            pumpState = true;
        }
        else{
            $(this).addClass("button_select");
            $(this).removeClass("button_cancel");
            $(this).attr('value', 'Włącz pompę');
            pumpState = false;
        }
    }
});

let updateInterval = setInterval(function(){
    if(pumpState > 0 && !coolingDown){
        mp.trigger("refinery_addOil", 30 * (pumpState/100) * pumpBonus);
    }
}, 1000);

$('.buttons .button_cancel').on("click", function(){
    if(coolingDown){
        mp.trigger("showNotification", "Poczekaj aż pompa się schłodzi!");
    }
    else{
        mp.trigger("refinery_closeBrowser");
    }
});

function updateVehiclesTank(amount){
    $('.veh_tank').text(parseInt(amount) + "L");
}

function setPumpBonus(bonus){
    pumpBonus = bonus;
}