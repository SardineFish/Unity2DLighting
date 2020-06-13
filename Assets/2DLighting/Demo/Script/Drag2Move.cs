using UnityEngine;


namespace Lighting2D.Demo
{
    public class Drag2Move : MonoBehaviour
    {
        public bool Drag = false;
        private void OnMouseDown()
        {
            Drag = true;
        }
        private void OnMouseUp()
        {
            Drag = false;
        }
        private void Update()
        {
            if (Drag)
            {
                var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                var pos = camera.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                transform.position = pos;
            }
        }
    }
}