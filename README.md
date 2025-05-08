Email Service Project
This is a .NET 8 project for sending and managing emails, featuring a class library for email logic, a Web API for email operations, a console client for sending emails via HTTP, and an MVC frontend with a Razor Pages dashboard. The project uses SQLite for data storage and includes a background service for processing queued emails.
Project Structure

EmailsServiceLibrary: Class library containing email sending logic, a background service for processing email queues, and SQLite database persistence.
EmailService.API: ASP.NET Core Web API that consumes EmailsServiceLibrary, providing endpoints for sending single and bulk emails, with Swagger UI for documentation.
EmailService.Console: Console application that sends single and bulk emails by making HTTP requests to EmailService.API.
EmailService.Frontend: ASP.NET Core MVC web application with Razor Pages, featuring a dashboard to view email records and forms for sending single and bulk emails.

Prerequisites

Visual Studio 2022 (with .NET 8 SDK installed)
.NET 8 Runtime and SDK (Download)
SQLite Browser (DB Browser for SQLite, Download)
Git (to clone the repository)

Setup Instructions
1.Open Visual Studio 2022.

2. Open the Solution

In Visual Studio, click File > Open > Project/Solution.
Navigate to the folder containing the project and select the .sln file (e.g., EmailSystem.sln).

3. Restore NuGet Packages

In Solution Explorer, right-click the solution and select Restore NuGet Packages.
Ensure the following packages are installed (check each projects .csproj file):
EmailsServiceLibrary:
Microsoft.EntityFrameworkCore.Sqlite (8.0.0)
Microsoft.EntityFrameworkCore (8.0.0)
Microsoft.EntityFrameworkCore.Tools (8.0.0)
Microsoft.EntityFrameworkCore.Design (8.0.0)
Microsoft.EntityFrameworkCore.Relational (8.0.0)


EmailService.API:
Microsoft.EntityFrameworkCore.Sqlite (8.0.0)
Microsoft.EntityFrameworkCore (8.0.0)
Microsoft.EntityFrameworkCore.Tools (8.0.0)
Microsoft.EntityFrameworkCore.Design (8.0.0)
Microsoft.Extensions.Hosting (8.0.0)
Swashbuckle.AspNetCore (6.5.0)


EmailService.Frontend:
Microsoft.AspNetCore.Mvc (8.0.0)
Microsoft.Extensions.Http (8.0.0)
Microsoft.Extensions.Logging (8.0.0)


EmailService.Console:
No additional packages (uses System.Net.Http).


4. Configure Multiple Startup Projects

In Solution Explorer, right-click the solution and select Set Startup Projects.
Choose Multiple startup projects.
Set the following projects to Start:
EmailService.API
EmailService.Frontend
EmailService.Console

Click Apply and OK.

5. Configure SQLite Database
The project uses SQLite for storing email records, managed by EmailsServiceLibrary.

Set Package Manager Console Project:

In Visual Studio, go to Tools > NuGet Package Manager > Package Manager Console.
In the Default project dropdown, select EmailsServiceLibrary.


Run Migrations:

In Package Manager Console, run:Add-Migration InitialCreate -StartupProject EmailService.API


This generates migration files in EmailsServiceLibrary/Migrations.


Update Database:

Run:Update-Database -StartupProject EmailService.API


This creates the SQLite database (EmailServiceDb.sqlite) in the EmailService.API project folder.


Verify Database:

Install DB Browser for SQLite if not already installed.
Open DB Browser for SQLite.
Click Open Database.
Navigate to the EmailService.API project folder (e.g., path\to\EmailService.API).
Select EmailServiceDb.sqlite and open.
Browse the Emails table to view email records.



6. Configure Email Settings
The email sending functionality uses Gmails SMTP server. Update the appsettings.json in EmailService.API with valid credentials.

Open EmailService.API/appsettings.json.
Update the EmailConfig section:
"EmailConfig": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "SenderName": "EmailService System",
    "Password": "your-app-password"
}


