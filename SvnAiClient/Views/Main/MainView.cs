using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using SvnAiClient.Core.Settings;
using SvnAiClient.Core.Svn;
using SvnAiClient.Helpers;
using SvnAiClient.Models;
using SvnAiClient.Views.AddWorkingCopy;
using SvnAiClient.Views.AiReviewHistory;
using SvnAiClient.Views.Commit;
using SvnAiClient.Views.Common;
using SvnAiClient.Views.DiffViewer;
using SvnAiClient.Views.LogViewer;
using SvnAiClient.Views.Settings;

namespace SvnAiClient.Views.Main
{
    /// <summary>작업 복사본 탐색과 파일 선택을 표시하고 메인 화면의 사용자 요청을 전달한다.</summary>
    public partial class MainView : ThemedView, MainInterface
    {
        #region Fields

        /// <summary>작업 복사본과 SVN 처리 흐름을 위임하는 Presenter.</summary>
        private readonly MainPresenter _presenter;

        /// <summary>프로그램에서 수명을 관리하며 하위 화면에도 공유하는 앱 전용 글꼴.</summary>
        private readonly ApplicationFonts _applicationFonts;

        /// <summary>다중 선택과 구분하여 보관하는 우클릭 당시 파일 로그 대상 경로.</summary>
        private string _contextLogFilePath;

        /// <summary>메인 화면 배치를 업무 설정과 분리하여 저장하는 서비스.</summary>
        private readonly MainViewLayoutService _layoutService = new MainViewLayoutService();

        /// <summary>열 생성과 배치 복원이 끝나 종료 시 저장할 수 있는지 여부.</summary>
        private bool _layoutReady;

        #endregion

        #region Properties

        /// <summary>화면에 조회 결과를 표시할 수 있는지 여부.</summary>
        public bool IsAvailable { get { return !IsDisposed && !Disposing; } }

        /// <summary>변경된 항목만 표시하는 필터의 활성화 여부.</summary>
        public bool OnlyChanged { get { return chkOnlyChanged.Checked; } }

        /// <summary>변경 필터에서 하위 폴더의 파일까지 표시할지 여부.</summary>
        public bool IncludeSubfolders { get { return chkIncludeSubfolders.Checked; } }

        /// <summary>파일 목록에서 선택한 파일들. 폴더는 제외한다.</summary>
        public List<SvnFileStatus> SelectedFiles { get { return GetSelectedFiles(); } }

        /// <summary>현재 파일 그리드에 바인딩된 표시 객체 목록.</summary>
        public List<SvnFileStatus> DisplayedFiles { get { return gridFiles.DataSource as List<SvnFileStatus>; } }

        #endregion

        #region Initialization

        public MainView()
        {
            _presenter = new MainPresenter(this, new SettingsService());
            InitializeComponent();
            Disposed += View_Disposed;
        }

        public MainView(ApplicationFonts fonts) : this()
        {
            _applicationFonts = fonts;
            InitializeFonts(fonts);
        }

        private void InitializeFonts(ApplicationFonts fonts)
        {
            Font = fonts.GetFont(10F);
            lblFolderTitle.Font = fonts.GetFont(10F, FontStyle.Bold);
            lblCommitTargets.Font = fonts.GetFont(10F, FontStyle.Bold);
            lblCurrentPath.Font = fonts.GetFont(10F);
            lblFileSelection.Font = fonts.GetFont(10F);
            lblFilesEmpty.Font = fonts.GetFont(10F);
            treeWorkingCopy.Appearance.Row.Font = fonts.GetFont(10F);
            treeWorkingCopy.Appearance.HeaderPanel.Font = fonts.GetFont(10F);
            gridViewFiles.Appearance.Row.Font = fonts.GetFont(10F);
            gridViewFiles.Appearance.HeaderPanel.Font = fonts.GetFont(10F);
            gridViewFiles.Appearance.Empty.Font = fonts.GetFont(10F);
            gridViewCommitTarget.Appearance.Row.Font = fonts.GetFont(10F);
            gridViewCommitTarget.Appearance.HeaderPanel.Font = fonts.GetFont(10F);
            gridViewCommitTarget.Appearance.Empty.Font = fonts.GetFont(10F);
            chkOnlyChanged.Properties.Appearance.Font = fonts.GetFont(10F);
            chkIncludeSubfolders.Properties.Appearance.Font = fonts.GetFont(10F);
            txtCurrentPath.Properties.Appearance.Font = fonts.GetFont(10F);
            popupFiles.Font = fonts.GetFont(10F);
            barStatus.BarAppearance.Normal.Font = fonts.GetFont(10F);
            barToolbar.BarAppearance.Normal.Font = fonts.GetFont(9F);
            barWorkingCopy.BarAppearance.Normal.Font = fonts.GetFont(9F);
            barItemWorkingCopyScope.ItemAppearance.Normal.Font = fonts.GetFont(9F, FontStyle.Bold);
            cboWorkingCopyRepo.Appearance.Font = fonts.GetFont(9F);
            cboWorkingCopyRepo.AppearanceDropDown.Font = fonts.GetFont(9F);
            btnLog.Appearance.Font = fonts.GetFont(9F);
            btnRevert.Appearance.Font = fonts.GetFont(9F);
            btnRemoveCommitTarget.Appearance.Font = fonts.GetFont(9F);
            btnCommit.Appearance.Font = fonts.GetFont(9F, FontStyle.Bold);
        }

