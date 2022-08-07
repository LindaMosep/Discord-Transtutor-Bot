using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

public class UserWithChannel
{
    public SocketUser user;
    public SocketTextChannel channel;
    public float second;
    public SocketMessage message;
    public bool isProcessFinished;

    public UserWithChannel(SocketUser user, SocketTextChannel channel, float second, SocketMessage message, bool isProcessFinished)
    {
        this.user = user;
        this.channel = channel;
        this.second = second;
        this.message = message;
        this.isProcessFinished=isProcessFinished;
    }

}

internal static class Program
{
    public static DiscordSocketClient _client;

    public static bool isWorking = false;
    public static EmbedFooterBuilder SignFooter;
    public static int numOfFiles = 0;
    public static List<UserWithChannel> users = new List<UserWithChannel>();
    public static int FileCount;
    public static int FileCount2;
    public static CookieContainer cookie;
    public static string footer;
    public static void Main()
    {
        Console.WriteLine(Environment.CurrentDirectory);
        MainAsync().Wait();


    }
    static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    public static async Task MainAsync()
    {

        _client = new DiscordSocketClient(new DiscordSocketConfig()
        { AlwaysDownloadUsers = true, GatewayIntents= GatewayIntents.All });

        SignFooter = new EmbedFooterBuilder().WithText("Powered by Meowdemia!").WithIconUrl("https://media.discordapp.net/attachments/917813714923683860/923807194640687134/Grey_Cute_Illustrated_Cat_and_Fish_Circle_Laptop_Sticker_1.png?width=559&height=559");


        footer = File.ReadAllText(Environment.CurrentDirectory + "/footer.txt");


        // driver = new EdgeDriver(edgeOptions);

        _client.Log += Log;

        var cfg = new DiscordSocketConfig();




        await _client.LoginAsync(TokenType.Bot, "");
        await _client.StartAsync();


        await _client.SetGameAsync("LindaMosep!", "https://github.com/LindaMosep", ActivityType.Playing);
        _client.MessageReceived += TakeDown;
        _client.Ready += _client_Ready; 
        await Task.Delay(-1);
    }

    private static async Task _client_Ready()
    {
        Console.WriteLine("xx");
        GetCookies();
        Console.WriteLine("ff");
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.transtutors.com");
        request.CookieContainer = cookie;
        request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
        request.Timeout = 50000;
       
      
        


    }

    public static Task TakeDown(SocketMessage e)
    {

        MessageRecieve(e);

        return Task.CompletedTask;
    }

    public static async Task SlowMode(SocketGuildUser user, SocketTextChannel channel, SocketMessage e, bool isProcessFinished)
    {

        var uch = new UserWithChannel(user, channel, 300, e, false);
        users.Add(uch);

        while (!users.Find(m => m.user == uch.user && m.channel == uch.channel).isProcessFinished)
        {

            await Task.Delay(1);
        }


        for (int i = 300; i > 1; i--)
        {

            await Task.Delay(1);
            users.Find(m => m.user == uch.user && m.channel == uch.channel).second = i;


        }

        SlowModeFinished(users.Find(m => m.user == uch.user && m.channel == uch.channel).message);
        users.Remove(uch);

    }

    public static EmbedBuilder NotValidLink()
    {
        var embed = new EmbedBuilder().WithTitle("```Please send a valid url!```").WithColor(new Color(255, 0, 0)).WithDescription("The url you sent is not valid, please sent a valid url!")
            .WithThumbnailUrl("https://media.discordapp.net/attachments/917813714923683860/931391847996211200/b26d38dd88deb86ab2ae4c0de4d1785e.gif").WithFooter(SignFooter);

        return embed;


    }


    public static async Task<RestUserMessage> ProcessStarted(SocketMessage e)
    {
        var embed = new EmbedBuilder().WithTitle("Process started succesfully!").WithColor(new Color(255, 255, 0)).WithDescription(e.Author.Mention+ $" Please wait until process completed.")
                      .WithThumbnailUrl("https://media.discordapp.net/attachments/917813714923683860/927371809119174666/loader_backinout_1.gif").WithFooter(SignFooter);
        var msg = await e.Channel.SendMessageAsync("", false, embed.Build(), null, null, new MessageReference(e.Id));
        return msg;

    }

    public static EmbedBuilder ProcessCompleted(SocketMessage e)
    {
        var embed = new EmbedBuilder().WithTitle("Process completed succesfully!").WithColor(new Color(0, 255, 0)).WithDescription(e.Author.Mention+ $" Please check your DM!.")
                      .WithThumbnailUrl("https://media.discordapp.net/attachments/917813714923683860/927372219431129148/c3b6e85cfdddd49e731f27c31e4fc5e6_1.gif").WithFooter(SignFooter);
        return embed;
    }

