mp.trigger("mainPanel_requestBankingData");

function setBankingData(data, transfers){
    console.log(data[54]);
    data = JSON.parse(data);
    let social = data[0];
    let name = data[1];
    let amount = data[3];

    let accnumber = data[2].replace(/.{4}/g, '$& ');
    accnumber = accnumber.trimEnd();
    $(".bank_data_history_body").empty();
    $(".bank_data_info_row > img").attr("src", `http://51.38.128.119/avatars/${social}/avatar.png`);
    $(".bank_name").text(name);
    $(".bank_acc").text(`Saldo: $${betterNumbers(amount)}`);
    $(".bank_card > p").text(accnumber);
    $(".bank_data_info_accnmbr").on("click", function(){
        copyToClipboard(accnumber);
    })

    if(transfers != ""){
        let transactions = JSON.parse(transfers);
        transactions.forEach((transaction, index) => {
            $('.bank_data_history_body').append(`
                <div class="bank_data_history_item" id="transaction_${index}">
                    <div class="bank_data_history_item_top">
                        <div class="bank_icon ${transaction[0] == "to" ? "" : "out"}"></div>
                        <div class="bank_from">${transaction[1]}</div>
                        <div class="bank_amount">$${betterNumbers(transaction[2])}</div>
                    </div>
                    <div class="bank_data_history_item_bottom">
                        <p>Data: ${transaction[4]}</p>
                        <p>Tytuł: ${transaction[3]}</p>
                    </div>
                </div>
            `);
            $(`#transaction_${index}`).on("click", function() {
                let state = $(`#transaction_${index} > .bank_data_history_item_bottom`).css("display");
                let newState = state == "none" ? "flex" : "none";
                $(`#transaction_${index} > .bank_data_history_item_bottom`).css("display", newState)
            });
        });
    }
}

function betterNumbers(val) {
    var spacer = " ";
    result = val.toString().trim();
    for(var i = val.toString().trim().length-1; i >= 0; i--) {
        if((val.toString().trim().length - i) % 3 == 0) {
            if(val.toString().slice(0,i) != '')
            result = result.slice(0,i) + spacer + result.slice(i);
        }
    }
    return result;
}

function copyToClipboard(str){
    const el = document.createElement('textarea');
    el.value = str;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
  };


$(".bank_controls .bank_control").on("click", function(){
    if(!$(this).hasClass("bank_control_selected")){
        let index = $(this).index();
        let panel = $(".bank_tab")[index];
        $(".bank_body_selected").removeClass("bank_body_selected");
        $(panel).addClass("bank_body_selected");
        $(".bank_control_selected").removeClass("bank_control_selected");
        $(this).addClass("bank_control_selected");
    }
});


$(".bank_transfer_confirm").on("click", function(){
    let money = $(".bank_transfer_amountInput").val();
    let target = $(".bank_transfer_targetInput").val();
    target = target.replace(/\s/g, "");
    if(/^\d+$/.test(target)){
        if($(".bank_transfer_title textarea").val().length >= 5 && $(".bank_transfer_title textarea").val().length <= 150){
            if(checkMoney(money)){
                mp.trigger("mainPanel_requestMoneyTransfer", target, parseInt(money), $(".bank_transfer_title textarea").val());
            }
            else{
                mp.trigger("showNotification", "Podano nieprawidłową wartość!");
            }
        }
        else{
            mp.trigger("showNotification", "Tytuł przelewu musi zawierać co najmniej 5 znaków!");
        }
    }
    else{
        mp.trigger("showNotification", "Podano błędny numer konta!");
    }
    
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

function transferCompleted(){
    mp.trigger("mainPanel_requestBankingData");
    $(".bank_control_selected").removeClass("bank_control_selected");
    $($(".bank_control")[0]).addClass("bank_control_selected");
    $(".bank_body_selected").removeClass("bank_body_selected");
    $($(".bank_tab")[0]).addClass("bank_body_selected");
}