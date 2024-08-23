using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Payments;

#pragma warning disable CA1305, IDE0059, IDE1006

internal static class Cafe
{
    internal record OrderItem(int id, int count);
    internal record FoodItem(string name, string descr, double price, string img, string png, string tgs);

    internal static FoodItem[] FoodItems = JsonSerializer.Deserialize<FoodItem[]>(File.ReadAllText("wwwroot/cafe.json"))!;

    internal static async Task<object> OnCafeApi(TelegramBotClient bot, IConfiguration config, IFormCollection form)
    {
        var query = AuthHelpers.ParseValidateData(form["_auth"], config["BotToken"]!);
        var orderData = JsonSerializer.Deserialize<OrderItem[]>(form["order_data"]!)!;
        if (form["method"] != "makeOrder" || form["invoice"] != "1")
            return new { ok = false, error = "only Invoice mode is implemented" };
        string payload = "";
        var prices = new List<LabeledPrice>();
        foreach (var orderItem in orderData)
        {
            payload += $"{orderItem.id}:{orderItem.count} ";
            var foodItem = FoodItems[orderItem.id];
            prices.Add(new($"{foodItem.name} x{orderItem.count}", (int)(foodItem.price * orderItem.count * 100)));
        }
        return new
        {
            ok = true,
            invoice_url = await bot.CreateInvoiceLinkAsync("Order #12345678", "Perfect lunch from Durger King",
                payload.ToString(), config["PaymentProviderToken"], "USD", prices, needName: true, needPhoneNumber: true, needShippingAddress: true)
        };
    }

    internal static string? OnPreCheckoutQuery(PreCheckoutQuery pcq)
    {
        int total_amount = 0; // recompute total price to be sure
        foreach (var orderItem in pcq.InvoicePayload.Split())
            if (orderItem.Split(':') is [var id, var count])
                total_amount += (int)(FoodItems[int.Parse(id)].price * int.Parse(count) * 100);
        return total_amount == pcq.TotalAmount ? null : "incorrect payment";
    }
}
