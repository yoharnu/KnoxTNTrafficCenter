function ShowModal(event) {

    var button = event.relatedTarget;

    var URL = button.getAttribute('data-bs-video-url');
    var width = button.getAttribute('data-bs-video-width');
    var height = button.getAttribute('data-bs-video-height');
    var title = button.getAttribute('data-bs-title');

    var modalTitle = videoModal.querySelector('.modal-title');

    modalTitle.textContent = title;

    var modalBody = videoModal.getElementsByClassName('modal-body')[0];
    var videoElement = document.createElement('video');
    videoElement.id = 'modalVideo';
    videoElement.classList.add('video-player');
    videoElement.style.width = '100%';
    videoElement.controls = true;
    videoElement.autoplay = true;
    videoElement.muted = true;
    videoElement.src = URL;

    while (modalBody.children.length > 0) {
        modalBody.removeChild(modalBody.children[0]);
    }

    modalBody.appendChild(videoElement);
    videoElement.play();
}

function HideModal(event) {
    var modalTitle = videoModal.querySelector('.modal-title');
    modalTitle.textContent = "";

    var videoElement = document.getElementById('modalVideo');
    if (videoElement) {
        videoElement.pause();
        videoElement.src = '';
    }
}