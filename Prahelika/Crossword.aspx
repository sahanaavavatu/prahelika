<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Crossword.aspx.cs" Inherits="Prahelika.Crossword" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crossword Puzzle</title>
    <style>
        .crossword-table {
            border-collapse: collapse;
            margin: 20px;
        }
        .crossword-table td {
            width: 30px;
            height: 30px;
            text-align: center;
            border: 1px solid black;
        }
        .crossword-input {
            width: 30px;
            height: 30px;
            text-align: center;
            border: 1px solid black;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Crossword Puzzle</h2>

        <table class="crossword-table">
            <tr>
                <td></td>
                <td><input type="text" class="crossword-input" id="txt1" runat="server" maxlength="1" /></td>
                <td><input type="text" class="crossword-input" id="txt2" runat="server" maxlength="1" /></td>
                <td><input type="text" class="crossword-input" id="txt3" runat="server" maxlength="1" /></td>
                <td><input type="text" class="crossword-input" id="txt4" runat="server" maxlength="1" /></td>
            </tr>
            <tr>
                <td><input type="text" class="crossword-input" id="txt5" runat="server" maxlength="1" /></td>
                <td colspan="4" align="center">20th Century Popular Movie</td>
            </tr>
            <tr>
                <td></td>
                <td><input type="text" class="crossword-input" id="txt6" runat="server" maxlength="1" /></td>
                <td><input type="text" class="crossword-input" id="txt7" runat="server" maxlength="1" /></td>
                <td><input type="text" class="crossword-input" id="txt8" runat="server" maxlength="1" /></td>
                <td><input type="text" class="crossword-input" id="txt9" runat="server" maxlength="1" /></td>
            </tr>
        </table>

        <asp:Button ID="btnCheck" runat="server" Text="Check Answer" OnClick="btnCheck_Click" />
        <asp:Label ID="lblResult" runat="server" ForeColor="Green"></asp:Label>
    </form>
</body>
</html>
