FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Socks5.Web.Test/Socks5.Web.Test.csproj", "Socks5.Web.Test/"]
RUN dotnet restore "Socks5.Web.Test/Socks5.Web.Test.csproj"
COPY . .
WORKDIR "/src/Socks5.Web.Test"
RUN dotnet build "Socks5.Web.Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Socks5.Web.Test.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Socks5.Web.Test.dll"]
