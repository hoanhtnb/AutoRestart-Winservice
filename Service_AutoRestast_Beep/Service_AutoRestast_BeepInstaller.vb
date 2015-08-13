Imports System.ComponentModel
Imports System.Configuration.Install

<RunInstaller(True)> Public Class Service_AutoRestast_BeepInstaller
    Inherits System.Configuration.Install.Installer

#Region " Component Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Installer overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer
    Friend WithEvents ServiceInstaller As System.ServiceProcess.ServiceInstaller
    Friend WithEvents ServiceProcessInstaller As System.ServiceProcess.ServiceProcessInstaller

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
        Me.ServiceInstaller = New System.ServiceProcess.ServiceInstaller
        Me.ServiceProcessInstaller = New System.ServiceProcess.ServiceProcessInstaller
        '
        'ServiceInstaller
        '
        Me.ServiceInstaller.DisplayName = "Service_AutoRestast_Beep"
        Me.ServiceInstaller.ServiceName = "Service_AutoRestast_Beep"
        '
        'ServiceProcessInstaller
        '
        Me.ServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem
        Me.ServiceProcessInstaller.Password = Nothing
        Me.ServiceProcessInstaller.Username = Nothing
        '
        'Service_AutoRestast_BeepInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.ServiceInstaller, Me.ServiceProcessInstaller})
    End Sub

#End Region

End Class
