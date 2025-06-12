// Camera image refresh logic
document.addEventListener('DOMContentLoaded', function () {
  const REFRESH_INTERVAL = 60000; // 60 seconds in milliseconds
  const failedImages = new Map(); // Track images that failed to load
  
  // Function to add timestamp to URLs to force cache invalidation
  function refreshCameraImages() {
    // Target all camera card images
    const cameraImages = document.querySelectorAll('.card-img-bottom');
    const timestamp = new Date().getTime();
    let refreshedCount = 0;
    let failedCount = 0;
    
    // Setup event listeners for error handling
    function setupImageErrorHandling(img) {
      // Keep track of error counts
      const originalSrc = img.src.split('?')[0];
      
      // Clear any previous error handlers
      img.onerror = null;
      
      // Set up new error handler
      img.onerror = () => {
        // Track failures by image URL
        const currentFailCount = failedImages.get(originalSrc) || 0;
        failedImages.set(originalSrc, currentFailCount + 1);
        
        console.warn(`Failed to load image: ${originalSrc} (attempt ${currentFailCount + 1})`);
        
        // If we've had less than 3 failures for this image, try again with a different cache buster
        if (currentFailCount < 3) {
          // Use a different cache buster strategy after failures
          const newTimestamp = Date.now() + Math.floor(Math.random() * 1000);
          setTimeout(() => {
            console.log(`Retrying failed image: ${originalSrc}`);
            img.src = `${originalSrc}?retry=${newTimestamp}`;
          }, 3000); // Wait 3 seconds before retry
        }
      };
      
      // Set up load handler to reset error count on success
      img.onload = () => {
        if (failedImages.has(originalSrc)) {
          failedImages.delete(originalSrc);
        }
      };
    }
    
    cameraImages.forEach(img => {
      setupImageErrorHandling(img);
      
      // Only refresh images that are currently visible in the viewport
      const isInViewport = isElementInViewport(img);
      
      if (img.complete && img.naturalWidth > 0 && isInViewport) {
        const originalUrl = img.src.split('?')[0]; // Get base URL without existing query params
        img.src = `${originalUrl}?t=${timestamp}`; // Append timestamp as query param
        refreshedCount++;
        
        // Apply subtle animation to visible refreshed images
        img.style.transition = 'opacity 0.2s';
        img.style.opacity = '0.8';
        setTimeout(() => {
          img.style.opacity = '1';
        }, 200);
      } else if (!img.complete || img.naturalWidth === 0) {
        failedCount++;
      }
    });
    
    // Log refresh status
    if (refreshedCount > 0) {
      console.log(`Refreshed ${refreshedCount} camera images at ${new Date().toLocaleTimeString()}`);
    }
    
    return { refreshedCount, failedCount };
  }
  
  // Helper function to determine if element is visible in viewport
  function isElementInViewport(el) {
    const rect = el.getBoundingClientRect();
    return (
      rect.top >= -100 && // Include partially visible elements above
      rect.left >= 0 &&
      rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) + 100 && // Include partially visible elements below
      rect.right <= (window.innerWidth || document.documentElement.clientWidth)
    );
  }

  // Create a status indicator to show when refreshes happen
  function createRefreshIndicator() {
    const indicator = document.createElement('div');
    indicator.id = 'refresh-indicator';
    indicator.style.position = 'fixed';
    indicator.style.bottom = '70px'; // Position above footer
    indicator.style.right = '10px';
    indicator.style.padding = '8px 12px';
    indicator.style.backgroundColor = 'var(--card-bg, rgba(35, 39, 43, 0.8))';
    indicator.style.color = 'var(--text-color, white)';
    indicator.style.borderRadius = '4px';
    indicator.style.fontSize = '12px';
    indicator.style.opacity = '0';
    indicator.style.transition = 'opacity 0.3s';
    indicator.style.zIndex = '1000';
    indicator.style.boxShadow = '0 2px 5px rgba(0, 0, 0, 0.2)';
    document.body.appendChild(indicator);
    
    return indicator;
  }
  
  const refreshIndicator = createRefreshIndicator();
  
  // Function to update the status indicator
  function updateRefreshStatus(isInitial = false, refreshStats = null) {
    const now = new Date();
    
    if (isInitial) {
      refreshIndicator.innerHTML = `<span class="bi bi-arrow-repeat"></span> Camera images will refresh every minute`;
    } else if (refreshStats) {
      refreshIndicator.innerHTML = `<span class="bi bi-arrow-clockwise"></span> ${refreshStats.refreshedCount} images refreshed at ${now.toLocaleTimeString()}`;
    } else {
      refreshIndicator.innerHTML = `<span class="bi bi-arrow-clockwise"></span> Images refreshed at ${now.toLocaleTimeString()}`;
    }
    
    refreshIndicator.style.opacity = '1';
    
    setTimeout(() => {
      refreshIndicator.style.opacity = '0';
    }, 3000);
  }

  // Start the refresh cycle - wait for initial page load before first refresh
  setTimeout(() => {
    // Show initial message
    updateRefreshStatus(true);
    
    // Set up recurring refreshes
    setInterval(() => {
      const stats = refreshCameraImages();
      updateRefreshStatus(false, stats);
    }, REFRESH_INTERVAL);
    
    console.log(`Camera refresh timer started - images will refresh every ${REFRESH_INTERVAL/1000} seconds`);
    
    // Add scroll event listener to refresh visible cameras when scrolling
    let scrollTimeout;
    window.addEventListener('scroll', () => {
      clearTimeout(scrollTimeout);
      scrollTimeout = setTimeout(() => {
        // Only refresh images if it's been at least 10 seconds since the last refresh
        if (Date.now() - lastRefreshTime > 10000) {
          const stats = refreshCameraImages();
          if (stats.refreshedCount > 0) {
            updateRefreshStatus(false, stats);
            lastRefreshTime = Date.now();
          }
        }
      }, 300);
    });
    
  }, 2000); // Short initial delay to ensure all images are loaded first
  
  // Track when we last refreshed to prevent excessive refreshes during scrolling
  let lastRefreshTime = Date.now();
});
