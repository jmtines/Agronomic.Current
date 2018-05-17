
Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports System.ComponentModel
Imports SharedLayer

Public Class BasePage
  Inherits System.Windows.Forms.Form

  Private Delegate Sub UpdateInstrumentStatusDelegate(ByVal InstrumentStatus As Enums.InstrumentStatus)
  Private Delegate Sub UpdateNetworkStatusDelegate()
  Public Delegate Sub OnSecurityChangedDelegate()
  Public Delegate Sub OnScreenBackColorChangedDelegate()

  ' This event calls all forms to let them know that a user logged out 
  ' Mainly used by forms that are not active data forms (forms that are main menus)
  ' Active data forms can override the Public Overrides Sub LogOut() subroutine
  Public Shared Event OnSecurityChanged As OnSecurityChangedDelegate
  Public Shared Event OnScreenBackColorChanged As OnScreenBackColorChangedDelegate

  Public _UnsavedChanges As Boolean
  'AJK 3/10/11  Removed.  Now using App.CurrentUser.Is___Admin
  'Public SupervisorLoggedIn As Boolean
  Public FormIsClosing As Boolean
  'Public Shared StationConfigurationShort As LabStationConfigurationShort
  'Public Shared StationConfigurationLong As LabStationConfigurationLong
  Public Shared Config As PageConfig

  Private Sub OnCloseChild() Handles Me.FormClosed
    For Each ChildForm As Form In Main.MdiChildren
      If ChildForm.TopMost Then
        ChildForm.Activate()
      End If
    Next
  End Sub

  Public Property UnsavedChanges() As Boolean
    Get
      Return _UnsavedChanges
    End Get
    Set(ByVal value As Boolean)
      _UnsavedChanges = value
    End Set
  End Property

  Public Sub New()
    InitializeComponent()
  End Sub

  Public Overridable Sub SetWindowState()
    Me.WindowState = FormWindowState.Maximized
  End Sub

  Private Sub BasePage_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
    SetWindowState()
  End Sub

  Private Sub BaseForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
    '***************
    ' IMPORTANT: load events of base forms are also run at design-time, sometimes causing errors (not being
    '            able to open forms in Visual Studio).  So only execute this code at run-time.  You can
    '            use the boolean "DesignMode" property to check for being in design-mode.
    '***************
    Try
      If DesignMode Then
        Exit Sub
      End If
      SetupConfiguration()
      SetNetworkStatus()
      'SetInstrumentStatus(Enums.InstrumentStatus.None)
      SetStatus("Ready")
      UpdateScreenBackColor()
      If Not Config Is Nothing Then
        Me.Font = Config.Font.ToFont
      End If
      AddHandler My.Computer.Network.NetworkAvailabilityChanged, AddressOf MyComputerNetwork_NetworkAvailabilityChanged
    Catch ex As Exception
      ErrorHandling.LogError("BasePage, BaseForm_load, Exception = " & ex.Message)
    End Try
  End Sub

  Public Sub MyComputerNetwork_NetworkAvailabilityChanged(ByVal sender As Object, ByVal e As Devices.NetworkAvailableEventArgs)
    SetNetworkStatus()
  End Sub

  Public Sub SetNetworkStatus()
    If Me.InvokeRequired Then
      Me.Invoke(New UpdateNetworkStatusDelegate(AddressOf Me.SetNetworkStatus))
      Exit Sub
    Else
      If My.Computer.Network.IsAvailable Then
        Main.lblNetworkStatus.Image = My.Resources.GreenLed
      Else
        Main.lblNetworkStatus.Image = My.Resources.RedLed
      End If
      Main.StatusStrip.Refresh()
    End If
  End Sub

  Public Sub SetInstrumentStatus(ByVal InstrumentStatus As Enums.InstrumentStatus)
    If Me.InvokeRequired Then
      Me.Invoke(New UpdateInstrumentStatusDelegate(AddressOf Me.SetInstrumentStatus), InstrumentStatus)
      Exit Sub
    Else
      If InstrumentStatus = Enums.InstrumentStatus.None Then
        Main.lblInstrumentStatus.Image = My.Resources.GreyLed
      ElseIf InstrumentStatus = Enums.InstrumentStatus.Good Then
        Main.lblInstrumentStatus.Image = My.Resources.GreenLed
      Else
        Main.lblInstrumentStatus.Image = My.Resources.RedLed
      End If
      Main.StatusStrip.Refresh()
    End If
  End Sub

  Public Sub SetStatus(ByVal strStatus As String)
    Main.lblStatus.Text = strStatus
    Main.StatusStrip.Refresh()
  End Sub

  Public Sub SetCalibrationStatus(ByVal Status As String)
    Main.lblCalibrationStatus.Text = Status
  End Sub

  Public Sub ResetCalibrationStatus()
    Main.lblCalibrationStatus.Text = "Calibration not required"
  End Sub

  ' I originally had this declared as MustOverride (abstract method)
  ' But then this class had to be marked abstract (MustInherit)
  ' so then the Visual Studio 2008 designer was throwing errors:
  ' The designer must create an instance of type 'agroLIMS.Windows.Common.BaseForm' but it cannot because the type is declared as abstract. 
  Public Overridable Sub UpdateScreenBackColor() Handles Me.OnScreenBackColorChanged
    ' Windows Forms that inherit this BaseForm should override this function and update the back colors of their screens
    SetStyle(ControlStyles.SupportsTransparentBackColor, True)
    If Not App.ScreenBackColor.IsEmpty Then
      Me.BackColor = App.ScreenBackColor
    Else
      Me.BackColor = AppColors.ActionButtonsPanelDefaultBackColor
    End If

    If Me.FormBorderStyle <> FormBorderStyle.Sizable AndAlso Me.FormBorderStyle <> FormBorderStyle.SizableToolWindow Then
      Main.StatusStrip.SizingGrip = False
    End If
  End Sub

  Public Overridable Sub UpdateSecurity() Handles Me.OnSecurityChanged
    Try
      SetUserName()
    Catch ex As Exception
      ErrorHandling.LogError("BasePage, UpdateSecurity(), Exception = " & ex.Message & ControlChars.CrLf & ex.StackTrace)
    End Try
  End Sub


