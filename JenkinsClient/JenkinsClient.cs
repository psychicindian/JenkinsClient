using JenkinsClient.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JenkinsClient
{
    public class JenkinsClient
    {
        private readonly string _jenkinsBaseUrl = string.Empty;
        private readonly RestApiService RestService;
        public JenkinsClient(string baseurl, string userName, string apiToken)
        {
            _jenkinsBaseUrl = baseurl;
            RestService = new RestApiService(userName, apiToken);
        }

        #region BuildExecutor Calls

        private BuildExecutorStatus GetBuildExecutorStatus()
        {
            var buildExecutorUrl = _jenkinsBaseUrl + @"/computer/api/json?pretty=true";
            var buildExecutorStatus = RestService.GetWebResponse<BuildExecutorStatus>(buildExecutorUrl);
            return buildExecutorStatus;
        }

        public bool IsSlaveAvailable()
        {
            var buildStatus = GetBuildExecutorStatus();
            var onlineSystems = buildStatus.computer.Count(node => !node.offline && node.jnlpAgent);
            var availableExecutors = onlineSystems - buildStatus.busyExecutors;
            return availableExecutors != 0;
        }

        public int OnlineSlaves()
        {
            var buildExecutorStatus = GetBuildExecutorStatus();
            var onlineSystems = buildExecutorStatus.computer.Count(node => !node.offline && node.jnlpAgent);
            var availableExecutors = onlineSystems - buildExecutorStatus.busyExecutors;
            return availableExecutors;
        }

        #endregion

        public string Build(string projectName, Dictionary<string, string> parameters)
        {
            string jobUrl;
            HttpWebResponse response;
            if (parameters == null)
            {
                jobUrl = string.Format("{0}/job/{1}", _jenkinsBaseUrl, projectName);
                response = RestService.GetWebResponse(jobUrl, string.Empty);
            }
            else
            {
                jobUrl = string.Format("{0}/job/{1}/buildWithParameters", _jenkinsBaseUrl, projectName);
                var parameterBuilder =
                    parameters.Select(parameter => string.Format("{0}={1}", parameter.Key, parameter.Value)).ToList();
                var postData = string.Join("&", parameterBuilder);
                response = RestService.GetWebResponse(jobUrl, postData);
            }
            Thread.Sleep(10000);//necessary for the job to kick-in
            var buildQueueUrl = response.GetResponseHeader("Location");
            response.Close();
            var queueUrl = string.Format("{0}/api/json?pretty=true", buildQueueUrl);
            var queueDetails = RestService.GetWebResponse<JobQueueDetails>(queueUrl);
            var buildDetailsUrl = string.Format("{0}/api/json?pretty=true", queueDetails.executable.url);
            return buildDetailsUrl;
        }
    }
}