Replace your-email@gmail.com with your Gmail address.
Generate an App Password in your Google Account:
Go to Google Account Settings.
Enable 2-Step Verification if not enabled.
Navigate to Security > App passwords.
Generate a new app password for Mail and copy it.
Paste the app password into the Password field.


7. Run the Project

Press F5 in Visual Studio to build and run all startup projects.
The following will launch:
EmailService.API: Opens Swagger UI in a browser at https://localhost:7109/swagger.
EmailService.Frontend: Opens the MVC dashboard in a browser at https://localhost:7027/Emails.
EmailService.Console: Opens a command-line window with an interactive menu.



8. Using the Applications
EmailService.API (Swagger)

Access Swagger at https://localhost:7109/swagger.
Test endpoints:
POST /api/Email/send-single-email: Send a single email.
POST /api/Email/send-bulk-emails: Send bulk emails.
GET /api/Email/all: Retrieve all email records.



EmailService.Frontend (MVC Dashboard)

Navigate to https://localhost:7027/Emails.
Single Email Form:
Enter a recipient email, subject, and message body.
Click Send Email.


Bulk Email Form:
Enter comma-separated recipient emails, subject, and message body.
Click Send Bulk Email.


Email Records Table:
Displays email records with status badges (green for Sent, red for others).



EmailService.Console

In the command-line window, choose an option:
1. Send single email: Enter recipient email, subject, and body.
2. Send bulk email: Enter comma-separated recipient emails, subject, and body.
3. Exit: Close the console app.


The console sends HTTP requests to EmailService.API.

9. Troubleshooting
Database Issues

Migration Fails:
Ensure EmailsServiceLibrary is selected in Package Manager Console.
Verify the connection string in EmailService.API/appsettings.json:"ConnectionStrings": {
    "DefaultConnection": "Data Source=EmailServiceDb.sqlite"
}


Delete the Migrations folder and re-run Add-Migration and Update-Database.


Cannot Open Database:
Confirm EmailServiceDb.sqlite exists in the EmailService.API folder.
Use DB Browser for SQLite to verify the database structure.



Email Sending Issues

SMTP Errors:
Verify the Gmail app password in appsettings.json.
Test SMTP connectivity:using System.Net.Mail;
var client = new SmtpClient("smtp.gmail.com", 587)
{
    Credentials = new System.Net.NetworkCredential("your-email@gmail.com", "your-app-password"),
    EnableSsl = true
};
client.Send("your-email@gmail.com", "test@example.com", "Test", "Hello");




Single Email Fails in Frontend:
Check the error in the dashboard (ViewData["SingleEmailError"]).
Inspect console logs in Visual Studios Output window for EmailApiService errors.
Test the API endpoint with Postman:curl -X POST https://localhost:7109/api/Email/send-single-email \
-H "Content-Type: application/json" \
-d '{"recipient":"test@example.com","subject":"Test","messageBody":"Hello"}'





Startup Issues

Projects Dont Start:
Confirm multiple startup projects are configured (see step 4).
Check port conflicts (default ports: API on 7109, Frontend on 7291).
Update ports in EmailService.API/Properties/launchSettings.json and EmailService.Frontend/Properties/launchSettings.json if needed.


Console App Closes:
Ensure the console app is set to Start in startup projects.
Check for exceptions in the console output.



10. Viewing Email Records

MVC Dashboard: View email records at https://localhost:7027/Emails.
SQLite Browser:
Open EmailServiceDb.sqlite in DB Browser for SQLite.
Select the Emails table to view columns: EmailId, Sender, Recipient, Subject, MessageBody, Status, DateSent, NumberOfAttempts.


Swagger: Use GET /api/Email/all to retrieve email records.

11. Notes

The project uses .NET 8 runtime and SDK.
The background service in EmailsServiceLibrary processes emails from a queue, ensuring asynchronous email sending.
The frontend uses Bootstrap 5 for styling the dashboard.
Ensure valid Gmail credentials and app password for email sending.


.
