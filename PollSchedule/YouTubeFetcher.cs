using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

public class YouTubeFetcher
{
    private readonly string apiKey;
    private readonly string applicationName;
    private readonly string liveVideoId;
    private readonly YouTubeService youtubeService;
    private string liveChatId;

    public YouTubeFetcher()
    {
        apiKey = ConfigurationManager.AppSettings["YouTubeApiKey"];
        applicationName = ConfigurationManager.AppSettings["YouTubeAppName"];
        liveVideoId = ConfigurationManager.AppSettings["LiveVideoId"];

        youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = apiKey,
            ApplicationName = applicationName
        });
    }

    // 👉 LiveChatId ని డైనమిక్ గా YouTube API ద్వారా తీయడం
    public async Task<bool> FetchLiveChatIdAsync()
    {
        Console.WriteLine($"LiveVideoId: {liveVideoId}"); // Debug

        var videoRequest = youtubeService.Videos.List("liveStreamingDetails");
        videoRequest.Id = liveVideoId;

        try
        {
            var response = await videoRequest.ExecuteAsync();
            if (response.Items.Count > 0 && response.Items[0].LiveStreamingDetails != null)
            {
                liveChatId = response.Items[0].LiveStreamingDetails.ActiveLiveChatId;
                return true;
            }

            Console.WriteLine("LiveChatId not found.");
            return false;
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine("Google API Error: " + ex.Message);
            return false;
        }
    }

    // 👉 Live chat message లు fetch చేయడం
    public async Task<List<LiveChatMessage>> FetchMessagesAsync()
    {
        var messages = new List<LiveChatMessage>();

        if (string.IsNullOrEmpty(liveChatId))
        {
            Console.WriteLine("LiveChatId is not available.");
            return messages;
        }

        var request = youtubeService.LiveChatMessages.List(liveChatId, "snippet,authorDetails");
        request.MaxResults = 200;

        var response = await request.ExecuteAsync();
        messages.AddRange(response.Items);

        return messages;
    }
}
