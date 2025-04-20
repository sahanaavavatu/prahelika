using System;
using System.Data.SqlClient;

public static class DbHelper
{
    private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

    // 1. Get active question
    public static (int QuestionID, string CorrectOption)? GetActiveQuestion()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT TOP 1 QuestionID, CorrectOption FROM Questions WHERE IsActive = 1 ORDER BY DisplayOrder";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return (reader.GetInt32(0), reader.GetString(1));
                }
                return null;
            }
        }
    }

    // 2. Check if this is first response by user for question
    public static bool IsFirstResponse(string authorChannelId, int questionId)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT COUNT(*) FROM Responses WHERE AuthorChannelId = @AuthorChannelId AND QuestionID = @QuestionID";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@AuthorChannelId", authorChannelId);
                cmd.Parameters.AddWithValue("@QuestionID", questionId);
                int count = (int)cmd.ExecuteScalar();
                return count == 0;
            }
        }
    }

    // 3. Get DisplayTime from Questions table
    public static DateTime? GetQuestionDisplayTime(int questionId)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT DisplayTime FROM Questions WHERE QuestionID = @QID", conn);
            cmd.Parameters.AddWithValue("@QID", questionId);
            object result = cmd.ExecuteScalar();
            if (result != DBNull.Value && result != null)
            {
                return Convert.ToDateTime(result);
            }
            return null;
        }
    }

    // 4. Insert new response
    public static void InsertResponse(string commentId, string authorChannelId, string authorName, string selectedOption, int questionId, bool isCorrect, DateTime responseTime)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"
INSERT INTO Responses (CommentID, AuthorChannelId, AuthorName, SelectedOption, QuestionID, IsCorrect, ResponseTime)
VALUES (@CommentID, @AuthorChannelId, @AuthorName, @SelectedOption, @QuestionID, @IsCorrect, @ResponseTime)";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CommentID", commentId);
                cmd.Parameters.AddWithValue("@AuthorChannelId", authorChannelId);
                cmd.Parameters.AddWithValue("@AuthorName", authorName);
                cmd.Parameters.AddWithValue("@SelectedOption", selectedOption);
                cmd.Parameters.AddWithValue("@QuestionID", questionId);
                cmd.Parameters.AddWithValue("@IsCorrect", isCorrect ? 1 : 0);
                cmd.Parameters.AddWithValue("@ResponseTime", responseTime);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // 5. Update leaderboard with bonus logic
    public static void UpdateLeaderboard(string authorChannelId, string authorName, bool isCorrect, bool isFirstResponse, DateTime responseTime, int questionId, string utubeURL, string imageUrl)
    {
        int basePoints = 5;
        int bonusMultiplier = 1;
        int delayInSeconds = 999;

        // ✅ Get DisplayTime from DB
        var displayTime = GetQuestionDisplayTime(questionId);
        if (displayTime.HasValue)
        {
            delayInSeconds = (int)(responseTime - displayTime.Value).TotalSeconds;

            // ✅ X3 if within 15 sec, X2 if within 30 sec
            if (isCorrect && isFirstResponse)
            {
                if (delayInSeconds <= 15)
                    bonusMultiplier = 3;
                else if (delayInSeconds <= 30)
                    bonusMultiplier = 2;
            }
        }

        int finalScoreToAdd = (isCorrect && isFirstResponse) ? basePoints * bonusMultiplier : 0;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand(@"
MERGE INTO Leaderboard AS Target
USING (SELECT @AuthorChannelId AS AuthorChannelId) AS Source
ON Target.AuthorChannelId = Source.AuthorChannelId
WHEN MATCHED THEN 
    UPDATE SET
        AuthorName = @AuthorName,
        AuthorChannelUrl = @AuthorChannelUrl,
        AuthorImageUrl = @AuthorImageUrl,
        TotalAttempts = TotalAttempts + CASE WHEN @IsFirstResponse = 1 THEN 1 ELSE 0 END,
        CorrectCount = CorrectCount + CASE WHEN @IsCorrect = 1 AND @IsFirstResponse = 1 THEN 1 ELSE 0 END,
        Score = Score + @ScoreToAdd,
        LastCorrectTime = CASE WHEN @IsCorrect = 1 THEN @CorrectTime ELSE LastCorrectTime END
WHEN NOT MATCHED THEN
    INSERT (
        AuthorChannelId, 
        AuthorName, 
        AuthorChannelUrl, 
        AuthorImageUrl, 
        Score, 
        CorrectCount, 
        TotalAttempts, 
        LastCorrectTime)
    VALUES (
        @AuthorChannelId,
        @AuthorName,
        @AuthorChannelUrl,
        @AuthorImageUrl,
        @ScoreToAdd,
        CASE WHEN @IsCorrect = 1 AND @IsFirstResponse = 1 THEN 1 ELSE 0 END,
        CASE WHEN @IsFirstResponse = 1 THEN 1 ELSE 0 END,
        CASE WHEN @IsCorrect = 1 THEN @CorrectTime ELSE NULL END);", conn);

            cmd.Parameters.AddWithValue("@AuthorChannelId", authorChannelId);
            cmd.Parameters.AddWithValue("@AuthorName", authorName);
            cmd.Parameters.AddWithValue("@IsCorrect", isCorrect ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsFirstResponse", isFirstResponse ? 1 : 0);
            cmd.Parameters.AddWithValue("@CorrectTime", isCorrect ? (object)responseTime : DBNull.Value);
            cmd.Parameters.AddWithValue("@ScoreToAdd", finalScoreToAdd);
            cmd.Parameters.AddWithValue("@AuthorImageUrl", imageUrl ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AuthorChannelUrl", utubeURL ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }
    }
}
