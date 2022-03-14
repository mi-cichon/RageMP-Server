function setPosition(pos){
    pos = JSON.parse(pos);
    let menu = $(".contextBody");
    menu.css("left", ($(window).width() * pos[0]).toString() + "px");
    menu.css("top", ($(window).height() * pos[1]).toString() + "px");
    menu.css("display", "block");
}

$(".dp").on("click", function (){
    mp.trigger("admin_deleteCar");
    mp.trigger("closeAdminContextBrowser");
});

$(".here").on("click", function (){
    mp.trigger("admin_bringCar");
    mp.trigger("closeAdminContextBrowser");
});

$(".last").on("click", function (){
    mp.trigger("admin_lastDriver");
    mp.trigger("closeAdminContextBrowser");
});

$(".owner").on("click", function (){
    mp.trigger("admin_carOwner");
    mp.trigger("closeAdminContextBrowser");
});

$(".flip").on("click", function (){
    mp.trigger("admin_flipCar");
    mp.trigger("closeAdminContextBrowser");
});

$(".fix").on("click", function (){
    mp.trigger("admin_fixCar");
    mp.trigger("closeAdminContextBrowser");
});

$(".close").on("click", function (){
    mp.trigger("closeAdminContextBrowser");
});