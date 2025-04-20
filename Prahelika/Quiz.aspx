<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuizPage.aspx.cs" Inherits="Prahelika.QuizPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>KBC Live Quiz</title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
    <style>
        body {
            background-color: #000814;
            font-family: 'Segoe UI', sans-serif;
            color: white;
            margin: 0;
            padding: 0;
            overflow-x: hidden;
        }

        .quiz-container {
            width: 100%;
            max-width: 800px;
            margin: 20px auto;
            padding: 20px 15px;
            background: #001d3d;
            border-radius: 15px;
            box-shadow: 0 0 15px #ffc300;
            display: flex;
            flex-direction: column;
        }

        .question-label {
            font-size: 28px;
            text-align: center;
            margin-bottom: 25px;
            word-wrap: break-word;
            line-height: 1.5;
        }

        .options {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 15px;
            margin-bottom: 25px;
        }

        .option-button {
            font-size: 20px;
            font-weight: bold;
            padding: 16px;
            border: 2px solid #fff;
            border-radius: 10px;
            background-color: #003566;
            color: #fff;
            transition: 0.3s ease;
            width: 100%;
            text-align: center;
            white-space: normal;
            word-break: break-word;
            min-height: 80px;
            line-height: 1.5;
        }

        .option-button:hover {
            background-color: #004080;
        }

        .correct {
            background-color: #28a745 !important;
            border-color: #28a745;
        }

        .wrong {
            background-color: #dc3545 !important;
            border-color: #dc3545;
        }

        .action-buttons {
            display: flex;
            flex-direction: row;
            justify-content: space-between;
            gap: 12px;
        }

        .reveal-button, .next-button {
            flex: 1;
            padding: 14px;
            font-size: 18px;
            border: none;
            border-radius: 8px;
            font-weight: bold;
            cursor: pointer;
        }

        .reveal-button {
            background-color: #00b4d8;
            color: black;
        }

        .next-button {
            background-color: #ffc300;
            color: black;
        }

        @media (max-width: 600px) {
            .quiz-container {
                padding: 15px;
                margin: 10px;
            }

            .question-label {
                font-size: 20px;
            }

            .options {
                grid-template-columns: 1fr;
                gap: 10px;
            }

            .option-button {
                font-size: 18px;
                padding: 16px;
            }

            .action-buttons {
                flex-direction: column;
            }

            .reveal-button, .next-button {
                font-size: 18px;
                padding: 16px;
            }

            .leaderboard-row {
                flex-direction: column;
                align-items: center;
                text-align: center;
            }

            .leaderboard-row div {
                width: 100%;
                margin: 2px 0;
            }
        }

        .leaderboard-panel {
            max-width: 800px;
            margin: 20px auto;
            padding: 20px;
            background: #001d3d;
            border-radius: 15px;
            box-shadow: 0 0 10px #ffc300;
        }

        .leaderboard-header {
            text-align: center;
            font-size: 22px;
            margin-bottom: 10px;
            color: #ffc300;
        }

        .leaderboard-row {
            display: flex;
            padding: 8px;
            border-bottom: 1px dashed #fff;
        }

        .leaderboard-row div {
            flex: 1;
            text-align: center;
        }

        .leaderboard-row.header {
            font-weight: bold;
            border-bottom: 2px solid #ffc300;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="noResponsesLabel" runat="server" CssClass="text-danger" Visible="false"></asp:Label>

        <div class="quiz-container">
            <h2 id="questionLabel" runat="server" class="question-label">ప్రశ్న ఇక్కడ వస్తుంది</h2>

            <div class="options">
                <asp:Button ID="btnA" runat="server" CssClass="option-button" Text="A" Enabled="false" />
                <asp:Button ID="btnB" runat="server" CssClass="option-button" Text="B" Enabled="false" />
                <asp:Button ID="btnC" runat="server" CssClass="option-button" Text="C" Enabled="false" />
                <asp:Button ID="btnD" runat="server" CssClass="option-button" Text="D" Enabled="false" />
            </div>

            <div class="action-buttons">
                <asp:Button ID="btnReveal" runat="server" Text="✅ Reveal Answer" CssClass="reveal-button" OnClick="btnReveal_Click" />
                <asp:Button ID="btnNext" runat="server" Text="➡️ తదుపరి ప్రశ్న " CssClass="next-button" OnClick="btnNext_Click" />
            </div>
        </div>

        <asp:Panel ID="leaderboardPanel" runat="server" CssClass="leaderboard-panel" Visible="false">
            <div class="leaderboard-header">💯విజేతలు - Leaderboard</div>
            <asp:Repeater ID="rptLeaderboard" runat="server">
                <HeaderTemplate>
                    <div class="leaderboard-row header">
                        <div>ర్యాంక్</div>
                        <div>పేరు</div>
                        <div>సమయం</div>
                        <div>ఫలితం </div>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="leaderboard-row">
                        <div><%# Container.ItemIndex + 1 %></div>
                        <div><%# Eval("AuthorName") %></div>
                        <div><%# Eval("LastCorrectTime", "{0:HH:mm:ss}") %></div>
                        <div><%# Eval("Score") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <div class="quiz-container" style="margin-top: 30px;">
            <h3 style="text-align: center; color: #ffc300;">📊 ఈ ప్రశ్నకి Stats</h3>
            <canvas id="questionStatsChart" style="width: 100%; max-width: 600px; margin: 0 auto;"></canvas>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
        <script>
            function renderChart(total, correct, wrong) {
                const ctx = document.getElementById('questionStatsChart').getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: ['Total Participants', 'Correct', 'Wrong'],
                        datasets: [{
                            label: 'Count',
                            data: [total, correct, wrong],
                            backgroundColor: ['#007bff', '#28a745', '#dc3545']
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: { display: false },
                            tooltip: { enabled: true }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: { precision: 0 }
                            }
                        }
                    }
                });
            }
        </script>
    </form>
</body>
</html>
