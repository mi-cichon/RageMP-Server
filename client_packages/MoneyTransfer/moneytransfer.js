let spacer = " ";
function setVars(name){
    $(".target p").text(name);
}

$(".confirm").on("click", function(){
    let money = $(".amountInput").val();
    if($(".title textarea").val().length >= 5 && $(".title textarea").val().length <= 150){
        if(checkMoney(money)){
            mp.trigger("transferMoneyToPlayer", parseInt(money), $(".title textarea").val());
        }
        else{
            mp.trigger("showNotification", "Podano nieprawidłową wartość!");
        }
    }
    else{
        mp.trigger("showNotification", "Tytuł przelewu musi zawierać co najmniej 5 znaków!");
    }
});

$(".close").on("click", function(){
    mp.trigger('closeMoneyTransferBrowser');
});

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