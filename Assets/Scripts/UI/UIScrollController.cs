using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollController : MonoBehaviour
{
    public Animator anim;
    public GameObject scrollContent;

    bool rolled;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rolled = false;
    }

    public void Roll()
    {
        anim.Play("scroll_rolling");
    }

    public void Unroll()
    {
        anim.Play("scroll_unrolling");
    }

    public void Reverse()
    {
        rolled = !rolled;
        if (rolled == true) Unroll();
        else Roll();
    }

    public void ShowContent(int show)
    {
        scrollContent.SetActive(System.Convert.ToBoolean(show));
    }
}