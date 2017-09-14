using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace VideoKiosK.objects
{
    public class Communicator
    {
        public enum STATE { IDLE = 1, CONNECTED, CONNECTING, DISCONNECTED };

        private TcpClient m_client = null;
        private NetworkStream m_stream = null;
        private bool m_connected = false;
        private bool m_enabled = false;
        private STATE m_state = STATE.DISCONNECTED;

        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        public bool connect(string host, int port)
        {
            if (m_enabled == true)
            {
                m_state = STATE.IDLE;
                return true;
            }

            m_state = STATE.CONNECTING;
            string HostName = host;
            int HostPort = port;
            try
            {
                m_client = new TcpClient(HostName, HostPort);
                m_stream = m_client.GetStream();
                m_connected = true;
                //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiver));
                return true;
            }
            catch
            {
                
            }

            return false;
        }

        public void disconnect()
        {
            m_state = STATE.DISCONNECTED;
            m_connected = false;
            if (m_stream != null)
            {
                m_stream.Close();
            }
        }

        public bool sendMessage(string msg)
        {
            if (m_connected)
            {
                string message = msg;
                try
                {
                    m_stream.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
                    return true;
                }
                catch (Exception)
                {
                  
                }
            }
            return false;
        }
    }
}
