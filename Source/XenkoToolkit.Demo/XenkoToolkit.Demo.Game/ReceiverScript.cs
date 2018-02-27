using System;
using System.Collections.Generic;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Core.MicroThreading;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Events;
using XenkoToolkit.Engine;

namespace XenkoToolkit.Demo
{
    public class ReceiverScript : StartupScript
    {
        private readonly EventReceiver<Vector2> _eventTwoReciever
            = new EventReceiver<Vector2>(SenderScript.EventTwo);

        private List<MicroThread> _tasks;

        public override void Start()
        {
            //Keep a list of tasks to stop on cancel
            _tasks = new List<MicroThread>
            {
                //Directly using EventKey so you don't have to declare EventReciever:
                Script.AddOnEventAction(SenderScript.EventOne, HandleOne),
                //Using an EventReciever:
                Script.AddOnEventAction(_eventTwoReciever, HandleTwo),

                Script.AddAction(DelayedAction, TimeSpan.FromSeconds(2)),

                Script.AddAction(DelayRepeatAction, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            };
        }

        private void HandleOne(Vector2 position)
        {
            DebugText.Print($"Even One - {position}", new Int2(10, 10));
        }

        private void HandleTwo(Vector2 position)
        {
            DebugText.Print($"Even Two - {position}", new Int2(10, 20));
        }

        private void DelayedAction()
        {
            DebugText.Print("Delay", new Int2(10, 30));
        }

        private void DelayRepeatAction()
        {
            DebugText.Print("Delay Repeat", new Int2(10, 40));
        }

        public override void Cancel()
        {
            base.Cancel();
            _tasks.CancelAll();
            
            DebugText.Update(Game.UpdateTime);
        }

        
    }
}
