using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : PlayerComponent
{
    [Header("Squad Indicator")]
    public TMP_Text defendersCount;
    public TMP_Text healersCount;
    public TMP_Text attackersCount;

    [Header("Weapon")]
    public TMP_Text bulletCount;

    [Header("Health")]
    public HealthBar healthBar;

    public RawImage heartbeat;
    bool heartbeatEffectStarted = false;
    bool heartbeatEffectStopped = false;
    public float heartbeatLow;
    public float heartbeatHigh;
    bool heartbeatAscending = false;
    float currentHeartBeat;

    [Header("Menu")]
    public Menu menu;

    protected override void Awake()
    {
        base.Awake();
        playerBehaviour.OnTakeDamage.AddListener(CallbackFuncUpdateHealth);
        playerBehaviour.OnHealDamage.AddListener(CallbackFuncUpdateHealth);

        playerBehaviour.OnEntityKilled.AddListener(OpenDeathMenu);

        menu.playerBehaviour = playerBehaviour;
    }

    private void Start()
    {
        playerBehaviour.squad.OnSquadEntityKilled.AddListener(UpdateSquadCount);
        UpdateSquadCount();
        UpdateWeaponInfo();
    }

    private void CallbackFuncUpdateHealth(Entity enity)
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.UpdateBar(playerBehaviour.HealthRatio);

        if (!heartbeatEffectStarted && playerBehaviour.HealthRatio < 0.3f)
        {
            heartbeatEffectStopped = false;
            HeartBeatEffect();
        }
        else if (playerBehaviour.HealthRatio > 0.3f && heartbeatEffectStarted)
        {
            heartbeatEffectStopped = true;
            heartbeatEffectStarted = false;
        }
    }

    private void Update()
    {
        UpdateWeaponInfo();
    }

    void UpdateSquadCount()
    {
        defendersCount.text = playerBehaviour.squad.GetDefenders.Count.ToString();
        healersCount.text = playerBehaviour.squad.GetHealers.Count.ToString();
        attackersCount.text = playerBehaviour.squad.GetAttackers.Count.ToString();
    }

    void UpdateWeaponInfo()
    {
        if (playerBehaviour.weapon == null)
            return;
        bulletCount.text = playerBehaviour.weapon.currentMag.ToString() + "/" + playerBehaviour.weapon.weaponInfo.magCapacity;

        if (playerBehaviour.weapon.currentMag <= 0)
        {
            bulletCount.color = Color.gray;
        }
        else
            bulletCount.color = Color.white;
    }

    void HeartBeatEffect()
    {
        StartCoroutine("HeartBeatEffectCoroutine");
        heartbeatEffectStarted = true;
    }

    IEnumerator HeartBeatEffectCoroutine()
    {
        while (!heartbeatEffectStopped)
        {
            if (heartbeatAscending)
            {
                currentHeartBeat += Time.fixedDeltaTime;

                heartbeat.color = new Color(heartbeat.color.r, heartbeat.color.g, heartbeat.color.b, currentHeartBeat);

                if(currentHeartBeat >= heartbeatHigh/100)
                    heartbeatAscending = false;
            }
            else
            {
                currentHeartBeat -= Time.fixedDeltaTime;

                heartbeat.color = new Color(heartbeat.color.r, heartbeat.color.g, heartbeat.color.b, currentHeartBeat);

                if (currentHeartBeat <= heartbeatLow/100)
                    heartbeatAscending = true;
            }

            yield return new WaitForFixedUpdate();
        }
        currentHeartBeat = 0;
        heartbeat.color = new Color(heartbeat.color.r, heartbeat.color.g, heartbeat.color.b, currentHeartBeat);
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            menu.OpenMenu();
        }
    }

    public void OpenDeathMenu(Entity player)
    {
        menu.PlayerDeath();
    }
}
