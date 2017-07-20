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
    fadeAllContent(true);
    var loading = document.getElementById("loading");
    loading.style.visibility = "visible";
}

function fadeAllContent(fade) {
    var i;
    var op = fade ? "0.3" : "1";        
    var elements = document.querySelectorAll("table,h1");    
    for (i = 0; i < elements.length; i++) {        
        if (elements[i].id !== "loading") {
            elements[i].style.opacity = op;
        }
    }
}

function stepLoad() {
    var i;

    var extLinks = document.getElementsByClassName("externalLink");
    for (i = 0; i < extLinks.length; i++) {
        var index = i;
        extLinks[i].onclick = function() {
            window.openLink(extLinks[index].href);
        };
    }

    var pElem = document.getElementById("prevStep");
    var pHeight = pElem.clientHeight + 10;
    var cElem = document.getElementById("currentStep");
    cElem.style.top = pHeight + "px";

    try {
        setNewClassName("div.prevStep .shortcut", "shortcutDisabled", false);
        setNewClassName("div.prevStep code", "codeDisabled", false);
        setNewClassName("div.prevStep .menuItem", "menuItemDisabled", false);
        setNewClassName("div.prevStep button", "nextButtonDisabled", true);
        setNewClassName("div.prevStep .externalLink", "done", true);
        setNewClassName("div.prevStep h1", "done", false);
    } catch (e) {

    }

    try {
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
    } catch (e) {

    }

    try {
        var nxtButtons = document.getElementsByClassName("nextButton");
        for (i = 0; i < nxtButtons.length; i++) {
            if (checkParentsClassName(nxtButtons[i], "currentStep") === true) {                
                nxtButtons[i].onmouseover = function () {
                    window.external.MouseOverNextStepButton(true);
                }
                nxtButtons[i].onmouseout = function () {
                    window.external.MouseOverNextStepButton(false);
                }
            }
        }
    } catch (e) {

    }

    window.external.MouseOverNextStepButton(false);
    
    window.scroll(0, findPos(cElem));

    pageHasFullyLoaded();
}

function setNewClassName(obj, newClassName, disable) {
    var i;
    var objects = document.querySelectorAll(obj);

    for (i = 0; i < objects.length; i++) {
        objects[i].className = newClassName;
        if (disable) {
            objects[i].disabled = true;
        }
    }
}

function setNextStepButtonText(isFocusOnEditor, text) {
    var i;
    var buttons = document.querySelectorAll("div.currentStep button");

    for (i = 0; i < buttons.length; i++) {        
        var textBefore = buttons[i].firstChild.nodeValue;

        if (isFocusOnEditor === true) {
            buttons[i].innerHTML = textBefore + " (hit " + text + ")";            
        } else {
            buttons[i].innerHTML = textBefore.replace(" (hit " + text + ")", "");     
        }        
    }   
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
        } else {
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
        buttons[i].className = "runButtonDisabled";
    }
}

function enableButtons() {
    var buttons = document.getElementsByTagName("button");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].disabled = false;
        buttons[i].title = "";
        buttons[i].className = "runButton";
    }
}

function buttonClick() {
    window.external.ClickNextButton();
}

function runTutorial(id) {
    window.external.RunTutorial(id);
}

function openLink(s) {
    window.external.OpenLink(s);
    return false;
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

    fadeAllContent(false);
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

function closeSolution() {
    window.external.CloseSolution();
}

function pageHasFullyLoaded() {
    window.external.PageHasFullyLoaded();
}

