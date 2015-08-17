Imports System.ServiceModel
Imports System.Diagnostics
Imports System.Configuration
Imports System.Timers
Imports System.Globalization
Imports System.ServiceProcess

Public Class WcfServiceDemo

#Region " Declare constants and variables "
    Private CheckTime As String
    Private TimeStop As String
    Private CheckRam As String
    Private hh, mm, ss As String
    Private aTimer As Timer
    Private mv_threadProcess As System.Threading.Thread = Nothing
    Private mv_TimeSleep As Double
    Private mv_TimeSleep_Emty As Double
    Private str_TimeSleep As String
    Private str_TimeSleep_Emty As String
    Private mv_RamMax As Double
    Private str_RamMax As String
    Private parts As String()
    Private RunTime As TimeSpan
    Private Process_sv As Process
    Private mv_Flag As Boolean

#End Region

#Region "Overrides Services"
    Protected Overrides Sub OnStart(ByVal args() As String)
        log4net.Config.XmlConfigurator.Configure()
        RedConfig()
        mv_Flag = True
        StartProcess()
    End Sub

    Protected Overrides Sub OnStop()
        mv_Flag = False
        StopProcess()
    End Sub
#End Region

#Region "Services Sub"
    Private Sub StartProcess()
        Try
            ''Do Work blabla :)

            If CheckTime = "1" And CheckRam = "1" Then
                LogError.log.Info("Mode Service theo ca ram va thoi gian: " & CheckRam & ":" & CheckTime)
                Time_Process()
                Thread_Process()
            Else
                If CheckTime = "1" Then
                    LogError.log.Info("Mode Service theo thoi gian: " & CheckTime)
                    Time_Process()
                End If
                If CheckRam = "1" Then
                    LogError.log.Info("Mode Service theo ram: " & CheckRam)
                    Thread_Process()
                End If
            End If
            LogError.log.Info("Khoi tao Service Demo thanh cong ...." & DateTime.Now.ToString("HH:mm"))
        Catch ex As Exception
            LogError.log.Error("Loi Khoi tao Service Demo ...." & ex.Message)
        End Try
    End Sub

    Private Sub StopProcess()
        Try
            LogError.log.Info("Service Demo Stop")
            Try
                ''Do Work blabla :)
                If mv_threadProcess.IsAlive Then
                    mv_threadProcess.Join(TimeSpan.FromSeconds(20))
                    If (mv_threadProcess.ThreadState And System.Threading.ThreadState.Running) = System.Threading.ThreadState.Running Then
                        mv_threadProcess.Abort()
                    End If
                End If
                aTimer.[Stop]()
                LogError.log.Info("Service Demo OnStop thanh cong ...." & DateTime.Now.ToString("HH:mm"))
            Catch ex As Exception
                LogError.log.Error("Service Demo_OnStoped" & ex.Message)
            End Try
        Catch ex As Exception
            LogError.log.Error("Service Demo_OnStoped" & ex.Message)
        End Try
    End Sub

    Private Sub RestartServices()
        Try
            Process_sv = New Process()
            Process_sv.StartInfo.FileName = "cmd"
            Process_sv.StartInfo.Arguments = "/c net stop ""ServiceDemo"" & net start ""ServiceDemo"""
            Process_sv.Start()
            LogError.log.Info("Restart Services ........" & DateTime.Now.ToString("HH:mm"))
        Catch ex As Exception
            LogError.log.Error(ex.Message)
        End Try
    End Sub

#End Region

