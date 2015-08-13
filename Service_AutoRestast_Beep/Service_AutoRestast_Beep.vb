Imports System.Timers
Imports System.Configuration
Imports System.Globalization
Imports System.Net

Public Class Service_AutoRestast_Beep
    Private serviceMode As String = ConfigurationManager.AppSettings("Mode")
    Private Url As String = ConfigurationManager.AppSettings("Url")
    Private TimeStop As String = ConfigurationManager.AppSettings("TimeStop")
    Private mv_TIMESLEEP As Decimal = CDec(System.Configuration.ConfigurationSettings.AppSettings("TIMESLEEP"))
    Private hh, mm, ss As String
    Private aTimer As Timer
    Private parts As String()
    Private RunTime As TimeSpan
    Private Process_sv As Process
    Private mv_threadProcess As System.Threading.Thread = Nothing
    Private mv_Flag As Boolean
    Public Declare Function Beep Lib "kernel32" Alias "Beep" _
    (ByVal dwFreq As Long, ByVal dwDuration As Long) As Long

    Protected Overrides Sub OnStart(ByVal args() As String)
        mv_Flag = True
        log4net.Config.XmlConfigurator.Configure()
        StartProcess()
    End Sub

    Protected Overrides Sub OnStop()
        mv_Flag = False
        StopProcess()
    End Sub

    Private Sub StartProcess()
        Try
            If ValidateTime(TimeStop) = True Then
                parts = Strings.Split(TimeStop.Trim, ":")
                hh = parts(0)
                mm = parts(1)
                ss = parts(2)
            Else
                hh = "02"
                mm = "10"
                ss = "00"
                LogError.log.Info("Time mac dinh do cau hinh sai:" & hh & ":" & mm & ":" & ss)
            End If
            mv_threadProcess = New System.Threading.Thread(AddressOf Prc_Tprocess)
            mv_threadProcess.IsBackground = True
            mv_threadProcess.Start()
            If serviceMode = "DAY_STOP" Then
                Time_Process()
            Else
                Time_Process()
            End If
            LogError.log.Info("Khoi tao Service_AutoRestast_Beep thanh cong ...." & DateTime.Now.ToString("HH:mm"))
        Catch ex As Exception
            LogError.log.Error("Loi Khoi tao Service_AutoRestast_Beep ...." & ex.Message)
        End Try
    End Sub

    Private Sub StopProcess()
        Try
            LogError.log.Info("Service_AutoRestast_Beep Stop")
            Try
                ''Bla bla
                ''Bla bla
                ''Bla bla
                aTimer.[Stop]()
                LogError.log.Info("Service_AutoRestast_Beep OnStop thanh cong ...." & DateTime.Now.ToString("HH:mm"))
            Catch ex As Exception
                LogError.log.Error("Service_AutoRestast_Beep_OnStoped" & ex.Message)
            End Try
        Catch ex As Exception
            LogError.log.Error("Service_AutoRestast_Beep_OnStoped" & ex.Message)
        End Try
    End Sub

    Private Sub Time_Process()
        Try
            RunTime = New TimeSpan(hh, mm, ss)
            Dim _Interval As Double = CDbl(Check_TargetTime(RunTime))
            aTimer = New System.Timers.Timer(_Interval)
            AddHandler aTimer.Elapsed, AddressOf OnTimedEvent
            aTimer.Enabled = True
        Catch ex As Exception
            LogError.log.Error("Loi ...." & ex.Message)
        End Try
    End Sub

    Private Function Check_TargetTime(targetTime As TimeSpan) As Double
        Try
            Dim dt As DateTime = DateTime.Today.Add(targetTime)
            If DateTime.Now > dt Then
                dt = dt.AddDays(1)
            End If
            Return dt.Subtract(DateTime.Now).TotalMilliseconds
        Catch ex As Exception
            LogError.log.Error(ex.Message)
            Return 20000 'Restart sau 2s neu Ex
        End Try
    End Function

    Private Sub OnTimedEvent(source As Object, e As ElapsedEventArgs)
        Try
            aTimer.[Stop]()
            LogError.log.Info("Stop by Time........")
            Stop_All()
            'aTimer.Start()
        Catch ex As Exception
            LogError.log.Error(ex.Message)
        End Try
    End Sub

    Private Sub Stop_All()
        Try
            Process_sv = New Process()
            Process_sv.StartInfo.FileName = "cmd"
            Process_sv.StartInfo.Arguments = "/c net stop ""Service_AutoRestast_Beep"" & net start ""Service_AutoRestast_Beep"""
            Process_sv.Start()
        Catch ex As Exception
            LogError.log.Error(ex.Message)
        End Try
    End Sub

    Public Function ValidateTime(time As String) As Boolean
        Dim ignored As DateTime
        Return DateTime.TryParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, ignored)
    End Function

    Private Function CheckForInternetConnection() As Boolean
        Try
            Using client = New WebClient()
                Using stream = client.OpenRead(Url)
                    Return True
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Sub DataProcess()
        Try
            Using client = New WebClient()
                Using stream = client.OpenRead(Url)
                    'bla bla
                End Using
            End Using
        Catch ex As Exception
            LogError.log.Error("DataProcess" & ex.Message)
            Beep(9999, 500)
        End Try
    End Sub

    Private Sub Prc_Tprocess()
        Try
            While (mv_Flag)
                Try
                    DataProcess()
                    Threading.Thread.Sleep(mv_TIMESLEEP)
                Catch ex As Exception
                    LogError.log.Error("Prc_Tprocess" & ex.Message)
                    Threading.Thread.Sleep(mv_TIMESLEEP)
                End Try
            End While
        Catch ex As Exception
            LogError.log.Error("Prc_Tprocess" & ex.Message)
        End Try
    End Sub

End Class
