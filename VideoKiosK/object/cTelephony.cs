using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace VideoKiosK.objects
{
    class cTelephony
    {
        public enum STATE { IDLE = 1, ON_DIAL, ON_CALL, ON_HOLD, CONNECTING, DISCONNECTED };

        private string m_deviceID = "";
        private TcpClient m_client = null;
        private NetworkStream m_stream = null;
        private bool m_connected = false;
        private bool m_enabled = false;
        private STATE m_state = STATE.DISCONNECTED;

        public event TelephonyEventHandler OnConnected;
        public event TelephonyEventHandler OnOpenStream;
        public event TelephonyEventHandler OnMonitor;
        public event TelephonyEventHandler OnDisconnected;
        public event TelephonyEventHandler OnCallEstablished;
        public event TelephonyEventHandler OnCallDelivered;
        public event TelephonyEventHandler OnCallFailed;
        public event TelephonyEventHandler OnConnectionCleared;
        public event TelephonyEventHandler OnConnectionFailure;
        public event TelephonyEventHandler OnHold;
        public event TelephonyEventHandler OnRetrieve;
        public event TelephonyEventHandler OnError;
        public event TelephonyEventHandler OnCallCleared;

        public cTelephony(string deviceID)
        {
            if (deviceID == null || deviceID.Length < 1)
            {
                throw(new Exception("INVALID DEVICE ID"));
            }
            m_deviceID = deviceID;
        }
       
        public STATE State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        protected virtual void Connected(cTelephonyEventArgs e)
        {
            login();
            if (OnConnected != null)
            {
                //Invokes the delegates.
                OnConnected(this, e);
            }
        }

        protected virtual void OpenStream(cTelephonyEventArgs e)
        {
            register();
            if (OnOpenStream != null)
            {
                //Invokes the delegates.
                OnOpenStream(this, e);
            }
        }

        protected virtual void Monitor(cTelephonyEventArgs e)
        {
            if (OnMonitor != null)
            {
                //Invokes the delegates.
                OnMonitor(this, e);
            }
        }

        protected virtual void Disconnected(cTelephonyEventArgs e)
        {
            if (OnDisconnected != null)
            {
                //Invokes the delegates.
                OnDisconnected(this, e);
            }
        }

        protected virtual void CallEstablished(cTelephonyEventArgs e)
        {
            if (OnCallEstablished != null)
            {
                //Invokes the delegates.
                OnCallEstablished(this, e);
            }
        }

        protected virtual void CallDelivered(cTelephonyEventArgs e)
        {
            if (OnCallDelivered != null)
            {
                //Invokes the delegates.
                OnCallDelivered(this, e);
            }
        }

        protected virtual void ConnectionCleared(cTelephonyEventArgs e)
        {
            if (OnConnectionCleared != null)
            {
                //Invokes the delegates.
                OnConnectionCleared(this, e);
            }
        }

        protected virtual void ConnectionFailed(cTelephonyEventArgs e)
        {
            if (OnCallFailed != null)
            {
                //Invokes the delegates.
                OnCallFailed(this, e);
            }
        }

        protected virtual void ConnectionFailure(cTelephonyEventArgs e)
        {            
            m_state = STATE.DISCONNECTED;
            m_connected = false;
            if (OnConnectionFailure != null)
            {
                //Invokes the delegates.
                OnConnectionFailure(this, e);
            }
        }

        protected virtual void CallCleared(cTelephonyEventArgs e)
        {
            if (OnCallCleared != null)
            {
                //Invokes the delegates.
                OnCallCleared(this, e);
            }
        }

        protected virtual void Hold(cTelephonyEventArgs e)
        {
            if (OnHold != null)
            {
                //Invokes the delegates.
                OnHold(this, e);
            }
        }

        protected virtual void Retrieve(cTelephonyEventArgs e)
        {
            if (OnRetrieve != null)
            {
                //Invokes the delegates.
                OnRetrieve(this, e);
            }
        }

        protected virtual void Error(cTelephonyEventArgs e)
        {
            if (OnError != null)
            {
                //Invokes the delegates.
                OnError(this, e);
            }
        }        

        public bool connect()
        {
            if (m_enabled == true)
            {
                m_state = STATE.IDLE;
                return true;
            }
            
            m_state = STATE.CONNECTING;
            string HostName = VideoKiosK.Properties.Settings.Default.CTIGatewayHost;
            int HostPort = VideoKiosK.Properties.Settings.Default.CTIGatewayPort;
            try
            {
                m_client = new TcpClient(HostName, HostPort);
                m_stream = m_client.GetStream();
                m_connected = true;
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiver));
                return true;
            }
            catch 
            {
                cTelephonyEventArgs args = new cTelephonyEventArgs("0;0;CONNECTION_FAILURE;");
                ConnectionFailure(args);
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
        
        private void login()
        {
            string UserID = VideoKiosK.Properties.Settings.Default.UserID;
            string Password = VideoKiosK.Properties.Settings.Default.Password;
            string Server = VideoKiosK.Properties.Settings.Default.Server;
            if (m_connected)
            {
                String message;
                message = "0006;LOGIN;" + UserID + ";" + Password + ";" + Server + ";\n";
                try
                {
                    m_stream.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
                }
                catch (Exception exc)
                {
                    utility.LogHelper.logger.Error("[cTelephony::Login]", exc);
                    cTelephonyEventArgs args = new cTelephonyEventArgs("0;0;CONNECTION_FAILURE;");
                    ConnectionFailure(args);
                }
            }
        }

        private void register()
        {
            if (m_connected)
            {
                string message = "0007;EXT;" + VideoKiosK.Properties.Settings.Default.DeviceID + ";" + "\n";
                try
                {
                    m_stream.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
                }
                catch (Exception)
                {
                    //utility.LogHelper.logger.Error("[cTelephony::Login]", exc);
                    cTelephonyEventArgs args = new cTelephonyEventArgs("0;0;CONNECTION_FAILURE;");
                    ConnectionFailure(args);
                }
            }
        }

        public bool MakeCall(string Destination, string UUI)
        {
            string dialPrefix = VideoKiosK.Properties.Settings.Default.dialPrefix;
            string dialSeparator = VideoKiosK.Properties.Settings.Default.dialSeparator;
            string dialPassword = VideoKiosK.Properties.Settings.Default.DialPassword;
            
            if (m_connected)
            {
                String message;
                message = "0002;CALL;" + dialPrefix + Destination + dialSeparator + dialPassword + ";" + UUI + ";" + "\n";
                try
                {
                    m_stream.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
                    return true;
                }
                catch (Exception)
                {
                    //utility.LogHelper.logger.Error("[cTelephony::MakeCall]", exc);
                    cTelephonyEventArgs args = new cTelephonyEventArgs("0;0;CONNECTION_FAILURE;");
                    ConnectionFailure(args);
                }
            }
            return false;
        }

        public bool DropCall()
        {
            if (m_connected)
            {
                String message;
                message = "0005;DROP" + ";" + "\n";
                try
                {
                    m_stream.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
                    return true;
                }
                catch (Exception)
                {
                    //utility.LogHelper.logger.Error("[cTelephony::DropCall]", exc);
                    cTelephonyEventArgs args = new cTelephonyEventArgs("0;0;CONNECTION_FAILURE;");
                    ConnectionFailure(args);
                }
            }
            return false;
        }

        public bool HoldCall()
        {
            if (m_connected)
            {
                string message = "0003;HOLD" + ";" + "\n";
                try
                {
                    m_stream.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
                    return true;
                }
                catch (Exception)
                {
                    //utility.LogHelper.logger.Error("[cTelephony::Hold]", exc);
                    cTelephonyEventArgs args = new cTelephonyEventArgs("0;0;CONNECTION_FAILURE;");
                    ConnectionFailure(args);
                }
            }
            return false;
        }

        public bool UnholdCall()
        {
            if (m_connected)
            {
                string message = "0004;UHLD" + ";" + "\n";
                try
                {
                    m_stream.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
                    return true;
                }
                catch (Exception)
                {
                    //utility.LogHelper.logger.Error("[cTelephony::UnholdCall]", exc);
                    cTelephonyEventArgs args = new cTelephonyEventArgs("0;0;CONNECTION_FAILURE;");
                    ConnectionFailure(args);
                }
            }
            return false;
        }

        private void receiver(Object stateInfo)
        {
            StreamReader strReader;
            String serverResponse;
            serverResponse = "";
            strReader = new StreamReader(m_stream);

            while (m_connected)
            {
                try
                {
                    serverResponse = "";
                    serverResponse = strReader.ReadLine();
                    Object param = serverResponse;
                    if (param != null)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(raiseEvent), param);
                    }
                }
                catch
                {
                    if (m_connected && m_state != STATE.DISCONNECTED)
                    {
                        m_connected = false;
                        m_state = STATE.DISCONNECTED;
                        Error(new cTelephonyEventArgs("0;0;DISCONNECTED;"));
                    }
                }

            }

        }        

        private void raiseEvent(Object stateInfo)
        {
            string message = (string)stateInfo;
            cTelephonyEventArgs eventArgs = new cTelephonyEventArgs(message);
            if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.CTIGATEWAY_CONNECTED)
            {
                m_state = STATE.CONNECTING;
                Connected(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_CLEARCONNECTION_CONF)
            {
                //Just a response from AES doesnt mean that the connection is really cleared
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_STREAM_OPEN)
            {
                m_state = STATE.CONNECTING;
                OpenStream(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_CSTA_MONITOR_CONF)
            {
                m_state = STATE.IDLE;
                Monitor(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_CONN_CLEARED)
            {               
                ConnectionCleared(eventArgs);
                
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_DEVICE_ESTABLISHED)
            {
                m_state = STATE.ON_CALL;
                CallEstablished(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_DEVICE_INITIATED)
            {
                m_state = STATE.ON_DIAL;
                CallDelivered(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_DEVICE_ORIGINATED)
            {
                m_state = STATE.ON_DIAL;
                CallDelivered(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_MAKECALL_CONF)
            {
                m_state = STATE.ON_DIAL;
                CallDelivered(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_NETWORK_REACHED)
            {
                m_state = STATE.ON_DIAL;
                CallDelivered(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_CALL_FAILED)
            {
                m_state = STATE.IDLE;
                ConnectionFailed(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_CALL_CLEARED)
            {
                m_state = STATE.IDLE;
                CallCleared(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_DEVICE_HELD)
            {
                m_state = STATE.ON_HOLD;
                Hold(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_DEVICE_RETR)
            {
                m_state = STATE.ON_CALL;
                Retrieve(eventArgs);
            }
            else if (eventArgs.EventType == cTelephonyEventArgs.EVENT_TYPE.EVS_UNIVERSAL_FAILURE)
            {
                if (m_state == STATE.CONNECTING)
                {
                    m_state = STATE.DISCONNECTED;
                    m_connected = false;
                    try
                    {
                        m_stream.Close();
                    }
                    catch (Exception)
                    {
                        //utility.LogHelper.logger.Error("[cTelephony::raiseEvent]", exc);
                    }
                }

                Error(eventArgs);
                m_state = STATE.IDLE;
            }
        }
    }
}
