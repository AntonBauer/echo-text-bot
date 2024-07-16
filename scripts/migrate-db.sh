#!/bin/bash

cat /app/migrations/01_offset.sql | sqlite3 /app/data/echo-text-bot.db