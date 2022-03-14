let speedStep = 0;
const defaultScale = 1.5;
let maxSpeed = 0;
;
let body = $(".speedometer_Body");
function instantiate(maxS, petrol, scale, carName, trip){
    if(trip){
        $(".speedometer_Trip").css("display", "flex");
    }
    $("html").css("font-size", (defaultScale + (scale / 100) - 0.5).toString() + "vh");
    if(!petrol){
        $(".speedometer_Petrol").css("display", "none");
        $(".speedometer_PetrolBack").css("display", "none");
        $(".speedometer_PetrolAmount").css("display", "none");
    }
    maxSpeed = maxS;
    createLines(maxS);
    $(".speedometer_Name").text(carName);
}

function setVars(speed, rpm, petrol, street, trip){
    if(trip != -1)
        setTrip(trip);
    rpmProgress.set(rpm);
    let s = $(".speed");
    s.text(Math.round(speed));
    let needle = $(".speedometer_Pointer");
    let rotation = `rotate(${(speed * speedStep) - 50}deg)`
    needle.css("transform", "translate(-50%, -50%) " + rotation);
    if(petrol > 0){
        petrolProgress.set(petrol * 2/9);
        $(".speedometer_PetrolAmount").text(Math.round(petrol * 100) + "%");
    }
    $(".speedometer_Street").text(street);
}

function setScale(scale){
    $("html").css("font-size", (defaultScale + (scale / 100) - 0.5).toString() + "vh");
    $('.label').remove();
    $('.line').remove();
    createLines(maxSpeed);
}



function createLine(angle, long){
    let position = getPositionByAngle(angle, 0.88);
    let line = `<div class="line ${long ? "long" : ""}" style="left: ${position.x}px; top: ${position.y}px; transform: translate(-50%, -50%) rotate(${angle}deg )"></div>`
    body.append(line);
}

function createLabel(angle, text){
    let position = getPositionByAngle(angle, 0.71);
    let label = `<div class="label" style="left: ${position.x}px; top: ${position.y}px">${text}</div>`
    body.append(label);
}

function getPositionByAngle(angle, scale){
    let radius =  0.5 * $(".speedometer_Body").width();
    let position = {
        x: -1 * radius * Math.sin(angle * (Math.PI / 180)) * scale,
        y: radius * Math.cos(angle * (Math.PI / 180)) * scale
    }
    position.x += radius;
    position.y += radius;
    return position;
}


function createLines(maxSpeed){
    let step = parseFloat((270/((maxSpeed / 20))).toFixed(1));
    let fractionStep = parseFloat((step/4).toFixed(1));
    step = parseInt(fractionStep * 40);
    fractionStep *= 10;

    for(let i = 400; i <= 3125; i += fractionStep){
        createLine(i/10, (i-400)%(step/2) == 0);
        if((i-400)%step == 0){
            createLabel(i/10, (i-400)/step * 20);
        }
    }

    speedStep = 270/maxSpeed;
}



  let rpmProgress = new ProgressBar.SemiCircle('.speedometer_RPM', {
    strokeWidth: 3,
    easing: 'easeInOut',
    duration: 10,
    color: 'url(#gradient)',
    trailColor: 'rgba(51,51,51,1)',
    trailWidth: 2,
    svgStyle: null
  });

  let linearGradient = `
  <defs>
    <linearGradient id="gradient" x1="0%" y1="0%" x2="100%" y2="0%" gradientUnits="userSpaceOnUse">
      <stop offset="50%" stop-color="#0c9"/>
      <stop offset="80%" stop-color="#520008"/>
    </linearGradient>
  </defs>
`

rpmProgress.svg.insertAdjacentHTML('afterBegin', linearGradient);

let petrolBackProgress = new ProgressBar.SemiCircle('.speedometer_PetrolBack', {
    strokeWidth: 1,
    easing: 'easeInOut',
    duration: 10,
    color: '#323635',
    trailColor: 'rgba(255,0,0,0)',
    trailWidth: 1,
    svgStyle: null,
  });

let petrolProgress = new ProgressBar.SemiCircle('.speedometer_Petrol', {
    strokeWidth: 2,
    easing: 'easeInOut',
    duration: 10,
    color: '#fff',
    trailColor: 'rgba(255,0,0,0)',
    trailWidth: 1,
    svgStyle: null,
  });

  petrolProgress.set(0);

  petrolBackProgress.set(2/9);

function setTrip(trip){
    trip = trip.toFixed(1);
    if(parseInt(trip) == trip){
        trip = parseInt(trip).toString() + "0";
    }
    else{
        trip = trip.toString().replace(".", "");
    }
    let index = 0;
    for(let i = trip.length - 1; i >= 0; i--){
        console.log(i);
        $(`.trip-${index} p`).text(trip[i]);
        index++;
    }
}