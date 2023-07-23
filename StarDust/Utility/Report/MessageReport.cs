using StarDust.Code.Text;

namespace StarDust.Code
{
    public sealed class Report
    {
        private Report(TextLocation location, string message, bool isError)
        {
            Message = message;
            Location = location;
            IsError = isError;
            IsWarning = !IsError;
        }

        public string Message { get; }
        public bool IsError { get; }
        public bool IsWarning { get; }
        public TextLocation Location { get; }

        public override string ToString()
        {
            return Message;
        }

        public static Report Error(TextLocation location, string message)
        {
            return new(location, message, isError: true);
        }

        public static Report Warning(TextLocation location, string message)
        {
            return new(location, message, isError: false);
        }
    }
}
