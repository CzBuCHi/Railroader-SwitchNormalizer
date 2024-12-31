using System;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Game.Events;
using HarmonyLib;
using JetBrains.Annotations;
using Railloader;
using SwitchNormalizer.TopRightArea;
using Track;

namespace SwitchNormalizer;

[PublicAPI]
public sealed class SwitchNormalizerPlugin(IModdingContext context, IUIHelper uiHelper) : SingletonPluginBase<SwitchNormalizerPlugin>
{
    private Messenger   _Messenger = null!;
    private Settings    _Settings  = null!;
    private TrackNode[] _Switches  = null!;

    public override void OnEnable() {
        _Messenger = Messenger.Default!;
        _Messenger.Register(this, new Action<MapDidLoadEvent>(OnMapDidLoad));
        _Messenger.Register(this, new Action<MapDidUnloadEvent>(OnMapDidUnload));
    }

    public override void OnDisable() => _Messenger.Unregister(this);

    private void OnMapDidLoad(MapDidLoadEvent @event) {
        TopRightAreaExtension.AddButton(OnButtonClick);
        var settings = context.LoadSettingsData<Settings>("SwitchNormalizer") ?? new Settings();
        _Settings = settings;
        _Switches = Graph.Shared!.Nodes!.Where(Graph.Shared.IsSwitch).ToArray();
    }

    private void OnMapDidUnload(MapDidUnloadEvent obj) {
        context.SaveSettingsData("SwitchNormalizer", _Settings);
    }

    private void OnButtonClick(bool shiftKey) {
        if (shiftKey) {
            _Settings.ThrownSwitches = _Switches.Where(o => o.isThrown).Select(o => o.id).ToArray();
        } else {
            _Switches.Do(node => node.isThrown = _Settings.ThrownSwitches.Contains(node.id));
        }
    }
}
