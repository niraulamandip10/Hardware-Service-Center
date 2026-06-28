// ============================================================
// Hardware Service Center - Global Scripts
// ============================================================

(function () {
  'use strict';

  // Auto-highlight active nav link based on current URL
  var currentPath = window.location.pathname.toLowerCase();
  document.querySelectorAll('.navbar-hsc .nav-link').forEach(function (link) {
    var href = link.getAttribute('href');
    if (href && href.toLowerCase() !== '#' && currentPath.indexOf(href.toLowerCase()) !== -1) {
      link.classList.add('active');
    }
  });

})();
