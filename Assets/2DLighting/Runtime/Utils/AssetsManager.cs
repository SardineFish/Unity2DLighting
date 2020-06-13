using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lighting2D
{
    public class AssetsManager<T> where T: UnityEngine.Object
    {
        static Lazy<AssetsManager<T>> manager = new Lazy<AssetsManager<T>>(() => new AssetsManager<T>());
        static AssetsManager<T> Instance => manager.Value;

        public static void Register(T asset)
            => Instance.assets.Add(asset);

        public static void Remove(T asset)
            => Instance.assets.Remove(asset);

        public static IEnumerable<T> Assets => Instance.assets.Where(asset => asset);

        HashSet<T> assets = new HashSet<T>();

    }

    public abstract class ManagedMonobehaviour<T> : MonoBehaviour where T : ManagedMonobehaviour<T>
    {

        public class AssetsManager : AssetsManager<T>
        {
        }

        public ManagedMonobehaviour() : base()
        {
            AssetsManager.Register(this as T);
        }

        protected virtual void Awake()
        {
        }
        protected virtual void OnDestroy()
        {
            AssetsManager.Remove(this as T);
        }
    }
}
