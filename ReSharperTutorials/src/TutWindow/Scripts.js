function stepLoad() {
    var pElem = document.getElementById("prevStep");
    var pHeight = pElem.clientHeight;
    var cElem = document.getElementById("currentStep");
    cElem.style.top = pHeight + "px";
}

function moveOutPrevStep() {
    var pElem = document.getElementById("prevStep");
    var cElem = document.getElementById("currentStep");
    cElem.className = "done";
    var prevStepHeight = pElem.clientHeight;
    var prevPos = 0;
    var currPos = 0;
    var id = setInterval(frame, 5);
    function frame() {
        if (Math.abs(prevPos) > prevStepHeight - 30) {
            clearInterval(id);
            cElem.style.top = 0 + "px";
            window.external.MoveOutPrevStepDone();            
        }
        else {
            prevPos = prevPos - 5;
            currPos = prevPos + prevStepHeight;
            pElem.style.top = prevPos + "px";
            cElem.style.top = currPos + "px";
        }
    }
}

function buttonClick() {    
    window.external.ClickNextButton();
}