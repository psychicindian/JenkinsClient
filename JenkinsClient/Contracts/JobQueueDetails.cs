using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsClient.Contracts
{
    public class QueueParameter
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class QueueCause
    {
        public string shortDescription { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
    }

    public class QueueAction
    {
        public List<QueueParameter> parameters { get; set; }
        public List<QueueCause> causes { get; set; }
    }

    public class Task
    {
        public string name { get; set; }
        public string url { get; set; }
        public string color { get; set; }
    }

    public class Executable
    {
        public int number { get; set; }
        public string url { get; set; }
    }

    public class JobQueueDetails
    {
        public List<QueueAction> actions { get; set; }
        public bool blocked { get; set; }
        public bool buildable { get; set; }
        public int id { get; set; }
        public long inQueueSince { get; set; }
        public string @params { get; set; }
        public bool stuck { get; set; }
        public Task task { get; set; }
        public string url { get; set; }
        public object why { get; set; }
        public bool cancelled { get; set; }
        public Executable executable { get; set; }
    }
}
