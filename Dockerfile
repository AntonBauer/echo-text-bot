# build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /source

COPY ./EchoTextBot.Cli/EchoTextBot.Cli.csproj ./EchoTextBot.Cli/
COPY ./TgBot/TgBot.csproj ./TgBot/
RUN dotnet restore ./EchoTextBot.Cli/ -a x64

COPY . .
RUN dotnet publish ./EchoTextBot.Cli/ --no-restore -a x64 -o /app

# runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy as runtime
RUN apt update && apt install -y ffmpeg
RUN apt install -y sqlite3

WORKDIR /app

COPY --from=build app .
COPY ./assets/ggml-base.bin ./assets/
COPY ./migrations/ ./migrations
COPY ./scripts/ ./scripts

RUN mkdir data
VOLUME [ "/app/data" ]

ENV ECHO_TEXT_BOT_WHISPER_MODEL_PATH="/app/assets/ggml-base.bin"
ENV ECHO_TEXT_BOT_FETCH_INTERVAL=5000
ENV ECHO_TEXT_BOT_OFFSET_DB_FILE_PATH="/app/data/echo-text-bot.db"

USER $APP_UID
ENTRYPOINT ["/app/scripts/entrypoint.sh"]
