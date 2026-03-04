using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable
{
    public static PlayerController Instance;

    [SerializeField] private float speed;

    [SerializeField] private float dashSpeed;

    [SerializeField] private float dashDistance;

    [SerializeField] private float dashCooldown;

    [SerializeField] private GameObject dashEffect;

    private float _health;

    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private float shortJumpAmount = 5f;
    [SerializeField] private float highJumpAmount = 10f;


    [SerializeField] private float attackPower;
    [SerializeField] private GameObject attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private float specialAttackThreshold;

    [SerializeField] private float specialAttackPower;

    [SerializeField] private float pushBackDuration;
    [SerializeField] private float pushBackSpeed;

    private float _specialAttackAmount;

    [SerializeField] private GameObject inventory;

    [SerializeField] private GameObject controls;

    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip specialAttackSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip openInventorySound;

    [SerializeField] private Image healthFillImage;

    [SerializeField] private Image attackFillImage;

    [SerializeField] private Animator specialAttackVFX;

    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private Color takeDamageColor;
    [SerializeField] private float damageFlashDuration;

    [SerializeField] private TextMeshProUGUI pickUpPrompt;

    [SerializeField] private TextMeshProUGUI attackPrompt;

    [SerializeField] private TextMeshProUGUI blessingPrompt;

    [SerializeField] private TextMeshProUGUI usedItemPrompt;

    [SerializeField] private GameObject specialAttackInputIcon;

    private bool _onAttackableGround = true;

    private bool _flashDone = false;

    private BoxCollider2D _playerCollider;

    private Vector2 _originalColliderOffset;

    private Vector2 _dashColliderOffset;

    private Rigidbody2D _playerRB;   

    private Vector2 _direction;

    public Animator playerAnimator;

    private bool _onGround = false;

    private PlayerInput _playerInput;

    InputAction jumpAction;

    private Door _currentDoor = null;

    private bool _atDoor = false;

    private bool _wallJumping = false;

    private int _playerLayer;
    private int _enemyLayer ;

    private bool _playerKilled = false;

    private bool _respawned = false;

    private bool _dashActive = false;

    private bool _dashAvailable = true;

    private bool _playerProtected = false;

    private bool _pushedBack = false;

    private bool _pushedBackStarted = false;

    private float _dashStartPos;

    private bool _moveInputHeld = false;

    [SerializeField] private float popupDuration = 3f;   

    private Vector2 _afterDashDirection = Vector2.zero;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _health = maxHealth;

        _playerCollider = GetComponent<BoxCollider2D>();

        _originalColliderOffset = _playerCollider.offset;

        _dashColliderOffset = new Vector2(1.03f, _playerCollider.offset.y);

        _playerRB = GetComponent<Rigidbody2D>();       

        _playerInput = GetComponent<PlayerInput>();
        jumpAction = _playerInput.actions["Jump"];

        _playerLayer = LayerMask.NameToLayer("Player");

        _enemyLayer = LayerMask.NameToLayer("Enemy");

        Physics2D.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);

        _specialAttackAmount = 0f;

        UpdateSpecialAttackUI();

        Debug.Assert(specialAttackThreshold > 0f);
        Debug.Assert(maxHealth > 0f);
    }

    /// <summary>
    /// Called when player presses movement inputs
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!inventory.activeInHierarchy) //Only allow movement if inventory is not open
        {
            _moveInputHeld = true;

            if (!_dashActive) //Only store direction if dash is not active
            {
                _direction = context.ReadValue<Vector2>();
            }

            if (_dashActive)//Stores direction to be assigned after dash is over. This is for if player changes movement inputs mid dash 
            {
                _afterDashDirection = context.ReadValue<Vector2>();
            }

            if (context.phase == InputActionPhase.Canceled) // If player leaves movement input
            {
                _moveInputHeld = false;
            }
        }               
    }

    /// <summary>
    /// Called when player pesses dash input sets all required variables to true and plays dash animation
    /// </summary>
    /// <param name="context"></param>
    public void OnDash(InputAction.CallbackContext context)
    {
        if (!inventory.activeInHierarchy)
        {
            if (!context.started)
                return;

            if (_dashAvailable && _direction.x != 0f)
            {
                SoundManager.Instance.PlaySound(dashSound);
                dashEffect.SetActive(true);
                _dashAvailable = false;
                _dashStartPos = transform.position.x;
                _dashActive = true;
                _playerCollider.offset = _dashColliderOffset; //Move the collider to fit the dash sprite
                playerAnimator.Play("PlayerDash");
            }
        }
                  
    }

    /// <summary>
    /// Called by animation event at start of Player attack animation
    /// </summary>
    private void PlayAttackSound()
    {
        SoundManager.Instance.PlaySound(attackSound);
    }

    /// <summary>
    /// Called by animation event at start of Player special attack animation 
    /// </summary>
    private void PlaySpecialAttackSound()
    {
        SoundManager.Instance.PlaySound(specialAttackSound);
    }

    /// <summary>
    /// Called when player presses attack input
    /// </summary>
    /// <param name="context"></param>
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.started)
            return; 
        
        if (!inventory.activeInHierarchy)
        {
            playerAnimator.Play("PlayerMelee");
        }
        
    }

    /// <summary>
    /// Called when special attack input is pressed and sets required variables to execute special attack only if the special attack amount gained is equal to threshold else shows prompt 
    /// </summary>
    /// <param name="context"></param>
    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (!inventory.activeInHierarchy && _specialAttackAmount>=specialAttackThreshold)
        {
            _playerInput.enabled = false;
            playerAnimator.Play("PlayerPowerAttack");
            specialAttackVFX.gameObject.SetActive(true);
            specialAttackVFX.Play("SpecialAttack");
        }

        else if (_specialAttackAmount <= specialAttackThreshold)
        {
            StartCoroutine(ShowPrompt(attackPrompt));
        }

    }

    /// <summary>
    /// Called when player presses inventory open input
    /// </summary>
    /// <param name="context"></param>
    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        _direction.x = 0f;

        SoundManager.Instance.PlaySound(openInventorySound);    

        ToggleInventory();
    }

    /// <summary>
    /// Called when player presses controls menu input
    /// </summary>
    /// <param name="context"></param>
    public void OnOpenControls(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        _direction.x = 0f;

        ToggleControls();
    }

    /// <summary>
    /// Calls move function, function for dash only if active and push back if being pushed back
    /// </summary>
    void FixedUpdate()
    {
        Move();

        if (_dashActive)
        {
            Dash();
        }

        if (_pushedBack)
        {
            PushBack();
            if (!_pushedBackStarted)
            {
                StartCoroutine(PushBackDuration());
            }
        }
    }

    /// <summary>
    /// Calls jump function and shows special attack input icon only once player can execute special attack
    /// </summary>
    private void Update()
    {
        if (!inventory.activeInHierarchy)
        {
            Jump();
        }
               
        if (_specialAttackAmount >= specialAttackThreshold)
        {
            specialAttackInputIcon.SetActive(true);
        }
    }

    /// <summary>
    /// Sets player rigidbody velocity based on specified direction and speed and sets animator variable to play the move animation
    /// </summary>
    private void Move()
    {

        _playerRB.linearVelocityX = _direction.x * speed;

        Vector3 playerScale = transform.localScale;

        if (_playerRB.linearVelocityX > 0 || _playerRB.linearVelocityX < 0)
        {
            playerAnimator.SetBool("Moving", true);

            if (_direction.x > 0)
            {
                playerScale.x = Mathf.Abs(playerScale.x);
            }

            else if (_direction.x < 0)
            {
                playerScale.x = -1f * Mathf.Abs(playerScale.x);
            }

            transform.localScale = playerScale;

        }

        else
        {
            playerAnimator.SetBool("Moving", false);           
        }
    }


    private void Jump()
    {
        
        if (jumpAction.WasPressedThisFrame() && _onGround)  //Player jumps higher when jump input is held
        {            
            if (!_wallJumping) // Allows vertical jumping only if player is not on wall for wall jump
            {
                _playerRB.linearVelocity = new Vector2(_playerRB.linearVelocityX, highJumpAmount);
            }

            else
            {
                if (_direction.x != transform.localScale.x && _direction.x != 0f) //While wall jumping allows player to jump of only while holding input in direction opposite to that of wall
                {
                    _playerRB.linearVelocity = new Vector2(_playerRB.linearVelocityX, highJumpAmount);
                }
            }
        }

        if (jumpAction.WasReleasedThisFrame() && _playerRB.linearVelocityY > shortJumpAmount)// Player jumps shorter when jump input is tapped
        {            
            if (!_wallJumping)
            {
                _playerRB.linearVelocity = new Vector2(_playerRB.linearVelocityX, shortJumpAmount);
            }

            else
            {
                if (_direction.x != transform.localScale.x && _direction.x != 0f)
                {
                    _playerRB.linearVelocity = new Vector2(_playerRB.linearVelocityX, shortJumpAmount);
                }
            }
            
        }
    }

    /// <summary>
    /// Sets velocity of player to specified amount for dash and stops it if player has reached specified position
    /// </summary>
    private void Dash()
    {
        float currentPos = transform.position.x;
        
        if (Mathf.Abs(currentPos - _dashStartPos) <= dashDistance)
        {
            _playerRB.linearVelocityX = _direction.x * dashSpeed;
        }

        else
        {
            StopDash();
        }
        
    }

    /// <summary>
    /// Resets player to original velocity and animation
    /// </summary>
    private void StopDash()
    {
        dashEffect.SetActive(false); //Sets the dash stream effect to false
        _dashActive = false; 
        _playerCollider.offset = _originalColliderOffset; // Reset player collider to original position
        if (!_moveInputHeld) //Make the player stationary if move input was released mid dash 
        {
            _direction.x = 0f;
        }
        if (_afterDashDirection.x != 0f && _moveInputHeld) //Set player in whatever direction the latest moveinput was before dash ended
        {
            _direction.x = _afterDashDirection.x;
            _afterDashDirection = Vector2.zero;
        }
        playerAnimator.Play("PlayerIdle");
        StartCoroutine(DashCooldown());
    }

    /// <summary>
    /// Makes sure dash cannot be used in cooldown time
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashCooldown()
    {       
        yield return new WaitForSeconds(dashCooldown);
        _dashAvailable = true;
    }

    /// <summary>
    /// Pushes back player by certain amount
    /// </summary>
    public void PushBack()
    {
        _pushedBack = true;
        _playerRB.AddForce(new Vector2(pushBackSpeed * -1f * transform.localScale.x, 0.2f), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Amount of time player will be pushed back
    /// </summary>
    /// <returns></returns>
    private IEnumerator PushBackDuration()
    {
        _pushedBackStarted = true;
        yield return new WaitForSeconds(pushBackDuration);
        _pushedBack = false;
        _pushedBackStarted = false;
    }

    /// <summary>
    /// Set animation of player while not on ground
    /// </summary>
    public void AirBorne()
    {     
        playerAnimator.SetBool("Jump", true);   
    }

    /// <summary>
    /// Open or close inventory
    /// </summary>
    public void ToggleInventory()
    {
        bool inventoryOpen = inventory.activeInHierarchy;
        inventory.SetActive(!inventoryOpen);        
    }

    /// <summary>
    /// Open or close controls menu
    /// </summary>
    public void ToggleControls()
    {
        bool controlsOpen = controls.activeInHierarchy;
        controls.SetActive(!controlsOpen);       
    }

    /// <summary>
    /// Called by animation event in Attack animation. Damages all targets that fall within radius of attack point object, increases special attack amount gained with each hit and updates UI
    /// </summary>
    private void Attack()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, targetLayer);

        foreach (Collider2D target in targets)
        {
            IDamageable damageableTarget = target.GetComponent<IDamageable>();
            if (damageableTarget != null)
            {
                damageableTarget.ReduceHealth(attackPower);
                _specialAttackAmount += attackPower;
                UpdateSpecialAttackUI();
            }
        }        
    }

    /// <summary>
    /// Damages all targets that fall within radius of attack point object, resets special attack amount to 0 and updates UI
    /// </summary>
    private void SpecialAttack()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, targetLayer);

        foreach (Collider2D target in targets)
        {
            EnemyController enemyController = target.GetComponent<EnemyController>();
            enemyController.PushBack();

            IDamageable damageableTarget = target.GetComponent<IDamageable>();
            if (damageableTarget != null)
            {
                damageableTarget.ReduceHealth(specialAttackPower);               
            }
        }

        _specialAttackAmount = 0f;
        specialAttackInputIcon.SetActive(false);
        UpdateSpecialAttackUI();        
    }

    /// <summary>
    /// Updates visual special attack meter
    /// </summary>
    private void UpdateSpecialAttackUI()
    {
        if (attackFillImage != null)
        {
            attackFillImage.fillAmount = Mathf.Clamp01(_specialAttackAmount / specialAttackThreshold);           
        }
    }

    /// <summary>
    /// Called by animation event at end of special attack animation
    /// </summary>
    private void endSpecialAttack()
    {
        _playerInput.enabled = true;
        SpecialAttack();
        playerAnimator.Play("PlayerIdle");
        StopVFX();
    }

    /// <summary>
    /// Stops special attack vfx
    /// </summary>
    private void StopVFX()
    {
        specialAttackVFX.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called vy animation event at end of player attack animation
    /// </summary>
    private void endAttack()
    {        
        playerAnimator.Play("PlayerIdle");        
    }

    /// <summary>
    /// Player takes damage by certain amount
    /// </summary>
    /// <param name="damage"></param>
    public void ReduceHealth(float damage)
    {
        if (!_playerProtected)
        {
            TakeDamage(damage);
        }
    }

    private void TakeDamage(float damage)
    {
        _health -= damage;

        UpdateHealthUI();

        if (!_flashDone)
        {
            StartCoroutine(DamageFlash());
        }

        if (_health <= 0f)
        {
            _health = 0f;
            if (!_playerKilled)
            {
                StopCoroutine(DamageFlash());
                DestroyPlayer();
                _playerKilled = true;
            }
        }
    }

    /// <summary>
    /// Player flashes a ceratin color when hit
    /// </summary>
    /// <returns></returns>
    public IEnumerator DamageFlash()
    {
        _flashDone = true;
        Color orig = characterSprite.color;
        characterSprite.color = takeDamageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        characterSprite.color = orig;
        _flashDone = false;
    }

    /// <summary>
    /// Updates the visual health bar
    /// </summary>
    private void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = Mathf.Clamp01(_health / maxHealth);
        }
            
    }

    private void Heal(float amount)
    {
        _health += amount;

        if (_health >= maxHealth)
        {
            _health = maxHealth;
        }

        UpdateHealthUI();
    }

    /// <summary>
    /// Play death animation, disable player input, decrease life, respawn if not out of lives
    /// </summary>
    private void DestroyPlayer()
    {
        SoundManager.Instance.PlaySound(deathSound);
        
        Physics2D.IgnoreLayerCollision(_playerLayer, _enemyLayer, true);

        this.enabled = false;

        playerAnimator.Play("PlayerDeath");

        _playerInput.enabled = false;

        _respawned = false;

        if (!LivesController.Instance.DecreaseLife())
        {
            GameController.Instance.RespawnAtCheckpoint();
        }   
    }

    /// <summary>
    /// Reset all values of player
    /// </summary>
    public void Respawn()
    {        
        playerAnimator.Play("PlayerIdle");

        _health = maxHealth;

        UpdateHealthUI();

        Physics2D.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);

        this.enabled = true;

        _playerInput.enabled = true;

        _playerKilled = false;

        _respawned = true;

        _wallJumping = false;
    }

    public bool PlayerRespawned()
    {
        return _respawned;
    }

    public void SetOnGround(bool ground)
    {
        _onGround = ground;
        playerAnimator.SetBool("Jump", false);
    }

    public void SetOnAttackableGround(bool attackableGround)
    {
        _onAttackableGround = attackableGround;
    }

    /// <summary>
    /// To make sure enemies cant chase player off ground into air when player leaves
    /// </summary>
    /// <returns></returns>
    public bool IsOnAttackableGround()
    {
        return _onAttackableGround;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);
    }

    public bool IsAlive()
    {
        if (_health > 0f)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// The effect each item use has
    /// </summary>
    /// <param name="usedCollectible"></param>
    /// <param name="itemStat"></param>
    /// <returns></returns>
    public bool ItemUseImpact(Collectible usedCollectible, float itemStat)
    {
        switch (usedCollectible)
        {
            case Collectible.Key:
                if (_atDoor)
                {
                    _currentDoor.Unlock();
                    Debug.Log("Used key");
                    return true;
                }

                else
                {
                    Debug.Log("No door in range to use the key on.");
                    return false;
                }

            case Collectible.HealthPotion:
                Heal(itemStat);
                usedItemPrompt.text = $"Used Health Potion. Gained {itemStat} HP";
                StartCoroutine(ShowPrompt(usedItemPrompt));
                return true;

            case Collectible.Protection:
                Debug.Log("Protection used");
                _playerProtected = true;
                usedItemPrompt.text = $"Used Protection Potion. Immune to damage for {itemStat} seconds";
                StartCoroutine(ShowPrompt(usedItemPrompt));
                StartCoroutine(ProtectionDuration(itemStat));
                return true;
                               
        }

        return false;
    }

    //Provides Player immunity from any damage for certain amount of time
    private IEnumerator ProtectionDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        _playerProtected = false;
    }


    /// <summary>
    /// Provides player with upgrades
    /// </summary>
    /// <param name="upgrade"></param>
    public void BlessingUpgrade(int upgrade)
    {
        switch (upgrade)
        {            
            case 1:
                blessingPrompt.text = $"Blessing Received. Max health increased by 20!";
                maxHealth += 20f;
                UpdateHealthUI();
                break;

            case 2:
                blessingPrompt.text = $"Blessing Received. Attack Power increased by 5!";
                attackPower += 5f;
                break;

            case 3:
                blessingPrompt.text = $"Blessing Received. Gained a life!";
                LivesController.Instance.IncreaseLife();
                break;
        }        
        StartCoroutine(ShowPrompt(blessingPrompt));
    }

    /// <summary>
    /// Shows prompt on screen for specified amount of time
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    private IEnumerator ShowPrompt(TextMeshProUGUI prompt)
    {        
        prompt.gameObject.SetActive(true);

        yield return new WaitForSeconds(popupDuration);     

        prompt.gameObject.SetActive(false);
    }

    public void TogglePlayerInput(bool toggle)
    {
        _playerInput.enabled = toggle;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsAlive())
        {
            //Make player stop dashing if they collide with anything other than the ground
            if (!collision.gameObject.CompareTag("Ground"))
            {
                StopDash();
            }

            if (collision.gameObject.CompareTag("Ground") && !_onGround)
            {
                StopDash();
            }

            //Player picking up items 
            if (collision.gameObject.GetComponent<CollectibleController>())
            {
                CollectibleController collectibleController = collision.gameObject.GetComponent<CollectibleController>();

                if (InventoryManager.Instance.AddItem(collectibleController.GetItem()))
                {
                    pickUpPrompt.text = $"Picked Up {collectibleController.GetItem().name}";
                    pickUpPrompt.color = Color.green;
                }

                else
                {
                    pickUpPrompt.text = "Cannot Add Item. Not Enough Space";
                    pickUpPrompt.color = Color.red;                   
                }

                StartCoroutine(ShowPrompt(pickUpPrompt));
            }

            if (collision.gameObject.GetComponent<Door>())
            {
                _currentDoor = collision.gameObject.GetComponent<Door>();
                _atDoor = true;
            }

            //If player on wall jump wall
            if (collision.gameObject.CompareTag("Wall Jump"))
            {
                _wallJumping = true;
                playerAnimator.SetBool("WallJump", true);
                Debug.Log("On wall");
                playerAnimator.Play("PlayerWallJump");
            }

            if (collision.gameObject.CompareTag("KillBox"))
            {
                ReduceHealth(maxHealth);
                Debug.Log("player hit killBox");
            }
            
        }
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsAlive())
        {
            if (collision.gameObject.GetComponent<Door>())
            {
                _currentDoor = null;
                _atDoor = false;
            }

            //If jumped off wall
            if (collision.gameObject.CompareTag("Wall Jump"))
            {
                _wallJumping = false;
                playerAnimator.SetBool("WallJump", false);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsAlive())
        {
            if (collision.gameObject.GetComponent<Checkpoint>())//Restores player health to full when at checkpoint
            {
                Heal(maxHealth);                
            }            
        }        
    }

}
