using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ATMSimulator.models;

namespace ATMSimulator.services
{
    

    public class AccountService
    {
        private readonly String baseUrl = "https://localhost:5001/api/Accounts";
        private readonly HttpClient _httpClient;

        public AccountService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(handler);
        }

        public async Task<(WithdrawalResult?,HttpStatusCode)> WithdrawMoney(string accountNumber, decimal amount)
        {
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/{accountNumber}/withdraw",amount);


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<WithdrawalResult>();
                return (result,response.StatusCode);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return (null,response.StatusCode);
            }
        }

        // Get the last 5 operations
        public async Task<(List<Operation>?, HttpStatusCode)> getLast5Operations(string accountNumber)
        {
            // Make the GET request to the API
            var response = await _httpClient.GetAsync($"{baseUrl}/operations/{accountNumber}");

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response to a list of Operations
                var result = await response.Content.ReadFromJsonAsync<List<Operation>>();

                // Return the list of operations and the status code
                return (result, response.StatusCode);
            }
            else
            {
                // Read the error message if the request failed
                var error = await response.Content.ReadAsStringAsync();
                // Optionally, log the error message or handle it further

                // Return null and the status code
                return (null, response.StatusCode);
            }
        }

    }
}
