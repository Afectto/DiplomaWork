using System;
using System.Collections;
using Coffee.UIExtensions;
using UnityEngine;

public class TutorialPart : MonoBehaviour
{
    [SerializeField] private TutorialPartName currentName;
    [SerializeField] private TutorialPartName nextName;

    [SerializeField] private bool isNeedClickInUnmask;
    
    private UnmaskRaycastFilter _unmaskFilter;
    private bool _isAlreadyInvoke;
    
    public TutorialPartName CurrentName => currentName;
    public TutorialPartName NextName => nextName;

    public event Action<TutorialPart> OnNextPart;
    
    private void Awake()
    {
        if (isNeedClickInUnmask)
        {
            _unmaskFilter = GetComponentInChildren<UnmaskRaycastFilter>();
            _unmaskFilter.OnClickInTargetUnmasks += unmask =>
            {
                if (Input.GetMouseButtonUp(0))
                {
                    StartCoroutine(SingleInvoke());
                }
            };
        }
    }

    private void Update()
    {
        if (!isNeedClickInUnmask && Input.GetMouseButtonUp(0))
        {
            StartCoroutine(SingleInvoke());
        }
    }

    private IEnumerator SingleInvoke()
    {
        if (_isAlreadyInvoke) yield break;
        
        _isAlreadyInvoke = true;
        yield return new WaitForSecondsRealtime(0.2f);
        OnNextPart?.Invoke(this);
        _isAlreadyInvoke = false;
    }

    public enum TutorialPartName
    {
        Buy,
        Play,
        BallsDestroy,
        Shield,
        Chain,
        MainMenu,
        Craft,
        CraftResult,
        Info,
        Trash,
        
        None,
        CraftResultDrag,
        SecondBuy,
        HoldToAim
    }
}
