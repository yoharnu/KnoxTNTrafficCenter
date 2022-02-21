var maxHeight = 240;
var maxWidth = maxHeight * 3 / 2;

var numVidsWide;
var numVidsTall;
var maxVids;
var width;
var height;

function loadMain() {
    calcMaxPlayers();

    if (maxVids >= 10)
        addVideo(2);
    addVideo(70);
    if (maxVids >= 18)
        addVideo(69);
    if (maxVids >= 5)
        addVideo(6);
    if (maxVids >= 8)
        addVideo(8);
    if (maxVids >= 7)
        addVideo(10);
    if (maxVids >= 11)
        addVideo(13);
    addVideo(16);
    if (maxVids >= 9)
        addVideo(18);
    if (maxVids >= 20)
        addVideo(55);
    if (maxVids >= 6)
        addVideo(20);
    if (maxVids >= 17)
        addVideo(23);
    if (maxVids >= 15)
        addVideo(26);
    if (maxVids >= 12)
        addVideo(35);
    addVideo(39);
    if (maxVids >= 16)
        addVideo(44);
    if (maxVids >= 13)
        addVideo(50);
    addVideo(29);
    if (maxVids >= 14)
        addVideo(31);
    if (maxVids >= 19)
        addVideo(88);
}

function addVideo(id, scale = 1) {
    let object = videojs('video' + id);
    object.show();
    object.height(height * scale);
    object.width(width * scale);
    if (object.paused())
        object.play();
}

function calcMaxPlayers() {
    numVidsWide = Math.floor(window.innerWidth * .8 / maxWidth);
    numVidsTall = Math.floor((window.innerHeight - 70) / maxHeight);
    console.log('numVidsWide ' + numVidsWide);
    console.log('numVidsTall ' + numVidsTall);
    maxVids = numVidsWide * numVidsTall;

    if (maxVids > 20) {
        maxVids = 20;

        if (numVidsWide > 5 && numVidsTall >= 4) {
            numVidsWide = 5;
            numVidsTall = 4;
        }
        if (numVidsTall > 4 && numVidsWide >= 5) {
            numVidsWide = 5;
            numVidsTall = 4;
        }
        if (numVidsWide > 4 && numVidsTall >= 5) {
            numVidsWide = 4;
            numVidsTall = 5;
        }
        if (numVidsTall > 5 && numVidsWide >= 4) {
            numVidsWide = 4;
            numVidsTall = 5;
        }
    }

    width = window.innerWidth * .8 / numVidsWide;
    height = width * 2 / 3;
    const altHeight = (window.innerHeight - 70) / numVidsTall;
    const altWidth = altHeight * 3 / 2;
    if (altWidth < width) {
        width = altWidth;
        height = altHeight;
    }
}

