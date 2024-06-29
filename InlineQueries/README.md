# Telegram.Bot Inline Queries Example

## About

This example demonstrates a Telegram Bot that respond to [Inline Queries](https://core.telegram.org/bots/inline).

It's a simple "Knowledge Base" bot that will list the articles whose names starts with the letters specified by the user.  
When the user select an article from the resulting list, the content of the article (formatted with HTML) is posted
and the bot may tell which article was selected.

## Prerequisites

* Same as Telegram.Bot.Examples.Polling.
* You need to set your bot token in the `TOKEN` environment variable *(in launchSettings.json or via Project Properties > Debug > General)*
* You need to enable "**Inline mode on**" in BotFather for this bot to work.
* If you want the bot to tell which article was selected by the user, you need to enable "**Inline feedback**" (for example: 100%)
