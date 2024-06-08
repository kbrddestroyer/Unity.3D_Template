using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField, Range(0f, 10f)] private float fWalkSpeed;
    [SerializeField, Range(0f, 10f)] private float fRunSpeed;
    [SerializeField, Range(0f, 10f)] private float fMouseSensBase;
    [SerializeField, Range(0f, 10f)] private float fMouseSensCombat;
    [SerializeField, Range(0f, 10f)] private float fHealAmount;
    [Header("Camera")]
    [SerializeField, Range(0f, 90f)] private float fMinRotation;
    [SerializeField, Range(0f, 90f)] private float fMaxRotation;
    [Header("Combat")]
    [SerializeField, Range(0f, 1f)] private float fPunchDistance;
    [SerializeField, Range(0f, 100f)] private float fMaxHp;
    [SerializeField, Range(1f, 10f)] private float fAttackPowerMul;
    [SerializeField, Range(1f, 10f)] private float fAttackPower;
    [SerializeField] private int iRevivesCount;
    [Header("Required Objects")]
    [SerializeField] private Transform tCameraAttachPoint;
    [SerializeField] private Transform tRaycastPoint;
    [SerializeField] private TMP_Text tmpCounter;
    [SerializeField] private TMP_Text reviveCounter;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Animator damageFX;

    private bool bInCombat = false;
    private bool bSprinting = false;
    private bool bImmune = false;
    private bool bCanPunch = true;
    private int iCollectables = 0;
    private float fHp = 0;


    public int Collectables
    {
        get => iCollectables;
        set
        {
            iCollectables = value;
            tmpCounter.text = $"Collected: {value}";
        }
    }

    private LayerMask mask;

    private bool InCombat
    {
        get => bInCombat;
        set
        {
            if (value != bInCombat) animator.SetBool("in_combat", value);
            bInCombat = value;
        }
    }

    private bool IsSprinting
    {
        get => bSprinting;
        set
        {
            if (value != bSprinting) animator.SetBool("running", value);
            bSprinting = value;
        }
    }

    private Animator animator;

    private float fMouseSens
    {
        get => (bInCombat) ? fMouseSensCombat : fMouseSensBase;
    }

    private float fSpeed
    {
        get => (bSprinting || bImmune) ? fRunSpeed : fWalkSpeed;
    }

    public float HP
    {
        get => fHp;
        set
        {
            if (bImmune)
                return;

            if (value < fHp && value > 0)
            {
                damageFX.SetTrigger("damage");
                animator.SetTrigger("take_damage");
            }
            
            bCanPunch = true;
            fHp = value;
            hpSlider.value = Mathf.Clamp(fHp / fMaxHp, 0, 1);
            if (fHp <= 0)
            {
                if (iRevivesCount > 0)
                    StartRevive();
                else 
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void ExitCombatAnim()
    {
        bCanPunch = true;
    }

    public void StartRevive()
    {
        damageFX.SetTrigger("death");
    }

    public void PlayRevive()
    {
        iRevivesCount--;
        reviveCounter.text = $"Revive count: {iRevivesCount}";
        HP = fMaxHp;
        fAttackPower *= fAttackPowerMul;
    }

    public void Damage()
    {
        RaycastHit hit;
        if (Physics.Raycast(tRaycastPoint.position, transform.forward, out hit, fPunchDistance, mask))
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            
            if (enemy != null)
            {
                enemy.HP -= fAttackPower;
            }
        }
    }

    private void Punch()
    {
        if (bCanPunch)
        {
            animator.SetInteger("combat_anim", Random.Range(0, 2));
            animator.SetTrigger("combat");
            bCanPunch = false;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        HP = fMaxHp;
        animator = GetComponent<Animator>();
        mask = LayerMask.GetMask("Enemy");

        reviveCounter.text = $"Revive count: {iRevivesCount}";
    }

    private void OnTriggerEnter(Collider other)
    {
        ICollectable collectable = other.GetComponent<ICollectable>();
        if (collectable != null) collectable.Collect(this);
    }

    public void ExitDash()
    {
        bImmune = false;
    }

    private void Update()
    {
        IsSprinting = Input.GetKey(KeyCode.LeftShift);
        InCombat = Input.GetKey(KeyCode.Mouse1);
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Punch();

        float fAxisX = Input.GetAxis("Horizontal");
        float fAxisY = Input.GetAxis("Vertical");

        animator.SetFloat("axisX", fAxisX);
        animator.SetFloat("axisY", fAxisY);

        float fMouseOffsetX = Input.GetAxis("Mouse X");
        float fMouseOffsetY = Input.GetAxis("Mouse Y");

        Vector3 vMovement = new Vector3(fAxisX, 0, fAxisY) * fSpeed * Time.deltaTime;
        Vector3 vCameraRotationEulers = new Vector3(-fMouseOffsetY, 0, 0) * fMouseSens;
        Vector3 vPlayerRootRotation = new Vector3(0, fMouseOffsetX, 0) * fMouseSens;

        vCameraRotationEulers.x = Mathf.Clamp(tCameraAttachPoint.rotation.eulerAngles.x + vCameraRotationEulers.x, fMinRotation, fMaxRotation) - tCameraAttachPoint.rotation.eulerAngles.x;

        if (iCollectables > 0 && Input.GetKeyDown(KeyCode.E))
        {
            Collectables--;
            HP += fHealAmount;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && vMovement.magnitude > 0) { 
            bImmune = true;
            animator.SetTrigger("dash");
        }

        if (bCanPunch)
        {
            transform.Translate(vMovement);
            transform.Rotate(vPlayerRootRotation);
            tCameraAttachPoint.Rotate(vCameraRotationEulers);
        }
    }
}
