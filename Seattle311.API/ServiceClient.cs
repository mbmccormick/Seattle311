using System;
using System.IO;
using System.Net;
using System.Text;
using Seattle311.API.Common;
using Seattle311.API.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using RestSharp;

namespace Seattle311.API
{
    public class ServiceClient
    {
        private string _serverAddress = null;
        private string _apiKey = null;

        public string ImgurServerAddress { get; set; }
        public string ImgurAPIKey { get; set; }

        public UserProfile UserData { get; set; }

        public ServiceClient(string serverAddress, string apiKey)
        {
            _serverAddress = serverAddress;
            _apiKey = apiKey;

            UserData = IsolatedStorageHelper.GetObject<UserProfile>("UserData");

            if (UserData == null)
                UserData = new UserProfile();
        }

        public void GetServices(Action<List<Service>> callback)
        {
            RestClient client = new RestClient("http://" + _serverAddress);

            RestRequest request = new RestRequest("/open311/v2/services.json", Method.GET);

            client.ExecuteAsync(request, (response) =>
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                StreamReader sr = new StreamReader(stream);

                JsonTextReader tr = new JsonTextReader(sr);
                List<Service> data = new JsonSerializer().Deserialize<List<Service>>(tr);

                tr.Close();
                sr.Close();

                callback(data);
            });
        }

        public void GetServiceDefinition(Action<ServiceDefinition> callback, string serviceId)
        {
            RestClient client = new RestClient("http://" + _serverAddress);

            RestRequest request = new RestRequest("/open311/v2/services/{id}.json", Method.GET);
            request.AddUrlSegment("id", serviceId);

            client.ExecuteAsync(request, (response) =>
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                StreamReader sr = new StreamReader(stream);

                JsonTextReader tr = new JsonTextReader(sr);
                ServiceDefinition data = new JsonSerializer().Deserialize<ServiceDefinition>(tr);

                tr.Close();
                sr.Close();

                callback(data);
            });
        }

        public void GetRecentServiceRequests(Action<List<ServiceRequest>> callback)
        {
            RestClient client = new RestClient("http://" + _serverAddress);

            RestRequest request = new RestRequest("/open311/v2/requests.json", Method.GET);

            client.ExecuteAsync(request, (response) =>
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                StreamReader sr = new StreamReader(stream);

                JsonTextReader tr = new JsonTextReader(sr);
                List<ServiceRequest> data = new JsonSerializer().Deserialize<List<ServiceRequest>>(tr);

                tr.Close();
                sr.Close();

                callback(data);
            });
        }

        public void CreateServiceRequest(Action<Response<ServiceRequestToken>> callback, ServiceRequest data, bool anonymous)
        {
            RestClient client = new RestClient("http://" + _serverAddress);

            RestRequest request = new RestRequest("/open311/v2/requests.json?api_key={key}", Method.POST);
            request.AddUrlSegment("key", _apiKey);

            request.AddParameter("service_code", data.service_code, ParameterType.GetOrPost);
            request.AddParameter("description", data.description, ParameterType.GetOrPost);
            request.AddParameter("lat", data.lat, ParameterType.GetOrPost);
            request.AddParameter("long", data.@long, ParameterType.GetOrPost);
            request.AddParameter("address_string", data.address, ParameterType.GetOrPost);

            if (data.media_url != null)
                request.AddParameter("media_url", data.media_url, ParameterType.GetOrPost);

            if (data.attributes != null)
            {
                foreach (KeyValuePair<string, string> attribute in data.attributes)
                {
                    request.AddParameter("attribute[" + attribute.Key + "]", attribute.Value, ParameterType.GetOrPost);
                }
            }

            if (anonymous == false)
            {
                request.AddParameter("device_id", UserData.device_id, ParameterType.GetOrPost);
                request.AddParameter("account_id", UserData.account_id, ParameterType.GetOrPost);
                request.AddParameter("first_name", UserData.first_name, ParameterType.GetOrPost);
                request.AddParameter("last_name", UserData.last_name, ParameterType.GetOrPost);
                request.AddParameter("email", UserData.email, ParameterType.GetOrPost);
                request.AddParameter("phone", UserData.phone, ParameterType.GetOrPost);
            }

            client.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    StreamReader sr = new StreamReader(stream);

                    JsonTextReader tr = new JsonTextReader(sr);
                    List<ServiceRequestToken> returnValue = new JsonSerializer().Deserialize<List<ServiceRequestToken>>(tr);

                    tr.Close();
                    sr.Close();

                    Response<ServiceRequestToken> result = new Response<ServiceRequestToken>();
                    result.ResponseObject = returnValue[0];

                    callback(result);
                }
                else
                {
                    MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                    StreamReader sr = new StreamReader(stream);

                    JsonTextReader tr = new JsonTextReader(sr);
                    List<ErrorMessage> returnValue = new JsonSerializer().Deserialize<List<ErrorMessage>>(tr);

                    Response<ServiceRequestToken> result = new Response<ServiceRequestToken>();
                    result.ErrorMessages = returnValue;

                    callback(result);
                }
            });
        }

        public void GetServiceRequest(Action<ServiceRequest> callback, string serviceRequestId)
        {
            RestClient client = new RestClient("http://" + _serverAddress);

            RestRequest request = new RestRequest("/open311/v2/requests/{id}.json", Method.GET);
            request.AddUrlSegment("id", serviceRequestId);

            client.ExecuteAsync(request, (response) =>
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                StreamReader sr = new StreamReader(stream);

                JsonTextReader tr = new JsonTextReader(sr);
                ServiceRequest data = new JsonSerializer().Deserialize<ServiceRequest>(tr);

                tr.Close();
                sr.Close();

                callback(data);
            });
        }

        public void GetServiceRequestFromToken(Action<ServiceRequest> callback, string token)
        {
            RestClient client = new RestClient("http://" + _serverAddress);

            RestRequest request = new RestRequest("/open311/v2/requests/{id}.json", Method.GET);
            request.AddUrlSegment("id", token);

            client.ExecuteAsync(request, (response) =>
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                StreamReader sr = new StreamReader(stream);

                JsonTextReader tr = new JsonTextReader(sr);
                List<ServiceRequest> data = new JsonSerializer().Deserialize<List<ServiceRequest>>(tr);

                tr.Close();
                sr.Close();

                callback(data[0]);
            });
        }

        public void UploadImage(Action<ImgurData> callback, byte[] data)
        {
            RestClient client = new RestClient("https://" + ImgurServerAddress);

            RestRequest request = new RestRequest("/3/upload", Method.POST);
            request.AddHeader("Authorization", "Client-ID " + ImgurAPIKey);
            request.AddParameter("image", Convert.ToBase64String(data), ParameterType.RequestBody);

            client.ExecuteAsync(request, (response) =>
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                StreamReader sr = new StreamReader(stream);

                JsonTextReader tr = new JsonTextReader(sr);
                ImgurData returnValue = new JsonSerializer().Deserialize<ImgurData>(tr);

                tr.Close();
                sr.Close();

                callback(returnValue);
            });
        }

        public void SaveData()
        {
            IsolatedStorageHelper.SaveObject<UserProfile>("UserData", UserData);
        }
    }
}
