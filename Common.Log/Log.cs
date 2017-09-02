//using System;
//using System.Collections.Generic;
//using System.Text;
//
//namespace Common.Log
//{
//    public class Log
//    {
//        public string LogType { get; private set; }
//        public LogInfo LogInfo { get; private set; }
//        public Dictionary<string, object> ContextProperties { get; private set; }
//
//        public Log(string logType, LogInfo logInfo)
//        {
//            LogType = logType;
//            LogInfo = logInfo;
//            ContextProperties = new Dictionary<string, object>();
//        }
//
//        public Log(string logType, string logMessageTemplate, params object[] logMessageParameters)
//            : this(logType, new LogInfo(logMessageTemplate, logMessageParameters))
//        { }
//
//        public void AddContextProperty(string name, object value)
//        {
//            ContextProperties.Add(name, value);
//        }
//    }
//}
