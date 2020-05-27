using System.Text;

namespace DFC.App.FindACourse.ViewModels
{
    public class Robot
    {
        private readonly StringBuilder robotData;

        public Robot()
        {
            this.robotData = new StringBuilder();
        }

        public string Data => robotData.ToString();

        public void Add(string text)
        {
            this.robotData.AppendLine(text);
        }
    }
}