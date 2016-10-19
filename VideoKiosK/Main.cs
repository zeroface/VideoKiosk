using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using VideoKiosk.objects;

namespace VideoKiosK
{
    public partial class Main : Form
    {
        public UpdateForm UPDATEFORM = null;
        private System.Timers.Timer _delayTimer;
        private bool m_EVS_CLEAR_CALL_state = false;
        private System.Drawing.Drawing2D.GraphicsPath _pathData;
        private int _activeIndex = -1;
//      private ArrayList _pathsArray;
        private ToolTip _toolTip;
        private Graphics _grapichs;
        private bool endCallStatus = false;
        private cTelephony m_telephony = null;
        private string m_UCID = "";
        private StreamReader clientStreamReader;
        private StreamWriter cleintStreamWriter;
        public delegate void RegionClickDelegate(int index, string key);
        public delegate void UpdateForm(UPDATE_FORM_CODE code);
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        
        [Category("Action")]
        public event RegionClickDelegate RegionClick;

        [DllImport("User32")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImportAttribute("User32.DLL")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        TcpClient tcpClient;
        TimeSpan start = new TimeSpan(Properties.Settings.Default.HourStart, Properties.Settings.Default.MinuteStart, 0);
        TimeSpan end = new TimeSpan(Properties.Settings.Default.HourEOD, Properties.Settings.Default.MinuteEOD, 0);

        private void ActiveApplication(string strAppName)
        {
            Process[] pList = Process.GetProcessesByName(strAppName);
            if (pList.Length > 0)
            {
                ShowWindow(pList[0].MainWindowHandle, SW_RESTORE);
                SetForegroundWindow(pList[0].MainWindowHandle);
            }
        }

         private void initiateTelephony(Object stateInfo)
        {
            m_telephony.connect();
        }

        public enum UPDATE_FORM_CODE
        {
            IDLE=1,
            CALL,
            HOLD,
        }

        private void KillApps(string Apps)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                //now we're going to see if any of the running processes
                //match the currently running processes by using the StartsWith Method,
                //this prevents us from incluing the .EXE for the process we're looking for.
                //. Be sure to not
                //add the .exe to the name you provide, i.e: NOTEPAD,
                //not NOTEPAD.EXE or false is always returned even if
                //notepad is running
                if (clsProcess.ProcessName.StartsWith(Apps))
                {
                    //since we found the proccess we now need to use the
                    //Kill Method to kill the process. Remember, if you have
                    //the process running more than once, say IE open 4
                    //times the loop thr way it is now will close all 4,
                    //if you want it to just close the first one it finds
                    //then add a return; after the Kill
                    clsProcess.CloseMainWindow();
                    //process killed, return true             
                }
            }
        }

        private void m_telephony_OnCallDelivered(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnCallDelivered]" + e.Message);
            if (e.AdditionalData.Length > 0)
            {
                m_UCID = e.AdditionalData[0];
            }
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.CALL });
        }

        private void m_telephony_OnCallEstablished(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnCallEstablished]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.CALL });
        }

        private void m_telephony_OnCallFailed(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnCallFailed]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.IDLE });
        }

        private void m_telephony_OnConnected(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnConnected]" + e.Message);
        }

        private void m_telephony_OnConnectionCleared(object sender, cTelephonyEventArgs e)
        {
        }

        private void m_telephony_OnConnectionFailure(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnConnectionFailure]" + e.Message);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(initiateTelephony));
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.IDLE });
        }

        private void m_telephony_OnDisconnected(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnDisconnected]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.IDLE });
        }

        private void m_telephony_OnError(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Error("[frmMain::m_telephony_OnError]" + e.Message);
            if (m_telephony.State == cTelephony.STATE.DISCONNECTED)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(initiateTelephony));
            }
            else if (m_telephony.State == cTelephony.STATE.IDLE)
            {
            }
        }

        private void m_telephony_OnOpenStream(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnOpenStream]" + e.Message);
        }

        private void m_telephony_OnMonitor(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnMonitor]" + e.Message);
        }

        private void m_telephony_OnHold(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnHold]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.HOLD });
        }

        private void m_telephony_OnRetrieve(object sender, cTelephonyEventArgs e)
        {
            VideoKiosk.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnRetrieve]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.CALL });
        }

        private void m_telephony_OnCallCleared(object sender, VideoKiosk.objects.cTelephonyEventArgs e)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;

            this.WindowState = FormWindowState.Maximized;

            if ((now > start) && (now < end))
            {
                Process.Start(Properties.Settings.Default.OpenVideo, "/c wtclient swf");
            }
            else
            {
                Process.Start(Properties.Settings.Default.OpenVideo, "/c wtclient eod");
            }

            KillApps(Properties.Settings.Default.Browser);

            if (m_EVS_CLEAR_CALL_state == true)
            {
                m_EVS_CLEAR_CALL_state = false;
//              ptBox.BringToFront();
//              ptBoxEnd.SendToBack();
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("end" + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
        }

        public Main()
        {
            InitializeComponent();
            m_telephony = new cTelephony(Properties.Settings.Default.DeviceID);
            if (!m_telephony.connect())
            {
                Process.Start(Properties.Settings.Default.CTIGatewayPath);
            }

            m_telephony.OnCallDelivered += new TelephonyEventHandler(m_telephony_OnCallDelivered);
            m_telephony.OnCallEstablished += new TelephonyEventHandler(m_telephony_OnCallEstablished);
            m_telephony.OnCallFailed += new TelephonyEventHandler(m_telephony_OnCallFailed);
            m_telephony.OnConnected += new TelephonyEventHandler(m_telephony_OnConnected);
            m_telephony.OnConnectionCleared += new TelephonyEventHandler(m_telephony_OnConnectionCleared);
            m_telephony.OnConnectionFailure += new TelephonyEventHandler(m_telephony_OnConnectionFailure);
            m_telephony.OnDisconnected += new TelephonyEventHandler(m_telephony_OnDisconnected);
            m_telephony.OnError += new TelephonyEventHandler(m_telephony_OnError);
            m_telephony.OnMonitor += new TelephonyEventHandler(m_telephony_OnMonitor);
            m_telephony.OnOpenStream += new TelephonyEventHandler(m_telephony_OnOpenStream);
            m_telephony.OnHold += new TelephonyEventHandler(m_telephony_OnHold);
            m_telephony.OnRetrieve += new TelephonyEventHandler(m_telephony_OnRetrieve);
            m_telephony.OnCallCleared += new TelephonyEventHandler(m_telephony_OnCallCleared);

        }
    }
    
}
