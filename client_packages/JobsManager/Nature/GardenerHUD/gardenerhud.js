function insertData(data){
    data = JSON.parse(data);
    $(".0").text(data[0]);
    $(".1").text(data[1]);
    $(".2").text(data[2]);
    $(".3").text(data[3]);
    $(".4").text(data[4]);
}

