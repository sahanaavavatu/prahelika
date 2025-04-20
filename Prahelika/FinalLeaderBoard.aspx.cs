using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Prahelika
{
    public partial class FinalLeaderboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTop10Participants();
            }
        }

        private void LoadTop10Participants()
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"SELECT TOP 10 AuthorName, Score, AuthorImageUrl 
                               FROM Leaderboard 
                               ORDER BY Score DESC, LastCorrectTime ASC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                rptTop10.DataSource = reader;
                rptTop10.DataBind();

                reader.Close();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("FinalWinner.aspx");
        }
    }
}
