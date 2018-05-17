Imports Common
Imports DataLayer
Imports System.Transactions

Public Class Soil

  Public Shared Function GetSoilTestResultsForSample(ByVal SoilID As Integer) As List(Of DataLayer.uspSoil_GetCalcResultsResult)
    '-- Get soil test results
    Using db As New AppDataContext
      Dim retCode As Integer
      Dim retMsg As String = ""
      'AJK moved to data context create.  8/10/12 db.CommandTimeout = 120  'AJk 3/12/2012 This fixes timeout issue, but stored proc should be reviewed for performance.
      Return db.uspSoil_GetCalcResults(SoilID, retCode, retMsg).ToList
    End Using
  End Function

  Public Shared Function GetDiagDataEntryReport(ByVal ReportID As Integer) As DiagDataEntryReport
    Dim result As New DiagDataEntryReport

    Using db As New AppDataContext()
      '=== Get plant symptoms ===
      Dim symp = (From s In db.SoilReportPlantSymptoms
                  Where s.ReportID = ReportID
                  Select s).SingleOrDefault()

      If Not symp Is Nothing Then
        With result
          .Growth_General = symp.GeneralGrowth
          .Growth_Specific = symp.SpecificGrowth
          .Growth_Root = symp.RootGrowth
          .Growth_Bud = symp.BudGrowth
          .Color_Location = symp.ColorLocation
          .Color_Leaf = symp.LeafColor
          .Color_Pattern = symp.LeafColorPattern
          .Color_OtherPattern = symp.LeafOtherPattern.Value
        End With
      End If

      '=== Get crop production: fertilizer applied ===
      Dim fertList = (From f In db.SoilReportFertApplied Where f.ReportID = ReportID
                      Select New FertApplied With {
                          .MethodID = f.MethodID, .N = f.N, .P2O5 = f.P2O5,
                          .K2O = f.K2O, .S = f.S, .Mn = f.Mn, .Zn = f.Zn,
                          .Cu = f.Cu, .B = f.B}).ToArray()

      '=== Get crop production: other ===
      Dim cropProd = (From cp In db.SoilReportCropProduction Where cp.ReportID = ReportID).SingleOrDefault()

      '=== Store results
      If (Not fertList Is Nothing AndAlso fertList.Length > 0) OrElse Not cropProd Is Nothing Then
        result.CropProd = New CropProduction()

        With result.CropProd
          If Not fertList Is Nothing Then
            .Broadcast = fertList.Where(Function(f) f.MethodID = FertAppliedMethod.Broadcast).SingleOrDefault()
            .RowBand = fertList.Where(Function(f) f.MethodID = FertAppliedMethod.Rowband).SingleOrDefault()
            .Topdress = fertList.Where(Function(f) f.MethodID = FertAppliedMethod.Topdress).SingleOrDefault()
          End If
          If Not cropProd Is Nothing Then
            .DatePlanted = cropProd.DatePlanted.ShortDateString()
            .PrevCropAffected = cropProd.IsPrevCropAffected
            .OtherNutrients = cropProd.OtherNutrients.Value
            .TillageID = cropProd.CropTillageID.GetValueOrDefault()
            .FieldCondID = cropProd.FieldConditionID.GetValueOrDefault()
            .GreenhouseID = cropProd.GreenhouseTypeID.GetValueOrDefault()
          End If
        End With
      End If

      Return result
    End Using

    Return Nothing
  End Function

  Public Shared Function GetSoilSamples(ByVal ReportID As Integer, ByVal ReportTypeID As Enums.ReportType) As List(Of SoilSample)
    Using ctx As New AppDataContext()
      Dim SsList As List(Of SoilSample) = New List(Of SoilSample)
      Dim SoilList As List(Of Soils) = (From s In ctx.Soils Where s.ReportID = ReportID AndAlso Not s.IsDeleted Select s Order By s.LabNumber).ToList

      For Each item In SoilList
        Dim SoilID As Integer = item.SoilID
        Dim ss As New SoilSample()
        ss.SoilID = SoilID
        ss.LabNumber = item.LabNumber
        ss.GrowerSampleID = IIf(Not item.GrowerSampleID Is Nothing, item.GrowerSampleID, Nothing)
        ss.LimePerAcre = IIf(Not item.LimeInTonnes Is Nothing, item.LimeInTonnes, Nothing)
        ss.LimeMonth = IIf(Not item.LimeMonth Is Nothing, item.LimeMonth, Nothing)
        ss.LimeYear = IIf(Not item.LimeYear Is Nothing, item.LimeYear, Nothing)
        ss.FirstCrop = IIf(Not item.FirstCrop Is Nothing, item.FirstCrop, Nothing)
        ss.SecondCrop = IIf(Not item.SecondCrop Is Nothing, item.SecondCrop, Nothing)

        ' Check if report type is  Diagnostic
        If ReportTypeID = Enums.ReportType.Diagnostic Then
          'ss.ProblemCrop = IIf(Not item.ProblemCrop Is Nothing, item.ProblemCrop, Nothing)
          'ss.NextCrop = IIf(Not item.NextCrop Is Nothing, item.NextCrop, Nothing)
          ss.PlantTissueID = IIf(Not item.CorrPlantSampleID Is Nothing, item.CorrPlantSampleID, Nothing)
          ss.NematodeAssayID = IIf(Not item.CorrNematodeSampleID Is Nothing, item.CorrNematodeSampleID, Nothing)
          ss.CropCondition = IIf(Not item.CropCondition Is Nothing, item.CropCondition, Nothing)
          ss.DroughtStress = IIf(Not item.DroughtStress Is Nothing, item.DroughtStress, Nothing)
          'Else
          '   ss.FirstCrop = IIf(Not item.FirstCrop Is Nothing, item.FirstCrop, Nothing)
          '   ss.SecondCrop = IIf(Not item.SecondCrop Is Nothing, item.SecondCrop, Nothing)
        End If

        SsList.Add(ss)
        ss = Nothing
      Next
      Return SsList
    End Using

  End Function

  Public Shared Function GetSoilSampleBySoilID(ByVal SoilID As Integer) As DataLayer.Soils
    Using ctx As New AppDataContext()
      Return (From s In ctx.Soils Where s.SoilID = SoilID).SingleOrDefault
    End Using
  End Function

  'AJK 11/24/2014  Changing "LimeInTonnes" to Decimal to prevent rounding when saving.  BOSS Ticket 329786
  Public Shared Sub UpdateSoilSampleLimeInfo(ByVal SoilID As Integer,
                                             ByVal LimeMonth As Integer?,
                                             ByVal LimeYear As Integer?,
                                             ByVal LimeInTonnes As Decimal?,
                                             ByVal User As String)
    Using ctx As New AppDataContext()
      Dim sample As DataLayer.Soils = (From s In ctx.Soils Where s.SoilID = SoilID).SingleOrDefault
      If Not sample Is Nothing Then
        With sample
          .LimeMonth = LimeMonth
          .LimeYear = LimeYear
          .LimeInTonnes = LimeInTonnes
          .ModifiedOn = System.DateTime.Now
          .ModifiedBy = User
        End With
        ctx.SubmitChanges()
        ctx.uspSoil_GetFertRecs(SoilID)
      End If
    End Using
  End Sub

  Public Shared Sub UpdateFertRecommendations(ByVal SoilID As Integer)
    Using ctx As New AppDataContext()
      ctx.uspSoil_GetFertRecs(SoilID)
    End Using
  End Sub

  Public Shared Sub UpdateCropCodes(ByVal SoilID As Integer, ByVal FirstCrop As String, ByVal SecondCrop As String, ByVal User As String, ByVal Save As Boolean)
    Using ctx As New AppDataContext()
      If SecondCrop.Trim.Length = 0 Then
        SecondCrop = Nothing
      End If
      ctx.uspSoil_UpdateCropCodes(SoilID, FirstCrop, SecondCrop, User, Save)
      ctx.SubmitChanges()
    End Using
  End Sub

  Public Shared Function CropComboOK(ByVal firstCropCode As String, ByVal secondCropCode As String) As Boolean
    '-- Verify that we are not mixing "Lawn and Garden" crops with other crop types.
    If firstCropCode.Trim.Length = 0 Or secondCropCode.Trim.Length = 0 Then
      '-- Valid: There is only one crop
      Return True
    Else
      '-- Verify that BOTH crops either are/are not "Lawn and Garden" crops
      Dim theFirstCrop As DataLayer.SoilCrops = BusinessLayer.SoilCrop.GetByCropCode(firstCropCode)
      Dim theSecondCrop As DataLayer.SoilCrops = BusinessLayer.SoilCrop.GetByCropCode(secondCropCode)
      If (theFirstCrop.CalcGroup = theSecondCrop.CalcGroup) Then
        Return True
      Else
        Return False
      End If
      Return False
    End If
  End Function

  Public Shared Sub SaveDiagDataEntryReport(ByVal ReportID As Integer, ByVal Data As DiagDataEntryReport, ByVal User As String)
    If Data Is Nothing Then Return

    Using db As New AppDataContext()
      '-------------------
      '   Plant Symptoms
      '-------------------
      Dim isNewSymp As Boolean = False
      'Dim symp As DataLayer.SoilReportPlantSymptoms

      Dim symp = (From s In db.SoilReportPlantSymptoms
                  Where s.ReportID = ReportID
                  Select s).SingleOrDefault()

      If symp Is Nothing Then
        symp = New DataLayer.SoilReportPlantSymptoms()
        isNewSymp = True
      End If
      With symp
        .GeneralGrowth = Data.Growth_General
        .SpecificGrowth = Data.Growth_Specific
        .RootGrowth = Data.Growth_Root
        .BudGrowth = Data.Growth_Bud
        .ColorLocation = Data.Color_Location
        .LeafColor = Data.Color_Leaf
        .LeafColorPattern = Data.Color_Pattern
        .LeafOtherPattern = Data.Color_OtherPattern.Nullable()
        .ModifiedOn = DateTime.Now
        .ModifiedBy = User
      End With
      If isNewSymp Then
        symp.ReportID = ReportID
        db.SoilReportPlantSymptoms.InsertOnSubmit(symp)
      End If

      '---------------------------------------
      '  Crop Production: Fertilizer Applied
      '---------------------------------------
      Dim isNewFert As Boolean = False
      Dim fertList = (From fl In db.SoilReportFertApplied Where fl.ReportID = ReportID Select fl).ToArray()

      '=== Broadcast (ID: 1) ===
      Dim broadcastRec = fertList.Where(Function(f) f.MethodID = FertAppliedMethod.Broadcast).SingleOrDefault()
      If broadcastRec Is Nothing Then
        broadcastRec = New DataLayer.SoilReportFertApplied()
        isNewFert = True
      End If
      SetDiagFertAppliedValues(broadcastRec, FertAppliedMethod.Broadcast, Data.CropProd.Broadcast, ReportID, isNewFert, User)
      If isNewFert Then db.SoilReportFertApplied.InsertOnSubmit(broadcastRec)

      '=== Row/band (ID: 2) ===
      isNewFert = False
      Dim rowbandRec = fertList.Where(Function(f) f.MethodID = FertAppliedMethod.Rowband).SingleOrDefault()
      If rowbandRec Is Nothing Then
        rowbandRec = New DataLayer.SoilReportFertApplied()
        isNewFert = True
      End If
      SetDiagFertAppliedValues(rowbandRec, FertAppliedMethod.Rowband, Data.CropProd.RowBand, ReportID, isNewFert, User)
      If isNewFert Then db.SoilReportFertApplied.InsertOnSubmit(rowbandRec)

      '=== Topdress / Foliar (ID: 3) ===
      isNewFert = False
      Dim topdressRec = fertList.Where(Function(f) f.MethodID = FertAppliedMethod.Topdress).SingleOrDefault()
      If topdressRec Is Nothing Then
        topdressRec = New DataLayer.SoilReportFertApplied()
        isNewFert = True
      End If
      SetDiagFertAppliedValues(topdressRec, FertAppliedMethod.Topdress, Data.CropProd.Topdress, ReportID, isNewFert, User)
      If isNewFert Then db.SoilReportFertApplied.InsertOnSubmit(topdressRec)

      '--------------------------
      '  Crop Production: Other
      '--------------------------
      Dim isNewCropProd As Boolean = False
      Dim cropProd = (From cp In db.SoilReportCropProduction
                      Where cp.ReportID = ReportID
                      Select cp).SingleOrDefault()

      If cropProd Is Nothing Then
        cropProd = New DataLayer.SoilReportCropProduction()
        isNewCropProd = True
      End If
      With cropProd
        .DatePlanted = Data.CropProd.DatePlanted.ToDate().Nullable()
        .IsPrevCropAffected = Data.CropProd.PrevCropAffected
        .OtherNutrients = Data.CropProd.OtherNutrients.Nullable()
        .CropTillageID = Data.CropProd.TillageID
        .FieldConditionID = Data.CropProd.FieldCondID
        .GreenhouseTypeID = Data.CropProd.GreenhouseID
        .ModifiedOn = DateTime.Now
        .ModifiedBy = User
      End With
      If isNewCropProd Then
        cropProd.ReportID = ReportID
        db.SoilReportCropProduction.InsertOnSubmit(cropProd)
      End If

      ' Save changes
      db.SubmitChanges()
    End Using
  End Sub

  Private Shared Sub SetDiagFertAppliedValues(ByRef DBRecord As DataLayer.SoilReportFertApplied, ByVal MethodID As FertAppliedMethod,
                                              ByVal Values As FertApplied, ByVal ReportID As Integer, ByVal IsNewRec As Boolean, ByVal User As String)
    With DBRecord
      If IsNewRec Then
        .ReportID = ReportID
        .MethodID = MethodID
      End If
      .N = Values.N
      .P2O5 = Values.P2O5
      .K2O = Values.K2O
      .S = Values.S
      .Mn = Values.Mn
      .Zn = Values.Zn
      .Cu = Values.Cu
      .B = Values.B
      .ModifiedOn = DateTime.Now
      .ModifiedBy = User
    End With
  End Sub

  'Public Shared Function SaveSoilInfoDataEntrySamples(ByRef sslist As List(Of SoilSample), ByVal ReportTypeID As Enums.ReportType, ByVal User As String) As Boolean
  '    Dim retVal As Boolean = False
  '    Using ctx As New AppDataContext()
  '        Try
  '            For Each item In sslist
  '                Dim SoilID As Integer = item.SoilID
  '                Dim row = (From s In ctx.Soils Where s.SoilID = SoilID Select s).Single()
  '                row.GrowerSampleID = item.GrowerSampleID
  '                row.ModifiedBy = User
  '                row.ModifiedOn = DateTime.Now
  '                If (row.Reports.StatusID <> Enums.ReportStatus.LoggedIn) Then
  '                    row.Reports.StatusID = Enums.ReportStatus.LoggedIn
  '                End If
  '                'check Report Type
  '                If Not ReportTypeID = Enums.ReportType.Research Then
  '                    row.LimeInTonnes = item.LimePerAcre
  '                    row.LimeMonth = item.LimeMonth
  '                    row.LimeYear = item.LimeYear
  '                    row.FirstCrop = item.FirstCrop.Nullable()
  '                    row.SecondCrop = item.SecondCrop.Nullable()
  '                End If
  '                ctx.SubmitChanges()
  '                row = Nothing
  '            Next
  '            retVal = True
  '        Catch ex As Exception
  '            ErrorHandling.ProcessError(ex, "An error ocurred while trying to save the soil information sample data.", , True)
  '        End Try
  '    End Using
  '    Return retVal
  'End Function

  'Public Shared Function SaveDiagSoilInfoDataEntrySamples(ByRef sslist As List(Of SoilSample), ByVal User As String) As Boolean

  '    Dim retVal As Boolean = False

  '    Using ctx As New AppDataContext()
  '        Try
  '            For Each item In sslist
  '                Dim SoilID As Integer = item.SoilID
  '                Dim row = (From s In ctx.Soils Where s.SoilID = SoilID Select s).Single()

  '                row.GrowerSampleID = item.GrowerSampleID
  '                row.LimeInTonnes = item.LimePerAcre
  '                row.LimeMonth = item.LimeMonth
  '                row.LimeYear = item.LimeYear

  '                row.FirstCrop = If(item.FirstCrop.Trim.Length = 0, Nothing, item.FirstCrop)
  '                row.SecondCrop = If(item.SecondCrop.Trim.Length = 0, Nothing, item.SecondCrop)

  '                row.ProblemCrop = Nothing
  '                row.NextCrop = Nothing

  '                row.CorrPlantSampleID = item.PlantTissueID
  '                row.CorrNematodeSampleID = item.NematodeAssayID
  '                row.CropCondition = item.CropCondition
  '                row.DroughtStress = item.DroughtStress
  '                row.ModifiedBy = User
  '                row.ModifiedOn = DateTime.Now
  '                If (row.Reports.StatusID <> Enums.ReportStatus.LoggedIn) Then
  '                    row.Reports.StatusID = Enums.ReportStatus.LoggedIn
  '                End If
  '                ctx.SubmitChanges()
  '                SoilID = Nothing
  '                row = Nothing
  '            Next
  '            retVal = True

  '        Catch ex As Exception
  '            ErrorHandling.ProcessError(ex, "An error ocurred while trying to save the soil diagnostic information sample data.", , True)
  '        End Try

  '    End Using

  'End Function

  Public Shared Function GetCheckSampleData(ByVal aTestID As Integer,
                                            Optional ByVal StartDate As String = Nothing,
                                            Optional ByVal EndDate As String = Nothing,
                                            Optional ByVal SetNum As String = Nothing,
                                            Optional ByVal LabNum As String = Nothing,
                                            Optional ByVal Series As String = Nothing) As DataTable
    Dim aDT As DataTable = Nothing
    Try
      '-- Find matching rechecks by RecheckBatchID
      Dim theParameters() As SqlClient.SqlParameter = {New SqlClient.SqlParameter("@TestID", aTestID),
                                                       New SqlClient.SqlParameter("@StartDate", StartDate),
                                                       New SqlClient.SqlParameter("@EndDate", EndDate),
                                                       New SqlClient.SqlParameter("@SetNum", SetNum),
                                                       New SqlClient.SqlParameter("@LabNum", LabNum),
                                                       New SqlClient.SqlParameter("@Series", Series)}

      aDT = SharedLayer.ADODotNetHelper.ExecuteSPReturnDatatable("uspSoil_GetCheckResults ", "CheckResults", theParameters)

    Catch ex As Exception
      ErrorHandling.ProcessError(ex, "An error ocurred while trying to retrieve check sample data.", , True)
    End Try
    Return aDT
  End Function

  Public Class SoilSample
    Private _soilID As Integer
    Private _labnumber As String
    Private _growersampleID As String
    Private _limePerAcre As Double
    Private _limeMonth As Integer
    Private _limeYear As Integer
    Private _firstCrop As String
    Private _secondCrop As String
    Private _problemCrop As String
    Private _nextCrop As String
    Private _plantTissueID As String
    Private _nematodeAssayID As String
    Private _cropCondition As String
    Private _droughtStress As Boolean

    Property SoilID() As Integer
      Get
        Return _soilID
      End Get
      Set(ByVal value As Integer)
        _soilID = value
      End Set
    End Property

    Property LabNumber() As String
      Get
        Return _labnumber
      End Get
      Set(ByVal value As String)
        _labnumber = value
      End Set
    End Property

    Property GrowerSampleID() As String
      Get
        Return _growersampleID
      End Get
      Set(ByVal value As String)
        _growersampleID = value
      End Set
    End Property


    Property LimePerAcre() As Double
      Get
        Return _limePerAcre
      End Get
      Set(ByVal value As Double)
        _limePerAcre = value
      End Set
    End Property

    Property LimeMonth() As Integer
      Get
        Return _limeMonth
      End Get
      Set(ByVal value As Integer)
        _limeMonth = value
      End Set
    End Property

    Property LimeYear() As Integer
      Get
        Return _limeYear
      End Get
      Set(ByVal value As Integer)
        _limeYear = value
      End Set
    End Property

    Property FirstCrop() As String
      Get
        Return _firstCrop
      End Get
      Set(ByVal value As String)
        _firstCrop = value
      End Set
    End Property

    Property SecondCrop() As String
      Get
        Return _secondCrop
      End Get
      Set(ByVal value As String)
        _secondCrop = value
      End Set
    End Property

    Property ProblemCrop() As String
      Get
        Return _problemCrop
      End Get
      Set(ByVal value As String)
        _problemCrop = value
      End Set
    End Property

    Property NextCrop() As String
      Get
        Return _nextCrop
      End Get
      Set(ByVal value As String)
        _nextCrop = value
      End Set
    End Property

    Property PlantTissueID() As String
      Get
        Return _plantTissueID
      End Get
      Set(ByVal value As String)
        _plantTissueID = value
      End Set
    End Property

    Property NematodeAssayID() As String
      Get
        Return _nematodeAssayID
      End Get
      Set(ByVal value As String)
        _nematodeAssayID = value
      End Set
    End Property

    Property CropCondition() As String
      Get
        Return _cropCondition
      End Get
      Set(ByVal value As String)
        _cropCondition = value
      End Set
    End Property

    Property DroughtStress() As Boolean
      Get
        Return _droughtStress
      End Get
      Set(ByVal value As Boolean)
        _droughtStress = value
      End Set
    End Property
  End Class


#Region "DiagDataEntryReport"
  Public Class DiagDataEntryReport
    Public Growth_General As Integer
    Public Growth_Specific As Integer
    Public Growth_Root As Integer
    Public Growth_Bud As Integer
    Public Color_Location As Integer
    Public Color_Leaf As Integer
    Public Color_Pattern As Integer
    Public Color_OtherPattern As String
    Public CropProd As CropProduction
  End Class

  Public Class CropProduction
    Public Broadcast As FertApplied
    Public RowBand As FertApplied
    Public Topdress As FertApplied
    Public DatePlanted As String
    Public PrevCropAffected As Boolean
    Public OtherNutrients As String
    Public TillageID As Integer
    Public FieldCondID As Integer
    Public GreenhouseID As Integer
  End Class

  Public Structure FertApplied
    Public MethodID As Integer
    Public N As Decimal
    Public P2O5 As Decimal
    Public K2O As Decimal
    Public S As Decimal
    Public Mn As Decimal
    Public Zn As Decimal
    Public Cu As Decimal
    Public B As Decimal
  End Structure

  Private Enum FertAppliedMethod
    Broadcast = 1
    Rowband
    Topdress
  End Enum

#End Region

End Class
