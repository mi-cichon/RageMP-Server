let spawnTimeout = null;

function selectSpawn(spawn){
    if(spawnTimeout == null){
        startShrinking();
        spawnTimeout = setTimeout(function(){
            mp.trigger("spawnSelected", spawn);
        }, 2000);
    }
}

function hasNotHouse()
{
    document.getElementById("houseBlock").style.display = "none";
}

function startShrinking(){
    $(".mainContainer").css("width", "0vh");
    $(".mainContainer").css("height", "0vh");
    $(".mainContainer").css("background-color", "rgba(90,90,90, 0)");
}