using TMPro;
using UnityEngine;
using PlanZ.Events;
using PlanZ.Healing.Events;

namespace PlanZ.UI
{
    public class MedkitCountHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countLabel;

        private void OnEnable() => EventBus.Subscribe<MedkitInventoryChangedEvent>(HandleChanged);

        private void OnDisable() => EventBus.Unsubscribe<MedkitInventoryChangedEvent>(HandleChanged);

        private void HandleChanged(MedkitInventoryChangedEvent evt)
        {
            if (countLabel == null) return;
            countLabel.text = $"0{evt.Current}";
        }
    }
}
