using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace UIParser
{
    public class ControlTypeOutput
    {
        public string Declaration = string.Empty;
        public StringBuilder Instantiate = new StringBuilder();
       

        public ControlTypeOutput(Control inputControl)
        {

            string type = inputControl.GetType().FullName.Replace("System.Windows.Forms.", "").Trim();

            Declaration = string.Format("        private {0} {1};", type,
                                                               inputControl.Name);
            Instantiate.AppendLine("");
            Instantiate.AppendLine(string.Format("            #region {0} setup", inputControl.Name));
            Instantiate.AppendLine("");
            Instantiate.AppendLine(string.Format("            this.{0} = new {1}({2});", inputControl.Name, type, ParseDesign.ManagerName));
            Instantiate.AppendLine(string.Format("            this.{0}.Init();", inputControl.Name));
            Instantiate.AppendLine(string.Format("            this.{0}.Name = \"{1}\";", inputControl.Name, inputControl.Name));
            Instantiate.AppendLine(string.Format("            this.{0}.Height = {1};", inputControl.Name, inputControl.Height));
            Instantiate.AppendLine(string.Format("            this.{0}.Width = {1};", inputControl.Name, inputControl.Width));
            Instantiate.AppendLine(string.Format("            this.{0}.Top = {1};", inputControl.Name, inputControl.Top));
            Instantiate.AppendLine(string.Format("            this.{0}.Left = {1};", inputControl.Name, inputControl.Left));
            Instantiate.AppendLine(string.Format("            this.{0}.Text = \"{1}\";", inputControl.Name, inputControl.Text));
            Instantiate.AppendLine(string.Format("            this.{0}.TextColor = Color.{1};", inputControl.Name, GetColorName(inputControl.ForeColor)));
            Instantiate.AppendLine(string.Format("            {0}.Add(this.{1});", ParseDesign.WindowName, inputControl.Name));
            Instantiate.AppendLine("");
            Instantiate.AppendLine(string.Format("            #endregion {0}", inputControl.Name));
            Instantiate.AppendLine("");
        }

        private string GetColorName(Color color)
        {
            var colorLookup = typeof(Color)
               .GetProperties(BindingFlags.Public | BindingFlags.Static)
               .Select(f => (Color)f.GetValue(null, null))
               .Where(c => c.IsNamedColor)
               .ToLookup(c => c.ToArgb());

            // There are some colours with multiple entries...
            foreach (var namedColor in colorLookup[color.ToArgb()])
            {
                return namedColor.Name;
            }

            return "";
        }
    }
}
