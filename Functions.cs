using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyWill
{
    class Functions
    {

        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static void LogIn(SessionManager sessionManager)
        {
            Console.Clear();
            var users = sessionManager.GetUsers();

            Console.Write("Enter your email: ");
            string email = Console.ReadLine();

            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            User user = users.Find(u => u.Email == email && u.HashedPassword == HashPassword(password, u.Salt));

            if (user != null)
            {
                sessionManager.LogIn(user);
                Console.WriteLine($"Welcome, {user.Username}! You are now logged in.");
            }
            else
            {
                Console.WriteLine("Invalid email or password. Please try again.");
            }
        }

        public static void SignUp(SessionManager sessionManager)
        {
            Console.Clear();
            var users = sessionManager.GetUsers();
            
            Console.Write("Enter a new username: ");
            string newUsername = Console.ReadLine();

            if (users.Exists(u => u.Username == newUsername))
            {
                Console.WriteLine("Username already taken. Please choose a different one.");
                return;
            }

            string email;
            do
            {
                Console.Write("Enter your email: ");
                email = Console.ReadLine();

                if (!Functions.IsValidEmail(email))
                {
                    Console.WriteLine("Invalid email format. Please enter a valid email.");
                }
                else if (users.Exists(u => u.Email == email))
                {
                    Console.WriteLine("Email already registered. Please choose a different one.");
                }
            } while (!Functions.IsValidEmail(email) || users.Exists(u => u.Email == email));

            Console.Write("Enter a password: ");
            string newPassword = Console.ReadLine();

            User newUser = new User
            {
                Username = newUsername,
                Salt = Functions.GenerateSalt(),
                Email = email
            };

            newUser.HashedPassword = Functions.HashPassword(newPassword, newUser.Salt);

            users.Add(newUser);

            Console.WriteLine($"Account created successfully for {newUsername}!");
        }

        public static bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, emailPattern);
        }

        public static void ChooseWallet(SessionManager sessionManager)
        {
            Console.Clear();
            if (!sessionManager.IsUserLoggedIn)
            {
                Console.WriteLine("Please log in first.");
                return;
            }

            var user = sessionManager.GetActiveUser();

            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            if (user.Wallets.Count == 0)
            {
                Console.WriteLine("You don't have any wallets. Please create one first.");
                return;
            }

            Console.WriteLine("Choose a wallet:");

            for (int i = 0; i < user.Wallets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Wallet ID: {user.Wallets[i].Id}, Currency: {user.Wallets[i].Currency}, Balance: {user.Wallets[i].BalanceIntPart}.{user.Wallets[i].BalanceFracPart:D2}");
            }

            Console.Write("Enter the number of the wallet: ");
            string selectedWalletNumberInput = Console.ReadLine();

            if (int.TryParse(selectedWalletNumberInput, out int selectedWalletNumber) &&
                selectedWalletNumber > 0 && selectedWalletNumber <= user.Wallets.Count)
            {
                Wallet selectedWallet = user.Wallets[selectedWalletNumber - 1];
                sessionManager.SetActiveWallet(selectedWallet);

                Console.WriteLine($"Wallet ID: {selectedWallet.Id}, Currency: {selectedWallet.Currency}, Balance: {selectedWallet.BalanceIntPart}.{selectedWallet.BalanceFracPart:D2}");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Add Operation");
                Console.WriteLine("2. Check Statistics");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        AddOperation(selectedWallet);
                        break;

                    case "2":
                        CheckStatistics(selectedWallet);
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid wallet number.");
            }
        }

        private static void CheckStatistics(Wallet selectedWallet)
        {
            Console.WriteLine("Enter the start date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.WriteLine("Enter the end date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.WriteLine($"Operations for the selected wallet ({startDate.ToShortDateString()} to {endDate.ToShortDateString()}):");

            Money totalIncome = new Money('+', 0, 0, selectedWallet.Currency);
            Money totalExpense = new Money('+', 0, 0, selectedWallet.Currency);

            foreach (var operation in selectedWallet.Operations)
            {
                if (operation.Date >= startDate && operation.Date <= endDate)
                {
                    if (operation is Expense expense)
                    {
                        Console.WriteLine($"Expense - Amount: {expense.ValueIntPart}.{expense.ValueFracPart:D2}, Date: {expense.Date}, Category: {expense.Category}");
                        totalExpense.MAdd2(expense._Value);
                    }
                    else if (operation is Income income)
                    {
                        Console.WriteLine($"Income - Amount: {income.ValueIntPart}.{income.ValueFracPart:D2}, Date: {income.Date}, Category: {income.Category}");
                        totalIncome.MAdd2(income._Value);
                    }
                }
            }

            Console.WriteLine($"Total Income: {totalIncome.integerPart}.{totalIncome.fractionalPart:D2}");
            Console.WriteLine($"Total Expense: {totalExpense.GetIntegerPart()}.{totalExpense.GetFractionalPart():D2}");
            Console.WriteLine($"Balance on the wallet: {selectedWallet.BalanceIntPart}.{selectedWallet.BalanceFracPart:D2}");
        }







        public static void CreateWallet(SessionManager sessionManager)
        {
            Console.Clear();
            if (!sessionManager.IsUserLoggedIn)
            {
                Console.WriteLine("Please log in first.");
                return;
            }

            var user = sessionManager.GetActiveUser();

            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            Console.Write("Enter starting amount: ");
            decimal startAmount;
            if (!decimal.TryParse(Console.ReadLine(), out startAmount))
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            Console.WriteLine("Choose currency:");
            Console.WriteLine("1. Dollars");
            Console.WriteLine("2. Rubles");

            string currencyChoice = Console.ReadLine();

            Currency currency;

            switch (currencyChoice)
            {
                case "1":
                    currency = Currency.Dollars;
                    break;
                case "2":
                    currency = Currency.Rubles;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            SplitAmount(startAmount, out int StartIntPart, out int StartFracPart);

            var newWallet = new Wallet
            {
                Id = Guid.NewGuid().ToString(),
                Currency = currency,
                _balance = new Money('+', StartIntPart, StartFracPart, currency)
            };

            user.Wallets.Add(newWallet);
            Console.WriteLine("Wallet created successfully.");
        }

        public static void DeleteWallet(SessionManager sessionManager)
        {
            Console.Clear();
            if (!sessionManager.IsUserLoggedIn)
            {
                Console.WriteLine("Please log in first.");
                return;
            }

            var user = sessionManager.GetActiveUser();

            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            if (user.Wallets.Count == 0)
            {
                Console.WriteLine("No wallets available to delete.");
                return;
            }

            Console.WriteLine("Choose a wallet to delete:");

            for (int i = 0; i < user.Wallets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Wallet ID: {user.Wallets[i].Id}, Currency: {user.Wallets[i].Currency}, Balance: {user.Wallets[i].BalanceIntPart}.{user.Wallets[i].BalanceFracPart:D2}");
            }


            Console.Write("Enter the number of the wallet to delete: ");
            string selectedWalletNumberInput = Console.ReadLine();

            if (int.TryParse(selectedWalletNumberInput, out int selectedWalletNumber) && selectedWalletNumber > 0 && selectedWalletNumber <= user.Wallets.Count)
            {
                var walletToDelete = user.Wallets[selectedWalletNumber - 1];

                user.Wallets.Remove(walletToDelete);
                Console.WriteLine("Wallet deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid input or wallet number not found.");
            }
        }

        private static void SplitAmount(decimal totalAmount, out int integerPart, out int fractionalPart)
        {
            integerPart = (int)totalAmount;
            fractionalPart = (int)((totalAmount - integerPart) * 10);
        }


        public static void AddOperation(Wallet wallet)
        {
            Console.Clear();
            Console.WriteLine("Choose operation type:");
            Console.WriteLine("1. Add Income");
            Console.WriteLine("2. Add Expense");

            string operationType = Console.ReadLine();

            switch (operationType)
            {
                case "1":
                    AddIncome(wallet);
                    break;

                case "2":
                    AddExpense(wallet);
                    break;

                default:
                    Console.WriteLine("Invalid operation type.");
                    break;
            }

        }

        public static void AddIncome(Wallet wallet)
        {
            Console.Clear();
            Console.WriteLine("Choose income category:");
            int i = 1;
            foreach (IncomeCategory category in Enum.GetValues(typeof(IncomeCategory)))
            {
                Console.WriteLine($"{i++}. {category}");
            }

            if (Enum.TryParse(Console.ReadLine(), out IncomeCategory incomeCategory))
            {
                if (Enum.IsDefined(typeof(IncomeCategory), incomeCategory))
                {
                    Console.Write("Enter income amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        Console.Write("Enter income date (yyyy-MM-dd): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                        {
                            SplitAmount(amount, out int incomeIntPart, out int incomeFracPart);

                            var newIncome = new Income
                            {
                                _Value = new Money('+', incomeIntPart, incomeFracPart, wallet.Currency),
                                Category = incomeCategory,
                                Date = date
                            };
                            wallet._balance.MAdd2(newIncome._Value);
                            wallet.Operations.Add(newIncome);

                            Console.WriteLine("Income added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid category.");
                }
            }
            else
            {
                Console.WriteLine("Invalid category.");
            }
        }

        public static void AddExpense(Wallet wallet)
        {
            Console.Clear();
            Console.WriteLine("Choose expense category:");
            int i = 1;
            foreach (ExpenseCategory category in Enum.GetValues(typeof(ExpenseCategory)))
            {
                Console.WriteLine($"{i++}. {category}");
            }

            if (Enum.TryParse(Console.ReadLine(), out ExpenseCategory expenseCategory))
            {
                if (Enum.IsDefined(typeof(ExpenseCategory), expenseCategory))
                {
                    Console.Write("Enter expense amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        Console.Write("Enter expense date (yyyy-MM-dd): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                        {
                            SplitAmount(amount, out int expenseIntPart, out int expenseFracPart);

                            
                             
                            var newExpense = new Expense
                            {
                                _Value = new Money('+', expenseIntPart, expenseFracPart, wallet.Currency),
                                Category = expenseCategory,
                                Date = date
                            };
                            wallet._balance.MSub2(newExpense._Value);

                            wallet.Operations.Add(newExpense);


                            Console.WriteLine("Expense added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid category.");
                }
            }
            else
            {
                Console.WriteLine("Invalid category.");
            }
        }



        public static void LogOut(SessionManager sessionManager)
        {
            sessionManager.LogOut();
            Console.WriteLine("You have been logged out. Goodbye!");
        }

        private const string UsersFilePath = "users.json";

        public static void SaveUsers(SessionManager sessionManager)
        {
            List<User> users = sessionManager.GetUsers();

            string json = JsonConvert.SerializeObject(users, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            try
            {
                using (StreamWriter writer = new StreamWriter("users.json"))
                {
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }


        public static List<User> LoadUsers()
        {
            if (File.Exists("users.json"))
            {
                string json = File.ReadAllText("users.json");
                return JsonConvert.DeserializeObject<List<User>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                }) ?? new List<User>();
            }
            else
            {
                return new List<User>();
            }
        }


    }
}
