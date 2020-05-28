using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool ConformX;
    public bool ConformY;

	private void Awake ()
	{
        ApplySafeArea (Screen.safeArea);
    }

	void ApplySafeArea (Rect r)
    {
        RectTransform panel = gameObject.GetComponent<RectTransform> ();
        Debug.Log (r);
        // Ignore x-axis?
        if (!ConformX) {
            r.x = 0;
            r.width = Screen.width;
        }

        // Ignore y-axis?
        if (!ConformY) {
            r.y = 0;
            r.height = Screen.height;
        }

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates

        SetTop (panel, r.y);
        SetBottom (panel, Screen.height - r.y - r.height);

    }

    public void SetLeft (RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2 (left, rt.offsetMin.y);
    }

    public void SetRight (RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2 (-right, rt.offsetMax.y);
    }

    public void SetTop (RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2 (rt.offsetMax.x, -top);
    }

    public void SetBottom (RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2 (rt.offsetMin.x, bottom);
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
