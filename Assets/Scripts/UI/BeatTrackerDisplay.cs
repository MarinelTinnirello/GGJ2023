using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class BeatTrackerDisplay : MonoBehaviour
{
    public Image displayBar;
    public float repeatTimer = 1f;

    //[Range(0f, 1f)]
    private float minFill;
    private float resetTime = 0.5f;
    private Coroutine startTrackerCO;

    private Tween fillTween;
    private Sequence fillSequence;

    private bool isReseting,isActive;
    
    public void InitTracker(float setTime, float delay = 0f)
    {
        repeatTimer = setTime;
        
        CallStartTracker(delay);
    }

    public void CallStartTracker(float delay)
    {
        if (!displayBar) return;
        if (isActive) StopCoroutine(startTrackerCO);

        isActive = true;
        displayBar.fillAmount = minFill;
        
        startTrackerCO = StartCoroutine(StartTracker(delay));
    }

    public void EndTracker()
    {
        isActive = false;

        fillSequence.Kill();
        fillTween.Kill();
        fillTween = displayBar.DOFillAmount(minFill, .25f).SetUpdate(true);
    }

    private IEnumerator StartTracker(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        RunSequence();

        /*while (isActive) { };

        while (isActive)
        {
            isReseting = false;

            fillTween.Kill();
            fillTween = displayBar.DOFillAmount(1, repeatTimer - resetTime).SetUpdate(true);
            
            yield return new WaitForSeconds(repeatTimer - resetTime);
            
            isReseting = true;

            GameManager.Instance?.UIManager?.ShowBeatHint(resetTime-.25f, .35f);

            fillTween.Kill();
            fillTween = displayBar.DOFillAmount(minFill, resetTime).SetUpdate(true);

            yield return new WaitForSeconds(resetTime);
        }
        
        fillTween.Kill();
        fillTween = displayBar.DOFillAmount(minFill, .25f).SetUpdate(true);*/
    }

    private void RunSequence()
    {
        fillSequence = DOTween.Sequence();

        fillSequence.Append(displayBar.DOFillAmount(1, resetTime))
                    .InsertCallback(0, () => { OnLoop(false); })
                    .AppendCallback(() => { OnLoop(true); })
                    .Append(displayBar.DOFillAmount(minFill, resetTime))
                    .SetLoops(600);
    }

    private void OnLoop(bool isResetLoop)
    {
        isReseting = isResetLoop;

        if(!isReseting){
            
        } else {
            GameManager.Instance?.UIManager?.ShowBeatHint(-.25f, .35f);
        }
    }

    public float GetAccuracy()
    {
        return isReseting? -1 : displayBar? displayBar.fillAmount : 0f;//-1f* displayBar.fillAmount	
        //return isReseting ? 0 : displayBar ? (displayBar.fillAmount - minFill) * (1 / minFill) : 0f;
    }
}
