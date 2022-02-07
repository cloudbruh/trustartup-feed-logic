FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CloudBruh.Trustartup.FeedLogic/CloudBruh.Trustartup.FeedLogic.csproj", "CloudBruh.Trustartup.FeedLogic/"]
RUN dotnet restore "CloudBruh.Trustartup.FeedLogic/CloudBruh.Trustartup.FeedLogic.csproj"
COPY . .
WORKDIR "/src/CloudBruh.Trustartup.FeedLogic"
RUN dotnet build "CloudBruh.Trustartup.FeedLogic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CloudBruh.Trustartup.FeedLogic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudBruh.Trustartup.FeedLogic.dll"]
