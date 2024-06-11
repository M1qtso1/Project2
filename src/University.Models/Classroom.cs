namespace University.Models
{
    public class Classroom
    {
        public long ClassroomId { get; set; } = 0;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int AvailableSeats { get; set; }
        public bool Projector { get; set; }
        public bool Whiteboard { get; set; }
        public bool Microphone { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
