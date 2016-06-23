using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.Control;

namespace UIParser
{
    public static class ParseDesign
    {
        public static string ManagerName = "Manager";
        public static string WindowName = "MainWindow";
        /// <summary>
        /// Example: "UserInterface.Windows"
        /// </summary>
        public static string NameSpace = "<Change to your namespace in the game project.>";
        public static string BaseWindow = ": BaseWindow";
        /// <summary>
        /// Example:@"C:\Users\lakedoo23\Documents\Visual Studio 2013\Projects\MudClientV1\UserInterface\Windows"
        /// </summary>
        public static string OutputDirectory = @"<Change to folder with UI windows>"; 

        private static Dictionary<string, ControlTypeOutput> _controls = new Dictionary<string, ControlTypeOutput>();

        static ParseDesign()
        {

        }

        public static void Input(Form input)
        {
            StringBuilder sb = new StringBuilder();

            GenerateNeoControlCode(input.Controls);

            sb.AppendLine("using TomShane.Neoforce.Controls;");
            sb.AppendLine("using Microsoft.Xna.Framework;");
            sb.AppendLine(string.Format("namespace {0}", NameSpace));
            sb.AppendLine("{");

            if (BaseWindow.Length > 0)
            {
                sb.AppendLine(string.Format("    public class {0} {1}".Trim(), input.Name, BaseWindow));
            }
            else
            {
                sb.AppendLine(string.Format("    public class {0}".Trim(), input.Name));
            }

            sb.AppendLine("    {");
            sb.AppendLine("");

            if (BaseWindow.Length > 0)
            {
                sb.AppendLine(string.Format("        public {0}(Manager manager) : base(manager)", input.Name));
            }
            else
            {
                sb.AppendLine(string.Format("        public {0}(Manager manager)", input.Name));
            }

            sb.AppendLine("        {");
            sb.AppendLine(string.Format("            {0}.ClientHeight = {1};", WindowName, input.Height));
            sb.AppendLine(string.Format("            {0}.ClientWidth = {1};", WindowName, input.Width));
            sb.AppendLine(string.Format("            {0}.Text = \"{1}\";", WindowName, input.Text));
            sb.AppendLine(string.Format("            {0}.Top = {1};", WindowName, input.Top));
            sb.AppendLine(string.Format("            {0}.Left = {1};", WindowName, input.Left));
            
            sb.AppendLine("");
            sb.AppendLine("        }");

            sb.AppendLine("");

            foreach (var control in _controls)
            {
                sb.AppendLine(control.Value.Declaration);
            }

            sb.AppendLine("");
            sb.AppendLine("        public override void Initialize()");
            sb.AppendLine("        {");
            
            foreach (var control in _controls)
            {
                sb.AppendLine(control.Value.Instantiate.ToString());
            }

            sb.AppendLine("        }");
            sb.AppendLine("   }");
            sb.AppendLine("}");

            File.WriteAllText(string.Format(@"{0}\{1}.cs", OutputDirectory, input.Name), sb.ToString());
        }

        private static void GenerateNeoControlCode(ControlCollection controls)
        {
            for(int i = 0; i < controls.Count; i++)
            {
                _controls.Add(controls[i].Name, new ControlTypeOutput(controls[i]));
            }
        }
    }
}
