let dmoney = null;
let dpp = null;
let spacer = ' ';
let time = 0;
let jobInterval;
let earnedMoney = 0;
let earnedEXP = 0;
let earningsPerHour = 0;
let pointsType = "";
let music = null;
let moneyInterval = null;
const defaultHudScale = 1.5;
const defaultChatScale = 1;
const defaultChatWidth = 50;

function setScales(hudScale, chatScale){
    $(".globalContainer").css("width", (defaultChatWidth + (chatScale/4 - 12.5)).toString() + "vh");
    $(".globalContainer").css("font-size", (defaultChatScale + (chatScale/200 - 0.25)).toString() + "vh");
    $("html").css("font-size", (defaultHudScale + (hudScale/100 -0.5)).toString() + "vh");
}
function setAvatar(social){
    let avatarString = `http://51.38.128.119/avatars/${social}/avatar.png`;
    $(".hud_avatar").css("background-image", `url(${avatarString})`);
}
function UpdateInfo(nickname, money, level = "0", exp="0/0"){
    // let currentMoney = $("#money").text().toString();
    if(dmoney != null){
        // currentMoney = currentMoney.slice(0, -1);
        let oldMoney = parseInt(dmoney);
        let newMoney = parseInt(money);
        dmoney = newMoney;
        if(newMoney > oldMoney)
            addMoney(oldMoney, newMoney);
        else if( newMoney < oldMoney)
            takeMoney(oldMoney, newMoney);
    }
    else{
        $(".hud_money").html("$"+ betterNumbers(money));
        dmoney = money;
    }
    $(".hud_lvl").text(level);
    setExp(exp);
    $(".hud_nick").text(nickname);
}

function setExp(e){
    let current = parseInt(e.split('/')[0]);
    let max = parseInt(e.split('/')[1]);
    let gradient = `linear-gradient(90deg, rgba(0,204,153,1) ${parseInt((current/max) * 100)}%, rgb(74, 74, 74) ${parseInt((current/max) * 100) + 5}%, rgb(74, 74, 74) 100%)`;
    $(".hud_avatar_wrap").css("background", gradient);
}

function addMoney(oldMoney, newMoney){
    $(".hud_money").css("color", "#090");
    $(".hud_money").html("+" + betterNumbers(Math.abs(oldMoney - newMoney)) + " $")
    setTimeout(function(){
        $(".hud_money").css("color", "#fff");
        let offset = getOffset(oldMoney, newMoney);
        
        if(moneyInterval != null){
            clearInterval(moneyInterval);
        }
            
        moneyInterval = setInterval(() =>{
            if(oldMoney < newMoney){
                oldMoney += offset;
                $(".hud_money").html(betterNumbers(oldMoney) + " $");
            }
            else if(oldMoney > newMoney){
                $(".hud_money").html(betterNumbers(newMoney) + " $");
                clearInterval(moneyInterval);
                moneyInterval = null;
            }
        }, 2)
    }, 3000);
    
}

function takeMoney(oldMoney, newMoney){
    $(".hud_money").css("color", "#e00");
    $(".hud_money").html("-" + betterNumbers(Math.abs(oldMoney - newMoney)) + " $")
    setTimeout(function(){
        $(".hud_money").css("color", "#fff");
        let offset = getOffset(oldMoney, newMoney);
        if(moneyInterval != null)
            clearInterval(moneyInterval);
        moneyInterval = setInterval(() =>{
            if(oldMoney > newMoney){
                oldMoney -= offset;
                $(".hud_money").html(betterNumbers(oldMoney) + " $");
            }
            else if(oldMoney < newMoney){
                $(".hud_money").html(betterNumbers(newMoney) + " $");
                clearInterval(moneyInterval);
                moneyInterval = null;
            }
        }, 2);
    }, 3000);
    
}

