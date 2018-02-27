﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Input;
using XenkoToolkit.Engine;
using XenkoToolkit.Mathematics;

namespace XenkoToolkit.Demo
{
    public class EasingDemo : AsyncScript
    {
        public Vector3 FirstPosition { get; set; }

        public float Height { get; set; }

        public Prefab SpherePrefab { get; set; }

        public override async Task Execute()
        {
            var bottom = new Vector3(0, Height, 0);

            var duration = TimeSpan.FromSeconds(2);
            var elapsed = TimeSpan.Zero;

            var easingFunctions = ((EasingFunction[])Enum.GetValues(typeof(EasingFunction))).Reverse().ToList();
            easingFunctions.RemoveAt(0);

            {
                var startPosition = FirstPosition;
                var endPosition = FirstPosition - bottom;

                var sphere = SpherePrefab.InstantiateSingle(FirstPosition);
                Entity.Scene.Entities.Add(sphere);

                Script.AddOverTimeAction(progress =>
                {
                    sphere.Transform.Position = MathUtilEx.Interpolate(startPosition, endPosition, progress,EasingFunction.ElasticEaseOut);   

                }, TimeSpan.FromSeconds(2));
            }
            


            var startPositions = new List<Vector3>();
            var transforms = new List<TransformComponent>();

            for (var i = 0; i < easingFunctions.Count; i++)
            {
                var startPosition = FirstPosition + new Vector3(i + 1, 0, 0);
                var sphere = SpherePrefab.InstantiateSingle(startPosition);

                transforms.Add(sphere.Transform);
                startPositions.Add(startPosition);
                Entity.Scene.Entities.Add(sphere);
            }

            


            while (Game.IsRunning)
            {
                var progress = (float)(elapsed.TotalSeconds / duration.TotalSeconds);

                if (progress > 1.0f)
                    progress = 1.0f;

                DebugText.Print($"Progress = {progress}", new Int2(10));

                for (var i = 0; i < transforms.Count; i++)
                {
                    var t = transforms[i];

                    var easing = easingFunctions[i];

                    t.Position = MathUtilEx.Interpolate(startPositions[i], startPositions[i] - bottom, progress, easing);

                }                

                elapsed += Game.UpdateTime.Elapsed;

                if (Input.IsKeyPressed(Keys.Space))
                {
                    //reset
                    elapsed = TimeSpan.Zero;
                }

                await Script.NextFrame();
            }

            
        }

        public override void Cancel()
        {
            base.Cancel();
            DebugText.Update(Game.UpdateTime);
        }
    }
}
