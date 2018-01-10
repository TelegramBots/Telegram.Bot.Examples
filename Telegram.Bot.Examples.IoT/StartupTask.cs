using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

namespace Telegram.Bot.Examples.IoT
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            RunBot().Wait();

            _deferral.Complete();
        }

        private static async Task RunBot()
        {
            InitGPIO(47);

            var Bot = new TelegramBotClient("Your API Key");

            var offset = 0;

            while (true)
            {
                var updates = await Bot.GetUpdatesAsync(offset);

                foreach (var update in updates)
                {
                    switch (update.Type)
                    {
                        case UpdateType.Message:
                            var message = update.Message;

                            switch (message.Type)
                            {
                                case MessageType.Text:

                                    if (message.Text == "/toggle")
                                    {
                                        ToggleLED();
                                        break;
                                    }

                                    await Bot.SendTextMessageAsync(
                                        message.Chat.Id,
                                        message.Text,
                                        replyToMessageId: message.MessageId);

                                    break;
                            }
                            break;
                    }

                    offset = update.Id + 1;
                }
            }
        }

        private static GpioPin LED;
        private static bool LEDOn;

        private static void InitGPIO(int pinNumber)
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null) return;

            LED = gpio.OpenPin(pinNumber);

            LED.Write(GpioPinValue.Low);
            LED.SetDriveMode(GpioPinDriveMode.Output);
        }

        private static void ToggleLED()
        {
            LEDOn = !LEDOn;
            LED?.Write(LEDOn ? GpioPinValue.High : GpioPinValue.Low);
        }
    }
}
