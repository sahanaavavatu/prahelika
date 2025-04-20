using System;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Prahelika
{
    public partial class FinalWinner : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFinalWinner();
            }
        }

        private void LoadFinalWinner()
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Assume highest score and earliest time wins
                string sql = @"
                    SELECT TOP 1 AuthorName, Authorimageurl 
                    FROM Leaderboard 
                    ORDER BY Score DESC, LastCorrectTime ASC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblWinnerName.Text = reader["AuthorName"].ToString();

                    string photoUrl = reader["Authorimageurl"].ToString();
                    if (!string.IsNullOrEmpty(photoUrl))
                    {
                        imgWinner.ImageUrl = photoUrl;
                    }
                    else
                    {
                        imgWinner.ImageUrl = "~/assets/default-avatar.png"; // fallback
                    }
                }

                reader.Close();
            }
        }
    }
}
