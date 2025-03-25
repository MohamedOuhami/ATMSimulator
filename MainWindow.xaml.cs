using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ATMSimulator.models;
using ATMSimulator.services;
using Newtonsoft.Json;

namespace ATMSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Injecting the services
        private readonly CardService _cardService;
        private readonly AccountService _accountService;

        private readonly string cardNumber = "4111111111111111";
        private Card card;
        private int permissibleTrials = 3;
        private string selectedAccountNumber;
        private string selectedAccountCurrency;
        private decimal selectedAccountBalance;


        public MainWindow()
        {
            InitializeComponent();

            // Inject the service
            _cardService = new CardService();
            _accountService = new AccountService();

            // Disable the card at first 
            disablePAD();

            // Start by showing the welcomePage
            WelcomeScreen.Visibility = Visibility.Visible;
            // The others should be collapsed
            PinScreen.Visibility = Visibility.Collapsed;
            OperationScreen.Visibility = Visibility.Collapsed;
            BalanceScreen.Visibility = Visibility.Collapsed;
            AmountScreen.Visibility = Visibility.Collapsed;
            AccountsScreen.Visibility = Visibility.Collapsed;
        }

        // =============== PAD Controls =====================

        private void PAD_Click(object sender, RoutedEventArgs e)
        {
            // Printing the content of the PAD
            Button clickedPAD = sender as Button;

            if (clickedPAD != null)
            {

                // If we are entering the PIN
                if (PinScreen.Visibility == Visibility.Visible)
                {

                    if (PinEntry.Password.Length < 4)
                    {
                        string numberClicked = clickedPAD.Content.ToString();
                        PinEntry.Password += numberClicked;
                    }
                }
                // Else, if we were in Withdrawal screen
                else if (AmountScreen.Visibility == Visibility.Visible)
                {


                    string numberClicked = clickedPAD.Content.ToString();
                    AmountEntry.Text += numberClicked;

                }

            }
        }

        // Enable the PAD
        private void enablePAD()
        {
            PAD0.IsEnabled = true;
            PAD1.IsEnabled = true;
            PAD2.IsEnabled = true;
            PAD3.IsEnabled = true;
            PAD4.IsEnabled = true;
            PAD5.IsEnabled = true;
            PAD6.IsEnabled = true;
            PAD7.IsEnabled = true;
            PAD8.IsEnabled = true;
            PAD9.IsEnabled = true;
            PAD00.IsEnabled = true;
            PADCancel.IsEnabled = true;
            PADCorrect.IsEnabled = true;
            PADConfirm.IsEnabled = true;
            PADDot.IsEnabled = true;
        }

        // Disable the PAD
        private void disablePAD()
        {
            PAD0.IsEnabled = false;
            PAD1.IsEnabled = false;
            PAD2.IsEnabled = false;
            PAD3.IsEnabled = false;
            PAD4.IsEnabled = false;
            PAD5.IsEnabled = false;
            PAD6.IsEnabled = false;
            PAD7.IsEnabled = false;
            PAD8.IsEnabled = false;
            PAD9.IsEnabled = false;
            PAD00.IsEnabled = false;
            PADCancel.IsEnabled = false;
            PADCorrect.IsEnabled = false;
            PADConfirm.IsEnabled = false;
            PADDot.IsEnabled = false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ATMResponse.Text = "Canceled the operation";
            showWelcomeScreen();
            disablePAD();
        }

        private void Correct_Click(object sender, RoutedEventArgs e)
        {
            PinEntry.Password = "";
            AmountEntry.Text = "";
        }

        // When clicking the confirm button
        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // Checking If we're in the PIN screen
            if (PinScreen.Visibility == Visibility.Visible)
            {

                if (PinEntry.Password.Length == 4)
                {

                    try
                    {
                        // Sending the request with the cardNumber and the cardPIN
                        var (responseBody, statusCode) = await _cardService.validatePIN(cardNumber, PinEntry.Password);

                        // If we get an OK, we show the card info
                        if (statusCode == HttpStatusCode.OK)
                        {
                            // Assuming you have logic to update the UI
                            ServerResponse.Text = "The card Info: \n" + responseBody;

                            card = System.Text.Json.JsonSerializer.Deserialize<Card>(responseBody);

                            showAccountScreen();
                        }
                        else if (statusCode == HttpStatusCode.NotFound)
                        {
                            ServerResponse.Text = "The card was not found in the core banking system.";
                        }
                        else if (statusCode == HttpStatusCode.BadRequest)
                        {
                            ServerResponse.Text = "Incorrect PIN.";
                        }
                        else
                        {
                            // For other unexpected status codes
                            ServerResponse.Text = $"Unexpected error occurred: {statusCode}. Please try again later.";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions from the calling method
                        ServerResponse.Text = $"An error occurred: {ex.Message}";
                        System.Diagnostics.Debug.WriteLine($"Exception in HandlePinValidation: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid PIN");
                }
            }


        }


        // =============== Card Controls =====================


        private void InsertCard(object sender, RoutedEventArgs e)
        {
            ATMResponse.Text = "Inserted the Card of Number " + cardNumber;
            enablePAD();
            showPINScreen();

        }


        private void EjectCard(object sender, RoutedEventArgs e)
        {
            ATMResponse.Text = "Canceled the operation";
            showWelcomeScreen();
            disablePAD();
        }

        // ============== Change screens ===============
        
        private void CheckBalance_Click(object sender, RoutedEventArgs e)
        {
            showBalanceScreen();
        }


        private void WithdrawCash_Click(object sender, RoutedEventArgs e)
        {
            showAmountScreen();
        }


        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            showOperationScreen();
        }

        private void BackToAccount_Click(object sender, RoutedEventArgs e)
        {
            showAccountScreen();
        }

        private async void ConfirmWithdraw_Click(object sender, RoutedEventArgs e)
        {
            // Validate the amount input
            if (!decimal.TryParse(AmountEntry.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount.");
                return;
            }

            try
            {
                // Attempt to withdraw money
                (WithdrawalResult? withdrawResult, HttpStatusCode statusCode) =
                    await _accountService.WithdrawMoney(selectedAccountNumber, amount);

                if (statusCode == HttpStatusCode.OK && withdrawResult != null)
                {
                    selectedAccountBalance = withdrawResult.NewBalance;
                    PDFCreatorOperations.CreateWithdrawalReceipt(selectedAccountNumber, amount, selectedAccountBalance, selectedAccountCurrency);
                    showOperationScreen();
                }
                else
                {
                    MessageBox.Show($"Transaction failed. Status Code: {statusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Network error: {ex.Message}");
            }

            // Clear the amount entry field
            AmountEntry.Text = "";
        }


        private void CancelOperation(object sender, RoutedEventArgs e)
        {
            showAccountScreen();
        }

        private void CancelWithdraw_Click(object sender, RoutedEventArgs e)
        {
            showOperationScreen();
            AmountEntry.Text = "";

        }

        // =============== Showing screens Controls =====================


        private void showWelcomeScreen()
        {
            WelcomeScreen.Visibility = Visibility.Visible;
            PinScreen.Visibility = Visibility.Collapsed;
            OperationScreen.Visibility = Visibility.Collapsed;
            BalanceScreen.Visibility = Visibility.Collapsed;
            AmountScreen.Visibility = Visibility.Collapsed;
            AccountsScreen.Visibility = Visibility.Collapsed;

        }

        private void showPINScreen()
        {
            WelcomeScreen.Visibility = Visibility.Collapsed;
            PinScreen.Visibility = Visibility.Visible;
            OperationScreen.Visibility = Visibility.Collapsed;
            BalanceScreen.Visibility = Visibility.Collapsed;
            AmountScreen.Visibility = Visibility.Collapsed;
            AccountsScreen.Visibility = Visibility.Collapsed;

        }

        private void showOperationScreen()
        {
            WelcomeScreen.Visibility = Visibility.Collapsed;
            PinScreen.Visibility = Visibility.Collapsed;
            OperationScreen.Visibility = Visibility.Visible;
            BalanceScreen.Visibility = Visibility.Collapsed;
            AmountScreen.Visibility = Visibility.Collapsed;
            AccountsScreen.Visibility = Visibility.Collapsed;

        }

        private void showBalanceScreen()
        {
            WelcomeScreen.Visibility = Visibility.Collapsed;
            PinScreen.Visibility = Visibility.Collapsed;
            OperationScreen.Visibility = Visibility.Collapsed;
            BalanceScreen.Visibility = Visibility.Visible;
            AmountScreen.Visibility = Visibility.Collapsed;
            AccountsScreen.Visibility = Visibility.Collapsed;

            BalanceDisplay.Content = selectedAccountBalance + " " + selectedAccountCurrency;

        }

        private void showAmountScreen()
        {
            WelcomeScreen.Visibility = Visibility.Collapsed;
            PinScreen.Visibility = Visibility.Collapsed;
            OperationScreen.Visibility = Visibility.Collapsed;
            BalanceScreen.Visibility = Visibility.Collapsed;
            AmountScreen.Visibility = Visibility.Visible;
            AccountsScreen.Visibility = Visibility.Collapsed;

        }

        private void AccountSelected_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            // Cast the Tag to an integer (assuming id is an integer)
            var accountId = button?.Tag as int?;

            if (accountId.HasValue)
            {
                selectedAccountBalance = card.accounts.FirstOrDefault(a => a.id == accountId).balance;
                selectedAccountNumber = card.accounts.FirstOrDefault(a => a.id == accountId).accountNumber;
                selectedAccountCurrency = card.accounts.FirstOrDefault(a => a.id == accountId).currency;
                showOperationScreen();
            }
            else
            {
                MessageBox.Show("No account ID found.");
            }
        }


        private void showAccountScreen()
        {
            WelcomeScreen.Visibility = Visibility.Collapsed;
            PinScreen.Visibility = Visibility.Collapsed;
            OperationScreen.Visibility = Visibility.Collapsed;
            BalanceScreen.Visibility = Visibility.Collapsed;
            AmountScreen.Visibility = Visibility.Collapsed;
            AccountsScreen.Visibility = Visibility.Visible;

            // Create display items for each account
            var accountItems = card.accounts.Select(a => new
            {
                Id = a.id,
                DisplayText = $"{a.type} - {a.accountNumber}"
            }).ToList();

            AccountsList.ItemsSource = accountItems;
        }

        private async void GetOperations(object sender, RoutedEventArgs e)
        {
            // Fetch the latest 5 operations for this account
            (List<Operation> last5Operations, HttpStatusCode statusCode) = await _accountService.getLast5Operations(selectedAccountNumber);

            if (last5Operations != null)
            {
                ServerResponse.Text = last5Operations[0].amount.ToString();
                PDFCreatorOperations.CreateAtmOperationsReceipt(last5Operations, selectedAccountNumber,selectedAccountCurrency);
            }
            else
            {
                ServerResponse.Text = "No operations";
            }
        }
    }
        
}