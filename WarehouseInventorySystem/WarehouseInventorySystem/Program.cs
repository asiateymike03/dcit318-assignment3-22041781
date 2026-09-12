using System;
using System.Collections.Generic;

// Custom exceptions
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}
public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}
public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// Marker interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Product classes
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    { Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths; }

    public override string ToString() =>
        $"[Electronic] {Id}: {Name} ({Brand}), Qty {Quantity}, Warranty {WarrantyMonths} months";
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    { Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate; }

    public override string ToString() =>
        $"[Grocery] {Id}: {Name}, Qty {Quantity}, Expires {ExpiryDate:d}";
}

// Generic repository 
public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id) =>
        _items.TryGetValue(id, out var item)
            ? item
            : throw new ItemNotFoundException($"Item with ID {id} not found.");

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Cannot remove: item {id} not found.");
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException($"Quantity cannot be negative (got {newQuantity}).");
        GetItemById(id).Quantity = newQuantity;
    }
}

// WareHouseManager
public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Phone", 25, "Samsung", 12));
        _electronics.AddItem(new ElectronicItem(3, "TV", 8, "LG", 36));

        _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 50, DateTime.Now.AddMonths(6)));
        _groceries.AddItem(new GroceryItem(102, "Milk", 30, DateTime.Now.AddDays(7)));
        _groceries.AddItem(new GroceryItem(103, "Bread", 20, DateTime.Now.AddDays(2)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        Console.WriteLine($"--- {typeof(T).Name} ---");
        foreach (var item in repo.GetAllItems()) Console.WriteLine(item);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock increased: {item.Name} now {item.Quantity}.");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item {id} removed.");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }
}

public class Program
{
    public static void Main()
    {
        var wh = new WareHouseManager();
        wh.SeedData();

        wh.PrintAllItems(wh._groceries);
        wh.PrintAllItems(wh._electronics);

        // Error demonstrations
        try
        {
            wh._groceries.AddItem(new GroceryItem(101, "Duplicate Rice", 5, DateTime.Now)); // duplicate
        }
        catch (DuplicateItemException ex) { Console.WriteLine($"Duplicate: {ex.Message}"); }

        try
        {
            wh._electronics.RemoveItem(999); // not found
        }
        catch (ItemNotFoundException ex) { Console.WriteLine($"Not found: {ex.Message}"); }

        try
        {
            wh._groceries.UpdateQuantity(102, -5); // invalid quantity
        }
        catch (InvalidQuantityException ex) { Console.WriteLine($"Invalid qty: {ex.Message}"); }
    }
}