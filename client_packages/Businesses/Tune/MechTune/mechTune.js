currentPart = [];
function setTune(data){
    data = JSON.parse(data);
    for(let i = 0; i < 5; i++){
        let tune = data[i];
        if(tune[0] == "1"){
            $(".current-tune").append(`
            <div class="current-t" id="current-${i}">
                <p>${tune[1]}</p>
                <img src="img/${i}.png">
                <p>${parseInt(parseInt(tune[2]) * 0.7)}$</p>
                <input type="button" id="current-button-${i}" class="button_cancel" value="Zdemontuj">
            </div>
        `);
        $(`#current-button-${i}`).on("click", function(){
            currentPart = ['remove', i, parseInt(parseInt(tune[2]) * 0.7), tune[1]];
            $('*').removeClass("selected");
            $(this).parent().addClass("selected");
        });
        }
        else{
            $(".todo-tune").append(`
                    <div class="todo-t" id="todo-${i}">
                        <p>${tune[1]}</p>
                        <img src="img/${i}.png">
                        <p>${parseInt(tune[2])}$</p>
                        <input type="button" id="todo-button-${i}" class="button_select" value="Montuj">
                    </div>
            `);
            $(`#todo-button-${i}`).on("click", function(){
                currentPart = ['install', i, parseInt(tune[2]), tune[1]];
                $('*').removeClass("selected");
                $(this).parent().addClass("selected");
            });
        }
    }
}

$("#sendOffer").on("click", function(){
    let offer = parseInt($(".offerInput").val());
    if(currentPart.length > 0){
        if(offer >= 0 && offer <= 500){
            mp.trigger("sendMechTuneOffer", currentPart[0], currentPart[1], currentPart[3], currentPart[0] == "install" ? parseInt(currentPart[2]) + offer : parseInt(currentPart[2]) - offer, offer);
        }
        else{
            resetButton("Max $500 doliczki!");
        }
    }
    else{
        resetButton("Nie wybrałeś części!");
    }
});

function resetButton(text){
    $("#sendOffer").html(text);
    setTimeout(function () {
        $("#sendOffer").html("Zaoferuj")
    }, 3000)
}

$("#closeMechTune").on("click", function () {
    mp.trigger("closeMechTuneBrowser");
});