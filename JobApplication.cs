using System;

namespace JobApplicationTracker;

public class JobApplication
{
    // Fälten på lappen
    public string CompanyName { get; set; }
    public string PositionTitle { get; set; }
    public Status Status { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime? ResponseDate { get; set; }   // ? = får vara tomt
    public int SalaryExpectation { get; set; }

    // Konstruktor: körs när en ny lapp skapas
    public JobApplication(string companyName, string positionTitle,
                          DateTime applicationDate, int salaryExpectation)
    {
        CompanyName = companyName;
        PositionTitle = positionTitle;
        ApplicationDate = applicationDate;
        SalaryExpectation = salaryExpectation;
        Status = Status.Applied;   // nya ansökningar börjar som Applied
        ResponseDate = null;       // inget svar ännu
    }

    // Antal dagar sedan ansökan skickades
    public int GetDaysSinceApplied()
    {
        return (DateTime.Today - ApplicationDate.Date).Days;
    }

    // Kort sammanfattning av lappen
    public string GetSummary()
    {
        string response = ResponseDate.HasValue
            ? $"svar {ResponseDate.Value:yyyy-MM-dd}"
            : "inget svar ännu";

        return $"{CompanyName} - {PositionTitle} | {Status} | skickad {ApplicationDate:yyyy-MM-dd} " +
               $"({GetDaysSinceApplied()} dagar sedan) | {response} | {SalaryExpectation} kr";
    }
}