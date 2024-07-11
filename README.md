# Echo Text Bot

Telegram bot that transcribes audio messages into text

## Getting started

1. Download whisper model  
``
docker run -it --rm -v ./assets:/models whisper.cpp:main "./models/download-ggml-model.sh base /models"
``

2. Build image
``
docker build -t <image name> .
``

3. Run image
``
docker run -it --rm -e ECHO_TEXT_BOT_TG_TOKEN=<Your Telegram Token> <image name>
``
