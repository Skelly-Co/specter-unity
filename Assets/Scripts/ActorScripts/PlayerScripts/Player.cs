﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInputSystem))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(EntityAudio))]
public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator = default;
    [SerializeField] private Transform _meleePoint = default;
    [SerializeField] private Transform _firePoint = default;
    [SerializeField] private GameObject _aim = default;
    [SerializeField] private GameObject _meleeEffectPrefab = default;
    [SerializeField] private GameObject _shotEffectPrefab = default;
    [SerializeField] private GameObject _shotImpactEffectPrefab = default;
    [SerializeField] private GameObject _deathEffectPrefab = default;
    [SerializeField] private SpriteRenderer _spriteRenderer = default;
    [SerializeField] private PlayerMovement _playerMovement = default;
    [SerializeField] private PlayerInputSystem _playerInputSystem = default;
    [SerializeField] private PlayerUI _playerUI = default;
    [SerializeField] private PlayerAim _playerAim = default;
    [SerializeField] private EntityAudio _playerAudio = default;
    private readonly float _meleeAttackCooldown = 0.3f;
    private readonly float _invisibilityFrames = 0.2f;
    private readonly int _invisibilityRepeat = 4;
    private int _maxHearts;
    private int _currentHealth;
    private int _attack;
    private bool _isInventoryOpen;
    private bool _isChargingShot;
    private bool _isInvisible;
    private bool _hasDied;

    public bool IsAttacking { get; set; }


    public void Melee()
    {
        if (!IsAttacking && _playerMovement.IsGrounded && !_playerAim.IsChargingDash && !_playerMovement.IsDashing)
        {
            _playerAudio.Play("PlayerMelee");
            _animator.SetTrigger("Melee");
            Damager damager = Instantiate(_meleeEffectPrefab, _meleePoint.position, _meleePoint.rotation).GetComponent<Damager>();
            damager._damageAmount = _attack;
            StartCoroutine(AttackCooldown(_meleeAttackCooldown, true));
        }
    }

    IEnumerator AttackCooldown(float attackCooldown, bool lockRotation)
    {
        IsAttacking = true;
        _playerMovement.LockMovement(true);
        if (lockRotation)
            _playerMovement.LockRotation(true);
        yield return new WaitForSeconds(attackCooldown);
        _animator.SetBool("IsCharging", false);
        IsAttacking = false;
        _playerMovement.LockMovement(false);
        if (lockRotation)
            _playerMovement.LockRotation(false);
    }

    public void ChargeShot()
    {
        if (!IsAttacking && _playerMovement.IsGrounded && !_playerAim.IsChargingDash && !_playerMovement.IsDashing)
        {
            _playerAudio.Play("PlayerChargeSwift");
            _animator.SetBool("IsCharging", true);
            _playerMovement.LockMovement(true);
            IsAttacking = true;
        }
    }

    public void HasCharged()
    {
        _playerAudio.Play("PlayerShotCharged");
        _playerAudio.Play("PlayerChargeShot");
        _isChargingShot = true;
    }

    public void Shoot()
    {
        if (_isChargingShot)
        {
            _playerAudio.Play("PlayerShot");
            _animator.SetTrigger("Shoot");
            Instantiate(_shotImpactEffectPrefab, _firePoint.position, _firePoint.rotation);
            Damager damager = Instantiate(_shotEffectPrefab, _firePoint.position, _firePoint.rotation).GetComponent<Damager>();
            damager._damageAmount = _attack;
        }
        _playerAudio.Stop("PlayerChargeShot");
        _animator.SetBool("IsCharging", false);
        _playerMovement.LockMovement(false);
        _isChargingShot = false;
        IsAttacking = false;
    }

    public void TakeDamage(int damageAmount, GameObject damagerObject)
    {
        if (!_isInvisible && !_hasDied)
        {
            _playerAudio.Play("PlayerHurt");
			_currentHealth -= damageAmount;
            _playerUI.StatsUI.SetHearts(_maxHearts, _currentHealth);
            if (_currentHealth <= 0)
            {
                _hasDied = true;
                Die();
            }
            else
            {
                StartCoroutine(InvicibilityFrames());
                _animator.SetTrigger("Hurt");
                _playerMovement.KnockBack(damagerObject);
            }
            //_playerMovement.ResetPlayerMovement();
        }
    }

    private void Die()
    {
        _playerAudio.Play("PlayerDie");
        _animator.SetTrigger("Die");
        _playerInputSystem.enabled = false;
        _playerMovement.LockMovement(true);
        Vector2 _playerHeadPosition = new Vector2(transform.position.x + 0.22f, transform.position.y + 1.18f);
        Instantiate(_deathEffectPrefab, _playerHeadPosition, transform.rotation);
        _playerUI.DeathUI.SetDeath(true);
    }

    public void SetHealth(int maxHearts, bool dontSetHearts)
    {
        _maxHearts = maxHearts;
        if (!dontSetHearts)
        {
            _currentHealth = _maxHearts;
        }
        _playerUI.StatsUI.SetHearts(_maxHearts, _currentHealth);
    }

    public void SetAttack(int attack)
    {
        _attack = attack;
    }

    public void Inventory()
    {
        _playerAudio.Play("PlayerInventoryOpen");
        if (!_isInventoryOpen)
        {
            _animator.SetBool("IsInventoryOpen", true);
            _playerInputSystem.enabled = false;
            _aim.SetActive(false);
            _playerMovement.LockMovement(true);
        }
        else
        {
            _animator.SetBool("IsInventoryOpen", false);
            _playerInputSystem.enabled = true;
            _aim.SetActive(true);
            _playerMovement.LockMovement(false);
        }
        _isInventoryOpen = !_isInventoryOpen;
    }

    IEnumerator InvicibilityFrames()
    {
        _isInvisible = true;
        for (int i = 0; i < _invisibilityRepeat; i++)
        {
            yield return new WaitForSeconds(_invisibilityFrames);
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
        }
        _isInvisible = false;
    }
}