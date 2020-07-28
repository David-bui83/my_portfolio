// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function() {
  
  $('ul.nav.navbar-nav').find('a[href="' + location.pathname + '"]')
    .closest('li').addClass('active');
 
  $('.btn-skills').click(function(){
    $('.sm-skills').toggle(2000);
  });

  $('.btn-icon').click(function(){
    $('.icons-container').toggle(2000);
  });

  $('.btn-education').click(function(){
    $('.table-education').toggle(2000);
  });

  $('.btn-goals').click(function(){
    $('.goals').toggle(2000);
  });

  $('.btn-interests').click(function(){
    $('.interests').toggle(2000);
  });

  $('.main-title-container').hide().fadeIn(3000,function(){
    $('.main-title').fadeTo(2000,1,function(){
      $('.resume-link').fadeTo(3000, 1);
    });
  });

  $('.contact-me-button').click(function(){
    $('html, body').animate({scrollTop: $('#contact-me').offset().top}, 2000);
  });

  setInterval(function(){
    removeClassML();
    removeSecondClassML();
    hideHamburger();
  },100);
  
  removeClassML();
  
  viewDisplayNav();

  headerBackgroundOn();

  getYearForFooter();
});

function removeClassML(){
  const w = $(document).width();
  if(w < 576){
    $("#login").removeClass("ml-auto");
  }else{
    $("#login").addClass("ml-auto");
  }
}

function removeSecondClassML(){
  const w = $(document).width();
  if(w < 576){
    $("#login-2").removeClass("ml-auto");
  }else{
    $("#login-2").addClass("ml-auto");
  }
}

function viewDisplayNav(){
  $(window).scroll(function() {
    var scrollTop = $(this).scrollTop();
    if(scrollTop > 150){
      $('.view-nav').fadeIn(1000);
    }else{
      $('.view-nav').fadeOut(1000);
    }
  });
}
 
function headerBackgroundOn(){
  $(document).ready(function(){
    $(window).bind('scroll', function() {
    var distance = $(window).height() - 70;
      if ($(window).scrollTop() >= distance) {
          $('.nav-tran-2').fadeIn(500)
      }else{
        $('.nav-tran-2').fadeOut(500);
      }
     });
 });
}

function hideHamburger(){
  var w = $(window).width()
  if(w>=561){
    $(".main-btn").hide();
  }else{
    $(".main-btn").show();
  }
}

function getYearForFooter(){
  var y = new Date();
  document.getElementById('year').innerHTML = y.getFullYear();
}
