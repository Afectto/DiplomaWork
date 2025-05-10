using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerSkillPoints : MonoBehaviour
{
    [SerializeField] private PlayerStatsItem addHealth;
    [SerializeField] private PlayerStatsItem addSpeed;
    [SerializeField] private PlayerStatsItem addDamage;
    [SerializeField] private PlayerStatsItem addArmor;
    [SerializeField] private PlayerStatsItem addAttackSpeed;

    [SerializeField] private TextMeshProUGUI skillPointsText;
    [SerializeField] private Button showStatsButton;
    [SerializeField] private GameObject showStats;
    private PlayerStats _playerStats;
    
    [Inject]
    private void Inject(PlayerStats playerStats)
    {
        _playerStats = playerStats;
        var playerSkillPointsData = SaveSystem.Load<PlayerSkillPointsData>();
        foreach (var stats in playerSkillPointsData.PlayerStatsAfterUseSkillPoints)
        {
            for (int i = 0; i < stats.Value; i++)
            {
                _playerStats.AddBuff(stats.Key);
            }
        }
        showStatsButton.onClick.AddListener(() =>
        {
            showStats.SetActive(!showStats.activeSelf);
        });
        InitialButtons();
        UpdateSkillPointsText();
        var codeExecutor = FindObjectOfType<CodeExecutor>();
        codeExecutor.OnTaskComplete += (isAlreadyComplete, count) =>
        {
            if (isAlreadyComplete) return;   
            AddSkillPoints(count);
        };
    }

    private void InitialButtons()
    {
        SetupButton(addArmor, StatsTypeName.Resists);
        SetupButton(addHealth, StatsTypeName.Health);
        SetupButton(addSpeed, StatsTypeName.MovementSpeed);
        SetupButton(addDamage, StatsTypeName.Damage);
        SetupButton(addAttackSpeed, StatsTypeName.AttackSpeed);
    }

    private void SetupButton(PlayerStatsItem button, string statType)
    {
        var playerSkillPointsData = SaveSystem.Load<PlayerSkillPointsData>();
        button.SetText(playerSkillPointsData.GetStatsCount(statType), statType);
        button.OnClick += () =>
        {
            UseSkillPoints(1, statType);
            UpdateSkillPointsText();
            var skillPointsData = SaveSystem.Load<PlayerSkillPointsData>();
            button.SetText(skillPointsData.GetStatsCount(statType), statType);
        };
    }
    
    private void UpdateSkillPointsText()
    {
        var playerSkillPointsData = SaveSystem.Load<PlayerSkillPointsData>();
        skillPointsText.text = playerSkillPointsData.SkillPoints.ToString();
    }
    
    private void AddSkillPoints(int amount)
    {
        var playerSkillPointsData = SaveSystem.Load<PlayerSkillPointsData>();
        playerSkillPointsData.AddSkillPoints(amount);
        UpdateSkillPointsText();
    }
    
    private void UseSkillPoints(int amount, string type)
    {
        var playerSkillPointsData = SaveSystem.Load<PlayerSkillPointsData>();
        if (playerSkillPointsData.SkillPoints >= amount)
        {
            playerSkillPointsData.UseSkillPoints(amount, type);
            _playerStats.AddBuff(type);
            SaveSystem.Save(playerSkillPointsData);
        }
    }
}