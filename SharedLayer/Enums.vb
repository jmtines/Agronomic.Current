Public Class Enums

  Public Class EmailGroups
    Public Const SoilLabAdminGroup = "ToBeDecided1@ncagr.gov"
    Public Const Group2 = "ToBeDecided2@ncagr.gov"
  End Class

  Public Enum Direction As Integer
    None = 0
    Up
    Down
    Left
    Right
  End Enum

  Public Enum GenericMessageType As Integer
    None = 0
    Information
    Warning
    ErrorMsg
  End Enum

  Public Enum NumberType As Integer
    Report = 1
    Lab
  End Enum

  Public Enum CustomerType As Integer
    Grower = 1
    Advisor = 2
    RegionalAgronomist = 3
    CESAgent = 4
    EscrowAccount = 5
  End Enum

  ' Originally we wanted to use the TestID of the data table Tests - but were worried about what effects it might have - adding to the Tests data table
  ' SO we created a new data table instead and have this Enum that mirrors the data table
  ' We are trying to follow the TestID of the data table Tests as much as possible
  Public Enum QcCheckSampleTests As Integer
    ' All of these are from the data table Tests (same as TestID)
    None = 0
    Al = 1 ' ICP test but already defined in data table Tests
    NH4 = 2
    CCE = 3
    C = 4
    CL = 6
    LI = 11 ' ICP test but already defined in data table Tests
    MO = 12
    NO3 = 13
    N = 14
    pH = 15
    SE = 16 ' ICP test but already defined in data table Tests
    EC = 17
    UN = 18
    VW = 19
    TKN = 24
    Co = 32 ' ICP test but already defined in data table Tests

    ' These are ICP tests, TestID = 10
    P = 1001
    K = 1002
    Ca = 1003
    Mg = 1004
    S = 1005
    Fe = 1006
    Mn = 1007
    Zn = 1008
    Cu = 1009
    B = 1010
    Na = 1011
    Cd = 1012
    Pb = 1013
    Ni = 1014
    [As] = 1015
    Cr = 1016

    ' These tests did not exactly fit in anywhere else
    Clplt = 2001 ' different test than Cl
    NO3plt = 2002 ' different test than NO3
    CO3 = 2003
    HCO3 = 2004
  End Enum

  Public Enum LabTestType As Integer
    None = 0
    Aluminum = 1 ' Al
    Ammonium = 2 ' NH4
    CCE = 3
    C = 4
    CBC = 5
    CL = 6
    DM = 7
    OrganicMatter = 8
    HM = 9
    ICP = 10
    LI = 11
    MO = 12
    NO3 = 13
    N = 14
    pH = 15
    SE = 16
    EC = 17
    UN = 18
    BD = 51
    NL = 52
    VolumeWeigh = 19
    Counting = 20
    RootCountAndWeight = 21
    TKN = 24
    AdditionalMetals = 25
    DW = 28
    SoilCount = 29
    Egg = 30
    Other = 31
    Co = 32
    HmAll = 33
    Phosphate = 35
    Sulfate = 36
    DNASeq = 37
    RTPCR = 38
    RootKnotBioassay = 42
    SoybeanCystBioassay = 45
    SixtyMesh = 50
  End Enum

  Public Enum CommentType As Integer
    None = 0
    Receiving = 1
    DataEntry = 2
    Lab = 3
    Recommendations = 4
    Agronomist = 5
  End Enum

  Public Enum CommentLevel As Integer
    [Set] = 1
    Sample = 2
    Report = 3
  End Enum

  Public Enum InstrumentStatus As Integer
    None = 0
    Good
    Bad
  End Enum

  Public Enum LabType As Integer
    All = -1
    None = 0
    Soil = 1
    Plant = 2
    Waste = 3
    Media = 4
    Solution = 5
    Nematode = 6
  End Enum

  Public Enum StatusType As Integer
    ReportStatus = 1
    SampleStatus = 2
    LabTestStatus = 3
  End Enum

  Public Enum PALSSubmissionStatus As Integer
    Unsubmitted = 1
    Paid = 2
    BillMe = 3
    NoCharge = 4
    Deleted = 5
    Refunded = 6  'added 07/13/15 RK ticket 340744
    FundsCanceled = 7 ' added 08/25/15 RK
  End Enum

  Public Enum PALSTransactionType As Integer
    NotSet = 0
    PrePaySubmission = 1
    BillMe = 2
    NoCharge = 3
    PayNow = 4
  End Enum

  Public Enum ReportType As Integer
    Diagnostic = 1
    Internal = 2
    OutOfState = 3
    Predictive = 4
    Research = 5
    Regulatory = 6
    HeavyMetal = 7
    PredictiveEX = 8
    PredictiveWW = 9
    PineWood = 10
    Ecology = 11
    MolecularDiagnosis = 12
  End Enum

  Public Enum ReportStatus As Integer
    Received = 1
    InProcess = 2
    Analyzed = 3
    UnderReview = 4
    Complete = 5
    PendingPayment = 6
    Released = 7
    WriteOff = 8
    DashboardComplete = 11
    LoggedIn = 13
    RecheckAssigned = 14
    RecheckInProcess = 15
    RecheckAnalyzed = 16
    RecheckResolved = 17
  End Enum

  Public Enum SampleStatus As Integer
    Received = 1
    InProcess = 2
    Analyzed = 3
    RecheckAssigned = 5
    RecheckInProcess = 6
    RecheckAnalyzed = 7
  End Enum

  ' Keith Baldwin 2/14/2011
  ' This is the status of each test.  
  ' A sample will usually have multiple tests that are run on the sample
  ' This enumeration corresponds with the TestStatus data table
  Public Enum LabTestStatus As Integer
    Assigned = 1
    ' Don't think InProcess is typically used - since a sample test changes from Pending to Analyzed when it is analyzed. However soil pH is measured and recorded and then sits for an hour waiting for BpH so the status would be InProcess then
    InProcess = 2
    Analyzed = 3
    RecheckAssigned = 7
    RecheckInProcess = 8
    RecheckAnalyzed = 9
    'This was to be used for ICP Selectivity. I'm leaving it in just in case I need to change how this works in the future - JRV 3/1/15
    Omitted = 11
  End Enum

  Public Enum RecheckBatchStatus As Integer
    Initiated = 1
    Submitted = 2
    InProcess = 3
    Complete = 4
  End Enum

  Public Enum RecheckStatus As Integer
    Initiated = 1
    Submitted = 2
    InProcess = 3
    Complete = 4
    Resolved = 5
  End Enum

  '---------------------------------------------------------------------------------
  ' NOTE: You can add more DataAction members, but do NOT change, remove, or
  '       change the order of those listed with defined values.  Those with
  '       defined values are usually shared between server and client script,
  '       and if they're shared, the common.js JavaScript file needs to be kept
  '       in sync. New members can be added to the bottom without declaring defined
  '       values, since they will increment from the previously defined value.
  '---------------------------------------------------------------------------------
  Public Enum DataAction
    None = 0
    Create = 1
    Add = 2
    Update = 3
    Delete = 4
    Load = 5
    FirstLoad = 6
    Refresh = 7
    GoPrevious = 8
    GoNext = 9
    Hide = 10
    Show = 11
    Disable = 12
    Enable = 13
    ConfirmContinue = 14
  End Enum

  Public Enum CustomerWorkListType
    NewPALSUser = 1
    NewGrowerRequest = 2
  End Enum

  Public Enum FTPFileType
    Other = 0
    NewCustomers = 1
    ModifiedCustomers = 2
    Invoices = 3
    InvoiceSampleDetail = 4
    InvoicePayments = 5
    PayPointPayments = 6
  End Enum

  ''' <summary>
  ''' Corresponds to back page of diagnostic soil samples Fertilizer Grid
  ''' </summary>
  ''' <remarks>Maps to the FertApplied Method IDs</remarks>
  Public Enum FertilizerAppliedMethod As Integer
    Broadcast = 1
    RowBand = 2
    TopdressFoliar = 3
  End Enum

  Public Enum States As Integer
    NorthCarolina = 37
    OutOfState = 101
  End Enum

End Class
