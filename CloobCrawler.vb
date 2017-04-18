Public Class CloobCrawler
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        GraphDb.CreateNode("USER", "CloobID", CloobIDTextBox.Text.ToString)
        GraphDb.SetProperty("USER", "CloobID", CloobIDTextBox.Text.ToString, "URL", URLTextBox.Text.ToString)
        GraphDb.SetProperty("USER", "CloobID", CloobIDTextBox.Text.ToString, "FriendsIndexed", "false")
        MsgBox("User Added !")
        CloobIDTextBox.Clear()
        URLTextBox.Clear()
    End Sub

    Private Sub CloobCrawler_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Browser.ScriptErrorsSuppressed = True
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Crawl()
        statusLbl.Text = ""
    End Sub

    Public Sub FindFriends(ByVal node As String)
        Dim url As String = "http://www.cloob.com/profile/friend/list/username/" + node + "/wrapper/true"
        'Dim Browser As New WebBrowser
        Browser.Navigate(url)
        URLTextBox.Text = url
        While (Browser.ReadyState <> WebBrowserReadyState.Complete)
            My.Application.DoEvents()
        End While
        GraphDb.SetProperty("USER", "CloobID", node, "FriendsIndexed", "true")
        'scroll to end
        Dim lastGuy = Browser.Document.GetElementById("lastElement")
        If lastGuy <> Nothing Then
            lastGuy.ScrollIntoView(True)
        End If
        For Each lnk As HtmlElement In Browser.Document.Links
            If lnk.GetAttribute("href").ToString.ToLower.Contains("/name/") Then
                Dim cloobID As String
                cloobID = lnk.GetAttribute("href")
                cloobID = cloobID.Substring(cloobID.IndexOf("/name/") + 6)
                cloobID = cloobID.Substring(0, cloobID.IndexOf("/"))
                If (GraphDb.GetNodeProperty("USER", "URL", "CloobID", cloobID) = "null") Then
                    GraphDb.CreateNode("USER", "CloobID", cloobID)
                    GraphDb.SetProperty("USER", "CloobID", cloobID, "URL", lnk.GetAttribute("href"))
                    GraphDb.SetProperty("USER", "CloobID", cloobID, "FriendsIndexed", "false")
                    GraphDb.SetProperty("USER", "CloobID", cloobID, "HasProfileInfo", "false")

                End If
                If (node <> cloobID) Then
                    Relation("FriendsWith", "USER", "USER", "CloobID", node, "CloobID", cloobID)
                    OutputTextBox.AppendText(cloobID + " is friends with " + node + vbNewLine)
                End If
            End If
        Next
    End Sub


    Public Sub ProfileInfo(ByVal node As String)
        On Error Resume Next
        Dim repo As String = "D:\CloobRepo\"
        Dim url As String = "http://www.cloob.com/profile/" + node
        'Dim Browser As New WebBrowser
        Browser.Navigate(url)
        URLTextBox.Text = url
        While (Browser.ReadyState <> WebBrowserReadyState.Complete)
            My.Application.DoEvents()
        End While
        'scroll to end
        If DownloadTXT.Text = "true" Then
            For Each img As HtmlElement In Browser.Document.Images
                If img.GetAttribute("src").Contains("/public/user_data/user_photo/") Then
                    GraphDb.SetProperty("USER", "CloobID", node, "ProfilePic", img.GetAttribute("src"))
                    My.Computer.Network.DownloadFile(img.GetAttribute("src"), repo + node + ".jpg")
                    While (My.Computer.FileSystem.FileExists(repo + node + ".jpg") = False)
                        My.Application.DoEvents()
                    End While
                    Exit For
                Else
                    GraphDb.SetProperty("USER", "CloobID", node, "ProfilePic", "")
                End If
            Next
        End If
        Dim Data As String = Browser.Document.GetElementById("userProfile_box_main").InnerText
        GraphDb.SetProperty("USER", "CloobID", node, "Name", subStringer("نام", "شغل", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Job", subStringer("شغل", "درباره من", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "AboutMe", subStringer("درباره من", "وضعیت", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Status", subStringer("وضعیت", "محل سکونت", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Location", subStringer("محل سکونت", "تحصيلات", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "WebSite", subStringer("صفحه وب", "تاریخ عضویت", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "MemberDate", subStringer("تاریخ عضویت", "", Data))
        Data = Browser.Document.GetElementById("userProfile_box_interest").InnerText
        GraphDb.SetProperty("USER", "CloobID", node, "Favorite", subStringer("علایقعلایق", "ورزش", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Sports", subStringer("ورزش", "فعاليتها", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Activity", subStringer("فعاليتها", "کتاب", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Books", subStringer("کتاب", "موسيقي", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Musics", subStringer("موسيقي", "برنامه تلويزيون", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Tv", subStringer("برنامه تلويزيون", "فيلمها", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Movie", subStringer("فيلمها", "غذا", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Food", subStringer("غذا", "", Data))
        Data = Browser.Document.GetElementById("userProfile_box_ability").InnerText
        GraphDb.SetProperty("USER", "CloobID", node, "University", subStringer("دانشگاه", "دبيرستان", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "HighSchool", subStringer("دبيرستان", "رشته", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "Field", subStringer("رشته", "سال فارغ التحصيلي", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "GradYear", subStringer("سال فارغ التحصيلي", "نام محل کار", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "JobTitle", subStringer("عنوان شغلي", "در مورد کار من", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "AboutMyJob", subStringer("در مورد کار من", "ايميل کاري", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "JobEmail", subStringer("ايميل کاري", "وب سايت کاري", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "WorkPhone", subStringer("شماره تلفن کار", "مهارتها", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "WorkFav", subStringer("علايق کاري", "", Data))
        GraphDb.SetProperty("USER", "CloobID", node, "HasProfileInfo", "true")

    End Sub
    Public Sub Crawl()
        ZzTimer.Enabled = False
        Dim node As String = ""
        While (1)
            node = GraphDb.GetNodeProperty("USER", "CloobID", "FriendsIndexed", "false")
            If (node <> "null") Then
                My.Application.DoEvents()
                FindFriends(node)
                ProfileInfo(node)
                OutputTextBox.AppendText("Listing Friends Of " + node + vbNewLine)
            Else
                Exit While
            End If
        End While
        ZzTimer.Enabled = True
    End Sub
    Private Sub CloobCrawler_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ' Crawl()
    End Sub

    Private Sub ZzTimer_Tick(sender As Object, e As EventArgs) Handles ZzTimer.Tick
        If (statusLbl.Text = "") Then
            statusLbl.Text = "System Idle"
        Else
            statusLbl.Text = ""

        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ProfileInfo(CloobIDTextBox.Text)
    End Sub
    Public Function subStringer(ByVal Str1 As String, ByVal Str2 As String, ByVal input As String) As String
        Dim result As String = ""
        If Str2 <> "" Then
            Dim x1 = input.IndexOf(Str1)
            Dim x2 = input.IndexOf(Str2)
            If (x1 = -1 Or x2 = -1) Then
                Return ""
            End If
            result = input.Substring(x1 + Str1.Length, x2 - x1 - Str1.Length)
        Else
            Dim x1 = input.IndexOf(Str1)
            If (x1 = -1) Then
                Return ""
            End If
            result = input.Substring(x1 + Str1.Length)
        End If
        Return result
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        End
    End Sub
End Class