        private void AddEventHandlers()
        {
            this.chkOnlyChanged.CheckedChanged += new EventHandler(this.chkOnlyChanged_CheckedChanged);
            this.chkIncludeSubfolders.CheckedChanged +=
                new EventHandler(this.chkIncludeSubfolders_CheckedChanged);
            gridViewFiles.SelectionChanged += GridSelection_Changed;
            gridViewFiles.FocusedRowChanged += GridSelection_Changed;
            gridViewCommitTarget.FocusedRowChanged += GridSelection_Changed;
        }

        private void InitFolderTree()
        {
            treeWorkingCopy.Columns.Clear();
            var col = treeWorkingCopy.Columns.Add();
            col.Caption = "폴더";
            col.VisibleIndex = 0;
            col.Width = 220;

            treeWorkingCopy.OptionsView.ShowColumns = false;
            treeWorkingCopy.OptionsView.ShowIndicator = false;
            treeWorkingCopy.OptionsBehavior.Editable = false;
            treeWorkingCopy.CustomDrawNodeCell += TreeWorkingCopy_CustomDrawNodeCell;
            treeWorkingCopy.FocusedNodeChanged += TreeWorkingCopy_FocusedNodeChanged;
        }

        private void InitFilesGrid()
        {
            gridViewFiles.Columns.Clear();

            var colCheck = gridViewFiles.Columns.Add();
            colCheck.FieldName = "IsChecked";
            colCheck.Caption = "커밋";
            colCheck.Width = 52;
            colCheck.OptionsColumn.FixedWidth = true;
            colCheck.VisibleIndex = 0;
            colCheck.OptionsColumn.AllowEdit = true; // 체크박스는 편집 가능
            var checkEditor =
                new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();

            checkEditor.AllowGrayed = false;

            // 체크값이 변경되는 즉시 그리드에 반영한다.
            checkEditor.EditValueChanged += CommitCheckEditor_EditValueChanged;

            gridFiles.RepositoryItems.Add(checkEditor);
            colCheck.ColumnEdit = checkEditor;

            var colIcon = gridViewFiles.Columns.Add();
            colIcon.FieldName = "StatusText";
            colIcon.Caption = "상태";
            colIcon.Width = 160;
            colIcon.MinWidth = 120;
            colIcon.VisibleIndex = 1; // 추가
            colIcon.OptionsColumn.AllowEdit = false;

            var colType = gridViewFiles.Columns.Add();
            colType.FieldName = "FileType";
            colType.Caption = "유형";
            colType.Width = 60;
            colType.VisibleIndex = 2; // 상태 다음
            colType.OptionsColumn.AllowEdit = false;

            var colName = gridViewFiles.Columns.Add();
            colName.FieldName = "FileName";
            colName.Caption = "이름";
            colName.Width = 250;
            colName.VisibleIndex = 3; // 추가
            colName.OptionsColumn.AllowEdit = false;

            var colPath = gridViewFiles.Columns.Add();
            colPath.FieldName = "RelativePath";
            colPath.Caption = "경로";
            colPath.Width = 350;
            colPath.VisibleIndex = 4; // 추가
            colPath.OptionsColumn.AllowEdit = false;

            var colDate = gridViewFiles.Columns.Add();
            colDate.FieldName = "LastModified";
            colDate.Caption = "수정일시";
            colDate.Width = 170;
            colDate.VisibleIndex = 5; // 추가
            colDate.OptionsColumn.AllowEdit = false;
            colDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colDate.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";

            gridViewFiles.OptionsBehavior.Editable = true;
            gridViewFiles.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDown;
            gridViewFiles.OptionsView.ShowGroupPanel = false;
            gridViewFiles.OptionsSelection.MultiSelect = true;

            gridViewFiles.DoubleClick += GridViewFiles_DoubleClick;
            gridViewFiles.CustomDrawCell += GridViewFiles_CustomDrawCell;
            gridViewFiles.CellValueChanged += GridViewFiles_CellValueChanged;
            gridViewFiles.ShowingEditor += GridViewFiles_ShowingEditor;
            gridViewFiles.CustomColumnDisplayText += Grid_CustomColumnDisplayText;
            gridViewFiles.ColumnFilterChanged += GridFiles_ColumnFilterChanged;
        }

