namespace LocalFileSharing.DesktopUI.Messages {
    public class ErrorMessage {
        public string Title { get; private set; }

        public string Description { get; private set; }

        public ErrorMessage(string title, string description) {

            Title = title;
            Description = description;
        }
    }
}
