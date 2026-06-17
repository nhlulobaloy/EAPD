FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TechMoveGLMS.API/TechMoveGLMS.API.csproj", "TechMoveGLMS.API/"]
RUN dotnet restore "TechMoveGLMS.API/TechMoveGLMS.API.csproj"
COPY . .
WORKDIR "/src/TechMoveGLMS.API"
RUN dotnet publish "TechMoveGLMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechMoveGLMS.API.dll"]
