$(document).ready(function(){
  var clientId = '1cc3be4c1cdf69d0c6e23715956bd466b5dc7caa1b3e6abf94e9b0c39fe9d1c6';
  var query = "coding";
  var url = "https://api.unsplash.com/search/photos/?client_id="+clientId+"&query="+query;

  fetch(url)
  .then(function(data){
    return data.json();
  })
  .then(function(data){
    var num = Math.floor(Math.random()*10);

    document.getElementById("title-div").style.background = "url("+data.results[num].urls.raw+") no-repeat center center/cover fixed";
  })
})