function getOffset(oldMoney, newMoney){
    let offset = Math.abs(oldMoney - newMoney);
    offset = Math.round(offset/10000);
    if(offset == 0)
        return 3;
    else
        return offset * 3;
}

function betterNumbers(val) {
    result = val.toString().trim();
    for(var i = val.toString().trim().length-1; i >= 0; i--) {
        if((val.toString().trim().length - i) % 3 == 0) {
            if(val.toString().slice(0,i) != '')
            result = result.slice(0,i) + spacer + result.slice(i);
        }
    }
    return result;
}

function startJob(workName, type) {
    $(".jobHUD_mainBody").css("display", "flex");
    if(time == 0) {
        jobInterval = setInterval(updateJobTime, 1000);
        $(".jobHUD_name").text(`${workName}`);
    }
}

function stopJob() {
    let hud = $(".jobHUD_mainBody");
    hud.css("display", "none");
    if(!hud.hasClass("jobHUD_hidden"))
        hud.addClass("jobHUD_hidden");
        $(".jobHUD_openText").html("Naciśnij <kbd>B</kbd> aby otworzyć HUD pracy");
    clearInterval(jobInterval);
    time = 0;
    earnedMoney = 0;
    earnedEXP = 0;
    earningsPerHour = 0;
    $(".jobHUD_time").text('0s');
    $(".jobHUD_earnings").text('0 $');
    $(".jobHUD_exp").text('0 exp');
    $(".jobHUD_average").text('0 $/h');
}

function switchJobHUD() {
    let hud = $(".jobHUD_mainBody");
    if(hud.hasClass("jobHUD_hidden")){
        hud.removeClass("jobHUD_hidden");
        $(".jobHUD_openText").html("Naciśnij <kbd>B</kbd> aby zamknąć HUD pracy");
    }
    else{
        hud.addClass("jobHUD_hidden");
        $(".jobHUD_openText").html("Naciśnij <kbd>B</kbd> aby otworzyć HUD pracy");
    }
}

function updateJobTime() {
    time++;
    if(time / 3600 >= 1) {
        var h = parseInt(time/3600);
        var m = parseInt((time-h*3600)/60);
        var s = (time-h*3600-m*60);
        $(".jobHUD_time").html(`${h}h ${m}m ${s}s`);
    } else if(time / 60 >= 1) {
        var m = parseInt(time/60);
        var s = parseInt(time-m*60);
        $(".jobHUD_time").html(`${m}m ${s}s`);
    } else {
        $(".jobHUD_time").html(`${time}s`);
    }
    earningsPerHour = earnedMoney/(time/3600);
    $(".jobHUD_average").html(`${betterNumbers(parseInt(earningsPerHour))} $/h`);
}

function updateJobVars(money, pp) {
    earnedMoney += money;
    earnedEXP += pp;
    $(".jobHUD_earnings").text(`${betterNumbers(earnedMoney)} $`);
    $(".jobHUD_exp").text(`${earnedEXP} exp`);
}

function terminateJob() {
    mp.trigger('terminateJob');

}

function setTime(time){
    $(".hud_time").text(time);
}

function warn(){
    $(".body").css("background-color", "rgba(255,0,0,0.8)")
    setTimeout(function(){
        $(".body").css("background-color", "rgba(255,0,0,0)")
    },1000);
    var audio = new Audio('warn.mp3');
    audio.play();
}

function notification(){
    var audio = new Audio('notification.mp3');
    audio.play();
}

function playmusic(){
    let m = new Audio("pnp.mp3");
    m.play();
}

$("body").on("keyup", function (e) {
    if(!$(".jobHUD_mainBody").hasClass("jobHUD_hidden") && e.which == 35){
        terminateJob();
    }
})

function setNewMessages(amount){
    if(amount > 0){
        $(".hud_messages").css("display", "block");
    }
    else{
        $(".hud_messages").css("display", "none");
    }
    
}

