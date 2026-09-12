using System;
using System.Collections.Generic;
using System.Linq;

// Generic Repository
public class Repository<T>
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);
    public List<T> GetAll() => new List<T>(_items);
    public T? GetById(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var match = _items.FirstOrDefault(predicate);
        return match != null && _items.Remove(match);
    }
}

// (b, c) Entities
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    { Id = id; Name = name; Age = age; Gender = gender; }

    public override string ToString() => $"[{Id}] {Name}, {Age}, {Gender}";
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    { Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued; }

    public override string ToString() =>
        $"{MedicationName} issued {DateIssued:d} (Patient {PatientId})";
}

// HealthSystemApp 
public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo = new();
    private readonly Repository<Prescription> _prescriptionRepo = new();
    private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Alice Mensah", 28, "Female"));
        _patientRepo.Add(new Patient(2, "Kwame Boateng", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Ama Serwaa", 12, "Female"));

        _prescriptionRepo.Add(new Prescription(101, 1, "Paracetamol", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(102, 1, "Vitamin C", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(103, 2, "Amoxicillin", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(104, 2, "Ibuprofen", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(105, 3, "Cough Syrup", DateTime.Now));
    }

    // Group prescriptions by PatientId
    public void BuildPrescriptionMap()
    {
        foreach (var rx in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(rx.PatientId))
                _prescriptionMap[rx.PatientId] = new List<Prescription>();
            _prescriptionMap[rx.PatientId].Add(rx);
        }
    }

    // Lookup from the map
    public List<Prescription> GetPrescriptionsByPatientId(int patientId) =>
        _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();

    public void PrintAllPatients()
    {
        Console.WriteLine("PATIENTS");
        foreach (var p in _patientRepo.GetAll()) Console.WriteLine(p);
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        Console.WriteLine($"\nPrescriptions for Patient {id}");
        foreach (var rx in GetPrescriptionsByPatientId(id)) Console.WriteLine(rx);
    }
}

public class Program
{
    public static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        app.PrintPrescriptionsForPatient(1);
    }
}