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

            function startPlayers() {
                videojs.options.controls = false;
                videojs.options.autoplay = 'muted';
                videojs.options.preload = 'auto';
                videojs.options.width = width;
                videojs.options.height = height;

                calcMaxPlayers();

                if (showcase) {
                    loadShowcase(showcase);
                } else {
                    loadMain();
                }
            }

            function destroyNode(node) {
                if (node.nodeName === 'VIDEO-JS') {
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
            <!--<button id="showAll" onClick="clearAll();
                    showAllCams();">Show all cams</button>-->
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

            /* <div id="video-wrapper">
              <div id="main"></div>
              <div id="left">
              <div id="top">
              </div>
              <div id="showcase">
              </div>
              </div>

              <div id="right">
              </div>

              <div id="bottom">
              </div>
              </div>
              <div id="vid-images">

              </div> */

            if ($_POST["showcase_id"]) {
                
            } else if ($_POST["showAll"]) {
                
            } else {
                echo '<div id="video-wrapper">' . "\n";
                $videos = [2, 70, 69, 6, 8, 10, 13, 16, 18, 55, 20, 23, 26, 35, 39, 44, 50, 29, 31, 88];
                foreach ($videos as $videoid) {


                    echo "\t\t\t\t<a href='javascript:document.forms[0].showcase_id.value = parseInt(" . $videoid . ");document.forms[0].submit();'><video-js id='video" . $videoid . "' style='display:hidden'>";
                    echo "<source src='" . getVideoURL($videoid) . "' />";
                    echo "</video-js></a>\n";
                }
                echo "\t\t\t" . '</div>';
            }
            ?>

        </div>
        <iframe id="incidents" src="https://smartway.tn.gov/traffic/text/region/1/incidents" sandbox="allow-scripts allow-same-origin"></iframe>

        <script src="js/video.min.js"></script>
    </body>
