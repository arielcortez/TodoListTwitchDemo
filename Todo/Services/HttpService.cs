﻿using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Services.Interfaces;

namespace Todo.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> PostAsync<T>(string url, object value)
        {
            var response = await PostAsync(url, value);
            return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task PostVoidAsync(string url, object value)
        {
            await PostAsync(url, value);
        }

        private async Task<HttpResponseMessage> PostAsync(string url, object value)
        {
            var response = await _httpClient.PostAsJsonAsync(new Uri(_httpClient.BaseAddress + "/" + url), value);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            throw new NotImplementedException("Error handling not implemented yet!");
        }
    }
}