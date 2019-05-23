using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Singleton<T> : UnityEngine.MonoBehaviour where T:Singleton<T>
{
    private static List<T> instances = new List<T>();
    public static T Instance
    {
        get
        {
            for(var i= instances.Count-1; i>=0;i--)
            {
                if (instances[i] && instances[i].gameObject.scene != null)
                    return instances[i];
            }
            return null;
        }
    }

    public Singleton() : base()
    {
        instances.Add(this as T);
    }
}