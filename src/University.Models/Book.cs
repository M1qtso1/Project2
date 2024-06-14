using System;

namespace University.Models
{
    public class Book
    {
        public long BookId { get; set; } = 0;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime? PublicationDate { get; set; } = null;
        public string ISBN { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public bool IsSelected { get; set; } = false; 
        public virtual ICollection<Subject>? Subjects { get; set; }
    }
}
