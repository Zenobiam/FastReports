using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    public abstract class CommandBase
    {
        private string name;
        private string separator;
        private string terminator;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Separator
        {
            get { return separator; }
            set { separator = value; }
        }
        public string Terminator
        {
            get { return terminator; }
            set { terminator = value; }
        }

        public CommandBase()
        {
            separator = ",";
            terminator = ";";
        }
        public abstract void AppendTo(StringBuilder s);
    }

    public class CommandBase<T> : CommandBase
    {
        private List<T> parameters;

        public List<T> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public CommandBase()
        {
            parameters = new List<T>();
        }

        public override void AppendTo(StringBuilder s)
        {
            s.Append(Name);
            for (int i = 0; i < parameters.Count; i++)
            {
                s.Append(parameters[i]);
                if (i < parameters.Count - 1)
                    s.Append(Separator);
            }
            s.Append(Terminator);
        }
    }
}
