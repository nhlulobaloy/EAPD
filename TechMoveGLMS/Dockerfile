FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TechMoveGLMS/TechMoveGLMS.csproj", "TechMoveGLMS/"]
COPY ["TechMoveGLMS.API/TechMoveGLMS.API.csproj", "TechMoveGLMS.API/"]
RUN dotnet restore "TechMoveGLMS/TechMoveGLMS.csproj"
COPY . .
WORKDIR "/src/TechMoveGLMS"
RUN dotnet publish "TechMoveGLMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechMoveGLMS.dll"]
