FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PhoneBook.Api.csproj", "./"]
RUN dotnet restore "PhoneBook.Api.csproj"
COPY . .
RUN dotnet build "PhoneBook.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhoneBook.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/logs
ENTRYPOINT ["dotnet", "PhoneBook.Api.dll"]
