#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Yousei"
RUN dotnet restore "Yousei.csproj" \
    && dotnet build "Yousei.csproj" --nologo -c Release -o /app/build -warnaserror

FROM build AS publish
RUN dotnet publish "Yousei.csproj" --nologo -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yousei.dll"]
