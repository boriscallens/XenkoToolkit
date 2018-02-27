using SiliconStudio.Xenko.Engine;

namespace XenkoToolkit.Demo
{
    class XenkoToolkitDemoApp
    {
        static void Main()
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
