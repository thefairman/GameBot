using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewTrainer
{
    public partial class Form1 : Form
    {
        MainWork mainWork = new MainWork();
        public Form1()
        {
            InitializeComponent();
            AppSettingsManager.LoadSettings();
        }

        private void btnResetBadProxy_Click(object sender, EventArgs e)
        {
            btnResetBadProxy.Enabled = false;
            RequestsLogic.wfReqLogic.ResetBadProxy();
            btnResetBadProxy.Enabled = true;
        }

        private void btnSaveProxyes_Click(object sender, EventArgs e)
        {
            btnSaveProxyes.Enabled = false;
            string[] stringArray = rtbProxyes.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in stringArray)
            {
                string[] str = item.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (str.Count() == 4)
                {
                    ProxyInfo prx = new ProxyInfo(str[0].Trim(), str[1].Trim(), str[2].Trim(), str[3].Trim());

                    RequestsLogic.wfReqLogic.AddProxy(prx);
                }
            }

            FillProxyes();
            btnSaveProxyes.Enabled = true;
        }

        void FillProxyes()
        {
            List<ProxyInfo> list = RequestsLogic.wfReqLogic.GetProxyList();

            rtbProxyes.Clear();
            foreach (var item in list)
            {
                rtbProxyes.AppendText($"{item.address}:{item.port}:{item.login}:{item.pass}{Environment.NewLine}");
            }

            lblProxyes.Text = list.Count.ToString();
        }

        void FillDGV(DataGridView dgv, List<UIDataRecordBase> listMsg)
        {
            foreach (var item in listMsg)
            {
                List<object> lst = item.GetListForDGV();
                int cols = dgv.ColumnCount > lst.Count ? lst.Count : dgv.ColumnCount;
                dgv.Rows.Insert(0, 1);
                for (int i = 0; i < cols; i++)
                {
                    dgv.Rows[0].Cells[i].Value = lst[i].ToString();
                }

                if (lst.Count > 0 && lst[lst.Count - 1] is Color)
                    dgv.Rows[0].DefaultCellStyle.BackColor = (Color)lst[lst.Count - 1];
            }
        }

        void FillDGVWithMaxRecords(DataGridView dgv, List<UIDataRecordBase> list, int MaxRowsInLog)
        {
            if (list.Count == 0)
                return;

            while (dgv.RowCount < MaxRowsInLog)
            {
                if (list.Count <= 0)
                    return;
                List<UIDataRecordBase> listTmp = new List<UIDataRecordBase>();

                listTmp.Add(list.First());
                list.RemoveAt(0);

                FillDGV(dgv, listTmp);
            }

            if (list.Count > MaxRowsInLog)
                list = list.Skip(Math.Max(0, list.Count() - MaxRowsInLog)).ToList();

            int moveCount = list.Count;
            for (int i = MaxRowsInLog - 1; i >= 0; i--)
            {
                int movedInd = i;
                int replaceInd = i + moveCount;
                if (replaceInd < MaxRowsInLog)
                {
                    for (int col = 0; col < dgv.ColumnCount; col++)
                    {
                        dgv.Rows[replaceInd].Cells[col].Value = dgv.Rows[movedInd].Cells[col].Value;
                    }
                }
            }

            int tmpi = 0;
            for (int i = moveCount - 1; i >= 0; i--)
            {
                List<object> lst = list[i].GetListForDGV();
                int cols = dgv.ColumnCount > lst.Count ? lst.Count : dgv.ColumnCount;
                for (int col = 0; col < cols; col++)
                {
                    dgv.Rows[tmpi].Cells[col].Value = lst[col].ToString();
                }
                if (lst.Count > 0 && lst[lst.Count - 1] is Color)
                    dgv.Rows[tmpi].DefaultCellStyle.BackColor = (Color)lst[lst.Count - 1];
                tmpi++;
            }
        }

        void FillDGVCheckMaxRecords(DataGridView dgv, List<UIDataRecordBase> listMsg, int maxRecords = 0)
        {
            if (maxRecords <= 0)
                FillDGV(dgv, listMsg);
            else
                FillDGVWithMaxRecords(dgv, listMsg, maxRecords);
        }

        void FillUserInfoTimer()
        {
            UserInfo uInfo = Global.userLogic.GetNoSetUserInfo();
            if (uInfo == null)
                return;
            if (txtLogin.Text != uInfo.Login)
            {
                txtLogin.Text = uInfo.Login;
                txtServer.Text = Launcher.servers[uInfo.Server - 1];
            }
        }

        async private void timer1_Tick(object sender, EventArgs e)
        {
            timerUIUpdater.Stop();
            try
            {
                if (!btnStart.Enabled)
                {
                    if (!MainWork.Running)
                    {
                        btnStart.Enabled = true;
                        btnStart.Text = "Start";
                    }
                }

                FillUserInfoTimer();

                UIData uiData = new UIData();
                await Task.Run(() =>
                {
                    uiData = Global.UIdata.GetUIData();
                });
                FillDGVCheckMaxRecords(dgvErrors, uiData.ErrororsUI, Global.settingsMain.maxErrorsUIMessages);
                FillDGVCheckMaxRecords(dgvMessages, uiData.MessagesUI, Global.settingsMain.maxLogUIMessages);

                if (uiData.ruCaptchaBalance != -2)
                    lblRuCaptchaBalance.Text = String.Format("{0:F3}", uiData.ruCaptchaBalance);
            }
            catch (Exception ex)
            {
                Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
            }
            timerUIUpdater.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvMessages.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvMessages.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvMessages.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMessages.Columns[1].Width = 100;

            dgvErrors.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvErrors.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvErrors.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvErrors.Columns[1].Width = 100;

            FillProxyes();
            FillSettings();


        }

        void FillSettings()
        {
            cbServer.Items.AddRange(Launcher.servers);
            cbServer.SelectedIndex = Global.settingsMain.server;
            checkbIsHost.Checked = Global.settingsMain.isHost;
            checkbUsingProxyWF.Checked = Global.settingsMain.usingProxyWF;
            formInitiaded = true;
        }

        bool timerStarted = false;
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                timerUIUpdater.Start();
                timerKeyLissener.Start();
            }
            if (MainWork.Running)
                MakeAPause();
            else
                MakeAStart();
        }

        void MakeAPause()
        {
            btnStart.Enabled = false;
            btnStart.Text = "Pausing...";
            mainWork.Pause();
        }

        void MakeAStart()
        {
            mainWork.Start();
            btnStart.Text = "Pause";
        }

        bool prevF5 = false;
        private void timerKeyLissener_Tick(object sender, EventArgs e)
        {
            if (WinApi.GetAsyncKeyState(Keys.F5) == true)
            {
                if (!prevF5 && MainWork.Running)
                {
                    MakeAPause();
                }
                prevF5 = true;
            }
            else
                prevF5 = false;
        }

        private void btnTesting_Click(object sender, EventArgs e)
        {
            //mainWork.TestingSetRunning(true);
            ////Testing.RotateTest();
            //Testing.MoveMouseYTest();
            var OFD = new System.Windows.Forms.OpenFileDialog(); // Get a browser to open an image file
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                var originalbmp = new Bitmap(Bitmap.FromFile(OFD.FileName)); // Load the  image
                ActionsWithGameLogic.TestingSetImage(originalbmp);
                var test = InGameChecker.FindCoordOfBombOnMinimap();
            }
                
        }
        bool formInitiaded = false;
        private void SettingsChanged(object sender, EventArgs e)
        {
            if (!formInitiaded)
                return;

            Global.settingsMain.server = cbServer.SelectedIndex;
            Global.settingsMain.isHost = checkbIsHost.Checked;
            Global.settingsMain.usingProxyWF = checkbUsingProxyWF.Checked;

            Global.settingsMain.Save(AppSettingsManager.GetSettingsPath());
        }
    }
}
