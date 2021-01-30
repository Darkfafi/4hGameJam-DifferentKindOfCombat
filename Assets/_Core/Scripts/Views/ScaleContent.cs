using DG.Tweening;
using UnityEngine;

public class ScaleContent : MonoBehaviour
{
	[SerializeField]
	private Transform _targetContent;

	[SerializeField]
	private float _duration = 0.6f;

	protected void OnEnable()
	{
		_targetContent.DOKill();
		_targetContent.localScale = Vector3.zero;
		_targetContent.DOScale(Vector3.one, _duration).SetEase(Ease.OutBack);
	}

	protected void OnDisable()
	{
		_targetContent.DOKill();
	}
}
