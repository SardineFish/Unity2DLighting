using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lighting2D
{
    public class ShaderPool : ScriptableObject
    {
        static ShaderPool _instance;
        static ShaderPool Instance
        {
            get
            {
                if (!_instance)
                    _instance = CreateInstance<ShaderPool>();
                return _instance;
            }
        }

        Dictionary<string, Material> pool = new Dictionary<string, Material>();

        public static Material Get(string name)
        {
            if(!Instance.pool.ContainsKey(name) || !Instance.pool[name])
            {
                var material = new Material(Shader.Find(name));
                Instance.pool[name] = material;
                return material;
            }
            return Instance.pool[name];
        }
    }

}
