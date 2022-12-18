This is an asp razor page .NET Core 3.1 with identity basic website.

It allows a user to sign up and share unique identity information with a manager. 

A user must request permission to become a manager.

The purpose was a simple KYC identity information management setup and interface for users & managers in one web app.
_

To run:

Replace api keys & db connection string.

Database Migration & update commands:

*From inside SwishIdentity.Data*

dotnet ef --startup-project ../SwishIdentity migrations add first -c SwishDbContext -o ../SwishIdentity.Data/Migrations

dotnet ef --startup-project ../SwishIdentity database update -c SwishDbContext

** cleanup ssh & renew cert

