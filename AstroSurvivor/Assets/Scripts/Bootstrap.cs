using AstroSurvivor;
using AstroSurvivor.UI;
using UnityEngine;

public class Bootstrap : MonoBehaviour {

    public PlayerContext Context { get; private set; }
    public PlayerStats Stats;
    public UpgradeUIController UpgradeUI;

    void Awake()
    {
        Context = new PlayerContext() {
            Stats = Stats,
            ModuleController = new PlayerModuleController(transform),
            WeaponController = new PlayerWeaponController()
        };

        PlayerBuildState buildState = BuildSaveSystem.Load();

        UpgradeUI.Initialize(Context, buildState);
    }
}
