FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as build
WORKDIR /app

COPY ./src/*.csproj ./
RUN dotnet restore

COPY ./src ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY --from=build /app/out .
ENV CONNECTION_DB_SR "Host=ec2-54-247-189-1.eu-west-1.compute.amazonaws.com;Database=d9kd2lupkhiavc;User Id=ewtbbtfgifawsd;Password=0abf762f65a09b4d683ac77cf80044ce0f892f6e53f3fc8ff5a60d507b341cf0;sslmode=Require;Trust Server Certificate=true;"
CMD ["dotnet", "SoftwareRequirements.dll"]