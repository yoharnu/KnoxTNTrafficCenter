document.addEventListener('DOMContentLoaded', function () {
  // Handle collapse toggles for weather alerts
  const weatherCollapse = document.getElementById('weatherCollapse');
  const weatherChevron = document.getElementById('weatherChevron');
  
  if (weatherCollapse) {
    weatherCollapse.addEventListener('hidden.bs.collapse', function () {
      if (weatherChevron) {
        weatherChevron.classList.remove('bi-chevron-up');
        weatherChevron.classList.add('bi-chevron-down');
      }
    });
    
    weatherCollapse.addEventListener('shown.bs.collapse', function () {
      if (weatherChevron) {
        weatherChevron.classList.remove('bi-chevron-down');
        weatherChevron.classList.add('bi-chevron-up');
      }
    });
    
    // Add pulse effect for severe weather alerts
    const weatherAlerts = document.querySelectorAll('.weather-alert');
    weatherAlerts.forEach(alert => {
      const badge = alert.querySelector('.badge');
      if (badge && badge.classList.contains('bg-danger')) {
        // Add pulsing effect for severe alerts
        alert.style.animation = 'pulse 2s infinite';
        const style = document.createElement('style');
        style.textContent = `
          @keyframes pulse {
            0% { box-shadow: 0 0 0 0 rgba(220, 53, 69, 0.4); }
            70% { box-shadow: 0 0 0 10px rgba(220, 53, 69, 0); }
            100% { box-shadow: 0 0 0 0 rgba(220, 53, 69, 0); }
          }
        `;
        document.head.appendChild(style);
      }
    });
  }
  
  // Handle existing collapse toggles
  const incidentsCollapse = document.getElementById('incidentsCollapse');
  const incidentsChevron = document.getElementById('incidentsChevron');
  
  if (incidentsCollapse) {
    incidentsCollapse.addEventListener('hidden.bs.collapse', function () {
      if (incidentsChevron) {
        incidentsChevron.classList.remove('bi-chevron-up');
        incidentsChevron.classList.add('bi-chevron-down');
      }
    });
    
    incidentsCollapse.addEventListener('shown.bs.collapse', function () {
      if (incidentsChevron) {
        incidentsChevron.classList.remove('bi-chevron-down');
        incidentsChevron.classList.add('bi-chevron-up');
      }
    });
  }
  
  const constructionCollapse = document.getElementById('constructionCollapse');
  const constructionChevron = document.getElementById('constructionChevron');
  
  if (constructionCollapse) {
    constructionCollapse.addEventListener('hidden.bs.collapse', function () {
      if (constructionChevron) {
        constructionChevron.classList.remove('bi-chevron-up');
        constructionChevron.classList.add('bi-chevron-down');
      }
    });
    
    constructionCollapse.addEventListener('shown.bs.collapse', function () {
      if (constructionChevron) {
        constructionChevron.classList.remove('bi-chevron-down');
        constructionChevron.classList.add('bi-chevron-up');
      }
    });
  }
});
