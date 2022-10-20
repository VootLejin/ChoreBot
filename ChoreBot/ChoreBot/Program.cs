using Discord;
using Discord.WebSocket;

namespace ChoreBot
{
    public class Program
    {
        private DiscordSocketClient _client;

        public Program()
        {
            _client = new DiscordSocketClient();
        }

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {

            _client.Log += Log;
            string token = GetBotToken();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private static string GetBotToken()
        {
            // @Dave, Hit me up to get the token.
            var token = File.ReadAllText("token.txt");
            return token;
            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}