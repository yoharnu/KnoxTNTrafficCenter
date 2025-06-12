function ShowModal(event) {

    var button = event.relatedTarget;

    var URL = button.getAttribute('data-bs-video-url');
    var width = button.getAttribute('data-bs-video-width');
    var height = button.getAttribute('data-bs-video-height');
    var title = button.getAttribute('data-bs-title');

    var modalTitle = videoModal.querySelector('.modal-title');
    modalTitle.textContent = title;

    var modalBody = videoModal.getElementsByClassName('modal-body')[0];
    
    // Create a container div for better positioning of elements
    var containerDiv = document.createElement('div');
    containerDiv.style.position = 'relative';
    containerDiv.style.width = '100%';
    // Use dark background that respects theme mode
    containerDiv.style.backgroundColor = 'var(--card-bg, #23272b)';
    containerDiv.classList.add('video-container');
    
    // Determine aspect ratio from width and height if available
    if (width && height) {
        var aspectRatio = parseFloat(width) / parseFloat(height);
        console.log("Detected aspect ratio:", aspectRatio);
        
        // Apply appropriate aspect ratio class
        if (Math.abs(aspectRatio - 1.33) < 0.1) { // Close to 4:3 (1.33)
            containerDiv.classList.add('aspect-4-3');
            console.log("Applied 4:3 aspect ratio");
        } else if (Math.abs(aspectRatio - 1.78) < 0.1) { // Close to 16:9 (1.78)
            containerDiv.classList.add('aspect-16-9');
            console.log("Applied 16:9 aspect ratio");
        } else {
            // Custom aspect ratio
            containerDiv.style.aspectRatio = `${width}/${height}`;
            console.log("Applied custom aspect ratio:", `${width}/${height}`);
        }
    } else {
        // Default to 4:3 as mentioned most videos are this ratio
        containerDiv.classList.add('aspect-4-3');
        console.log("Applied default 4:3 aspect ratio");
    }
    
    // Clear any existing content in the modal body
    while (modalBody.children.length > 0) {
        modalBody.removeChild(modalBody.children[0]);
    }
    
    // Create video element with modern compatibility settings
    var videoElement = document.createElement('video');
    videoElement.id = 'modalVideo';
    videoElement.classList.add('video-player');
    videoElement.style.width = '100%';
    videoElement.controls = true;
    videoElement.autoplay = true;
    videoElement.muted = true;
    videoElement.playsInline = true;
    videoElement.crossOrigin = 'anonymous'; // Add cross-origin attribute
    
    // Add loading indicator
    var loadingIndicator = document.createElement('div');
    loadingIndicator.id = 'videoLoadingIndicator';
    loadingIndicator.style.position = 'absolute';
    loadingIndicator.style.top = '50%';
    loadingIndicator.style.left = '50%';
    loadingIndicator.style.transform = 'translate(-50%, -50%)';
    loadingIndicator.style.color = 'var(--text-color, white)';
    loadingIndicator.style.fontSize = '1.2rem';
    loadingIndicator.style.zIndex = '20';
    loadingIndicator.textContent = 'Loading video...';
    
    containerDiv.appendChild(videoElement);
    containerDiv.appendChild(loadingIndicator);
    modalBody.appendChild(containerDiv);
    
    // Handle m3u8 videos with HLS.js if supported
    if (URL.toLowerCase().includes('.m3u8')) {
        // Show debugging info
        console.log("Processing m3u8 stream");
        
        // Check if HLS.js is supported
        if (typeof Hls !== 'undefined' && Hls.isSupported()) {
            console.log("HLS.js is supported");
            var hls = new Hls({
                debug: false,
                enableWorker: true,
                lowLatencyMode: true,
                backBufferLength: 90
            });
            
            hls.on(Hls.Events.ERROR, function(event, data) {
                console.error("HLS error:", data);
                if (data.fatal) {
                    showErrorMessage(containerDiv, loadingIndicator, "Error loading video stream. Please try again later.");
                }
            });
            
            hls.on(Hls.Events.MANIFEST_PARSED, function() {
                console.log("HLS manifest parsed, attempting to play");
                loadingIndicator.style.display = 'none';
                videoElement.play().catch(function(error) {
                    console.warn("Autoplay prevented:", error);
                    showPlayButton(containerDiv, videoElement);
                });
            });
            
            // Listen for video metadata to adjust aspect ratio if needed
            videoElement.addEventListener('loadedmetadata', function() {
                adjustAspectRatio(containerDiv, videoElement);
            });
            
            hls.loadSource(URL);
            hls.attachMedia(videoElement);
            videoElement.hls = hls; // Store reference for cleanup
        } else if (videoElement.canPlayType('application/vnd.apple.mpegurl')) {
            // For browsers that support HLS natively (Safari)
            console.log("Using native HLS support");
            videoElement.src = URL;
            videoElement.addEventListener('loadedmetadata', function() {
                loadingIndicator.style.display = 'none';
                adjustAspectRatio(containerDiv, videoElement);
                videoElement.play().catch(function(error) {
                    console.warn("Autoplay prevented:", error);
                    showPlayButton(containerDiv, videoElement);
                });
            });
            videoElement.addEventListener('error', function(e) {
                console.error("Video error:", e);
                showErrorMessage(containerDiv, loadingIndicator, "Error loading video. Please try again later.");
            });
        } else {
            showErrorMessage(containerDiv, loadingIndicator, "Your browser doesn't support HLS video playback.");
        }
    } else {
        // Handle regular video formats
        videoElement.src = URL;
        videoElement.addEventListener('loadeddata', function() {
            loadingIndicator.style.display = 'none';
        });
        videoElement.addEventListener('loadedmetadata', function() {
            adjustAspectRatio(containerDiv, videoElement);
        });
        videoElement.addEventListener('error', function(e) {
            console.error("Video error:", e);
            showErrorMessage(containerDiv, loadingIndicator, "Error loading video. Please try again later.");
        });
        
        videoElement.play().catch(function(error) {
            console.warn("Autoplay prevented:", error);
            showPlayButton(containerDiv, videoElement);
        });
    }
}

