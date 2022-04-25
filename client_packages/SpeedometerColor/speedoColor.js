var color1 = [0, 204, 153];


$(".speedoColor_preview").on("click", function (){
    mp.trigger('speedoColor_preview', RGBToHex(color1[0], color1[1], color1[2]));
});

$(".speedoColor_confirm").on("click", function (){
    mp.trigger('speedoColor_confirm', RGBToHex(color1[0], color1[1], color1[2]));
});

$(".speedoColor_close").on("click", function (){
    mp.trigger('speedoColor_closeBrowser');
});

function RGBToHex(r,g,b) {
    r = r.toString(16);
    g = g.toString(16);
    b = b.toString(16);
  
    if (r.length == 1)
      r = "0" + r;
    if (g.length == 1)
      g = "0" + g;
    if (b.length == 1)
      b = "0" + b;
  
    return "#" + r + g + b;
  }