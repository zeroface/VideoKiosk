using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoKiosK.objects
{
    public delegate void TelephonyEventHandler(object sender, cTelephonyEventArgs e);

    public class cTelephonyEventArgs
    {
        public enum EVENT_TYPE
        {
            CTIGATEWAY_CONNECTED = 1,
            EVS_STREAM_OPEN,
            EVS_CSTA_MONITOR_CONF,
            EVS_MAKECALL_CONF,
            EVS_DEVICE_INITIATED,
            EVS_DEVICE_ORIGINATED,
            EVS_NETWORK_REACHED,
            EVS_DEVICE_ESTABLISHED,
            EVS_CONN_CLEARED,
            EVS_CLEARCONNECTION_CONF,
            EVS_UNIVERSAL_FAILURE,
            EVS_CALL_FAILED,
            EVS_CALL_CLEARED,
            EVS_HOLDCALL_CONF,
            EVS_DEVICE_HELD,
            EVS_RETRIEVECALL_CONF,
            EVS_DEVICE_RETR,
            CONNECTION_FAILURE,
            EVS_UNKNOWN
        };

        protected string m_message = "";
        protected string m_deviceID = "";
        protected string m_code = "";
        protected string m_eventName = "";
        protected EVENT_TYPE m_event = EVENT_TYPE.EVS_UNKNOWN;

        protected string[] m_additionalData = null;
        
        public cTelephonyEventArgs()
        {
       
        }

        public cTelephonyEventArgs(string message)
        {
            m_message = message;
            string[] splitResult = m_message.Split(new char[] { ';' });
            if (splitResult.Length >= 3)
            {
                m_deviceID = splitResult[0];
                m_code = splitResult[1];
                m_eventName = splitResult[2];
                m_event = getEventType(m_eventName);
                
                if (splitResult.Length > 3)
                {
                    m_additionalData = new string[splitResult.Length - 3];
                    for (int i = 3; i < splitResult.Length; i++)
                    {
                        m_additionalData[i - 3] = splitResult[i];
                    }
                }
            }
        }

        public string Message
        {
            get { return m_message; }
        }

        public string Code
        {
            get { return m_code; }
        }

        public string EventName
        {
            get { return m_eventName; }
        }

        public EVENT_TYPE EventType
        {
            get { return m_event; }
        }
       
        public string[] AdditionalData
        {
            get { return m_additionalData; }
        }

        private EVENT_TYPE getEventType(string eventString)
        {
            if (m_eventName.Equals("CTIGATEWAY_CONNECTED"))
            {
                return EVENT_TYPE.CTIGATEWAY_CONNECTED;
            }
            else if (m_eventName.Equals("EVS_CLEARCONNECTION_CONF"))
            {
                return EVENT_TYPE.EVS_CLEARCONNECTION_CONF;
            }
            else if (m_eventName.Equals("EVS_CONN_CLEARED"))
            {
                return EVENT_TYPE.EVS_CONN_CLEARED;
            }
            else if (m_eventName.Equals("EVS_CALL_CLEARED"))
            {
                return EVENT_TYPE.EVS_CALL_CLEARED;
            }
            else if (m_eventName.Equals("EVS_CSTA_MONITOR_CONF"))
            {
                return EVENT_TYPE.EVS_CSTA_MONITOR_CONF;
            }
            else if (m_eventName.Equals("EVS_DEVICE_ESTABLISHED"))
            {
                return EVENT_TYPE.EVS_DEVICE_ESTABLISHED;
            }
            else if (m_eventName.Equals("EVS_DEVICE_INITIATED"))
            {
                return EVENT_TYPE.EVS_DEVICE_INITIATED;
            }
            else if (m_eventName.Equals("EVS_DEVICE_ORIGINATED"))
            {
                return EVENT_TYPE.EVS_DEVICE_ORIGINATED;
            }
            else if (m_eventName.Equals("EVS_MAKECALL_CONF"))
            {
                return EVENT_TYPE.EVS_MAKECALL_CONF;
            }
            else if (m_eventName.Equals("EVS_NETWORK_REACHED"))
            {
                return EVENT_TYPE.EVS_NETWORK_REACHED;
            }
            else if (m_eventName.Equals("EVS_STREAM_OPEN"))
            {
                return EVENT_TYPE.EVS_STREAM_OPEN;
            }
            else if (m_eventName.Equals("EVS_UNIVERSAL_FAILURE"))
            {
                return EVENT_TYPE.EVS_UNIVERSAL_FAILURE;
            }
            else if (m_eventName.Equals("EVS_CALL_FAILED"))
            {
                return EVENT_TYPE.EVS_CALL_FAILED;
            }
            else if (m_eventName.Equals("CONNECTION_FAILURE"))
            {
                return EVENT_TYPE.CONNECTION_FAILURE;
            }
            else if (m_eventName.Equals("EVS_HOLDCALL_CONF"))
            {
                return EVENT_TYPE.EVS_HOLDCALL_CONF;
            }
            else if (m_eventName.Equals("EVS_DEVICE_HELD"))
            {
                return EVENT_TYPE.EVS_DEVICE_HELD;
            }
            else if (m_eventName.Equals("EVS_RETRIEVECALL_CONF"))
            {
                return EVENT_TYPE.EVS_RETRIEVECALL_CONF;
            }
            else if (m_eventName.Equals("EVS_DEVICE_RETR"))
            {
                return EVENT_TYPE.EVS_DEVICE_RETR;
            }
            
            return EVENT_TYPE.EVS_UNKNOWN;
                
        }
    }
}
