using UnityEngine;

public class BouleBase : MonoBehaviour {

    public bool start = true;

    public delegate void EventChange(bool up, bool start);
    public static event EventChange OnChange;

    public void goingDown()
    {
        if (OnChange != null)
            OnChange(false, start);
    }

    public void goingUp()
    {
        if (OnChange != null)
            OnChange(true, start);
    }
}
