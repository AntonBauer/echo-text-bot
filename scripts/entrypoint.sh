#!/bin/bash

/app/scripts/migrate-db.sh
dotnet /app/EchoTextBot.Cli.dll
