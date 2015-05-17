using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsClient.Contracts
{
    public class Executor
    {
    }

    public class LoadStatistics
    {
    }

    public class SwapSpaceMonitor
    {
        public long availablePhysicalMemory { get; set; }
        public long availableSwapSpace { get; set; }
        public long totalPhysicalMemory { get; set; }
        public long totalSwapSpace { get; set; }
    }

    public class ResponseTimeMonitor
    {
        public int average { get; set; }
    }

    public class TemporarySpaceMonitor
    {
        public string path { get; set; }
        public long size { get; set; }
    }

    public class DiskSpaceMonitor
    {
        public string path { get; set; }
        public long size { get; set; }
    }

    public class ClockMonitor
    {
        public int diff { get; set; }
    }

    public class MonitorData
    {
        public SwapSpaceMonitor SwapSpaceMonitor { get; set; }
        public string ArchitectureMonitor { get; set; }
        public ResponseTimeMonitor ResponseTimeMonitor { get; set; }
        public TemporarySpaceMonitor TemporarySpaceMonitor { get; set; }
        public DiskSpaceMonitor DiskSpaceMonitor { get; set; }
        public ClockMonitor ClockMonitor { get; set; }
    }

    public class Computer
    {
        public List<object> actions { get; set; }
        public string displayName { get; set; }
        public List<Executor> executors { get; set; }
        public string icon { get; set; }
        public string iconClassName { get; set; }
        public bool idle { get; set; }
        public bool jnlpAgent { get; set; }
        public bool launchSupported { get; set; }
        public LoadStatistics loadStatistics { get; set; }
        public bool manualLaunchAllowed { get; set; }
        public MonitorData monitorData { get; set; }
        public int numExecutors { get; set; }
        public bool offline { get; set; }
        public object offlineCause { get; set; }
        public string offlineCauseReason { get; set; }
        public List<object> oneOffExecutors { get; set; }
        public bool temporarilyOffline { get; set; }
    }

    public class BuildExecutorStatus
    {
        public int busyExecutors { get; set; }
        public List<Computer> computer { get; set; }
        public string displayName { get; set; }
        public int totalExecutors { get; set; }
    }
}
