using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Ovikello
{
	public class Program
	{
		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.MessageReceived += _client_MessageReceived;

            var token = "NzQyNjc2OTg3NTg3NTkyMjUy.XzJl2A._ycOarcx7nyFCeQSuOWC6_uOg4c";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await doorbellAsync();

            await Task.Delay(-1);
        }

        private async Task doorbellAsync()
        {
            GpioController controller = new GpioController(PinNumberingScheme.Board);
            controller.OpenPin(10, PinMode.InputPullDown);
            while (true)
            {
                var pin = controller.Read(10);
                if (pin == PinValue.High)
                {
                    await Console.Out.WriteLineAsync("Ovikello soi! " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                    await _client.GetGuild(742635035865382932).GetTextChannel(742635035865382936).SendMessageAsync("@everyone Ovikello soi!");
                    "cvlc --play-and-exit /sound.mp3".Bash();
                    await Task.Delay(3000);
                }
            }
        }

        private async Task _client_MessageReceived(SocketMessage msg)
        {
            if (msg.Content == "!siivoa")
            {
                IEnumerable<IMessage> messages = await msg.Channel.GetMessagesAsync(10).FlattenAsync();
                foreach (var item in messages)
                {
                    await item.DeleteAsync();
                }
            }
        }

        private async Task Log(LogMessage msg)
        {
            await Console.Out.WriteLineAsync(msg.ToString());
        }
    }
}
