type = null;


function start(t = ""){
    switch(t){
        case "rick":
            $("video").attr("src", "http://51.38.135.199/img/roll.webm");
            break;
    }
    type = t;
}

$('body').on('keydown', function(){
    if(type){
        $('video').css('display', 'block')
        document.getElementById("video").play();
        document.getElementById("video").onended = function(){
            mp.trigger("closeTrollBrowser");
        }
    }
    
});