using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable inventory record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded)
    : IInventoryEntity;

// Generic inventory logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_log, options);
            File.WriteAllText(_filePath, json);
            Console.WriteLine($"Saved {_log.Count} item(s) to {_filePath}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File error while saving: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Permission denied while saving: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error while saving: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No saved file found — starting with an empty log.");
                return;
            }

            string json = File.ReadAllText(_filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);

            _log.Clear();
            if (items != null)
            {
                _log.AddRange(items);
            }
            Console.WriteLine($"Loaded {_log.Count} item(s) from {_filePath}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"The data file is corrupt or not valid JSON: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File error while loading: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error while loading: {ex.Message}");
        }
    }
}

// Integration layer
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger = new("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Printer Paper", 100, DateTime.Now.AddDays(-10)));
        _logger.Add(new InventoryItem(2, "USB Cables", 45, DateTime.Now.AddDays(-8)));
        _logger.Add(new InventoryItem(3, "Mouse Pads", 60, DateTime.Now.AddDays(-5)));
        _logger.Add(new InventoryItem(4, "Webcams", 15, DateTime.Now.AddDays(-2)));
        _logger.Add(new InventoryItem(5, "Keyboards", 30, DateTime.Now));
        Console.WriteLine("Sample data seeded.");
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        Console.WriteLine("INVENTORY");
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("(no items)");
            return;
        }
        foreach (var item in items)
        {
            Console.WriteLine($"#{item.Id} {item.Name} | Qty: {item.Quantity} | Added: {item.DateAdded:d}");
        }
    }
}

// Main application flow 
public class Program
{
    public static void Main()
    {
        var app = new InventoryApp();
        app.SeedSampleData();
        app.SaveData();

        // Simulate a brand-new session: reload from disk and verify
        Console.WriteLine("\nNew session started");
        var freshApp = new InventoryApp();
        freshApp.LoadData(); 
        freshApp.PrintAllItems();        
    }
}