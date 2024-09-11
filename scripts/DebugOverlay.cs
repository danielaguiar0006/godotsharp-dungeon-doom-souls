using Godot;
using static InputActions;
using Game.StatsManager;


// The Debug Overlay Shows debug information related to the player's state, stats, and other information
public partial class DebugOverlay : CanvasLayer
{
    [Export]
    public bool m_ShowDebugInfo { get; private set; } = false;

    [Export]
    protected Label m_CurrentStateDebugLabel;
    [Export]
    protected Label m_IsOnFloorDebugLabel;
    [Export]
    protected Label m_StatsDebugLabel;

    private Player m_Owner;
    private const float STAT_CHANGE_AMOUNT = 0.1f;
    private const float HEALTH_CHANGE_AMOUNT = 1.0f;


    public override void _Ready()
    {
        m_Owner = GetParent<Player>();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed(s_ToggleDebugInfo)) { m_ShowDebugInfo = !m_ShowDebugInfo; }
        HandleDebugInput(@event);
    }

    public override void _Process(double delta)
    {
        if (m_ShowDebugInfo) { ShowDebugInfo(); }
        else { HideDebugInfo(); }

    }

    private void ShowDebugInfo()
    {
        // Show the debug info labels
        m_CurrentStateDebugLabel.Visible = true;
        m_IsOnFloorDebugLabel.Visible = true;
        m_StatsDebugLabel.Visible = true;

        // Information about the current state on the screen
        m_CurrentStateDebugLabel.Text = "Current State: " + m_Owner.m_CurrentPlayerState.GetType().Name;

        UpdateFloorDebugLabel();
        m_StatsDebugLabel.Text = GetPlayerStatsInfo();

        // Show the player's current velocity
        // TODO: this.GetNode<Label>("VelocityDebugInfo").Text = "Velocity: " + m_CurrentVelocity;
        // Show the player's current movement speed factor
        // TODO: this.GetNode<Label>("MovementSpeedFactorDebugInfo").Text = "Movement Speed Factor: " + m_CurrentMovementSpeedFactor;
    }

    private void HideDebugInfo()
    {
        m_CurrentStateDebugLabel.Visible = false;
        m_IsOnFloorDebugLabel.Visible = false;
        m_StatsDebugLabel.Visible = false;
    }

    private void UpdateFloorDebugLabel()
    {
        if (m_Owner.IsOnFloor())
        {
            m_IsOnFloorDebugLabel.AddThemeColorOverride("font_color", new Color(0, 1, 0));
            m_IsOnFloorDebugLabel.Text = "On Floor: True";
        }
        else
        {
            m_IsOnFloorDebugLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0));
            m_IsOnFloorDebugLabel.Text = "On Floor: False";
        }
    }

    private string GetPlayerStatsInfo()
    {
        string statsInfo = "PLAYER STATS:\n";
        foreach (var stat in m_Owner.m_MobStats.m_BaseStatTypeToCurrentValue)
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        foreach (var stat in m_Owner.m_MobStats.m_AttributeTypeToCurrentLevel)
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        foreach (var stat in m_Owner.m_MobStats.m_SpecialStatTypeToAmountFactor)
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        return statsInfo;
    }

    private void HandleDebugInput(InputEvent @event)
    {
        if (Input.IsPhysicalKeyPressed(Key.Up))
        {
            m_Owner.m_MobStats.SetSpecialStatAmountFactor(
                    SpecialStatType.MovementSpeedFactor,
                    m_Owner.m_MobStats.m_SpecialStatTypeToAmountFactor
                    [SpecialStatType.MovementSpeedFactor] + STAT_CHANGE_AMOUNT);
        }
        else if (Input.IsPhysicalKeyPressed(Key.Down))
        {
            m_Owner.m_MobStats.SetSpecialStatAmountFactor(
                    SpecialStatType.MovementSpeedFactor,
                    m_Owner.m_MobStats.m_SpecialStatTypeToAmountFactor
                    [SpecialStatType.MovementSpeedFactor] - STAT_CHANGE_AMOUNT);
        }
        else if (Input.IsPhysicalKeyPressed(Key.Left))
        {
            // m_Owner.m_MobStats.SetCurrentBaseStatValue(
            //         BaseStatType.Health,
            //         m_Owner.m_MobStats.m_BaseStatTypeToCurrentValue
            //         [BaseStatType.Health] - HEALTH_CHANGE_AMOUNT);

            m_Owner.Die();
        }
    }
}
