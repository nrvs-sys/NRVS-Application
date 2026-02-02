using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;


    // TODO - move into DLL? better perf + removes extra stack traces
    public static class Log
    {
        public struct Channel
        {
            public bool exclude;
            public Color color;
        }

        // TODO -
        // * Serialize these using EditorPrefs
        // * make them editable from "Tools/" menu
        public static bool logThread = true;
        public static bool logTime = true;
        public static bool logName = true;
        public static bool processChannels = true;
        public static Dictionary<string, Channel> channels = new();

        public static void Message(string text, LogType severity, Object context = null)
        {
#if !DISABLE_LOGS
            if (processChannels && !ProcessChannel(text, out text))
                return;

            if (logName)
                text = $"[{(context == null ? "Null" : context.name)}]" + text;

            if (logTime)
                text = $"[{Time.time}]" + text;

            if (logThread)
                text = $"[{Thread.CurrentThread.ManagedThreadId.ToString()}]" + text;

            switch (severity)
            {
                default:
                    Debug.Log(text, context);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(text, context);
                    break;
                case LogType.Error:
                    Debug.LogError(text, context);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(text, context);
                    break;
            }
#endif
        }

        // TODO - process channel from regex - check if channel is exlcuded and add color
        static bool ProcessChannel(in string message, out string processedMessage)
        {
            processedMessage = message;
            return true;
        }

        // static Regex channelRegex = new("\[[^\[\]]+\]");

        public static void Info(string message, Object context = null) => Message(message, LogType.Log, context);
        public static void Warn(string message, Object context = null) => Message(message, LogType.Warning, context);
        public static void Error(string message, Object context = null) => Message(message, LogType.Error, context);

        public static void LogInfo(this Object context, string message) => Message(message, LogType.Log, context);
        public static void LogWarn(this Object context, string message) => Message(message, LogType.Warning, context);
        public static void LogError(this Object context, string message) => Message(message, LogType.Error, context);
    }
