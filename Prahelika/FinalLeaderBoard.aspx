<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinalLeaderboard.aspx.cs" Inherits="Prahelika.FinalLeaderboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Top 10 Leaderboard</title>
    <style>
        body {
            background-color: #000814;
            color: white;
            font-family: 'Segoe UI', sans-serif;
        }

        .leaderboard {
            max-width: 800px;
            margin: 30px auto;
            background: #001d3d;
            padding: 20px;
            border-radius: 15px;
            box-shadow: 0 0 20px #ffc300;
        }

        h2 {
            text-align: center;
            color: #ffc300;
        }

        .row {
            display: flex;
            padding: 10px;
            border-bottom: 1px solid #ffc300;
            align-items: center;
        }

        .row.header {
            font-weight: bold;
            background-color: #003566;
        }

        .row div {
            flex: 1;
            text-align: center;
        }

        .profile-pic {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            object-fit: cover;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="Button1" Text="Final Winner" runat="server" OnClick="Button1_Click"/>

        <div class="leaderboard">
            <h2>🏆 Final Leaderboard - Top 10</h2>
            


            <asp:Repeater ID="rptTop10" runat="server">
                <HeaderTemplate>
                    <div class="row header">
                        <div>Rank</div>
                        <div>Photo</div>
                        <div>Name</div>
                        <div>Score</div>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="row">
                        <div><%# Container.ItemIndex + 1 %></div>
                        <div>
                            <img class="profile-pic" src='<%# Eval("AuthorImageUrl") %>' alt="Profile" />
                        </div>
                        <div><%# Eval("AuthorName") %></div>
                        <div><%# Eval("Score") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </form>
</body>
</html>
