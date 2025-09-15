using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zycalipse.Managers
{
    public class EffectManager : MonoBehaviour
    {
        private static EffectManager instance;
        public static EffectManager Instance
        {
            get
            {
                return instance;
            }
        }

        public GameObject FireworkEff;
        public GameObject ExplosionEff;
        public GameObject ElectricEffect;
        public GameObject EnemyHitEff;
        public GameObject PlayerHitEff;
        public GameObject PlayerHealEff;
        public GameObject PoisonEffect;
        public GameObject PlayerReviveEff;
        public GameObject WalkEff;
        public GameObject BurnEff;
        public GameObject StunEff;
        public GameObject BleedEffect;
        public GameObject FreezeEffect;
        public GameObject LevelUpEffect;

        // Start is called before the first frame update
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
        }

        public async void ActivateEffect(Effect effect, Vector3 position)
        {
            GameObject effectInitiated = null;

            switch (effect)
            {
                case Effect.Firework:
                    effectInitiated = FireworkEff;
                    break;
                case Effect.Burn:
                    effectInitiated = BurnEff;
                    break;
                case Effect.Freeze:
                    effectInitiated = FreezeEffect;
                    break;
                case Effect.Poison:
                    effectInitiated = PoisonEffect;
                    break;
                case Effect.Bleed:
                    effectInitiated = BleedEffect;
                    break;
                case Effect.Blind:
                    effectInitiated = StunEff;
                    break;
                case Effect.Lightning:
                    effectInitiated = ElectricEffect;
                    break;
                case Effect.Stun:
                    effectInitiated = StunEff;
                    break;
                case Effect.EnemyHit:
                    effectInitiated = EnemyHitEff;
                    break;
                case Effect.PlayerHit:
                    effectInitiated = PlayerHitEff;
                    break;
                case Effect.PlayerRevive:
                    effectInitiated = PlayerReviveEff;
                    break;
                case Effect.PlayerWalk:
                    effectInitiated = WalkEff;
                    break;
                case Effect.Explosion:
                    effectInitiated = ExplosionEff;
                    break;
                case Effect.Heal:
                    effectInitiated = PlayerHealEff;
                    break;
                case Effect.LevelUp:
                    effectInitiated = LevelUpEffect;
                    break;
            }

            if(effectInitiated != null)
            {
                var obj = Instantiate(effectInitiated, position, Quaternion.identity);
                if(effect  != Effect.PlayerWalk)
                    obj.GetComponent<ParticleSystem>().startSize *= 5;
            }
        }
    }
}
