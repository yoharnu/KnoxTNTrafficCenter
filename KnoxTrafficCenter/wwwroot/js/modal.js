function ShowModal(event) {

    var button = event.relatedTarget;

    var URL = button.getAttribute('data-bs-video-url');
    var width = button.getAttribute('data-bs-video-width');
    var height = button.getAttribute('data-bs-video-height');
    var title = button.getAttribute('data-bs-title');

    var modalTitle = videoModal.querySelector('.modal-title');

    modalTitle.textContent = title;

    var modalBody = videoModal.getElementsByClassName('modal-body')[0];
    
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
    videoElement.playsInline = true; // Required for autoplay in some browsers
    videoElement.preload = 'auto';
    
    // Add source elements instead of directly setting src attribute
    var sourceElement = document.createElement('source');
    sourceElement.src = URL;
    
    // Determine video type from URL
    if (URL.toLowerCase().includes('.mp4')) {
        sourceElement.type = 'video/mp4';
    } else if (URL.toLowerCase().includes('.webm')) {
        sourceElement.type = 'video/webm';
    } else if (URL.toLowerCase().includes('.m3u8')) {
        sourceElement.type = 'application/x-mpegURL';
    }
    
    videoElement.appendChild(sourceElement);
    modalBody.appendChild(videoElement);
    
    // Play using promise pattern to handle autoplay restrictions
    var playPromise = videoElement.play();
    
    if (playPromise !== undefined) {
        playPromise.then(_ => {
            // Playback started successfully
            console.log("Video playback started");
        }).catch(error => {
            console.log("Autoplay prevented by browser: " + error);
            // Create play button overlay for user interaction
            var playButton = document.createElement('button');
            playButton.classList.add('video-play-button');
            playButton.innerHTML = '▶';
            playButton.style.position = 'absolute';
            playButton.style.top = '50%';
            playButton.style.left = '50%';
            playButton.style.transform = 'translate(-50%, -50%)';
            playButton.style.fontSize = '3rem';
            playButton.style.padding = '1rem 2rem';
            playButton.style.background = 'rgba(0,0,0,0.5)';
            playButton.style.color = 'white';
            playButton.style.border = 'none';
            playButton.style.borderRadius = '5px';
            playButton.style.cursor = 'pointer';
            
            playButton.onclick = function() {
                videoElement.play();
                this.remove();
            };
            
            modalBody.appendChild(playButton);
        });
    }
}

function HideModal(event) {
    var modalTitle = videoModal.querySelector('.modal-title');
    modalTitle.textContent = "";

    var videoElement = document.getElementById('modalVideo');
    if (videoElement) {
        try {
            videoElement.pause();
            
            // Remove all sources
            while(videoElement.firstChild) {
                videoElement.removeChild(videoElement.firstChild);
            }
            
            // Reset source and load to ensure resources are fully released
            videoElement.removeAttribute('src');
            videoElement.load();
        } catch (e) {
            console.error("Error cleaning up video element:", e);
        }
    }
    
    // Remove any play button overlay if it exists
    var playButton = document.querySelector('.video-play-button');
    if (playButton) {
        playButton.remove();
    }
}