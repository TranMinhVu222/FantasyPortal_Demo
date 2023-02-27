using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.SceneManagement;
//NOTE: gravityScale anh huong toi trajectory
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask platformLayerMask;
    public Behaviour Trajectory;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider2D;
    public Animator animator;
    public GameObject entryPortal, enterPortal, grabButton,pauseMenuScreen, gameOverPanel, winPanel;
    public Transform rayPoint, exitPosition, enterPosition;
    public float rayDistance, boost, speed, horizontalMove, jumpSpeed, deathEffect = 20f;
    private int layerIndex, magicShardes;
    public bool checkGrab, checkGround, checkWin, dead, moveLeft, moveRight, pointingButton, pressKey, pressSpace, completeWin, deathFTUE, checkColliderObj;
    public Level unlockLevel;
    public string nameScene, sceneNextLevel, SceneGameToLoadAB;
    public AudioManager audioManager;
    private string[] scene;
    [HideInInspector] public Vector3 pos
    {
        get { return transform.position;  }
    }
    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
        checkGrab = false;
        moveLeft = false;
        moveRight = false;
        gameOverPanel.SetActive(false);
        // noCastSpell.SetActive(false);
    }
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        layerIndex = LayerMask.NameToLayer("Item");
        rb = GetComponent<Rigidbody2D>();
        Time.timeScale = 1;
        dead = false;
        checkWin = false;
        completeWin = false;
        deathFTUE = false;
        checkColliderObj = false;
        // assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "AB/scenes"));
    }

    // Update is called once per frame
    public void Update()
    {
        MovePlayer();
        if (!Mathf.Approximately(0, horizontalMove))
        {
            transform.rotation = horizontalMove < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        if (!checkGround)
        {
            animator.SetBool("isjumping",true);
            animator.SetBool("isWalking",false);
        }
        if (checkGround)
        {
            if(moveRight || moveLeft)
            {
                animator.SetBool("isWalking",true);
            }
            else
            {
                animator.SetBool("isjumping",false);
                animator.SetBool("isWalking",false);
            }
        }
        RaycastHit2D hit = Physics2D.Raycast(rayPoint.position, transform.right , rayDistance,1<<LayerMask.NameToLayer("Item"));
        if (hit.collider != null && hit.collider.gameObject.layer == layerIndex)
        {
            checkGrab = true;
            grabButton.SetActive(true);
            checkColliderObj = true;
        }
        Debug.DrawRay(rayPoint.position,transform.right * rayDistance);
    }

    private void FixedUpdate()
    {
        //CHECK: is ground
        if (IsGround())
        {
            checkGround = true;
        }
        if (IsGround() == false)
        {
            checkGround = false;
        }
        //----------------------
        

        // ------Control by keyboard------
        if (checkGround && Input.GetAxis("Vertical") > 0)
        {
            if (!dead)
            {
                pressSpace = true;
                rb.velocity = Vector2.up * jumpSpeed;
                // checkGround = false;
            }
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            pressSpace = false;
        }
        
        if (Input.GetAxis("Horizontal") < 0)
        {
            if (!dead)
            {
                pressKey = true;
                moveRight = false;
                moveLeft = true;
            }
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            if (!dead)
            {
                pressKey = true;
                moveRight = true;
                moveLeft = false;
            }
        }
        
        if (Input.GetAxis("Horizontal") == 0)
        {
            if (!dead && !pointingButton && !pressSpace) 
            {
                pressKey = false;
                moveLeft = false;
                moveRight = false;
            }
        }
        // -----------------------------
    }
    
    public void UnPointRight()
    {
        if (!pressKey)
        {
            moveRight = false;
            pointingButton = false;
        }
    }
    public void UnPointLeft()
    {
        if (!pressKey)
        {
            moveLeft = false;
            pointingButton = false;
        }
    }
    
    public void PointerLeftButton()
    {
        if (!dead)
        {
            moveLeft = true;
            pointingButton = true;
        }
    }
    public void PointerRightButton()
    {
        if (!dead)
        {
            moveRight = true;
            pointingButton = true;
        }
    }
    public void PressUpButton()
    {
        if(!dead && checkGround && !pressSpace)
        {
            rb.velocity = Vector2.up * jumpSpeed;
            checkGround = false;
        }
    }

    private void MovePlayer()
    {
        if (moveLeft)
        {
            horizontalMove = -speed;
            transform.Translate(new Vector2(speed*Time.deltaTime, rb.velocity.y * Time.deltaTime));
        }
    
        else if (moveRight)
        {
            horizontalMove = speed; 
            transform.Translate(new Vector2(speed*Time.deltaTime, rb.velocity.y * Time.deltaTime));
        }
        
        else
        {
            horizontalMove = 0;
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuScreen.SetActive(true);
        // noCastSpell.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuScreen.SetActive(false);
    }

    public void MenuGame()
    {
        PlayerPrefs.SetInt("Used booster", 1);
        SceneManager.LoadScene("MenuScene");
        PlayerPrefs.SetInt("Completed FTUE",1);
    }
    public void ReplayGame()
    {
        if (PlayerPrefs.GetInt("Completed FTUE") == 1)
        {
            ReadJSON.LoadScene(PlayerPrefs.GetInt("Present Level"));
        }
    }

    public void WinGame()
    {
        if (PlayerPrefs.GetInt("Completed FTUE") == 1)
        {
            if (PlayerPrefs.GetInt("Present Level") == ReadJSON.scene.Length - 1)
            {
                SceneManager.LoadScene("MenuScene");
            }
            else
            {
                Debug.Log("Present Level: " + PlayerPrefs.GetInt("Present Level") + 1) ;
                int nextLevel = PlayerPrefs.GetInt("Present Level") + 1;
                PlayerPrefs.SetInt("Present Level", nextLevel);
                ReadJSON.LoadScene(nextLevel);
            }
        }
    }

    public void GameOver()
    {
        if (PlayerPrefs.GetInt("Completed FTUE") == 0)
        {
            deathFTUE = true;
            SceneManager.LoadScene("FTUE");
        }
        if (PlayerPrefs.GetInt("Completed FTUE") == 1)
        {
            audioManager.PlaySFX("BeKilled");
            PlayerPrefs.SetInt("Used booster",1);
            dead = true;
            boxCollider2D.isTrigger = true;
            rb.velocity = Vector2.up * deathEffect;
            StartCoroutine(delayFall());
            StartCoroutine(delayDeath());   
        }
    }

    private bool IsGround()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, 
            boxCollider2D.bounds.size,0f, Vector2.down,extraHeightText, platformLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x,0),
            Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x,0),
            Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 
            boxCollider2D.bounds.extents.y + extraHeightText),Vector2.right * (boxCollider2D.bounds.extents.x)*2f,rayColor);
        // Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnterPortal") && entryPortal.activeInHierarchy && enterPortal.activeInHierarchy)
        {
            transform.position = exitPosition.position;
            rb.AddForce(exitPosition.localPosition * boost);
        }
        if (other.gameObject.CompareTag("EntryPortal") && entryPortal.activeInHierarchy && enterPortal.activeInHierarchy)
        {
            transform.position = enterPosition.position;
            rb.AddForce(enterPosition.localPosition * boost);
        }

        if (other.gameObject.CompareTag("DeathZone"))
        {
            if (PlayerPrefs.GetInt("Completed FTUE") == 0)
            {
                deathFTUE = true;
                SceneManager.LoadScene("FTUE");
            }
            if (PlayerPrefs.GetInt("Completed FTUE") == 1)
            {
                GameOver();    
            }
        }

        if (other.gameObject.CompareTag("VictoryZone"))
        {
            if (PlayerPrefs.GetInt("Completed FTUE") == 0)
            {
                completeWin = true;
                Time.timeScale = 1;
                audioManager.PlaySFX("Victory");
            }
            if (PlayerPrefs.GetInt("Completed FTUE") == 1)
            {
                checkWin = true;
                Time.timeScale = 0;
                unlockLevel.Pass();
                PlayerPrefs.SetInt("Used booster",1);
                audioManager.PlaySFX("Victory");
                winPanel.SetActive(true);    
                // noCastSpell.SetActive(true);
            }
        }
    }

    IEnumerator delayDeath()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0;
        audioManager.PlaySFX("GameOver");
        gameOverPanel.SetActive(true);
        // noCastSpell.SetActive(true);
    }
    IEnumerator delayFall()
    {
        yield return new WaitForSeconds(0.2f);
        rb.velocity = Vector2.down * 12f;
    }
}

    


