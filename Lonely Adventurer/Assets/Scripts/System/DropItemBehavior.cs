using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zycalipse.Managers;
using Zycalipse.Systems;

namespace Zycalipse.Systems
{
    public class DropItemBehavior : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider boxCollider;
        [SerializeField]
        private Rigidbody rigid;
        public EnemyDropType type;
        [SerializeField]
        private int healAmount;
        public bool TimeToCollect { get; set; }
        public bool ForceToCollect { get; set; }
        private float speed;
        private bool CanBeCollectedByPlayerAttack;
        private bool benCollected = false;

        async void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            ForceToCollect = TimeToCollect = CanBeCollectedByPlayerAttack = false;
            LevelManager.Instance.AddDropToList(this);

            if (type == EnemyDropType.Exp && rigid != null)
            {
                int rand = Random.Range(1, 10);
                rand += (int)Time.unscaledTime * 1000;
                rand %= 4;
                Vector3 dir = Vector3.zero;

                if (rand == 3)
                    dir = Vector3.left;
                else if (rand == 2)
                    dir = Vector3.right;
                else if (rand == 1)
                    dir = Vector3.back;
                else
                    dir = Vector3.forward;

                rand++;
                rand *= 2;
                speed = 30f + rand;

                rand = GameManager.Instance.PrecentageRandomizer();
                rand %= 5;
                rand++;

                rigid.AddForce(Vector3.up, ForceMode.Impulse);
                await Task.Delay(50);

                rigid.AddForce(dir * rand, ForceMode.Impulse);
                await Task.Delay(500);

                if(rigid != null)
                    rigid.constraints = RigidbodyConstraints.FreezePosition;
            }
        }

        async void Update()
        {
            if (benCollected)
                return;

            if (TimeToCollect && type == EnemyDropType.Exp)
            {
                if (Vector3.Distance(transform.position, CombatSystem.Instance.playerManager.transform.position) < 0.1f)
                {
                    benCollected = true;
                    await CombatSystem.Instance.playerManager.playerData.PlayerGainExp(0.5f);
                    LevelManager.Instance.RemoveDrop(this);
                    return;
                }
                transform.position = Vector3.MoveTowards(transform.position, CombatSystem.Instance.playerManager.transform.position, speed * Time.deltaTime);
            }
            else if (TimeToCollect)
            {
                if (this.CanBeCollectedByPlayerAttack)
                {
                    benCollected = true;
                    await CombatSystem.Instance.playerManager.Heal(healAmount);
                    LevelManager.Instance.RemoveDrop(this);
                    Destroy(gameObject);
                }
                else if (ForceToCollect)
                {
                    benCollected = true;
                    LevelManager.Instance.RemoveDrop(this);
                    Destroy(gameObject);
                }
            }
        }

        private async void OnTriggerEnter(Collider collision)
        {
            if (type == EnemyDropType.Exp)
                return;

            if (collision.CompareTag("PlayerAttackArea"))
            {
                this.CanBeCollectedByPlayerAttack = true;
            }
        }
        private void OnTriggerStay(Collider collision)
        {
            if (type == EnemyDropType.Exp)
                return;

            if (collision.CompareTag("PlayerAttackArea"))
            {
                this.CanBeCollectedByPlayerAttack = true;
            }
        }

        private async void OnTriggerExit(Collider collision)
        {
            if (type == EnemyDropType.Exp)
                return;

            if (collision.CompareTag("PlayerAttackArea"))
            {
                this.CanBeCollectedByPlayerAttack = false;
            }
        }
    }
}
