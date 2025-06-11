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

  // Helper to render alerts in Bootstrap style
  function renderAlerts(containerId, alerts, type, icon, badgeClass) {
    const container = document.getElementById(containerId);
    if (!container) return;
    if (!alerts || alerts.length === 0) {
      container.innerHTML = '';
      return;
    }
    let chevronId = `${type}Chevron`;
    let collapseId = `${type}Collapse`;
    let alertClass = {
      incidents: 'alert-danger',
      construction: 'alert-warning',
      weather: 'alert-info'
    }[type] || 'alert-secondary';
    
    // Default state based on alert type
    const startExpanded = type === 'incidents';
    const showClass = startExpanded ? 'show' : '';
    const chevronClass = startExpanded ? 'bi-chevron-up' : 'bi-chevron-down';
    const ariaExpanded = startExpanded ? 'true' : 'false';
    
    // Generate HTML for the alert container with collapsible content
    let html = `
      <div class="alert ${alertClass} text-center mb-4" role="alert">
        <h6 class="fw-bold p-0" data-bs-toggle="collapse" data-bs-target="#${collapseId}" aria-expanded="${ariaExpanded}" aria-controls="${collapseId}">
          <span class="bi ${icon}"></span> ${type.charAt(0).toUpperCase() + type.slice(1)} Alerts <span id="${chevronId}" class="bi ${chevronClass}"></span>
        </h6>
        <div class="collapse ${showClass} mt-2" id="${collapseId}">
          <ul class="list-unstyled mb-0">
            ${alerts.map(a => `<li class="mb-2">${a.description || a.Description}</li>`).join('')}
          </ul>
          <div class="text-end small text-muted mt-2">
            <span>Last updated: ${new Date().toLocaleTimeString()}</span>
          </div>
        </div>
      </div>
    `;
    container.innerHTML = html;
    setupCollapseChevron(collapseId, chevronId);
  }

  async function fetchAndRenderAlerts() {
    // Incidents
    try {
      const incidentsRes = await fetch('/api/alerts/incidents');
      const incidents = await incidentsRes.json();
      renderAlerts('incidents-alerts-container', incidents, 'incidents', 'bi-exclamation-triangle-fill', 'bg-danger');
    } catch (error) {
      console.error('Error fetching incidents alerts:', error);
    }
    // Construction
    try {
      const constructionRes = await fetch('/api/alerts/construction');
      const construction = await constructionRes.json();
      renderAlerts('construction-alerts-container', construction, 'construction', 'bi-cone-striped', 'bg-warning');
    } catch (error) {
      console.error('Error fetching construction alerts:', error);
    }
    // Weather
    try {
      const weatherRes = await fetch('/api/alerts/weather');
      const weather = await weatherRes.json();
      renderAlerts('weather-alerts-container', weather, 'weather', 'bi-cloud-lightning-rain', 'bg-info');
    } catch (error) {
      console.error('Error fetching weather alerts:', error);
    }
  }

  fetchAndRenderAlerts();
  // Optionally, refresh alerts every 60 seconds
  setInterval(fetchAndRenderAlerts, 60000);
});
