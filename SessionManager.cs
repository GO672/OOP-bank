using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace MyWill
{

    class User
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }

        public List<Wallet> Wallets { get; } = new List<Wallet>();
    }

    class SessionManager
    {
        private List<User> users;
        private Wallet activeWallet;


        public void SetActiveWallet(Wallet wallet)
        {
            activeWallet = wallet;
        }


        public SessionManager()
        {
            users = Functions.LoadUsers();
        }

        private User currentUser;

        public User CurrentUser
        {
            get { return currentUser; }
        }

        public bool IsUserLoggedIn
        {
            get { return currentUser != null; }
        }

        public void LogIn(User user)
        {
            currentUser = user;
        }

        public void LogOut()
        {
            currentUser = null;
        }

        public User GetActiveUser()
        {
            return CurrentUser;
        }

        public List<User> GetUsers()
        {
            return users;
        }

    }


    public class Wallet
    {
        public string Id { get; set; }
        public Currency Currency { get; set; }

        public Money _balance ;

        public int BalanceIntPart
        {
            get { return _balance.GetIntegerPart(); }
            set { _balance.SetIntegerPart(value); }
        }

        public int BalanceFracPart
        {
            get { return _balance.GetFractionalPart(); }
            set { _balance.SetFractionalPart(value); }
        }

        public List<Operation> Operations { get; } = new List<Operation>();
    }


    public abstract class Operation
    {
        public Money _Value;
        public int ValueIntPart
        {
            get { return _Value.GetIntegerPart(); }
            set { _Value.SetIntegerPart(value); }
        }

        public int ValueFracPart
        {
            get { return _Value.GetFractionalPart(); }
            set { _Value.SetFractionalPart(value); }
        }
        public DateTime Date { get; set; }
    }

    public class Expense : Operation
    {
        public ExpenseCategory Category { get; set; }
    }

    public class Income : Operation
    {
        public IncomeCategory Category { get; set; }
    }



}
