FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY peteryoungwebsitebackend.API.csproj ./
RUN dotnet restore peteryoungwebsitebackend.API.csproj

COPY . ./
RUN dotnet publish peteryoungwebsitebackend.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "peteryoungwebsitebackend.API.dll"]
