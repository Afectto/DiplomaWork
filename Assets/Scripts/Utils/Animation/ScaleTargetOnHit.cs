using DG.Tweening;
using UnityEngine;

public class ScaleTargetOnHit : MonoBehaviour
{
    [SerializeField] private Transform skin;
    [SerializeField] private new ParticleSystem particleSystem;

    [SerializeField] private float endValueScale = 0.2f;
    [SerializeField] private float totalDuration = 0.2f;

    [SerializeField] private bool isShowParticleOnHit;
    
    private IInteractiveItemGame _point;
    
    private void Awake()
    {
        _point = GetComponent<IInteractiveItemGame>();
        
        _point.OnItemDestroy += PlayParticleOnDeath;
        _point.OnItemHit += HandleBallCollision;
    }

    private void HandleBallCollision()
    {
        var mySequence = DOTween.Sequence();
        mySequence
            .Append(skin.transform.DOScale(endValueScale, totalDuration/2)
                .OnComplete(PlayParticleIfNeed))
            .Append(skin.transform.DOScale(1f, totalDuration/2));
    }

    private void PlayParticleIfNeed()
    {
        if (!isShowParticleOnHit) return;
        if(particleSystem == null) return;
        particleSystem.Play();
    }
    
    private void PlayParticleOnDeath()
    {
        if (particleSystem == null) return;

        var tempParticleSystem = Instantiate(particleSystem, transform.position, Quaternion.identity);
        tempParticleSystem.Play();

        Destroy(tempParticleSystem.gameObject, tempParticleSystem.main.duration + tempParticleSystem.main.startLifetime.constantMax);
    }
}
