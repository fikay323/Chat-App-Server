namespace Backend.Models {
    public class Message {
        public string content { get; set; }
        public string to { get; set; }
        public string sentBy { get; set; }
        public string senderName { get; set; }
        public string messageID { get; set; }
        public DateTime timestamp { get; set; }
    }
}
