using UnityEngine;

public class ButtonFlameColorChange : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // I tried animating using Color Over Lifetime, but that doesnt work with aanimation clips
    // So instead, animate the start color, and with this trigger we simply switch between states.
    public void OnButtonClicked()
    {
        animator.SetTrigger("TriggerColorChange");
    }
}
