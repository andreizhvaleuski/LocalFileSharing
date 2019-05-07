namespace LocalFileSharing.DesktopUI.Messages {
    public class ErrorMessage {
        public string Title { get; private set; }

        public string Description { get; private set; }

        public ErrorMessage(string title, string description) {
            if (string.IsNullOrWhiteSpace(title)) {
                throw new System.ArgumentException("message", nameof(title));
            }

            if (string.IsNullOrWhiteSpace(description)) {
                throw new System.ArgumentException("message", nameof(description));
            }

            Title = title;
            Description = description;
        }
    }
}
