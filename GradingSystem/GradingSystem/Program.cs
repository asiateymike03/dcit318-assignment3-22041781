using System;
using System.Collections.Generic;
using System.IO;

// Student
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    { Id = id; FullName = fullName; Score = score; }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

// Custom exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}
public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// Processor
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string? line;
            int lineNumber = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 3)
                    throw new MissingFieldException(
                        $"Line {lineNumber}: expected 3 fields (ID, Name, Score) but got {parts.Length}.");

                if (!int.TryParse(parts[0].Trim(), out int id))
                    throw new InvalidScoreFormatException($"Line {lineNumber}: ID '{parts[0]}' is not a valid number.");
                if (!int.TryParse(parts[2].Trim(), out int score))
                    throw new InvalidScoreFormatException($"Line {lineNumber}: score '{parts[2]}' is not a valid integer.");

                students.Add(new Student(id, parts[1].Trim(), score));
            }
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("STUDENT GRADE REPORT");
            foreach (var s in students)
                writer.WriteLine($"{s.FullName} (ID:{s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        try
        {
            var processor = new StudentResultProcessor();
            var students = processor.ReadStudentsFromFile("students.txt");
            processor.WriteReportToFile(students, "report.txt");
            Console.WriteLine("Report generated successfully.");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: the input file was not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Score format error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Missing data: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}