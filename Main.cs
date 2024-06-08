using CounterStrikeSharp.API.Core.Capabilities;
using Hooks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.Logging;


namespace EndRoundHooks;
public class EndRoundHooks : BasePlugin
{

    public override string ModuleAuthor => "Nick Fox";
    public override string ModuleName => "EndRound Hooks";
    public override string ModuleVersion => "1.0";

    private bool round_end;

    private IHooksApi? _hooks;
    private PluginCapability<IHooksApi> PluginHooks { get; } = new("hooks:nfcore");

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _hooks = PluginHooks.Get();

        if (_hooks == null) return;


        _hooks.AddHook((playerinfo) => {

            if (round_end)
                playerinfo.Set(HookState.Enabled);
        });

        RegisterEventHandler<EventRoundEnd>((@event, info) =>
        {            
            round_end = true;
            return HookResult.Continue;
        });


        RegisterEventHandler<EventRoundStart>((@event, info) =>
        {
            round_end = false;
            return HookResult.Continue;
        });
    }



}
