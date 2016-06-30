function stepLoad() {
    var pElem = document.getElementById("prevStep");
    var pStepHeight = pElem.clientHeight;
    var cElem = document.getElementById("currentStep");

    if (pElem != null) {
        cElem.style.top = pStepHeight;
    }
}

function moveOutPrevStep() {
    var elem = document.getElementById("prevStep");
    var cElem = document.getElementById("currentStep");
    cElem.className = "done";
    var prevStepHeight = elem.clientHeight;
    var prevPos = 0;
    var currPos = 0;
    var id = setInterval(frame, 5);
    function frame() {
        if (Math.abs(prevPos) > prevStepHeight + 10) {
            clearInterval(id);
            cElem.style.top = 0 + "px";
            window.external.MoveOutPrevStepDone();
        }
        else {
            prevPos = prevPos - 3;
            currPos = prevPos + prevStepHeight;
            elem.style.top = prevPos + "px";
            cElem.style.top = currPos + "px";
        }
    }
}