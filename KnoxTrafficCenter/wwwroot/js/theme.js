// Theme toggle logic
(function() {
  // Helper function to update video player theme
  function updateVideoPlayerTheme(theme) {
    const videoPlayer = document.getElementById('modalVideo');
    if (videoPlayer) {
      videoPlayer.dataset.colorTheme = theme;
      videoPlayer.style.colorScheme = theme;
    }
  }
  
  function setTheme(mode) {
    if (mode === 'dark') {
      document.body.classList.add('dark-mode');
      localStorage.setItem('theme', 'dark');
      updateVideoPlayerTheme('dark');
    } else {
      document.body.classList.remove('dark-mode');
      localStorage.setItem('theme', 'light');
      updateVideoPlayerTheme('light');
    }
  }

  // On load, set theme from localStorage or default to light
  const saved = localStorage.getItem('theme');
  setTheme(saved === 'dark' ? 'dark' : 'light');

  // Expose toggle function globally
  window.toggleTheme = function() {
    var isDark = document.body.classList.contains('dark-mode');
    setTheme(isDark ? 'light' : 'dark');
  };
})();
