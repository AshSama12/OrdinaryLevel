namespace OdinaryLevel.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}