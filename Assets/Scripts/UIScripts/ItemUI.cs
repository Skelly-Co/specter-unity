﻿using UnityEngine;

public class ItemUI : MonoBehaviour
{
	[SerializeField] private Animator _animator = default;
	[SerializeField] private GameObject _itemCanvas = default;
	[SerializeField] private GameObject _playerPoints = default;
	[SerializeField] private PlayerMovement _playerMovement = default;
	private bool _isVisible;


	// This Class needs refactoring
	public void ShowItem(bool state)
	{
		_isVisible = true;
		_itemCanvas.SetActive(state);
		_animator.SetBool("IsOpen", state);
		if (!state)
		{
			_playerMovement.LockMovement(false);
			_playerPoints.SetActive(true);
			GlobalSettings._hasDash = true;
		}
		else
		{
			_playerMovement.LockMovement(true);
		}
	}


	void Update()
	{
		if (_isVisible)
		{
			if (Input.GetKeyDown(KeyCode.X))
			{
				ShowItem(false);
			}
		}
	}
}
