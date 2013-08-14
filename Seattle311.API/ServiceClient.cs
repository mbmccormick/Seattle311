using System;
using System.IO;
using System.Net;
using System.Text;
using Open311.Common;
using Open311.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Open311
{
    public class ServiceClient
    {
        string _serverAddress = null;
        string _apiKey = null;

        public ServiceClient(string serverAddress, string apiKey)
        {
            _serverAddress = serverAddress;
            _apiKey = apiKey;
        }

        public void GetServices(Action<List<Service>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + _serverAddress + "/open311/v2/services.json") as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                var response = request.EndGetResponse(result);

                Stream stream = response.GetResponseStream();
                UTF8Encoding encoding = new UTF8Encoding();
                StreamReader sr = new StreamReader(stream, encoding);

                JsonTextReader tr = new JsonTextReader(sr);
                List<Service> data = new JsonSerializer().Deserialize<List<Service>>(tr);

                tr.Close();
                sr.Close();

                callback(data);

            }, state);
        }

        public void GetServiceDefinition(Action<ServiceDefinition> callback, string serviceId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + _serverAddress + "/open311/v2/services/" + serviceId + ".json") as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                var response = request.EndGetResponse(result);

                Stream stream = response.GetResponseStream();
                UTF8Encoding encoding = new UTF8Encoding();
                StreamReader sr = new StreamReader(stream, encoding);

                JsonTextReader tr = new JsonTextReader(sr);
                ServiceDefinition data = new JsonSerializer().Deserialize<ServiceDefinition>(tr);

                tr.Close();
                sr.Close();

                callback(data);

            }, state);
        }
    }
}
