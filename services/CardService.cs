using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ATMSimulator.models;

namespace ATMSimulator.services
{
    public class CardService
    {
        // Setting up the base URL
        private readonly string baseURL = "https://localhost:5001/api/Cards";
        private string apiURL;

        // Setting up the httpClient
        private readonly HttpClient _httpClient;
        private string publicKey;

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

            string keyUrl = "https://localhost:5001/api/Security/publicKey";

            // Getting the public key from the server
            try
            {
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(keyUrl);

                publicKey = await responseMessage.Content.ReadAsStringAsync();

                Debug.WriteLine("This is the public key " + publicKey);


                // Encrypt the key with the public key
                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
                    byte[] encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(pin), RSAEncryptionPadding.OaepSHA256);
                    string encryptedBase64 = Convert.ToBase64String(encryptedData);

                    Debug.WriteLine("This is the encrypted Data " + encryptedBase64);
                    string urlSafeEncrypted = WebUtility.UrlEncode(encryptedBase64); // Fixes +,/ issues


                    apiURL = $"{baseURL}/{cardNumber}?cardPIN={urlSafeEncrypted}";

                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("A problem in the encryption process" + ex.Message);

                return (null, HttpStatusCode.InternalServerError);
            }

            
            // Setup the apiUrl template


            try
            {
                // Send a GET request to the API
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(apiURL);

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
