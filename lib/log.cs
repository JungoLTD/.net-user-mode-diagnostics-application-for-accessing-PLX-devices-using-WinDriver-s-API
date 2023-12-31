// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using Jungo.wdapi_dotnet;

using BYTE = System.Byte;
using WORD = System.UInt16;
using DWORD = System.UInt32;
using UINT32 = System.UInt32;
using UINT64 = System.UInt64;

namespace Jungo.plx_lib
{
    public class Log
    {
        public delegate void TRACE_LOG(string str);
        public delegate void ERR_LOG(string str);

        public static TRACE_LOG dTraceLog;
        public static ERR_LOG dErrLog;

        public Log(TRACE_LOG funcTrace, ERR_LOG funcErr)
        {
            dTraceLog = funcTrace;
            dErrLog = funcErr;
        }

        public static void TraceLog(string str)
        {
            if(dTraceLog == null)
                return;

            dTraceLog(str);
        }

        public static void ErrLog(string str)
        {
            if(dErrLog == null)
                return;

            dErrLog(str);
        }
    }
}
