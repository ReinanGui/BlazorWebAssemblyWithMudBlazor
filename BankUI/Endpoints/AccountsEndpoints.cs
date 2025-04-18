﻿namespace BankUI.Endpoints
{
    public class AccountsEndpoints
    {
        public const string Add = "api/accounts/add";
        public const string Transact = "api/accounts/transact";
        public const string GetAll = "api/accounts/all";

        public static string GetById(int id)
        {
            return $"api/accounts/by-id/{id}";
        }

        public static string GetByAccountId(int id)
        {
            return $"api/accounts/transactions/{id}";
        }

        public static string GetByAccountNumber(string accountNumber)
        {
            return $"api/accounts/by-account-number/{accountNumber}";
        }
    }
}
