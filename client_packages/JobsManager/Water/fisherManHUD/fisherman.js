let pointer;
let balanceInterval;
let fishingLevel;
let balanceDirection = 0;
let pointerRotation = 0;
let directionSwitches = 0;
let lastKey = null;

function startFishing(level){
    fishingLevel = level;
    pointer = $(".fisherPointer");
    setTimeout(() => {
        balanceInterval = setInterval(balance, 10);
        switchDirection();
    }, 1000);
}


function balance(){
    if(pointerRotation < 80 && pointerRotation > -80){
        switch(balanceDirection){
            case 0:
                pointer.css("transform", `rotate(${pointerRotation - fishingLevel/5}deg)`);
                pointerRotation -= fishingLevel/5;
                break;
            case 1:
                pointer.css("transform", `rotate(${pointerRotation + fishingLevel/5}deg)`);
                pointerRotation += fishingLevel/5;
                break;
        }
    }
    else{
        clearInterval(balanceInterval);
        gameEnd(false);
    }
}

document.addEventListener('mousedown', logMouseButton);

function logMouseButton(e) {
  if (typeof e === 'object' && lastKey == null) {
    switch (e.button) {
      case 0:
        if(balanceInterval){
            pointer.css("transform", `rotate(${pointerRotation - 20}deg)`);
            pointerRotation -= 20;
            lastKey = 0;
        }
        break;
      case 2:
        if(balanceInterval){
            pointer.css("transform", `rotate(${pointerRotation + 20}deg)`);
            pointerRotation += 20;
            lastKey = 2;
        }
        break;
    }
  }
}

document.addEventListener('mouseup', function (e) {
    if(lastKey == 0 || lastKey == 2){
        lastKey = null;
    }
});


$(document).keydown(function(e) {
    if(lastKey == null){
        if(e.which == 65 || e.which == 37) {
            //A lub strzałka w lewo
            lastKey = e.which;
            if(balanceInterval){
                pointer.css("transform", `rotate(${pointerRotation - 20}deg)`);
                pointerRotation -= 20;
            }
        }
        else if(e.which == 68 || e.which == 39) {
            //D lub strzałka w prawo
            lastKey = e.which;
            if(balanceInterval){
                pointer.css("transform", `rotate(${pointerRotation + 20}deg)`);
                pointerRotation += 20;
                
            }
        }
    }
});

$(document).keyup(function(e) {
    if(e.which == lastKey){
        lastKey = null;
    }
});

function switchDirection(){
    setTimeout(() => {
        balanceDirection = balanceDirection == 0 ? 1 : 0;
        directionSwitches++;
        if(directionSwitches < fishingLevel + 2){
            switchDirection();
        }
        else{
            clearInterval(balanceInterval);
            gameEnd(true);
        }
    }, getRandomInt(2, 6) * 1000);
}

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
  }

  function gameEnd(state){
    mp.events.call("fishermanEnd", state);
  }