#Region "Helper Methods"

  Public Overridable Sub ShowCheckSampleViewer()
    Dim strWebLink As String
    Try
      If System.Diagnostics.Debugger.IsAttached Then
        strWebLink = My.Settings.CheckSampleViewerTest
      Else
        strWebLink = My.Settings.CheckSampleViewer
      End If
      Process.Start("iexplore.exe", "-new " + strWebLink)
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "showing check sample pass/fail")
    End Try
  End Sub

  Public Overridable Sub ShowWait(ByVal statusMessage As String)
    DisableForm()
    Me.Cursor = Cursors.WaitCursor
    SetStatus(statusMessage)
    App.ScreenIsReady = False
  End Sub

  Public Overridable Sub ShowDone()
    EnableForm()
    Me.Cursor = Cursors.Default
    SetStatus("Ready")
    App.ScreenIsReady = True
  End Sub

  Public Overridable Sub DisableForm()
  End Sub

  Public Overridable Sub EnableForm()
  End Sub
#End Region

  Public Sub SetUserName()
    Try
      If (App.CurrentUser Is Nothing) Then
        Main.lblUser.Text = "Please log in"
      Else
        Main.lblUser.Text = "User:  " + App.CurrentUser.LoginName
      End If
    Catch ex As Exception
      ErrorHandling.LogError("BasePage, SetUserName(), Exception = " & ex.Message & ControlChars.CrLf & ex.StackTrace)
    End Try
  End Sub

  Public Function FormAlreadyExists(ByVal aForm As Form) As Boolean
    Dim blnResult As Boolean = False
    ' Close all child forms of the parent.
    For Each ChildForm As Form In Main.MdiChildren
      If ChildForm.Name = aForm.Name Then
        blnResult = True
      End If
    Next
    Return blnResult
  End Function

  Public Function DoLogin() As Boolean
    Try
      Using myfrmLogin As New frmLogin
        If myfrmLogin.ShowDialog() = System.Windows.Forms.DialogResult.Yes Then
          App.CurrentUser = myfrmLogin.LoginResult.ValidatedUser
        Else
          Return False
          Exit Function
        End If
      End Using
      RaiseEvent OnSecurityChanged()
      Return True
    Catch ex As Exception
      ErrorHandling.LogError("BasePage, DoLogin(), Exception = " & ex.Message & ControlChars.CrLf & ex.StackTrace)
    End Try
  End Function

  Public Sub DoUserOptions()
    Using myfrmUserOptions As New frmUserOptions
      If myfrmUserOptions.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
        RaiseEvent OnScreenBackColorChanged()
      End If
    End Using
  End Sub

  Public Sub SetupConfiguration()
    'If Not LabStations.VerifyLocalConfigurationFile() Then
    '    Using myfrmStationConfiguration As New frmStationConfiguration
    '        myfrmStationConfiguration.ShowDialog()
    '    End Using
    'End If
    '' LabStationConfigurations are stored as "Shared" in BaseForm
    'StationConfigurationShort = LabStations.ReadLocalLabConfigurationFile()
    'StationConfigurationLong = New LabStationConfigurationLong(StationConfigurationShort)

    If ConfigFileName <> "" Then
      If Not PageConfiguration.ConfigFileExists(ConfigFileName) AndAlso
              Me.Name <> "frmPageConfiguration" Then
        Using frm As New frmPageConfiguration
          If frm.DoConfig(ConfigFileName) Then
            Config = PageConfiguration.ReadFile(ConfigFileName)
          Else
            '??
          End If
        End Using
      Else
        If Me.Name <> "Main" Then
          Config = PageConfiguration.ReadFile(ConfigFileName)
        End If
      End If
    End If
  End Sub

  Public Overridable ReadOnly Property ConfigFileName() As String
    Get
      Return "LimsConfigurationData"
    End Get
  End Property

  ' for temporarily allowing technician to update data. J. Ren   2/27/2012
  Sub supervisorLogin(ByRef btnText As String, ByRef theUser As User)

    'Determine if this is a supervisor login or logout
    Select Case btnText.Trim.ToUpper

      Case "SUPERVISOR LOGIN"

        'Save original user info
        theUser = App.CurrentUser

        'Attempt login as a supervisor
        If DoLogin() Then
          'Sucessful login
          If App.CurrentUser.IsSoilAdmin Then
            'Logged in user IS a soil administrator 
            'Note: LIMS administrators are by default soil administrators
            btnText = "Supervisor Logout"
            MsgBox("Your are in overwrite mode. Click Supervisor Logout to change back. ")
          Else
            'Logged in user is NOT a supervisor, reset to original user
            Beep()
            MsgBox("You did not supply supervisor login credentials.")
            App.CurrentUser = theUser
          End If
        Else
          'Unsuccessful login or cancel button was clicked; therefore, do nothing
        End If

      Case "SUPERVISOR LOGOUT"
        'Logout as supervisor and reset to original user
        App.CurrentUser = theUser
        btnText = "Supervisor Login"
        MsgBox("Your are in read-only mode. Thank you!")

    End Select

    ' Update Status Strip 
    Main.StatusStrip.Items(2).Text = "User: " & App.CurrentUser.LoginName

  End Sub

  Private Sub InitializeComponent()
    Me.SuspendLayout()
    '
    'BasePage
    '
    Me.ClientSize = New System.Drawing.Size(284, 262)
    Me.Name = "BasePage"
    Me.Text = "BasePage"
    Me.ResumeLayout(False)

  End Sub
End Class