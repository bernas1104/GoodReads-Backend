# GoodReads Backend
This application was developed as a challenge for the mentorship program that I participate. Some of the implementations
could be simpler, but I used the challenge as an opportunity to practice some concepts.

## Architecture
The applications follows a modular monolith approach. There are three modules:

1. Users - Responsible for storing all user information;
2. Books - Responsible for storing all books information;
3. Ratings - Responsible for store all ratings information.

The Users and Books module are simpler. The majority of their code is composed of a simple CRUD application.

The Ratings module is where the magic happens. When a user creates a new rating, he must provide a valid user and book.
When the rating is created, the module communicates the creation using MediatR and a saga begins.

If no errors occurs, a BookRating and a UserRating are added to the provided book and user, respectively.
If an error occurs on the User module, the created Rating is removed.
If an error occurs on the Book module, both the UserRating and Rating are removed.

As part of the challenge, different modules use different databases. The User and Book modules operate on a SQL Server
database. The Ratings module operates on a MongoDb database.

There is also a MemoryCache/Redis Cache available for use.

## Local usage
To run the application locally, you will need the following:

- Docker;
- SQL Server Docker image;
- MongoDb Docker image;
- Redis Docker image (optional).

How to setup the docker images is beyond the scope of this README documentation.

After setting everything up, just provide the connection strings and other configurations to the appsettings.json file.

## Migrations
- Users
```bash
dotnet ef --project src/GoodReads.Infrastructure -s src/GoodReads.Api migrations add \<UserMigrationName\> --context UsersContext -o ./EntityFramework/Migrations/Users;

dotnet ef --project src/GoodReads.Infrastructure -s src/GoodReads.Api database update --context UsersContext;
```

- Books
```bash
dotnet ef --project src/GoodReads.Infrastructure -s src/GoodReads.Api migrations add \<BookMigrationName\> --context BooksContext -o ./EntityFramework/Migrations/Books;

dotnet ef --project src/GoodReads.Infrastructure -s src/GoodReads.Api database update --context BooksContext;
```

## Tests & Sonarqube
```bash
dotnet test -p:CollectCoverage=true -p:CoverletOutputFormat=opencover -p:CoverletOutput=./coverage.opencover.xml;

dotnet sonarscanner begin -k:"GoodReads" -d:sonar.host.url="http://localhost:9000" -d:sonar.login="sqp_a957f68bafa7a0345d464184573c4420f9c75924" -d:sonar.cs.opencover.reportsPaths="./test/GoodReads.Unit.Tests/coverage.opencover.xml,./test/GoodReads.Integration.Tests/coverage.opencover.xml";

dotnet build;

dotnet sonarscanner end -d:sonar.login="sqp_a957f68bafa7a0345d464184573c4420f9c75924";
```

**Disclaimer** The token "sqp_a957f68bafa7a0345d464184573c4420f9c75924" was created for my local Sonarqube instance and must
be replaced for your own token.