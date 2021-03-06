﻿using UnityEngine;

public class TrapDoor : MonoBehaviour
{
	[SerializeField] private Animator _animator = default;
	[SerializeField] private EntityAudio _doorAudio = default;
	[SerializeField] private Vector2 _promptOffset = default;


	public void OpenDoor()
	{
		_doorAudio.Play("DoorOpen");
		_animator.SetTrigger("Open");
	}

	public void CloseDoor()
	{
		_animator.SetTrigger("Close");
	}

	public Vector2 GetInteractPromptPosition()
	{
		Vector2 interactPromptPosition = new Vector2(transform.position.x + _promptOffset.x, transform.position.y + _promptOffset.y);
		return interactPromptPosition;
	}
}
