<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SitecoreBulkItemUpdate.aspx.cs" Inherits="Sitecore.BulkItemUpdate.SitecoreBulkItemUpdate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sitecore bulk item update</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.2.0/dist/js/bootstrap.bundle.min.js"></script>
</head>
<body>
    <form id="form1" runat="server" class="form">
        <div class="mb-3">
            <label for="TextBox2" class="form-label">NUmberOfItemsToInsert</label>
            <asp:TextBox ID="TextBox2" TextMode="Number" runat="server" class="form-control" Text="1"></asp:TextBox>
        </div>
        <div class="mb-3">
            <label for="TextBox3" class="form-label">Number of batch items per page</label>
            <asp:TextBox ID="TextBox3" TextMode="Number" runat="server" class="form-control" Text="15"></asp:TextBox>
        </div>
        <div class="mb-3">
            <label for="TextBox1" class="form-label">Report</label>
            <asp:TextBox ID="TextBox1" TextMode="multiline" row="20" runat="server" class="form-control"></asp:TextBox>
        </div>
        <div class="mb-3">
            <asp:Button ID="ReadItemRecursively" runat="server" Text="ReadItemRecursively" class="form-control btn btn-primary" OnClick="ReadItemRecursively_Click" />
            <asp:Button ID="BulkImport" runat="server" Text="BulkImport" class="form-control btn btn-primary" OnClick="BulkImport_Click" />
        </div>
    </form>
</body>

</html>
