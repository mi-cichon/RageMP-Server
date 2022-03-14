
let changingPanel = false;

$(".panel_control").on('click', function (){
    if(!changingPanel){
        let index = $('.panel_controls .panel_control').index($(this));
        let currentPanel = $('.panel_panel_show');
        let selectedPanel = $('.panel_content').find(`.panel_panel:eq(${index})`);
        $(".panel_controls").css("display", "none");
        currentPanel.removeClass("panel_panel_show");
        selectedPanel.addClass("panel_panel_show");
        if(selectedPanel.hasClass("tetris")){
            tetris_run(document.querySelector('.tetris_game'));
        }
    }
});

$(".panel_tablet_button").on("click", function(){
    if(!listenToKey){
        $('*').removeClass("panel_panel_show");
        $(".panel_controls").css("display", "flex");
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

const copyToClipboard = str => {
    const el = document.createElement('textarea');
    el.value = str;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
  };


  function setTime(time){
      $('.panel_tablet_time').text(time);
  }

  $(window).on('load', function(){
    $('.panel_tablet_body').css("left", "50%");
  });


  function closeTablet(){
    $('.panel_tablet_body').css("left", "-50%");
  }