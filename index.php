<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8"/>
        <meta name="description" content="View all TDOT traffic cameras for the Knoxville and Knox County Tennessee area. Conveniently compiles the feeds in one place."/>
        <meta name="keywords" content="Knoxville, Knox County, Knox, traffic, TDOT, transportation, smart way, smartway" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0">

        <link rel="stylesheet" href="css/video-js.css" />
        <link rel="stylesheet" href="css/default.css" />
        <title>Knox County TN Traffic Center</title>
        <script type="text/javascript" src="js/support.js"></script>
        <script type="text/javascript">
            var showAll = null;
            var showcase = null;

            calcMaxPlayers();

            function resize() {
                if (showAll !== null) {
                    return;
                }

                let videos = document.getElementsByClassName('video-js');
<?php
if ($_POST["showcase_id"] > 0) {
    echo "numVidsWide=5;"
    . "numVidsTall=3;\n"
    . "if (maxVids > 12){"
    . "maxVids = 12;}\n"
    . "width = window.innerWidth * .78 / numVidsWide;"
    . "height = width * 2 / 3;\n"
    . "altHeight = (window.innerHeight - 70) / numVidsTall;"
    . "altWidth = altHeight * 3 / 2;\n"
    . "if (altWidth < width) {"
    . "width = altWidth;"
    . "height = altHeight;"
    . "}\n";
}
?>
                for (let video of videos) {
                    let object = videojs(video.id);
<?php
if ($_POST["showcase_id"] > 0) {
    echo "let scale = video.getAttribute('scale');";
    echo "object.width(width*scale);";
    echo "object.height(height*scale);";
} else if ($_POST["showcase_id"] == 0) {
    echo "object.pause();";
    echo "object.hide();";
}
?>
                }

<?php
if ($_POST["showcase_id"] == 0) {
    echo "loadMain();";
}
?>
            }

            function startPlayers() {
                videojs.options.controls = false;
                videojs.options.autoplay = 'muted';
                videojs.options.preload = 'auto';
                videojs.options.width = width;
                videojs.options.height = height;

                calcMaxPlayers();
<?php

function getShowcaseVideos($showcase) {
    if ($showcase == 3) {
        $support1 = 2;
        $support2 = 70;
    } else if ($showcase == 4) {
        $support1 = 70;
        $support2 = 5;
    } else if ($showcase == 70) {
        $support1 = 3;
        $support2 = 4;
    } else if (($showcase >= 2 && $showcase <= 31) || ($showcase >= 34 && $showcase <= 51) || ($showcase >= 54 && $showcase <= 56) || ($showcase >= 60 && $showcase <= 65) || ($showcase == 68) || ($showcase == 72) || ($showcase >= 77 && $showcase <= 83) || ($showcase >= 86 && $showcase <= 88) || ($showcase >= 91 && $showcase <= 92) || ($showcase >= 207 && $showcase <= 209)) {
        $support1 = $showcase - 1;
        $support2 = $showcase + 1;
    } else if ($showcase == 1 || $showcase == 33 || $showcase == 53 || $showcase == 58 || $showcase == 67 || $showcase == 76 || $showcase == 84 || $showcase == 90 || $showcase == 100 || $showcase == 71 || $showcase == 59 || $showcase == 206) {
        $support1 = $showcase + 1;
        if ($showcase == 88) {
            $support2 = $support1;
            $support1 = $showcase - 3;
        } else if ($showcase == 71) {
            $support2 = $support1;
            $support1 = $showcase - 2;
        } else if ($showcase == 58) {
            $support1 = 21;
            $support2 = 75;
        } else if ($showcase == 59) {
            $support2 = $support1;
            $support1 = 75;
        }
    } else if ($showcase == 32 || $showcase == 52 || $showcase == 57 || $showcase == 66 || $showcase == 69 || $showcase == 89 || $showcase == 93 || $showcase == 101 || $showcase == 73 || $showcase == 210) {
        $support1 = $showcase - 1;
        if ($showcase == 69) {
            $support2 = $showcase + 2;
        } else if ($showcase == 57) {
            $support2 = 18;
        }
    } else if ($showcase == 85) {
        $support1 = $showcase - 2;
        $support2 = $showcase + 3;
    } else if ($showcase == 74) {
        $support1 = 22;
    } else if ($showcase == 75) {
        $support1 = 58;
        $support2 = 59;
    }

    if ($support1 && !$support2) {
        $support2 = $support1;
        $support1 = null;
    }
    if (($showcase < 3 || $showcase > 4) && $showcase != 70) {
        $video1 = 70;
    } else {
        $video1 = 29;
    }
    if ($showcase < 15 || $showcase > 17) {
        $video2 = 16;
    } else {
        $video2 = 29;
    }
    if ($showcase < 38 || $showcase > 40) {
        $video3 = 39;
    } else {
        $video3 = 29;
    }

    if (!$support1) {
        if ($showcase == 1) {
            $support1 = 29;
        } else {
            $support1 = 2;
        }
        $support3 = 10;
        $support4 = 23;
        $support5 = 35;
    }
    return array($support1, $support2, $video1, $video2, $video3, $support3, $support4, $support5);
}

if ($_POST["showcase_id"] > 0) {
    $showcase = $_POST["showcase_id"];
    $videos = getShowcaseVideos($showcase);
    echo "numVidsWide=5;"
    . "numVidsTall=3;\n"
    . "if (maxVids > 12){"
    . "maxVids = 12;}\n"
    . "width = window.innerWidth * .8 / numVidsWide;"
    . "height = width * 2 / 3;\n"
    . "altHeight = (window.innerHeight - 70) / numVidsTall;"
    . "altWidth = altHeight * 3 / 2;\n"
    . "if (altWidth < width) {"
    . "width = altWidth;"
    . "height = altHeight;"
    . "}\n";
    echo "addVideo(" . $showcase . ",3);";
    echo "addVideo(" . $videos["2"] . ");";
    echo "addVideo(" . $videos["3"] . ");";
    echo "addVideo(" . $videos["4"] . ");";
    if ($videos["0"]) {
        if ($videos["5"]) {
            echo "addVideo(" . $videos["0"] . ");";
        } else {
            echo "addVideo(" . $videos["0"] . ",2);";
        }
    }
    if ($videos["5"]) {
        echo "addVideo(" . $videos["5"] . ");";
    }
    if ($videos["6"]) {
        echo "addVideo(" . $videos["6"] . ");";
    }
    if ($videos["7"]) {
        echo "addVideo(" . $videos["7"] . ");";
    }
    if ($videos["1"]) {
        echo "addVideo(" . $videos["1"] . ",2);";
    }
} else if ($_POST["showcase_id"] == 0) {
    echo "loadMain();";
}
echo "\n";
?>
            }

            function destroyNode(node) {
                if (node.nodeName == 'VIDEO-JS') {
                    videojs(node.id).dispose();
                } else {
                    node.remove();
                }
            }
        </script>

        <!-- Global site tag (gtag.js) - Google Analytics -->
        <script async src="https://www.googletagmanager.com/gtag/js?id=G-0XDM6XG7CW"></script>
        <script>
            window.dataLayer = window.dataLayer || [];
            function gtag() {
                dataLayer.push(arguments);
            }
            gtag('js', new Date());

            gtag('config', 'G-0XDM6XG7CW');
        </script>
        <!-- Global site tag (gtag.js) - Google Analytics -->

        <script data-ad-client="ca-pub-9117372724148268" async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js"></script>
    </head>
    <body onLoad="startPlayers()" onResize="resize()">
        <div>
            <h1>Knox County TN Traffic Center</h1>
            <form action="<?php echo $_SERVER["PHP_SELF"] ?>" method="POST">
                <input name="showcase_id" type="hidden" value="0"/>
                <!--<button id="showAll" onClick="clearAll();
                    showAllCams();">Show all cams</button>-->
            </form>

            <?php

            function getVideoServer($videoid) {
                if ($videoid <= 9 || ($videoid >= 50 and $videoid <= 59) || $videoid == 84) {
                    return "5";
                } else if ($videoid <= 19) {
                    return "1";
                } else if ($videoid <= 29 || in_array($videoid, [70, 71, 72, 73, 74, 75, 86, 87, 100, 101])) {
                    return "2";
                } else if ($videoid <= 39 || in_array($videoid, [79, 80, 82, 93]) || ($videoid >= 206 and $videoid <= 210)) {
                    return "3";
                } else if ($videoid <= 49) {
                    return "4";
                }
                return "1";
            }

            function getVideoURL($videoid) {
                $video_url = "https://mcleansfs" . getVideoServer($videoid) . ".us-east-1.skyvdn.com/rtplive/R1_" . str_pad($videoid, 3, "0", STR_PAD_LEFT) . "/playlist.m3u8";
                return $video_url;
            }

            if ($_POST["showcase_id"] > 0) {
                $showcase = $_POST["showcase_id"];

                $videos = getShowcaseVideos($showcase);
                $support1 = $videos["0"];
                $support2 = $videos["1"];
                $video1 = $videos["2"];
                $video2 = $videos["3"];
                $video3 = $videos["4"];
                $support3 = $videos["5"];
                $support4 = $videos["6"];
                $support5 = $videos["7"];

                echo "<div id=\"video-wrapper\"><div id=\"left\">\n"
                . "\t\t\t\t<div id=\"top\">\n"
                . "\t\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $video1 . ");document.forms[0].submit();'>"
                . "<video-js id='video" . $video1 . "' scale='1'><source src='" . getVideoURL($video1) . "' /></video-js></a>\n"
                . "\t\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $video2 . ");document.forms[0].submit();'>"
                . "<video-js id='video" . $video2 . "' scale='1'><source src='" . getVideoURL($video2) . "' /></video-js></a>\n"
                . "\t\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $video3 . ");document.forms[0].submit();'>"
                . "<video-js id='video" . $video3 . "' scale='1'><source src='" . getVideoURL($video3) . "' /></video-js></a>\n"
                . "\t\t\t\t</div>\n"
                . "\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(0);document.forms[0].submit();'>"
                . "<div id=\"showcase\"><video-js id='video" . $showcase . "' scale='3'><source src='" . getVideoURL($showcase) . "' /></video-js></a></div>\n"
                . "\t\t\t</div>\n"
                . "\t\t\t<div id=\"right\">\n"
                . "\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $support1 . ");document.forms[0].submit();'>";
                if ($support3) {
                    echo "<video-js id='video" . $support1 . "' scale='1'><source src='" . getVideoURL($support1) . "' /></video-js></a>\n";
                    echo "\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $support3 . ");document.forms[0].submit();'>"
                    . "<video-js id='video" . $support3 . "'><source src='" . getVideoURL($support3) . "' /></video-js></a>\n";
                } else {
                    echo "<video-js id='video" . $support1 . "' scale='2'><source src='" . getVideoURL($support1) . "' /></video-js></a>\n";
                }
                if ($support4) {
                    echo "\t\t\t\t<br/>\n"
                    . "\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $support4 . ");document.forms[0].submit();'>"
                    . "\t\t\t\t<video-js id='video" . $support4 . "' scale='1'><source src='" . getVideoURL($support4) . "' /></video-js></a>\n";
                }
                if ($support5) {
                    echo "\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $support5 . ");document.forms[0].submit();'>"
                    . "<video-js id='video" . $support5 . "' scale='1'><source src='" . getVideoURL($support5) . "' /></video-js></a>\n";
                }
                if ($support2) {
                    echo "\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $support2 . ");document.forms[0].submit();'>"
                    . "<video-js id='video" . $support2 . "' scale='2'><source src='" . getVideoURL($support2) . "' /></video-js></a>\n";
                }
                echo "\t\t\t</div></div>";
            } else if ($_POST["showcase_id"] == 0) {
                echo '<div id="video-wrapper">' . "\n";
                $videos = [2, 70, 69, 6, 8, 10, 13, 16, 18, 55, 20, 23, 26, 35, 39, 44, 50, 29, 31, 88];
                foreach ($videos as $videoid) {
                    echo "\t\t\t\t"
                    . "<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $videoid . ");document.forms[0].submit();'>"
                    . "<video-js id='video" . $videoid . "' style='display:hidden'>"
                    . "<source src='" . getVideoURL($videoid) . "' />"
                    . "</video-js>"
                    . "</a>"
                    . "\n";
                }
                echo "\t\t\t" . '</div>';
            }
            ?>

        </div>
        <iframe id="incidents" src="https://smartway.tn.gov/traffic/text/region/1/incidents" sandbox="allow-scripts allow-same-origin"></iframe>

        <script src="js/video.min.js"></script>
    </body>
