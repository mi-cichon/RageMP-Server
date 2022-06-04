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

function initializeTabview(tabviewElem){
    let tabs = $(tabviewElem).children()[0];
    let view = $(tabviewElem).children()[1];
    let tabChildren = $(tabs).find(".panel_tabview_tab");

    let selected = $(tabs).find(".panel_tabview_tab.tabselected");
    let panel = $(selected).attr("source");
    $(view).load(`partials/${panel}`);

    $(tabChildren).on('click', function (){
        selected = $(tabs).find(".panel_tabview_tab.tabselected");
        $(selected).removeClass("tabselected");
        $(this).addClass("tabselected");
        let selectedPanel = $(this).attr("source");
        $(view).load(`partials/${selectedPanel}`);
    });
}

function copyToClipboard(str){
    const el = document.createElement('textarea');
    el.value = str;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
  };