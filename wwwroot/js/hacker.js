$(document).ready(function(){
  var device = navigator.userAgent;
  $.get("https://cors-anywhere.herokuapp.com/https://api.ipify.org?format=json",function(data){
    console.log(data.ip)
    getHackerInfo(data.ip, device);
  });
  
  function getHackerInfo(ip,device){
    $.ajax({
      url: '/security/danger',
      method: "POST",
      data: {
        HackerDevice: device,
        HackerIp: ip
      },
      success: function(result){
        document.querySelector(".device-name").innerHTML = result.hackerDevice;
        document.querySelector(".ip-address").innerHTML = result.hackerIp;
      }
    })
  }
})