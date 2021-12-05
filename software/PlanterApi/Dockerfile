FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /tmp2

COPY *.sln .
COPY PlanterApi/*.csproj ./PlanterApi/
RUN dotnet restore -r linux-arm64

COPY PlanterApi/. ./PlanterApi/
WORKDIR /tmp2/PlanterApi
RUN dotnet publish -c release -o /app -r linux-arm64 --self-contained false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-arm64v8
EXPOSE 5000
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./PlanterApi"]
