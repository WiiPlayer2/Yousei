#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
RUN apt-get update \
    && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_15.x | bash - \
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /src
COPY . .
RUN dotnet restore "Yousei.Web/Yousei.Web.csproj"
WORKDIR "/src/Yousei.Web"
RUN dotnet msbuild "Yousei.Web.csproj" -t:YarnInstall
RUN dotnet build "Yousei.Web.csproj" -c Release -o /app/build -warnaserror

FROM build AS publish
RUN dotnet publish "Yousei.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yousei.Web.dll"]
