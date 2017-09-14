using System;
using VideoKiosK.objects;
using System.Windows.Forms;

namespace VideoKiosK
{
    public partial class CC : Form
    {
        private Communicator m_communicator;
        private Main m_frm;
        private Communicator m_communicator_pop = null;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public CC(Communicator communicator, Communicator communicator_pop, Main frm)
        {
            m_frm = frm;
            m_communicator = communicator;
            m_communicator_pop = communicator_pop;

            InitializeComponent();
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:Kartu Kredit Reguler$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLCC;
                SetForegroundWindow(this.Handle);
                this.BringToFront();

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

        private void roundButton3_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:Kartu Kredit Premium$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();                
                object URL = Properties.Settings.Default.webURLCC;
                SetForegroundWindow(this.Handle);
                this.BringToFront();

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
            m_communicator_pop.sendMessage("Info:Kartu Kredit Bisnis$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLCC;
                SetForegroundWindow(this.Handle);
                this.BringToFront();

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

        private void roundButton1_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:Main Menu Kartu Kredit$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLCC;
                SetForegroundWindow(this.Handle);
                this.BringToFront();

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
            m_communicator_pop.sendMessage("Info:Kartu Kredit Corporate$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLCC;
                SetForegroundWindow(this.Handle);
                this.BringToFront();
                                
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

        private void roundButton7_Click(object sender, EventArgs e)
        {
            m_frm.doCall();
            m_communicator.sendMessage("call$");
            m_communicator_pop.sendMessage("Info:Kartu Kredit Lainnya$");

            try
            {
                var IE = new SHDocVw.InternetExplorer();
                object URL = Properties.Settings.Default.webURLCC;
                SetForegroundWindow(this.Handle);
                this.BringToFront();

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
