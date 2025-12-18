using UnityEngine;
using UnityEngine.InputSystem;

namespace AstroSurvivor
{
    public class TalentTreeDebugInput : MonoBehaviour
    {
        [SerializeField] private TalentTreeManager talentManager;

        private void Awake()
        {
            if (talentManager == null)
                talentManager = GetComponent<TalentTreeManager>();
        }

        private void Update()
        {
            if (talentManager == null || Keyboard.current == null)
                return;

            var kb = Keyboard.current;

            if (kb.pKey.wasPressedThisFrame)
            {
                talentManager.AddTalentPoints(5);
                Debug.Log("DEBUG: +5 Talent Points");
            }

            if (kb.lKey.wasPressedThisFrame)
            {
                talentManager.LevelUp();
            }

            if (kb.digit1Key.wasPressedThisFrame)
                talentManager.TryUnlockTalent("hp_root");

            if (kb.digit2Key.wasPressedThisFrame)
                talentManager.TryUnlockTalent("damage_1");

            if (kb.digit3Key.wasPressedThisFrame)
                talentManager.TryUnlockTalent("atk_speed_1");

            if (kb.digit4Key.wasPressedThisFrame)
                talentManager.TryUnlockTalent("projectile_1");

            if (kb.digit5Key.wasPressedThisFrame)
                talentManager.TryUnlockTalent("crit_chance_1");

            if (kb.digit6Key.wasPressedThisFrame)
                talentManager.TryUnlockTalent("crit_damage_1");
        }
    }
}
