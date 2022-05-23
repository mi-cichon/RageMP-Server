let content = $(".panel_content");
let hourBonus = false; 
var currentPanelName = "";
$(window).on("load", () => {
    $(content).load("partials/controls.html");
});

$(".panel_tablet_button").on("click", function(){
    if(!hourBonus){
        $(content).load("partials/controls.html");
    }
});

window.addEventListener('DOMContentLoaded', function(){
    $(".panel_panel_show").css('opacity', 1);
});

$('.panel_social_discord').on('click', function(){
    copyToClipboard('discord.gg/2vrMdXsdRW');
    $('.panel_social_discord p').text("Copied!");
    setTimeout(function(){
        $('.panel_social_discord p').text("Discord");
    }, 2000);
});

  function setTime(time){
      $('.panel_tablet_time').text(time);
  }

  $(window).on('load', function(){
    $('.panel_tablet_body').css("left", "50%");
  });


  function closeTablet(){
    $('.panel_tablet_body').css("left", "-50%");
  }

function setHourBonusState(state){
    hourBonus = state;
}