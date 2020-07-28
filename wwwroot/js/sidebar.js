$('.sidenav').find('a[href="' + location.pathname + '"]')
      .closest('.nav-link').addClass('side-active');