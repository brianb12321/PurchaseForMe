dotnet publish PurchaseForMeService.csproj -c Release /p:PublishProfile=FolderProfile
docker build -t ghcr.io/brianb12321/purchase-for-me-service-image:latest -f Dockerfile .