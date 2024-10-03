namespace oa.Models
{
    public class LogModel
    {
        public int Id { get; set; }
        public DateTime ChangeDate { get; set; }
        public string UserId { get; set; }
        public string ChangedFrom { get; set; }
        public string ChangedTo { get; set; }
    }
}
