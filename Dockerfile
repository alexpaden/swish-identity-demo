FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["SwishIdentity.sln", "./"]
COPY ["SwishIdentity/SwishIdentity.csproj", "./SwishIdentity/"]
COPY ["SwishIdentity.Data/SwishIdentity.Data.csproj", "./SwishIdentity.Data/"]
COPY ["SwishIdentity.Tools/DependencyService/SwishIdentity.Tools.csproj", "./SwishIdentity.Tools/DependencyService/"]
RUN echo $ls
RUN dotnet restore --disable-parallel

COPY . .
WORKDIR "/src/."

RUN dotnet build "SwishIdentity/SwishIdentity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SwishIdentity/SwishIdentity.csproj" -c Release -o /app 

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SwishIdentity.dll"]
