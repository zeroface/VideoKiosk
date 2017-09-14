using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using VideoKiosK.objects;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VideoKiosK
{
    public partial class Main : Form
    {
        public delegate void UpdateForm(UPDATE_FORM_CODE code);
        public UpdateForm UPDATEFORM = null;
        private System.Timers.Timer _delayTimer;
        private bool m_EVS_CLEAR_CALL_state = false;
        public System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        TcpClient tcpClient;

        private bool endCallStatus = false;
        private cTelephony m_telephony = null;
        private Communicator m_communicator = null;
        private Communicator m_communicator_pop = null;

        private string m_UCID = "";

        private StreamReader clientStreamReader;
        private StreamWriter clientStreamWriter;

        public delegate void RegionClickDelegate(int index, string key);
        [Category("Action")]

        [DllImport("User32")]
        private static extern int SetForegroundWindow(IntPtr hwnd);
        // Activate or minimize a window 
        [DllImportAttribute("User32.DLL")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;
        
        TimeSpan start = new TimeSpan(Properties.Settings.Default.HourStart, Properties.Settings.Default.MinuteStart, 0);
        TimeSpan end = new TimeSpan(Properties.Settings.Default.HourEOD, Properties.Settings.Default.MinuteEOD, 0);

        void initiateTelephony(Object stateInfo)
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
            /*
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.StartsWith(Apps))
                {
                    clsProcess.CloseMainWindow();
                }
            }*/

            Process[] ps = Process.GetProcessesByName(Apps);

            foreach (Process p in ps)
            {
                try
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine(String.Format("Unable to kill process {0}, exception: {1}", p.ToString(), ex.ToString()));
                }
            }
        
        }

        void m_telephony_OnCallDelivered(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnCallDelivered]" + e.Message);
            if (e.AdditionalData.Length > 0)
            {
                m_UCID = e.AdditionalData[0];
            }
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.CALL });
        }

        void m_telephony_OnCallEstablished(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnCallEstablished]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.CALL });
        }

        void m_telephony_OnCallFailed(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnCallFailed]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.IDLE });
        }

        void m_telephony_OnConnected(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnConnected]" + e.Message);
        }

        void m_telephony_OnConnectionCleared(object sender, cTelephonyEventArgs e)
        {
        }

        void m_telephony_OnConnectionFailure(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnConnectionFailure]" + e.Message);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(initiateTelephony));
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.IDLE });
        }

        void m_telephony_OnDisconnected(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnDisconnected]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.IDLE });
        }

        void m_telephony_OnError(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Error("[frmMain::m_telephony_OnError]" + e.Message);
            if (m_telephony.State == cTelephony.STATE.DISCONNECTED)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(initiateTelephony));
            }
            else if (m_telephony.State == cTelephony.STATE.IDLE)
            {
            }
        }

        void m_telephony_OnOpenStream(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnOpenStream]" + e.Message);
        }

        void m_telephony_OnMonitor(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnMonitor]" + e.Message);
        }

        void m_telephony_OnHold(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnHold]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.HOLD });
        }

        void m_telephony_OnRetrieve(object sender, cTelephonyEventArgs e)
        {
            VideoKiosK.utility.LogHelper.logger.Debug("[frmMain::m_telephony_OnRetrieve]" + e.Message);
            this.Invoke(UPDATEFORM, new object[] { UPDATE_FORM_CODE.CALL });
        }

        void m_telephony_OnCallCleared(object sender, VideoKiosK.objects.cTelephonyEventArgs e)
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
            this.WindowState = FormWindowState.Normal;
            if (m_EVS_CLEAR_CALL_state == true)
            {
                m_EVS_CLEAR_CALL_state = false;
                m_communicator.sendMessage("end$");
            }
        }

        private void makeCall()
        {
            this.WindowState = FormWindowState.Minimized;
            if (m_telephony.MakeCall(Properties.Settings.Default.DestinationID, Properties.Settings.Default.UUI))
            {
                m_EVS_CLEAR_CALL_state = true;
            }
        }

        private void ProcessTimerEventEOD(object obj)
        {
            if (m_telephony.State == cTelephony.STATE.ON_CALL ||
               m_telephony.State == cTelephony.STATE.ON_HOLD ||
               m_telephony.State == cTelephony.STATE.ON_DIAL)
            {

            }
            else
            {
                Process.Start(Properties.Settings.Default.OpenVideo, "/c wtclient eod");
            }
        }

        private void ProcessTimerEventStart(object obj)
        {
            Process.Start(Properties.Settings.Default.OpenVideo, "/c wtclient swf");
        }

        private bool ConnectToCall()
        {
            //connect to server at given port
            TcpClient tcpClient = new TcpClient(Properties.Settings.Default.Host, Properties.Settings.Default.Port);

            try
            {
                //get a network stream from server
                NetworkStream clientSockStream = tcpClient.GetStream();
                clientStreamReader = new StreamReader(clientSockStream);
                clientStreamWriter = new StreamWriter(clientSockStream);

                clientStreamWriter.WriteLine("Call");
                clientStreamWriter.Flush();

                clientSockStream.Close();
            }
            catch (Exception)
            {
                return false;
            }

            tcpClient.Close();
            return true;
        }

        private bool ConnectToEndCall()
        {
            //connect to server at given port
            TcpClient tcpClient = new TcpClient(Properties.Settings.Default.Host, Properties.Settings.Default.Port);

            try
            {
                //get a network stream from server
                NetworkStream clientSockStream = tcpClient.GetStream();
                clientStreamReader = new StreamReader(clientSockStream);
                clientStreamWriter = new StreamWriter(clientSockStream);

                clientStreamWriter.WriteLine("EndCall");
                clientStreamWriter.Flush();

                clientSockStream.Close();
            }
            catch (Exception)
            {
                return false;
            }

            tcpClient.Close();
            return true;
        }

        private void SendMessageCall()
        {
            try
            {
                //send message to server            
                clientStreamWriter = new StreamWriter(tcpClient.GetStream());
                clientStreamWriter.WriteLine("a");
                clientStreamWriter.Flush();
            }
            catch (Exception)
            {

            }
        }

        private void SendMessageEndCall()
        {
            try
            {
                //send message to server                
                clientStreamWriter = new StreamWriter(tcpClient.GetStream());
                clientStreamWriter.WriteLine("");
                clientStreamWriter.Flush();
            }
            catch (Exception)
            {

            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            System.Threading.TimerCallback callbackEOD = new System.Threading.TimerCallback(ProcessTimerEventEOD);
            var dtEOD = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Properties.Settings.Default.HourEOD, Properties.Settings.Default.MinuteEOD, 0);

            if (DateTime.Now < dtEOD)
            {
                var timerEOD = new System.Threading.Timer(callbackEOD, null, dtEOD - DateTime.Now, TimeSpan.FromHours(24));
            }

            System.Threading.TimerCallback callbackStart = new System.Threading.TimerCallback(ProcessTimerEventStart);
            var dtStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Properties.Settings.Default.HourStart, Properties.Settings.Default.MinuteStart, 0);

            if (DateTime.Now < dtStart)
            {
                var timerStart = new System.Threading.Timer(callbackStart, null, dtStart - DateTime.Now, TimeSpan.FromHours(24));
            }

            m_communicator.connect(VideoKiosK.Properties.Settings.Default.Host, VideoKiosK.Properties.Settings.Default.Port);
            m_communicator_pop.connect(VideoKiosK.Properties.Settings.Default.HostPop, VideoKiosK.Properties.Settings.Default.PortPop);
            m_communicator_pop.sendMessage(VideoKiosK.Properties.Settings.Default.Location+"$");
        }        

        public Main()
        {
            InitializeComponent();
            m_telephony = new cTelephony(Properties.Settings.Default.DeviceID);
            m_communicator = new Communicator();
            m_communicator_pop = new Communicator();
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

        private void roundButton1_Click(object sender, EventArgs e)
        {
            doCall();
            m_communicator_pop.sendMessage("Info:Main Menu$");
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            CC frm = new CC(m_communicator, m_communicator_pop, this);
            frm.Show(this);
        }

        private void roundButton3_Click(object sender, EventArgs e)
        {
            Simpanan frm = new Simpanan(m_communicator, m_communicator_pop, this);
            frm.Show(this);
        }

        private void roundButton4_Click(object sender, EventArgs e)
        {
            ATM frm = new ATM(m_communicator, m_communicator_pop, this);
            frm.Show(this);
        }

        private void roundButton5_Click(object sender, EventArgs e)
        {
            Pinjaman frm = new Pinjaman(m_communicator, m_communicator_pop, this);
            frm.Show(this);
        }

        private void roundButton6_Click(object sender, EventArgs e)
        {
            doCall();
        }

        public void doCall()
        {
            makeCall();
            m_communicator.sendMessage("call$");
        }
    }    
}
