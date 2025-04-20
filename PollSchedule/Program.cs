using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var fetcher = new YouTubeFetcher();
        bool chatReady = await fetcher.FetchLiveChatIdAsync();

        if (!chatReady)
        {
            Console.WriteLine("❌ Live Chat ID తీసుకోలేకపోయాం. VideoId తప్పు అయుండచ్చు.");
            return;
        }

        Console.WriteLine("✅ Live Chat Monitoring Start అవుతోంది...");

        while (true)
        {
            try
            {
                var messages = await fetcher.FetchMessagesAsync();

                foreach (var msg in messages)
                {
                    string commentId = msg.Id;
                    if (QuizSession.IsAlreadyProcessed(commentId))
                        continue;

                    QuizSession.MarkAsProcessed(commentId);

                    string authorName = msg.AuthorDetails.DisplayName;
                    string authorChannelId = msg.AuthorDetails.ChannelId;
                    string commentText = msg.Snippet.DisplayMessage;
                    string youtubeurl = msg.AuthorDetails.ChannelUrl;
                    string profileurl = msg.AuthorDetails.ProfileImageUrl;

                    ResponseProcessor.ProcessResponse(commentId, authorChannelId, authorName, commentText, QuizSession.GetQuestionStartTime(), youtubeurl, profileurl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Error occurred: " + ex.Message);
            }

            await Task.Delay(5000);
        }
    }
}