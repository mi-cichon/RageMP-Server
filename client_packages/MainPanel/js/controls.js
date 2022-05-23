$(".panel_control").on('click', function (){
    let selectedPanel = $(this).attr("source");
    $(".panel_content").load(`partials/${selectedPanel}`);
});