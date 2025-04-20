using System;
using System.Collections.Generic;

public static class ResponseProcessor
{
    // 👉 సరైన సమాధానాల ఎంపికలు A, B, C, D మాత్రమే అనుమతించాలి
    private static readonly HashSet<string> validOptions = new HashSet<string> { "A", "B", "C", "D" };

    // 👉 ప్రతి new comment కి ఈ method కాల్ అవుతుంది
    public static void ProcessResponse(string commentId, string authorChannelId, string authorName, string commentText, DateTime questionStartTime, string youtubeurl, string profileurl)
    {
        // 👉 Comment ను clean చేసి uppercase లో మార్చడం (e.g., a → A)
        string cleanComment = commentText.Trim().ToUpper();

        // 👉 సరైన ఎంపిక కాదంటే method ని exit చేయడం
        if (!validOptions.Contains(cleanComment))
            return;

        // 👉 ప్రస్తుతం Active గా ఉన్న ప్రశ్నను DB నుంచి తీసుకోవడం
        var activeQuestion = DbHelper.GetActiveQuestion();
        if (activeQuestion == null) return; // 👉 ఎలాంటి ప్రశ్న లేకపోతే బయటకి రావడం

        int questionId = activeQuestion.Value.QuestionID; // 👉 ప్రశ్న ID తీసుకోవడం
        string correctOption = activeQuestion.Value.CorrectOption.ToUpper(); // 👉 Correct option ను Uppercase లోకి మార్చడం

        // 👉 యూజర్ నుండి వచ్చిన మొదటి attempt మాత్రమే పరిగణనలోకి తీసుకోవాలి
        bool isFirstResponse = DbHelper.IsFirstResponse(authorChannelId, questionId);
        if (!isFirstResponse) return; // 👉 ఇదివరకే attempt చేసినవాడు అయితే బయటకి రావాలి

        // 👉 Answer correct ఉందో లేదో చెక్ చేయడం
        bool isCorrect = cleanComment == correctOption;

        // 👉 ఇప్పుడు సమాధానం ఇచ్చిన టైమ్ తీసుకోవడం
        DateTime responseTime = DateTime.Now;

        // 👉 ప్రశ్న display అయిన టైం తో ఇప్పుడు సమాధానం ఇచ్చిన టైం తేడా లెక్కించడం (delay)
        int delayInSeconds = (int)(responseTime - questionStartTime).TotalSeconds;

        // 👉 Response details ను DB లో save చేయడం
        DbHelper.InsertResponse(commentId, authorChannelId, authorName, cleanComment, questionId, isCorrect, responseTime);

        // 👉 Leaderboard ను అప్డేట్ చేయడం (responseTime తప్పనిసరిగా pass చేయాలి)
        DbHelper.UpdateLeaderboard(
            authorChannelId,        // 👉 Channel ID
            authorName,             // 👉 Display name
            isCorrect,              // 👉 సమాధానం సరైందా?
            isFirstResponse,        // 👉 మొదటి attempt అా?
            responseTime,           // 👉 సమాధానం ఇచ్చిన టైం
            questionId,             // 👉 ప్రశ్న ID
            youtubeurl,             // 👉 యూజర్ యొక్క YouTube URL
            profileurl              // 👉 యూజర్ యొక్క profile photo URL
        );
    }
}
