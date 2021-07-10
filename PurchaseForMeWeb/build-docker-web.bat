dotnet publish PurchaseForMeWeb.csproj -c Release /p:PublishProfile=FolderProfile
docker build -t ghcr.io/brianb12321/purchase-for-me-web-image:latest -f Dockerfile .