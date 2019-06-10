using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Lighting2D.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    class ButtonEditorHelper : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            target.GetType().GetMethods()
                .Where(method => method.GetCustomAttributes(typeof(EditorButtonAttribute), true).FirstOrDefault() != null)
                .ForEach(method =>
                {
                    var attr = method.GetCustomAttributes(typeof(EditorButtonAttribute), true).FirstOrDefault() as EditorButtonAttribute;
                    var label = attr.Label;
                    if (label == "")
                        label = method.Name;
                    if (GUILayout.Button(label))
                        method.Invoke(target, new object[] { });
                });
        }
    }
}