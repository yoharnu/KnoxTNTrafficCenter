var maxHeight = 240;
var maxWidth = maxHeight * 3 / 2;

var numVidsWide;
var numVidsTall;
var maxVids;
var width;
var height;

function addSnapshot(id) {
    var snapshot = document.createElement("img");
    snapshot.id = "knoxville-" + id;
    var srcStr = "https://tnsnapshots.com/thumbs/R1_";
    if (id < 100)
        srcStr = srcStr + "0";
    if (id < 10)
        srcStr = srcStr + "0";
    srcStr = srcStr + id + ".flv.png";
    snapshot.src = srcStr;
    snapshot.style.width = 320;
    snapshot.style.height = 240;

    snapshot.onclick = function () {
        clearAll();
        var incidents = document.getElementById('incidents');
        incidents.style.display = 'inline';
        document.body.style.overflow = 'hidden';

        var button = document.getElementById('showAll');
        button.innerHTML = "Show all cams";
        button.onclick = function () {
            clearAll();
            showAllCams();
        };
        showcase = id;
        showAll = null;
        startPlayers();
    };

    var div = document.getElementById("main");
    div.appendChild(snapshot);
}

function showAllCams() {
    showAll = true;
    var incidents = document.getElementById('incidents');
    incidents.style.display = 'none';

    document.body.style.overflow = 'auto';
    var button = document.getElementById('showAll');
    button.innerHTML = "Return to overview";
    button.onclick = function () {
        showAll = null;
        var incidents = document.getElementById('incidents');
        incidents.style.display = 'inline';
        button.innerHTML = "Show all cams";
        button.onclick = function () {
            clearAll();
            showAllCams();
        };
        showcase = null;
        showAll = null;
        clearAll();
        document.body.style.overflow = 'hidden';

        calcMaxPlayers();
        startPlayers();
    };

    var i;
    for (i = 1; i <= 93; i++) {
        addSnapshot(i);
    }
    for (i = 206; i <= 210; i++) {
        addSnapshot(i);
    }
    addSnapshot(100);
    addSnapshot(101);
}

