using System;
using System.Collections.Generic;
using System.Linq;

namespace JobApplicationTracker;

public class JobManager
{
    // Pärmen som innehåller alla lappar
    public List<JobApplication> Applications { get; } = new List<JobApplication>();

    // Lägger en ny lapp i pärmen
    public void AddJob(JobApplication job)
    {
        Applications.Add(job);
    }

    // Ändrar status. "number" är numret som visas i listan (börjar på 1).
    public bool UpdateStatus(int number, Status newStatus)
    {
        if (number < 1 || number > Applications.Count)
        {
            return false;   // numret finns inte
        }

        JobApplication job = Applications[number - 1];
        job.Status = newStatus;

        // När företaget svarat sparar vi datumet (behövs för svarstid)
        if (newStatus == Status.Applied)
        {
            job.ResponseDate = null;
        }
        else if (job.ResponseDate == null)
        {
            job.ResponseDate = DateTime.Today;
        }

        return true;
    }

    // Tar bort en lapp ur pärmen
    public bool RemoveJob(int number)
    {
        if (number < 1 || number > Applications.Count)
        {
            return false;
        }

        Applications.RemoveAt(number - 1);
        return true;
    }


    // LINQ Where: bara lappar med vald status
    public void ShowByStatus(Status status)
    {
        List<JobApplication> matches = Applications
            .Where(a => a.Status == status)
            .ToList();

        PrintList($"Ansökningar med status {status}", matches);
    }

    // LINQ OrderBy: sorterar efter datum, äldst först
    public void ShowSortedByDate()
    {
        List<JobApplication> sorted = Applications
            .OrderBy(a => a.ApplicationDate)
            .ToList();

        PrintList("Sorterat efter datum (äldst först)", sorted);
    }

    // Extra filter: utan svar och äldre än 14 dagar
    public void ShowWaitingOverTwoWeeks()
    {
        List<JobApplication> waiting = Applications
            .Where(a => a.ResponseDate == null && a.GetDaysSinceApplied() > 14)
            .OrderByDescending(a => a.GetDaysSinceApplied())
            .ToList();

        PrintList("Utan svar i mer än 14 dagar", waiting);
    }




    // Visar alla ansökningar
    public void ShowAll()
    {
        PrintList("Alla ansökningar", Applications);
    }

    // Skriver ut en lista, med färg beroende på status
    private void PrintList(string title, List<JobApplication> jobs)
    {
        Console.WriteLine($"=== {title} ({jobs.Count} st) ===");

        if (jobs.Count == 0)
        {
            Console.WriteLine("Inga ansökningar att visa.");
            return;
        }

        foreach (JobApplication job in jobs)
        {
            int number = Applications.IndexOf(job) + 1;
            Console.ForegroundColor = GetColor(job.Status);
            Console.WriteLine($"{number}. {job.GetSummary()}");
            Console.ResetColor();
        }
    }

    // Grön = Offer, röd = Rejected, gul = Interview
    private static ConsoleColor GetColor(Status status)
    {
        return status switch
        {
            Status.Offer => ConsoleColor.Green,
            Status.Rejected => ConsoleColor.Red,
            Status.Interview => ConsoleColor.Yellow,
            _ => ConsoleColor.Gray
        };
    }
}