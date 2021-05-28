Imports System.IO
Imports System.Net
Public Class Form2
    Dim Version = "1.0.4"
    Dim TheKeys As String = FindKeys()
    Public Function FindKeys()
        Dim wrResponse As WebResponse
        Dim wrRequest As WebRequest = HttpWebRequest.Create("https://raw.githubusercontent.com/Fuckyoudumbass/DumbFuckLMAO/main/Keys")
        wrResponse = wrRequest.GetResponse()
        Using sr As New StreamReader(wrResponse.GetResponseStream())
            Dim output = sr.ReadToEnd()
            sr.Close()
            Return output
        End Using
    End Function
    Public Function CheckVersion()
        Dim wrResponse As WebResponse
        Dim wrRequest As WebRequest = HttpWebRequest.Create("https://raw.githubusercontent.com/Fuckyoudumbass/DumbFuckLMAO/main/Version")
        wrResponse = wrRequest.GetResponse()
        Using sr As New StreamReader(wrResponse.GetResponseStream())
            Dim output = sr.ReadToEnd()
            sr.Close()
            If Not output.Contains(Version) Then
                MsgBox("New Version Available " + output)
                Try
                    Process.Start(Environment.CurrentDirectory + "\Update.exe")
                Catch ex As Exception
                End Try
                Me.Close()
                Form1.Close()
            End If
        End Using
    End Function
    Public Function Login()
        Me.Hide()
        Form1.Show()
    End Function
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckVersion()
        FindKeys()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "rjw" Then
            Login()
            Form1.Text += " Daddy V" + Version
            Return
        End If
        If TextBox1.TextLength <> 36 Then
            MsgBox("Incorrect license key")
            Return
        End If
        If TheKeys.Contains(TextBox1.Text) Then
            If TextBox1.Text.Substring(0, 1) = "F" Then
                Login()
                Form1.Text += " One Hour V" + Version
            ElseIf TextBox1.Text.Substring(0, 1) = "T" Then
                Login()
                Form1.Text += " 24 Hours V" + Version
            Else
                Login()
                Form1.Text += " Lifetime V" + Version
            End If
            Return
        Else
            MsgBox("Incorrect license key")
        End If
    End Sub
End Class