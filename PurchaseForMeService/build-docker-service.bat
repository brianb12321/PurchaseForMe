dotnet publish PurchaseForMeService.csproj -c Release /p:PublishProfile=FolderProfile
docker build -t purchase-for-me-service-image:latest -f Dockerfile .