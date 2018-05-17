Option Explicit On
Option Strict On

Imports BusinessLayer
Imports System.IO
Imports System.Xml.Serialization

Public Class frmSoilICP
  Inherits BasePage

  'Private XmlConfig As LabStationConfigurationShort
  Private strLastFileOpened As String = ""
  Private strSafeFileName As String = ""
  Protected XMLData As DataTable
  Protected DataCompare As DataTable
  Private ProcessedList As List(Of String)
  Private ResultList As List(Of DataLayer.Lab_ICPResult)
  Private HeavyMetals As Boolean
  Private ToleranceLevels As List(Of SoilICPToleranceLevel)
  Private CCVToleranceLevels As List(Of SoilICPToleranceLevel)
  Private ccvSampleOutOfRange As Boolean = False
  Dim ccvMessageBox As frmMessageBox

  ' for Supervisor login  J. Ren 2/22/2012 
  Dim oldUser As User = Nothing

  Public Overrides ReadOnly Property ConfigFileName() As String
    Get
      Return "ICP"
    End Get
  End Property

  Private ReadOnly Property TodaysBackupDirectory() As String
    Get
      If Not Config.Settings.BackupDir.EndsWith("\") Then
        Config.Settings.BackupDir = Config.Settings.BackupDir + "\"
      End If
      Return Config.Settings.BackupDir &
                    Now.Month.ToString & "_" & Now.Day.ToString & "_" & Now.Year.ToString & "\"
    End Get
  End Property

  Private Shared ReadOnly Property testValueErrorCellColor As Color
    Get
      Return Color.Red
    End Get
  End Property
  Private Shared ReadOnly Property testValueWarningCellColor As Color
    Get
      Return Color.Orange
    End Get
  End Property
  Private Shared ReadOnly Property recheckCellColor As Color
    Get
      Return Color.Yellow
    End Get
  End Property
  Private Sub frmSoilICP_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
    If Config.Settings Is Nothing Then
      SetupConfiguration()
    End If
  End Sub

  Private Sub frmSoilICP_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    If Config.Settings Is Nothing Then
      ICPConfigurationToolStripMenuItem_Click(Nothing, Nothing)
    Else
      SetDefaults()
      PurgeOldBackupFiles()
    End If
    UpdateSecurity()
    EnableDisableCompare(False)
    ResizeControls()
    ResultList = New List(Of DataLayer.Lab_ICPResult)
    ToleranceLevels = SoilICP.GetICPTestLimits(Config.StationID)
    CCVToleranceLevels = SoilICP.GetICPCcvLimits()
    createBackupDirectory()
  End Sub

  Private Sub createBackupDirectory()
    If Not Directory.Exists(TodaysBackupDirectory) Then
      Directory.CreateDirectory(TodaysBackupDirectory)
    End If
  End Sub
  Private Sub PurgeOldBackupFiles()
    Try
      ShowWait("Purging old ICP Backup files...")
      'AJK Commented out to prevent unwanted deletes.  Can be readded later.  
      'If Directory.Exists(XmlConfig.ICPConf.BackupDir) And Not XmlConfig.ICPConf Is Nothing AndAlso _
      '   XmlConfig.ICPConf.PurgeOldBackups Then
      '    Dim FileNames As String() = Directory.GetFiles(XmlConfig.ICPConf.BackupDir)
      '    Dim myFileInfo As FileInfo
      '    For Each FileName As String In FileNames
      '        myFileInfo = New FileInfo(FileName)
      '        If FileName.EndsWith(".xml") Or FileName.EndsWith(".ade") Or _
      '            FileName.Contains("ICPDAT") Or FileName.EndsWith(".csv") Then
      '            If (DateTime.Now.Subtract(myFileInfo.LastWriteTime).TotalDays > XmlConfig.ICPConf.NumDaysToSaveBackups) Then
      '                myFileInfo.Delete()
      '            End If
      '        End If
      '    Next
      'End If
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "Purging old backup files.")
    Finally
      ShowDone()
    End Try
  End Sub

  Private Sub SetDefaults()
    nudFiscalYear.Value = App.FiscalYear
    dgvResults.Font = Config.Font.ToFont
    If Config.InstrumentTypeModel = "ARCOS" Then
      chkXMLLoading.Visible = True
      chkXMLLoading.Checked = True
    Else
      chkXMLLoading.Visible = False
      chkXMLLoading.Checked = False
    End If
  End Sub

  Private Sub DoStartXMLListening()
    Try
      If ProcessedList Is Nothing Then ProcessedList = New List(Of String)
      watcher = New FileSystemWatcher(Config.Settings.InputDir, "*.xml")
      watcher.SynchronizingObject = Me
      AddHandler watcher.Changed, AddressOf OnChanged

      watcher.NotifyFilter = (NotifyFilters.LastWrite)
      ' Begin watching.
      watcher.EnableRaisingEvents = chkXMLLoading.Checked
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "DoStartXMLListening.")
    End Try

  End Sub

  Private Sub OnChanged(ByVal sender As System.Object, ByVal e As System.IO.FileSystemEventArgs)
    If e.ChangeType = WatcherChangeTypes.Changed Or e.ChangeType = WatcherChangeTypes.Created Then
      Dim fileReceived As DateTime = DateTime.Now
      While True
        If FileUploadCompleted(e.FullPath) Then
          ReadXMLFile(e.FullPath)
          Exit While
        End If
        ' Calculate the elapsed time and stop if the maximum retry
        ' period has been reached.
        Dim timeElapsed As TimeSpan = DateTime.Now - fileReceived

        If timeElapsed.TotalSeconds > 30 Then 'Try for 30 seconds.
          Exit While
        End If
        System.Threading.Thread.Sleep(1000)  'Wait 1 second
      End While
    End If
  End Sub

  Private Sub ReadXMLFile(ByVal AFile As String)
    Dim xSerializer As XmlSerializer = Nothing
    Dim fData As New Arcos.SampleResults

    Try
      If Config.Settings Is Nothing Then
        SetupConfiguration()
      End If
      strLastFileOpened = AFile
      If XMLData Is Nothing Then
        XMLData = New DataTable
        dgvResults.DataSource = XMLData
      End If
      Using myStreamReader As New StreamReader(AFile)
        xSerializer = New XmlSerializer(GetType(Arcos.SampleResults))
        fData = DirectCast(xSerializer.Deserialize(myStreamReader), Arcos.SampleResults)
        myStreamReader.Close()
      End Using

      Dim SampleName As String = fData.SampleResult.First.Name
      Dim SampleMeasureDateTime As String = fData.SampleResult.First.MeasureDateTime

      If ProcessedList.Contains(SampleName + SampleMeasureDateTime) Then
        Exit Try
      Else
        Dim strCopyLoc As String = TodaysBackupDirectory & SampleName & "_" & getFileTimeName() & ".xml"
        File.Copy(AFile, strCopyLoc, True)
        ProcessedList.Add(SampleName + SampleMeasureDateTime)
      End If
      If Not ThisIsCalibrationData(SampleName) Then
        fData.AddToDataTable(XMLData)
      Else
        fData.AddCalToDataTable(XMLData)
      End If
      UpdateScreen()
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "Read XML File.")
    Finally
      If Not xSerializer Is Nothing Then xSerializer = Nothing
      If Not fData Is Nothing Then fData = Nothing
    End Try
  End Sub

  Private Sub UpdateScreen()
    Try
      If Me.InvokeRequired Then
        Me.Invoke(New MethodInvoker(AddressOf UpdateScreen))
      Else
        dgvResults.DataSource = XMLData
        PrettyUpGrid()
        ValidateResults()
        ValidateLabNumbers() 'This validates check sample also, should be done after validateresults
        dgvResults.Refresh()
      End If

    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "Update Screen.")
    End Try

  End Sub

  Private Shared Function FileUploadCompleted(ByVal filename As String) As Boolean
    ' If the file can be opened for exclusive access it means that the file
    ' is no longer locked by another process.
    Try
      Using inputStream As FileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None)
        Return True
      End Using
    Catch generatedExceptionName As IOException
      Return False
    Catch ex As Exception
      Return False
    End Try
  End Function

  Public Overrides Sub UpdateSecurity()
    MyBase.UpdateSecurity()
    Me.Enabled = App.CurrentUser.HasRightsToPage(Me)
    mnuICPConfig.Enabled = App.CurrentUser.HasRightsToPage(frmICPConfiguration) Or
                               App.CurrentUser.IsSoilAdmin
    mnuOverwriteICPData.Enabled = App.CurrentUser.HasRightsToPage(frmSoilICPReset) Or
                       App.CurrentUser.IsSoilAdmin

  End Sub

  Private Sub frmSoilICP_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
    ResizeControls()
  End Sub

  Private Sub ResizeControls()
    TabControl1.Top = pnlHeader.Bottom
    TabControl1.Width = Me.Width
    TabControl1.Height = Me.Height - pnlHeader.Height - 55
    tbResults.Height = TabControl1.Height - 40
    tbResults.Width = TabControl1.Width - 40
    tbCompare.Height = TabControl1.Height - 40
    tbCompare.Width = TabControl1.Width - 40
    dgvResults.Height = tbResults.Height - 10
    dgvResults.Width = tbResults.Width - 15
    dgvCompare.Height = tbCompare.Height - 10
    dgvCompare.Width = tbCompare.Width - 15
  End Sub

  Private Function LabNumIsValid(ByRef SoilData As SoilsData) As Boolean
    Dim labNumber As String = ""
    Dim blnResult As Boolean = False

    Try
      If (ThisIsCalibrationData(SoilData.LabNumber) Or SoilData.LabNumber.Contains("QC")) Then
        If Not SoilData.LabNumber.Contains("CCV") Then
          SoilData.CheckType = SoilData.LabNumber
          SoilData.CheckData = SoilICP.GetCheckSampleData(SoilData.CheckType)
        End If
        blnResult = True
      Else
        labNumber = Helper.ParseLabNumber(Enums.LabType.Soil, SoilData.LabNumber).FullNumber
        '' Allows duplicate labNumbers to be transfered without throwing process error.
        '' Caused the second labnumber to Not link to a SoilID
        'If SoilData.LabNumber.Contains(labNumber) Then
        '	labNumber = SoilData.LabNumber
        'End If

        SoilData = SoilICP.GetSoilDataFromLabNumber(labNumber, CInt(nudFiscalYear.Value))
        blnResult = SoilData.SoilID > 0
      End If
      Return blnResult

    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "validating the lab number")
    End Try
  End Function

  Private Function ThisIsCalibrationData(ByVal aLabNumber As String) As Boolean
    Dim blnResult As Boolean
    blnResult = aLabNumber = "Standard 1" Or
                                        aLabNumber = "Standard 2" Or
                                        aLabNumber = "Standard 3" Or
                                        aLabNumber = "Low S" Or
                                        aLabNumber = "Blank" Or
                                        aLabNumber = "Std_1" Or
                                        aLabNumber = "Std_2" Or
                                        aLabNumber = "High HM" Or
                                        aLabNumber = "Low HM" 'Or _
    '               aLabNumber = "ICV Low QC" Or _
    '               aLabNumber = "ICV QC" Or _
    '               aLabNumber = "CCV QC" Or _
    '               aLabNumber = "QC"
    Return blnResult
  End Function

  Private Sub PrettyUpGrid()
    For I = 0 To dgvResults.ColumnCount - 1
      If dgvResults.Columns(I).Name <> "Confirm" Then
        dgvResults.Columns(I).ReadOnly = True
      End If
      If dgvResults.Columns(I).Name = "SampleID" Or
                                dgvResults.Columns(I).Name = "BatchID" Or
                                dgvResults.Columns(I).Name = "Unit" Or
                                dgvResults.Columns(I).Name = "Recheck" Or
                                dgvResults.Columns(I).Name = "Dilution" Or
                                dgvResults.Columns(I).Name = "Final Vol." Or
                                dgvResults.Columns(I).Name = "Init. Vol." Then
        dgvResults.Columns(I).Visible = False
      End If
    Next

  End Sub

  Private Sub ValidateResults()

    HeavyMetals = False
    For I = 1 To dgvResults.Columns.Count - 1
      If dgvResults.Columns(I).HeaderText.ToUpper.Equals("PB") Then HeavyMetals = True
      Dim myToleranceLevel As SoilICPToleranceLevel
      Dim ccvToleranceLevel As SoilICPToleranceLevel

      ccvToleranceLevel = FindToleranceLevel(dgvResults.Columns(I).HeaderText, CCVToleranceLevels)
      myToleranceLevel = FindToleranceLevel(dgvResults.Columns(I).HeaderText, ToleranceLevels)

      If Not myToleranceLevel Is Nothing Or Not ccvToleranceLevel Is Nothing Then
        ValidateRowResults(I, myToleranceLevel, ccvToleranceLevel)
      End If
    Next
  End Sub

  Private Sub ValidateLabNumbers()
    Dim SavedList As New List(Of DataGridViewRow)
    Dim SoilData As New SoilsData
    Try
      'Dim adt As DataTable = CType(dgvResults.DataSource, DataTable)
      If dgvResults.Rows.Count > 0 Then
        dgvResults.Columns("LabNumber").ReadOnly = False
        dgvResults.Columns("SampleID").ReadOnly = False
        For I = 0 To dgvResults.Rows.Count - 1
          With dgvResults.Rows(I)
            If .Cells("SampleID").Value.Equals("") Then
              SoilData.LabNumber = CStr(.Cells("LabNumber").Value)
              SoilData.SoilID = 0
              ' Updates SoilData along with returning Boolean
              If LabNumIsValid(SoilData) Then
                .Cells("LabNumber").Value = SoilData.LabNumber
                .Cells("SampleID").Value = SoilData.SoilID
                If Not SoilData.CheckType Is Nothing Then
                  .Cells("Check").Value = SoilData.CheckType
                  ValidateCheck(I, SoilData)
                End If
                If SoilData.SoilID <> 0 Then
                  Dim blnHasTestResults As Boolean = BusinessLayer.SoilICP.HasTestResults(SoilData.SoilID, Enums.LabType.Soil, HeavyMetals)
                  If blnHasTestResults Then
                    Dim blnIsRecheck As Boolean = BusinessLayer.SoilICP.IsRecheckStatus(SoilData.SoilID, HeavyMetals)
                    If blnIsRecheck Then
                      .Cells("Confirm").Value = True
                      .Cells("Recheck").Value = True
                      .Cells("LabNumber").Style.BackColor = recheckCellColor
                      .Cells("LabNumber").ToolTipText = "This is a recheck.  Transfer to LIMS to compare with original values."
                      EnableDisableCompare(True)
                    Else
                      .Cells("Confirm").Value = False
                      .Cells("Recheck").Value = False
                      .Cells("LabNumber").Style.BackColor = Color.Green
                      .Cells("LabNumber").ToolTipText = "Tests have been saved for this lab number"
                      SavedList.Add(dgvResults.Rows(I))
                    End If
                  End If
                End If
              Else
                .Cells("Confirm").Value = False
                .Cells("LabNumber").Style.BackColor = testValueErrorCellColor
                .Cells("LabNumber").ToolTipText = "Invalid Lab Number." & vbCrLf &
                                                                    "Double click to update"
              End If
            End If
          End With
        Next
        dgvResults.Columns("LabNumber").ReadOnly = True
        dgvResults.Columns("SampleID").ReadOnly = True
        If Config.Settings.RemoveSavedRecords Then
          For I = 0 To SavedList.Count - 1
            dgvResults.Rows.Remove(SavedList(I))
          Next
        End If
      End If
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "Validating Lab Numbers")
    Finally
      SoilData = Nothing
      SavedList = Nothing
    End Try

  End Sub

  Private Function CvtToDbl(ByVal aValue As Decimal?) As Double
    If Not aValue Is Nothing Then
      Return CDbl(aValue)
    Else
      Return 0
    End If
  End Function

  Private Sub ValidateCheck(ByVal aRow As Integer, ByVal SoilData As SoilsData)
    Dim UpperTol As Double = 0
    Dim LowerTol As Double = 0
    If SoilData.CheckData Is Nothing Then
      dgvResults.Rows(aRow).DefaultCellStyle.BackColor = dgvResults.DefaultCellStyle.BackColor
      Exit Sub
    End If
    For I = 0 To dgvResults.Columns.Count - 1
      With dgvResults.Rows(aRow).Cells(I)
        If IsNumeric(.Value) Then
          Select Case .OwningColumn.Name
                        'Because of the table structure these are the only results we can validate.
            Case "P"
              UpperTol = CvtToDbl(SoilData.CheckData.PMean) + CvtToDbl(SoilData.CheckData.PVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.PMean) - CvtToDbl(SoilData.CheckData.PVariance)
            Case "K"
              UpperTol = CvtToDbl(SoilData.CheckData.KMean) + CvtToDbl(SoilData.CheckData.KVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.KMean) - CvtToDbl(SoilData.CheckData.KVariance)
            Case "Ca"
              UpperTol = CvtToDbl(SoilData.CheckData.CaMean) + CvtToDbl(SoilData.CheckData.CaVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.CaMean) - CvtToDbl(SoilData.CheckData.CaVariance)
            Case "Mg"
              UpperTol = CvtToDbl(SoilData.CheckData.MgMean) + CvtToDbl(SoilData.CheckData.MgVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.MgMean) - CvtToDbl(SoilData.CheckData.MgVariance)
            Case "S"
              UpperTol = CvtToDbl(SoilData.CheckData.SMean) + CvtToDbl(SoilData.CheckData.SVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.SMean) - CvtToDbl(SoilData.CheckData.SVariance)
            Case "Mn"
              UpperTol = CvtToDbl(SoilData.CheckData.MnMean) + CvtToDbl(SoilData.CheckData.MnVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.MnMean) - CvtToDbl(SoilData.CheckData.MnVariance)
            Case "Zn"
              UpperTol = CvtToDbl(SoilData.CheckData.ZnMean) + CvtToDbl(SoilData.CheckData.ZnVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.ZnMean) - CvtToDbl(SoilData.CheckData.ZnVariance)
            Case "Cu"
              UpperTol = CvtToDbl(SoilData.CheckData.CuMean) + CvtToDbl(SoilData.CheckData.CuVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.CuMean) - CvtToDbl(SoilData.CheckData.CuVariance)
            Case "B"
              UpperTol = CvtToDbl(SoilData.CheckData.BMean) + CvtToDbl(SoilData.CheckData.BVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.BMean) - CvtToDbl(SoilData.CheckData.BVariance)
            Case "Na"
              UpperTol = CvtToDbl(SoilData.CheckData.NaMean) + CvtToDbl(SoilData.CheckData.NaVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.NaMean) - CvtToDbl(SoilData.CheckData.NaVariance)
            Case "Cd"
              UpperTol = CvtToDbl(SoilData.CheckData.CdMean) + CvtToDbl(SoilData.CheckData.CdVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.CdMean) - CvtToDbl(SoilData.CheckData.CdVariance)
            Case "Ni"
              UpperTol = CvtToDbl(SoilData.CheckData.NiMean) + CvtToDbl(SoilData.CheckData.NiVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.NiMean) - CvtToDbl(SoilData.CheckData.NiVariance)
            Case "Pb"
              UpperTol = CvtToDbl(SoilData.CheckData.PbMean) + CvtToDbl(SoilData.CheckData.PbVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.PbMean) - CvtToDbl(SoilData.CheckData.PbVariance)
            Case "Se"
              UpperTol = CvtToDbl(SoilData.CheckData.SeMean) + CvtToDbl(SoilData.CheckData.SeVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.SeMean) - CvtToDbl(SoilData.CheckData.SeVariance)
            Case "Cr"
              UpperTol = CvtToDbl(SoilData.CheckData.CrMean) + CvtToDbl(SoilData.CheckData.CrVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.CrMean) - CvtToDbl(SoilData.CheckData.CrVariance)
            Case "Al"
              UpperTol = CvtToDbl(SoilData.CheckData.AlMean) + CvtToDbl(SoilData.CheckData.AlVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.AlMean) - CvtToDbl(SoilData.CheckData.AlVariance)
            Case "Mo"
              UpperTol = CvtToDbl(SoilData.CheckData.MoMean) + CvtToDbl(SoilData.CheckData.MoVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.MoMean) - CvtToDbl(SoilData.CheckData.MoVariance)
            Case "As"
              UpperTol = CvtToDbl(SoilData.CheckData.AsMean) + CvtToDbl(SoilData.CheckData.AsVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.AsMean) - CvtToDbl(SoilData.CheckData.AsVariance)
            Case "Fe"
              UpperTol = CvtToDbl(SoilData.CheckData.FeMean) + CvtToDbl(SoilData.CheckData.FeVariance)
              LowerTol = CvtToDbl(SoilData.CheckData.FeMean) - CvtToDbl(SoilData.CheckData.FeVariance)
            Case Else
              UpperTol = 999999999
              LowerTol = -999999999
          End Select
          If CDbl(.Value) > UpperTol Or CDbl(.Value) < LowerTol Then
            dgvResults.Rows(aRow).Cells("Confirm").Value = False
            .Style.BackColor = testValueWarningCellColor
          End If
        End If
      End With
    Next
  End Sub

  Private Sub ValidateRowResults(ByVal currentCol As Integer, ByVal SampleTolerance As SoilICPToleranceLevel, ByVal CcvToleranceLevel As SoilICPToleranceLevel)
    dgvResults.Columns(currentCol).ReadOnly = False
    Try
      For Each currentRow As DataGridViewRow In dgvResults.Rows
        Dim currentCell As DataGridViewCell = currentRow.Cells(currentCol)
        Dim labNumber As String = CStr(currentRow.Cells("LabNumber").Value)
        Dim check As String = currentRow.Cells("Check").Value.ToString
        If IsNumeric(currentCell.Value) Then
          If Not ThisIsCalibrationData(labNumber) AndAlso Not labNumber.Contains("QC") AndAlso Not check.Length > 0 Then
            ValidateCell(currentCell, SampleTolerance)
          ElseIf labNumber.Contains("CCV") Then
            ValidateCell(currentCell, CcvToleranceLevel)
          End If
          currentCell.Value = CDbl(FormatNumber(currentCell.Value, Config.Settings.DecPlaces, TriState.True, TriState.True, TriState.UseDefault))
        End If
        ValidateRowConfirmedBox(currentRow, currentCell, labNumber)
      Next
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "ValidateRowResults; Col: " & currentCol.ToString)
    Finally
      dgvResults.Columns(currentCol).ReadOnly = True
    End Try
  End Sub
  Private Sub ValidateRowConfirmedBox(ByVal currentRow As DataGridViewRow, ByVal currentCell As DataGridViewCell, ByVal labNumber As String)
    If labNumber.Contains("CCV") AndAlso currentCell.Style.BackColor = testValueErrorCellColor Then
      currentRow.Cells("Confirm").Value = False
      CheckForCcvErrorMessage(labNumber)
    ElseIf currentCell.Style.BackColor = testValueWarningCellColor Or currentCell.Style.BackColor = testValueErrorCellColor Then
      currentRow.Cells("Confirm").Value = False
      CheckForCcvErrorMessage(labNumber)
    End If
  End Sub
  Private Sub ValidateCell(ByVal currentCell As DataGridViewCell, ByVal toleranceRanges As SoilICPToleranceLevel)
    Dim errorMessage As String = "Value out of acceptable range"
    Dim warningMessage As String = "Warning, value out of normal range."

    toleranceRanges.ErrorLowerLimit = CDbl(FormatNumber(toleranceRanges.ErrorLowerLimit, Config.Settings.DecPlaces,
                                                                         TriState.True, TriState.True, TriState.UseDefault))
    toleranceRanges.ErrorUpperLimit = CDbl(FormatNumber(toleranceRanges.ErrorUpperLimit, Config.Settings.DecPlaces,
                                                                         TriState.True, TriState.True, TriState.UseDefault))
    toleranceRanges.WarningLowerLimit = CDbl(FormatNumber(toleranceRanges.WarningLowerLimit, Config.Settings.DecPlaces,
                                                                         TriState.True, TriState.True, TriState.UseDefault))
    toleranceRanges.WarningUpperLimit = CDbl(FormatNumber(toleranceRanges.WarningUpperLimit, Config.Settings.DecPlaces,
                                                                         TriState.True, TriState.True, TriState.UseDefault))

    Dim cellValue As Double = CDbl(FormatNumber(currentCell.Value, Config.Settings.DecPlaces, TriState.True, TriState.True, TriState.UseDefault))
    If IsValueOutOfRange(cellValue, toleranceRanges.ErrorLowerLimit, toleranceRanges.ErrorUpperLimit) Then
      UpdateOutOfRangeCell(currentCell, errorMessage, testValueErrorCellColor)
    ElseIf IsValueOutOfRange(cellValue, toleranceRanges.WarningLowerLimit, toleranceRanges.WarningUpperLimit) Then
      UpdateOutOfRangeCell(currentCell, warningMessage, testValueWarningCellColor)
    Else
      currentCell.Style.BackColor = currentCell.OwningRow.DefaultCellStyle.BackColor
    End If
  End Sub
  Private Function IsValueOutOfRange(ByVal cellValue As Double, ByVal lowerLimit As Double, ByVal upperLimit As Double) As Boolean
    Dim inLimit As Boolean = False
    If cellValue < lowerLimit Or cellValue > upperLimit Then
      inLimit = True
    End If
    Return inLimit
  End Function
  Private Sub UpdateOutOfRangeCell(ByVal currentCell As DataGridViewCell, ByVal message As String, ByVal cellColor As Color)
    currentCell.Style.BackColor = cellColor
    currentCell.ToolTipText = message
  End Sub
  Private Sub CheckForCcvErrorMessage(ByVal labNumber As String)
    If Not ccvSampleOutOfRange AndAlso labNumber.Contains("CCV") Then
      ccvErrorMessage()
    End If
  End Sub
  Private Sub ccvErrorMessage()
    Try
      Dim errorMessage As String = "One or More CCV Samples is OUT of Error Range"
      Dim MsgBoxTitle As String = "CCV QC Error"

      ccvMessageBox = New frmMessageBox(errorMessage, MsgBoxTitle)
      AddHandler ccvMessageBox.frm_Closing, AddressOf ccvMessageBox_OnClose
      Main.CreateChildForm(ccvMessageBox)
      ccvMessageBox.StartPosition = FormStartPosition.CenterParent
      ccvMessageBox.WindowState = FormWindowState.Normal
      ccvMessageBox.TopMost = True
      ccvMessageBox.Focus()
      ccvSampleOutOfRange = True
      btnSave.Enabled = False
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "loading Soil CCV Error Message")
    End Try
  End Sub

  Private Sub ccvMessageBox_OnClose(ByVal ccvMessageOkClicked As Boolean)
    btnSave.Enabled = True
  End Sub
  Private Function FindToleranceLevel(ByVal Element As String,
                                        ByVal Levels As List(Of SoilICPToleranceLevel)) As SoilICPToleranceLevel
    For I = 0 To Levels.Count - 1
      If Levels(I).Element = Element Then
        Return Levels(I)
        Exit Function
      End If
    Next
    Return Nothing
  End Function

  Private Function GetInitialFileName() As String
    If Config.InstrumentTypeModel = "3300 DV" Then
      Return ""
    ElseIf Config.InstrumentTypeModel = "ARCOS" Then
      Return "Arcos Data.ade"
    ElseIf Config.InstrumentTypeModel = "Thermo 61E" Then
      Return "ICPDAT"
    Else
      Return ""
    End If
  End Function

  Private Sub dgvResults_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvResults.Sorted
    ValidateResults()
    ValidateLabNumbers()
  End Sub

  Private Function DoAddComment(ByVal CurRows As List(Of Integer)) As Boolean
    Dim blnResult As Boolean = False
    Dim lstSoilID As New List(Of Integer)

  Private Sub MarkNoSoil(ByVal aRow As Integer)
    For I = 1 To dgvResults.Columns.Count - 1
      If dgvResults.Columns(I).Visible AndAlso
                dgvResults.Columns(I) IsNot dgvResults.Columns("Check") AndAlso
                dgvResults.Columns(I) IsNot dgvResults.Columns("LabNumber") AndAlso
                dgvResults.Columns(I) IsNot dgvResults.Columns("Pos") Then
        dgvResults.Columns(I).ReadOnly = False
        dgvResults.Rows(aRow).Cells(I).Value = GlobalConstants.Soil_NoSoilIndicator
        dgvResults.Columns(I).ReadOnly = True
      End If
    Next
    ValidateResults()

    ErrorHandling.ProcessError(ex, "DeleteFile")
    End Try
  End Sub

  Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
    Try
      btnSave.Enabled = False
      ShowWait("Transfering data to LIMS")
      If ValidResults() Then
        If BusinessLayer.SoilICP.SaveICPResults(ResultList, HeavyMetals) Then
          dgvResults.Refresh()
          MsgBox("Results Saved")
          If MsgBox("Would you like to reset the screen?", MsgBoxStyle.YesNo, "Reset?") = MsgBoxResult.Yes Then
            btnReset_Click(sender, e)
          End If
        End If
      End If
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "Transfering to LIMS")
    Finally
      btnSave.Enabled = True

      End Function

  Private Function ValidResults() As Boolean
    Dim blnResult As Boolean = True
    HeavyMetals = False
    Try
      If dgvResults.IsCurrentRowDirty Then
        dgvResults.CommitEdit(DataGridViewDataErrorContexts.Commit)

                                              (Not IsNull(dgvResults.Rows(aRow).Cells(aCol).Value) AndAlso
                        CStr(dgvResults.Rows(aRow).Cells(aCol).Value) = GlobalConstants.Soil_NoSoilIndicator)) Then
            Dim Result As New DataLayer.Lab_ICPResult
        Result.BatchID = CInt(dgvResults.Rows(aRow).Cells("BatchID").Value)
        Result.Confirmed = CBool(dgvResults.Rows(aRow).Cells("Confirm").Value)
        Result.InstrumentID = Config.InstrumentID
        Result.IsRecheck = CBool(dgvResults.Rows(aRow).Cells("Recheck").Value)
        Result.LabID = Enums.LabType.Soil
        Result.LabNumber = CStr(dgvResults.Rows(aRow).Cells("LabNumber").Value)
        Result.ModifiedBy = App.CurrentUser.LoginName
        Result.MResult = CStr(dgvResults.Columns(aCol).Name)
        If Result.MResult.ToUpper.Equals("PB") Then HeavyMetals = True
        Result.NoSoil = CBool(CStr(dgvResults.Rows(aRow).Cells(aCol).Value) = GlobalConstants.Soil_NoSoilIndicator)
        If Not IsDBNull(dgvResults.Rows(aRow).Cells("Pos").Value) AndAlso
                        Not dgvResults.Rows(aRow).Cells("Pos").Value Is Nothing Then
          Result.Position = CStr(dgvResults.Rows(aRow).Cells("Pos").Value)
        End If
        If Not Result.NoSoil Then
          Result.Result = CDec(dgvResults.Rows(aRow).Cells(aCol).Value)
        End If
        If dgvResults.Rows(aRow).Cells("SampleID").Value.Equals("") Then
          Result.SampleID = 0
        Else
          Result.SampleID = CInt(dgvResults.Rows(aRow).Cells("SampleID").Value)
        End If
        Result.SavedDate = Now
        Result.UnitOfMeasure = CStr(dgvResults.Rows(aRow).Cells("Unit").Value)  '(dgvResults.Rows(aRow).Cells("LabNumber").Value).Contains("CCV")
        ResultList.Add(Result)
      End If
      Next
      Next
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "Valid Results")
      blnResult = False
    End Try
    Return blnResult

  End Function


  End If
  Next

  If Valuelst.Count <= 0 Then
  Return False
  Else
  Dim strPos As String = ""
  If Not IsDBNull(.Item("Pos")) AndAlso
               Not .Item("Pos") Is Nothing Then
            strPos = CStr(.Item("Pos"))
          End If
          blnResult = BusinessLayer.SoilICP.SaveICPResults(CStr(.Item("SampleID")), Testlst, Valuelst,
                                                                App.CurrentUser.LoginName,
                                                                Enums.LabType.Soil, Config.InstrumentID,
                                                                CStr(.Item("LabNumber")), CInt(.Item("BatchID")),
                                                                CBool(.Item("Confirm")), strPos,
                                                                CStr(.Item("Unit")),
                                                                CBool(.Item("Recheck")),
                                                                App.CurrentUser.LoginName)
          If blnResult And CBool(.Item("Recheck")) Then

  If SoilData.LabNumber.Length > 0 AndAlso LabNumIsValid(SoilData) Then
          dgvResults.Columns("LabNumber").ReadOnly = False
          dgvResults.Columns("SampleID").ReadOnly = False
          dgvResults.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = SoilData.LabNumber
          dgvResults.Rows(e.RowIndex).Cells("SampleID").Value = SoilData.SoilID
          If Not SoilData.CheckType Is Nothing Then
            dgvResults.Rows(e.RowIndex).Cells("Check").Value = SoilData.CheckType
            ValidateCheck(e.RowIndex, SoilData)
          End If
          dgvResults.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.BackColor = Color.White
          dgvResults.Columns("LabNumber").ReadOnly = True
          dgvResults.Columns("SampleID").ReadOnly = True
        End If
      End If
    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "Update lab number")
    Finally
      SoilData = Nothing
    End Try
  End Sub

  Private Sub mnuConfirm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuConfirm.Click
    'For I = 0 To dgvResults.SelectedRows.Count - 1
    '    dgvResults.SelectedRows(I).Cells("Confirm").Value = True
    'Next
    'AJK 3/1/2013 Changed from above because they wanted the grid to select cells instead of rows. 
    For I = 0 To dgvResults.SelectedCells.Count - 1
      dgvResults.Rows(dgvResults.SelectedCells(I).RowIndex).Cells("Confirm").Value = True
    Next
  End Sub

  Private Sub AddCommentToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddCommentToolStripMenuItem.Click
    btnAddComment_Click(sender, e)
  End Sub

  Private Sub MarkAsNoSoilToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MarkAsNoSoilToolStripMenuItem.Click
    btnNoSoil_Click(sender, e)
  End Sub

  Private Sub chkXMLLoading_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkXMLLoading.CheckedChanged
    DoStartXMLListening()
  End Sub

  Private Sub CalibrationsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalibrationsToolStripMenuItem.Click
    Main.CreateDialogForm(frmSoilICPCalibration)
  End Sub

  Private Sub EnableDisableCompare(ByVal Enable As Boolean)
    If Enable Then
      If Not TabControl1.TabPages.Contains(tbCompare) Then
        TabControl1.TabPages.Add(tbCompare)
      End If
      If dgvResults.Columns.Contains("Recheck") Then
        dgvResults.Columns("Recheck").Visible = True
      End If
      If DataCompare Is Nothing Then
        DataCompare = New DataTable
        dgvCompare.DataSource = DataCompare
      End If
    Else
      If TabControl1.TabPages.Contains(tbCompare) Then
        TabControl1.TabPages.Remove(tbCompare)
      End If
      If dgvResults.Columns.Contains("Recheck") Then
        dgvResults.Columns("Recheck").Visible = False
      End If
    End If
  End Sub

  Private Function CompareRow(ByVal RowIndex As Integer) As Boolean
    Dim aDT As DataTable = CType(dgvResults.DataSource, DataTable)
    Dim CompareDT As DataTable = CType(dgvCompare.DataSource, DataTable)
    Try
      If Not CompareDT.Columns.Contains("LabNumber") Then
        CompareDT.Columns.Add("LabNumber")
        CompareDT.Columns.Add("TestType")
      End If
      With aDT.Rows(RowIndex)
        For I = 1 To aDT.Columns.Count - 1
          If aDT.Columns(I).ColumnName <> "Check" AndAlso
              aDT.Columns(I).ColumnName <> "Recheck" AndAlso
              aDT.Columns(I).ColumnName <> "BatchID" AndAlso
              aDT.Columns(I).ColumnName <> "Pos" AndAlso
              aDT.Columns(I).ColumnName <> "SampleID" AndAlso
              aDT.Columns(I).ColumnName <> "Confirm" AndAlso
              aDT.Columns(I).ColumnName <> "Unit" AndAlso
             (IsNumeric(.Item(I)) Or CStr(.Item(I)) = GlobalConstants.Soil_NoSoilIndicator) Then

            If Not CompareDT.Columns.Contains(aDT.Columns(I).ColumnName) Then
              CompareDT.Columns.Add(aDT.Columns(I).ColumnName)
            End If
          End If
        Next
        Dim rows As DataRowCollection = CompareDT.Rows
        Dim newRow As DataRow = rows.Add()
        For I = 1 To aDT.Columns.Count - 1
          If CompareDT.Columns.Contains(aDT.Columns(I).ColumnName) Then
            newRow.Item(aDT.Columns(I).ColumnName) = aDT.Rows(RowIndex).Item(I)
          End If
        Next
        newRow.Item("TestType") = "Recheck"

        Dim OrgValues As List(Of DataLayer.Lab_ICPResult) = SoilICP.GetOriginalResults(CInt(.Item("SampleID")), Enums.LabType.Soil)
        Dim OrgRow As DataRow = rows.Add
        OrgRow.Item("TestType") = "Original"
        OrgRow.Item("LabNumber") = OrgValues(0).LabNumber
        For I = 0 To OrgValues.Count - 1
          If CompareDT.Columns.Contains(OrgValues(I).MResult) Then
            OrgRow.Item(OrgValues(I).MResult) = CDbl(FormatNumber(OrgValues(I).Result, Config.Settings.DecPlaces,
                         TriState.True, TriState.True, TriState.UseDefault))
          End If
        Next

      End With

    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "Compare Row")
    End Try

  End Function

  Private Sub btnSendFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSendFile.Click, mnuCompare.Click
    Main.CreateDialogForm(frmICPExport)
  End Sub

  Private Sub btnSupervisorLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSupervisorLogin.Click
    ' call login sub in BasePage.vb     2/22/2012
    Call supervisorLogin(btnSupervisorLogin.Text, oldUser)

  End Sub

  Private Sub mnuOverwriteICPData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuOverwriteICPData.Click
    Main.CreateDialogForm(frmSoilICPReset)
  End Sub

End Class
