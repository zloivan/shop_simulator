using Sim.Features.PlayerSystem.Concrete;

namespace Sim.Features.PlayerSystem.Base
{
    public interface IPlayerComponent
    {
        /// <summary>
        /// Инициализирует компонент, устанавливая ссылку на фасад
        /// </summary>
        /// <param name="facade">Фасад игрока</param>
        void Initialize(PlayerFacade facade);
    }
}