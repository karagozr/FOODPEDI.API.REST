#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["FOODPEDI.API.REST/FOODPEDI.API.REST.csproj", "FOODPEDI.API.REST/"]
RUN dotnet restore "FOODPEDI.API.REST/FOODPEDI.API.REST.csproj"
COPY . .
WORKDIR "/src/FOODPEDI.API.REST"
RUN dotnet build "FOODPEDI.API.REST.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FOODPEDI.API.REST.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FOODPEDI.API.REST.dll"]