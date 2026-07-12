using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;


namespace PlanZ.Combat.Damage
{
    public class Drop : MonoBehaviour
    {
        [SerializeField] private GameObject drop;
        [SerializeField] private bool dropOnDie;
        [SerializeField] private bool dropOnDieWhitDelay;

        [Space]

        public UnityEvent OnDie;
        public UnityEvent OnDieWhitDelay;

        public void CallOnDie()
        {
            if(dropOnDie)
                Instantiate(drop, transform.position, Quaternion.identity);
            OnDie?.Invoke();
        }

        public void CallOnDieWhitDelay()
        {
            if (dropOnDieWhitDelay)
                Instantiate(drop, transform.position, Quaternion.identity);
            Instantiate(drop);
            OnDieWhitDelay?.Invoke();
        }
    }
}