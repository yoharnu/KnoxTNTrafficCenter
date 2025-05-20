function ShowModal(event) {

    var button = event.relatedTarget;

    var URL = button.getAttribute('data-bs-video-url');
    var width = button.getAttribute('data-bs-video-width');
    var height = button.getAttribute('data-bs-video-height');
    var title = button.getAttribute('data-bs-title');

    var modalTitle = videoModal.querySelector('.modal-title');

    modalTitle.textContent = title;

    var modalBody = videoModal.getElementsByClassName('modal-body')[0];
    var videoJS = document.createElement('video-js');
    videoJS.id = 'modalVideoJS';
    videoJS.classList.add('video-js');

    while (modalBody.children.length > 0) {
        modalBody.removeChild(modalBody.children[0]);
    }

    modalBody.appendChild(videoJS);

    var myPlayer = videojs('modalVideoJS');
    myPlayer.muted(true);
    myPlayer.autoplay(true);
    myPlayer.controls(true);
    myPlayer.fluid(true);
    myPlayer.src(URL);
    myPlayer.ready(function () {
        myPlayer.play();
    });
}

function HideModal(event) {
    var modalTitle = videoModal.querySelector('.modal-title');
    modalTitle.textContent = "";

    var myPlayer = videojs('modalVideoJS');
    if (myPlayer)
        myPlayer.dispose();
}