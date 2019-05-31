using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[System.AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class EditorButtonAttribute : Attribute
{
    public string Label { get; private set; }
    public EditorButtonAttribute(string label = "")
    {
        Label = label;
    }
}
