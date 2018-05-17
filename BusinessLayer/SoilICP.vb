Option Explicit On
Option Strict On
Imports System.IO
Imports BusinessLayer

Public Class SoilICP
  Inherits Soil
  Public Const Abbreviation As String = "ICP"

  Public Overloads Shared Function GetCheckSampleData(ByVal aCheckID As String) As DataLayer.SoilCheckSampleValues
    Using db As New AppDataContext
      Return (From CS In db.SoilCheckSampleValues
              Where CS.CheckSampleTypeID = aCheckID _
                And CS.IsDeleted = False
              Select CS).SingleOrDefault
    End Using

  End Function
  Public Shared Function GetICPSoilsDataByLabNumber(ByVal labnumberSource As String, ByVal fiscalYear As Integer) As ICPSoilsDatabaseInfo
    If String.IsNullOrEmpty(labnumberSource).Equals(True) Then
      Throw New ArgumentNullException("labnumberSource")
    ElseIf fiscalYear = 0 Then
      Throw New ArgumentOutOfRangeException("fiscalYear")
    Else
      Using db As New AppDataContext
        Dim sampleInfo = (From s In db.Soils
                          Where s.LabNumber = labnumberSource And s.FiscalYear = fiscalYear
                          Select s.LabNumber, s.SoilID, s.CheckSampleTypeID).SingleOrDefault
        If sampleInfo IsNot Nothing Then
          Return New ICPSoilsDatabaseInfo With {
          .LabNumber = sampleInfo.LabNumber,
          .SampleID = sampleInfo.SoilID,
          .CheckType = sampleInfo.CheckSampleTypeID
        }
        Else
          Return New ICPSoilsDatabaseInfo()
        End If
      End Using
    End If
  End Function

  Public Shared Function GetICPRecheckBatchBySampleID(ByVal sampleID As Integer, ByVal lab As Enums.LabType) As Integer
    Using db As New AppDataContext
      Return (From rch In db.Rechecks
              Where rch.SampleID = sampleID And rch.LabID = lab And rch.TestID = Enums.LabTestType.ICP
              Select rch.BatchID).FirstOrDefault.ToInt
    End Using
  End Function

  Public Shared Function HasDuplicateConfirmedSample(ByVal soilID As Integer, ByVal isRecheck As Boolean) As Boolean
    Using db As New AppDataContext
      Return (From ICP In db.Lab_ICPResults
              Where ICP.SampleID = soilID And ICP.LabID = Enums.LabType.Soil And ICP.Confirmed = True And ICP.IsRecheck = isRecheck).Any
    End Using
  End Function

  Public Shared Function IsICPSoilRecheckSample(ByVal soilID As Integer, ByVal labID As Enums.LabType) As Boolean
    Using db As New AppDataContext
      Return (From rch In db.Rechecks
              Where rch.SampleID = soilID And rch.LabID = labID And rch.TestID = Enums.LabTestType.ICP And rch.IsDeleted = False).Any
    End Using
  End Function

  Public Shared Function GetSoilDataFromLabNumber(ByVal aLabNumber As String, ByVal aFiscalYear As Integer) As SoilsData
    Using db As New AppDataContext
      Dim alist As DataLayer.Soils = (From s In db.Soils
                                      Where s.LabNumber = aLabNumber _
                        And s.FiscalYear = aFiscalYear
                                      Select s).SingleOrDefault
      Dim myData As New SoilsData
      If Not alist Is Nothing Then
        myData.LabNumber = aLabNumber
        myData.SoilID = alist.SoilID
        myData.CheckType = alist.CheckSampleTypeID
        If Not myData.CheckType Is Nothing Then
          myData.CheckData = GetCheckSampleData(myData.CheckType)
        End If
      End If

      Return myData
    End Using
  End Function

  Public Shared Function GetICPTestLimits(ByVal myStationID As Integer) As List(Of SoilICPToleranceLevel)
    Dim aList As New List(Of SoilICPToleranceLevel)
    Using db As New AppDataContext
      Dim aData = (From sl In db.Lab_StationLimits
                   Where sl.TestID = Enums.LabTestType.ICP _
                         And sl.StationID = myStationID
                   Select sl.ErrorLowerLimit, sl.ErrorUpperLimit,
                                sl.WarningLowerLimit, sl.WarningUpperLimit,
                                sl.MResult)
      For Each item In aData
        Dim newItem As New SoilICPToleranceLevel With {
          .Element = item.MResult,
          .ErrorLowerLimit = item.ErrorLowerLimit,
          .ErrorUpperLimit = item.ErrorUpperLimit,
          .WarningLowerLimit = item.WarningLowerLimit,
          .WarningUpperLimit = item.WarningUpperLimit
        }
        aList.Add(newItem)
      Next
    End Using
    Return aList
  End Function

  Public Shared Function GetSoilCheckSampleValues() As DataTable
    Using db As New AppDataContext
      Return (From cs In db.SoilCheckSampleValues Where cs.IsDeleted <> True Select cs).ToDataTable
    End Using
  End Function

  Public Shared Function GetSoilControlSampleValues(ByVal stationId As Integer) As DataTable
    Using db As New AppDataContext
      Return (From lsl In db.Lab_StationLimits Where (lsl.StationID = stationId) Select lsl).ToDataTable
    End Using
  End Function

  Public Shared Function GetICPCcvLimits() As List(Of SoilICPToleranceLevel)
    Dim aList As New List(Of SoilICPToleranceLevel)
    Using db As New AppDataContext
      Dim aData = (From sl In db.uspLAB_getSoilIcpCcvLimits()
                   Select sl.Test, sl.ErrorLowerLimit, sl.WarningLowerLimit,
                       sl.ErrorUpperLimit, sl.WarningUpperLimit)

      For Each item In aData
        Dim newItem As New SoilICPToleranceLevel With {
          .Element = item.Test,
          .ErrorLowerLimit = CDbl(item.ErrorLowerLimit),
          .ErrorUpperLimit = CDbl(item.ErrorUpperLimit),
          .WarningLowerLimit = CDbl(item.WarningLowerLimit),
          .WarningUpperLimit = CDbl(item.WarningUpperLimit)
        }
        aList.Add(newItem)
      Next
    End Using
    Return aList
  End Function

  Public Shared Function HasTestResults(ByVal SampleID As Integer,
                                          ByVal LabID As Integer,
                                          ByVal HeavyMetals As Boolean) As Boolean
    Dim blnResult As Boolean
    Dim MResult As String = "P"

    If HeavyMetals Then
      MResult = "PB"
    End If
    Using db As New AppDataContext
      Dim aData = (From ICP In db.Lab_ICPResults
                   Where ICP.SampleID = SampleID _
                         And ICP.LabID = LabID _
                         And ICP.MResult.ToUpper = MResult _
                         And ICP.Confirmed = True
                   Select ICP).ToList
      blnResult = aData.Count > 0
    End Using
    Return blnResult
  End Function

  Public Shared Function GetOriginalResults(ByVal SampleID As Integer, ByVal LabID As Integer) As List(Of DataLayer.Lab_ICPResult)
    Using db As New AppDataContext
      Dim aData As List(Of DataLayer.Lab_ICPResult) = (From ICP In db.Lab_ICPResults
                                                       Where ICP.SampleID = SampleID _
                                                             And ICP.LabID = LabID _
                                                             And ICP.Confirmed = True _
                                                             And ICP.IsRecheck = False
                                                       Select ICP).ToList
      Return aData
    End Using

  End Function

  Public Shared Function IsRecheckStatus(ByVal SoilID As Integer,
                                           ByVal HeavyMetals As Boolean) As Boolean

    Dim blnResult As Boolean
    Dim TestType As Enums.LabTestType = Enums.LabTestType.ICP

    If HeavyMetals Then
      TestType = Enums.LabTestType.HM
    End If
    Using db As New AppDataContext
      Dim aData = (From ST In db.SoilTests
                   Where ST.SoilID = SoilID _
                         And ST.TestID = TestType _
                         And ST.IsDeleted = False _
                         And ST.TestStatusID = Enums.LabTestStatus.RecheckAssigned
                   Select ST).ToList

      blnResult = aData.Count > 0
    End Using
    Return blnResult
  End Function

  Public Shared Function SaveICPResults(ByVal SampleID As String,
                                          ByVal Tests As List(Of String),
                                          ByVal Results As List(Of String),
                                          ByVal ModifiedBy As String,
                                          ByVal LabID As Integer,
                                          ByVal InstrumentID As Integer,
                                          ByVal LabNumber As String,
                                          ByVal BatchID As Integer,
                                          ByVal Confirmed As Boolean,
                                          ByVal Position As String,
                                          ByVal UnitOfMeasure As String,
                                          ByVal IsRecheck As Boolean,
                                          ByVal UserName As String) As Boolean
    Dim blnResult As Boolean = False
    Try
      If Tests.Count <> Results.Count Then
        Return False
        Exit Function
      End If
      Using db As New AppDataContext()
        For I = 0 To Tests.Count - 1
          If Results(I) <> "" Then
            Dim myResult As DataLayer.Lab_ICPResult = (From ICP In db.Lab_ICPResults
                                                       Where ICP.LabNumber = LabNumber _
                                                          And ICP.BatchID = BatchID _
                                                          And ICP.MResult = Tests(I) _
                                                          And ICP.Position = Position
                                                       Select ICP).SingleOrDefault
            If myResult Is Nothing Then
              myResult = New DataLayer.Lab_ICPResult
            ElseIf Not Confirmed Then
              Exit For
            Else
              UnConfirmPreviousRec(SampleID, LabNumber, ModifiedBy, Tests(I), IsRecheck)
            End If

            If IsRecheck Then
              UnConfirmPreviousRec(SampleID, LabNumber, ModifiedBy, Tests(I), IsRecheck)
            End If

            myResult.MResult = Tests(I)
            If SampleID <> "" And SampleID <> "0" And SampleID <> Nothing Then
              myResult.SampleID = CInt(SampleID)
            End If
            myResult.SavedDate = Now
            myResult.ModifiedBy = ModifiedBy
            myResult.LabID = LabID
            If Results(I) = GlobalConstants.Soil_NoSoilIndicator Then
              myResult.NoSoil = True
            Else
              myResult.Result = CDec(Results(I))
            End If
            myResult.InstrumentID = InstrumentID
            myResult.LabNumber = LabNumber
            myResult.BatchID = BatchID
            myResult.Confirmed = Confirmed
            myResult.Position = Position
            myResult.UnitOfMeasure = UnitOfMeasure
            myResult.IsRecheck = IsRecheck

            If Not myResult.TestResultID > 0 Then
              db.Lab_ICPResults.InsertOnSubmit(myResult)
            End If
          End If
        Next
        If Confirmed Then
          Dim SoilTest As DataLayer.SoilTests = (From ST In db.SoilTests
                                                 Where ST.SoilID = CInt(SampleID) _
                                                          And ST.TestID = Enums.LabTestType.ICP
                                                 Select ST).SingleOrDefault
          If Not SoilTest Is Nothing Then
            If SoilTest.TestStatusID = Enums.LabTestStatus.Assigned Or
                            SoilTest.TestStatusID = Enums.LabTestStatus.InProcess Then
              'Soil.UpdateTestAndSampleStatus(CInt(SampleID), Enums.LabTestType.ICP, Enums.LabTestStatus.Analyzed, UserName)
              SoilTest.TestStatusID = Enums.LabTestStatus.Analyzed

            ElseIf SoilTest.TestStatusID = Enums.LabTestStatus.RecheckAssigned Or
                                SoilTest.TestStatusID = Enums.LabTestStatus.RecheckInProcess Then
              'Soil.UpdateTestAndSampleStatus(CInt(SampleID), Enums.LabTestType.ICP, Enums.LabTestStatus.RecheckAnalyzed, UserName)
              SoilTest.TestStatusID = Enums.LabTestStatus.RecheckAnalyzed
            End If
            SoilTest.ModifiedBy = ModifiedBy
            SoilTest.ModifiedOn = Now
          End If
        End If
        db.SubmitChanges()
        blnResult = True
      End Using
    Catch ex As Exception
      Throw New Exception("Error Saving ICP Results: " & LabNumber)
    End Try
    Return blnResult
  End Function

  Private Shared Sub UnConfirmPreviousRec(ByVal sampleID As String,
                                         ByVal labNumber As String,
                                         ByVal modifiedby As String,
                                         ByVal MResult As String,
                                         ByVal isRecheck As Boolean)
    Using db As New AppDataContext()
      Dim myResult = (From ICP In db.Lab_ICPResults
                      Where ICP.LabNumber = labNumber _
                             And ICP.SampleID = CInt(sampleID) _
                             And ICP.IsRecheck = isRecheck _
                             And ICP.Confirmed = True _
                             And ICP.MResult = MResult
                      Select ICP)
      For Each rec As DataLayer.Lab_ICPResult In myResult
        rec.Confirmed = False
        rec.ModifiedBy = modifiedby
      Next
      'Submit changes called at the end of save function.
      'db.SubmitChanges()
    End Using

  End Sub

  Public Shared Function SaveICPResults(ByVal Results As List(Of DataLayer.Lab_ICPResult), ByVal HeavyMetals As Boolean) As Boolean
    Dim blnResult As Boolean = False
    Dim UniqLst As New List(Of Integer)
    Try
      If Results.Count > 0 Then
        Using db As New AppDataContext()
          Dim oldBatch = (From ICP In db.Lab_ICPResults
                          Where ICP.BatchID = Results(0).BatchID _
                                       And ICP.InstrumentID = Results(0).InstrumentID
                          Select ICP).ToList
          db.Lab_ICPResults.DeleteAllOnSubmit(oldBatch)
          db.Lab_ICPResults.InsertAllOnSubmit(Results)
          Dim SoilTests As List(Of DataLayer.SoilTests)
          If HeavyMetals Then
            SoilTests = (From ST In db.SoilTests
                         Where ((From R In Results Where R.Confirmed = True Select R.SampleID).Distinct.Contains(ST.SoilID)) _
                                      And ST.TestID = Enums.LabTestType.HM
                         Select ST).ToList
          Else
            SoilTests = (From ST In db.SoilTests
                         Where ((From R In Results Where R.Confirmed = True Select R.SampleID).Distinct.Contains(ST.SoilID)) _
                                      And ST.TestID = Enums.LabTestType.ICP
                         Select ST).ToList
          End If
          For Each soiltest As DataLayer.SoilTests In SoilTests
            If Not soiltest Is Nothing Then
              If soiltest.TestStatusID = Enums.LabTestStatus.Assigned Or
                                soiltest.TestStatusID = Enums.LabTestStatus.InProcess Then
                soiltest.TestStatusID = Enums.LabTestStatus.Analyzed
              ElseIf soiltest.TestStatusID = Enums.LabTestStatus.RecheckAssigned Or
                                    soiltest.TestStatusID = Enums.LabTestStatus.RecheckInProcess Then
                soiltest.TestStatusID = Enums.LabTestStatus.RecheckAnalyzed
              End If
              soiltest.ModifiedBy = Results(0).ModifiedBy
              soiltest.ModifiedOn = Now
            End If
          Next
          db.SubmitChanges()
          blnResult = True
        End Using
      End If
    Catch ex As Exception
      blnResult = False
      Throw New Exception("Error Saving ICP Results: " + ex.Message)
    End Try
    Return blnResult
  End Function

  Public Shared Shadows Function Find(ByVal htParameters As Hashtable) As List(Of DataLayer.uspLab_SoilICPResults_FindResult)
    '-- Find matching soil EC Station rows
    Using ctx As New AppDataContext()
      Return ctx.uspLab_SoilICPResults_Find(BuildFindWhereClause(htParameters), Convert.ToInt32(htParameters("MaxResults"))).ToList
    End Using
  End Function

  Public Shared Shadows Function BuildFindWhereClause(ByVal htParameters As Hashtable) As String

    '-- Build WHERE clause for finding matching rows
    Dim FiscalYear As String = htParameters("FiscalYear").ToString
    Dim BegLabNum As String = htParameters("BegLabNum").ToString
    Dim EndLabNum As String = htParameters("EndLabNum").ToString
    Dim BegDate As String = DBHelper.BuildBeginDateString(htParameters("BegDate").ToString)
    Dim EndDate As String = DBHelper.BuildEndDateString(htParameters("EndDate").ToString)
    Dim User As String = htParameters("User").ToString
    Dim InstrumentID As String = htParameters("InstrumentID").ToString

    '-- Build WHERE clause
    Dim WhereClause As String = ""
    WhereClause = DBHelper.BuildWhereClause(WhereClause, "FiscalYear", FiscalYear.ToString, "", False, DBHelper.SearchOperator.Equals)
    WhereClause = DBHelper.BuildWhereClause(WhereClause, "LabNumber", BegLabNum, EndLabNum, True, DBHelper.SearchOperator.Between)
    WhereClause = DBHelper.BuildWhereClause(WhereClause, "SavedDate", BegDate, EndDate, True, DBHelper.SearchOperator.Between)
    WhereClause = DBHelper.BuildWhereClause(WhereClause, "ModifiedBy", User, "", True, DBHelper.SearchOperator.Equals)
    WhereClause = DBHelper.BuildWhereClause(WhereClause, "InstrumentID", InstrumentID, "", False, DBHelper.SearchOperator.Equals)
    WhereClause = "(" + WhereClause + ")"

    '-- Return WHERE clause
    Return WhereClause

  End Function

  Public Shared Function GetICPResetData(ByVal StartLabNum As String,
                                            ByVal EndLabNum As String,
                                            ByVal FiscalYear As Integer) As DataSet
    Dim aDS As DataSet = Nothing
    Try
      'AJK 07/31/13  Using ADO because this stored procedure is returning multiple datasets and I didn't have
      'time to figure out how to do it in LINQ
      Dim theParameters() As SqlClient.SqlParameter = {New SqlClient.SqlParameter("@StartLabNum", StartLabNum),
                                                             New SqlClient.SqlParameter("@EndLabNum", EndLabNum),
                                                             New SqlClient.SqlParameter("@FiscalYear", FiscalYear),
                                                             New SqlClient.SqlParameter("@ViewData", True),
                                                             New SqlClient.SqlParameter("@ModifiedBy", SharedLayer.SecurityHelper.GetUserName)}

      aDS = ADODotNetHelper.ExecuteSPReturnDataSet("uspSoil_ICPReset", theParameters)

    Catch ex As Exception
      Throw New Exception("Error finding data to reset.")
    End Try
    Return aDS

  End Function

  Public Shared Function ICPDataReset(ByVal StartLabNum As String,
                                        ByVal EndLabNum As String,
                                        ByVal FiscalYear As Integer) As Boolean
    Dim aResult As Boolean = False
    Try
      Using db As New AppDataContext
        db.uspSoil_ICPReset(StartLabNum, EndLabNum, FiscalYear, False, SharedLayer.SecurityHelper.GetUserName)
        aResult = True
      End Using
    Catch ex As Exception
      Throw New Exception("Error doing data reset.")
    End Try
    Return aResult
  End Function

  Public Shared Function GetFileList(ByVal aLabNumber As String,
                                  ByVal DoBatches As Boolean,
                                  ByVal FiscalYear As Integer) As List(Of String)

    Dim lstResult As List(Of String) = Nothing
    Dim adata As SoilsData = GetSoilDataFromLabNumber(aLabNumber, FiscalYear)
    If adata.SoilID > 0 Then
      lstResult = PWSMICP.GetFileList(adata.SoilID, DoBatches, 1)
    Else
      lstResult = New List(Of String)
    End If
    Return lstResult
  End Function
End Class
'Public Class ParseLabNumber
'  Private _LabNumber As LabNumber
'  Public Sub New(ByVal labNubmer As String)
'    If String.IsNullOrEmpty(labNubmer).Equals(False) Then
'      Dim labNumber As String = Helper.LabNumberRemovedInvalidCharacters(labNubmer)
'      If Helper.LabNumberIsValidFormat(labNubmer) Then
'        _LabNumber = New LabNumber
'        _LabNumber.FullNumber = labNubmer
'      End If
'    Else
'      Throw New ArgumentNullException("labNumber")
'    End If
'  End Sub
'End Class
Public Class SoilICPToleranceLevel
  Public ErrorLowerLimit As Double
  Public ErrorUpperLimit As Double
  Public WarningLowerLimit As Double
  Public WarningUpperLimit As Double
  Public Element As String
End Class