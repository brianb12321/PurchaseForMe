FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/net5.0/publish/ App/
ENV CLUSTER_DOCKER "True"
ENV PROJECT_PATH "/projects"
WORKDIR /App
EXPOSE 80
ENTRYPOINT ["dotnet", "PurchaseForMeWeb.dll"]