function loadShowcase(p_showcase) {
    var showcase = parseInt(p_showcase);

    calcMaxPlayers();

    numVidsWide = 5;
    numVidsTall = 4;

    if (maxVids > 12)
        maxVids = 12;
    width = window.innerWidth * .8 / numVidsWide;
    height = width * 2 / 3;
    altHeight = (window.innerHeight - 70) / numVidsTall;
    altWidth = altHeight * 3 / 2;
    if (altWidth < width) {
        width = altWidth;
        height = altHeight;
    }

    var support1;
    var support2;
    var video1;
    var video2;
    var video3;

    if (showcase === 3) {
        support1 = 2;
        support2 = 70;
    } else if (showcase === 4) {
        support1 = 70;
        support2 = 5;
    } else if (showcase === 70) {
        support1 = 3;
        support2 = 4;
    } else if ((showcase >= 2 && showcase <= 31) || (showcase >= 34 && showcase <= 51) || (showcase >= 54 && showcase <= 56) || (showcase >= 60 && showcase <= 65) || (showcase === 68) || (showcase === 72) || (showcase >= 77 && showcase <= 83) || (showcase >= 86 && showcase <= 88) || (showcase >= 91 && showcase <= 92) || (showcase >= 207 && showcase <= 209)) {
        support1 = parseInt(showcase) - 1;
        support2 = parseInt(showcase) + 1;
    } else if (showcase === 1 || showcase === 33 || showcase === 53 || showcase === 58 || showcase === 67 || showcase === 76 || showcase === 84 || showcase === 90 || showcase === 100 || showcase === 71 || showcase === 59 || showcase === 206) {
        support1 = parseInt(showcase) + 1;
        if (showcase === 88) {
            support2 = support1;
            support1 = parseInt(showcase) - 3;
        } else if (showcase === 71) {
            support2 = support1;
            support1 = parseInt(showcase) - 2;
        } else if (showcase === 58) {
            support1 = 21;
            support2 = 75;
        } else if (showcase === 59) {
            support2 = support1;
            support1 = 75;
        }
    } else if (showcase === 32 || showcase === 52 || showcase === 57 || showcase === 66 || showcase === 69 || showcase === 89 || showcase === 93 || showcase === 101 || showcase === 73 || showcase === 210) {
        support1 = parseInt(showcase) - 1;
        if (showcase === 69) {
            support2 = parseInt(showcase) + 2;
        } else if (showcase === 57) {
            support2 = 18;
        }
    } else if (showcase === 85) {
        support1 = parseInt(showcase) - 2;
        support2 = parseInt(showcase) + 3;
    } else if (showcase === 74) {
        support1 = 22;
    } else if (showcase === 75) {
        support1 = 58;
        support2 = 59;
    }


    if (typeof support2 === 'undefined' && typeof support1 !== 'undefined') {
        support2 = support1;
        support1 = null;
    }
    if ((showcase < 3 || showcase > 4) && showcase !== 70) {
        video1 = 70;
    } else {
        video1 = 29;
    }
    if (showcase < 15 || showcase > 17) {
        video2 = 16;
    } else {
        video2 = 29;
    }
    if (showcase < 38 || showcase > 40) {
        video3 = 39;
    } else {
        video3 = 29;
    }

    addVideo(video1, "top", 1);
    addVideo(video2, "top", 1);
    addVideo(video3, "top", 1);

    var div = document.getElementById("showcase");
    addVideo(showcase, "showcase", 3);

    if (typeof support1 === 'undefined' || support1 === null) {
        if (showcase !== 1)
            addVideo(2, "right", 1);
        else
            addVideo(29, "right", 1);
        addVideo(10, "right", 1);

        div.appendChild(document.createElement("br"));
        addVideo(23, "right", 1);
        addVideo(35, "right", 1);
        div.appendChild(document.createElement("br"));
    } else {
        addVideo(support1, "right", 2);
    }
    if (typeof support2 !== 'undefined')
        addVideo(support2, "right", 2);
}

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
    var altHeight = (window.innerHeight - 70) / numVidsTall;
    var altWidth = altHeight * 3 / 2;
    if (altWidth < width) {
        width = altWidth;
        height = altHeight;
    }
}

function clearAll() {
    clearDiv("main");
    clearDiv("top");
    clearDiv("showcase");
    clearDiv("right");
}

function clearDiv(divID) {
    var div = document.getElementById(divID);
    var nodes = div.childNodes;
    while (nodes.length > 0) {
        destroyNode(nodes[0]);
    }
}

function resize() {
    if (showAll !== null) {
        return;
    }

    clearAll();
    calcMaxPlayers();
    startPlayers();
}

function getVideoURL(id) {
    var srcStr = "https://mcleansfs";
    if (id <= 9)
        srcStr = srcStr + "5";
    else if (id <= 19)
        srcStr = srcStr + "1";
    else if (id <= 29)
        srcStr = srcStr + "2";
    else if (id <= 39)
        srcStr = srcStr + "3";
    else if (id <= 49)
        srcStr = srcStr + "4";
    else if (id <= 59)
        srcStr = srcStr + "5";
    else if (id === 70 || id === 71 || id === 72 || id === 73 || id === 74 || id === 75)
        srcStr = srcStr + "2";
    else if (id === 100 || id === 101)
        srcStr = srcStr + "2";
    else if (id === 79 || id === 80 || id === 82 || id === 93)
        srcStr = srcStr + "3";
    else if (id === 84)
        srcStr = srcStr + "5";
    else if (id >= 206 && id <= 210)
        srcStr = srcStr + "3";
    else if (id === 86 || id === 87)
        srcStr = srcStr + "2";
    else
        srcStr = srcStr + "1";
    srcStr = srcStr + ".us-east-1.skyvdn.com/rtplive/R1_";
    if (id < 100)
        srcStr = srcStr + "0";
    if (id < 10)
        srcStr = srcStr + "0";
    srcStr = srcStr + id;
    srcStr = srcStr + "/playlist.m3u8";
    return srcStr;
}