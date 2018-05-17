<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSoilICP
  Inherits LabAnalysis2.BasePage

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()>
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
  <System.Diagnostics.DebuggerStepThrough()>
  Private Sub InitializeComponent()
    Me.components = New System.ComponentModel.Container
    Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
    Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
    Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
    Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
    Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
    Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
    Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
    Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.CalibrationsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.mnuICPConfig = New System.Windows.Forms.ToolStripMenuItem
    Me.ExportResultsToExcelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.mnuCompare = New System.Windows.Forms.ToolStripMenuItem
    Me.btnSave = New System.Windows.Forms.Button
    Me.btnReset = New System.Windows.Forms.Button
    Me.btnClose = New System.Windows.Forms.Button
    Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
    Me.mnuConfirm = New System.Windows.Forms.ToolStripMenuItem
    Me.AddCommentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.MarkAsNoSoilToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
    Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
    Me.pnlHeader = New SharedLayer.HeaderPanel
    Me.btnSupervisorLogin = New System.Windows.Forms.Button
    Me.btnSendFile = New System.Windows.Forms.Button
    Me.chkXMLLoading = New System.Windows.Forms.CheckBox
    Me.btnNoSoil = New System.Windows.Forms.Button
    Me.nudFiscalYear = New System.Windows.Forms.NumericUpDown
    Me.Label1 = New System.Windows.Forms.Label
    Me.btnDeleteFile = New System.Windows.Forms.Button
    Me.btnAddComment = New System.Windows.Forms.Button
    Me.btnOpenFile = New System.Windows.Forms.Button
    Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
    Me.TabControl1 = New System.Windows.Forms.TabControl
    Me.tbResults = New System.Windows.Forms.TabPage
    Me.dgvResults = New System.Windows.Forms.DataGridView
    Me.tbCompare = New System.Windows.Forms.TabPage
    Me.dgvCompare = New System.Windows.Forms.DataGridView
    Me.watcher = New System.IO.FileSystemWatcher
    Me.mnuOverwriteICPData = New System.Windows.Forms.ToolStripMenuItem
    Me.MenuStrip1.SuspendLayout()
    Me.ContextMenuStrip1.SuspendLayout()
    Me.pnlHeader.SuspendLayout()
    CType(Me.nudFiscalYear, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.TabControl1.SuspendLayout()
    Me.tbResults.SuspendLayout()
    CType(Me.dgvResults, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.tbCompare.SuspendLayout()
    CType(Me.dgvCompare, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.watcher, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'MenuStrip1
    '
    Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewToolStripMenuItem})
    Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
    Me.MenuStrip1.MdiWindowListItem = Me.ViewToolStripMenuItem
    Me.MenuStrip1.Name = "MenuStrip1"
    Me.MenuStrip1.Size = New System.Drawing.Size(877, 24)
    Me.MenuStrip1.TabIndex = 45
    Me.MenuStrip1.Text = "MenuStrip1"
    '
    'ViewToolStripMenuItem
    '
    Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CalibrationsToolStripMenuItem, Me.mnuICPConfig, Me.ExportResultsToExcelToolStripMenuItem, Me.mnuCompare, Me.mnuOverwriteICPData})
    Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
    Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(77, 20)
    Me.ViewToolStripMenuItem.Text = "ICP Station"
    '
    'CalibrationsToolStripMenuItem
    '
    Me.CalibrationsToolStripMenuItem.Name = "CalibrationsToolStripMenuItem"
    Me.CalibrationsToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
    Me.CalibrationsToolStripMenuItem.Text = "View &Calibrations"
    '
    'mnuICPConfig
    '
    Me.mnuICPConfig.Name = "mnuICPConfig"
    Me.mnuICPConfig.Size = New System.Drawing.Size(190, 22)
    Me.mnuICPConfig.Text = "ICP Configuration"
    '
    'ExportResultsToExcelToolStripMenuItem
    '
    Me.ExportResultsToExcelToolStripMenuItem.Name = "ExportResultsToExcelToolStripMenuItem"
    Me.ExportResultsToExcelToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
    Me.ExportResultsToExcelToolStripMenuItem.Text = "Export Results to Excel"
    '
    'mnuCompare
    '
    Me.mnuCompare.Name = "mnuCompare"
    Me.mnuCompare.ShortcutKeys = System.Windows.Forms.Keys.F2
    Me.mnuCompare.Size = New System.Drawing.Size(190, 22)
    Me.mnuCompare.Text = "Send File To ICP"
    '
    'btnSave
    '
    Me.btnSave.BackColor = System.Drawing.SystemColors.Control
    Me.btnSave.Location = New System.Drawing.Point(164, 12)
    Me.btnSave.Name = "btnSave"
    Me.btnSave.Size = New System.Drawing.Size(135, 21)
    Me.btnSave.TabIndex = 48
    Me.btnSave.Text = "&Transfer to LIMS"
    Me.btnSave.UseVisualStyleBackColor = True
    '
    'btnReset
    '
    Me.btnReset.BackColor = System.Drawing.SystemColors.Control
    Me.btnReset.Location = New System.Drawing.Point(446, 12)
    Me.btnReset.Name = "btnReset"
    Me.btnReset.Size = New System.Drawing.Size(135, 21)
    Me.btnReset.TabIndex = 47
    Me.btnReset.Text = "&Reset"
    Me.btnReset.UseVisualStyleBackColor = True
    '
    'btnClose
    '
    Me.btnClose.BackColor = System.Drawing.SystemColors.Control
    Me.btnClose.Location = New System.Drawing.Point(728, 12)
    Me.btnClose.Name = "btnClose"
    Me.btnClose.Size = New System.Drawing.Size(135, 21)
    Me.btnClose.TabIndex = 13
    Me.btnClose.Text = "&Exit"
    Me.btnClose.UseVisualStyleBackColor = True
    '
    'ContextMenuStrip1
    '
    Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuConfirm, Me.AddCommentToolStripMenuItem, Me.MarkAsNoSoilToolStripMenuItem})
    Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
    Me.ContextMenuStrip1.Size = New System.Drawing.Size(157, 70)
    '
    'mnuConfirm
    '
    Me.mnuConfirm.Name = "mnuConfirm"
    Me.mnuConfirm.Size = New System.Drawing.Size(156, 22)
    Me.mnuConfirm.Text = "Confirm"
    Me.mnuConfirm.ToolTipText = "Confirm selected rows"
    '
    'AddCommentToolStripMenuItem
    '
    Me.AddCommentToolStripMenuItem.Name = "AddCommentToolStripMenuItem"
    Me.AddCommentToolStripMenuItem.Size = New System.Drawing.Size(156, 22)
    Me.AddCommentToolStripMenuItem.Text = "Add Comment"
    '
    'MarkAsNoSoilToolStripMenuItem
    '
    Me.MarkAsNoSoilToolStripMenuItem.Name = "MarkAsNoSoilToolStripMenuItem"
    Me.MarkAsNoSoilToolStripMenuItem.Size = New System.Drawing.Size(156, 22)
    Me.MarkAsNoSoilToolStripMenuItem.Text = "Mark as No Soil"
    '
    'OpenFileDialog1
    '
    Me.OpenFileDialog1.FileName = "OpenFileDialog1"
    '
    'pnlHeader
    '
    Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(225, Byte), Integer), CType(CType(230, Byte), Integer), CType(CType(239, Byte), Integer))
    Me.pnlHeader.Controls.Add(Me.btnSupervisorLogin)
    Me.pnlHeader.Controls.Add(Me.btnSendFile)
    Me.pnlHeader.Controls.Add(Me.chkXMLLoading)
    Me.pnlHeader.Controls.Add(Me.btnNoSoil)
    Me.pnlHeader.Controls.Add(Me.nudFiscalYear)
    Me.pnlHeader.Controls.Add(Me.Label1)
    Me.pnlHeader.Controls.Add(Me.btnDeleteFile)
    Me.pnlHeader.Controls.Add(Me.btnAddComment)
    Me.pnlHeader.Controls.Add(Me.btnReset)
    Me.pnlHeader.Controls.Add(Me.btnSave)
    Me.pnlHeader.Controls.Add(Me.btnOpenFile)
    Me.pnlHeader.Controls.Add(Me.btnClose)
    Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
    Me.pnlHeader.Location = New System.Drawing.Point(0, 24)
    Me.pnlHeader.Name = "pnlHeader"
    Me.pnlHeader.PaintTopBorder = False
    Me.pnlHeader.Size = New System.Drawing.Size(877, 91)
    Me.pnlHeader.TabIndex = 46
    '
    'btnSupervisorLogin
    '
    Me.btnSupervisorLogin.BackColor = System.Drawing.Color.Wheat
    Me.btnSupervisorLogin.Location = New System.Drawing.Point(728, 41)
    Me.btnSupervisorLogin.Name = "btnSupervisorLogin"
    Me.btnSupervisorLogin.Size = New System.Drawing.Size(135, 21)
    Me.btnSupervisorLogin.TabIndex = 59
    Me.btnSupervisorLogin.Text = "Supervisor Login"
    Me.btnSupervisorLogin.UseVisualStyleBackColor = False
    '
    'btnSendFile
    '
    Me.btnSendFile.BackColor = System.Drawing.SystemColors.Control
    Me.btnSendFile.Location = New System.Drawing.Point(164, 40)
    Me.btnSendFile.Name = "btnSendFile"
    Me.btnSendFile.Size = New System.Drawing.Size(135, 21)
    Me.btnSendFile.TabIndex = 58
    Me.btnSendFile.Text = "&Send File"
    Me.btnSendFile.UseVisualStyleBackColor = True
    '
    'chkXMLLoading
    '
    Me.chkXMLLoading.AutoSize = True
    Me.chkXMLLoading.Location = New System.Drawing.Point(23, 43)
    Me.chkXMLLoading.Name = "chkXMLLoading"
    Me.chkXMLLoading.Size = New System.Drawing.Size(111, 17)
    Me.chkXMLLoading.TabIndex = 57
    Me.chkXMLLoading.Text = "Real Time loading"
    Me.chkXMLLoading.UseVisualStyleBackColor = True
    Me.chkXMLLoading.Visible = False
    '
    'btnNoSoil
    '
    Me.btnNoSoil.BackColor = System.Drawing.SystemColors.Control
    Me.btnNoSoil.Location = New System.Drawing.Point(305, 40)
    Me.btnNoSoil.Name = "btnNoSoil"
    Me.btnNoSoil.Size = New System.Drawing.Size(135, 21)
    Me.btnNoSoil.TabIndex = 56
    Me.btnNoSoil.Text = "&No Soil"
    Me.btnNoSoil.UseVisualStyleBackColor = True
    '
    'nudFiscalYear
    '
    Me.nudFiscalYear.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.nudFiscalYear.Location = New System.Drawing.Point(602, 43)
    Me.nudFiscalYear.Maximum = New Decimal(New Integer() {2050, 0, 0, 0})
    Me.nudFiscalYear.Minimum = New Decimal(New Integer() {1950, 0, 0, 0})
    Me.nudFiscalYear.Name = "nudFiscalYear"
    Me.nudFiscalYear.Size = New System.Drawing.Size(120, 23)
    Me.nudFiscalYear.TabIndex = 55
    Me.nudFiscalYear.Value = New Decimal(New Integer() {1950, 0, 0, 0})
    '
    'Label1
    '
    Me.Label1.AutoSize = True
    Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.Label1.Location = New System.Drawing.Point(509, 45)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(86, 17)
    Me.Label1.TabIndex = 54
    Me.Label1.Text = "Fiscal Year: "
    '
    'btnDeleteFile
    '
    Me.btnDeleteFile.BackColor = System.Drawing.SystemColors.Control
    Me.btnDeleteFile.Location = New System.Drawing.Point(587, 12)
    Me.btnDeleteFile.Name = "btnDeleteFile"
    Me.btnDeleteFile.Size = New System.Drawing.Size(135, 21)
    Me.btnDeleteFile.TabIndex = 53
    Me.btnDeleteFile.Text = "&Delete File"
    Me.btnDeleteFile.UseVisualStyleBackColor = True
    '
    'btnAddComment
    '
    Me.btnAddComment.BackColor = System.Drawing.SystemColors.Control
    Me.btnAddComment.Location = New System.Drawing.Point(305, 12)
    Me.btnAddComment.Name = "btnAddComment"
    Me.btnAddComment.Size = New System.Drawing.Size(135, 21)
    Me.btnAddComment.TabIndex = 52
    Me.btnAddComment.Text = "&Add Comment"
    Me.btnAddComment.UseVisualStyleBackColor = True
    '
    'btnOpenFile
    '
    Me.btnOpenFile.BackColor = System.Drawing.SystemColors.Control
    Me.btnOpenFile.Location = New System.Drawing.Point(23, 12)
    Me.btnOpenFile.Name = "btnOpenFile"
    Me.btnOpenFile.Size = New System.Drawing.Size(135, 21)
    Me.btnOpenFile.TabIndex = 23
    Me.btnOpenFile.Text = "&Open File"
    Me.btnOpenFile.UseVisualStyleBackColor = True
    '
    'TabControl1
    '
    Me.TabControl1.Controls.Add(Me.tbResults)
    Me.TabControl1.Controls.Add(Me.tbCompare)
    Me.TabControl1.Location = New System.Drawing.Point(1, 115)
    Me.TabControl1.Name = "TabControl1"
    Me.TabControl1.SelectedIndex = 0
    Me.TabControl1.Size = New System.Drawing.Size(876, 357)
    Me.TabControl1.TabIndex = 50
    '
    'tbResults
    '
    Me.tbResults.Controls.Add(Me.dgvResults)
    Me.tbResults.Location = New System.Drawing.Point(4, 22)
    Me.tbResults.Name = "tbResults"
    Me.tbResults.Padding = New System.Windows.Forms.Padding(3)
    Me.tbResults.Size = New System.Drawing.Size(868, 331)
    Me.tbResults.TabIndex = 0
    Me.tbResults.Text = "Results"
    Me.tbResults.UseVisualStyleBackColor = True
    '
    'dgvResults
    '
    Me.dgvResults.AllowUserToAddRows = False
    Me.dgvResults.AllowUserToDeleteRows = False
    Me.dgvResults.AllowUserToOrderColumns = True
    Me.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
    Me.dgvResults.CausesValidation = False
    DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
    DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
    DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
    DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
    DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
    DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
    Me.dgvResults.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
    Me.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
    Me.dgvResults.ContextMenuStrip = Me.ContextMenuStrip1
    DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
    DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
    DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
    DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
    DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
    DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
    Me.dgvResults.DefaultCellStyle = DataGridViewCellStyle2
    Me.dgvResults.Location = New System.Drawing.Point(3, 4)
    Me.dgvResults.Name = "dgvResults"
    Me.dgvResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
    DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
    Me.dgvResults.RowsDefaultCellStyle = DataGridViewCellStyle3
    Me.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
    Me.dgvResults.Size = New System.Drawing.Size(862, 147)
    Me.dgvResults.TabIndex = 49
    '
    'tbCompare
    '
    Me.tbCompare.Controls.Add(Me.dgvCompare)
    Me.tbCompare.Location = New System.Drawing.Point(4, 22)
    Me.tbCompare.Name = "tbCompare"
    Me.tbCompare.Padding = New System.Windows.Forms.Padding(3)
    Me.tbCompare.Size = New System.Drawing.Size(868, 331)
    Me.tbCompare.TabIndex = 1
    Me.tbCompare.Text = "Compare"
    Me.tbCompare.UseVisualStyleBackColor = True
    '
    'dgvCompare
    '
    Me.dgvCompare.AllowUserToAddRows = False
    Me.dgvCompare.AllowUserToDeleteRows = False
    Me.dgvCompare.AllowUserToOrderColumns = True
    Me.dgvCompare.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
    DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
    DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
    DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
    DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
    DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
    DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
    Me.dgvCompare.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
    Me.dgvCompare.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
    Me.dgvCompare.ContextMenuStrip = Me.ContextMenuStrip1
    DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
    DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
    DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
    DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
    DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
    DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
    Me.dgvCompare.DefaultCellStyle = DataGridViewCellStyle5
    Me.dgvCompare.Location = New System.Drawing.Point(3, 4)
    Me.dgvCompare.Name = "dgvCompare"
    DataGridViewCellStyle6.BackColor = System.Drawing.Color.White
    Me.dgvCompare.RowsDefaultCellStyle = DataGridViewCellStyle6
    Me.dgvCompare.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
    Me.dgvCompare.Size = New System.Drawing.Size(862, 324)
    Me.dgvCompare.TabIndex = 50
    '
    'watcher
    '
    Me.watcher.EnableRaisingEvents = True
    Me.watcher.SynchronizingObject = Me
    '
    'mnuOverwriteICPData
    '
    Me.mnuOverwriteICPData.Name = "mnuOverwriteICPData"
    Me.mnuOverwriteICPData.Size = New System.Drawing.Size(190, 22)
    Me.mnuOverwriteICPData.Text = "Overwrite ICP Data"
    '
    'frmSoilICP
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(877, 484)
    Me.Controls.Add(Me.pnlHeader)
    Me.Controls.Add(Me.MenuStrip1)
    Me.Controls.Add(Me.TabControl1)
    Me.DoubleBuffered = True
    Me.Name = "frmSoilICP"
    Me.Text = "ICP"
    Me.MenuStrip1.ResumeLayout(False)
    Me.MenuStrip1.PerformLayout()
    Me.ContextMenuStrip1.ResumeLayout(False)
    Me.pnlHeader.ResumeLayout(False)
    Me.pnlHeader.PerformLayout()
    CType(Me.nudFiscalYear, System.ComponentModel.ISupportInitialize).EndInit()
    Me.TabControl1.ResumeLayout(False)
    Me.tbResults.ResumeLayout(False)
    CType(Me.dgvResults, System.ComponentModel.ISupportInitialize).EndInit()
    Me.tbCompare.ResumeLayout(False)
    CType(Me.dgvCompare, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.watcher, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
  Friend WithEvents ViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents CalibrationsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuICPConfig As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents pnlHeader As SharedLayer.HeaderPanel
  Friend WithEvents btnSave As System.Windows.Forms.Button
  Friend WithEvents btnReset As System.Windows.Forms.Button
  Friend WithEvents btnClose As System.Windows.Forms.Button
  Friend WithEvents btnOpenFile As System.Windows.Forms.Button
  Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
  Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
  Friend WithEvents btnDeleteFile As System.Windows.Forms.Button
  Friend WithEvents btnAddComment As System.Windows.Forms.Button
  Friend WithEvents ExportResultsToExcelToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents nudFiscalYear As System.Windows.Forms.NumericUpDown
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents mnuCompare As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents btnNoSoil As System.Windows.Forms.Button
  Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
  Friend WithEvents mnuConfirm As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents AddCommentToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents MarkAsNoSoilToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents chkXMLLoading As System.Windows.Forms.CheckBox
  Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
  Friend WithEvents tbResults As System.Windows.Forms.TabPage
  Friend WithEvents dgvResults As System.Windows.Forms.DataGridView
  Friend WithEvents tbCompare As System.Windows.Forms.TabPage
  Friend WithEvents dgvCompare As System.Windows.Forms.DataGridView
  Friend WithEvents watcher As System.IO.FileSystemWatcher
  Friend WithEvents btnSendFile As System.Windows.Forms.Button
  Friend WithEvents btnSupervisorLogin As System.Windows.Forms.Button
  Friend WithEvents mnuOverwriteICPData As System.Windows.Forms.ToolStripMenuItem
End Class
