# My Portfolio 
This is my portfolio site where I display all the porjects I have worked on. The project is a C#/ASP.Net Core server side rendering site with MySQL as the database

## Usage
To run the project: *dotnet run*

## Functionality
Modify appsetting.json to send text message, connect to database, and send email via Gmail

### Twilio
Create a Twilio account for text messaging functionality and uncomment twilio route in controller/HomeController.cs
> - AccountSid: 'enter twilio api key here'
> - AuthToken: 'enter twilio authentication token here'
> - TwilioPhoneNumber: 'enter twilio phone number here'

### Database
Connect to database 
> - "Name": 'enter connect name here'
> - ConnectionString: 'replace all examples'
>   - server=example
>   - userid=example
>   - password=example
>   - port=example
>   - database=example
>   - SslMode=example

### Google Api
Connect to Gmail to send email
> - Key: 'enter google api key here'
> - Email
>   - Account: 'enter email account here'
>   - Password: 'enter email password here'