#Region "Redconfig Sub And Deafault Var"
    Private Sub RedConfig()
        Try
            Dim flag As Boolean
            CheckTime = ConfigurationManager.AppSettings("CheckTime")
            TimeStop = ConfigurationManager.AppSettings("TimeStop")
            CheckRam = ConfigurationManager.AppSettings("CheckRam")
            str_RamMax = ConfigurationManager.AppSettings("RamMax")
            str_TimeSleep = ConfigurationManager.AppSettings("TimeSleep")
            str_TimeSleep_Emty = ConfigurationManager.AppSettings("TimeSleep_Emty")

            mv_RamMax = Double.Parse(str_RamMax)
            mv_TimeSleep = Double.Parse(str_TimeSleep)
            mv_TimeSleep_Emty = Double.Parse(str_TimeSleep_Emty)
            flag = True

            If CheckTime <> "0" And CheckTime <> "1" And CheckRam <> "0" And CheckRam <> "1" Then
                CheckTime = "1"
                CheckRam = "1"
                LogError.log.Error("Loi config ram va thoi gian, lay gia tri mac dinh la: " & CheckTime & ":" & CheckRam)
            Else
                If CheckRam <> "0" And CheckRam <> "1" Then
                    CheckRam = "1"
                    LogError.log.Error("Loi config mode theo ram lay gia tri mac dinh la: " & CheckRam)
                    flag = False
                End If
                If CheckTime <> "0" And CheckTime <> "1" And flag = False Then
                    CheckTime = "1"
                    LogError.log.Error("Loi config mode theo thoi gian lay mac dinh: " & CheckTime)
                End If
            End If

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
            Dim check_decimal As Double
            If Double.TryParse(str_RamMax, check_decimal) Then
                mv_RamMax = check_decimal
            Else
                mv_RamMax = 1024
                LogError.log.Info("Ram mac dinh do cau hinh sai:" & ":" & mv_RamMax)
            End If

            Dim dec_TimeSleep As Double
            If Double.TryParse(str_TimeSleep, dec_TimeSleep) Then
                mv_TimeSleep = dec_TimeSleep
            Else
                mv_TimeSleep = 10
                LogError.log.Info("Thoi gian ngu mac dinh do cau hinh sai:" & ":" & mv_TimeSleep)
            End If

            Dim dec_TimeSleep_Emty As Double
            If Double.TryParse(str_TimeSleep_Emty, dec_TimeSleep_Emty) Then
                mv_TimeSleep_Emty = dec_TimeSleep_Emty
            Else
                mv_TimeSleep_Emty = 10
                LogError.log.Info("Thoi gian ngu khi khong co DL mac dinh do cau hinh sai:" & ":" & mv_TimeSleep_Emty)
            End If
        Catch ex As Exception
            LogError.log.Error("Loi File Config" & ex.Message)
        End Try
    End Sub

    Public Function ValidateTime(time As String) As Boolean
        Dim ignored As DateTime
        Return DateTime.TryParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, ignored)
    End Function

#End Region

#Region "Check Ram"
    Private Sub Thread_Ram()
        Try
            Dim f As Double = 1024.0
            mv_TimeSleep = mv_TimeSleep * mv_TimeSleep_Emty
            While (mv_Flag)
                Try
                    Dim prWorkingSet As Double = 0
                    Dim prcName As String = Process.GetCurrentProcess().ProcessName
                    Dim counter = New PerformanceCounter("Process", "Working Set - Private", prcName)
                    prWorkingSet = Math.Round(((counter.RawValue / f) / f), 2)
                    If prWorkingSet >= mv_RamMax Then
                        LogError.log.Error("Khoi dong lai Services..." & DateTime.Now.ToString("HH:mm") & ":" & _
                        "do muc su dung Ram hien tai : " & prWorkingSet & " vuot qua han muc Ram cau hinh: " & mv_RamMax)
                        RestartServices()
                    End If
                    Threading.Thread.Sleep(mv_TimeSleep)
                Catch ex As Exception
                    LogError.log.Error(ex.Message)
                    Threading.Thread.Sleep(mv_TimeSleep)
                End Try
            End While
        Catch ex As Exception
            LogError.log.Error(ex.Message)
        End Try
    End Sub

    Private Sub Thread_Process()
        Try
            If mv_threadProcess IsNot Nothing Then
                mv_threadProcess.Abort()
            End If
            LogError.log.Info("Thread....")
            mv_threadProcess = New System.Threading.Thread(AddressOf Thread_Ram)
            mv_threadProcess.IsBackground = True
            mv_threadProcess.Start()
            LogError.log.Info(" Start Thread Successful....")
        Catch ex As Exception
            LogError.log.Error(ex.Message)
        End Try
    End Sub

#End Region

#Region "Check Time"
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

    Private Sub OnTimedEvent(source As Object, e As ElapsedEventArgs)
        Try
            aTimer.[Stop]()
            LogError.log.Info("Khoi dong lai Services luc:" & DateTime.Now.ToString("HH:mm") & _
            "do cau hinh khoi dong lai luc: " & hh & ":" & mm & ":" & ss)
            RestartServices()
        Catch ex As Exception
            LogError.log.Error(ex.Message)
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

#End Region

End Class
