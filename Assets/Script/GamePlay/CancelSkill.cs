using UnityEngine;
using UnityEngine.UI;

public class CancelSkill : MonoBehaviour
{
    // Start is called before the first frame update
    public bool checkCancel;
    void Start()
    {
        checkCancel = false;
    }
    // Update is called once per frame
    public void ExitCancelButton()
    {
        checkCancel = false;
    }
    public void EnterCancelButton()
    {
        checkCancel = true;
    }
}
