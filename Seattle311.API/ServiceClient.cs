using System;
using System.IO;
using System.Net;
using System.Text;
using Seattle311.API.Common;
using Seattle311.API.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;

namespace Seattle311.API
{
    public class ServiceClient
    {
        string _serverAddress = null;
        string _apiKey = null;

        public string ImgurServerAddress { get; set; }
        public string ImgurAPIKey { get; set; }

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

        public void GetRecentServiceRequests(Action<List<ServiceRequest>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + _serverAddress + "/open311/v2/requests.json") as HttpWebRequest;
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
                List<ServiceRequest> data = new JsonSerializer().Deserialize<List<ServiceRequest>>(tr);

                tr.Close();
                sr.Close();

                callback(data);

            }, state);
        }

        public void CreateServiceRequest(Action<Response<ServiceRequestToken>> callback, ServiceRequest data)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + _serverAddress + "/open311/v2/requests.json?api_key=" + _apiKey) as HttpWebRequest;
            request.Accept = "application/json";

            byte[] payload = null;

            StringBuilder postData = new StringBuilder();

            postData.Append("service_code=" + HttpUtility.UrlEncode(data.service_code) + "&");
            postData.Append("description=" + HttpUtility.UrlEncode(data.description) + "&");

            if (data.attributes != null)
            {
                foreach (KeyValuePair<string, string> attribute in data.attributes)
                {
                    postData.Append("attribute[" + attribute.Key + "]=" + attribute.Value + "&");
                }
            }

            if (data.media_url != null)
                postData.Append("media_url=" + HttpUtility.UrlEncode(data.media_url) + "&");

            postData.Append("lat=" + HttpUtility.UrlEncode(data.lat.ToString()) + "&");
            postData.Append("long=" + HttpUtility.UrlEncode(data.@long.ToString()));

            UTF8Encoding encoding1 = new UTF8Encoding();
            payload = encoding1.GetBytes(postData.ToString());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = payload.Length;

            request.BeginGetRequestStream((result1) =>
            {
                using (Stream stream = request.EndGetRequestStream(result1))
                {
                    stream.Write(payload, 0, payload.Length);
                }

                request.BeginGetResponse((result2) =>
                {
                    try
                    {
                        var response = request.EndGetResponse(result2);

                        Stream stream = response.GetResponseStream();
                        UTF8Encoding encoding2 = new UTF8Encoding();
                        StreamReader sr = new StreamReader(stream, encoding2);

                        JsonTextReader tr = new JsonTextReader(sr);
                        List<ServiceRequestToken> returnValue = new JsonSerializer().Deserialize<List<ServiceRequestToken>>(tr);

                        tr.Close();
                        sr.Close();

                        Response<ServiceRequestToken> result = new Response<ServiceRequestToken>();
                        result.ResponseObject = returnValue[0];

                        callback(result);
                    }
                    catch (WebException ex)
                    {
                        var response = ex.Response;

                        Stream stream = response.GetResponseStream();
                        UTF8Encoding encoding2 = new UTF8Encoding();
                        StreamReader sr = new StreamReader(stream, encoding2);

                        JsonTextReader tr = new JsonTextReader(sr);
                        List<ErrorMessage> returnValue = new JsonSerializer().Deserialize<List<ErrorMessage>>(tr);

                        Response<ServiceRequestToken> result = new Response<ServiceRequestToken>();
                        result.ErrorMessages = returnValue;

                        callback(result);
                    }

                }, null);
            }, null);
        }

        public void GetServiceRequest(Action<ServiceRequest> callback, string serviceRequestId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + _serverAddress + "/open311/v2/requests/" + serviceRequestId + ".json") as HttpWebRequest;
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
                ServiceRequest data = new JsonSerializer().Deserialize<ServiceRequest>(tr);

                tr.Close();
                sr.Close();

                callback(data);

            }, state);
        }

        public void GetServiceRequestFromToken(Action<ServiceRequest> callback, string token)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + _serverAddress + "/open311/v2/requests/" + token + ".json") as HttpWebRequest;
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
                List<ServiceRequest> data = new JsonSerializer().Deserialize<List<ServiceRequest>>(tr);

                tr.Close();
                sr.Close();

                callback(data[0]);

            }, state);
        }

        public void UploadImage(Action<ImgurData> callback, byte[] data)
        {
            HttpWebRequest request = HttpWebRequest.Create("https://" + ImgurServerAddress + "/3/upload") as HttpWebRequest;
            request.Accept = "application/json";

            request.Headers["Authorization"] = "Client-ID " + ImgurAPIKey;

            string postData = "image=" + Convert.ToBase64String(data);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            request.BeginGetRequestStream((result1) =>
            {
                using (Stream stream = request.EndGetRequestStream(result1))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(postData);
                        writer.Flush();
                    }
                }

                request.BeginGetResponse((result2) =>
                {
                    var response = request.EndGetResponse(result2);

                    Stream stream = response.GetResponseStream();
                    UTF8Encoding encoding2 = new UTF8Encoding();
                    StreamReader sr = new StreamReader(stream, encoding2);

                    JsonTextReader tr = new JsonTextReader(sr);
                    ImgurData returnValue = new JsonSerializer().Deserialize<ImgurData>(tr);

                    tr.Close();
                    sr.Close();

                    callback(returnValue);

                }, null);
            }, null);
        }
    }
}
