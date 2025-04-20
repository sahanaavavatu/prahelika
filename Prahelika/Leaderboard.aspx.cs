using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prahelika
{
	using System;
using System.Data;
using System.Data.SqlClient;

public partial class Leaderboard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadLeaderboard();
        }
    }

    private void LoadLeaderboard()
    {
        string connStr = "Data Source=DESKTOP-LN69N0L\\AMRUTHASIDDHI;Initial Catalog=QDB;Integrated Security=True;MultipleActiveResultSets=true";
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 10 AuthorName, Score, LastCorrectTime, AuthorImageUrl 
                FROM Leaderboard 
                ORDER BY Score DESC, LastCorrectTime ASC", conn);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            rptLeaderboard.DataSource = dt;
            rptLeaderboard.DataBind();
        }
    }
}

}