        private void InitCommitTargetGrid()
        {
            gridViewCommitTarget.Columns.Clear();

            var colStatus = gridViewCommitTarget.Columns.Add();
            colStatus.FieldName = "StatusText";
            colStatus.Caption = "상태";
            colStatus.Width = 160;
            colStatus.MinWidth = 120;
            colStatus.VisibleIndex = 0;
            colStatus.OptionsColumn.AllowEdit = false;

            var colName = gridViewCommitTarget.Columns.Add();
            colName.FieldName = "FileName";
            colName.Caption = "파일명";
            colName.Width = 200;
            colName.VisibleIndex = 1;
            colName.OptionsColumn.AllowEdit = false;

            var colPath = gridViewCommitTarget.Columns.Add();
            colPath.FieldName = "RelativePath";
            colPath.Caption = "경로";
            colPath.Width = 500;
            colPath.VisibleIndex = 2;
            colPath.OptionsColumn.AllowEdit = false;

            gridViewCommitTarget.OptionsBehavior.Editable = false;
            gridViewCommitTarget.OptionsView.ShowGroupPanel = false;
            gridViewCommitTarget.CustomColumnDisplayText += Grid_CustomColumnDisplayText;
            gridViewCommitTarget.CustomDrawCell += GridViewCommitTarget_CustomDrawCell;
            gridViewCommitTarget.KeyDown += GridViewCommitTarget_KeyDown;

            this.btnRemoveCommitTarget.Click += BtnRemoveCommitTarget_Click;
        }

        #endregion

        #region Event Handlers

        private void GridSelection_Changed(object sender, EventArgs e)
        {
            UpdateActionState();
        }

        private async void MainView_Load(object sender, EventArgs e)
        {
            AddEventHandlers();
            InitFilesGrid();
            InitCommitTargetGrid();
            InitFolderTree();
            RestoreLayout();
            _layoutReady = true;
            await _presenter.InitializeAsync();
            UpdateListHint();
        }

        private void chkOnlyChanged_CheckedChanged(object sender, EventArgs e)
        {
            chkIncludeSubfolders.Enabled = chkOnlyChanged.Checked;
            if (!chkOnlyChanged.Checked)
                chkIncludeSubfolders.Checked = false;
            _presenter.NavigateToFolder(_presenter.CurrentRelativePath);
        }

        private void chkIncludeSubfolders_CheckedChanged(object sender, EventArgs e)
        {
            _presenter.NavigateToFolder(_presenter.CurrentRelativePath);
        }

        private async void BarEditWorkingCopy_EditValueChanged(object sender, EventArgs e)
        {
            int index = cboWorkingCopyRepo.Items.IndexOf(barEditWorkingCopy.EditValue);
            await _presenter.SelectWorkingCopyAsync(index);
        }

        private void TreeWorkingCopy_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            var folderPath = e.Node.Tag as string;
            if (!string.IsNullOrWhiteSpace(folderPath) && _presenter.HasFolderChanges(folderPath))
            {
                e.Appearance.FontStyleDelta = FontStyle.Bold;
                if (e.Node != treeWorkingCopy.FocusedNode)
                    e.Appearance.ForeColor = Color.DarkGreen;
            }
        }

