﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public bool HasEvent = false;
        public StringBuilder CodeBehind = new StringBuilder();

        public ControlTypeOutput(Control inputControl)
        {

            string type = inputControl.GetType().FullName.Replace("System.Windows.Forms.", "").Trim();

            if(ParseDesign.ControlConvertor.ContainsKey(type) == true)
            {
                type = ParseDesign.ControlConvertor[type];
            }

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
            Instantiate.AppendLine("");
            Instantiate.AppendLine(string.Format("            AddControl(this.{0});", inputControl.Name));

            string cType = inputControl.GetType().Name;

            if (ParseDesign.ControlConvertor.ContainsKey(cType) == true)
            {
                cType = ParseDesign.ControlConvertor[cType];
            }

            switch (cType)
            {
                case "TabControl":
                    HandleTabControls(inputControl);
                    break;
                case "GroupBox":
                    HandleGroupBox(inputControl);
                    break;
            }

            Instantiate.AppendLine(string.Format("            {0}.Add(this.{1});", ParseDesign.WindowName, inputControl.Name));
      
            HasEvent = HasEventHandler(inputControl, "EventClick");

            if(HasEvent == true)
            {
                Instantiate.AppendLine(string.Format("            this.{0}.Click += {1}Events.On{0}Click;", inputControl.Name, ParseDesign.DerivedName));
            }

            Instantiate.AppendLine("");
            Instantiate.AppendLine(string.Format("            #endregion {0}", inputControl.Name));
            Instantiate.AppendLine("");


        }

        private void HandleGroupBox(Control inputControl)
        {
            for (int i = 0; i < inputControl.Controls.Count; i++)
            {
                Control control = inputControl.Controls[i];
                    string subType = control.GetType().FullName.Replace("System.Windows.Forms.", "").Trim();

                if (ParseDesign.ControlConvertor.ContainsKey(subType) == true)
                {
                    subType = ParseDesign.ControlConvertor[subType];
                }


                if (subType == "SpinBox")
                {
                    Instantiate.AppendLine(string.Format("            {1} {0} = new {1}({2}, SpinBox.);", control.Name, subType, ParseDesign.ManagerName));
                }
                else
                {
                    Instantiate.AppendLine(string.Format("            {1} {0} = new {1}({2});", control.Name, subType, ParseDesign.ManagerName));
                }
                    Instantiate.AppendLine(string.Format("            {0}.Init();", control.Name));
                    Instantiate.AppendLine(string.Format("            {0}.Name = \"{1}\";", control.Name, control.Name));
                    Instantiate.AppendLine(string.Format("            {0}.Height = {1};", control.Name, control.Height));
                    Instantiate.AppendLine(string.Format("            {0}.Width = {1};", control.Name, control.Width));
                    Instantiate.AppendLine(string.Format("            {0}.Top = {1};", control.Name, control.Top));
                    Instantiate.AppendLine(string.Format("            {0}.Left = {1};", control.Name, control.Left));
                    Instantiate.AppendLine(string.Format("            {0}.Text = \"{1}\";", control.Name, control.Text));
                    Instantiate.AppendLine(string.Format("            {0}.TextColor = Color.{1};", control.Name, GetColorName(control.ForeColor)));
                    Instantiate.AppendLine("");
                    Instantiate.AppendLine(string.Format("            this.{0}.Add({1});", inputControl.Name, control.Name));

                    bool hasEvent = HasEventHandler(control, "EventClick");

                if (hasEvent == true)
                {
                    Instantiate.AppendLine(string.Format("            {0}.Click += {1}Events.On{0}Click;", control.Name, ParseDesign.DerivedName));

                    if (hasEvent == true)
                    {

                        CodeBehind.AppendLine(string.Format("        public static void On{0}Click(object sender, EventArgs e)", control.Name));
                        CodeBehind.AppendLine("        {");
                        CodeBehind.AppendLine("");
                        CodeBehind.AppendLine("        }");
                        CodeBehind.AppendLine("");
                    }
                }

                Instantiate.AppendLine("");
                    Instantiate.AppendLine(string.Format("           AddControl({0});", control.Name));


            }
        }

        private void HandleTabControls(Control inputControl)
        {
            for (int i = 0; i < inputControl.Controls.Count; i++)
            {
                Instantiate.AppendLine(string.Format("            this.{0}.AddPage(\"{1}\");", inputControl.Name, inputControl.Controls[i].Name));
                Instantiate.AppendLine(string.Format("            this.{0}.TabPages[{1}].Text = \"{2}\";", inputControl.Name, i, inputControl.Controls[i].Text));
                Instantiate.AppendLine("            // Adding sub controls to tabs.");
                Instantiate.AppendLine("");

                foreach (Control control in inputControl.Controls[i].Controls)
                {
                    string subType = control.GetType().FullName.Replace("System.Windows.Forms.", "").Trim();

                    if (ParseDesign.ControlConvertor.ContainsKey(subType) == true)
                    {
                        subType = ParseDesign.ControlConvertor[subType];
                    }

                    Instantiate.AppendLine(string.Format("            {1} {0} = new {1}({2});", control.Name, subType, ParseDesign.ManagerName));
                    Instantiate.AppendLine(string.Format("            {0}.Init();", control.Name));
                    Instantiate.AppendLine(string.Format("            {0}.Name = \"{1}\";", control.Name, control.Name));
                    Instantiate.AppendLine(string.Format("            {0}.Height = {1};", control.Name, control.Height));
                    Instantiate.AppendLine(string.Format("            {0}.Width = {1};", control.Name, control.Width));
                    Instantiate.AppendLine(string.Format("            {0}.Top = {1};", control.Name, control.Top));
                    Instantiate.AppendLine(string.Format("            {0}.Left = {1};", control.Name, control.Left));
                    Instantiate.AppendLine(string.Format("            {0}.Text = \"{1}\";", control.Name, control.Text));
                    Instantiate.AppendLine(string.Format("            {0}.TextColor = Color.{1};", control.Name, GetColorName(control.ForeColor)));
                    Instantiate.AppendLine("");
                    Instantiate.AppendLine(string.Format("            this.{0}.TabPages[{1}].Add({2});", inputControl.Name, i, control.Name));
                    Instantiate.AppendLine("");
                    Instantiate.AppendLine(string.Format("           AddControl({0});", inputControl.Name));
                }


            }

        }

        private bool HasEventHandler(Control control, string eventName)
        {
            EventHandlerList events =
                (EventHandlerList)
                typeof(Component)
                 .GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance)
                 .GetValue(control, null);

            object key = typeof(Control)
                .GetField(eventName, BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);

            Delegate handlers = events[key];

            return handlers != null && handlers.GetInvocationList().Any();
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
