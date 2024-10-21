let zIndexCounter = 1;
function toggleWidget(elemName) {
    const x = document.getElementById(elemName);
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
    bringToFront(x);
}
function dragElement(elmnt) {
    let pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    const titleBar = document.getElementById(elmnt.id + "title-bar");
    if (titleBar) {
        titleBar.addEventListener("mousedown", dragMouseDown);
    } else {
        elmnt.addEventListener("mousedown", dragMouseDown);
    }
    function dragMouseDown(e) {
        e.preventDefault();
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.addEventListener("mouseup", closeDragElement);
        document.addEventListener("mousemove", elementDrag);
        bringToFront(elmnt);
    }
    function elementDrag(e) {
        e.preventDefault();
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
        elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
    }
    function closeDragElement() {
        document.removeEventListener("mouseup", closeDragElement);
        document.removeEventListener("mousemove", elementDrag);
    }
}
function closeButtonClick(elemName) {
    const x = document.getElementById(elemName);
    x.style.display = "none";
}
document.addEventListener("keydown", function (evt) {
    if (evt.key === "Escape") {
        document.getElementById("onacalc").style.display = "none";
        document.getElementById("onahelp").style.display = "none";
        document.getElementById("onacalendar").style.display = "none";
    }
});
function bringToFront(elmnt) {
    zIndexCounter++;
    elmnt.style.zIndex = zIndexCounter;
}
document.addEventListener("DOMContentLoaded", function () {
    dragElement(document.getElementById("onacalc"));
    dragElement(document.getElementById("onahelp"));
    dragElement(document.getElementById("onacalendar"));
});