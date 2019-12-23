FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as build
WORKDIR /app

COPY ./src/*.csproj ./
RUN dotnet restore

COPY ./src ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app

COPY --from=build /app/out .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet "SoftwareRequirements.dll" 