using MyWill;
using System;
using System.Collections.Generic;

class Program
{
    static List<User> users = new List<User>();
    static SessionManager sessionManager = new SessionManager();

    static void Main()
    {
        users = Functions.LoadUsers();

        Console.WriteLine("Welcome to the Finance Management App!");

        while (true)
        {
            Console.WriteLine("\nOptions:");
            if (!sessionManager.IsUserLoggedIn)
            {
                Console.WriteLine("1. Log In");
                Console.WriteLine("2. Sign Up");
            }
            else
            {
                Console.WriteLine("3. Log Out");
                Console.WriteLine("4. Choose Wallet");
                Console.WriteLine("5. Create Wallet");
                Console.WriteLine("6. Delete Wallet");
            }

            Console.WriteLine("7. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Functions.LogIn(sessionManager);

                    break;

                case "2":
                    Functions.SignUp(sessionManager);
                    break;

                case "3":
                    if (sessionManager.IsUserLoggedIn)
                    {
                        Functions.LogOut(sessionManager);
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please choose again.");
                    }
                    break;

                case "4":
                    Functions.ChooseWallet(sessionManager);
                    break;

                case "5":
                    Functions.CreateWallet(sessionManager);
                    break;

                case "6":
                    Functions.DeleteWallet(sessionManager);
                    break;

                case "7":
                    Console.WriteLine("Exiting the application. Goodbye!");
                    Functions.SaveUsers(sessionManager);
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please choose again.");
                    break;
            }
        }
    }
}
