//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using M.Algorithm;

//namespace Model
//{
//    public class HttpComponent : Component, IAwake
//    {
//        public static int INITIAL_LENGHT = 3;
//        public static int DEF_TIMEOUT_VALUE = 3000;

//        private Queue<HttpClient> unusedClients = new Queue<HttpClient>();
//        private StaticLinkedList<HttpClient> usedClients = new StaticLinkedList<HttpClient>(0);

//        public void Awake()
//        {
//            for (int i = 0; i < INITIAL_LENGHT; i++)
//            {
//                unusedClients.Enqueue(CreateNew());
//            }
//        }

//        private HttpClient CreateNew()
//        {
//            var client = new HttpClient();

//            return client;
//        }

//        private HttpClient Hatch()
//        {
//            HttpClient client;
//            if (unusedClients.Count == 0)
//            {
//                client = CreateNew();
//            }
//            else
//            {
//                client = unusedClients.Dequeue();
//            }

//            usedClients.Add(client);

//            return client;
//        }

//        private void Recycle(HttpClient client)
//        {
//            client.CancelPendingRequests();
//            usedClients.Remove(client);
//            unusedClients.Enqueue(client);
//        }

//        private async UniTask<HttpResponseMessage> RequestAsync(HttpClient client, Uri url, HttpMethod httpMethod, CancellationTokenSource cts = null, byte[] bytes = null)
//        {
//            HttpRequestMessage message = new HttpRequestMessage(httpMethod, url);
//            if (bytes != null)
//            {
//                message.Content = new ByteArrayContent(bytes);
//            }

//            var ctsTime = new CancellationTokenSource();
//            ctsTime.CancelAfterSlim(DEF_TIMEOUT_VALUE);

//            HttpResponseMessage response;
//            bool isCanceled;
//            if (cts == null)
//            {
//                (isCanceled, response) = await client.SendAsync(message, ctsTime.Token).AsUniTask().SuppressCancellationThrow();
//            }
//            else
//            {
//                var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ctsTime.Token);
//                (isCanceled, response) = await client.SendAsync(message, linkedTokenSource.Token).AsUniTask().SuppressCancellationThrow();
//            }

//            if (isCanceled)
//            {
//            }
//            else
//            {
//                if (response.StatusCode != HttpStatusCode.OK)
//                {
//                    Game.Instance.EventSystem.InvokeAsync<E_HttpStatusError, HttpStatusCode>(response.StatusCode);
//                }

//                NLog.Log.Debug($"Response Status Code: {response.StatusCode} {response.ReasonPhrase}");
//            }

//            return response;
//        }

//        #region Async

//        public async UniTask<byte[]> ToBytesAsync(Uri url, HttpMethod httpMethod, CancellationTokenSource cts = null, byte[] bytes = null)
//        {
//            HttpClient client = Hatch();

//            var response = await RequestAsync(client, url, httpMethod, cts, bytes);

//            if (response == null)
//            {
//                Recycle(client);
//                return null;
//            }

//            var buffer = await response.Content.ReadAsByteArrayAsync().AsUniTask();

//            Recycle(client);

//            return buffer;
//        }

//        public async UniTask<string> ToStringAsync(Uri url, HttpMethod httpMethod, CancellationTokenSource cts = null, byte[] bytes = null)
//        {
//            HttpClient client = Hatch();

//            var response = await RequestAsync(client, url, httpMethod, cts, bytes);

//            if (response == null)
//            {
//                Recycle(client);
//                return null;
//            }

//            var str = await response.Content.ReadAsStringAsync().AsUniTask();

//            Recycle(client);

//            return str;
//        }

//        public async UniTask ToStreamAsync(Uri url, HttpMethod httpMethod, Func<HttpResponseMessage, UniTask> call, CancellationTokenSource cts = null, byte[] bytes = null)
//        {
//            HttpClient client = Hatch();

//            var response = await RequestAsync(client, url, httpMethod, cts, bytes);

//            await call(response);

//            Recycle(client);
//        }

//        #endregion Async

//        #region Sync

//        public byte[] ToBytes(Uri url, HttpMethod httpMethod, CancellationTokenSource cts = null, byte[] bytes = null)
//        {
//            HttpClient client = Hatch();

//            var response = RequestAsync(client, url, httpMethod, cts, bytes).GetAwaiter().GetResult();

//            if (response == null)
//            {
//                Recycle(client);
//                return null;
//            }

//            var buffer = response.Content.ReadAsByteArrayAsync().AsUniTask().GetAwaiter().GetResult();

//            Recycle(client);

//            return buffer;
//        }

//        public string ToString(Uri url, HttpMethod httpMethod, CancellationTokenSource cts = null, byte[] bytes = null)
//        {
//            HttpClient client = Hatch();

//            var response = RequestAsync(client, url, httpMethod, cts, bytes).GetAwaiter().GetResult();

//            if (response == null)
//            {
//                Recycle(client);
//                return null;
//            }

//            var str = response.Content.ReadAsStringAsync().AsUniTask().GetAwaiter().GetResult();

//            Recycle(client);

//            return str;
//        }

//        public void ToStream(Uri url, HttpMethod httpMethod, Action<HttpResponseMessage> call, CancellationTokenSource cts = null, byte[] bytes = null)
//        {
//            HttpClient client = Hatch();

//            var response = RequestAsync(client, url, httpMethod, cts, bytes).GetAwaiter().GetResult();

//            call(response);

//            Recycle(client);
//        }

//        #endregion Sync

//        public override void Dispose()
//        {
//            var usedClient = usedClients[1];
//            while (usedClient.cur != 0)
//            {
//                usedClient = usedClients[usedClient.cur];
//                usedClient.element.Dispose();
//            }

//            foreach (var client in unusedClients)
//            {
//                client.Dispose();
//            }

//            usedClients = null;
//            unusedClients = null;
//            base.Dispose();
//        }
//    }
//}