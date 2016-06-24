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
        public static string NameSpace = "UserInterface.Windows";
        public static string BaseWindow = ": BaseWindow";
        public static string DerivedName = "";

        public static string OutputDirectory = @"C:\Users\lakedoo23\Documents\Visual Studio 2013\Projects\MudClientV1\UserInterface\Windows";

        private static Dictionary<string, ControlTypeOutput> _controls = new Dictionary<string, ControlTypeOutput>();

        static ParseDesign()
        {

        }

        public static void Input(Form input, bool generateCodeBehind)
        {
            StringBuilder layout = new StringBuilder();
            StringBuilder codeBehind = new StringBuilder();

            DerivedName = input.Name;

            GenerateNeoControlCode(input.Controls);


            layout.AppendLine(string.Format("using {0}.CodeBehind;", NameSpace));
            
            layout.AppendLine("using TomShane.Neoforce.Controls;");
            layout.AppendLine("using Microsoft.Xna.Framework;");
            layout.AppendLine(string.Format("namespace {0}", NameSpace));
            layout.AppendLine("{");

            if (BaseWindow.Length > 0)
            {
                layout.AppendLine(string.Format("    public class {0} {1}".Trim(), input.Name, BaseWindow));
            }
            else
            {
                layout.AppendLine(string.Format("    public class {0}".Trim(), input.Name));
            }

            codeBehind.AppendLine("using TomShane.Neoforce.Controls;");
            codeBehind.AppendLine(string.Format("namespace {0}.CodeBehind", NameSpace));
            codeBehind.AppendLine("{");
            codeBehind.AppendLine(string.Format("    public class {0}Events", input.Name));
            codeBehind.AppendLine("    {");
            codeBehind.AppendLine("");

            layout.AppendLine("    {");
            layout.AppendLine("");

            if (BaseWindow.Length > 0)
            {
                layout.AppendLine(string.Format("        public {0}(Manager manager) : base(manager)", input.Name));
            }
            else
            {
                layout.AppendLine(string.Format("        public {0}(Manager manager)", input.Name));
            }

            layout.AppendLine("        {");
            layout.AppendLine(string.Format("            {0}.ClientHeight = {1};", WindowName, input.Height));
            layout.AppendLine(string.Format("            {0}.ClientWidth = {1};", WindowName, input.Width));
            layout.AppendLine(string.Format("            {0}.Text = \"{1}\";", WindowName, input.Text));
            layout.AppendLine(string.Format("            {0}.Top = {1};", WindowName, input.Top));
            layout.AppendLine(string.Format("            {0}.Left = {1};", WindowName, input.Left));
            
            layout.AppendLine("");
            layout.AppendLine("        }");

            layout.AppendLine("");

            foreach (var control in _controls)
            {
                layout.AppendLine(control.Value.Declaration);
            }

            layout.AppendLine("");
            layout.AppendLine("        public override void Initialize()");
            layout.AppendLine("        {");
            
            foreach (var control in _controls)
            {
                layout.AppendLine(control.Value.Instantiate.ToString());

                if(control.Value.HasEvent == true)
                {

                    codeBehind.AppendLine(string.Format("        public static void On{0}Click(object sender, EventArgs e)", control.Key));
                    codeBehind.AppendLine("        {");
                    codeBehind.AppendLine("");
                    codeBehind.AppendLine("        }");
                    codeBehind.AppendLine("");
                }
            }

            codeBehind.AppendLine("   }");
            codeBehind.AppendLine("}");

            layout.AppendLine("        }");
            layout.AppendLine("   }");
            layout.AppendLine("}");

            File.WriteAllText(string.Format(@"{0}\{1}.cs", OutputDirectory, input.Name), layout.ToString());

            if (generateCodeBehind == true)
            {
                File.WriteAllText(string.Format(@"{0}\CodeBehind\{1}Events.cs", OutputDirectory, input.Name), codeBehind.ToString());
            }
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
