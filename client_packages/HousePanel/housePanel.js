function confirm()
{
    let text = $(".daysInput").val();
    if(!isNumeric(text)){
        showError("Podaj poprawną liczbę");
    }
    else{
        if(parseInt(text) > 14 || parseInt(text) < 1)
        {
            showError("Dom można wynająć na maksymalnie 14 dni naprzód")
        }
        else
        {
            mp.trigger("confirmHouseBuy", parseInt(text));
        }
    }
}

function extend()
{
    let text = $(".daysInput").val();
    if(!isNumeric(text)){
        showError("Podaj poprawną liczbę");
    }
    else{
        if(parseInt(text) > 14 || parseInt(text) < 1)
        {
            showError("Dom można wynająć na maksymalnie 14 dni naprzód")
        }
        else
        {
            mp.trigger("confirmHouseExtend", parseInt(text));
        }
    }
}
function isNumeric(str) {
    if (typeof str != "string") return false
    return !isNaN(str) && !isNaN(parseFloat(str)) 
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
function closeHouseBrowser()
{
    mp.trigger("closeHousePanelBrowser");
}

function changeDate(datetext)
{
    let date = $(".dateText").text(datetext);
}

function setPrice(pricetext)
{
    let price = $(".moneyText").text(pricetext + "$");
}

function giveHouseUp()
{
    mp.trigger("giveHouseUp");
}