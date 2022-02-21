var maxHeight = 240;
var maxWidth = maxHeight * 3 / 2;

var numVidsWide;
var numVidsTall;
var maxVids;
var width;
var height;

class Video {
    constructor(id) {
        this.id = id;
        this.width = width;
        this.height = height;
    }

    setRegion(region) {
        this.region = region;
    }

    setSize(width, height, scale = 1) {
        this.width = width * scale;
        this.height = height * scale;
    }

    getURL() {
        let tempURL = "https://mcleansfs";
        if (this.id <= 9)
            tempURL = tempURL + "5";
        else if (this.id <= 19)
            tempURL = tempURL + "1";
        else if (this.id <= 29)
            tempURL = tempURL + "2";
        else if (this.id <= 39)
            tempURL = tempURL + "3";
        else if (this.id <= 49)
            tempURL = tempURL + "4";
        else if (this.id <= 59)
            tempURL = tempURL + "5";
        else if (this.id === 70 || this.id === 71 || this.id === 72 || this.id === 73 || this.id === 74 || this.id === 75)
            tempURL = tempURL + "2";
        else if (this.id === 100 || this.id === 101)
            tempURL = tempURL + "2";
        else if (this.id === 79 || this.id === 80 || this.id === 82 || this.id === 93)
            tempURL = tempURL + "3";
        else if (this.id === 84)
            tempURL = tempURL + "5";
        else if (this.id >= 206 && this.id <= 210)
            tempURL = tempURL + "3";
        else if (this.id === 86 || this.id === 87)
            tempURL = tempURL + "2";
        else
            tempURL = tempURL + "1";
        tempURL = tempURL + ".us-east-1.skyvdn.com/rtplive/R1_";
        if (this.id < 100)
            tempURL = tempURL + "0";
        if (this.id < 10)
            tempURL = tempURL + "0";
        tempURL = tempURL + this.id;
        tempURL = tempURL + "/playlist.m3u8";

        return tempURL;
    }

    getSnapshotURL() {
        let tempURL = "https://tnsnapshots.com/thumbs/R1_";
        if (this.id < 100)
            tempURL = tempURL + "0";
        if (this.id < 10)
            tempURL = tempURL + "0";
        tempURL = tempURL + this.id + ".flv.png";

        return tempURL;
    }

    getVideoElement() {
        let video = document.createElement("video-js");
        video.id = 'video' + this.id;
        let source = document.createElement("source");
        source.src = this.getURL();
        video.appendChild(source);

        if (this.region === "showcase") {
            video.onclick = () => {
                showcase = null;
                clearAll();
                loadMain();
            };
            video.ontouchstart = () => {
                showcase = null;
                clearAll();
                loadMain();
            };
        } else {
            let id = this.id;
            video.onclick = () => {
                showcase = id;
                clearAll();
                loadShowcase(id);
            };
            video.ontouchstart = () => {
                showcase = id;
                clearAll();
                loadShowcase(id);
            };
        }

        return video;
    }

    start() {
        videojs('video' + this.id, {width: this.width, height: this.height});
    }

    getSnapshotElement() {
        let snapshot = document.createElement("img");
        snapshot.id = "knoxville-" + this.id;
        snapshot.src = this.getSnapshotURL();
        snapshot.loading = 'lazy';
        snapshot.width = 320;
        snapshot.height = 240;

        snapshot.onclick = () => {
            clearAll();
            let incidents = document.getElementById('incidents');
            incidents.style.display = 'inline';
            document.body.style.gridTemplateColumns = '4fr 1fr';
            document.body.style.overflow = 'hidden';

            let button = document.getElementById('showAll');
            button.innerHTML = "Show all cams";
            button.onclick = function () {
                clearAll();
                showAllCams();
            };
            showcase = this.id;
            showAll = null;
            startPlayers();
        };

        return snapshot;
    }
}

function addSnapshot(id) {
    let video = new Video(id);
    let div = document.getElementById("vid-images");
    div.appendChild(video.getSnapshotElement());
}

function showAllCams() {
    showAll = true;
    let incidents = document.getElementById('incidents');
    incidents.style.display = 'none';
    document.body.style.gridTemplateColumns = '1fr';

    document.body.style.overflow = 'auto';
    let button = document.getElementById('showAll');
    button.innerHTML = "Return to overview";
    button.onclick = () => {
        showAll = null;
        let incidents = document.getElementById('incidents');
        incidents.style.display = 'inline';
        document.body.style.gridTemplateColumns = '4fr 1fr';
        button.innerHTML = "Show all cams";
        button.onclick = () => {
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

    for (let i = 1; i <= 93; i++) {
        addSnapshot(i);
    }
    for (let i = 206; i <= 210; i++) {
        addSnapshot(i);
    }
    addSnapshot(100);
    addSnapshot(101);
}

function loadShowcase(p_showcase) {
    const showcase = parseInt(p_showcase);

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

    let support1, support2, video1, video2, video3;

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
        support1 = showcase - 1;
        support2 = showcase + 1;
    } else if (showcase === 1 || showcase === 33 || showcase === 53 || showcase === 58 || showcase === 67 || showcase === 76 || showcase === 84 || showcase === 90 || showcase === 100 || showcase === 71 || showcase === 59 || showcase === 206) {
        support1 = showcase + 1;
        if (showcase === 88) {
            support2 = support1;
            support1 = showcase - 3;
        } else if (showcase === 71) {
            support2 = support1;
            support1 = showcase - 2;
        } else if (showcase === 58) {
            support1 = 21;
            support2 = 75;
        } else if (showcase === 59) {
            support2 = support1;
            support1 = 75;
        }
    } else if (showcase === 32 || showcase === 52 || showcase === 57 || showcase === 66 || showcase === 69 || showcase === 89 || showcase === 93 || showcase === 101 || showcase === 73 || showcase === 210) {
        support1 = showcase - 1;
        if (showcase === 69) {
            support2 = showcase + 2;
        } else if (showcase === 57) {
            support2 = 18;
        }
    } else if (showcase === 85) {
        support1 = showcase - 2;
        support2 = showcase + 3;
    } else if (showcase === 74) {
        support1 = 22;
    } else if (showcase === 75) {
        support1 = 58;
        support2 = 59;
    }


    if (support1 && !support2) {
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

    addVideo(video1, "top");
    addVideo(video2, "top");
    addVideo(video3, "top");

    let div = document.getElementById("showcase");
    addVideo(showcase, "showcase", 3);

    if (support1) {
        addVideo(support1, "right", 2);
    } else {
        if (showcase !== 1)
            addVideo(2, "right");
        else
            addVideo(29, "right");
        addVideo(10, "right");

        div.appendChild(document.createElement("br"));
        addVideo(23, "right");
        addVideo(35, "right");
        div.appendChild(document.createElement("br"));
    }
    if (support2)
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

function clearAll() {
    clearDiv("main");
    clearDiv("top");
    clearDiv("showcase");
    clearDiv("right");
    clearDiv("vid-images");
}

function clearDiv(divID) {
    let div = document.getElementById(divID);
    if (div) {
        let nodes = div.childNodes;
        while (nodes.length > 0) {
            destroyNode(nodes[0]);
        }
    }
}

