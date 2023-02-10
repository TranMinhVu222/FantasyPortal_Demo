using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Trajectory: MonoBehaviour
{
    public float power = 5f;
    public bool completeCastSpell;
    
    LineRenderer lr;
    Rigidbody2D rb;
    Vector2 startDragPos;
    private Energy enegryPortal;
    public PoolEnergy poolEnergy;
    public Vector3 energyPos;
    public GameObject UI,cancelButton;
    public CancelSkill cancelSkill;
    public Image manaBar;
    public int manaTotal;
    public AudioManager audioManager;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        manaBar.fillAmount = 1f;
        manaTotal = PlayerPrefs.GetInt("Total Mana");
        currentMana = manaTotal;
        completeCastSpell = false;
    }

    private float currentMana;
    void Update()
    {
        if (UI.GetComponent<IsTouchUI>().checkTouch == false && currentMana > 0f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                energyPos = new Vector3(0, 0.3f, 0);
                enegryPortal = poolEnergy.GetEnergy();
                enegryPortal.transform.position = transform.position + energyPos;
                UI.GetComponent<IsTouchUI>().countTouchUI = 2;
                cancelSkill.checkCancel = false;
                cancelButton.gameObject.SetActive(true);
            }

            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                
                energyPos = new Vector3(0, 0.3f, 0);
                enegryPortal = poolEnergy.GetEnergy();
                enegryPortal.transform.position = transform.position + energyPos;
                lr.enabled = true;

                Vector2 endDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 _velocity = (endDragPos - startDragPos) * power;

                Vector2[] trajectory = Plot(rb, (Vector2)enegryPortal.transform.position, _velocity, 500);

                lr.positionCount = trajectory.Length;

                Vector3[] positions = new Vector3[trajectory.Length];

                for (int i = 0; i < trajectory.Length; i++)
                {
                    positions[i] = trajectory[i];
                }
                cancelButton.gameObject.SetActive(true);
                lr.SetPositions(positions);
            }
            else
            {
                lr.enabled = false;
            }

            if (!lr.enabled)
            {
                cancelButton.SetActive(false);
            }

            if (Input.GetMouseButtonUp(0) && UI.GetComponent<IsTouchUI>().countTouchUI == 2 && !cancelSkill.checkCancel)
            {
                completeCastSpell = true;
                audioManager.PlaySFX("CastingSpell");
                enegryPortal.gameObject.SetActive(true);
                cancelButton.SetActive(false);
                Vector2 endDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 _velocity = (endDragPos - startDragPos) * power;
                if (_velocity.x <= 0.2f && _velocity.y <= 0.2f)
                {
                    if (gameObject.transform.rotation.y != 0)
                    {
                        _velocity.x = -2f;
                    }
                    else
                    {
                        _velocity.x = 2f;
                    }
                }
                enegryPortal.GetComponent<Rigidbody2D>().velocity = _velocity;
                currentMana -= 1f;
                manaBar.fillAmount = currentMana / manaTotal;
            }
        }
    }
    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        Vector2[] results = new Vector2[steps];

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * 1f * timestep * timestep;
        
        float drag = 1f;
        Vector2 moveStep = velocity * timestep;
        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }
        return results;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("MagicShard") && manaBar.fillAmount < 1f)
        {
            currentMana += 1 * PlayerPrefs.GetInt("Used booster");
            manaBar.fillAmount = currentMana / manaTotal;
        }
    }
}