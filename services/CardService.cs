using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ATMSimulator.models;

namespace ATMSimulator.services
{
    public class CardService
    {
        // Setting up the base URL
        private readonly string baseURL = "https://localhost:5001/api/Cards";

        // Setting up the httpClient
        private readonly HttpClient _httpClient;

        // Setting the constructor
        public CardService()
        {
            // Bypass certificate validation (DON'T USE IN PRODUCTION)
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(handler);
        }

        // Validate the PIN and get the card info
        public async Task<(string?, HttpStatusCode)> validatePIN(string cardNumber, string pin)
        {
            // Setup the apiUrl template
            string apiUrl = $"{baseURL}/{cardNumber}?cardPIN={pin}";

            Console.WriteLine(apiUrl);

            try
            {
                // Send a GET request to the API
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(apiUrl);

                // Get the response body
                string responseBody = await responseMessage.Content.ReadAsStringAsync();

                // Log the response for debugging purposes
                System.Diagnostics.Debug.WriteLine($"Response: {responseBody}");

                // Check if the status code is OK
                if (responseMessage.IsSuccessStatusCode)
                {
                    // Deserialize the response into the Card object
                    try
                    {
                        var card = System.Text.Json.JsonSerializer.Deserialize<Card>(responseBody);
                        System.Diagnostics.Debug.WriteLine($"Card Number: {card?.cardNumber}");
                        return (responseBody, responseMessage.StatusCode);
                    }
                    catch (Exception ex)
                    {
                        // Handle deserialization error
                        System.Diagnostics.Debug.WriteLine($"Error during deserialization: {ex.Message}");
                        return (null, HttpStatusCode.InternalServerError);
                    }
                }

                // Handle specific status codes
                return (null, responseMessage.StatusCode);
            }
            catch (Exception ex)
            {
                // Log network/connection-related exceptions
                System.Diagnostics.Debug.WriteLine($"Error during API call: {ex.Message}");
                return (null, HttpStatusCode.InternalServerError);
            }
        }


    }
}
