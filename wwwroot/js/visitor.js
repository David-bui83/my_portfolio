const device = navigator.userAgent;

$(document).ready(function(){
  $.get("https://api.ipify.org?format=json",function(data){
    getVisitorInfo(data.ip, device);
  });
  
  function getVisitorInfo(ip,device){
    $.ajax({
      url: 'visitor/tracker',
      method: "POST",
      data: {
        VisitorDevice: device,
        VisitorIp: ip
      }
    })
  }
})