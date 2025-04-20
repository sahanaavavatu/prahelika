using System;
using System.Collections.Generic;

public static class QuizSession
{
    public static DateTime QuestionStartTime { get; private set; } = DateTime.Now;

    private static HashSet<string> processedCommentIds = new HashSet<string>();

    public static void Reset()
    {
        QuestionStartTime = DateTime.Now;
        processedCommentIds.Clear();
        Console.WriteLine("🔄 QuizSession reset for new question.");
    }

    public static bool IsAlreadyProcessed(string commentId)
    {
        return processedCommentIds.Contains(commentId);
    }

    public static void MarkAsProcessed(string commentId)
    {
        processedCommentIds.Add(commentId);
    }

    public static DateTime GetQuestionStartTime()
    {
        return QuestionStartTime;
    }
}
