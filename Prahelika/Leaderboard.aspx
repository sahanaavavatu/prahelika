<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Leaderboard.aspx.cs" Inherits="Prahelika.Leaderboard" %>

<!DOCTYPE html>
<html lang="te">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>క్విజ్ లీడర్‌బోర్డ్</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            font-family: 'Noto Sans Telugu', sans-serif;
            padding: 20px;
            background-color: #f7f7f7;
        }
        .leaderboard-container {
            overflow-x: auto;
            overflow-y: auto;
            max-height: 500px;
            background: #ffffff;
            border-radius: 15px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        }
        table {
            width: 100%;
            min-width: 600px;
        }
        th, td {
            text-align: center;
            vertical-align: middle;
            white-space: nowrap;
        }
        .profile-img {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            object-fit: cover;
            border: 2px solid #007bff;
        }
        .rank-badge {
            font-size: 1.2rem;
            font-weight: bold;
        }
        .table thead {
            position: sticky;
            top: 0;
            background-color: #343a40;
            color: white;
        }
        .telugu-header {
            font-size: 1.1rem;
        }
        @media (max-width: 768px) {
            .profile-img {
                width: 30px;
                height: 30px;
            }
        }
    </style>
</head>
<body>

    <div class="container leaderboard-container">
        <asp:Label ID="noResponsesLabel" runat="server" CssClass="text-danger" Visible="false"></asp:Label>

        <asp:Repeater ID="rptLeaderboard" runat="server">
            <HeaderTemplate>
                <table class="table table-bordered">
                    <thead>
                        <tr class="telugu-header">
                            <th>స్థానం</th>
                            <th>ప్రొఫైల్</th>
                            <th>పేరు</th>
                            <th>పాయింట్లు</th>
                            <th>గెలిచిన టైం</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><span class="rank-badge"><%# Container.ItemIndex + 1 %></span></td>
                    <td><img src='<%# Eval("AuthorImageUrl") %>' class="profile-img" /></td>
             <div><%# Container.ItemIndex + 1 %></div>
            <div><%# Eval("AuthorName") %></div>
            <div><%# Eval("LastCorrectTime", "{0:HH:mm:ss}") %></div>
            <div><%# Eval("Score") %></div>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</body>
</html>