// setInterval(() => {
//     let icon = $(".hud_messages");
//     if(icon.css("display") == "block"){
//         if(parseFloat(icon.attr("shadow")) > 1.2 && icon.attr("dir") == "up"){
//             icon.attr("dir", "down"); 
//         }
//         else if(parseFloat(icon.attr("shadow")) < 0.7 && icon.attr("dir") == "down"){
//             icon.attr("dir", "up"); 
//         }
//         else if(icon.attr("dir") == "down"){
//             let val = parseFloat(icon.attr("shadow"));
//             val -= 0.005;
//             icon.attr("shadow", val.toString());
//             $(".hud_messages > div").css("box-shadow", `0 0 0 red, 0 0 ${val.toFixed(2)}em 0.6em rgba(255,0,0,0.6)`);
//         }
//         else if(icon.attr("dir") == "up"){
//             let val = parseFloat(icon.attr("shadow"));
//             val += 0.005;
//             icon.attr("shadow", val.toString());
//             $(".hud_messages > div").css("box-shadow", `0 0 0 red, 0 0 ${val.toFixed(2)}em 0.6em rgba(255,0,0,0.6)`);
//         }
//     }
// },10);

const copyToClipboard = str => {
    const el = document.createElement('textarea');
    el.value = str;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
  };


function screenShotData(url){
    //url = "img/avatars/default.png";
    encodeImage(url, (code) =>{
        mp.trigger("carMugshot_send", code);
    });
}

function encodeImage(src, callback) {
    var canvas = document.createElement('canvas'),
        ctx = canvas.getContext('2d'),
        img = document.getElementById("image");
        img.src = src;

    img.addEventListener('load', function() {
        ctx.drawImage(img, 425, 240, 71, 40, 0, 0, 425, 240);
        console.log(canvas.toDataURL());
        let urls = [];
        for(let i = 0; i < 36; i++){
            let fractions = getFractionsByIndex(i);
            let h = $("body").height();
            let w = $("body").width();
            canvas.width = w/18;
            canvas.height = h/18;
            cv = canvas;
            c = cv.getContext('2d');
            let pX = w/3 + (w * fractions[0]);
            let pY = h/3 + (h * fractions[1])
            c.drawImage(img, pX, pY, 1/18 * w, 1/18 * h, 0, 0, 1/18 * w, 1/18 * h); 
            urls.push(cv.toDataURL());
        }
        callback(JSON.stringify(urls));
    });  
}

function cropImage(imagePath, canvas, newX, newY, newWidth, newHeight) {
    //create an image object from the path
    const originalImage = new Image();
    originalImage.src = imagePath;
    var cnv = canvas;
    //initialize the canvas object
    const ctx = cnv.getContext('2d');
 
    //wait for the image to finish loading
    originalImage.addEventListener('load', function() {
 
        //set the canvas size to the new width and height
        cnv.width = newWidth;
        cnv.height = newHeight;
         
        //draw the image
        ctx.drawImage(originalImage, newX, newY, newWidth, newHeight, 0, 0, newWidth, newHeight); 
        return cnv;
    });
    return cnv;
}

function getFractionsByIndex(index){
    let row = parseInt((index)/6) + 1;
    let col = (index)%6 + 1;
    return[col/18, row/18];
}

const cropCanvas = (sourceCanvas,left,top,width,height) => {
    let destCanvas = document.createElement('canvas');
    destCanvas.width = width;
    destCanvas.height = height;
    destCanvas.getContext("2d").drawImage(
        sourceCanvas,
        left,top,width,height,  // source rect with content to crop
        0,0,width,height);      // newCanvas, same size as source rect
    return destCanvas;
}

function toDataUrl(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onload = function() {
        var reader = new FileReader();
        reader.onloadend = function() {
            callback(reader.result);
        }
        reader.readAsDataURL(xhr.response);
    };
    console.log("open");
    xhr.open('GET', url);
    console.log("response");
    xhr.responseType = 'blob';
    console.log("send");
    xhr.send();
}