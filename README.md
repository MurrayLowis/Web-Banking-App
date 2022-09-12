# S3852734-S3862651-a2

![languages](https://img.shields.io/badge/-dotnet-informational?logo=csharp&logoColor=F8F8FF&style=flat-square) [![RMIT](https://img.shields.io/badge/RMIT-WebDevTech-informational)](https://rmit.instructure.com/courses/102750) ![.NET](https://img.shields.io/badge/.6.0-Core-informational?logo=dotnet&logoColor=AAF683&style=flat-square) ![Azure Data Studio](https://img.shields.io/badge/Azure-SQL%20Server-informational?logo=microsoftsqlserver&logoColor=AAF683&style=flat-square)

## GitHub Repository URL

[Web Development Technologies assignment 2](https://github.com/rmit-wdt-sp2-2022/S3852734-S3862651-a2)

## Trello Board

[Trello Board - assignment 2](https://trello.com/b/ZmFE58I5/assignment-2)

## Contribution Form

[Contribution Form](https://github.com/rmit-wdt-sp2-2022/S3852734-S3862651-a2/Contribution/WDT_Assignment2_ContributionForm_Group3.pdf)

## Application Description

### Changes to Database Schema

- Added a boolean to the 
  - Customer and BillPay tables to denote a FROZEN status 
  - BillPay to denote a CANCELLED status
- Changed LoginID and PasswordHash to be nvarchar, not nvchar.

## Notes

The programs pull data from the API which interacts directly with the database, and then stores certain key values in the session including customerID, customer name, account number, and also some parameters used for filtering on certain pages.

Certain shared view components are used to store common elements.

We've made both the Customer and Admin websites work through the API, instead of just the Admin website.

If a user does not have any active (not paid one-off Billpays, and not cancelled Billpays), opening the BillPays menu will go straight to the create a new BillPay Menu.

We've used TailwindCSS and DaisyUI instead of Bootstrap.

## NOTE REGARDING TESTING DOCUMENTATION

The current testing directory is partially complete and refactoring immediately prior to submission has resulted in some issues with the rest of the project. As such we have just included the testing folder in its current state as a .zip file.