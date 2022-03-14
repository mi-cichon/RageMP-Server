
let removing = null;
let id = 0;
function showNotification(text){
    let element = `<div class="notification id-${id}">${text}</div>`;
    id++;
    $(`.first`).before(element);
    moveBack($(`.id-${id-1}`));
}


function moveBack(element){
    console.log(element);
    setTimeout(function(){
        element.css("margin-left", "30vh");
        setTimeout(() => {
            element.remove();
        }, 1200);
    },5000);
}

function startRemoving(){
    if(!removing){
        removing = setInterval(() => {
            if(!($('.notification').first().html() === undefined)){
                moveFirst();
            }
            else{
                clearInterval(removing);
                removing = null;
            }
        }, 3000);
    }
}