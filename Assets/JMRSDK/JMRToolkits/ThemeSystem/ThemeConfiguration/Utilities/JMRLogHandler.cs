// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;

public static class JMRLogHandler
{
    private static bool isDebugBuild = Debug.isDebugBuild;
    private static ILogger logger = isDebugBuild ? Debug.unityLogger : null;
    private const string logTag = "JioGlass ";

    public static void Log(object message)
    {
        logger?.Log(logTag, message);
    }

    public static void Log(object message, Object context)
    {
        logger?.Log(logTag, message, context);
    }

    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertion(object message, Object context)
    {
        if(isDebugBuild)
        {
            Debug.LogAssertion(logTag + ": " + message, context);
        }
    }
    
    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertion(object message)
    {
        if (isDebugBuild)
        {
            Debug.LogAssertion(logTag + ": " + message);
        }
    }
    
    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertionFormat(Object context, string format, params object[] args)
    {
        if (isDebugBuild)
        {
            Debug.LogAssertionFormat(context, logTag + ": " + format, args);
        }
    }
    
    [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertionFormat(string format, params object[] args)
    {
        if (isDebugBuild)
        {
            Debug.LogAssertionFormat(logTag + ": " + format, args);
        }
    }

    public static void LogError(object message, Object context)
    {
        logger?.LogError(logTag, message, context);
    }

    public static void LogError(object message)
    {
        logger?.LogError(logTag, message);
    }    

    public static void LogException(System.Exception exception)
    {
        logger?.LogException(exception);
    }

    public static void LogException(System.Exception exception, Object context)
    {
        logger?.LogException(exception, context);
    }

    public static void LogFormat(string format, params object[] args)
    {
        logger?.LogFormat(LogType.Log, logTag + ": " + format, args);
    }

    public static void LogFormat(Object context, string format, params object[] args)
    {
        logger?.LogFormat(LogType.Log, context, logTag + ": " + format, args);
    }

    public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
    {
        if(isDebugBuild)
        {
            Debug.LogFormat(logType, logOptions, context, logTag + ": " + format, args);
        }
    }

    public static void LogWarning(object message, Object context)
    {
        logger?.LogWarning(logTag, message, context);
    }

    public static void LogWarning(object message)
    {
        logger?.LogWarning(logTag, message);
    }

    public static void LogWarningFormat(Object context, string format, params object[] args)
    {
        if(isDebugBuild)
        {
            Debug.LogWarningFormat(context, logTag + ": " + format, args);
        }
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
        if (isDebugBuild)
        {
            Debug.LogWarningFormat(logTag + ": " + format, args);
        }
    }
}
