#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
RUN apt-get update \
    && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /src
COPY . .
WORKDIR "/src/Yousei.Web"
RUN dotnet restore "Yousei.Web.csproj" \
    && dotnet build "Yousei.Web.csproj" -nologo -c Release -o /app/build -warnaserror -nowarn:CS0436,CS8604

FROM build AS publish
RUN dotnet publish "Yousei.Web.csproj" --nologo -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yousei.Web.dll"]
