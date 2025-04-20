using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Prahelika
{
    public partial class QuizPage : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadActiveQuestion();
            }
        }

        private void LoadActiveQuestion()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT TOP 1 * FROM Questions WHERE IsActive = 1 ORDER BY DisplayOrder";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // ప్రశ్న & ఆప్షన్లు UI లో వేయి
                    questionLabel.InnerText = reader["QuestionText"].ToString();
                    btnA.Text = "A. " + reader["OptionA"].ToString();
                    btnB.Text = "B. " + reader["OptionB"].ToString();
                    btnC.Text = "C. " + reader["OptionC"].ToString();
                    btnD.Text = "D. " + reader["OptionD"].ToString();

                    // Correct Option ను ViewState లో ఉంచు
                    ViewState["CorrectOption"] = reader["CorrectOption"].ToString();
                    ViewState["QuestionID"] = reader["QuestionID"].ToString();
                }
                else
                {
                    Response.Redirect("FinalLeaderBoard.aspx");
                }
                reader.Close();

                // ✅ Check if DisplayTime is null and update it
                string checkDisplayTimeQuery = "SELECT DisplayTime FROM Questions WHERE QuestionID = @QID";
                SqlCommand checkCmd = new SqlCommand(checkDisplayTimeQuery, conn);
                checkCmd.Parameters.AddWithValue("@QID", ViewState["QuestionID"]);
                object displayTimeObj = checkCmd.ExecuteScalar();

                if (displayTimeObj == DBNull.Value || displayTimeObj == null)
                {
                    SetQuestionDisplayTime(Convert.ToInt32(ViewState["QuestionID"]), DateTime.Now);
                }
            }

            // styles reset చేయి
            ResetButtonStyles();
        }

        private void ResetButtonStyles()
        {
            btnA.CssClass = "option-button";
            btnB.CssClass = "option-button";
            btnC.CssClass = "option-button";
            btnD.CssClass = "option-button";
        }

        protected void btnReveal_Click(object sender, EventArgs e)
        {
            string correct = ViewState["CorrectOption"]?.ToString();

            // సరైనది green, మిగతా మూడు red
            btnA.CssClass += (correct == "A") ? " correct" : " wrong";
            btnB.CssClass += (correct == "B") ? " correct" : " wrong";
            btnC.CssClass += (correct == "C") ? " correct" : " wrong";
            btnD.CssClass += (correct == "D") ? " correct" : " wrong";

            LoadLeaderboardForCurrentQuestion(); // ✅ ప్రతి ప్రశ్నకి leaderboard చూపించు
            LoadLeaderboardForCurrentQuestionChart();
        }
        public  void SetQuestionDisplayTime(int questionId, DateTime displayTime)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Questions SET DisplayTime = @DisplayTime WHERE QuestionID = @QuestionID", conn);
                cmd.Parameters.AddWithValue("@DisplayTime", displayTime);
                cmd.Parameters.AddWithValue("@QuestionID", questionId);
                cmd.ExecuteNonQuery();
            }
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Get current question order
                int currentOrder = 0;
                string getCurrent = "SELECT DisplayOrder FROM Questions WHERE IsActive = 1";
                SqlCommand cmdGet = new SqlCommand(getCurrent, conn);
                object result = cmdGet.ExecuteScalar();
                if (result != null)
                {
                    currentOrder = Convert.ToInt32(result);
                }

                // Deactivate current question
                string deactivate = "UPDATE Questions SET IsActive = 0 WHERE IsActive = 1";
                SqlCommand cmdDeactivate = new SqlCommand(deactivate, conn);
                cmdDeactivate.ExecuteNonQuery();

                // Activate next question
                string activate = @"
            UPDATE Questions
            SET IsActive = 1
            OUTPUT INSERTED.QuestionID
            WHERE DisplayOrder = (
                SELECT MIN(DisplayOrder)
                FROM Questions
                WHERE DisplayOrder > @CurrentDisplayOrder
            )";
                SqlCommand cmdActivate = new SqlCommand(activate, conn);
                cmdActivate.Parameters.AddWithValue("@CurrentDisplayOrder", currentOrder);
                object newQIDObj = cmdActivate.ExecuteScalar();

                if (newQIDObj != null)
                {
                    int newQID = Convert.ToInt32(newQIDObj);
                    // ✅ Set DisplayTime = NOW()
                    SetQuestionDisplayTime(newQID, DateTime.Now);
                }
            }

            LoadActiveQuestion();
            leaderboardPanel.Visible = false;
        }

        //protected void btnNext_Click(object sender, EventArgs e)
        //{
        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();

        //        // ప్రస్తుతం యాక్టివ్ ప్రశ్న యొక్క DisplayOrder తీసుకోండి
        //        int currentOrder = 0;
        //        string getCurrent = "SELECT DisplayOrder FROM Questions WHERE IsActive = 1";
        //        SqlCommand cmdGet = new SqlCommand(getCurrent, conn);
        //        object result = cmdGet.ExecuteScalar();
        //        if (result != null)
        //        {
        //            currentOrder = Convert.ToInt32(result);
        //        }

        //        // యాక్టివ్ ప్రశ్నను deactivate చేయండి
        //        string deactivate = "UPDATE Questions SET IsActive = 0 WHERE IsActive = 1";
        //        SqlCommand cmdDeactivate = new SqlCommand(deactivate, conn);
        //        cmdDeactivate.ExecuteNonQuery();

        //        // తదుపరి ప్రశ్నను activate చేయండి
        //        string activate = @"
        //            UPDATE Questions
        //            SET IsActive = 1
        //            WHERE DisplayOrder = (
        //                SELECT MIN(DisplayOrder)
        //                FROM Questions
        //                WHERE DisplayOrder > @CurrentDisplayOrder
        //            )";
        //        SqlCommand cmdActivate = new SqlCommand(activate, conn);
        //        cmdActivate.Parameters.AddWithValue("@CurrentDisplayOrder", currentOrder);
        //        cmdActivate.ExecuteNonQuery();
        //    }

        //    LoadActiveQuestion();

        //    leaderboardPanel.Visible = false;


        //}
        private void LoadLeaderboardForCurrentQuestionChart()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string statsSql = @"
            SELECT 
                COUNT(*) AS Total,
                SUM(CASE WHEN IsCorrect = 1 THEN 1 ELSE 0 END) AS Correct,
                SUM(CASE WHEN IsCorrect = 0 THEN 1 ELSE 0 END) AS Wrong
            FROM Responses 
            WHERE QuestionID = @QID";

                SqlCommand statsCmd = new SqlCommand(statsSql, conn);
                statsCmd.Parameters.AddWithValue("@QID", ViewState["QuestionID"]);
                SqlDataReader statsReader = statsCmd.ExecuteReader();

                int total = 0, correct = 0, wrong = 0;

                if (statsReader.Read())
                {
                    total = statsReader["Total"] != DBNull.Value ? Convert.ToInt32(statsReader["Total"]) : 0;
                    correct = statsReader["Correct"] != DBNull.Value ? Convert.ToInt32(statsReader["Correct"]) : 0;
                    wrong = statsReader["Wrong"] != DBNull.Value ? Convert.ToInt32(statsReader["Wrong"]) : 0;
                }
                statsReader.Close();

                // JS Chart rendering
                string script = $"<script>renderChart({total}, {correct}, {wrong});</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "chartScript", script);

                // 🏆 Load Top Participants for this question
                string sql = @"
            SELECT TOP 10 
                r.AuthorName,
                r.ResponseTime AS LastCorrectTime,
                CASE 
                    WHEN r.IsCorrect = 1 THEN 1 
                    ELSE 0 
                END AS Score
            FROM Responses r
            WHERE r.QuestionID = @QID
            ORDER BY Score DESC, r.ResponseTime ASC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@QID", ViewState["QuestionID"]);
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    leaderboardPanel.Visible = false;
                    noResponsesLabel.Text = "ఈ ప్రశ్నకి ఇంకా ఎవరూ జవాబులివ్వలేదు.";
                    noResponsesLabel.Visible = true;
                }
                else
                {
                    rptLeaderboard.DataSource = reader;
                    rptLeaderboard.DataBind();
                    leaderboardPanel.Visible = true;
                    noResponsesLabel.Visible = false;
                }
            }
        }

        private void LoadLeaderboardForCurrentQuestion()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT TOP 10 
                        r.AuthorName,
                        r.ResponseTime AS LastCorrectTime,
                        CASE 
                            WHEN r.IsCorrect = 1 THEN 1 
                            ELSE 0 
                        END AS Score
                    FROM Responses r
                    INNER JOIN Questions q ON q.QuestionID = r.QuestionID
                    WHERE q.IsActive = 1
                    ORDER BY Score DESC, r.ResponseTime ASC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    leaderboardPanel.Visible = false;
                    noResponsesLabel.Text = "ఈ ప్రశ్నకి ఇంకా ఎవరూ జవాబులివ్వలేదు.";
                    noResponsesLabel.Visible = true;
                }
                else
                {
                    try
                    {
                        rptLeaderboard.DataSource = reader;
                        rptLeaderboard.DataBind();
                        leaderboardPanel.Visible = true;
                        noResponsesLabel.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        noResponsesLabel.Text = "లీడర్‌బోర్డ్ లో లోపం ఉంది: " + ex.Message;
                        noResponsesLabel.Visible = true;
                    }
                }
            }
        }
    }
}
