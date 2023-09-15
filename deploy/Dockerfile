FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Fuksi.Loyalty.Web/Fuksi.Loyalty.Web.csproj", "src/Fuksi.Loyalty.Web/"]
COPY ["src/Fuksi.Loyalty.Module.Auth/Fuksi.Loyalty.Module.Auth.csproj", "src/Fuksi.Loyalty.Module.Auth/"]
RUN dotnet restore "src/Fuksi.Loyalty.Web/Fuksi.Loyalty.Web.csproj"
COPY . .
WORKDIR "/src/src/Fuksi.Loyalty.Web"
RUN dotnet build "Fuksi.Loyalty.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fuksi.Loyalty.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fuksi.Loyalty.Web.dll"]
