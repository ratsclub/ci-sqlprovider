FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SomethingPro.Core/SomethingPro.Core.fsproj", "SomethingPro.Core/"]
COPY ["SomethingPro.Database/SomethingPro.Database.fsproj", "SomethingPro.Database/"]
COPY ["SomethingPro.Infrastructure/SomethingPro.Infrastructure.fsproj", "SomethingPro.Infrastructure/"]
RUN dotnet restore "SomethingPro.Core/SomethingPro.Core.fsproj"
COPY . .
WORKDIR "/src/SomethingPro.Core"
RUN dotnet build "SomethingPro.Core.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SomethingPro.Core.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SomethingPro.Core.dll"]
