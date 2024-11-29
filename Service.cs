using PM2E2GRUPO2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;


namespace PM2E2GRUPO2
{
    public class Service
    {
        private readonly HttpClient _httpClient;
        private const string MainUrl = "http://34.55.138.115:15000/sites/";

        public Service()
        {
            _httpClient = new HttpClient{ 
                Timeout = TimeSpan.FromMinutes(5),
            };
        }

        public async Task<List<Sitio>> GetAllSitiosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Sitio>>(MainUrl);
        }

        public async Task<bool> DeleteSitioAsync(Sitio sitio)
        {

            var url = $"{MainUrl}{sitio.Id}";
            Console.WriteLine("URL generada: " + url);
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        public async Task<Byte[]> getVideo(int id) {
            try
            {
                var url = $"{MainUrl}video/{id}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var jsonObject = JsonDocument.Parse(jsonResponse);
                var videoBuffer = jsonObject.RootElement.GetProperty("videoDigital").GetProperty("data");
                Byte[] videoBytes = videoBuffer.EnumerateArray().Select(x => (byte)x.GetInt32()).ToArray();
                return videoBytes;
            }
            catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
                return null;
            }
        }
        public async Task<bool> CreateSitioAsync(Sitio sitio)
        {
            var response = await _httpClient.PostAsJsonAsync(MainUrl, sitio);
            return response.IsSuccessStatusCode;
        }
    }
}
