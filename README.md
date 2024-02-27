# Migrations
- Users
```bash
dotnet ef --project src/GoodReads.Infrastructure -s src/GoodReads.Api migrations add InitialUsersMigration --context UsersContext -o ./EntityFramework/Migrations/Users;

dotnet ef --project src/GoodReads.Infrastructure -s src/GoodReads.Api database update --context UsersContext;
```

- Books
```bash
dotnet ef --project src/GoodReads.Infrastructure -s src/GoodReads.Api migrations add InitialBooksMigration --context BooksContext -o ./EntityFramework/Migrations/Books;

dotnet ef --project src/GoodReads.Infrastructure -s src/GoodReads.Api database update --context BooksContext;
```

# Tests & Sonarqube
dotnet test -p:CollectCoverage=true -p:CoverletOutputFormat=opencover -p:CoverletOutput=./coverage.opencover.xml;

dotnet sonarscanner begin -k:"GoodReads" -d:sonar.host.url="http://localhost:9000" -d:sonar.login="sqp_a957f68bafa7a0345d464184573c4420f9c75924" -d:sonar.cs.opencover.reportsPaths="./test/GoodReads.Unit.Tests/coverage.opencover.xml,./test/GoodReads.Integration.Tests/coverage.opencover.xml";

dotnet build;

dotnet sonarscanner end -d:sonar.login="sqp_a957f68bafa7a0345d464184573c4420f9c75924";