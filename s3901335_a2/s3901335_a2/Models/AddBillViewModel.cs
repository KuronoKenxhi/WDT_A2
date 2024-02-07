namespace s3901335_a2.Models;
public class AddBillViewModel
{
    public int AccountNumber { get; set; }
    public int PayeeID { get; set; }
    public decimal Amount { get; set; }
    public DateTime ScheduleTimeLocal { get; set; }
    public DateTime ScheduleTimeUtc { get; set; }
    public Period Period { get; set; }

    public List<int> AccountNumbers { get; set; }
    public List<string> PayeeNames { get; set; }
    public string PayeeName { get; set; }
}