function agreeToRunTutorial(buttonId, imgSrc) {
    var button = document.getElementById(buttonId);
    var imgId = "load" + buttonId;
    var img = document.getElementById(imgId);

    if (img == null) {
        img = createImage(imgId, imgSrc, "Loading...", "");
        img.style.float = "right";
        button.parentNode.insertBefore(img, button);
    } else {
        img.style.visibility = "visible";
    }    
}

function agreeToRunTutorial() {
    var loading = document.getElementById("loading");
    loading.style.visibility = "visible";
}

function stepLoad() {
    var pElem = document.getElementById("prevStep");
    var pHeight = pElem.clientHeight;
    var cElem = document.getElementById("currentStep");
    cElem.style.top = pHeight + "px";

    var buttons = document.querySelectorAll("div.prevStep button");
    var i;
    for (i = 0; i < buttons.length; i++) {
        buttons[i].disabled = true;        
        buttons[i].style.textDecoration = "line-through";
    }

    var navLinks = document.getElementsByClassName("navigate");

    for (i = 0; i < navLinks.length; i++) {
        if (checkParentsClassName(navLinks[i], "prevStep") === true) {            
            navLinks[i].className = "noNavigate";
            navLinks[i].onclick = window.doNothing;
        }
        if (checkParentsClassName(navLinks[i], "currentStep") === true) {
            navLinks[i].onclick = runStepNavigation;           
        }
    }

    window.scroll(0, findPos(cElem));
}

function checkParentsClassName(obj, className) {
    var parent = obj.parentNode;
    if (parent.className === className) {        
        return true;
    }
    if (parent.parentElement != null) {
        return checkParentsClassName(parent, className);        
    }
    return false;
}

function showLog(array) {
    var result = "";
    for (var i = 0; i < array.length; i++) {
        result.concat(array[i]);
        result.concat(" | ");        
    }
    alert(result);
}

function doNothing() {}

function findPos(obj) {
    var curtop = 0;
    if (obj.offsetParent) {
        do {
            curtop += obj.offsetTop;
        } while (obj === obj.offsetParent);
        return [curtop];
    }
    return [curtop];
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

function disableButtons() {
    var buttons = document.getElementsByTagName("button");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].disabled = true;
        buttons[i].title = "You must first close the currently open tutorial";
    }
}

function enableButtons() {
    var buttons = document.getElementsByTagName("button");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].disabled = false;
        buttons[i].title = "";
    }
}

function buttonClick() {    
    window.external.ClickNextButton();
}

function runTutorial(id) {    
    window.external.RunTutorial(id);
}

function runStepNavigation() {
    window.external.RunStepNavigation();
    return false;
}

function showLoadingImg(id, visible) {
    var img = document.getElementById(id);
    img.style.visibility = (visible ? "visible" : "hidden");
}

function hideImages() {
    var images = document.getElementsByTagName("img");
    for (var i = 0; i < images.length; i++) {
        images[i].style.visibility = "hidden";        
    }

    // test
    var loading = document.getElementById("loading");
    loading.style.visibility = "hidden";
}

function createImage(id, src, alt, title) {   
    var img = document.createElement("img");
    img.id = id;
    img.src = src;
    if (alt != null) img.alt = alt;
    if (title != null) img.title = title;
    return img;
}