class ContactForm {
  constructor(){
    this.name = '',
    this.subject = '',
    this.email = '',
    this.phone = '',
    this.message = ''
  }

  setName(name){
    if(name === ''){
      document.querySelector('span.name').innerHTML = 'Name is required';
      return false;
    }
    document.querySelector('span.name').innerHTML = '';
    this.name = name;
    return true;
  }

  getName(){return this.name;}

  setSubject(subject){
    if(subject === ''){
      document.querySelector('span.subject').innerHTML = "Subject is required";
      return false;
    }
    document.querySelector('span.subject').innerHTML = '';
    this.subject = subject;
    return true;
  }

  getSubject(){return this.subject}

  setEmail(email){
    const regEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if(email === ''){
      document.querySelector('span.email').innerHTML = "Email is required";
      return false;
    }else if(regEmail.test(email) === false){
      document.querySelector('span.email').innerHTML = "Not a valid email address";
      return false;
    }else{
      document.querySelector('span.email').innerHTML = '';
      this.email = email;
      return true;
    }
  }

  getEmail(){return this.email};

  setPhone(phone) {
    const regPhone = /\(?\d{3}\)?-?.? *\d{3}-?.? *-?\d{4}|\d{1}?-?.? *\(?\d{3}\)?-?.? *\d{3}-?.? *-?\d{4}/;
    if(phone === ''){
      document.querySelector('span.phone').innerHTML = "Phone is required";
      return false;
    }else if(regPhone.test(phone) === false){
      document.querySelector('span.phone').innerHTML = "Not a valid phone number";
      return false;
    }else{
      document.querySelector('span.phone').innerHTML = '';
      this.phone = phone;
      return true;
    }
  }

  getPhone(){return this.phone};

  setMessage(message){
    if(message === ''){
      document.querySelector('span.message').innerHTML = "Message is required";
      return false;
    }else if(message.length < 10){
      document.querySelector('span.message').innerHTML = "Message must be atleast 10 characters";
      return false;
    }else{
      document.querySelector('span.message').innerHTML = '';
      this.message = message;
      return true;
    }
  }

  getMessage(){return this.message};
}

$(document).ready(function(){
  $('#contact-btn').click(function(e){
    const contact = $('#contact-form').serializeArray();
    e.preventDefault();
    const message = new ContactForm();
    if(message.setName(contact[0].value) === true && message.setSubject(contact[1].value) === true && message.setEmail(contact[2].value) === true && message.setPhone(contact[3].value) === true && message.setMessage(contact[4].value) === true){
      $.ajax({
        url: '/mail',
        method: "POST",
        data: {
          Name: message.getName(),
          Subject: message.getSubject(),
          PhoneNumber: message.getPhone(),
          Message: message.getMessage(),
          ToEmail: '',
          FromEmail: message.getEmail()
        },
        success: function(result){
          document.querySelector('input#Name').value = '';
          document.querySelector('input#Subject').value = '';
          document.querySelector('input#FromEmail').value = '';
          document.querySelector('input#PhoneNumber').value = '';
          document.querySelector('textarea#Message').value = '';
          
          const nameList = result.name.split(' ');
          const name = nameList[0][0].toUpperCase()+nameList[0].slice(1);
          $('.sender-name').text(name);
          $('#exampleModalCenter').modal('toggle');
        }
      })
    }
  })
})
