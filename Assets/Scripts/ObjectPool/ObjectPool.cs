using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPool
{
    private Queue<Dot> _dots = new Queue<Dot>();
	private Dot _prefab;
	private Transform _parent;
	public ObjectPool(int count, Dot dot, Transform parent)
	{
		_parent = parent;
		_prefab = dot;
		Init(count);
	}
	private void Init(int count)
	{
        for (int i = 0; i < count; i++)
        {
			CreateDot();
        }
    }

	private void CreateDot()
	{
        Dot dot = MonoBehaviour.Instantiate(_prefab, _parent);
        dot.gameObject.SetActive(false);
        _dots.Enqueue(dot);
    }

	public Dot Get()
	{
        Dot dot = null;

        if (_dots.Count == 0)
		{
            dot = MonoBehaviour.Instantiate(_prefab, _parent);
			dot.gameObject.SetActive(true);
			return dot;
		}
		dot = _dots.Dequeue();
		dot.gameObject.SetActive(true);

		return dot;
	}

	public void Return(Dot dot)
	{
		dot.gameObject.SetActive(false);
		_dots.Enqueue(dot);
	}


}