        private void TreeWorkingCopy_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            var path = e.Node?.Tag as string;
            if (path != null)
                _presenter.NavigateToFullPath(path);
        }

        private async void GridViewFiles_DoubleClick(object sender, EventArgs e)
        {
            var hit = gridViewFiles.CalcHitInfo(gridFiles.PointToClient(Cursor.Position));
            if (!hit.InRowCell || hit.RowHandle < 0 || hit.Column?.FieldName == "IsChecked")
                return;
            await _presenter.OpenItemAsync(gridViewFiles.GetRow(hit.RowHandle) as SvnFileStatus);
        }

        private async void btnUpdate_Click(object sender, ItemClickEventArgs e)
        {
            await _presenter.UpdateAsync();
        }

        private async void btnCommit_Click(object sender, EventArgs e)
        {
            await _presenter.RequestCommitAsync();
        }

        private async void btnRevert_Click(object sender, EventArgs e)
        {
            await _presenter.RevertSelectedAsync();
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            _presenter.OpenFolderLog();
        }

        private void menuViewFileLog_Click(object sender, EventArgs e)
        {
            string path = _contextLogFilePath;
            _contextLogFilePath = null;
            _presenter.OpenLogViewerForPath(path);
        }

        private async void btnRefresh_Click(object sender, ItemClickEventArgs e)
        {
            await _presenter.RefreshAsync();
        }

        private async void menuSettings_Click(object sender, ItemClickEventArgs e)
        {
            await _presenter.OpenSettingsAsync();
        }

        private async void btnAddWorkingCopy_Click(object sender, ItemClickEventArgs e)
        {
            await _presenter.AddWorkingCopyAsync();
        }

        private void btnRemoveWorkingCopy_Click(object sender, ItemClickEventArgs e)
        {
            _presenter.RemoveCurrentWorkingCopy();
        }

        private void GridViewCommitTarget_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveFromCommitTarget();
        }

        private void BtnRemoveCommitTarget_Click(object sender, EventArgs e)
        {
            RemoveFromCommitTarget();
        }

        private void GridViewFiles_CellValueChanged(
            object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName != "IsChecked") return;
            var file = gridViewFiles.GetRow(e.RowHandle) as SvnFileStatus;
            if (file == null) return;

            bool requested = object.Equals(e.Value, true);
            bool accepted = _presenter.SetCommitChecked(file, requested);
            if (requested && !accepted)
            {
                var editor = gridViewFiles.ActiveEditor;
                if (editor != null)
                    editor.EditValue = false;
                gridViewFiles.RefreshRow(e.RowHandle);
            }
        }

        private async void menuSvnAdd_Click(object sender, EventArgs e)
        {
            await _presenter.ScheduleSelectedFilesAsync(true);
        }

        private async void menuSvnDelete_Click(object sender, EventArgs e)
        {
            await _presenter.ScheduleSelectedFilesAsync(false);
        }

        private async void menuViewDiff_Click(object sender, EventArgs e)
        {
            await _presenter.OpenDiffViewerAsync(gridViewFiles.GetFocusedRow() as SvnFileStatus);
        }

        private void ReviewHistoryButton_Click(object sender, ItemClickEventArgs e)
        {
            _presenter.OpenReviewHistory();
        }

        private void View_Disposed(object sender, EventArgs e)
        {
            _presenter.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!e.Cancel)
            {
                SaveLayout();
                _presenter.BeginClose();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _presenter.Close();
            base.OnFormClosed(e);
        }

        private void CommitCheckEditor_EditValueChanged(object sender, EventArgs e)
        {
            gridViewFiles.PostEditor();
            gridViewFiles.UpdateCurrentRow();
        }

        private void GridViewFiles_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            var item = gridViewFiles.GetRow(e.RowHandle) as SvnFileStatus;
            if (item == null) return;
            if (e.Column.FieldName == "IsChecked" && (item.IsDirectory || item.FileName == ".."))
            {
                e.Appearance.DrawBackground(e.Cache, e.Bounds);
                e.Handled = true;
                return;
            }

            ApplyStatusAppearance(gridViewFiles, item, e);
        }

        private void GridViewCommitTarget_CustomDrawCell(
            object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            var item = gridViewCommitTarget.GetRow(e.RowHandle) as SvnFileStatus;
            if (item != null) ApplyStatusAppearance(gridViewCommitTarget, item, e);
        }

        private void GridViewFiles_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var item = gridViewFiles.GetFocusedRow() as SvnFileStatus;
            if (item == null || item.IsDirectory || item.FileName == ".." || _presenter.IsBusy)
                e.Cancel = true;
        }

        private void Grid_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "LastModified" && e.Value is DateTime
                && (DateTime)e.Value == DateTime.MinValue)
            {
                e.DisplayText = string.Empty;
                return;
            }

            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null || e.Column.FieldName != "StatusText" || e.ListSourceRowIndex < 0) return;
            var item = view.GetRow(view.GetRowHandle(e.ListSourceRowIndex)) as SvnFileStatus;
            if (item != null) e.DisplayText = GetStatusDisplayText(item);
        }

        private void GridFiles_ColumnFilterChanged(object sender, EventArgs e)
        {
            UpdateActionState();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                if (btnRefresh.Enabled) btnRefresh_Click(this, null);
                return true;
            }
            if (keyData == (Keys.Control | Keys.F))
            {
                if (!_presenter.IsBusy && _presenter.HasWorkingCopy) gridViewFiles.ShowFindPanel();
                return true;
            }
            if (keyData == Keys.Escape && !string.IsNullOrEmpty(gridViewFiles.FindFilterText))
            {
                gridViewFiles.FindFilterText = string.Empty;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void treeWorkingCopy_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || _presenter.IsBusy || _presenter.IsClosing) return;
            popupTree.ShowPopup(barManager, treeWorkingCopy.PointToScreen(e.Location));
        }

        private void gridFiles_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            _contextLogFilePath = null;
            menuViewFileLog.Enabled = false;

            if (!_presenter.HasWorkingCopy || _presenter.IsBusy || _presenter.IsClosing ||
                !btnUpdate.Enabled)
            {
                return;
            }

            gridViewFiles.PostEditor();
            gridViewFiles.UpdateCurrentRow();

            var hit = gridViewFiles.CalcHitInfo(e.Location);

            if (!hit.InRow || hit.RowHandle < 0)
                return;

            var clickedFile =
                gridViewFiles.GetRow(hit.RowHandle) as SvnFileStatus;

            if (clickedFile == null)
                return;

            // 로그 대상은 우클릭한 행 하나이며, 다중 선택과 별도로 보관한다.
            if (MainPresenter.CanViewFileLog(clickedFile))
            {
                _contextLogFilePath = clickedFile.FullPath;
                menuViewFileLog.Enabled = true;
            }

            // 이미 선택된 행을 우클릭하면 기존 다중 선택을 유지한다.
            if (!gridViewFiles.IsRowSelected(hit.RowHandle))
            {
                gridViewFiles.ClearSelection();
                gridViewFiles.SelectRow(hit.RowHandle);
            }

            gridViewFiles.FocusedRowHandle = hit.RowHandle;

            var files = GetSelectedFiles();

            menuSvnAdd.Enabled =
                files.Count > 0 &&
                files.All(file =>
                    file.Status == SvnStatusKind.Unversioned);

            menuSvnDelete.Enabled =
                files.Count > 0 &&
                files.All(file =>
                    file.Status == SvnStatusKind.Normal ||
                    file.Status == SvnStatusKind.Modified ||
                    file.Status == SvnStatusKind.Missing);

            menuViewDiff.Enabled = MainPresenter.CanViewDiff(clickedFile);

            popupFiles.Show(gridFiles, e.Location);
        }

        #endregion

        #region Control Methods

        public void RebuildFolderTree(WorkingCopyEntry entry, string relativePath)
        {
            bool firstLoad = treeWorkingCopy.Nodes.Count == 0;

            var expandedPaths = new HashSet<string>(
                GetExpandedFolderPaths(treeWorkingCopy.Nodes),
                StringComparer.OrdinalIgnoreCase);

            var selectedPath = entry == null
                ? null
                : Path.Combine(entry.LocalPath, relativePath);

            treeWorkingCopy.FocusedNodeChanged -=
                TreeWorkingCopy_FocusedNodeChanged;

            treeWorkingCopy.BeginUpdate();

            try
            {
                TreeListNode root = null;

                treeWorkingCopy.BeginUnboundLoad();

                try
                {
                    treeWorkingCopy.Nodes.Clear();

                    if (entry == null)
                        return;

                    root = treeWorkingCopy.AppendNode(
                        new object[] { entry.GetDisplayName() }, -1);

                    root.Tag = entry.LocalPath;
                    AddFolderNodes(root, entry.LocalPath);
                }
                finally
                {
                    treeWorkingCopy.EndUnboundLoad();
                }

                RestoreFolderExpansion(treeWorkingCopy.Nodes, expandedPaths);

                // 최초 로드에서는 루트의 바로 아래 폴더까지만 표시한다.
                if (firstLoad)
                    root.Expanded = true;

                var selected =
                    FindNodeByPath(treeWorkingCopy.Nodes, selectedPath) ?? root;

                // 새로고침 후 현재 폴더가 보이도록 부모 경로만 펼친다.
                for (var parent = selected.ParentNode;
                     parent != null;
                     parent = parent.ParentNode)
                {
                    parent.Expanded = true;
                }

                treeWorkingCopy.FocusedNode = selected;
            }
            finally
            {
                treeWorkingCopy.EndUpdate();

                treeWorkingCopy.FocusedNodeChanged +=
                    TreeWorkingCopy_FocusedNodeChanged;
            }
        }

        private void AddFolderNodes(TreeListNode parentNode, string dirPath)
        {
            try
            {
                foreach (var dir in Directory.GetDirectories(dirPath))
                {
                    var name = Path.GetFileName(dir);
                    if (string.Equals(name, ".svn", StringComparison.OrdinalIgnoreCase)
                        || !_presenter.ShouldShowFolder(dir)) continue;

                    var node = treeWorkingCopy.AppendNode(
                        new object[] { name }, parentNode);
                    node.Tag = dir;

                    AddFolderNodes(node, dir);
                }
            }
            catch
            {
                /* 접근 불가 폴더 무시 */
            }
        }

        public void SyncTreeToPath(string fullPath)
        {
            txtCurrentPath.Text = fullPath ?? string.Empty;
            txtCurrentPath.ToolTip = fullPath ?? string.Empty;
            var node = FindNodeByPath(treeWorkingCopy.Nodes, fullPath);
            if (node != null)
            {
                treeWorkingCopy.FocusedNodeChanged -= TreeWorkingCopy_FocusedNodeChanged;
                treeWorkingCopy.FocusedNode = node;
                treeWorkingCopy.FocusedNodeChanged += TreeWorkingCopy_FocusedNodeChanged;
            }
        }

        public void SetBusy(bool busy)
        {
            btnSettings.Enabled = !busy;
            btnReviewHistory.Enabled = !busy;
            btnAddWorkingCopy.Enabled = !busy;
            btnRemoveWorkingCopy.Enabled = !busy;
            barEditWorkingCopy.Enabled = !busy;
            treeWorkingCopy.Enabled = !busy;
            gridFiles.Enabled = !busy;
            gridCommitTarget.Enabled = !busy;
            UpdateActionState();
        }

        public void SetStatusText(string message)
        {
            if (_presenter.IsClosing || IsDisposed || Disposing || !IsHandleCreated)
                return;
            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() => SetStatusText(message)));
                }
                catch (InvalidOperationException)
                {
                    // 폼 종료와 진행 알림이 겹친 경우 알림을 폐기한다.
                }

                return;
            }

            barItemOperation.Caption = message;
        }

        public void SetWorkingCopies(List<string> names, int selectedIndex)
        {
            barEditWorkingCopy.EditValueChanged -= BarEditWorkingCopy_EditValueChanged;
            try
            {
                barEditWorkingCopy.EditValue = null;
                cboWorkingCopyRepo.Items.Clear();
                cboWorkingCopyRepo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                foreach (string name in names)
                    cboWorkingCopyRepo.Items.Add(name);
                barEditWorkingCopy.EditValue = selectedIndex >= 0 && selectedIndex < names.Count
                    ? cboWorkingCopyRepo.Items[selectedIndex] : null;
            }
            finally
            {
                barEditWorkingCopy.EditValueChanged += BarEditWorkingCopy_EditValueChanged;
            }
            UpdateActionState();
        }

        public void UpdateWorkingCopyRoot(WorkingCopyEntry entry)
        {
            if (treeWorkingCopy.Nodes.Count == 0) return;
            treeWorkingCopy.Nodes[0].SetValue(0, entry.GetDisplayName());
            treeWorkingCopy.Nodes[0].Tag = entry.LocalPath;
            txtCurrentPath.Text = Path.Combine(entry.LocalPath, _presenter.CurrentRelativePath);
            txtCurrentPath.ToolTip = txtCurrentPath.Text;
        }

        public void PostFileEdits()
        {
            gridViewFiles.PostEditor();
            gridViewFiles.UpdateCurrentRow();
        }

        public void ShowFiles(List<SvnFileStatus> files)
        {
            if (gridViewFiles.Columns.Count == 0)
                InitFilesGrid();
            gridFiles.DataSource = files;
            UpdateActionState();
        }

        public void ShowCommitTargets(List<SvnFileStatus> targets)
        {
            gridCommitTarget.DataSource = null;
            gridCommitTarget.DataSource = targets;
            UpdateActionState();
        }

        public void RefreshFiles()
        {
            gridFiles.RefreshDataSource();
            UpdateActionState();
        }

        public void ClearWorkspace()
        {
            gridViewFiles.FindFilterText = string.Empty;
            gridFiles.DataSource = null;
            treeWorkingCopy.Nodes.Clear();
            txtCurrentPath.Text = string.Empty;
            txtCurrentPath.ToolTip = string.Empty;
            barItemOperation.Caption = "준비 완료";
            UpdateActionState();
        }

        public void InvalidateTree()
        {
            treeWorkingCopy.Invalidate();
        }

        public void SetStatusBar(string provider, string repository)
        {
            barItemProvider.Caption = "AI: " + provider;
            barItemRepoUrl.Caption = repository;
            barItemRepoUrl.Hint = repository;
        }

        public void ShowMessage(string message, string title, MainNotice kind)
        {
            var icon = kind == MainNotice.Error ? MessageBoxIcon.Error
                : kind == MainNotice.Warning ? MessageBoxIcon.Warning : MessageBoxIcon.Information;
            XtraMessageBox.Show(this, message, title, MessageBoxButtons.OK, icon);
        }

        public bool Confirm(string message, string title, bool warning)
        {
            return XtraMessageBox.Show(this, message, title, MessageBoxButtons.YesNo,
                warning ? MessageBoxIcon.Warning : MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public WorkingCopyEntry CreateWorkingCopy()
        {
            using (var dialog = new AddWorkingCopyView(null, _applicationFonts))
            {
                return dialog.ShowDialog(this) == DialogResult.OK ? dialog.Result : null;
            }
        }

        public AppSettings EditSettings(SettingsService service, AppSettings settings)
        {
            using (var dialog = new SettingsView(service, settings, _applicationFonts))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return null;
                if (dialog.SavedSettings == null)
                    ShowMessage("저장된 설정을 가져올 수 없습니다.", "오류", MainNotice.Error);
                return dialog.SavedSettings;
            }
        }

        public MainCommitRequest EditCommit(List<SvnFileStatus> targets,
            AppSettings settings, SvnService service, string workingCopyPath)
        {
            using (var dialog = new CommitView(targets, settings, service, workingCopyPath, _applicationFonts))
            {
                try
                {
                    if (dialog.ShowDialog(this) != DialogResult.OK) return null;
                    return new MainCommitRequest
                    {
                        FilePaths = dialog.GetCheckedFilePaths(),
                        Message = dialog.CommitMessage,
                        ApprovedFiles = dialog.GetApprovedCommitFiles(),
                        ReviewHistoryId = dialog.GetReviewHistoryId()
                    };
                }
                finally
                {
                    gridFiles.RefreshDataSource();
                    gridCommitTarget.RefreshDataSource();
                    UpdateActionState();
                }
            }
        }

        public void ShowLogViewer(string path, SvnService service)
        {
            using (var dialog = new LogViewerView(path, service, _applicationFonts))
                dialog.ShowDialog(this);
        }

        public void ShowReviewHistory(string workingCopyPath)
        {
            using (var dialog = new AiReviewHistoryView(workingCopyPath, _applicationFonts))
                dialog.ShowDialog(this);
        }

        public void ShowDiff(string path, string diff)
        {
            using (var dialog = new DiffViewerView(_applicationFonts))
            {
                dialog.SetDiff(path, diff);
                dialog.ShowDialog(this);
            }
        }

        private void RemoveFromCommitTarget()
        {
            PostFileEdits();
            _presenter.RemoveCommitTarget(gridViewCommitTarget.GetFocusedRow() as SvnFileStatus);
        }

        #endregion

        #region Operations

        /// <summary>선택 파일 수와 누적 커밋 대상 수를 구분하여 표시하고 작업 가능 상태를 반영한다.</summary>
        private void UpdateActionState()
        {
            if (IsDisposed || Disposing)
                return;

            bool available = !_presenter.IsBusy && !_presenter.IsClosing && _presenter.HasWorkingCopy;
            int selectedCount = GetSelectedFiles().Count;
            var targets = gridCommitTarget.DataSource as List<SvnFileStatus>;
            int targetCount = targets == null ? 0 : targets.Count(file => file.IsChecked);

            lblFileSelection.Text = "행 선택 · " + selectedCount + "개";
            lblCommitTargets.Text = "커밋 대상 · " + targetCount + "개";
            btnRefresh.Enabled = available;
            btnUpdate.Enabled = available;
            btnLog.Enabled = available;
            btnRevert.Enabled = available && selectedCount > 0;
            btnCommit.Enabled = available && targetCount > 0;
            btnRemoveCommitTarget.Enabled = available && gridViewCommitTarget.GetFocusedRow() is SvnFileStatus;
            chkOnlyChanged.Enabled = available;
            chkIncludeSubfolders.Enabled = available && chkOnlyChanged.Checked;
            UpdateListHint();
        }

        /// <summary>상태 강조를 이름과 상태 열에 한정하고 선택 행의 배색을 유지한다.</summary>
        private static void ApplyStatusAppearance(DevExpress.XtraGrid.Views.Grid.GridView view,
            SvnFileStatus item, RowCellCustomDrawEventArgs e)
        {
            if (item.FileName == ".." || (e.Column.FieldName != "FileName" && e.Column.FieldName != "StatusText"))
                return;
            if (item.IsDirectory && item.HasDescendantChanges && e.Column.FieldName == "FileName")
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            if (view.IsRowSelected(e.RowHandle) || view.FocusedRowHandle == e.RowHandle)
                return;

            switch (item.Status)
            {
                case SvnStatusKind.Modified: e.Appearance.ForeColor = Color.DarkGreen; break;
                case SvnStatusKind.Added: e.Appearance.ForeColor = Color.DarkBlue; break;
                case SvnStatusKind.Deleted:
                case SvnStatusKind.Missing: e.Appearance.ForeColor = Color.DarkRed; break;
                case SvnStatusKind.Conflicted: e.Appearance.ForeColor = Color.OrangeRed; break;
                default:
                    if (item.IsDirectory && item.HasDescendantChanges) e.Appearance.ForeColor = Color.DarkGreen;
                    break;
            }
        }

        /// <summary>항목 자체의 상태와 폴더 하위 변경을 구분하는 짧은 표시 문구를 만든다.</summary>
        private static string GetStatusDisplayText(SvnFileStatus item)
        {
            if (item.FileName == "..") return string.Empty;
            bool descendantOnly = item.IsDirectory && item.HasDescendantChanges
                && !item.HasPropertyChanges && (item.Status == SvnStatusKind.Normal || item.Status == SvnStatusKind.None);
            if (descendantOnly) return "하위 변경";
            var text = item.Status == SvnStatusKind.None ? "미확인" : item.Status.ToString();
            if (item.HasPropertyChanges) text += " / 속성";
            if (item.IsDirectory && item.HasDescendantChanges) text += " / 하위";
            return text;
        }

        /// <summary>작업 복사본, 검색, 변경 필터 상태에 따라 빈 목록의 이유를 안내한다.</summary>
        private void UpdateListHint()
        {
            if (IsDisposed || Disposing) return;
            string message = null;
            if (!_presenter.HasWorkingCopy)
                message = "작업 폴더를 선택하거나 상단의 '작업 폴더 등록'으로 추가하세요.";
            else if (_presenter.IsBusy && gridViewFiles.DataRowCount == 0)
                message = "파일 목록을 불러오는 중입니다.";
            else if (gridFiles.DataSource == null)
                message = "조회된 파일 목록이 없습니다. 새로고침(F5)으로 상태를 조회하세요.";
            else
            {
                bool hasItems = false;
                for (int index = 0; index < gridViewFiles.DataRowCount; index++)
                {
                    var item = gridViewFiles.GetRow(index) as SvnFileStatus;
                    if (item != null && item.FileName != "..") { hasItems = true; break; }
                }
                if (!hasItems)
                    message = !string.IsNullOrWhiteSpace(gridViewFiles.FindFilterText)
                        ? "검색 결과가 없습니다. 검색어를 지우거나 다른 폴더를 선택하세요. (Esc: 검색 해제)"
                        : chkOnlyChanged.Checked ? "현재 조회 범위에 변경 항목이 없습니다. '변경 항목만'을 해제해 전체 목록을 볼 수 있습니다."
                        : "현재 폴더에 표시할 파일이나 하위 폴더가 없습니다.";
            }
            lblFilesEmpty.Text = message ?? string.Empty;
            lblFilesEmpty.Visible = message != null;
        }

        /// <summary>저장된 분할 위치와 현재 존재하는 열의 너비만 복원한다.</summary>
        private void RestoreLayout()
        {
            var layout = _layoutService.Load();
            if (layout == null) return;
            if (layout.FolderPanelWidth >= 120 && layout.FolderPanelWidth <= splitMain.Width - 320)
                splitMain.SplitterPosition = layout.FolderPanelWidth;
            if (layout.CommitPanelHeight >= 100 && layout.CommitPanelHeight <= splitRight.Height - 200)
                splitRight.SplitterPosition = layout.CommitPanelHeight;
            RestoreColumnWidths(gridViewFiles, layout.FileColumnWidths);
            RestoreColumnWidths(gridViewCommitTarget, layout.CommitColumnWidths);
        }

        /// <summary>현재 존재하는 열에 유효한 저장 너비만 적용한다.</summary>
        private static void RestoreColumnWidths(DevExpress.XtraGrid.Views.Grid.GridView view,
            Dictionary<string, int> widths)
        {
            if (widths == null) return;
            view.BeginUpdate();
            try
            {
                foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.Columns)
                {
                    int width;
                    if (!column.OptionsColumn.FixedWidth && widths.TryGetValue(column.FieldName, out width)
                        && width >= Math.Max(40, column.MinWidth) && width <= 4000)
                        column.Width = width;
                }
            }
            finally { view.EndUpdate(); }
        }

        /// <summary>화면 종료 시 현재 분할 위치와 열 너비를 업무 설정과 별도로 저장한다.</summary>
        private void SaveLayout()
        {
            if (!_layoutReady) return;
            var layout = new MainViewLayout
            {
                FolderPanelWidth = splitMain.SplitterPosition,
                CommitPanelHeight = splitRight.SplitterPosition
            };
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in gridViewFiles.Columns)
                layout.FileColumnWidths[column.FieldName] = column.Width;
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in gridViewCommitTarget.Columns)
                layout.CommitColumnWidths[column.FieldName] = column.Width;
            _layoutService.Save(layout);
        }

        /// <summary>현재 펼쳐진 트리 노드의 폴더 경로를 재귀적으로 수집한다.</summary>
        private static IEnumerable<string> GetExpandedFolderPaths(
            TreeListNodes nodes)
        {
            foreach (TreeListNode node in nodes)
            {
                var path = node.Tag as string;

                if (node.Expanded && !string.IsNullOrWhiteSpace(path))
                    yield return path;

                foreach (var childPath in GetExpandedFolderPaths(node.Nodes))
                    yield return childPath;
            }
        }

        /// <summary>저장한 폴더 경로를 기준으로 트리 노드의 펼침 상태를 복원한다.</summary>
        private static void RestoreFolderExpansion(
            TreeListNodes nodes,
            HashSet<string> expandedPaths)
        {
            foreach (TreeListNode node in nodes)
            {
                node.Expanded = expandedPaths.Contains(node.Tag as string);
                RestoreFolderExpansion(node.Nodes, expandedPaths);
            }
        }

        /// <summary>지정한 전체 경로에 해당하는 트리 노드를 재귀적으로 찾는다.</summary>
        private TreeListNode FindNodeByPath(TreeListNodes nodes, string fullPath)
        {
            foreach (TreeListNode node in nodes)
            {
                if (string.Equals(node.Tag as string, fullPath, StringComparison.OrdinalIgnoreCase))
                    return node;

                var found = FindNodeByPath(node.Nodes, fullPath);
                if (found != null) return found;
            }

            return null;
        }

        /// <summary>그리드의 선택 행을 파일 목록으로 변환하고 선택이 없으면 포커스 행을 사용한다.</summary>
        private List<SvnFileStatus> GetSelectedFiles()
        {
            var result = new List<SvnFileStatus>();
            var rows = gridViewFiles.GetSelectedRows();
            foreach (var h in rows)
            {
                var s = gridViewFiles.GetRow(h) as SvnFileStatus;
                if (s != null && !s.IsDirectory) result.Add(s);
            }

            if (result.Count == 0)
            {
                var s = gridViewFiles.GetFocusedRow() as SvnFileStatus;
                if (s != null && !s.IsDirectory) result.Add(s);
            }

            return result;
        }

        #endregion
    }
}