    public static EmbedBuilder NotAnswered(SocketMessage e)
    {
        var embed = new EmbedBuilder().WithTitle("This question not answered yet!").WithColor(Color.Blue).WithDescription(e.Author.Mention+ $" This question not answered yet, please try again later!")
                      .WithThumbnailUrl("https://media.discordapp.net/attachments/917813714923683860/933374941934526514/google-question-mark.gif").WithFooter(SignFooter);
        return embed;
    }

    public static async Task SlowModeMessage(SocketMessage e, float second, SocketGuildUser user, SocketTextChannel channel)
    {
        var embed = new EmbedBuilder().WithTitle("Your in cooldown!").WithColor(new Color(255, 255, 0)).WithDescription(e.Author.Mention+ $" Your currently in cooldown, wait a **{second / 100:F2}** second.")
                      .WithThumbnailUrl("https://images-ext-1.discordapp.net/external/8pBdRwZ4cXwRdZPAoBhesOdgZa2EQMSznqHFGYt7iyg/https/cdn.dribbble.com/users/2015153/screenshots/6592242/progess-bar2.gif?width=745&height=559").WithFooter(SignFooter);
        users.Find(m => m.user == user && m.channel == channel).message = e;
        await e.Channel.SendMessageAsync("", false, embed.Build(), null, null, new MessageReference(e.Id));
    }

