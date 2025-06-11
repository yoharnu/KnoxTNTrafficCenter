document.addEventListener('DOMContentLoaded', function () {
  // Handle collapse toggles for weather alerts
  const weatherCollapse = document.getElementById('weatherCollapse');
  const weatherChevron = document.getElementById('weatherChevron');
  
  // Helper to DRY up collapse/chevron toggling
  function setupCollapseChevron(collapseId, chevronId) {
    const collapse = document.getElementById(collapseId);
    const chevron = document.getElementById(chevronId);
    if (!collapse) return;
    collapse.addEventListener('hidden.bs.collapse', function () {
      if (chevron) {
        chevron.classList.remove('bi-chevron-up');
        chevron.classList.add('bi-chevron-down');
      }
    });
    collapse.addEventListener('shown.bs.collapse', function () {
      if (chevron) {
        chevron.classList.remove('bi-chevron-down');
        chevron.classList.add('bi-chevron-up');
      }
    });
  }

  // Use helper for weather, incidents, and construction
  setupCollapseChevron('weatherCollapse', 'weatherChevron');
  setupCollapseChevron('incidentsCollapse', 'incidentsChevron');
  setupCollapseChevron('constructionCollapse', 'constructionChevron');

  // Add pulse effect for severe weather alerts
  const weatherAlerts = document.querySelectorAll('.weather-alert');
  
  // Add keyframes definition once
  const style = document.createElement('style');
  style.textContent = `
    @keyframes pulse {
      0% { box-shadow: 0 0 0 0 rgba(220, 53, 69, 0.4); }
      70% { box-shadow: 0 0 0 10px rgba(220, 53, 69, 0); }
      100% { box-shadow: 0 0 0 0 rgba(220, 53, 69, 0); }
    }
  `;
  document.head.appendChild(style);
  
  weatherAlerts.forEach(alert => {
    const badge = alert.querySelector('.badge');
    if (badge && badge.classList.contains('bg-danger')) {
      // Add pulsing effect for severe alerts
      alert.style.animation = 'pulse 2s infinite';
    }
  });
});
