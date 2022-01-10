Database Migration & update commands:
*From inside SwishIdentity.Data*


dotnet ef --startup-project ../SwishIdentity migrations add first -c SwishDbContext -o ../SwishIdentity.Data/Migrations

dotnet ef --startup-project ../SwishIdentity database update -c SwishDbContext

** cleanup ssh & renew cert
