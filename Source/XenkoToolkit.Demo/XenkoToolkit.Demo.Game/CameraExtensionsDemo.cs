﻿using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Events;
using SiliconStudio.Xenko.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenkoToolkit.Engine;
using XenkoToolkit.Mathematics;
using XenkoToolkit.Physics;

namespace XenkoToolkit.Demo
{
    public class CameraExtensionsDemo : SyncScript
    {
        public static readonly EventKey<Entity> TargetAcquired = new EventKey<Entity>(eventName: "TargetAcquired");
        
        private string message;

        public CameraComponent MainCamera { get; set; }

        public Entity Cube { get; set; }

        public override void Start()
        {
            // MainCamera = SceneSystem.GetMainCamera();
        }

        public override void Update()
        {
            Cube?.Transform.Rotate(new Vector3(MathUtil.DegreesToRadians(60) * Game.GetDeltaTime(), 0, 0));


            DebugText.Print($"Screen {MainCamera.WorldToScreen(Entity.Transform.Position)}", new Int2(20, 20));

            DebugText.Print($"ScreenToWorldRaySegment {MainCamera.ScreenToWorldRaySegment(Input.MousePosition)}", new Int2(20, 40));

            if (Input.IsMouseButtonPressed(SiliconStudio.Xenko.Input.MouseButton.Left))
            {
                message = "";


                var ray = MainCamera.ScreenToWorldRaySegment(Input.MousePosition);

                var hitResult = this.GetSimulation().Raycast(ray);
                if (hitResult.Succeeded)
                {
                    message = hitResult.Collider.Entity.Name;
                   
                    MainCamera.Entity.Transform.LookAt(hitResult.Collider.Entity.Transform);
                    TargetAcquired.Broadcast(hitResult.Collider.Entity);
                }
                else
                {
                    TargetAcquired.Broadcast(null);
                }
                DebugText.Print($"Clicked on {message}", new Int2(20, 60));
            }

            //DebugText.Print($"Main {SceneSystem.GetMainCamera() != null}", new Int2(20, 40));
        }

        //Dirty Hacks
        public override void Cancel()
        {
            base.Cancel();
            DebugText.Update(Game.UpdateTime);
        }
    }
}