    public static async Task SlowModeFinished(SocketMessage e)
    {
        var embed = new EmbedBuilder().WithTitle("Your cooldown finished!").WithColor(new Color(0, 0, 255)).WithDescription(e.Author.Mention+ $" You can use me now.")
                      .WithThumbnailUrl("https://images-ext-2.discordapp.net/external/-xCsuG6EsrPfO15glJR33j2U57ztvS432HaW0D0oRV0/https/cdn.dribbble.com/users/1162077/screenshots/5427775/media/612968fb2a4690f4959deb23a00eb2d0.gif?width=745&height=559").WithFooter(SignFooter);
        try
        {
            await e.Author.SendMessageAsync("", false, embed.Build(), null, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    public static EmbedBuilder MessageError(SocketMessage e)
    {
        var embed = new EmbedBuilder();
        if ((e.Channel as IGuildChannel).Guild.GetRole(922634093471092737) != null)
        {
            embed = new EmbedBuilder().WithTitle("```It's seems there is an error!```").WithColor(new Color(255, 0, 0)).WithDescription(e.Author.Mention+ $" **Please report to {(e.Channel as IGuildChannel).Guild.GetRole(922634093471092737).Mention}**")
                   .WithThumbnailUrl("https://images-ext-2.discordapp.net/external/wOqzpmK--O8wLGpd1AHc4Bv2lBFebnihQSV0JpkcD7k/https/cdn.dribbble.com/users/2182116/screenshots/13933572/media/cc32730b1eb3a657a48a6ceacefc72fb.gif?width=745&height=559").WithFooter(SignFooter);

        }
        else
        {
            embed = new EmbedBuilder().WithTitle("```It's seems there is an error!```").WithColor(new Color(255, 0, 0)).WithDescription(e.Author.Mention+ $" **Please report to Admin!**")
                  .WithThumbnailUrl("https://images-ext-2.discordapp.net/external/wOqzpmK--O8wLGpd1AHc4Bv2lBFebnihQSV0JpkcD7k/https/cdn.dribbble.com/users/2182116/screenshots/13933572/media/cc32730b1eb3a657a48a6ceacefc72fb.gif?width=745&height=559").WithFooter(SignFooter);

        }

        return embed;

    }

    public static void GetCookies()
    {

        cookie = new CookieContainer();
        var msgs = _client.GetGuild(922634093445918751).GetTextChannel(933345284682035270).GetMessagesAsync(1).ToListAsync();
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(msgs.Result.ToList()[0].ToList()[0].Attachments.ToList()[0].Url);


        WebClient myWebClient = new WebClient();



        // Download the resource and load the bytes into a buffer.
        byte[] buffer = myWebClient.DownloadData(msgs.Result.ToList()[0].ToList()[0].Attachments.ToList()[0].Url);

        // Encode the buffer into UTF-8
        string download = Encoding.UTF8.GetString(buffer);
        var CookiesString = download;

        var CookieLines = Regex.Split(CookiesString, "},");
        foreach (var text in CookieLines)
        {
            var name = text.Substring(text.IndexOf("\"name\": \"") + "\"name\": \"".Length);
            name = name.Remove(name.IndexOf("\""));


            var value = text.Substring(text.IndexOf("\"value\": \"") + "\"value\": \"".Length);
            value = value.Remove(value.IndexOf("\""));

            var domain = text.Substring(text.IndexOf("\"domain\": \"") + "\"domain\": \"".Length);
            domain = domain.Remove(domain.IndexOf("\""));


            cookie.Add(new Cookie(name, value, "/", domain));
        }
    }


    public static async Task MessageRecieve(SocketMessage e)
    {

        if (e.Content.StartsWith("https://www.transtutors.com/questions/"))
        {
            var user = (e.Author as IGuildUser);
            if (user != null)
            {
                var guild = _client.GetGuild((e.Author as IGuildUser).GuildId);
                if (guild != null)
                {
                    if (!users.Exists(m => m.user == e.Author as SocketGuildUser && m.channel == e.Channel as SocketTextChannel))
                    {

                        List<string> linksinmessage = new List<string>();
                        foreach (Match item in Regex.Matches(e.Content, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
                        {
                            linksinmessage.Add(item.Value);
                        }

                        if (linksinmessage.Count > 0)
                        {
                            var isUserHere = false;
                            if (!(e.Author as IGuildUser).RoleIds.ToList().Contains(925007529572962405))
                            {
                                SlowMode(e.Author as SocketGuildUser, e.Channel as SocketTextChannel, e, false);
                                isUserHere = true;
                            }
                            var startmsg = await ProcessStarted(e);
                            GetCookies();
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(linksinmessage[0]);
                            request.CookieContainer = cookie;
                            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                            request.Timeout = 50000;

                            WebResponse wbc = null;
                            try
                            {
                                WebResponse wbb = request.GetResponse();
                                wbc = wbb;



                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine(ex.Message);
                                await startmsg.ModifyAsync(m => m.Embed = NotValidLink().Build());
                                wbc = null;

                                if (isUserHere)
                                {
                                    users.Find(m => m.user.Id == e.Author.Id && m.channel.Id == e.Channel.Id).isProcessFinished = true;
                                }
                            }

                            if (wbc != null)
                            {
                                try
                                {
                                    var stream = wbc.GetResponseStream();
                                    var sr = new StreamReader(stream);


                                    var data = sr.ReadToEnd();

                                    data = data.Replace("href='//", "href='https://");
                                    data = data.Replace("href=\"//", "href=\"https://");
                                    data = data.Replace("href='/", "href='https://transtutor.com/");
                                    data = data.Replace("href=\"/", "href=\"https://transtutor.com/");
                                    data = data.Replace("src='//", "src='https://");
                                    data = data.Replace("src=\"//", "src=\"https://");
                                    data = data.Replace("src='/", "src='https://transtutor.com/");
                                    data = data.Replace("src=\"/", "src=\"https://transtutor.com/");

                                   

                                    if (!data.Contains("Page not found -Transtutors"))
                                    {
                                        if(!data.Contains("<p id=\"pGetSolvedBtn\""))
                                        {
                                            try
                                            {

                                                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                                doc.Load(new StringReader(data));



                                                var count = FileCount + 1;
                                                FileCount++;
                                                doc.DocumentNode.Descendants("header").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower() == "active").ToList().ForEach(m => m.Remove());
                                                doc.DocumentNode.Descendants("div").Where(n => n.Attributes["id"] != null).Where(n => n.Attributes["id"].Value.ToLower() == "review").ToList().ForEach(m => m.Remove());
                                                doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower() == "topcont").ToList().ForEach(m => m.Remove());
                                                doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower().Contains("pe-optin")).ToList().ForEach(m => m.Remove());
                                                doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower() == "right").ToList().ForEach(m => m.Remove());
                                                doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower() == "relquestion").ToList().ForEach(m => m.Remove());
                                                doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower().Contains("qa_left_right")).ToList().ForEach(m => m.Remove());
                                                doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower().Contains("qa_left_right")).ToList().ForEach(m => m.Remove());
                                                doc.DocumentNode.Descendants("footer").ToList().ForEach(m => m.Remove());
                                                // doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower().Contains("headertitle")).ToList().ForEach(m => m.Remove());
                                                // doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower().Contains("tutor-driver")).ToList().ForEach(m => m.Remove());
                                                // doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower().Contains("searchbrowse")).ToList().ForEach(m => m.Remove());
                                                // doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower().Contains("clearfix answers")).ToList().ForEach(m => m.Remove());
                                                // doc.DocumentNode.Descendants("div").Where(n => n.Attributes["class"] != null).Where(n => n.Attributes["class"].Value.ToLower().Contains("relatedcontenttabs")).ToList().ForEach(m => m.Remove());
                                                // doc.DocumentNode.Descendants("footer").ToList().ForEach(m => m.Remove());

                                                
                                                FileStream sw = new FileStream(Environment.CurrentDirectory + $@"\Meowdemia_Transtutor({count}).html", FileMode.Create);
                                              
                                                doc.Save(sw);
                                               
                                              
                                                while (!File.Exists(Environment.CurrentDirectory  + $@"\Meowdemia_Transtutor({count}).html"))
                                                {
                                                    int c = 0;
                                                    c++;
                                                }
                                               
                                                sw.Close();
                                                File.WriteAllText(Environment.CurrentDirectory  + $@"\Meowdemia_Transtutor({count}).html", File.ReadAllText(Environment.CurrentDirectory  + $@"\Meowdemia_Transtutor({count}).html").ToString() + "\n" + footer);
                                                var msg = await _client.GetUser(411997699647340545).SendFileAsync(Environment.CurrentDirectory + $@"\Meowdemia_Transtutor({count}).html");
                                                    
                                                string urlOfFile = "";
                                                foreach (var mg in msg.Attachments)
                                                {
                                                    urlOfFile = mg.Url;
                                                }


                                                File.Delete(Environment.CurrentDirectory  +$@"\Meowdemia_Transtutor({count}).html");

                                                EmbedBuilder TakeThis = new EmbedBuilder().WithTitle("Here it is, best postman in the whole World!").WithColor(Color.Green).AddField("Could you open the door please? I got some documentation for you. Study well!", $"[Download!]({urlOfFile})")
                                                                                                                                .WithThumbnailUrl("https://images-ext-1.discordapp.net/external/LUCi0IWmG8Jd-2yZlGW2Z1IHes2_A5KOuOfnV1KecwQ/https/cdn.dribbble.com/users/1616371/screenshots/6042217/media/844e55475e98ada8d64e1166e4d5a1b1.gif").WithFooter(SignFooter);
                                                try
                                                {
                                                    await e.Author.SendMessageAsync(null, false, TakeThis.Build());
                                                }
                                                catch
                                                {
                                                    TakeThis.Description += " \n Please activate \"Allow direct messages from this server.\" setting.";
                                                    await e.Channel.SendMessageAsync(null, false, TakeThis.Build());
                                                }

                                                await startmsg.ModifyAsync(m => m.Embed = ProcessCompleted(e).Build());


                                                if (isUserHere)
                                                {
                                                    users.Find(m => m.user.Id == e.Author.Id && m.channel.Id == e.Channel.Id).isProcessFinished = true;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                               
                                                await startmsg.ModifyAsync(m => m.Embed = MessageError(e).Build());
                                                if (isUserHere)
                                                {
                                                    users.Find(m => m.user.Id == e.Author.Id && m.channel.Id == e.Channel.Id).isProcessFinished = true;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            await startmsg.ModifyAsync(m => m.Embed = NotAnswered(e).Build());
                                            if (isUserHere)
                                            {
                                                users.Find(m => m.user.Id == e.Author.Id && m.channel.Id == e.Channel.Id).isProcessFinished = true;
                                            }
                                        }
                                      
                                    }
                                    else
                                    {
                                        await startmsg.ModifyAsync(m => m.Embed = NotValidLink().Build());

                                        if (isUserHere)
                                        {
                                            users.Find(m => m.user.Id == e.Author.Id && m.channel.Id == e.Channel.Id).isProcessFinished = true;
                                        }


                                    }


                                }
                                catch (Exception ex)
                                {
                                    
                                    await startmsg.ModifyAsync(m => m.Embed = MessageError(e).Build());
                                    if (isUserHere)
                                    {
                                        users.Find(m => m.user.Id == e.Author.Id && m.channel.Id == e.Channel.Id).isProcessFinished = true;
                                    }

                                }






                            }



                        }
                    }
                    else
                    {
                        if (users.Find(m => m.user == e.Author as SocketGuildUser && m.channel == e.Channel as SocketTextChannel) != null)
                        {
                            if (users.Find(m => m.user == e.Author as SocketGuildUser && m.channel == e.Channel as SocketTextChannel).isProcessFinished)
                            {
                                await SlowModeMessage(e, users.Find(m => m.user == e.Author as SocketGuildUser && m.channel == e.Channel as SocketTextChannel).second, e.Author as SocketGuildUser, e.Channel as SocketTextChannel);
                            }

                        }
                    }
                }


            }
        }

    }
}


