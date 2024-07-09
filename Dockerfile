FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./EchoTextBot.Cli/EchoTextBot.Cli.csproj ./EchoTextBot.Cli/
RUN dotnet restore ./EchoTextBot.Cli/

# copy and publish app and libraries
COPY . .
RUN dotnet publish ./EchoTextBot.Cli/ --no-restore -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy
RUN apt update && apt install -y ffmpeg
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["."]