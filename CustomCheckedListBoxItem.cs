using System.Drawing;

namespace DesktopOrganize
{
    public class CustomCheckedListBoxItem
    {
        public string Text { get; set; }
        public Image Icon { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
