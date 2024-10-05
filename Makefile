docker-build:
	docker build -t catscraper:latest . 

docker-remove-image:
	docker rmi catscraper:latest

add-migration:
	dotnet ef migrations add $(name) -s ./CatScraper/CatScraper.csproj -p ./CatScraper/CatScraper.csproj --output-dir Infrastructure/Persistence/Migrations
	
database-update:
	dotnet ef database update -s ./CatScraper/CatScraper.csproj -p ./CatScraper/CatScraper.csproj

remove-migration:
	dotnet ef migrations remove -s ./CatScraper/CatScraper.csproj -p ./CatScraper/CatScraper.csproj 

docker-mssql:
	docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin12345" -p 1433:1433 --name cat-scraper-mssql -d mcr.microsoft.com/mssql/server:2022-latest

dotnet-test:
	dotnet test CatScraper.Tests/CatScraper.Tests.csproj

