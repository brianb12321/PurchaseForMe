dotnet publish PurchaseForMeWeb.csproj -c Release /p:PublishProfile=FolderProfile
docker build -t purchase-for-me-web-image:latest -f Dockerfile .