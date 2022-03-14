function closeNicknameBrowser()
{
    mp.trigger("closeNicknameBrowser");
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

function confirmNicknameChange()
{
    mp.trigger("changeNickname", $(".nickInput").val());
}

function freeChange()
{
    $(".moneyText").text("0$");
}