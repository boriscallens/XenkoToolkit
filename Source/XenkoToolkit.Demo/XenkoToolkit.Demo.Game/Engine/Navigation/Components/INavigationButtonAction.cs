using System.Threading.Tasks;

namespace XenkoToolkit.Demo.Engine.Navigation.Components
{
    public interface INavigationButtonAction
    {
        Task<bool> Handle(ISceneNavigationService navigationService);
    }
}
