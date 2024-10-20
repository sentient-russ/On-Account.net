function toggleWidget(elemName){
  var x = document.getElementById(elemName);
  if (x.style.display == "none"){
    x.style.display = "block";
  }
  else{
    x.style.display = "none";
  }
}

function dragElement(elmnt) {
  var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
  if (document.getElementById(elmnt.id + "title-bar")) {
    document.getElementById(elmnt.id + "title-bar").onmousedown = dragMouseDown;
  } else {
    elmnt.onmousedown = dragMouseDown;
  }

  function dragMouseDown(e) {
    e = e || window.event;
    e.preventDefault();
    pos3 = e.clientX;
    pos4 = e.clientY;
    document.onmouseup = closeDragElement;
    document.onmousemove = elementDrag;
  }

  function elementDrag(e) {
    e = e || window.event;
    e.preventDefault();
    pos1 = pos3 - e.clientX;
    pos2 = pos4 - e.clientY;
    pos3 = e.clientX;
    pos4 = e.clientY;
    elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
    elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
  }
  function closeDragElement() {
    document.onmouseup = null;
    document.onmousemove = null;
  }
}

function closeButtonClick(elemName){
  var x = document.getElementById(elemName);
  x.style.display = "none";
}

document.onkeydown = function(evt) {
  evt = evt || window.event;
  if (evt.keyCode == 27) {
    document.getElementById("onacalc").style.display = "none";
    document.getElementById("onahelp").style.display = "none";
    document.getElementById("onacalendar").style.display = "none";
  }
};

dragElement(document.getElementById("onacalc"));
dragElement(document.getElementById("onahelp"));
dragElement(document.getElementById("onacalendar"));