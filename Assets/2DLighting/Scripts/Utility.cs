using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Lighting2D
{
    public static class Utility
    {
        public static void ForEach<T>(this IEnumerable<T> ts, Action<T> callback)
        {
            foreach (var item in ts)
                callback(item);
        }

        public static IEnumerable<T> RandomTake<T>(this IEnumerable<T> list, int count)
        {
            var source = list.ToArray();

            for (var i = 0; i < count; i++)
            {
                var idx = UnityEngine.Random.Range(0, source.Length - i);
                yield return source[idx];
                source[idx] = source[count - i - 1];
            }
        }

        public static IEnumerable<GameObject> GetChildren(this GameObject gameObject)
        {
            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                yield return gameObject.transform.GetChild(i).gameObject;
            }
        }


        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> collection, Func<T, U> callback) => collection.Select(callback);



        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                if (item != null)
                    yield return item;
            }
        }

        public static TResult Merge<TResult, Tkey, TElement>(this IGrouping<Tkey, TElement> group, TResult mergeTarget, Func<TElement, TResult, TResult> mergeFunc)
        {
            foreach (var element in group)
            {
                mergeTarget = mergeFunc(element, mergeTarget);
            }
            return mergeTarget;
        }

        public static GameObject Instantiate(this UnityEngine.Object self, GameObject original, Scene scene)
        {
            var obj = UnityEngine.Object.Instantiate(original);
            SceneManager.MoveGameObjectToScene(obj, scene);
            return obj;
        }

        public static GameObject Instantiate(GameObject original, Scene scene)
        {
            var obj = UnityEngine.Object.Instantiate(original);
            SceneManager.MoveGameObjectToScene(obj, scene);
            return obj;
        }
        public static GameObject Instantiate(GameObject original, GameObject parent)
        {
            var obj = Instantiate(original, parent.scene);
            obj.transform.SetParent(parent.transform);
            return obj;
        }

        public static GameObject Instantiate(GameObject original, GameObject parent, Vector3 relativePosition, Quaternion relativeRotation)
        {
            var obj = Instantiate(original, parent);
            obj.transform.localPosition = relativePosition;
            obj.transform.localRotation = relativeRotation;
            return obj;
        }

        public static void ClearChildren(this GameObject self)
        {
            self.GetChildren().ForEach((child) =>
            {
                child.ClearChildren();
                GameObject.Destroy(child);
            });
        }

        public static void ClearChildImmediate(this GameObject self)
        {
            while (self.transform.childCount > 0)
            {
                var obj = self.transform.GetChild(0).gameObject;
                obj.ClearChildImmediate();
                GameObject.DestroyImmediate(obj);
            }
        }

        public static void SetLayerRecursive(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            gameObject.GetChildren().ForEach(child => child.SetLayerRecursive(layer));
        }

        public static void NextFrame(this MonoBehaviour context, Action callback)
        {
            context.StartCoroutine(NextFrameCoroutine(callback));
        }

        public static IEnumerator NextFrameCoroutine(Action callback)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
        }

        public static Coroutine NumericAnimate(this MonoBehaviour context, float time, Action<float> tick, Action complete = null)
        {
            return context.StartCoroutine(NumericAnimateEnumerator(time, tick, complete));
        }

        public static IEnumerator NumericAnimateEnumerator(float time, Action<float> callback, Action complete)
        {
            var startTime = Time.time;
            for (float t = 0; t < time; t = Time.time - startTime)
            {
                callback?.Invoke(t / time);
                yield return new WaitForEndOfFrame();
            }
            callback?.Invoke(1);
            complete?.Invoke();
        }

        public static Coroutine WaitForSecond(this MonoBehaviour context, Action callback, float seconds = 0)
        {
            return context.StartCoroutine(WaitForSecondEnumerator(callback, seconds));
        }

        public static Coroutine SetInterval(this MonoBehaviour context, Action callback, float seconds = 0)
        {
            return context.StartCoroutine(IntervalCoroutine(callback, seconds));
        }

        public static IEnumerator IntervalCoroutine(Action callback, float seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                callback?.Invoke();
            }
        }

        public static IEnumerator WaitForSecondEnumerator(Action callback, float seconds = 0)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }

        public static IEnumerable<float> Timer(float time)
        {
            for (var startTime = Time.time; Time.time < startTime + time;)
            {
                yield return Time.time - startTime;
            }
            yield return time;
        }
        public static IEnumerable<float> TimerNormalized(float time)
        {
            foreach (var t in Timer(time))
            {
                yield return t / time;
            }
        }

        public class CallbackYieldInstruction : CustomYieldInstruction
        {
            Func<bool> callback;
            public override bool keepWaiting => callback?.Invoke() ?? true;

            public CallbackYieldInstruction(Func<bool> callback)
            {
                this.callback = callback;
            }
        }

        public static IEnumerator ShowUI(UnityEngine.UI.Graphic ui, float time, float targetAlpha = 1)
        {
            var color = ui.color;
            color.a = 0;
            ui.gameObject.SetActive(true);
            foreach (var t in TimerNormalized(time))
            {
                color.a = t * targetAlpha;
                ui.color = color;
                yield return null;
            }
        }

        public static IEnumerator ShowUI(CanvasGroup canvasGroup, float time)
        {
            time = (1 - canvasGroup.alpha) * time;
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(true);
            var alpha = canvasGroup.alpha;
            foreach (var t in TimerNormalized(time))
            {
                canvasGroup.alpha = alpha + t * (1 - alpha);
                yield return null;
            }
        }

        public static IEnumerator HideUI(CanvasGroup canvasGroup, float time)
        {
            foreach (var t in TimerNormalized(time))
            {
                canvasGroup.alpha = 1 - t;
                yield return null;
            }
            canvasGroup.gameObject.SetActive(false);
        }

        public static IEnumerator HideUI(UnityEngine.UI.Graphic ui, float time, bool deactive = false)
        {
            var color = ui.color;
            color.a = 1;
            foreach (var t in TimerNormalized(time))
            {
                color.a = 1 - t;
                ui.color = color;
                yield return null;
            }
            if (deactive)
                ui.gameObject.SetActive(false);
        }
        public static T GetInterface<T>(this Component component)
            => (T)(object)(component.GetComponents<Component>().Where(c => c is T).FirstOrDefault());
        public static T GetInterface<T>(this GameObject obj)
            => (T)(object)obj.GetComponents<Component>().Where(c => c is T).FirstOrDefault();
        public static bool All<T>(this IEnumerable<T> ts, Func<T, int, bool> predicate)
        {
            int idx = 0;
            foreach (var item in ts)
            {
                if (!predicate(item, idx++))
                    return false;
            }
            return true;
        }
        public static bool Any<T>(this IEnumerable<T> ts, Func<T, int, bool> predicate)
        {
            int idx = 0;
            foreach (var item in ts)
            {
                if (predicate(item, idx++))
                    return true;
            }
            return false;
        }

        public static void ForceDestroy(GameObject gameObject)
        {
            if (Application.isPlaying)
                GameObject.Destroy(gameObject);
            else
                GameObject.DestroyImmediate(gameObject);
        }

        public static void DestroyChildren(this GameObject gameObject)
        {
            if (Application.isPlaying)
            {
                foreach (var child in gameObject.GetChildren())
                {
                    child.SetActive(false);
                    GameObject.Destroy(child);
                }
            }
            else
            {
                var count = gameObject.transform.childCount;
                for (var i = 0; i < count; i++)
                {
                    GameObject.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
                }
            }
        }

        public static bool Diff<T>(this IEnumerable<T> ts, IEnumerable<T> target) where T : class
            => Diff<T, T>(ts, target, (s, t) => s == t);

        public static bool Diff<T, U>(this IEnumerable<T> ts, IEnumerable<U> target, Func<T, U, bool> comparerer)
        {
            var targetEnumerator = target.GetEnumerator();
            foreach (var element in ts)
            {
                if (!targetEnumerator.MoveNext())
                    return false;
                var targetElement = targetEnumerator.Current;
                if (!comparerer(element, targetElement))
                    return false;
            }
            if (targetEnumerator.MoveNext())
                return false;
            return true;
        }

        public static Color RandomColor()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }

        public static Vector2 Sum<T>(this IEnumerable<T> ts, Func<T, Vector2> selector)
        {
            Vector2 v = Vector2.zero;
            foreach (var el in ts)
            {
                v += selector(el);
            }
            return v;
        }

        public static Vector3 Sum<T>(this IEnumerable<T> ts, Func<T, Vector3> selector)
        {
            Vector3 v = Vector3.zero;
            foreach (var el in ts)
            {
                v += selector(el);
            }
            return v;
        }

        public static GenericPlatform GetGenericPlatform(RuntimePlatform platform)
        {
            if (platform == RuntimePlatform.WindowsEditor ||
                platform == RuntimePlatform.WindowsPlayer ||
                platform == RuntimePlatform.LinuxEditor ||
                platform == RuntimePlatform.LinuxPlayer ||
                platform == RuntimePlatform.OSXEditor ||
                platform == RuntimePlatform.OSXPlayer)
                return GenericPlatform.Desktop;
            return GenericPlatform.Mobile;
        }

        public static IEnumerable<T> FindObjectOfAll<T>() where T : UnityEngine.Component
        {
            return Resources.FindObjectsOfTypeAll<T>()
                .Where(obj => obj.gameObject.scene != null && obj.gameObject.scene.isLoaded);
        }

        public static IEnumerable<int> Times(int times)
        {
            for (var i = 0; i < times; i++)
            {
                yield return i;
            }
        }

        public static bool IsInHierarchy(this GameObject gameObject)
        {
            return gameObject && gameObject.scene != null && gameObject.scene.name != null;
        }

    }

    public enum GenericPlatform
    {
        Desktop,
        Mobile,
    }
}
