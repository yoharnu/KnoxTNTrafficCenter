// Theme toggle logic
(function() {
  function setTheme(mode) {
    if (mode === 'dark') {
      document.body.classList.add('dark-mode');
      localStorage.setItem('theme', 'dark');
    } else {
      document.body.classList.remove('dark-mode');
      localStorage.setItem('theme', 'light');
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
