cd PurchaseForMeService
call build-docker-service.bat
cd ..\PurchaseForMeWeb
call build-docker-web.bat
cd ..
docker compose build