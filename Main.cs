using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Admin;
using System.Text.Json.Serialization;
using HGR;

namespace EveryoneHGR;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("RoundStart")] public HGRInfo RoundStart { get; set; } = new HGRInfo(1,2,1);
    [JsonPropertyName("RoundEnd")] public HGRInfo RoundEnd { get; set; } = new HGRInfo(-1,-1,-1);
}

public class AdminHGR : BasePlugin, IPluginConfig<PluginConfig>
{
    public PluginConfig Config { get; set; }
    public override string ModuleName => "EveryoneHGR";
    public override string ModuleVersion => "2.0";
    public override string ModuleAuthor => "Nick Fox";

    private IHGRApi? _hgr;
    private PluginCapability<IHGRApi> PluginHooks { get; } = new("hgr:nfcore");
    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
    }


    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _hgr = PluginHooks.Get();

        hgrCount = new int[3][];
        for (int i = 0; i < 3; i++)
            hgrCount[i] = new int[65];

        if (_hgr == null) return;
        _hgr.AddHook(HGRHook, 100);
    }

    public override void Unload(bool hotReload)
    {
        _hgr.RemHook(HGRHook);
    }


    private int[][] hgrCount; // 0 - Hooks, 1 - Grabs, 2 - Ropes


    private void HGRHook(PlayerHGR info)
    {
        if (info.State() == HGRState.Disabled)
        {
            int i = 0;
            switch (info.Mode())
            {
                case HGRMode.Hook: i = 0; break;
                case HGRMode.Grab: i = 1; break;
                case HGRMode.Rope: i = 2; break;
                default: return;
            }

            if (hgrCount[i][info.Player().Slot] == -1)
                info.Enable();
            else
                if (hgrCount[i][info.Player().Slot] > 0)
            {
                hgrCount[i][info.Player().Slot]--;
                info.Enable();

                if (hgrCount[i][info.Player().Slot] == 0)
                    info.Player().PrintToChat(Localizer["ev_hgr.expired"]);
                else
                    info.Player().PrintToChat(String.Format(Localizer["ev_hgr.use_count"], hgrCount[i][info.Player().Slot]));
            }
        }
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 65; j++)
                hgrCount[i][j] = 0;
        return HookResult.Continue;
    }


    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        OnRoundEvent(true);
        return HookResult.Continue;
    }


    [GameEventHandler]
    public HookResult OnFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        OnRoundEvent(false);
        return HookResult.Continue;
    }

    private void OnRoundEvent(bool endRound)
    {
        HGRInfo info;
        if (endRound)
            info = Config.RoundEnd;
        else
            info = Config.RoundStart;

        foreach (var player in Utilities.GetPlayers())
        {
            var slot = player.Slot;            

            AddHGRValues(slot, 0, info.Hooks);
            AddHGRValues(slot, 1, info.Grabs);
            AddHGRValues(slot, 2, info.Ropes);

        }
    }
    
    

    private void AddHGRValues(int slot, int index, int value)
    {
        if (value == -1)
            hgrCount[index][slot] = -1;
        else if (hgrCount[index][slot] != -1)
            hgrCount[index][slot] += value;
    }

}

