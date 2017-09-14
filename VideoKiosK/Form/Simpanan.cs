using System;
using VideoKiosK.objects;
using System.Windows.Forms;

namespace VideoKiosK
{
    public partial class Simpanan : Form
    {
        private Communicator m_communicator;
        private Main m_frm;
        private Communicator m_communicator_pop = null;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public Simpanan(Communicator communicator, Communicator communicator_pop, Main frm)
        {
            m_frm = frm;
            m_communicator = communicator;
            m_communicator_pop = communicator_pop;

            InitializeComponent();
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:Main Menu Simpanan$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLPinj;
                SetForegroundWindow(this.Handle);

                IE.ToolBar = 0;
                IE.StatusBar = false;
                IE.MenuBar = false;
                IE.AddressBar = false;
                IE.Visible = true;
                IE.FullScreen = true;
                IE.Navigate2(ref URL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:BNI Taplus$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLSimp;
                SetForegroundWindow(this.Handle);

                IE.ToolBar = 0;
                IE.StatusBar = false;
                IE.MenuBar = false;
                IE.AddressBar = false;
                IE.Visible = true;
                IE.FullScreen = true;
                IE.Navigate2(ref URL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }

        private void roundButton3_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:BNI Taplus Bisnis$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLSimp;
                SetForegroundWindow(this.Handle);

                IE.ToolBar = 0;
                IE.StatusBar = false;
                IE.MenuBar = false;
                IE.AddressBar = false;
                IE.Visible = true;
                IE.FullScreen = true;
                IE.Navigate2(ref URL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }

        private void roundButton4_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:BNI Haji$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLSimp;
                SetForegroundWindow(this.Handle);

                IE.ToolBar = 0;
                IE.StatusBar = false;
                IE.MenuBar = false;
                IE.AddressBar = false;
                IE.Visible = true;
                IE.FullScreen = true;
                IE.Navigate2(ref URL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }

        private void roundButton5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void roundButton7_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:Simpanan Lainnya$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLSimp;
                SetForegroundWindow(this.Handle);

                IE.ToolBar = 0;
                IE.StatusBar = false;
                IE.MenuBar = false;
                IE.AddressBar = false;
                IE.Visible = true;
                IE.FullScreen = true;
                IE.Navigate2(ref URL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }

        private void roundButton6_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:BNI Tappa$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLSimp;
                SetForegroundWindow(this.Handle);

                IE.ToolBar = 0;
                IE.StatusBar = false;
                IE.MenuBar = false;
                IE.AddressBar = false;
                IE.Visible = true;
                IE.FullScreen = true;
                IE.Navigate2(ref URL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }
    }
}
