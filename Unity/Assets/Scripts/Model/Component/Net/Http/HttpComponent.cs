using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Model
{
    [LifeCycle]
    public class HttpComponent : Component, IAwake
    {
        public static int INITIAL_LENGHT = 3;

        private Queue<HttpClient> unusedClients = new Queue<HttpClient>();
        private StaticLinkedList<HttpClient> usedClients = new StaticLinkedList<HttpClient>(0);

        public void Awake()
        {
            for (int i = 0; i < INITIAL_LENGHT; i++)
            {
                unusedClients.Enqueue(CreateNew());
            }
        }

        private HttpClient CreateNew()
        {
            var client = new HttpClient();

            return client;
        }

        private HttpClient Hatch()
        {
            HttpClient client;
            if (unusedClients.Count == 0)
            {
                client = CreateNew();
            }
            else
            {
                client = unusedClients.Dequeue();
            }

            usedClients.Add(client);

            return client;
        }

        private void Recycle(HttpClient client)
        {
            client.CancelPendingRequests();
            unusedClients.Enqueue(client);
        }

        private async Task<HttpResponseMessage> RequestAsync(HttpClient client, Uri url, string method, HttpMethod httpMethod, byte[] bytes = null)
        {
            HttpRequestMessage message = new HttpRequestMessage(httpMethod, new Uri(url, method));
            if (bytes != null)
            {
                message.Content = new ByteArrayContent(bytes);
            }

            HttpResponseMessage response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();

            Debug.LogWarning($"Response Status Code: {response.StatusCode} {response.ReasonPhrase}");

            return response;
        }

        public async Task<byte[]> ToBytesAsync(Uri url, string method, HttpMethod httpMethod, byte[] bytes = null)
        {
            HttpClient client = Hatch();

            var response = await RequestAsync(client, url, method, httpMethod, bytes);

            var buffer = await response.Content.ReadAsByteArrayAsync();

            Recycle(client);

            return buffer;
        }

        public async Task<string> ToStringAsync(Uri url, string method, HttpMethod httpMethod, byte[] bytes = null)
        {
            HttpClient client = Hatch();

            var response = await RequestAsync(client, url, method, httpMethod, bytes);

            var str = await response.Content.ReadAsStringAsync();

            Recycle(client);

            return str;
        }

        public async Task ToStreamAsync(Uri url, string method, HttpMethod httpMethod, Func<HttpResponseMessage, Task> call, byte[] bytes = null)
        {
            HttpClient client = Hatch();

            var response = await RequestAsync(client, url, method, httpMethod, bytes);

            await call(response);

            Recycle(client);
        }

        public byte[] ToBytes(Uri url, string method, HttpMethod httpMethod, byte[] bytes = null)
        {
            HttpClient client = Hatch();

            var response = RequestAsync(client, url, method, httpMethod, bytes).Result;

            var buffer = response.Content.ReadAsByteArrayAsync().Result;

            Recycle(client);

            return buffer;
        }

        public string ToString(Uri url, string method, HttpMethod httpMethod, byte[] bytes = null)
        {
            HttpClient client = Hatch();

            var response = RequestAsync(client, url, method, httpMethod, bytes).Result;

            var str = response.Content.ReadAsStringAsync().Result;

            Recycle(client);

            return str;
        }

        public void ToStream(Uri url, string method, HttpMethod httpMethod, Action<HttpResponseMessage> call, byte[] bytes = null)
        {
            HttpClient client = Hatch();

            var response = RequestAsync(client, url, method, httpMethod, bytes).Result;

            call(response);

            Recycle(client);
        }

        public override void Dispose()
        {
            foreach (var client in unusedClients)
            {
                client.Dispose();
            }

            foreach (var client in usedClients)
            {
                client.Dispose();
            }

            Entity = null;
        }
    }
}