// Helper function to show play button when autoplay is blocked
function showPlayButton(container, videoElement) {
    var loadingIndicator = document.getElementById('videoLoadingIndicator');
    if (loadingIndicator) {
        loadingIndicator.style.display = 'none';
    }
    
    var playButton = document.createElement('button');
    playButton.classList.add('video-play-button');
    playButton.innerHTML = '▶';
    playButton.style.position = 'absolute';
    playButton.style.top = '50%';
    playButton.style.left = '50%';
    playButton.style.transform = 'translate(-50%, -50%)';
    playButton.style.fontSize = '3rem';
    playButton.style.padding = '1rem 2rem';
    playButton.style.background = 'var(--card-bg, rgba(35,39,43,0.8))';
    playButton.style.color = 'var(--text-color, white)';
    playButton.style.border = 'none';
    playButton.style.borderRadius = '5px';
    playButton.style.cursor = 'pointer';
    playButton.style.zIndex = '20';
    
    playButton.onclick = function() {
        videoElement.muted = true; // Ensure muted for autoplay
        videoElement.play().then(function() {
            playButton.remove();
        }).catch(function(error) {
            console.error("Play failed after button click:", error);
        });
    };
    
    container.appendChild(playButton);
}

// Helper function to show error message
function showErrorMessage(container, loadingIndicator, message) {
    if (loadingIndicator) {
        loadingIndicator.textContent = message;
        // Use a red color that works in both light and dark themes
        loadingIndicator.style.color = '#ff6b6b';
        // Add a warning icon for better visibility
        loadingIndicator.innerHTML = '⚠️ ' + message;
    }
}

// Helper function to adjust container aspect ratio based on video dimensions
function adjustAspectRatio(container, videoElement) {
    // Only adjust if video metadata has loaded and has valid dimensions
    if (videoElement.videoWidth && videoElement.videoHeight) {
        var videoAspectRatio = videoElement.videoWidth / videoElement.videoHeight;
        console.log("Video metadata loaded. Actual dimensions:", videoElement.videoWidth, "x", videoElement.videoHeight);
        console.log("Actual aspect ratio:", videoAspectRatio);
        
        // Remove any previously set aspect ratio classes
        container.classList.remove('aspect-4-3', 'aspect-16-9');
        
        // Apply appropriate aspect ratio class based on actual video dimensions
        if (Math.abs(videoAspectRatio - 1.33) < 0.1) { // Close to 4:3 (1.33)
            container.classList.add('aspect-4-3');
            console.log("Applied 4:3 aspect ratio from metadata");
        } else if (Math.abs(videoAspectRatio - 1.78) < 0.1) { // Close to 16:9 (1.78)
            container.classList.add('aspect-16-9');
            console.log("Applied 16:9 aspect ratio from metadata");
        } else {
            // Set precise aspect ratio using style
            container.style.aspectRatio = `${videoElement.videoWidth}/${videoElement.videoHeight}`;
            console.log("Applied custom aspect ratio from metadata:", `${videoElement.videoWidth}/${videoElement.videoHeight}`);
        }
    }
}

function HideModal(event) {
    var modalTitle = videoModal.querySelector('.modal-title');
    modalTitle.textContent = "";

    var videoElement = document.getElementById('modalVideo');
    if (videoElement) {
        try {
            // Stop video playback
            videoElement.pause();
            
            // Clean up HLS.js if it was used
            if (videoElement.hls) {
                videoElement.hls.destroy();
                delete videoElement.hls;
            }
            
            // Remove all child elements
            while(videoElement.firstChild) {
                videoElement.removeChild(videoElement.firstChild);
            }
            
            // Reset source and load to ensure resources are fully released
            videoElement.removeAttribute('src');
            videoElement.load();
            
            // Remove event listeners
            videoElement.onloadeddata = null;
            videoElement.onerror = null;
        } catch (e) {
            console.error("Error cleaning up video element:", e);
        }
    }
    
    // Remove any play button overlay or loading indicator if they exist
    var playButton = document.querySelector('.video-play-button');
    if (playButton) {
        playButton.remove();
    }
    
    var loadingIndicator = document.getElementById('videoLoadingIndicator');
    if (loadingIndicator) {
        loadingIndicator.remove();
    }
    
    // Remove the container div
    var videoContainer = document.querySelector('.video-container');
    if (videoContainer) {
        videoContainer.remove();
    }
}