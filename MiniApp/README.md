# Telegram.Bot Mini-App example

This example was built starting from Visual Studio template project: ASP.NET Core Web App (Razor Pages)

Minimal changes were made:
- Program.cs: for the bot webhook and starting the example website as a Telegram Mini-App
- _Layout.cshtml: importing required telegram-web-app.js

-----

Now we also included the WebAppDemo and DurgerKingBot example bot
(in demo.cshtml, cafe.cshtml, Cafe.cs and some wwwroot static files)

## WebAppDemo & DurgerKingBot examples

Static data imported from official WebApps:
- https://webappcontent.telegram.org/cafe
- https://webappcontent.telegram.org/demo

Server-side code reconstructed from above, and adapted from:
- https://github.com/arynyklas/DurgerKingBot
- https://github.com/telegram-bot-php/durger-king

## Notes

For DurgerKing to serve invoices, you will need to set a "PaymentProviderToken" in appsettings.json
_(typically from [Stripe in TEST mode](https://telegrambots.github.io/book/4/payments.html))_

Sending WebAppData to bot (button "Send time to bot") works only when opening the webapp via a ReplyKeyboardButton
_(try using the second "Hello World!" button)_
