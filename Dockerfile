# build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /source

COPY ./EchoTextBot.Cli/EchoTextBot.Cli.csproj ./EchoTextBot.Cli/
RUN dotnet restore ./EchoTextBot.Cli/ -a x64

COPY . .
RUN dotnet publish ./EchoTextBot.Cli/ --no-restore -a x64 -o /app

# runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy as runtime
RUN apt update && apt install -y ffmpeg
RUN apt insatll sqlite3

WORKDIR /app

COPY --from=build /app .
COPY --from=build /app/db ./db
COPY --from=build ./assets/ggml-base.bin ./assets/

RUN mkdir /data
VOLUME [ "/data" ]

ENV ECHO_TEXT_BOT_WHISPER_MODEL_PATH="assets/ggml-base.bin"
ENV ECHO_TEXT_BOT_FETCH_INTERVAL=5000
ENV ECHO_TEXT_BOT_DB_FILE_PATH="/data/echo-bot.db"

USER $APP_UID
ENTRYPOINT ["dotnet", "EchoTextBot.Cli.dll"]