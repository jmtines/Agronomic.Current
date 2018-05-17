Public Class frmMessageBox

  Friend Event frm_Closing(ByVal buttonOkClicked As Boolean)
  Public Sub New(ByVal message As String, ByVal caption As String)
    ' This call is required by the designer.
    InitializeComponent()

    _messsage.Text = message
    Text = caption
  End Sub
  Private Sub frmMessageBox_Click() Handles btnOk.Click
    RaiseEvent frm_Closing(True)
    Close()
  End Sub

  Private Sub frmMessageBox_Closing() Handles Me.Closing
    RaiseEvent frm_Closing(False)
  End Sub
End Class