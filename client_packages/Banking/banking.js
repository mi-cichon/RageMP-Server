let spacer = " ";
let operation = null;
function setVars(money, social){
    operation = null;
    $(".bank_avatar").css("background-image", `url('http://51.38.135.199/avatars/${social}/avatar.png')`);
    $("*").removeClass("bank_selected");
    $(".bank_account_state").text("$ " + betterNumbers(money));
    getRandomCardNumber(social.toString());
}

$(".bank_withdraw").on("click", function(){
    operation = false;
    $("*").removeClass("bank_selected");
    $(this).addClass("bank_selected");
});

$(".bank_deposit").on("click", function(){
    operation = true;
    $("*").removeClass("bank_selected");
    $(this).addClass("bank_selected");
});

$(".bank_accept").on("click", function(){
    if(operation != null){
        let money = $(".bank_input").val();
        if(operation){
            if(checkMoney(money)){
                mp.trigger("depositBankMoney", parseInt(money));
            }
            else{
                mp.trigger("showNotification", "Podano nieprawidłową wartość!");
            }
        }
        else{
            if(checkMoney(money)){
                mp.trigger("withdrawBankMoney", parseInt(money));
            }
            else{
                mp.trigger("showNotification", "Podano nieprawidłową wartość!");
            }
        }
    }
    else{
        mp.trigger("showNotification", "Wybierz rodzaj operacji!");
    }
});

$(".bank_cancel").on("click", function(){
    mp.trigger('closeBankingBrowser');
});


function getRandomCardNumber(s){
    let seed1 = s.toString();
    let seed2 = (s + 1).toString();
    Math.seedrandom(seed1);
    let numbers = Math.random().toString();
    let number = `${numbers[2]}${numbers[3]}${numbers[4]}${numbers[5]} ${numbers[6]}${numbers[7]}${numbers[8]}${numbers[9]}`;
    Math.seedrandom(seed2);
    numbers = Math.random().toString();
    number += ` ${numbers[2]}${numbers[3]}${numbers[4]}${numbers[5]} ${numbers[6]}${numbers[7]}${numbers[8]}${numbers[9]}`
    $(".bank_card p").text(number);
}

function checkMoney(money){
    try{
        money = parseInt(money);
        if(!isNaN(money) && money > 0 && money < 2147483647){
            return true;
        }
        else{
            return false;
        }
    }
    catch{
        return false;
    }
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