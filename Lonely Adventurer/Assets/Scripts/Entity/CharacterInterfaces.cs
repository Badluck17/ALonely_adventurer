using System.Threading.Tasks;

namespace Zycalipse.Entitys
{
    public interface ICharacterBattleActions
    {
        public Task TakeDamage(bool knockback, int damage, bool fromPoison = false, bool fromBurn = false, bool fromBleed = false);
        public Task Attacking(ICharacterBattleActions target);
        public Task Defending();
        public Task RefreshUI();
    }
}
