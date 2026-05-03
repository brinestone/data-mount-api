FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef
WORKDIR /src
COPY ["DataMount.Api/DataMount.Api.csproj", "DataMount.Api/"]
COPY . .
RUN dotnet restore "DataMount.Api/DataMount.Api.csproj"
RUN /root/.dotnet/tools/dotnet-ef migrations bundle --self-contained -r linux-x64 --project DataMount.Infra --startup-project DataMount.Api
WORKDIR "/src/DataMount.Api"
RUN dotnet build "./DataMount.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DataMount.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/efbundle .
ENTRYPOINT ["dotnet", "DataMount.Api.dll"]
