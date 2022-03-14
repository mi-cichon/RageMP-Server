let currentAnswer = null;
let timeInterval = null;
let rightAnswers = 0;
let currentQuestion = 0;
let questions = rollQuestions();
console.log(questions);
$(".answer").on("click", function(){
    if(currentAnswer != null){
        $("." + currentAnswer).parent().parent().removeClass("checked");
    }
    $(this).parent().parent().addClass("checked");
    if($(this).hasClass("a")){
        currentAnswer = "a";
    }
    else if($(this).hasClass("b")){
        currentAnswer = "b";
    }
    else if($(this).hasClass("c")){
        currentAnswer = "c";
    }
});

function setQuestion(id){
    if(currentAnswer != null){
        $("." + currentAnswer).parent().parent().removeClass("checked");
        currentAnswer = null;
    }
    if(currentQuestion == 11){
        $(".button_select").attr("value", "Zakończ egzamin");
    }
    $(".time h3").text(`Pytanie ${id + 1}/12`);
    $(".question").text(questions[id].question);
    $(".a").text(questions[id].a);
    $(".b").text(questions[id].b);
    $(".c").text(questions[id].c);
    startQuestionTimer();
}

function startQuestionTimer(){
    let timeLeft = 45;
    $(".time b").text(`Pozostały czas na odpowiedź: ${timeLeft} sekund`);
    timeInterval = setInterval(function(){
        timeLeft--;
        if(timeLeft != -1){
            $(".time b").text(`Pozostały czas na odpowiedź: ${timeLeft} sekund`);
        }
        else{
            clearInterval(timeInterval);
            timeInterval = null;
            if(currentQuestion == 11){
                end();
            }
            else{
                currentQuestion++;
                setQuestion(currentQuestion);
            }
        }
    }, 1000);
}

function end(){
    mp.trigger("endLicenceTest", rightAnswers);
}

function rollQuestions(){
    let p = pytania.sort(function () {
        return Math.random() - 0.5;
    });
    p = p.splice(0, 12);
    let pyt = [];
    p.forEach(pp => {
        pyt.push({
            question: pp.question,
            a: pp.a,
            b: pp.b,
            c: pp.c,
            answer: pp.answer
        });
    })
    return pyt;
}


function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

$(".test .button_select").on("click", function() {
    if(currentAnswer != null){
        if(timeInterval != null){
            clearInterval(timeInterval);
            timeInterval = null;
        }
        if(currentAnswer == questions[currentQuestion].answer.toLowerCase()){
            rightAnswers++;
        }
        if(currentQuestion == 11){
            end();
        }
        else{
            currentQuestion++;
            setQuestion(currentQuestion);
        }
    }
});

$(".info .button_select").on("click", function() {
    mp.trigger("licenceCheckMoney");
});

$(".button_cancel").on("click", function(){
    mp.trigger("closeLicenceBrowser");
});

function startTest(){
    $(".info").css("display", "none");
    $(".test").css("display", "flex");
    setQuestion(0);
}