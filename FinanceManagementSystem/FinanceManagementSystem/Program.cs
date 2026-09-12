using System;
using System.Collections.Generic;

// Record for financial data Record
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Concrete processors
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[Bank Transfer] Processing {t.Category}: GHc{t.Amount} on {t.Date:d}");
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[Mobile Money] Sent GHc{t.Amount} for {t.Category} (ID {t.Id})");
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[Crypto Wallet] Paid {t.Amount} BTC-equivalent for {t.Category} on {t.Date:d}");
}

// Account
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// Sealed SavingsAccount
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. Updated balance: GHc{Balance}");
        }
    }
}

// FinanceApp
public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("ACC-001", 1000m);

        var t1 = new Transaction(1, DateTime.Now, 200m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 150m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 900m, "Entertainment");

        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();
        ITransactionProcessor p3 = new CryptoWalletProcessor();

        p1.Process(t1); account.ApplyTransaction(t1);
        p2.Process(t2); account.ApplyTransaction(t2);
        p3.Process(t3); account.ApplyTransaction(t3); 

        _transactions.AddRange(new[] { t1, t2, t3 });
        Console.WriteLine($"Total transactions logged: {_transactions.Count}");
    }
}

public class Program
{
    public static void Main()
    {
        new FinanceApp().Run();
        Console.ReadKey();
    }
}