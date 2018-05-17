<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMessageBox
  Inherits System.Windows.Forms.Form

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Me._messsage = New System.Windows.Forms.TextBox()
    Me.btnOk = New System.Windows.Forms.Button()
    Me.SuspendLayout()
    '
    '_messsage
    '
    Me._messsage.Location = New System.Drawing.Point(12, 12)
    Me._messsage.MaxLength = 200
    Me._messsage.Multiline = True
    Me._messsage.Name = "_messsage"
    Me._messsage.ReadOnly = True
    Me._messsage.Size = New System.Drawing.Size(240, 40)
    Me._messsage.TabIndex = 3
    Me._messsage.TabStop = False
    '
    'btnOk
    '
    Me.btnOk.Location = New System.Drawing.Point(202, 58)
    Me.btnOk.Name = "btnOk"
    Me.btnOk.Size = New System.Drawing.Size(50, 23)
    Me.btnOk.TabIndex = 2
    Me.btnOk.Text = "Ok"
    Me.btnOk.UseVisualStyleBackColor = True
    '
    'frmMessageBox
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(261, 87)
    Me.Controls.Add(Me._messsage)
    Me.Controls.Add(Me.btnOk)
    Me.Name = "frmMessageBox"
    Me.Text = "frmMessageBox"
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub

  Friend WithEvents _messsage As TextBox
  Friend WithEvents btnOk As Button
End Class
