// ============================================================
// Hardware Service Center - Notification Toaster
// Reads TempData from a hidden #toaster-data element and
// displays a bottom-right toast notification.
// ============================================================

(function () {
  'use strict';

  // Inject toaster styles once
  var style = document.createElement('style');
  style.textContent = `
    .hsc-toaster {
      position: fixed;
      bottom: 24px;
      right: 24px;
      z-index: 9999;
      min-width: 300px;
      max-width: 420px;
      background: #fff;
      border-radius: 10px;
      box-shadow: 0 8px 30px rgba(0,0,0,0.12), 0 2px 8px rgba(0,0,0,0.06);
      border-left: 4px solid;
      padding: 1rem 1.25rem;
      display: flex;
      align-items: center;
      gap: 0.75rem;
      font-family: 'Inter', system-ui, -apple-system, sans-serif;
      font-size: 0.9rem;
      transform: translateX(calc(100% + 30px));
      opacity: 0;
      transition: transform 0.35s cubic-bezier(0.4, 0, 0.2, 1),
                  opacity 0.35s cubic-bezier(0.4, 0, 0.2, 1);
      pointer-events: none;
    }
    .hsc-toaster.show {
      transform: translateX(0);
      opacity: 1;
      pointer-events: auto;
    }
    .hsc-toaster.hide {
      transform: translateX(calc(100% + 30px));
      opacity: 0;
      pointer-events: none;
    }
    .hsc-toaster .toaster-icon {
      width: 36px;
      height: 36px;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      font-size: 1.15rem;
    }
    .hsc-toaster .toaster-body {
      flex: 1;
      min-width: 0;
    }
    .hsc-toaster .toaster-title {
      font-weight: 600;
      font-size: 0.85rem;
      line-height: 1.3;
      margin-bottom: 0;
    }
    .hsc-toaster .toaster-close {
      background: none;
      border: none;
      color: #94A3B8;
      font-size: 1.1rem;
      cursor: pointer;
      padding: 0;
      line-height: 1;
      transition: color 0.2s;
      flex-shrink: 0;
    }
    .hsc-toaster .toaster-close:hover {
      color: #475569;
    }

    /* Success variant */
    .hsc-toaster.toaster-success {
      border-left-color: #10B981;
      background: #ECFDF5;
    }
    .hsc-toaster.toaster-success .toaster-icon {
      background: rgba(16, 185, 129, 0.1);
      color: #10B981;
    }
    .hsc-toaster.toaster-success .toaster-title {
      color: #065F46;
    }

    /* Error variant */
    .hsc-toaster.toaster-error {
      border-left-color: #EF4444;
      background: #FEF2F2;
    }
    .hsc-toaster.toaster-error .toaster-icon {
      background: rgba(239, 68, 68, 0.1);
      color: #EF4444;
    }
    .hsc-toaster.toaster-error .toaster-title {
      color: #991B1B;
    }

    /* Warning variant */
    .hsc-toaster.toaster-warning {
      border-left-color: #F59E0B;
    }
    .hsc-toaster.toaster-warning .toaster-icon {
      background: rgba(245, 158, 11, 0.1);
      color: #F59E0B;
    }
    .hsc-toaster.toaster-warning .toaster-title {
      color: #B45309;
    }

    /* Info variant */
    .hsc-toaster.toaster-info {
      border-left-color: #06B6D4;
    }
    .hsc-toaster.toaster-info .toaster-icon {
      background: rgba(6, 182, 212, 0.1);
      color: #06B6D4;
    }
    .hsc-toaster.toaster-info .toaster-title {
      color: #155E75;
    }

    @media (max-width: 576px) {
      .hsc-toaster {
        right: 12px;
        bottom: 12px;
        left: 12px;
        min-width: auto;
        max-width: none;
      }
    }
  `;
  document.head.appendChild(style);

  var iconMap = {
    success: 'bi-check-circle-fill',
    error: 'bi-exclamation-triangle-fill',
    warning: 'bi-exclamation-circle-fill',
    info: 'bi-info-circle-fill'
  };

  var labelMap = {
    success: 'Success',
    error: 'Error',
    warning: 'Warning',
    info: 'Info'
  };

  function showToaster(message, type) {
    type = type || 'success';
    var iconName = iconMap[type] || iconMap.success;
    var label = labelMap[type] || labelMap.success;

    var toast = document.createElement('div');
    toast.className = 'hsc-toaster toaster-' + type;
    toast.innerHTML =
      '<div class="toaster-icon"><i class="bi ' + iconName + '"></i></div>' +
      '<div class="toaster-body"><p class="toaster-title">' + message + '</p></div>' +
      '<button class="toaster-close" aria-label="Close">&times;</button>';

    document.body.appendChild(toast);

    // Close button
    toast.querySelector('.toaster-close').addEventListener('click', function () {
      dismiss(toast);
    });

    // Slide in
    requestAnimationFrame(function () {
      requestAnimationFrame(function () {
        toast.classList.add('show');
      });
    });

    // Auto-dismiss after 4s
    setTimeout(function () {
      dismiss(toast);
    }, 4000);
  }

  function dismiss(toast) {
    if (!toast || !toast.parentNode) return;
    toast.classList.remove('show');
    toast.classList.add('hide');
    setTimeout(function () {
      if (toast.parentNode) toast.parentNode.removeChild(toast);
    }, 400);
  }

  // On DOM ready, read the hidden element and fire
  function init() {
    var el = document.getElementById('toaster-data');
    if (!el) return;

    var message = el.getAttribute('data-toaster-message');
    var type = el.getAttribute('data-toaster-type') || 'success';

    if (message) {
      showToaster(message, type);
    }
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }

  // Expose globally for manual use
  window.HscToaster = { show: showToaster };

})();
