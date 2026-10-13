using System;
using System.Globalization;
using System.Text;

namespace JobApplicationTracker;

public class Program
{
    // ===== DEL 1: Menyn och loopen =====
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;   // så att åäö visas rätt

        JobManager manager = new JobManager();
        bool running = true;

        while (running)   // loopar tills användaren väljer att avsluta
        {
            ShowMenu();
            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1": AddJobFromInput(manager); break;
                case "2": manager.ShowAll(); break;
                case "3": manager.ShowByStatus(ReadStatus("Vilken status vill du filtrera på?")); break;
                case "4": manager.ShowSortedByDate(); break;
                case "5": manager.ShowStatistics(); break;
                case "6": UpdateStatusFromInput(manager); break;
                case "7": RemoveJobFromInput(manager); break;
                case "8": manager.ShowWaitingOverTwoWeeks(); break;
                case "9": AddTestData(manager); break;
                case "0":
                    Console.WriteLine("Hej då och lycka till med jobbsökandet!");
                    running = false;
                    break;
                default:
                    PrintError("Ogiltigt val. Välj ett nummer från menyn.");
                    break;
            }

            if (running)
            {
                Console.WriteLine();
                Console.Write("Tryck Enter för att fortsätta...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("===== Job Application Tracker =====");
        Console.WriteLine("1. Lägg till ny ansökan");
        Console.WriteLine("2. Visa alla ansökningar");
        Console.WriteLine("3. Filtrera efter status");
        Console.WriteLine("4. Sortera efter datum");
        Console.WriteLine("5. Visa statistik");
        Console.WriteLine("6. Uppdatera status");
        Console.WriteLine("7. Ta bort en ansökan");
        Console.WriteLine("8. Utan svar i över 14 dagar");
        Console.WriteLine("9. Lägg till testdata");
        Console.WriteLine("0. Avsluta");
        Console.Write("Ditt val: ");
    }

    // ===== DEL 2: Åtgärder som frågar användaren =====
    private static void AddJobFromInput(JobManager manager)
    {
        Console.WriteLine("--- Ny ansökan ---");
        string company = ReadText("Företag: ");
        string position = ReadText("Titel: ");
        DateTime date = ReadDate("Ansökningsdatum");
        int salary = ReadInt("Önskad lön (kr): ");

        manager.AddJob(new JobApplication(company, position, date, salary));
        PrintSuccess("Ansökan sparad!");
    }

    private static void UpdateStatusFromInput(JobManager manager)
    {
        if (manager.Applications.Count == 0)
        {
            PrintError("Det finns inga ansökningar att uppdatera.");
            return;
        }

        manager.ShowAll();
        Console.WriteLine();
        int number = ReadInt("Nummer på ansökan: ");
        Status newStatus = ReadStatus("Vilken ny status?");

        if (manager.UpdateStatus(number, newStatus))
            PrintSuccess("Status uppdaterad!");
        else
            PrintError("Det finns ingen ansökan med det numret.");
    }

    private static void RemoveJobFromInput(JobManager manager)
    {
        if (manager.Applications.Count == 0)
        {
            PrintError("Det finns inga ansökningar att ta bort.");
            return;
        }

        manager.ShowAll();
        Console.WriteLine();
        int number = ReadInt("Nummer på ansökan som ska tas bort: ");

        if (manager.RemoveJob(number))
            PrintSuccess("Ansökan borttagen!");
        else
            PrintError("Det finns ingen ansökan med det numret.");
    }

    // Exempeldata så att du kan testa LINQ direkt
    private static void AddTestData(JobManager manager)
    {
        JobApplication a = new JobApplication("Spotify", "Backend-utvecklare", DateTime.Today.AddDays(-30), 45000);
        a.Status = Status.Interview;
        a.ResponseDate = DateTime.Today.AddDays(-20);

        JobApplication b = new JobApplication("Volvo", "Junior utvecklare", DateTime.Today.AddDays(-25), 38000);
        b.Status = Status.Offer;
        b.ResponseDate = DateTime.Today.AddDays(-10);

        JobApplication c = new JobApplication("Klarna", "Systemutvecklare", DateTime.Today.AddDays(-18), 42000);
        c.Status = Status.Rejected;
        c.ResponseDate = DateTime.Today.AddDays(-5);

        manager.AddJob(a);
        manager.AddJob(b);
        manager.AddJob(c);
        manager.AddJob(new JobApplication("Ericsson", "Mjukvaruutvecklare", DateTime.Today.AddDays(-20), 41000));
        manager.AddJob(new JobApplication("King", "Spelutvecklare", DateTime.Today.AddDays(-3), 39000));

        PrintSuccess("5 testansökningar tillagda!");
    }

    // ===== DEL 3: Inmatning med felhantering =====
    // Varje metod frågar om och om igen tills användaren skriver något giltigt.
    private static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) return input.Trim();
            PrintError("Fältet får inte vara tomt.");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            // TryParse försöker göra om text till tal utan att krascha
            if (int.TryParse(input, out int value) && value >= 0) return value;
            PrintError("Ogiltigt tal. Skriv ett heltal som är 0 eller större.");
        }
    }

    private static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (ÅÅÅÅ-MM-DD, Enter = idag): ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return DateTime.Today;

            if (DateTime.TryParseExact(input.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime date))
            {
                if (date.Date <= DateTime.Today) return date;
                PrintError("Datumet kan inte ligga i framtiden.");
                continue;
            }

            PrintError("Ogiltigt datum. Använd formatet ÅÅÅÅ-MM-DD, till exempel 2026-10-12.");
        }
    }

    private static Status ReadStatus(string prompt)
    {
        Console.WriteLine(prompt);
        Status[] all = Enum.GetValues<Status>();

        for (int i = 0; i < all.Length; i++)
            Console.WriteLine($"  {i + 1}. {all[i]}");

        while (true)
        {
            Console.Write("Ditt val: ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int number) && number >= 1 && number <= all.Length)
                return all[number - 1];
            PrintError("Ogiltigt val. Skriv ett nummer från listan.");
        }
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}