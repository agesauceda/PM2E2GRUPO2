using PM2E2GRUPO2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;


namespace PM2E2GRUPO2
{
    public class Service
    {
        private readonly HttpClient _httpClient;
        private const string MainUrl = "http://34.55.138.115:15000/sites/";

        public Service() 
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Sitio>> GetAllSitiosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Sitio>>(MainUrl);
        }

        public async Task<bool> CreateSitioAsync(Sitio sitio)
        {
            var response = await _httpClient.PostAsJsonAsync(MainUrl, sitio);
            return response.IsSuccessStatusCode;
        }
    }
}
