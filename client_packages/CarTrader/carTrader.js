let lastCarClicked = null;
let lastPlayerClicked = null;
let lastClickedCarId = null;
let lastClickedPlayerId = null;

function addCar(id, name)
{
    $('.carEndRow').append(`
    <div class="row carRow car${id}">
        <div class="carCol">
            ${id}
        </div>
        <div class="carCol">
            ${name}
        </div>
    </div>
`);
$( `.car${id}` ).on( "click", function() {
    if(lastCarClicked){
        lastCarClicked.css("border", "none")
    }
    lastCarClicked = $(this);
    $(this).css("border", "solid 1px #00cc99")
    lastClickedCarId = parseInt(id);
  });
}



function addPlayer(id, name)
{
    $('.playerEndRow').append(`
    <div class="row playerRow player${id}">
        <div class="playerCol">
            ${id}
        </div>
        <div class="playerCol">
            ${name}
        </div>
    </div>
`);
$( `.player${id}` ).on( "click", function() {
    if(lastPlayerClicked!= null){
        lastPlayerClicked.css("border", "none")
    }
    lastPlayerClicked = $(this);
    $(this).css("border", "solid 1px #00cc99")
    lastClickedPlayerId = parseInt(id);
  });
}


function confirmTrade(){
    if(lastClickedCarId == null || lastClickedPlayerId == null){
        showError("Nie zaznaczyłeś gracza i/lub pojazdu!");
        
    }
    else if(!isNumeric($(".priceInput").val()) || $(".priceInput").val() == ""){
        showError("Podałeś błędną cenę!");
    }
    else{
        mp.trigger("sendTrade", lastClickedPlayerId, lastClickedCarId, parseInt($(".priceInput").val()));
    }
}

function showError(message = "Wprowadziłeś błędną liczbę!")
{
    let errorBlock = $(".errorCol");
    errorBlock.css("display", "block");
    errorBlock.text(message)
    setTimeout(() => {
        errorBlock.css("display", "none");
    }, 3000)
}

function isNumeric(str) {
    if (typeof str != "string") return false 
    return !isNaN(str);
  }

  function closeCarTraderBrowser(){
    mp.events.call("closeCarTraderBrowser");
  }